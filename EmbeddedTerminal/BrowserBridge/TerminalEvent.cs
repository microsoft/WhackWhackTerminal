namespace EmbeddedTerminal
{
    using Microsoft.VisualStudio.PlatformUI;
    using System.Threading.Tasks;

    internal class TerminalEvent
    {
        private readonly BetterBrowser browser;
        private readonly SolutionUtils solutionUtils;

        public TerminalEvent(BetterBrowser browser, SolutionUtils solutionUtils)
        {
            this.browser = browser;
            this.solutionUtils = solutionUtils;

            this.solutionUtils.SolutionChanged += SolutionUtils_SolutionChanged;
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        private async void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "themeChanged", TerminalThemer.GetTheme());
        }

        private async void SolutionUtils_SolutionChanged(string solutionDir)
        {
            if (TermWindowPackage.Instance.OptionChangeDirectory)
            {
                await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
                this.browser.Invoke("triggerEvent", "directoryChanged", solutionDir);
            }
        }

        public async Task PtyData(string data)
        {
            await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "ptyData", data);
        }

        public async Task PtyExit(int? code)
        {
            await TermWindowPackage.Instance.JoinableTaskFactory.SwitchToMainThreadAsync();
            this.browser.Invoke("triggerEvent", "ptyExited", code);
        }
    }
}