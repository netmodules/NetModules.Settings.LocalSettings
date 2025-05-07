using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetModules.Settings.LocalSettings.Classes
{
    internal class Constants
    {
        internal const string ModuleDescription = @"A basic settings module that loads local configuration files into memory.
 This module handles the NetModules.Events.GetSettingEvent and configuration files must be JSON object formatted
 with a filename which starts with the module's namespace followed by the module's name and ends with a .json extension.
 Multiple configuration files can exist per module and settings will be merged and overwritten on a key/value basis.
 See AdditionalInformation.";

        internal const string ModuleAdditional1 = @"An example of a configuration file naming convension: NetModules.LocalSettings.SettingsModule.settings.json
 or NetModules.LocalSettings.SettingsModule.settings.default.json where a configuration file containing
 *.default.* would be loaded into memory first and any other configuration files for this module would be loaded and
 merged with the default settings, replacing any pre-existing setting.";

        internal const string ModuleAdditional2 = @"While it is not part of JSON architecture, it is possible to add single line and multiline comments to your configuration
 files and these comments will be stripped from the configuration file using NetTools.Serialization.Json.MinifyJson()
 before parsing the JSON object into memory using NetTools.Serialization.Json.ToDictionary() string extension method.";
        
        internal const string ModuleAdditional3 = @"This module loads with the highest priority of short.MaxValue and handles GetSettingEvent with the lowest priority of
 short.MinValue, this is so that the module is ready to provide settings to all other modules while they are loading
 and if another module is loaded which handles GetSettingEvent, the newly loaded module would take presidence. This is
 useful for example, if you would like to load a module with a high priority which loads settings for other modules from
 a remote server or database. This module would provide settings to the remote settings module, which would then load and
 take over the GetSettingsEvent handling for all other modules.";
    }
}
