using System;
using System.Threading;
using NetModules;

namespace NetModules.Settings.LocalSettings.TestApplication
{
    class Program
    {
        static EventWaitHandle BlockingHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static void Main(string[] args)
        {
            ModuleHost host = new BasicModuleHost(new string[] { "test-settings-module" });
            host.Modules.LoadModules();

            var myModule = host.Modules.GetModulesByType<SettingsModule>();

            if (myModule.Count > 0)
            {
                var testInt = myModule[0].GetSetting("testInt", 0);

                // This setting can not be read here because it is in the settings module's secureSettings array
                // in the example settings JSON file in this project.
                var testString = myModule[0].GetSetting("testString", "This is the default value, not the settings value!");


                if (testInt == 0)
                {
                    Console.WriteLine("Settings not loaded! testInt is {0}, which is the default value.", testInt);
                    Console.WriteLine("The value of testString is {0}, which is the default value.", testString);
                }
                else
                {
                    Console.WriteLine("The setting value for testInt was set to {0}. Printing the testString value {1} times", testInt, testInt);
                    for (var i = 0; i < testInt; i++)
                    {
                        Console.WriteLine(testString);
                    }
                }
            }

            BlockingHandle.WaitOne();
        }
    }
}
