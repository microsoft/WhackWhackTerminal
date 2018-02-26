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

        public IEmbeddedTerminalService TerminalService
        {
            get;
            set;
        }
    }
}
