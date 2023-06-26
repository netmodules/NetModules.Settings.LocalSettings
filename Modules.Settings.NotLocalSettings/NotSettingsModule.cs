using System;
using NetModules;
using NetModules.Events;
using NetModules.Interfaces;
using Modules.Settings.LocalSettings;

namespace Modules.Settings.NotLocalSettings
{
    /// <summary>
    /// A module which loads local settings from JSON configuration files with {ModuleName}.*.json, where * is anything such as
    /// if we were loading settings for this module the configuration filename would be
    /// Modules.LocalSettings.SettingsModule.settings.json or
    /// Modules.LocalSettings.SettingsModule.settings.default.json and configuration filename containing
    /// *.default.* is loaded first, where settings are overwritten by any configuration files loaded in sequence.
    /// 
    /// This module loads with the highest priority of short.MaxValue and handles GetSettingEvent with the lowest priority of
    /// short.MinValue, this is so that the module is ready to provide settings to all other modules while they are loading
    /// and if another module is loaded which handles GetSettingEvent, the newly loaded module would take presidence. This is
    /// useful for example, if you would like to load a module with a high priority which loads settings for other modules from
    /// a remote server or database. This module would provide settings to the remote settings module, which would then load and
    /// take over the GetSettingsEvent handling for all other modules."
    /// </summary>
    [Serializable]
    [Module(
        LoadFirst = true, LoadPriority = 0, HandlePriority = short.MaxValue
    )]
    public class NotSettingsModule : Module
    {
        public override bool CanHandle(IEvent e)
        {
            return false;
        }

        public override void Handle(IEvent e)
        {
        }

        public override void OnLoading()
        {
            base.OnLoading();
        }


        public override void OnLoaded()
        {
            base.OnLoaded();
            var settingsModule = Host.Modules.GetModulesByType<SettingsModule>();
            var setting = settingsModule[0].GetSetting("testString", "Not returned");
            Log(LoggingEvent.Severity.Debug, $"{ModuleAttributes.Name} tried to load a setting from settings module and received response: {setting}");
        }


        public override void OnUnloading()
        {
            base.OnUnloading();
        }
    }
}
