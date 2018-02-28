using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal
{
    /// <summary>
    /// Handle to a specific terminal instance.
    /// </summary>
    [ComImport, Guid("E195D61C-2821-49F1-BE0E-B2CD82F1F856")]
    public interface ITerminal
    {
        /// <summary>
        /// Shows the terminal window.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes once the tool window has been shown.</returns>
        Task ShowAsync();

        /// <summary>
        /// Hides the terminal window.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes once the tool window has been hidden.</returns>
        Task HideAsync();

        /// <summary>
        /// Closes the terminal window and destroys the underlying terminal instance.
        /// It is considered an error to call any methods on this object after close has been called.
        /// </summary>
        /// <returns>A <see cref="Task"/> that completes once the tool window has been closed.</returns>
        Task CloseAsync();

        /// <summary>
        /// Changes the current working directory of the terminal. Note that this will restart the backend process
        /// so any running tasks will be killed.
        /// </summary>
        /// <param name="newDirectory">The directory to switch the terminal to.</param>
        /// <returns>A <see cref="Task"/> that completes once the terminal has switched directories.</returns>
        Task ChangeWorkingDirectoryAsync(string newDirectory);

        /// <summary>
        /// An event that is fired when the terminal closes. 
        /// </summary>
        event EventHandler Closed;
    }
}
