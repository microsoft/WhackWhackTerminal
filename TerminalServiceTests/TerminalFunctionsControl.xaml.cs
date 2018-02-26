namespace TerminalServiceTests
{
    using Microsoft.VisualStudio.Terminal;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for TerminalFunctionsControl.
    /// </summary>
    public partial class TerminalFunctionsControl : UserControl
    {
        private readonly IEmbeddedTerminalService terminalService;
        private readonly TerminalFunctionsPackage package;
        private IEmbeddedTerminal terminal;

        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalFunctionsControl"/> class.
        /// </summary>
        public TerminalFunctionsControl(ToolWindowContext context)
        {
            this.InitializeComponent();
            this.terminalService = context.TerminalService;
            this.package = context.Package;
        }

        /// <summary>
        /// Handles click on the button by displaying a message box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(async () =>
            {
                this.terminal = (IEmbeddedTerminal)await this.terminalService.CreateTerminalAsync("test name", "C:\\", Enumerable.Empty<string>(), Enumerable.Empty<string>());
                this.terminal.Closed += Terminal_Closed;
            });
        }

        private void Terminal_Closed(object sender, System.EventArgs e)
        {
            MessageBox.Show("Terminal has closed");
        }

        private void Show_Click(object sender, RoutedEventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(() => this.terminal?.ShowAsync());
        }

        private void Hide_Click(object sender, RoutedEventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(() => this.terminal?.HideAsync());
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.package.JoinableTaskFactory.RunAsync(() => this.terminal?.CloseAsync());
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            this.terminal?.ChangeWorkingDirectory(this.DirectoryPath.Text);
        }
    }
}