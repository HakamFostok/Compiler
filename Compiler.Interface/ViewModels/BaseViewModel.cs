using NLog;
using Prism.Events;
using Prism.Mvvm;
using System;
using Unity.Attributes;

namespace Compiler.Interface.ViewModels
{
    public abstract class BaseViewModel : BindableBase
    {
        [Dependency]
        protected ILogger Logger { get; set; }

        protected IEventAggregator EventAggregator { get; }

        public BaseViewModel()
        {
            EventAggregator = CommonServiceLocator.ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        protected void HandleException(Exception ex)
        {
            Logger.Error(ex);
        }
    }
}
