using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminalService : SEmbeddedTerminalService, IEmbeddedTerminalService
    {
        private int nextToolWindowId = 1;
        private readonly TermWindowPackage package;

        public EmbeddedTerminalService(TermWindowPackage package)
        {
            this.package = package;
        }

        public async Task<IEmbeddedTerminal> CreateTerminalAsync(string name, string workingDirectory, IEnumerable<string> args, IEnumerable<string> environment)
        {
            var pane = (TermWindow)await package.FindToolWindowAsync(
                    typeof(TermWindow),
                    nextToolWindowId++,
                    create: true,
                    cancellationToken: package.DisposalToken);
            pane.Caption = name;
            ((TermWindowControl)pane.Content).FinishInitialize(false, workingDirectory, string.Join(" ", args));

            return new EmbeddedTerminal(this.package, pane);
        }
    }
}
