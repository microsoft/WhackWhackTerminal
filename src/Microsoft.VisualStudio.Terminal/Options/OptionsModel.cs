using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Terminal
{
    internal abstract class OptionsModel
    {
        private readonly ShellSettingsManager settingsManager;

        public OptionsModel(IVsSettingsManager settingsManager)
        {
            this.settingsManager = new ShellSettingsManager(settingsManager);
            this.LoadData();
        }

        protected abstract string CollectionName { get; }

        public void LoadData()
        {
            var settingsStore = this.settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            this.EnsureCollection(settingsStore, forceWrite: false);

            foreach (var property in GetOptionProperties())
            {
                var serializedProp = settingsStore.GetString(this.CollectionName, property.Name);
                var value = Newtonsoft.Json.JsonConvert.DeserializeObject(serializedProp, property.PropertyType);
                property.SetValue(this, value);
            }
        }

        public void SaveData()
        {
            var settingsStore = this.settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            this.EnsureCollection(settingsStore, forceWrite: true);
        }

        private void EnsureCollection(WritableSettingsStore settingsStore, bool forceWrite)
        {
            if (!settingsStore.CollectionExists(this.CollectionName))
            {
                settingsStore.CreateCollection(this.CollectionName);
            }

            foreach (var property in GetOptionProperties())
            {
                if (forceWrite || !settingsStore.PropertyExists(this.CollectionName, property.Name))
                {
                    var output = Newtonsoft.Json.JsonConvert.SerializeObject(property.GetValue(this));
                    settingsStore.SetString(this.CollectionName, property.Name, output);
                }
            }
        }

        private IEnumerable<PropertyInfo> GetOptionProperties()
        {
            return this.GetType()
                .GetProperties()
                .Where(p => p.IsDefined(typeof(OptionItemAttribute), false))
                .Where(p => p.PropertyType.IsSerializable);
        }
    }
}
