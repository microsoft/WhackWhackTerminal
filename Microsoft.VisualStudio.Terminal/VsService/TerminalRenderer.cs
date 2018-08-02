using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    internal class TerminalRenderer : ITerminal, ITerminalRenderer
    {
        private readonly TermWindowPane pane;
        private readonly TerminalScriptingObject scriptingObject;
        protected readonly TermWindowPackage package;

        public TerminalRenderer(TermWindowPackage package, TermWindowPane pane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            this.package = package;
            this.pane = pane;

            if (this.pane.Frame is IVsWindowFrame2 windowFrame)
            {
                new WindowFrameEvents(windowFrame)
                {
                    Closed = (sender, e) => OnClosed(),
                };
            }

            this.Control.TermData += Control_TermData;
            this.Control.TermResize += Control_TermResize;
            this.Control.TermInit += Control_TermInit;
        }

        private void Control_TermInit(TermInitEventArgs obj)
        {
            this.OnTerminalInit(this, obj);
        }

        private void Control_TermResize(object arg1, (int row, int col) arg2)
        {
            this.OnTerminalResized(arg2.col, arg2.row);
        }

        private void Control_TermData(object arg1, string arg2)
        {
            this.OnTerminalDataRecieved(arg2);
        }

        public event EventHandler<TermInitEventArgs> TerminalInit;
        public event EventHandler TerminalClosed;
        public event Func<string> SolutionDirectoryProvider;

        internal protected virtual string SolutionDirectory => null;

        private TermWindowControl Control => ((TermWindowControl)this.pane.Content);

        internal Task InitAsync(CancellationToken cancellationToken) =>
            this.pane.InitAsync(this.scriptingObject, cancellationToken);

        #region ITerminal

        public async Task ShowAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            (this.pane.Frame as IVsWindowFrame)?.Show();
        }

        public async Task HideAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            (this.pane.Frame as IVsWindowFrame)?.Hide();
        }

        public async Task CloseAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            (this.pane.Frame as IVsWindowFrame)?.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        public async Task ChangeWorkingDirectoryAsync(string newDirectory)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            Control.ChangeWorkingDirectory(newDirectory);
        }

        public event EventHandler Closed;

        #endregion

        #region ITerminalRenderer

        public int Cols { get; private set; }
        public int Rows { get; private set; }
        public event EventHandler TerminalResized;
        public event TerminalDataRecievedEventHandler TerminalDataRecieved;

        public async Task ResizeAsync(int cols, int rows, CancellationToken cancellationToken)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            Control.Resize(cols, rows);
        }

        public async Task PtyDataAsync(string data, CancellationToken cancellationToken)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            Control.PtyData(data);
        }

        #endregion

        internal async Task PtyExitedAsync(int? code, CancellationToken cancellationToken)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            Control.PtyExited(code);
        }

        internal protected virtual void OnTerminalDataRecieved(string data) =>
            TerminalDataRecieved?.Invoke(this, data);

        internal protected virtual void OnTerminalResized(int cols, int rows)
        {
            bool changed = Cols != cols || Rows != rows;
            Cols = cols;
            Rows = rows;

            if (changed)
            {
                TerminalResized?.Invoke(this, EventArgs.Empty);
            }
        }

        internal protected virtual void OnTerminalInit(object sender, TermInitEventArgs e) =>
            TerminalInit?.Invoke(this, e);

        internal protected virtual void OnTerminalClosed() =>
            TerminalClosed?.Invoke(this, EventArgs.Empty);

        internal string ScriptingObject_GetSolutionDirectory()
        {
            var provider = SolutionDirectoryProvider;
            return provider != null ? provider() : null;
        }

        protected virtual void OnClosed() =>
            Closed?.Invoke(this, EventArgs.Empty);

        private sealed class WindowFrameEvents : IVsWindowFrameNotify, IVsWindowFrameNotify2
        {
            private readonly IVsWindowFrame2 frame;
            private readonly uint cookie;

            public WindowFrameEvents(IVsWindowFrame2 frame)
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                this.frame = frame;
                ErrorHandler.ThrowOnFailure(frame.Advise(this, out this.cookie));
            }

            public EventHandler Closed
            {
                get;
                set;
            }

            public int OnClose(ref uint pgrfSaveOptions)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                Closed?.Invoke(this, EventArgs.Empty);
                this.frame.Unadvise(this.cookie);
                return VSConstants.S_OK;
            }

            public int OnShow(int fShow) => VSConstants.S_OK;
            public int OnMove() => VSConstants.S_OK;
            public int OnSize() => VSConstants.S_OK;
            public int OnDockableChange(int fDockable) => VSConstants.S_OK;
        }
    }
}
