namespace Microsoft.VisualStudio.Terminal
{
    using Microsoft.VisualStudio.PlatformUI;
    using StreamJsonRpc;
    using System.Threading.Tasks;

    internal class TerminalEvent
    {
        private readonly TermWindowPackage package;
        private readonly BetterBrowser browser;

        public TerminalEvent(TermWindowPackage package, BetterBrowser browser, SolutionUtils solutionUtils)
        {
            this.package = package;
            this.browser = browser;
        }

        [JsonRpcMethod("PtyData")]
        public async Task PtyDataAsync(string data)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "ptyData", data);
        }

        [JsonRpcMethod("PtyExit")]
        public async Task PtyExitAsync(int? code)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "ptyExited", code);
        }
    }
}