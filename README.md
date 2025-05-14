# NetModules.Settings.LocalSettings

**NetModules.Settings.LocalSettings** is a basic settings module that is built using the [NetModules](https://github.com/netmodules/NetModules) architecture.

It is designed to load and return configuration settings for other modules from a local file if a NetModules module raises a [GetSettingEvent](https://github.com/netmodules/NetModules/blob/main/NetModules/Events/GetSettingEvent.cs) or uses its [GetSetting(settingName, defaultValue)](https://github.com/netmodules/NetModules/blob/master/NetModules/Interfaces/IModule.cs#L71) wrapper method. The settings are stored in a JSON file and loaded using [NetTools.Serialization.Json](https://github.com/netmodules/NetTools.Serialization.Json). NetTools.Serialization.Json allows line comments in JSON so you can add descriptions and documentation, while JSON makes configuration files easy to read and modify.

> [!NOTE]  
> A Module doesn't need to worry about how settings are provided unless it's a settings provider Module!

When added to your project, this module is attributed with `AutoLoad` and `LoadFirst`, a high `LoadPriority` and a lowest `HandlePriority`.

See [NetModules.Interfaces.IModuleAttribute](https://github.com/netmodules/NetModules/blob/main/NetModules/Interfaces/IModuleAttribute.cs) and [NetModules.Attributes.ModuleAttribute](https://github.com/netmodules/NetModules/blob/main/NetModules/Attributes/ModuleAttribute.cs) for detailed explanation regarding module attributes.

This means that the module should be ready to serve required settings to other modules while they are loading, and if another settings provider module is loaded with a higher `HandlePriority`, it will override the settings provided by this module. This allows for a flexible and extensible settings architecture, where multiple modules can provide settings, and the most appropriate module will be used based on priority.

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

## Quick Examples

### Create a Default Settings File  

If you are implementing user defined settings in your own module, and would like to add support for NetModules.Settings.LocalSettings, you should include a default settings file with your module. This file will be used to create a new settings file if one does not exist. The default settings file should be named `settings.json` and placed in the root of your module project.

```json
{
  // Comments are supported to explain your default settings.
  "mySetting": "Default Value",
  "myOtherSetting": 42
}
```

you can request a setting from your module using the `GetSetting` method:

```csharp
var mySetting = this.GetSetting("mySetting", "Default Value");
```

If you are packaging your module for distribution, you should include the `settings` file in your NuGet package. This will ensure that the default settings file is included when your module is installed. You can do this by adding the file to your `nuspec` or `csproj` file.

```xml
<ItemGroup>
    <None Update="ExampleModuleNamespace.ExampleModuleName.settings.default.json">
      <CopyToOutputDirectory>Always</CopyToOutputDirectory>
	  <Pack>true</Pack>
	  <PackageCopyToOutput>true</PackageCopyToOutput>
    </None>
  </ItemGroup>
```

### Overriding a Local Default Settings File

If you wish to override the default settings file with a local settings file, you can create a `settings` file in the root of your module host project. This file will be used to override the default settings file when the module is loaded.

E.g. `ExampleModuleNamespace.ExampleModuleName.settings.json`

```json
{
  "mySetting": "Overridden Value",
  "myOtherSetting": 100
}
```

### Handling Secure Settings

Add sensitive settings to a `"secureSettings"` array in your `settings` file:

```json
{
  "apiKey": "Super-Secret-Key",
  "secureSettings": ["apiKey"]
}
```

>[!TIP]
>- Always include a default settings file to document available configurations.
>- Use secure settings for sensitive data to help restrict unauthorized access if needed.
>- Consider alternative settings storage/retrieval solutions (e.g. Remote database, secure/cloud storage, etc...).
>- For more reference on implementation, refer to the other [NetModules repositories](https://github.com/orgs/netmodules/repositories).


## Contributing

We welcome contributions! To get involved:
1. Fork [NetModules.Settings.LocalSettings](https://github.com/netmodules/NetModules.Settings.LocalSettings), make improvements, and submit a pull request.
2. Code will be reviewed upon submission.
3. Join discussions via the [issues board](https://github.com/netmodules/NetModules.Settings.LocalSettings/issues).

## License

NetModules.Settings.LocalSettings is licensed under the [MIT License](https://tldrlegal.com/license/mit-license), allowing unrestricted use, modification, and distribution. If you use NetModules.Settings.LocalSettings in your own project, we’d love to hear about your experience, and possibly feature you on our website!

Full documentation coming soon!

[NetModules Foundation](https://netmodules.net/)
