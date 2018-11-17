using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        #endregion

        private string file;
        public string File
        {
            get => file;
            set => SetProperty(ref file, value);
        }
    }
}
