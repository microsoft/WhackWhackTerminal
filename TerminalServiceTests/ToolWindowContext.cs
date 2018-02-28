using Microsoft.VisualStudio.Terminal;

namespace TerminalServiceTests
{
    public class ToolWindowContext
    {
        public TerminalFunctionsPackage Package
        {
            get;
            set;
        }

        public ITerminalService TerminalService
        {
            get;
            set;
        }
    }
}
