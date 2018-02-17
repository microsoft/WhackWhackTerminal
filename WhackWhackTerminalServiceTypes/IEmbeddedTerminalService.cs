using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhackWhackTerminalServiceTypes
{
    public interface IEmbeddedTerminalService
    {
        Task<IEmbeddedTerminal> CreateTerminalAsync(string name, string workingDirectory, IEnumerable<string> args, IEnumerable<string> environment);
    }
}
