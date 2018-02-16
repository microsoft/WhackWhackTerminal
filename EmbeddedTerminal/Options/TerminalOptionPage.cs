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
