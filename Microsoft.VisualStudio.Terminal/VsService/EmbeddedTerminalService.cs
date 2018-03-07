using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminalService : STerminalService, ITerminalService
    {
        private int nextToolWindowId = 1;
        private readonly TermWindowPackage package;

        public EmbeddedTerminalService(TermWindowPackage package)
        {
            this.package = package;
        }

        public async Task<object> CreateTerminalAsync(string name, string shellPath, string workingDirectory, IEnumerable<string> args, IDictionary<string, string> environment)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var pane = (ServiceToolWindow)await package.FindToolWindowAsync(
                    typeof(ServiceToolWindow),
                    nextToolWindowId++,
                    create: true,
                    cancellationToken: package.DisposalToken);
            pane.Caption = name;
            pane.FinishInitialize(workingDirectory, shellPath, args, environment);

            return new EmbeddedTerminal(this.package, pane);
        }
    }
}
