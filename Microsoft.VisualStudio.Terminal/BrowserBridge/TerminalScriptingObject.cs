using Microsoft.VisualStudio.Shell;
using StreamJsonRpc;
using System;
using System.Collections.Generic;
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
        private readonly IEnumerable<string> args;
        private readonly IDictionary<string, string> env;

        internal TerminalScriptingObject(
            TermWindowPackage package,
            JsonRpc ptyService,
            SolutionUtils solutionUtils,
            string workingDirectory,
            bool useSolutionDir,
            string shellPath,
            IEnumerable<string> args,
            IDictionary<string, string> env)
        {
            this.package = package;
            this.ptyService = ptyService;
            this.solutionUtils = solutionUtils;
            this.workingDirectory = workingDirectory;
            this.useSolutionDir = useSolutionDir;
            this.shellPath = shellPath;
            this.args = args;
            this.env = env;
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
            string configuredShellPath;
            if (this.package.OptionTerminal == DefaultTerminal.Other)
            {
                configuredShellPath = this.package.OptionShellPath;
            }
            else
            {
                configuredShellPath = this.package.OptionTerminal.ToString();
            }

            this.ptyService.InvokeAsync("initTerm", this.shellPath ?? configuredShellPath, cols, rows, directory, ((object)this.args) ?? this.package.OptionStartupArgument, env).FileAndForget("WhackWhackTerminal/InitPty");
        }

        public void ResizePty(int cols, int rows)
        {
            this.ptyService.InvokeAsync("resizeTerm", cols, rows).FileAndForget("WhackWhackTerminal/ResizePty");
        }

        public void TermData(string data)
        {
            this.ptyService.InvokeAsync("termData", data).FileAndForget("WhackWhackTerminal/TermData");
        }

        public bool ValidateLocalLink(string link)
        {
            return TerminalRegex.ValidateLocalLink(link);
        }
    }
}
