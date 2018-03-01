using Microsoft.VisualStudio.Shell.Interop;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Terminal
{
    internal class TerminalOptionsModel : OptionsModel
    {
        public TerminalOptionsModel(IVsSettingsManager settingsManager): base(settingsManager)
        {
        }

        protected override string CollectionName => "WhackWhackTerminalSettings";

        [Category("Shell Selection")]
        [DisplayName("Default Shell")]
        [OptionItem]
        public DefaultTerminal OptionTerminal { get; set; } = DefaultTerminal.Powershell;

        [Category("Shell Selection")]
        [DisplayName("Shell Path")]
        [Description("Specifies the shell path when other is selected")]
        [OptionItem]
        public string ShellPath { get; set; } = string.Empty;

        [Category("Appearance")]
        [DisplayName("Font Family")]
        [OptionItem]
        public string FontFamily { get; set; } = "Consolas";

        [Category("Appearance")]
        [DisplayName("Font Size")]
        [OptionItem]
        public int FontSize { get; set; } = 12;

        [DisplayName("Startup Argument")]
        [OptionItem]
        public string StartupArgument { get; set; } = string.Empty;

        [DisplayName("Change to Current Solution")]
        [OptionItem]
        public bool ChangeDirectory { get; set; } = true;
    }

    public enum DefaultTerminal
    {
        Powershell,
        CMD,
        WSLBash,
        Other
    }
}
