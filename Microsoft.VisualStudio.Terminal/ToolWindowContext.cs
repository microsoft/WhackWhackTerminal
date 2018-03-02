using Microsoft.VisualStudio.Shell;
using System.IO;

namespace Microsoft.VisualStudio.Terminal
{
    public class ToolWindowContext
    {
        internal TermWindowPackage Package
        {
            set;
            get;
        }

        internal SolutionUtils SolutionUtils
        {
            set;
            get;
        }

        internal Stream ServiceHubStream
        {
            set;
            get;
        }

        internal OleMenuCommandService CommandService
        {
            get;
            set;
        }
    }
}
