using Avalonia;
using System;

namespace ECommerce.UI;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {

        // UNCOMMENT TO RUN BACKEND INTEGRATION TESTS: DO NOT DELETE @AMJADIDDY
        // IntegrationTester.RunTests();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
