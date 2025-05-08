# NetModules.Settings.LocalSettings

**NetModules.Settings.LocalSettings** is a basic settings module that is built using the [NetModules](https://github.com/netmodules/NetModules) architecture.

It is designed to load and return configuration settings for other modules from a local file if a NetModules module raises a [GetSettingEvent](https://github.com/netmodules/NetModules/blob/main/NetModules/Events/GetSettingEvent.cs) or uses its [GetSetting(settingName, defaultValue)](https://github.com/netmodules/NetModules/blob/master/NetModules/Interfaces/IModule.cs#L71) wrapper method. The settings are stored in a JSON file and loaded using [NetTools.Serialization.Json](https://github.com/netmodules/NetTools.Serialization.Json). NetTools.Serialization.Json allows line comments in JSON so you can add descriptions and documentation, while JSON makes configuration files easy to read and modify.

When added to your project, this module is set to `AutoLoad` and `LoadFirst`, with a high `LoadPriority` and a lowest `HandlePriority`.

This means that the module should be ready to serve required settings to other modules while they are loading, and if another settings provider module is loaded with a higher `HandlePriority`, it will override the settings provided by this module. This allows for a flexible and extensible settings architecture, where multiple modules can provide settings, and the most appropriate one will be used based on priority.

## Features

- **Local Settings:** Store module settings in local JSON files.
- **Event-Driven:** Uses events to retrieve settings, allowing for dynamic configuration.
- **Easy Integration:** Simple to integrate with existing modules.


## Getting Started

### Installation

To use **MetModules.Settings.LocalSettings**, add the library to your module hosting project via NuGet Package Manager:

```bash
Install-Package NetModules.Settings.LocalSettings
```

When you load modules using [ModuleHost.Modules.LoadModules();](https://github.com/netmodules/NetModules/tree/main?tab=readme-ov-file#creating-and-loading-a-module-host), NetModules.Settings.LocalSettings will be loaded and ready to serve settings to other modules.

### Quick Examples

#### Adding a Local Settings File

```csharp
using System;
using NetTools.Logging;

// This is an incredibly simple implementation of the ILogger interface.
public class ConsoleLogger : ILogger
{
    public void Error(params object[] args) => Console.WriteLine($"[ERROR] {string.Join(" ", args)}");
    public void Analytic(params object[] args) => Console.WriteLine($"[ANALYTIC] {string.Join(" ", args)}");
    public void Information(params object[] args) => Console.WriteLine($"[INFO] {string.Join(" ", args)}");
    public void Debug(params object[] args) => Console.WriteLine($"[DEBUG] {string.Join(" ", args)}");
}
```

## Contributing

We welcome contributions! To get involved:
1. Fork [NetModules.Settings.LocalSettings](https://github.com/netmodules/NetModules.Settings.LocalSettings), make improvements, and submit a pull request.
2. Code will be reviewed upon submission.
3. Join discussions via the [issues board](https://github.com/netmodules/NetModules.Settings.LocalSettings/issues).

## License

NetModules.Settings.LocalSettings is licensed under the [MIT License](https://tldrlegal.com/license/mit-license), allowing unrestricted use, modification, and distribution. If you use NetModules.Settings.LocalSettings in your own project, we’d love to hear about your experience, and possibly feature you on our website!

Full documentation coming soon!

[NetModules Foundation](https://netmodules.net/)
