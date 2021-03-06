﻿using NLog;
using Prism.Events;
using Prism.Mvvm;
using System;
using Unity.Attributes;

namespace Compiler.Interface.ViewModels
{
    public abstract class BaseViewModel : BindableBase
    {
        private string errorMessage;
        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }

        [Dependency]
        protected IFileDialogsService DialogService { get; set; }

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

            ErrorMessage = string.Empty;    // this is a workaround
            ErrorMessage = ex.Message;
        }
    }
}
