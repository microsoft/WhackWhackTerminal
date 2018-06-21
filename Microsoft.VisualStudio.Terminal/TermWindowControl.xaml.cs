using Microsoft.VisualStudio.PlatformUI;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static System.FormattableString;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// Interaction logic for ServiceToolWindowControl.
    /// </summary>
    public partial class TermWindowControl : UserControl, IDisposable
    {
        private readonly TermWindowPackage package;
        private bool pendingFocus;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceToolWindowControl"/> class.
        /// </summary>
        public TermWindowControl(TermWindowPackage package)
        {
            this.package = package;

            this.InitializeComponent();

            this.Focusable = true;
            this.GotFocus += TermWindowControl_GotFocus;
            this.LostFocus += TermWindowControl_LostFocus;
        }

        internal void PtyData(string data) => this.terminalView.Invoke("triggerEvent", "ptyData", data);

        internal async Task InitAsync(TerminalScriptingObject scriptingObject, CancellationToken cancellationToken)
        {
            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var tcs = new TaskCompletionSource<object>();
            void TerminalInitHandler(object sender, TermInitEventArgs e) => tcs.TrySetResult(null);
            scriptingObject.TerminalInit += TerminalInitHandler;
            try
            {
                VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

                this.terminalView.ScriptingObject = scriptingObject;

                string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string rootPath = Path.Combine(extensionDirectory, "WebView\\default.html").Replace("\\\\", "\\");
                this.terminalView.Navigate(new Uri(rootPath));

                using (var registration = cancellationToken.Register(() => tcs.TrySetCanceled(cancellationToken)))
                {
                    await tcs.Task;
                }
            }
            finally
            {
                scriptingObject.TerminalInit -= TerminalInitHandler;
            }

            if (this.pendingFocus)
            {
                this.terminalView.Invoke("triggerEvent", "focus");
                this.pendingFocus = false;
            }
        }

        internal void Resize(int cols, int rows) => this.terminalView.Invoke(
            "triggerEvent",
            "resize",
            Invariant($"{{\"cols\": {cols}, \"rows\": {rows}}}"));

        internal void ChangeWorkingDirectory(string newDirectory) =>
            this.terminalView.Invoke("triggerEvent", "directoryChanged", newDirectory);

        public void Dispose()
        {
            var helper = (ITerminalScriptingObject)this.terminalView.ScriptingObject;
            helper?.ClosePty();
        }

        internal void PtyExited(int? code) =>
            this.terminalView.Invoke("triggerEvent", "ptyExited", code);

        private void TermWindowControl_GotFocus(object sender, RoutedEventArgs e)
        {
            // We call focus here because if we don't, the next call will prevent the toolbar from turning blue.
            // No functionality is lost when this happens but it is not consistent with VS design conventions.
            this.Focus();
            if (this.terminalView.HasDocument)
            {
                this.pendingFocus = false;
                this.terminalView.Invoke("triggerEvent", "focus");
            }
            else
            {
                this.pendingFocus = true;
            }
        }

        private void TermWindowControl_LostFocus(object sender, RoutedEventArgs e) =>
            this.pendingFocus = false;

        private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(async () =>
            {
                await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
                this.terminalView.Invoke("triggerEvent", "themeChanged", TerminalThemer.GetTheme());
            });
        }
    }
}