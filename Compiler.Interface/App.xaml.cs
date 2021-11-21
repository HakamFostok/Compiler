using System.Windows;
using Compiler.Core;
using NLog;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;

namespace Compiler.Interface;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : PrismApplication
{
    protected override Window CreateShell() =>
        //return null;
        new Views.MainWindow();

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterInstance<ICompiler>(new AubCompiler());
        containerRegistry.RegisterInstance<IEventAggregator>(new EventAggregator());
        containerRegistry.RegisterInstance<NLog.ILogger>(LogManager.GetCurrentClassLogger());
        containerRegistry.RegisterInstance<IFileDialogsService>(new FileDialogsService());
    }
}
