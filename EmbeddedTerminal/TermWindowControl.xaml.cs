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



            var client = new HubClient();
            var clientStream = client.RequestServiceAsync("wwt.pty").Result;

            var helper = new ScriptHelper(clientStream, this.terminalView, Dispatcher.CurrentDispatcher);
            this.terminalView.ScriptingObject = helper;
            string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rootPath = Path.Combine(extensionDirectory, "view\\default.html").Replace("\\\\", "\\");

            this.terminalView.Navigate(new Uri(rootPath));
        }
    }

    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public class ScriptHelper
    {
        private JsonRpc rpc;
        private BetterBrowser browser;
        private Dispatcher ui;
        public ScriptHelper(Stream serviceStream, BetterBrowser browser, Dispatcher ui)
        {
            this.rpc = JsonRpc.Attach(serviceStream, this);
            this.browser = browser;
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

        public void TermData(string data)
        {
            rpc.InvokeAsync("termData", data);
        }

        public void InitTerm(int cols, int rows, string directory)
        {
            rpc.InvokeAsync("initTerm", cols, rows, directory);
        }

        public void ResizeTerm(int cols, int rows)
        {
            rpc.InvokeAsync("resizeTerm", cols, rows);
        }

        public void PtyData(string data)
        {
            ui.InvokeAsync(() =>
            {
                this.browser.Invoke("invokeTerm", "ptyData", data);
            });
        }

        public void ReInitTerm(int? code)
        {
            ui.InvokeAsync(() =>
            {
                this.browser.Invoke("invokeTerm", "reInitTerm", code);
            });
        }
    }
}