using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhackWhackTerminalServiceTypes
{
    public interface IEmbeddedTerminal
    {
        Task ShowAsync();
        void Hide();
        void Close();

        event EventHandler Closed;
    }
}
