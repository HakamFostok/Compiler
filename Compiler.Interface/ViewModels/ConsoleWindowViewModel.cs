using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler.Interface.ViewModels
{
    public class ConsoleWindowViewModel : BindableBase
    {
        private string consoleResult;
        public string ConsoleResult
        {
            get => consoleResult;
            set => SetProperty(ref consoleResult, value);
        }
    }
}
