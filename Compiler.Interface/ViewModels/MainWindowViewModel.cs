using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Compiler.Interface.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
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

        private void BuildCommandExecuted()
        {
            try { }
            catch (Exception ex)
            {

            }
        }

        private void ExecuteCommandExecuted()
        {
            try { }
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
    }
}
