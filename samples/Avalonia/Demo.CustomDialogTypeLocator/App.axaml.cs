using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Demo.Avalonia.CustomDialogTypeLocator.ComponentA;
using HanumanInstitute.MvvmDialogs;
using HanumanInstitute.MvvmDialogs.Avalonia;
using Microsoft.Extensions.Logging;
using Splat;

namespace Demo.Avalonia.CustomDialogTypeLocator;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var build = Locator.CurrentMutable;
        var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter(logLevel => true).AddDebug());

        build.RegisterLazySingleton(() => (IDialogService)new DialogService(
            new DialogManager(
                viewLocator: new ViewLocator(),
                logger: loggerFactory.CreateLogger<DialogManager>()),
            viewModelFactory: x => Locator.Current.GetService(x)));

        SplatRegistrations.Register<MainWindowVm>();
        SplatRegistrations.Register<MyDialogVM>();
        SplatRegistrations.SetupIOC();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        GC.KeepAlive(typeof(DialogService));
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = MainWindow
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public static MainWindowVm MainWindow => Locator.Current.GetService<MainWindowVm>()!;
    public static IDialogService DialogService => Locator.Current.GetService<IDialogService>()!;
}
