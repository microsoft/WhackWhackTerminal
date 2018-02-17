using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WhackWhackTerminalServiceTypes;

namespace EmbeddedTerminal
{
    public class EmbeddedTerminalService : SEmbeddedTerminalService, IEmbeddedTerminalService
    {
        private readonly TermWindowPackage package;

        public EmbeddedTerminalService(TermWindowPackage package)
        {
            this.package = package;
        }

        public Task<IEmbeddedTerminal> CreateTerminalAsync(string name, string workingDirectory, IEnumerable<string> args, IEnumerable<string> environment)
        {
            throw new NotImplementedException();
        }
    }
}
