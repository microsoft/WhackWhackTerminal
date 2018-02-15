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
        private readonly ITerminalBackend backend;
        internal readonly SolutionUtils solutionUtils;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindowControl"/> class.
        /// </summary>
        public TermWindowControl(ITerminalBackend backend)
        {
            this.InitializeComponent();

            this.Focusable = true;
            this.GotFocus += TermWindowControl_GotFocus;
            this.backend = backend;
            var solutionService = ThreadHelper.JoinableTaskFactory.Run(async () => (IVsSolution)await TermWindowPackage.Instance.GetServiceAsync(typeof(SVsSolution)));
            var componentModel = ThreadHelper.JoinableTaskFactory.Run(async () => (IComponentModel)await TermWindowPackage.Instance.GetServiceAsync(typeof(SComponentModel)));
            var workspaceService = componentModel.GetService<IVsFolderWorkspaceService>();
            this.solutionUtils = new SolutionUtils(solutionService, workspaceService);

            var client = new HubClient();
            var clientStream = client.RequestServiceAsync("wwt.pty").Result;

            var target = new TerminalEvent(this.terminalView, this.solutionUtils);
            var rpc = JsonRpc.Attach(clientStream, target);
            this.terminalView.ScriptingObject = new TerminalScriptingObject(rpc, this.solutionUtils);

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