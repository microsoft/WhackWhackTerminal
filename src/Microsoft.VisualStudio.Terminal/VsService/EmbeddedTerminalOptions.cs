using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Terminal.VsService
{
    [DebuggerStepThrough]
    internal sealed class EmbeddedTerminalOptions
    {
        private const string DefaultName = "Terminal Window";

        public static EmbeddedTerminalOptions Default { get; } = new EmbeddedTerminalOptions();

        public EmbeddedTerminalOptions()
        {
            Name = DefaultName;
        }

        public EmbeddedTerminalOptions(string name, string shellPath, string workingDirectory, IEnumerable<string> args, IDictionary<string, string> environment)
        {
            Name = name ?? DefaultName;
            ShellPath = shellPath;
            WorkingDirectory = workingDirectory;
            Args = args?.ToArray();
            Environment = environment == null ? null : new Dictionary<string, string>(environment, StringComparer.OrdinalIgnoreCase);
        }

        public string Name { get; }
        public string ShellPath { get; }
        public string WorkingDirectory { get; }
        public IEnumerable<string> Args { get; }
        public IDictionary<string, string> Environment { get; }
    }
}
