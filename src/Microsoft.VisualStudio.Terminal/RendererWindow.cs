using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    [Guid(ToolWindowGuid)]
    public class RendererWindow : TermWindowPane
    {
        public const string ToolWindowGuid = "D3F9A61A-63E2-4AA8-8BCF-229D733CA27E";

        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindow"/> class.
        /// </summary>
        public RendererWindow(TermWindowPackage package) : base(package)
        {
        }
    }
}
