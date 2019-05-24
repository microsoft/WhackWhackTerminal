using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace TerminalServiceTests
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class TerminalFunctionsCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("17aa71f3-6754-4047-86c2-83ff7e345402");

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeCommandAsync(AsyncPackage package)
        {
            var commandService = (IMenuCommandService)await package.GetServiceAsync(typeof(IMenuCommandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand((sender, e) => ShowToolWindow(package, sender, e), menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private static void ShowToolWindow(AsyncPackage package, object sender, EventArgs e)
        {
            package.JoinableTaskFactory.RunAsync(async () =>
            {
                await package.ShowToolWindowAsync(
                    typeof(TerminalFunctions),
                    0,
                    create: true,
                    cancellationToken: package.DisposalToken);
            }).FileAndForget("WhackWhackTerminal/TerminalFunctionsWindow/Open");
        }
    }
}
