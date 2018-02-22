using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminal : IEmbeddedTerminal
    {
        private readonly TermWindowPackage package;
        private readonly ServiceToolWindow windowPane;

        public event EventHandler Closed;

        public EmbeddedTerminal(TermWindowPackage package, ServiceToolWindow windowPane)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.package = package;
            this.windowPane = windowPane;

            if (this.windowPane.Frame is IVsWindowFrame2 windowFrame)
            {
                var events = new WindowFrameEvents(windowFrame, this);
                windowFrame.Advise(events, out var cookie);
                events.Cookie = cookie;
            }
        }

        public async Task CloseAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            (this.windowPane.Frame as IVsWindowFrame)?.CloseFrame((uint)__FRAMECLOSE.FRAMECLOSE_NoSave);
        }

        public async Task HideAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            (this.windowPane.Frame as IVsWindowFrame)?.Hide();
        }

        public async Task ShowAsync()
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            (this.windowPane.Frame as IVsWindowFrame)?.Show();
        }

        public void ChangeWorkingDirectory(string newDirectory)
        {
            ((ServiceToolWindowControl)this.windowPane.Content).ChangeWorkingDirectory(newDirectory);
        }

        private class WindowFrameEvents: IVsWindowFrameNotify, IVsWindowFrameNotify2
        {
            private readonly IVsWindowFrame2 frame;
            private EmbeddedTerminal terminal;

            public WindowFrameEvents(IVsWindowFrame2 frame, EmbeddedTerminal terminal)
            {
                this.frame = frame;
                this.terminal = terminal;
            }

            public uint? Cookie
            {
                get;
                set;
            }

            public int OnClose(ref uint pgrfSaveOptions)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                terminal.Closed?.Invoke(terminal, EventArgs.Empty);

                if (this.Cookie.HasValue)
                {
                    this.frame?.Unadvise(this.Cookie.Value);
                }
                return VSConstants.S_OK;
            }

            public int OnShow(int fShow)
            {
                return VSConstants.S_OK;
            }

            public int OnMove()
            {
                return VSConstants.S_OK;
            }

            public int OnSize()
            {
                return VSConstants.S_OK;
            }

            public int OnDockableChange(int fDockable)
            {
                return VSConstants.S_OK;
            }
        }
    }
}
