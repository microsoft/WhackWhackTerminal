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

        [Category("Whack Whack Terminal")]
        [DisplayName("Default Shell")]
        [OptionItem]
        public DefaultTerminal OptionTerminal { get; set; } = DefaultTerminal.Powershell;

        [Category("Whack Whack Terminal")]
        [DisplayName("Font Family")]
        [OptionItem]
        public string FontFamily { get; set; } = "Consolas";

        [Category("Whack Whack Terminal")]
        [DisplayName("Font Size")]
        [OptionItem]
        public int FontSize { get; set; } = 12;

        [Category("Whack Whack Terminal")]
        [DisplayName("Startup Argument")]
        [OptionItem]
        public string StartupArgument { get; set; } = string.Empty;

        [Category("Whack Whack Terminal")]
        [DisplayName("Change Working Directory on Solution Load")]
        [OptionItem]
        public bool ChangeDirectory { get; set; } = true;
    }

    public enum DefaultTerminal
    {
        Powershell,
        CMD,
        WSLBash
    }
}
