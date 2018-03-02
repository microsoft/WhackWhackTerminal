namespace Microsoft.VisualStudio.Terminal
{
    using Microsoft.VisualStudio.Shell;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using StreamJsonRpc;
    using Microsoft.VisualStudio.PlatformUI;
    using Microsoft.VisualStudio.ScriptedHost;

    /// <summary>
    /// Interaction logic for TermWindowControl.
    /// </summary>
    public partial class TermWindowControl : UserControl
    {
        private readonly TermWindowPackage package;
        private readonly ScriptedControl scriptedControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindowControl"/> class.
        /// </summary>
        public TermWindowControl(ToolWindowContext context)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            this.InitializeComponent();

            string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string daytonaRoot = Path.Combine(extensionDirectory, "WebView");
            string daytonaManifest = Path.Combine(daytonaRoot, "daytona.json");
            this.scriptedControl = new ScriptedControl(daytonaManifest, daytonaRoot, context.CommandService, context.CommandService);

            var target = new TerminalEvent(context.Package);
            var rpc = JsonRpc.Attach(context.ServiceHubStream, target);
            var marshal = new TerminalScriptingObject(context.Package, rpc, context.SolutionUtils, null, true, null, null, null);
            this.scriptedControl.PublishObject("hostmarshal", marshal);
            this.scriptedControl.PublishObject("terminalEvents", target);

            this.scriptedControl.GetFrameworkElement(out var scriptedControlElement);
            this.terminalView.Content = scriptedControlElement;
        }

        //public void Dispose()
        //{
        //    var helper = (ITerminalScriptingObject)this.terminalView.ScriptingObject;
        //    helper.ClosePty();
        //}

        //private void TermWindowControl_GotFocus(object sender, RoutedEventArgs e)
        //{
        //    // We call focus here because if we don't, the next call will prevent the toolbar from turning blue.
        //    // No functionality is lost when this happens but it is not consistent with VS design conventions.
        //    this.Focus();
        //    this.terminalView.Invoke("triggerEvent", "focus");
        //}

        //private void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        //{
        //    this.package.JoinableTaskFactory.RunAsync(async () =>
        //    {
        //        await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
        //        this.terminalView.Invoke("triggerEvent", "themeChanged", TerminalThemer.GetTheme());
        //    });
        //}

        //private void SolutionUtils_SolutionChanged(object sender, string solutionDir)
        //{
        //    if (this.package.OptionChangeDirectory)
        //    {
        //        this.package.JoinableTaskFactory.RunAsync(async () =>
        //        {
        //            await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
        //            this.terminalView.Invoke("triggerEvent", "directoryChanged", solutionDir);
        //        });
        //    }
        //}
    }
}