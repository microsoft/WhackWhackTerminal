using Microsoft.VisualStudio.Terminal.VsService;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows;

namespace Microsoft.VisualStudio.Terminal
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisible(true)]
    public sealed class TerminalScriptingObject : ITerminalScriptingObject
    {
        private readonly TerminalRenderer terminal;
        private readonly TermWindowPackage package;

        internal TerminalScriptingObject(TerminalRenderer terminal, TermWindowPackage package)
        {
            this.terminal = terminal;
            this.package = package;
        }

        public event EventHandler<TermInitEventArgs> TerminalInit;

        public string GetTheme() => TerminalThemer.GetTheme();

        public string GetFontFamily() => this.package.OptionFontFamily;

        public int GetFontSize() => this.package.OptionFontSize;

        public void CopyStringToClipboard(string stringToCopy) => Clipboard.SetText(stringToCopy ?? "");

        public string GetClipboard() => Clipboard.GetText();

        public string GetLinkRegex() => TerminalRegex.LocalLinkRegex.ToString();

        public void HandleLocalLink(string uri) => TerminalRegex.HandleLocalLink(uri);

        public bool ValidateLocalLink(string link) => TerminalRegex.ValidateLocalLink(link);

        public void ClosePty() =>
            this.terminal.OnTerminalClosed();

        public void InitPty(int cols, int rows, string directory) =>
            TerminalInit?.Invoke(this, new TermInitEventArgs(cols, rows, directory));

        public void ResizePty(int cols, int rows) =>
            this.terminal.OnTerminalResized(cols, rows);

        public void TermData(string data) => this.terminal.OnTerminalDataRecieved(data);

        public string GetSolutionDir() => this.terminal.SolutionDirectory;
    }

    [DebuggerStepThrough]
    public class ResizeEventArgs : EventArgs
    {
        public ResizeEventArgs(int cols, int rows)
        {
            Cols = cols;
            Rows = rows;
        }

        public int Cols { get; }
        public int Rows { get; }
    }

    [DebuggerStepThrough]
    public class TermInitEventArgs : ResizeEventArgs
    {
        public TermInitEventArgs(int cols, int rows, string directory) : base(cols, rows)
        {
            Directory = directory;
        }

        public string Directory { get; }
    }
}
