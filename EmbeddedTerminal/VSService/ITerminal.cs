using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedTerminal.VSService
{
    public interface ITerminal
    {
        void Show();
        void Hide();
        void Close();

        event EventHandler Closed;
    }
}
