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
    public class MainWindowViewModel : BindableBase
    {
        private bool redoTrigger;
        public bool RedoTrigger
        {
            get => redoTrigger;
            set => SetProperty(ref redoTrigger, value);
        }

        private bool undoTrigger;
        public bool UndoTrigger
        {
            get => undoTrigger;
            set => SetProperty(ref undoTrigger, value);
        }

        private bool clearTextTrigger;
        public bool ClearTextTrigger
        {
            get => clearTextTrigger;
            set => SetProperty(ref clearTextTrigger, value);
        }

        private bool selectTrigger;
        public bool SelectTrigger
        {
            get => selectTrigger;
            set => SetProperty(ref selectTrigger, value);
        }

        private bool deselectTrigger;
        public bool DeselectTrigger
        {
            get => deselectTrigger;
            set => SetProperty(ref deselectTrigger, value);
        }

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

            UndoCommand = new DelegateCommand(UndoCommandExecuted);
            RedoCommand = new DelegateCommand(RedoCommandExecuted);
            SelectAllCommand = new DelegateCommand(SelectAllCommandExecuted);
            DeselectCommand = new DelegateCommand(DeselectCommandExecuted);
            ClearCommand = new DelegateCommand(ClearCommandExecuted);
            CutCommand = new DelegateCommand(CutCommandExecuted);
            CopyCommand = new DelegateCommand(CopyCommandExecuted);
            PasteCommand = new DelegateCommand(PasteCommandExecuted);

            EventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
        }

        private void PasteCommandExecuted()
        {
        }

        private void CopyCommandExecuted()
        {
        }

        private void CutCommandExecuted()
        {
        }

        private void ClearCommandExecuted()
        {
            ClearTextTrigger = true;
        }

        private void DeselectCommandExecuted()
        {
            SelectTrigger = false;
            DeselectTrigger = true;
        }

        private void SelectAllCommandExecuted()
        {
            SelectTrigger = true;
            DeselectTrigger = false;
        }

        private void RedoCommandExecuted()
        {
            RedoTrigger = true;
            RedoTrigger = false;
        }

        private void UndoCommandExecuted()
        {
            UndoTrigger = true;
            UndoTrigger = false;
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

        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand CutCommand { get; }
        public ICommand CopyCommand { get; }
        public ICommand PasteCommand { get; }

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
