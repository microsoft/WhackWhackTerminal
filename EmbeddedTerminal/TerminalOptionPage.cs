using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EmbeddedTerminal
{
    public class TerminalOptionPage : DialogPage
    {
        private DefaultTerminal term = DefaultTerminal.Powershell;
        string customCSSPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "view\\custom.css").Replace("\\\\", "\\");

        [Category("Whack Whack Terminal")]
        [DisplayName("Default Shell")]
        public DefaultTerminal OptionTerminal
        {
            get { return term; }
            set { term = value; }
        }

        [Category("Whack Whack Terminal")]
        [DisplayName("Custom CSS File")]
        public string CustomCSSPath
        {
            get { return customCSSPath; }
            set { customCSSPath = value; }
        }
    }

    public enum DefaultTerminal
    {
        Powershell,
        CMD,
        WSLBash
    }

}
