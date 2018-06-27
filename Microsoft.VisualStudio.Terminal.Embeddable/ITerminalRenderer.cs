using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal
{
    public delegate void TerminalDataRecievedEventHandler(object sender, string data);

    /// <summary>
    /// Terminal renderer.
    /// The terminal starts in auto-fit mode where terminal size always matches the window size when window resizes.
    /// After a first call to <c>ResizeAsyc()</c>, the terminal switches to fixed size mode where the terminal size stays
    /// fixed and doesn't change when the window size changes. In this mode the terminal size can only be changed
    /// with <c>ResizeAsync()</c>. To show the terminal inside the window, a dotted border will be displayed 
    /// on the bottom and right sides of it.
    /// If the terminal window gets smaller than the terminal, some parts of the terminal may get clipped from view.
    /// </summary>
    [ComImport, Guid("150B7535-03F9-41C0-9515-17ECB8199FFE")]
    public interface ITerminalRenderer : ITerminal
    {
        /// <summary>
        /// Gets current cols of the terminal window.
        /// </summary>
        int Cols { get; }

        /// <summary>
        /// Gets current rows of the terminal window.
        /// </summary>
        int Rows { get; }

        /// <summary>
        /// An event that is fired when the terminal window is resized and either Rows or Cols have changed. 
        /// </summary>
        event EventHandler TerminalResized;

        /// <summary>
        /// An event that is fired when user input is recieved.
        /// </summary>
        event TerminalDataRecievedEventHandler TerminalDataRecieved;

        /// <summary>
        /// Changes the size of the terminal.
        /// After that the terminal size may not match the terminal window size.
        /// <c>Rows</c> and <c>Cols</c> always match the terminal window, and may differ from the terminal size
        /// after <c>ResizeAsync</c> is called.
        /// Calling<c>ResizeAsync</c> doesn't cause <c>TerminalResized</c> event.
        /// </summary>
        Task ResizeAsync(int cols, int rows, CancellationToken cancellationToken);

        /// <summary>
        /// Sends data to the terminal making it render the data.
        /// </summary>
        Task PtyDataAsync(string data, CancellationToken cancellationToken);
    }
}
