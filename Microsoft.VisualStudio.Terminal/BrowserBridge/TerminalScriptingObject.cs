using StreamJsonRpc;
using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;

namespace EmbeddedTerminal
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public class TerminalScriptingObject : ITerminalScriptingObject
    {
        private readonly TermWindowPackage package;
        private readonly JsonRpc ptyService;
        private readonly SolutionUtils solutionUtils;

        internal TerminalScriptingObject(TermWindowPackage package, JsonRpc ptyService, SolutionUtils solutionUtils)
        {
            this.package = package;
            this.ptyService = ptyService;
            this.solutionUtils = solutionUtils;
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
            this.ptyService.InvokeAsync("initTerm", this.package.OptionTerminal.ToString(), cols, rows, directory, this.package.OptionStartupArgument);
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
