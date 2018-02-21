using StreamJsonRpc;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;

namespace Microsoft.VisualStudio.Terminal
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public class TerminalScriptingObject : ITerminalScriptingObject
    {
        private readonly TermWindowPackage package;
        private readonly JsonRpc ptyService;
        private readonly SolutionUtils solutionUtils;
        private readonly string workingDirectory;
        private readonly bool useSolutionDir;
        private readonly string shellPath;
        private readonly string args;

        internal TerminalScriptingObject(
            TermWindowPackage package,
            JsonRpc ptyService,
            SolutionUtils solutionUtils,
            string workingDirectory,
            bool useSolutionDir,
            string shellPath,
            string args)
        {
            this.package = package;
            this.ptyService = ptyService;
            this.solutionUtils = solutionUtils;
            this.workingDirectory = workingDirectory;
            this.useSolutionDir = useSolutionDir;
            this.shellPath = shellPath;
            this.args = args;
        }

        public string GetTheme()
        {
            return TerminalThemer.GetTheme();
        }

        public string GetFontFamily()
        {
            return this.package.OptionFontFamily;
        }

        public int GetFontSize()
        {
            return this.package.OptionFontSize;
        }

        public string GetSolutionDir()
        {
            if (!this.useSolutionDir)
            {
                return this.workingDirectory;
            }

            var solutionDir = this.solutionUtils.GetSolutionDir();
            if (solutionDir == null)
            {
                solutionDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            return solutionDir;
        }

        public void ClosePty()
        {
            this.ptyService.InvokeAsync("closeTerm");
        }

        public void CopyStringToClipboard(string stringToCopy)
        {
            Clipboard.SetText(stringToCopy ?? "");
        }

        public string GetClipboard()
        {
            return Clipboard.GetText();
        }

        public string GetLinkRegex()
        {
            return TerminalRegex.LocalLinkRegex.ToString();
        }

        public void HandleLocalLink(string uri)
        {
            TerminalRegex.HandleLocalLink(uri);
        }

        public void InitPty(int cols, int rows, string directory)
        {
            this.ptyService.InvokeAsync("initTerm", this.shellPath ?? this.package.OptionTerminal.ToString(), cols, rows, directory, this.args ?? this.package.OptionStartupArgument);
        }

        public void ResizePty(int cols, int rows)
        {
            this.ptyService.InvokeAsync("resizeTerm", cols, rows);
        }

        public void TermData(string data)
        {
            this.ptyService.InvokeAsync("termData", data);
        }

        public bool ValidateLocalLink(string link)
        {
            return TerminalRegex.ValidateLocalLink(link);
        }
    }
}
