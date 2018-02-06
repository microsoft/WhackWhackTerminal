using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedTerminal
{
    public interface ITerminalBackend
    {
        event EventHandler<string> PtyData;
        event EventHandler<int?> PtyExit;

        Task OnFrontEndDataAsync(string data);
        Task ResizeAsync(int cols, int rows);
        Task ExitAsync();
    }
}
