using CommonServiceLocator;
using Compiler.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity.Attributes;

namespace Compiler.Interface.ViewModels
{
    public class WriteEventPubSub : PubSubEvent<WriteEventArgs>
    {
    }

    public class MainWindowViewModel : BindableBase
    {
        [Dependency]
        internal ICompiler compiler { get; set; }

        internal IEventAggregator EventAggregator { get; }

        public InteractionRequest<Notification> AboutWindowInteractionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> OptionsWindowInteractionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> ConsoleWindowInteractionRequest { get; } = new InteractionRequest<Notification>();

        #region StatusBar

        private CompilerStatus status;
        public CompilerStatus Status
        {
            get => status;
            set
            {
                SetProperty(ref status, value);
                RaisePropertyChanged(nameof(StatusDescription));
            }
        }

        public string StatusDescription => Status.ToString();

        public MainWindowViewModel()
        {
            Status = CompilerStatus.Ready;

            BuildCommand = new DelegateCommand(BuildCommandExecuted);
            ExecuteCommand = new DelegateCommand(ExecuteCommandExecuted);
            ExecuteFromObjFileCommand = new DelegateCommand(ExecuteFromObjFileCommandExecuted);
            AboutCommand = new DelegateCommand(AboutCommandExecuted);
            OptionsCommand = new DelegateCommand(OptionsCommandExecuted);
            ExitApplicationCommand = new DelegateCommand(ExitApplicationCommandExecuted);

            EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        private void ExitApplicationCommandExecuted()
        {
            Application.Current.Shutdown();
        }

        #endregion

        private string file;
        public string File
        {
            get => file;
            set => SetProperty(ref file, value);
        }

        public ICommand BuildCommand { get; }
        public ICommand ExecuteCommand { get; }
        public ICommand ExecuteFromObjFileCommand { get; }
        public ICommand AboutCommand { get; }
        public ICommand OptionsCommand { get; }
        public ICommand ExitApplicationCommand { get; }

        private void BuildCommandExecuted()
        {
            try
            {
                compiler.CompileMainProgram("file1");
            }
            catch (Exception ex)
            {

            }
        }

        private void ExecuteCommandExecuted()
        {
            try
            {
                //compiler.CompileMainProgram("file1");
                ConsoleWindowInteractionRequest.Raise(new Notification { Title = "Console" });

                Executer exe = new Executer();

                Action<object, WriteEventArgs> writeHandler = (obj, eve) =>
                {
                    EventAggregator.GetEvent<WriteEventPubSub>().Publish(eve);
                };

                exe.EndOfExecute += new EventHandler<WriteEventArgs>(writeHandler);
                exe.WriteEvent += new EventHandler<WriteEventArgs>(writeHandler);
            }
            catch (Exception ex)
            {

            }
        }

        private void ExecuteFromObjFileCommandExecuted()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
        }

        private void AboutCommandExecuted()
        {
            AboutWindowInteractionRequest.Raise(new Notification { Title = "About" });
        }

        private void OptionsCommandExecuted()
        {
            OptionsWindowInteractionRequest.Raise(new Notification { Title = "Options" });
        }
    }
}
