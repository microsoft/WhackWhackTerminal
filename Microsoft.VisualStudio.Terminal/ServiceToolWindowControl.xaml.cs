namespace Microsoft.VisualStudio.Terminal
{
    using Microsoft.VisualStudio.PlatformUI;
    using StreamJsonRpc;
    using System;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ServiceToolWindowControl.
    /// </summary>
    public partial class ServiceToolWindowControl : UserControl
    {
        private readonly SolutionUtils solutionUtils;
        private readonly TermWindowPackage package;
        private readonly JsonRpc rpc;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceToolWindowControl"/> class.
        /// </summary>
        public ServiceToolWindowControl(ToolWindowContext context)
        {
            this.InitializeComponent();

            this.Focusable = true;
            this.GotFocus += TermWindowControl_GotFocus;

            var target = new TerminalEvent(context.Package, this.terminalView, context.SolutionUtils);
            this.rpc = JsonRpc.Attach(context.ServiceHubStream, target);
            this.package = context.Package;
            this.solutionUtils = context.SolutionUtils;
        }

        internal void FinishInitialize(string workingDirectory = null, string shellPath = null, string args = null)
        {
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;

            this.terminalView.ScriptingObject = new TerminalScriptingObject(this.package, this.rpc, this.solutionUtils, workingDirectory, false, shellPath, args);

            string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rootPath = Path.Combine(extensionDirectory, "WebView\\default.html").Replace("\\\\", "\\");
            this.terminalView.Navigate(new Uri(rootPath));
        }

        public void Dispose()
        {
            var helper = (ITerminalScriptingObject)this.terminalView.ScriptingObject;
            helper.ClosePty();
        }

        private void TermWindowControl_GotFocus(object sender, RoutedEventArgs e)
        {
            // We call focus here because if we don't, the next call will prevent the toolbar from turning blue.
            // No functionality is lost when this happens but it is not consistent with VS design conventions.
            this.Focus();
            this.terminalView.Invoke("triggerEvent", "focus");
        }

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