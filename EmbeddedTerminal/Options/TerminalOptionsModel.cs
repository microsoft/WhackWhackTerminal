using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
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
    internal class TerminalOptionsModel : OptionsModel
    {
        public TerminalOptionsModel(IVsSettingsManager settingsManager): base(settingsManager)
        {
        }

        protected override string CollectionName => "WhackWhackTerminalSettings";

        [Category("Whack Whack Terminal")]
        [DisplayName("Default Shell")]
        [OptionItem]
        public DefaultTerminal OptionTerminal { get; set; } = DefaultTerminal.Powershell;

        [Category("Whack Whack Terminal")]
        [DisplayName("Custom CSS File")]
        [OptionItem]
        public string CustomCSSPath { get; set; } = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "view\\custom.css").Replace("\\\\", "\\");

        [Category("Whack Whack Terminal")]
        [DisplayName("Startup Argument")]
        [OptionItem]
        public string StartupArgument { get; set; } = string.Empty;
    }

    public enum DefaultTerminal
    {
        Powershell,
        CMD,
        WSLBash
    }
}
