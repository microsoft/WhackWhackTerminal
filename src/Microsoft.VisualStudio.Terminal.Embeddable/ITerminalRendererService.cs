using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal
{
    [ComImport, Guid("250301DA-0BEA-4C4E-B199-C363C7C164B2")]
    public interface ITerminalRendererService
    {
        /// <summary>
        /// Create a new terminal renderer instance with the given name that fits the window.
        /// </summary>
        /// <param name="name">The name that will be displayed as the tool window title.</param>
        /// <returns>An instance of ITerminalRenderer</returns>
        Task<object> CreateTerminalRendererAsync(string name);

        /// <summary>
        /// Create a new terminal renderer instance with the given name and dimensions.
        /// The terminal size won't change when user resizes the window.
        /// </summary>
        /// <param name="name">The name that will be displayed as the tool window title.</param>
        /// <returns>An instance of ITerminalRenderer</returns>
        Task<object> CreateTerminalRendererAsync(string name, int cols, int rows);
    }
}
