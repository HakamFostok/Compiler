using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Interface.ViewModels
{
    public class StatusBarViewModel : BaseViewModel
    {
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

        public StatusBarViewModel()
        {
            Status = CompilerStatus.Ready;
        }
    }
}
