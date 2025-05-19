using System;
using System.Collections.Generic;
using NetModules;
using NetModules.Events;
using NetModules.Interfaces;
using NetModules.Settings.LocalSettings.Classes;

namespace NetModules.Settings.LocalSettings
{
    /// <summary>
    /// A module that loads local settings from JSON configuration files with {ModuleName}.*.json, where * is anything such as
    /// if we were loading settings for this module the configuration filename would be
    /// NetModules.LocalSettings.SettingsModule.settings.json or
    /// NetModules.LocalSettings.SettingsModule.settings.default.json and configuration filename containing
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
        LoadFirst = true,
        LoadPriority = short.MinValue,
        HandlePriority = short.MaxValue,
        Description = Constants.ModuleDescription,
        AdditionalInformation = new string[]
        {
            Constants.ModuleAdditional1,
            Constants.ModuleAdditional2,
            Constants.ModuleAdditional3,
        }
    )]
    public class SettingsModule : Module
    {
        SettingsHandler SettingsHandler;

        /// <summary>
        /// This is used to store log messages until the module is fully loaded. A precautionary
        /// measure to prevent any log messages from being lost during the loading process due to
        /// possible cyclic dependency where a logging module may rely on a settings module.
        /// </summary>
        List<Tuple<LoggingEvent.Severity, object[]>> TempLog;


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Log(LoggingEvent.Severity severity, params object[] arguments)
        {
            if (TempLog != null)
            {
                TempLog.Add(new Tuple<LoggingEvent.Severity, object[]>(LoggingEvent.Severity.Error, arguments));
                return;
            }
            
            base.Log(severity, arguments);
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanHandle(IEvent e)
        {
            if (e is GetSettingEvent)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Handle(IEvent e)
        {
            if (e is GetSettingEvent getSetting)
            {
                if (!string.IsNullOrWhiteSpace(getSetting.Input.ModuleName)
                    && !string.IsNullOrWhiteSpace(getSetting.Input.SettingName))
                {
                    var setting = SettingsHandler.GetSetting(getSetting.Input.ModuleName
                        , getSetting.Input.SettingName
                        , out bool hasSetting);

                    if (hasSetting)
                    {
                        getSetting.Output = new GetSettingEventOutput()
                        {
                            Setting = setting
                        };

                        getSetting.Handled = true;
                    }
                    else
                    {
                        var message = string.Format("Setting with name {0} not found for module {1}"
                            , getSetting.Input.SettingName, getSetting.Input.ModuleName.ToString());

                        Log(LoggingEvent.Severity.Trace, message);

                        getSetting.SetMetaValue("NetModules.LocalSettings.SettingsModule"
                            , string.Format(message));
                    }
                }
            }
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnLoading()
        {
            if (TempLog == null)
            {
                TempLog = new List<Tuple<LoggingEvent.Severity, object[]>>();
            }

            SettingsHandler = new SettingsHandler(this);
            base.OnLoading();
        }


        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void OnLoaded()
        {
            FlushTempLog();
            base.OnLoaded();
        }


        void FlushTempLog()
        {
            var logs = TempLog;
            TempLog = null;

            if (logs == null)
            {
                return;
            }

            foreach (var log in logs)
            {
                Log(log.Item1, log.Item2);
            } 
        }
    }
}
