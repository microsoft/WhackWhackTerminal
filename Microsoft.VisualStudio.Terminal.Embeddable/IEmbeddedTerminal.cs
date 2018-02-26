using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Terminal
{
    [ComImport, Guid("E195D61C-2821-49F1-BE0E-B2CD82F1F856")]
    public interface IEmbeddedTerminal
    {
        Task ShowAsync();
        Task HideAsync();
        Task CloseAsync();
        void ChangeWorkingDirectory(string newDirectory);

        event EventHandler Closed;
    }
}
