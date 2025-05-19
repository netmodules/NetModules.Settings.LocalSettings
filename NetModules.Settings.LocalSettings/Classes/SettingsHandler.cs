using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetModules;
using NetTools;
using NetTools.Serialization;

namespace NetModules.Settings.LocalSettings.Classes
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

            LoadJsonSettingsForKnownModules();
        }


        /// <summary>
        /// 
        /// </summary>
        internal void LoadJsonSettingsForKnownModules()
        {
            // This should return a list of all module names known to this.Host whether they
            // are loaded or not.
            var moduleNames = Module.Host.Modules.GetModuleNames().Select(m => m.ToString());
            var files = Directory.GetFiles(Module.Host.WorkingDirectory.LocalPath, "*.json", SearchOption.AllDirectories);

            if (files == null || files.Length == 0)
            {
                return;
            }

            foreach (var m in moduleNames)
            {
                // Get the setting files json for the individual module and load them into the dictionary by order of default first.
                var settings = files.Where(f => Path.GetFileNameWithoutExtension(f).StartsWith(m, StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(f => f.IndexOf(".default.", StringComparison.OrdinalIgnoreCase) > -1);

                if (settings == null || settings.Count() == 0)
                {
                    continue;
                }

                foreach (var f in settings)
                {
                    var json = LoadResourceAsString(f);

                    // Strip any comments and whitespace from the JSON object and converts the JSON settings file to a
                    // dictionary using NetTools.Serialization.Json extension method.
                    var moduleSettings = json.MinifyJson().ToDictionary();

                    if (moduleSettings == null)
                    {
                        var message = $"Unable to read settings file for module. If this is a settings file, it may contain invalid characters or malformed JSON.";

                        if (System.Diagnostics.Debugger.IsAttached)
                        {
                            Module.Log(Events.LoggingEvent.Severity.Error, message, f.ToString());
                            continue;
                        }

                        Module.Log(Events.LoggingEvent.Severity.Trace, message, f.ToString());
                        continue;
                    }

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


        /// <summary>
        /// 
        /// </summary>
        internal object GetSetting(ModuleName moduleName, string settingName, out bool hasSetting)
        {
            if (ModuleSettings.TryGetValue(moduleName, out var settings)
                && settings.TryGetValue(settingName, out object value))
            {
                hasSetting = true;

                // Placing setting keys within a "secureSettings" array is a way to prevent other modules
                // from reading the setting value unless they are part of the module's loading process.
                // This adds an additional level of security to the settings module and prevents other modules
                // from accessing sensitive information.
                if (settings.TryGetValue("secureSettings", out var secureSettings)
                    && secureSettings is List<object> secure
                    && secure.Any(x => x.Equals(settingName)))
                {
                    // We use the current stacktrace string instead of new System.Diagnostics.StackTrace() so
                    // that we don't need to mess around with trying to get [External Code] entries...
                    var callstack = Environment.StackTrace
                        .Split(nameof(Module.Handle))
                        .SelectMany(s => s.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('.'))).ToArray();
                    var callingModules = Module.Host.Modules.GetModuleNames().Where(x => x != moduleName && x != Module.ModuleAttributes.Name).Where(x => callstack.Any(y => y.Contains(x, StringComparison.OrdinalIgnoreCase))).ToArray();
                    
                    if (callingModules.Length > 0)
                    {
                        var message = $"The setting with name {settingName} is included in the {moduleName} module's secure settings and cannot be read by or as part of a chain with {callingModules[0]}.";
                        
                        Module.Log(Events.LoggingEvent.Severity.Error, message);
                        return null;
                    }
                    else if (!Module.Host.Arguments.Contains("no-secure-settings")
                        && !callstack.Any(x => x.Equals($"{moduleName}.{nameof(Module.OnLoading)}()", StringComparison.OrdinalIgnoreCase)
                        || x.Equals($"{moduleName}.{nameof(Module.OnLoaded)}()", StringComparison.OrdinalIgnoreCase)
                        || x.Equals($"{moduleName}.{nameof(Module.OnAllModulesLoaded)}()", StringComparison.OrdinalIgnoreCase)))
                    {
                        var message = $"The setting with name {settingName} is included in the {moduleName} module's secure settings and can only be read during the {moduleName} module's {nameof(Module.OnLoading)}, {nameof(Module.OnLoaded)}, and/or {nameof(Module.OnAllModulesLoaded)} methods.";
                        
                        Module.Log(Events.LoggingEvent.Severity.Error, message);
                        return null;
                    }
                }

                return value;
            }

            hasSetting = false;
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        internal string LoadResourceAsString(params string[] path)
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
