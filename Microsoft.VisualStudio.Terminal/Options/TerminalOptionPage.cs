using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Terminal
{
    public class TerminalOptionPage : DialogPage
    {
        private TerminalOptionsModel model;

        public TerminalOptionPage()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            IVsSettingsManager settingsManager = (IVsSettingsManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsSettingsManager));
            this.model = new TerminalOptionsModel(settingsManager);
        }

        public override object AutomationObject => this.model;

        public override void LoadSettingsFromStorage()
        {
            this.model.LoadData();
        }

        public override void SaveSettingsToStorage()
        {
            this.model.SaveData();
        }
    }
}
