namespace EmbeddedTerminal
{
    using Microsoft.VisualStudio.Shell;
    using System.ComponentModel.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using EnvDTE;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Runtime.InteropServices;
    using Microsoft.ServiceHub.Client;
    using StreamJsonRpc;
    using System.Security.Permissions;
    using System.Windows.Threading;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Interaction logic for TermWindowControl.
    /// </summary>
    public partial class TermWindowControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindowControl"/> class.
        /// </summary>
        public TermWindowControl()
        {
            this.InitializeComponent();

            this.Focusable = true;
            this.GotFocus += TermWindowControl_GotFocus;

            var client = new HubClient();
            var clientStream = client.RequestServiceAsync("wwt.pty").Result;

            var helper = new ScriptHelper(this, clientStream, Dispatcher.CurrentDispatcher);
            this.terminalView.ScriptingObject = helper;
            string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rootPath = Path.Combine(extensionDirectory, "view\\default.html").Replace("\\\\", "\\");

            this.terminalView.Navigate(new Uri(rootPath));
        }

        private void TermWindowControl_GotFocus(object sender, RoutedEventArgs e)
        {
            // We call focus here because if we don't, the next call will prevent the toolbar from turning blue.
            // No functionality is lost when this happens but it is not consistent with VS design conventions.
            this.Focus();
            this.terminalView.Invoke("invokeTerm", "focus");
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptHelper
    {
        #region regexconsts
        /* Regex's originally from VSCode.
         * VSCode is licensed under the MIT license, original sources and license.txt can be found here:
         * https://github.com/Microsoft/vscode
         */
        const string winDrivePrefix = "[a-zA-Z]:";
        const string winPathPrefix = "(" + winDrivePrefix + "|\\.\\.?|\\~)";
        const string winPathSeparatorClause = "(\\\\|\\/)";
        const string winExcludedPathCharactersClause = "[^\\0<>\\?\\|\\/\\s!$`&*()\\[\\]+\'\":;]";
        /** A regex that matches paths in the form c:\foo, ~\foo, .\foo, ..\foo, foo\bar */
        const string winLocalLinkClause = "((" + winPathPrefix + "|(" + winExcludedPathCharactersClause + ")+)?(" + winPathSeparatorClause + "(" + winExcludedPathCharactersClause + ")+)+)";

        /** As xterm reads from DOM, space in that case is nonbreaking char ASCII code - 160,
        replacing space with nonBreakningSpace or space ASCII code - 32. */
        private static string lineAndColumnClause = String.Join("|", new string[] {
            "((\\S*) on line ((\\d+)(, column (\\d+))?))", // (file path) on line 8, column 13
            "((\\S*):line ((\\d+)(, column (\\d+))?))", // (file path):line 8, column 13
            "(([^\\s\\(\\)]*)(\\s?[\\(\\[](\\d+)(,\\s?(\\d+))?)[\\)\\]])", // (file path)(45), (file path) (45), (file path)(45,18), (file path) (45,18), (file path)(45, 18), (file path) (45, 18), also with []
            "(([^:\\s\\(\\)<>\'\"\\[\\]]*)(:(\\d+))?(:(\\d+))?)" // (file path):336, (file path):336:9
        }).Replace(" ", "[" + "\u00A0" + "]");

        private static Regex localLinkPattern = new Regex(winLocalLinkClause + "(" + lineAndColumnClause + ")");
        // Changing any regex may effect this value, hence changes this as well if required.
        const int winLineAndColumnMatchIndex = 12;
        const int unixLineAndColumnMatchIndex = 15;

        // Each line and column clause have 6 groups (ie no. of expressions in round brackets)
        const int lineAndColumnClauseGroupCount = 6;
        #endregion

        private JsonRpc rpc;
        private TermWindowControl uiControl;
        private Dispatcher ui;
        public ScriptHelper(TermWindowControl uiControl, Stream serviceStream, Dispatcher ui)
        {
            this.rpc = JsonRpc.Attach(serviceStream, this);
            this.uiControl = uiControl;
            this.ui = ui;
        }

        public string GetSolutionDir()
        {
            IVsSolution solutionService = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
            solutionService.GetSolutionInfo(out string solutionDir, out _, out _);
            if (solutionDir == null)
            {
                solutionDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            return solutionDir;
        }

        public void CopyStringToClipboard(string stringToCopy)
        {
            stringToCopy = stringToCopy == null ? "" : stringToCopy;
            Clipboard.SetText(stringToCopy);
        }

        public string GetClipboard()
        {
            return Clipboard.GetText();
        }

        #region terminal controllers
        public void TermData(string data)
        {
            rpc.InvokeAsync("termData", data);
        }

        public void InitTerm(int cols, int rows, string directory)
        {
            rpc.InvokeAsync("initTerm", TermWindowPackage.Instance?.OptionTerminal.ToString(), cols, rows, directory);
        }

        public void ResizeTerm(int cols, int rows)
        {
            rpc.InvokeAsync("resizeTerm", cols, rows);
        }

        public void PtyData(string data)
        {
            ui.InvokeAsync(() =>
            {
                this.uiControl.terminalView.Invoke("invokeTerm", "ptyData", data);
            });
        }

        public void ReInitTerm(int? code)
        {
            ui.InvokeAsync(() =>
            {
                this.uiControl.terminalView.Invoke("invokeTerm", "reInitTerm", code);
            });
        }
        #endregion

        #region error link helpers
        public string GetLinkRegex()
        {
            return localLinkPattern.ToString();
        }

        public void HandleLocalLink(string link)
        {
            var url = this.ExtractLinkUrl(link);
            var path = this.PreprocessPath(url);

            var lcinfo = this.ExtractLineColumnInfo(link);

            EnvDTE80.DTE2 dte2;
            dte2 = (EnvDTE80.DTE2)Marshal.GetActiveObject("VisualStudio.DTE");
            dte2.MainWindow.Activate();
            EnvDTE.Window w = dte2.ItemOperations.OpenFile(path, EnvDTE.Constants.vsViewKindTextView);
            ((TextSelection)dte2.ActiveDocument.Selection).MoveToDisplayColumn(lcinfo.Item1, lcinfo.Item2);
        }

        public bool ValidateLocalLink(string link)
        {
            var url = this.ExtractLinkUrl(link);
            if (url == null)
            {
                return false;
            }

            var path = this.PreprocessPath(url);
            if (link == null)
            {
                return false;
            }

            return File.Exists(path);
        }

        private string PreprocessPath(string path)
        {
            // resolve ~
            if (path[0] == '~')
            {
                var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");

                if (homeDrive == null || homePath == null)
                {
                    return null;
                }
                path = homeDrive + "\\" + homePath + path.Substring(1);
            }

            // resolve relative links
            var regex = new Regex("^[a-zA-Z]:");
            if (!regex.IsMatch(path))
            {
                IVsSolution solutionService = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                solutionService.GetSolutionInfo(out string solutionDir, out _, out _);
                if (solutionDir == null)
                {
                    return null;
                }
                path = Path.Combine(solutionDir, path);
            }

            return path;
        }

        private string ExtractLinkUrl(string link)
        {
            var groups = localLinkPattern.Match(link).Groups;

            if (groups.Count == 0)
            {
                return null;
            }
            return groups[1].ToString();
        }

        private Tuple<int, int> ExtractLineColumnInfo(string link)
        {
            var row = 0;
            var col = 0;
            var groups = localLinkPattern.Match(link).Groups;

            for (var i = 0; i < lineAndColumnClause.Length; i++)
            {
                var lineMatchIndex = winLineAndColumnMatchIndex + (lineAndColumnClauseGroupCount * i);
                var rowNumber = groups[lineMatchIndex].ToString();

                if (rowNumber != "")
                {
                    row = int.Parse(rowNumber);

                    var colNumber = groups[lineMatchIndex + 2].ToString();
                    if (colNumber != "")
                    {
                        col = int.Parse(colNumber);
                    }
                    break;
                }
            }

            return new Tuple<int, int>(row, col);
        }
#endregion
    }
}