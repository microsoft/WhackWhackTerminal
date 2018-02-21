using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminal : IEmbeddedTerminal
    {
        private readonly TermWindowPackage package;
        private readonly ServiceToolWindow windowPane;

        public event EventHandler Closed;

        public EmbeddedTerminal(TermWindowPackage package, ServiceToolWindow windowPane)
        {
            this.package = package;
            this.windowPane = windowPane;
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
            throw new NotImplementedException();
        }
    }
}
