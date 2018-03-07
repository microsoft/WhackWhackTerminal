namespace Microsoft.VisualStudio.Terminal
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.ScriptedHost;
    using Microsoft.VisualStudio.Shell;
    using StreamJsonRpc;
    using Task = System.Threading.Tasks.Task;

    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    /// <remarks>
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
    /// usually implemented by the package implementer.
    /// <para>
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its
    /// implementation of the IVsUIElementPane interface.
    /// </para>
    /// </remarks>
    [Guid(ServiceToolWindowGuid)]
    public class ServiceToolWindow : ToolWindowPane
    {
        public const string ServiceToolWindowGuid = "ebfb63ec-6efd-4fb3-834f-e9da3a40f2a1";
        private readonly ToolWindowContext context;
        private ScriptedControl scriptedControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceToolWindow"/> class.
        /// </summary>
        public ServiceToolWindow(ToolWindowContext context) : base(null)
        {
            this.context = context;
        }

        internal void FinishInitialize(string workingDirectory, string shellPath, IEnumerable<string> args, IDictionary<string, string> env)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string daytonaRoot = Path.Combine(extensionDirectory, "WebView");
            string daytonaManifest = Path.Combine(daytonaRoot, "daytona.json");
            this.scriptedControl = new ScriptedControl(daytonaManifest, daytonaRoot, context.CommandService, context.CommandService);

            var target = new ServiceHubTarget();
            var rpc = JsonRpc.Attach(context.ServiceHubStream, target);
            var marshal = new TerminalScriptingObject(context.Package, rpc, context.SolutionUtils, workingDirectory, false, shellPath, args, env);
            this.scriptedControl.PublishObject("hostmarshal", marshal);
            this.scriptedControl.PublishObject("terminalEvents", target);

            this.scriptedControl.GetFrameworkElement(out var scriptedControlElement);
            this.Content = scriptedControlElement;
        }

        internal async Task ChangeWorkingDirectoryAsync(string workingDirectory)
        {
            await Task.Yield();
            //await this.package.JoinableTaskFactory.SwitchToMainThreadAsync();
            //this.terminalView.Invoke("triggerEvent", "directoryChanged", workingDirectory);
        }
    }
}
