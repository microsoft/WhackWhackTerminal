using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Terminal.VsService;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    [Guid(ToolWindowGuid)]
    public class TermWindow : TermWindowPane
    {
        public const string ToolWindowGuid = "7f989465-4c63-4820-aa1c-c320682d2c73";

        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindow"/> class.
        /// </summary>
        public TermWindow(TermWindowPackage package) : base(package)
        {
            this.Caption = "Terminal Window";
            package.CreateTerminalAsync(
                EmbeddedTerminalOptions.Default, 
                pane: this
                )
                .FileAndForget("WhackWhackTerminal/TerminalWindow/Open");
        }
    }
}
