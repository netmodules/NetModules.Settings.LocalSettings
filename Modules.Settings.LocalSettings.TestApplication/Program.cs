using System;
using System.Threading;
using NetModules;

namespace Modules.Settings.LocalSettings.TestApplication
{
    class Program
    {
        static EventWaitHandle BlockingHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

        static void Main(string[] args)
        {
            ModuleHost host = new BasicModuleHost();
            host.Modules.LoadModules();

            var myModule = host.Modules.GetModulesByType<SettingsModule>();

            if (myModule.Count > 0)
            {
                var testInt = myModule[0].GetSetting("testInt", 0);
                var testString = myModule[0].GetSetting("testString", "This is the default value, not the settings string!");

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
