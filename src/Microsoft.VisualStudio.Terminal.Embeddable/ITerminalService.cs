using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal
{
    [ComImport, Guid("0A251003-6B81-4441-A00F-FB1FC45DB09B")]
    public interface ITerminalService
    {
        /// <summary>
        /// Create a new hidden terminal instance with the given options.
        /// </summary>
        /// <param name="name">The name that will be displayed as the tool window title.</param>
        /// <param name="shellPath">The path to the executable that will be launched. If the value is null the default shell set in the options will be used.</param>
        /// <param name="workingDirectory">Directory that the shell should start in.</param>
        /// <param name="args">Arguments to pass to the shell on the command line. If the value is null the default startup arguments set in the options will be used.</param>
        /// <param name="environment">Extra environment variables to set, this is additive only.</param>
        /// <returns></returns>
        Task<object> CreateTerminalAsync(string name, string shellPath, string workingDirectory, IEnumerable<string> args, IDictionary<string, string> environment);
    }
}
