using Compiler.Core;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity.Attributes;

namespace Compiler.Interface.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        [Dependency]
        internal ICompiler compiler { get; set; }

        public InteractionRequest<Notification> AboutWindowInteractionRequest { get; } = new InteractionRequest<Notification>();
        public InteractionRequest<Notification> OptionsWindowInteractionRequest { get; } = new InteractionRequest<Notification>();

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
                compiler.CompileMainProgram("file1");
            }
            catch (Exception ex)
            {

            }
        }

        private void ExecuteFromObjFileCommandExecuted()
        {
            try { }
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
