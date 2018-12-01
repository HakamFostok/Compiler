using Compiler.Core;
using NLog;
using Prism.Events;
using Prism.Ioc;
using Prism.Unity;
using System;
using System.Windows;

namespace Compiler.Interface
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return new Views.MainWindow();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<ICompiler>(new AubCompiler());
            containerRegistry.RegisterInstance<IEventAggregator>(new EventAggregator());
            containerRegistry.RegisterInstance<ILogger>(LogManager.GetCurrentClassLogger());
            containerRegistry.RegisterInstance<IFileDialogsService>(new FileDialogsService());
        }
    }
}
