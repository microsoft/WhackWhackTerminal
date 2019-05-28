namespace TerminalServiceTests
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Terminal;

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
    [Guid("4d0c7b72-bb72-4fbb-a7ce-5fdc81ed6721")]
    public class TerminalFunctions : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalFunctions"/> class.
        /// </summary>
        public TerminalFunctions(ToolWindowContext context) : base(null)
        {
            this.Caption = "TerminalFunctions";

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
            // the object returned by the Content property.
            this.Content = new TerminalFunctionsControl(context);
        }
    }
}
