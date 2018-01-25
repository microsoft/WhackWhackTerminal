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
        private TerminalOptionsModel model;

        public TerminalOptionPage()
        {
            this.model = new TerminalOptionsModel(AsyncServiceProvider.GlobalProvider);
        }

        public override object AutomationObject => this.model;

        public override void LoadSettingsFromStorage()
        {
            ThreadHelper.JoinableTaskFactory.Run(this.model.LoadDataAsync);
        }

        public override void SaveSettingsToStorage()
        {
            ThreadHelper.JoinableTaskFactory.Run(this.model.SaveDataAsync);
        }
    }
}
