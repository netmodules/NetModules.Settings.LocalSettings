using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetModules;
using NetTools;
using NetTools.Serialization;

namespace Modules.Settings.LocalSettings.Classes
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
            var moduleNames = Module.Host.Modules.GetModuleNames().Select(m => m.ToString());
            var files = Directory.GetFiles(Module.Host.WorkingDirectory.LocalPath, "*.json", SearchOption.AllDirectories);

            files = files.Where(f => moduleNames.Any(m => Path.GetFileNameWithoutExtension(f).StartsWith(m, StringComparison.OrdinalIgnoreCase))).ToArray();

            foreach (var m in moduleNames)
            {
                // Get the setting files json for the individual module and load them into the dictionary by order of default first.
                var settings = files.Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(m, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => f.IndexOf(".default.", StringComparison.OrdinalIgnoreCase) > -1);

                if (settings != null && settings.Count() > 0)
                {
                    foreach (var f in settings)
                    {
                        var json = LoadResourceAsString(f);

                        // Strips any comments and whitespace from the JSON object and converts the JSON settings file to a
                        // dictionary using NetTools.Serialization.Json extension method.
                        var moduleSettings = json.MinifyJson().ToDictionary();

                        if (ModuleSettings.ContainsKey(m))
                        {
                            foreach(var kv in moduleSettings)
                            {
                                // We check here to see if the setting already exists and if it does then we update it,
                                // otherwise we add it to the settings for the current module
                                if (ModuleSettings[m].ContainsKey(kv.Key))
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
        internal object GetSetting(ModuleName moduleName, string settingName, out bool hasSetting)
        {
            if (ModuleSettings.ContainsKey(moduleName))
            {
                if (ModuleSettings[moduleName].TryGetValue(settingName, out object value))
                {
                    hasSetting = true;
                    return value;
                }
            }

            hasSetting = false;
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        public string LoadResourceAsString(params string[] path)
        {
            string file;

            if (path.Length > 1)
            {
                file = string.Join(Path.DirectorySeparatorChar, path);
            }
            else
            {
                file = path[0];
            }

            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(Module.WorkingDirectory.LocalPath, file);
            }

            if (File.Exists(file))
            {
                return File.ReadAllText(file);
            }

            return null;
        }
    }
}
