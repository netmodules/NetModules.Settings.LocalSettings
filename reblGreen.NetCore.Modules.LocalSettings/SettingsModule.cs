using System;
using reblGreen.NetCore.Modules.Events;
using reblGreen.NetCore.Modules.Interfaces;
using reblGreen.NetCore.Modules.LocalSettings.Classes;

namespace reblGreen.NetCore.Modules.LocalSettings
{
    /// <summary>
    /// A module which loads local settings from JSON configuration files with {ModuleName}.*.json, where * is anything such as
    /// if we were loading settings for this module the configuration filename would be
    /// reblGreen.NetCore.Modules.LocalSettings.SettingsModule.settings.json or
    /// reblGreen.NetCore.Modules.LocalSettings.SettingsModule.settings.default.json and configuration filename containing
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
        LoadFirst = true, LoadPriority = short.MinValue, HandlePriority = short.MaxValue,
        Description = "A basic settings module which loads local configuration files into memory. This module handles "
        + "the reblGreen.NetCore.Modules.Events.GetSettingEvent and configuration files must be JSON object formatted "
        + "with a filename which starts with the module's name and has a .json extension. Multiple configuration files "
        + "can exist per module and settings will be merged and overwritten on a key/value basis. See AdditionalInformation.",
        AdditionalInformation = new string[]
        {
            "An example of a configuration file naming convension: reblGreen.NetCore.Modules.LocalSettings.SettingsModule.settings.json "
            + "or reblGreen.NetCore.Modules.LocalSettings.SettingsModule.settings.default.json where a configuration file containing "
            + "*.default.* would be loaded into memory first and any other configuration files for this module would be loaded and "
            + "merged with the default settings, replacing any pre-existing setting.",
            "While it is not part of JSON architecture, it is possible to add single line and multiline comments to your configuration "
            + "files and these comments will be stripped from the configuration file using reblGreen.Serialization.Json.MinifyJson() "
            + "before parsing the JSON object into memory using reblGreen.Serialization.Json.ToDictionary() string extension method.",
            "This module loads with the highest priority of short.MaxValue and handles GetSettingEvent with the lowest priority of "
            + "short.MinValue, this is so that the module is ready to provide settings to all other modules while they are loading "
            + "and if another module is loaded which handles GetSettingEvent, the newly loaded module would take presidence. This is "
            + "useful for example, if you would like to load a module with a high priority which loads settings for other modules from "
            + "a remote server or database. This module would provide settings to the remote settings module, which would then load and "
            + "take over the GetSettingsEvent handling for all other modules."
        }
    )]
    public class SettingsModule : Module
    {
        SettingsHandler SettingsHandler;

        public override bool CanHandle(IEvent e)
        {
            if (e is GetSettingEvent)
            {
                return true;
            }

            return false;
        }

        public override void Handle(IEvent e)
        {
            if (e is GetSettingEvent @event)
            {
                if (!string.IsNullOrWhiteSpace(@event.Input.ModuleName)
                    && !string.IsNullOrWhiteSpace(@event.Input.SettingName))
                {
                    var setting = SettingsHandler.GetSetting(@event.Input.ModuleName, @event.Input.SettingName, out bool hasSetting);

                    if (hasSetting)
                    {
                        @event.Output = new GetSettingEventOutput()
                        {
                            Setting = setting
                        };
                        @event.Handled = true;
                    }
                    else
                    {
                        var message = string.Format("Setting with name {0} not found for module {1}"
                            , @event.Input.SettingName, @event.Input.ModuleName);

                        Log(LoggingEvent.Severity.Debug, message);

                        @event.SetMetaValue("reblGreen.NetCore.Modules.LocalSettings.SettingsModule"
                            , string.Format(message));
                    }
                }
            }
        }

        public override void OnLoading()
        {
            SettingsHandler = new SettingsHandler(this);
            SettingsHandler.LoadJsonSettingsForKnownModules();
            base.OnLoading();
        }


        public override void OnLoaded()
        {
            base.OnLoaded();
        }


        public override void OnUnloading()
        {
            base.OnUnloading();
        }
    }
}
