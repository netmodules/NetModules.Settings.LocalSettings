using reblGreen.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace reblGreen.NetCore.Modules.LocalSettings.Classes
{
    [Serializable]
    internal class SettingsHandler
    {
        Module Module;
        Dictionary<string, Dictionary<string, object>> ModuleSettings;


        /// <summary>
        /// 
        /// </summary>
        internal SettingsHandler(Module module)
        {
            Module = module;
            ModuleSettings = new Dictionary<string, Dictionary<string, object>>();
        }


        /// <summary>
        /// 
        /// </summary>
        internal void LoadJsonSettingsForKnownModules()
        {
            var moduleNames = Module.Host.Modules.GetModuleNames();
            var files = Directory.GetFiles(Module.Host.WorkingDirectory.LocalPath, "*.json", SearchOption.AllDirectories);

            files = files.Where(f => moduleNames.Any(m => Path.GetFileNameWithoutExtension(f).ToLower().StartsWith(m.ToString().ToLower()))).ToArray();

            foreach (var m in moduleNames)
            {
                var settings = files.Where(f => Path.GetFileNameWithoutExtension(f).ToLower().StartsWith(m.ToString().ToLower()))
                    .OrderByDescending(f => f.IndexOf(".default.", StringComparison.OrdinalIgnoreCase) > -1);

                if (settings != null && settings.Count() > 0)
                {
                    foreach (var f in settings)
                    {
                        var json = LoadResourceAsString(f);

                        // Strips any comments and whitespace from the JSON object and converts the JSON settings file to a
                        // dictionary using reblGreen.Serialization.Json extension method.
                        var moduleSettings = json.MinifyJson().ToDictionary();

                        if (ModuleSettings.ContainsKey(m))
                        {
                            foreach(var kv in moduleSettings)
                            {
                                // We check here to see if the setting already exists and if it does then we update it,
                                // otherwise we add it to the settings for the current module
                                if(ModuleSettings[m].ContainsKey(kv.Key))
                                {
                                    ModuleSettings[m][kv.Key] = kv.Value;
                                }
                                else
                                {
                                    // No existing setting to update so just add it as a new setting...
                                    ModuleSettings[m].Add(kv.Key, kv.Value);
                                }
                            }
                        }
                        else
                        {
                            // There are no other settings for the current module name so we can just add the settings here,
                            // no need to merge...
                            ModuleSettings.Add(m, moduleSettings);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        internal object GetSetting(ModuleName moduleName, string settingName)
        {
            if (ModuleSettings.ContainsKey(moduleName))
            {
                if (ModuleSettings[moduleName].TryGetValue(settingName, out object value))
                {
                    return value;
                }
            }

            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public string LoadResourceAsString(string resource)
        {
            var path = Path.Combine(Module.WorkingDirectory.LocalPath, resource);

            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }

            return null;
        }
    }
}
