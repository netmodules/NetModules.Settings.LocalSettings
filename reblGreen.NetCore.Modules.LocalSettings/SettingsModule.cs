using System;
using reblGreen.NetCore.Modules.Events;
using reblGreen.NetCore.Modules.Interfaces;
using reblGreen.NetCore.Modules.LocalSettings.Classes;

namespace reblGreen.NetCore.Modules.LocalSettings
{
    [Serializable]
    [Module(
        LoadPriority = 1000, HandlePriority = -1000,
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
            + "before parsing the JSON object into memory using reblGreen.Serialization.Json.ToDictionary() string extension method."
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
                if (@event.Input != null
                    && !string.IsNullOrWhiteSpace(@event.Input.ModuleName)
                    && !string.IsNullOrWhiteSpace(@event.Input.SettingName))
                {
                    var setting = SettingsHandler.GetSetting(@event.Input.ModuleName, @event.Input.SettingName);

                    if (setting != null)
                    {
                        @event.Output = new GetSettingEventOutput();
                        @event.Output.Setting = setting;
                        @event.Handled = true;
                    }
                    else
                    {
                        @event.SetMetaValue("reblGreen.NetCore.Modules.LocalSettings.SettingsModule"
                            , string.Format("Setting with name {0} not found for module {1}"
                            , @event.Input.SettingName, @event.Input.ModuleName));
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
