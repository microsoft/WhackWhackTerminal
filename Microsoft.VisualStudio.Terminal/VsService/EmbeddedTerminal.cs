using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminal : IEmbeddedTerminal
    {
        private readonly TermWindowPackage package;
        private readonly TermWindow windowPane;

        public event EventHandler Closed;

        public EmbeddedTerminal(TermWindowPackage package, TermWindow windowPane)
        {
            this.package = package;
            this.windowPane = windowPane;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public Task ShowAsync()
        {
            throw new NotImplementedException();
        }
    }
}
