using System;
using System.Threading;

namespace reblGreen.NetCore.Modules.LocalSettings.TestApplication
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
                var testString = myModule[0].GetSetting("testString", "This is the default value, not the test string!");
                Console.WriteLine("printing the testString value {0} times", testInt);
                for (var i = 0; i < testInt; i++)
                {
                    Console.WriteLine(testString);
                }
            }

            BlockingHandle.WaitOne();
        }
    }
}
