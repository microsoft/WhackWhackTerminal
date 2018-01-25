using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace EmbeddedTerminal
{
    internal abstract class OptionsModel
    {
        private readonly Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider;

        public OptionsModel(Microsoft.VisualStudio.Shell.IAsyncServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected abstract string CollectionName { get; }

        public async Task LoadDataAsync()
        {
            IVsSettingsManager settingsManager = (IVsSettingsManager)await this.serviceProvider.GetServiceAsync(typeof(SVsSettingsManager));
            ShellSettingsManager shellSettingsManager = new ShellSettingsManager(settingsManager);

            var settingsStore = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            this.EnsureCollection(settingsStore, forceWrite: false);
        }

        public async Task SaveDataAsync()
        {
            IVsSettingsManager settingsManager = (IVsSettingsManager)await this.serviceProvider.GetServiceAsync(typeof(SVsSettingsManager));
            ShellSettingsManager shellSettingsManager = new ShellSettingsManager(settingsManager);

            var settingsStore = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            this.EnsureCollection(settingsStore, forceWrite: true);
        }

        private void EnsureCollection(WritableSettingsStore settingsStore, bool forceWrite)
        {
            if (!settingsStore.CollectionExists(this.CollectionName))
            {
                settingsStore.CreateCollection(this.CollectionName);
            }

            var properties = this.GetType()
                .GetProperties()
                .Where(p => p.IsDefined(typeof(OptionItemAttribute), false))
                .Where(p => p.PropertyType.IsSerializable)
                .Select(p => new { p.Name, Value = p.GetValue(this)});

            foreach (var property in properties)
            {
                if (forceWrite || !settingsStore.PropertyExists(this.CollectionName, property.Name))
                {
                    var output = Newtonsoft.Json.JsonConvert.SerializeObject(property.Value);
                    settingsStore.SetString(this.CollectionName, property.Name, output);
                }
            }
        }
    }
}
