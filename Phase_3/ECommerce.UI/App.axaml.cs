using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ECommerce.UI.Views;

namespace ECommerce.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Console.WriteLine("App starting...");

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            Console.WriteLine("Creating LoginWindow...");
            desktop.MainWindow = new LoginWindow();
            Console.WriteLine("LoginWindow created!");
        }

        Console.WriteLine("Calling base method...");
        base.OnFrameworkInitializationCompleted();
        Console.WriteLine("App started!");
    }
}