namespace Microsoft.VisualStudio.Terminal
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Windows;
    using Microsoft.VisualStudio.ScriptedHost;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using StreamJsonRpc;

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
    [Guid(TermWindowGuidString)]
    public class TermWindow : ToolWindowPane
    {
        public const string TermWindowGuidString = "f7090fd8-8163-4e9a-9616-45ff87e0816e";
        private readonly ScriptedControl scriptedControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindow"/> class.
        /// </summary>
        public TermWindow(ToolWindowContext context) : base()
        {
            this.Caption = "Terminal Window";
            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.

            ThreadHelper.ThrowIfNotOnUIThread();

            string extensionDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string daytonaRoot = Path.Combine(extensionDirectory, "WebView");
            string daytonaManifest = Path.Combine(daytonaRoot, "daytona.json");

            this.scriptedControl = new ScriptedControl(daytonaManifest, daytonaRoot, context.CommandService, context.CommandService);

            var target = new ServiceHubTarget();
            var rpc = JsonRpc.Attach(context.ServiceHubStream, target);
            var marshal = new TerminalScriptingObject(context.Package, rpc, context.SolutionUtils, null, true, null, null, null);

            this.scriptedControl.PublishObject("hostmarshal", marshal);
            this.scriptedControl.PublishObject("terminalEvents", target);

            this.scriptedControl.GetFrameworkElement(out var scriptedControlElement);
            this.Content = scriptedControlElement as UIElement;

            this.scriptedControl.GotFocus += ScriptedControl_GotFocus;
        }

        private void ScriptedControl_GotFocus(object sender, EventArgs e)
        {
            ((IVsWindowFrame)this.Frame).Show();
        }
    }
}
