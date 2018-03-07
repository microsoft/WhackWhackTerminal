

using Microsoft.VisualStudio.ScriptedHost;
using Microsoft.VisualStudio.Shell;
using StreamJsonRpc;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// Interaction logic for TerminalWindowControl.xaml
    /// </summary>
    public partial class TerminalWindowControl : UserControl
    {
        private readonly ScriptedControl scriptedControl;

        public TerminalWindowControl(ToolWindowContext context)
        {
            InitializeComponent();

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
            this.dialogContent.Content = scriptedControlElement as UIElement;

            this.GotFocus += TermWindow_GotFocus;
        }

        private void TermWindow_GotFocus(object sender, RoutedEventArgs e)
        {
            this.Focus();
        }
    }
}
