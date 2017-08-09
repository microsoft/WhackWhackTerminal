using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedTerminal
{
    public class TerminalOptionPage : DialogPage
    {
        private DefaultTerminal term = DefaultTerminal.Powershell;

        [Category("Whack Whack Terminal")]
        [DisplayName("Default Shell")]
        public DefaultTerminal OptionTerminal
        {
            get { return term; }
            set { term = value; }
        }
    }

    public enum DefaultTerminal
    {
        Powershell,
        CMD,
        WSLBash
    }
}
