using System.IO;

namespace EmbeddedTerminal
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
    }
}
