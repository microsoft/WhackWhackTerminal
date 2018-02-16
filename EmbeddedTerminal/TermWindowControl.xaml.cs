namespace EmbeddedTerminal
{
    using Microsoft.VisualStudio.Shell;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.ServiceHub.Client;
    using Microsoft.VisualStudio.ComponentModelHost;
    using Microsoft.VisualStudio.Workspace.VSIntegration.Contracts;
    using StreamJsonRpc;

    /// <summary>
    /// Interaction logic for TermWindowControl.
    /// </summary>
    public partial class TermWindowControl : UserControl, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindowControl"/> class.
        /// </summary>
        public TermWindowControl(ToolWindowContext context)
        {
            this.InitializeComponent();

            this.Focusable = true;
            this.GotFocus += TermWindowControl_GotFocus;

            var target = new TerminalEvent(context.Package, this.terminalView, context.SolutionUtils);
            var rpc = JsonRpc.Attach(context.ServiceHubStream, target);
            this.terminalView.ScriptingObject = new TerminalScriptingObject(context.Package, rpc, context.SolutionUtils);

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
    }
}