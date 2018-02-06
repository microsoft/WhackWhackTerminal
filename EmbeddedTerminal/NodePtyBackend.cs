using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedTerminal
{
    public class NodePtyBackend : ITerminalBackend
    {
        private readonly JsonRpc rpc;

        public NodePtyBackend(Stream connectionStream, DefaultTerminal terminal, int cols, int rows, string workingDirectory, string args)
        {
            var target = new ServiceTarget(this);
            this.rpc = JsonRpc.Attach(connectionStream, target);
        }

        public event EventHandler<string> PtyData;
        public event EventHandler<int?> PtyExit;

        public Task ExitAsync()
        {
            throw new NotImplementedException();
        }

        public Task OnFrontEndDataAsync(string data)
        {
            throw new NotImplementedException();
        }

        public Task ResizeAsync(int cols, int rows)
        {
            throw new NotImplementedException();
        }

        private class ServiceTarget
        {
            private readonly NodePtyBackend backend;

            public ServiceTarget(NodePtyBackend backend)
            {
                this.backend = backend;
            }

            public void PtyData(string data)
            {
                this.backend.PtyData?.Invoke(backend, data);
            }

            public void PtyExit(string code)
            {
                this.backend.PtyExit?.Invoke(backend, 0);
            }
        }
    }
}
