using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminalService : SEmbeddedTerminalService, IEmbeddedTerminalService
    {
        private readonly TermWindowPackage package;

        public EmbeddedTerminalService(TermWindowPackage package)
        {
            this.package = package;
        }

        public async Task<IEmbeddedTerminal> CreateTerminalAsync(string name, string workingDirectory, IEnumerable<string> args, IEnumerable<string> environment)
        {
            var pane = (TermWindow)await package.ShowToolWindowAsync(
                    typeof(TermWindow),
                    0,
                    create: true,
                    cancellationToken: package.DisposalToken);

            return new EmbeddedTerminal(this.package, pane);
        }
    }
}
