using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    public class EmbeddedTerminalService : STerminalService, ITerminalService, ITerminalRendererService
    {
        private readonly TermWindowPackage package;

        public EmbeddedTerminalService(TermWindowPackage package)
        {
            this.package = package;
        }

        public async Task<object> CreateTerminalAsync(string name, string shellPath, string workingDirectory, IEnumerable<string> args, IDictionary<string, string> environment) =>
            await this.package.CreateTerminalAsync(new EmbeddedTerminalOptions(name, shellPath, workingDirectory, args, environment), pane: null);

        async Task<object> ITerminalRendererService.CreateTerminalRendererAsync(string name) =>
            await this.package.CreateTerminalRendererAsync(name);

        public async Task<object> CreateTerminalRendererAsync(string name, int cols, int rows)
        {
            var result = await this.package.CreateTerminalRendererAsync(name);
            await result.ResizeAsync(cols, rows, package.DisposalToken);
            return result;
        }
    }
}
