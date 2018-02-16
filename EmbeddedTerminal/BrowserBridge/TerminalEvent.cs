namespace EmbeddedTerminal
{
    using Microsoft.VisualStudio.PlatformUI;
    using StreamJsonRpc;
    using System.Threading.Tasks;

    internal class TerminalEvent
    {
        private readonly TermWindowPackage package;
        private readonly BetterBrowser browser;
        private readonly SolutionUtils solutionUtils;

        public TerminalEvent(TermWindowPackage package, BetterBrowser browser, SolutionUtils solutionUtils)
        {
            this.package = package;
            this.browser = browser;
            this.solutionUtils = solutionUtils;

            this.solutionUtils.SolutionChanged += SolutionUtils_SolutionChanged;
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(async () =>
            {
                await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
                this.browser.Invoke("triggerEvent", "themeChanged", TerminalThemer.GetTheme());
            });
        }

        private void SolutionUtils_SolutionChanged(string solutionDir)
        {
            if (this.package.OptionChangeDirectory)
            {
                this.package.JoinableTaskFactory.RunAsync(async () =>
                {
                    await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
                    this.browser.Invoke("triggerEvent", "directoryChanged", solutionDir);
                });
            }
        }

        [JsonRpcMethod("PtyData")]
        public async Task PtyDataAsync(string data)
        {
            await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "ptyData", data);
        }

        [JsonRpcMethod("PtyExit")]
        public async Task PtyExitAsync(int? code)
        {
            await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "ptyExited", code);
        }
    }
}