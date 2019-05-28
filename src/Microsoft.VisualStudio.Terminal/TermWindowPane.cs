using Microsoft.VisualStudio.Shell;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    /// </summary>
    public class TermWindowPane : ToolWindowPane
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermWindow"/> class.
        /// </summary>
        public TermWindowPane(TermWindowPackage package) : base(null)
        {
            this.Content = new TermWindowControl(package);
        }

        internal Task InitAsync(TerminalScriptingObject scriptingObject, CancellationToken cancellationToken) =>
            ((TermWindowControl)Content).InitAsync(scriptingObject, cancellationToken);
    }
}
