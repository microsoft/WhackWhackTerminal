using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WhackWhackTerminalServiceTypes
{

    [ComImport, Guid("E195D61C-2821-49F1-BE0E-B2CD82F1F856")]
    public interface IEmbeddedTerminal
    {
        Task ShowAsync();
        void Hide();
        void Close();

        event EventHandler Closed;
    }
}
