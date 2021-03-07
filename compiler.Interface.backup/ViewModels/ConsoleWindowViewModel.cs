using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Windows.Input;

namespace Compiler.Interface.ViewModels
{
    public class ConsoleWindowViewModel : BaseViewModel, IInteractionRequestAware
    {
        private bool isExecutionEnded;
        public bool IsExecutionEnded
        {
            get => isExecutionEnded;
            set => SetProperty(ref isExecutionEnded, value);
        }

        private string consoleResult;
        public string ConsoleResult
        {
            get => consoleResult;
            set => SetProperty(ref consoleResult, value);
        }

        public DelegateCommand<KeyEventArgs> KeyUpEventCommand { get; }

        public ConsoleWindowViewModel()
        {
            KeyUpEventCommand = new DelegateCommand<KeyEventArgs>(KeyUpEventCommandExecuted);
           
            EventAggregator.GetEvent<WriteEventPubSub>().Subscribe(e =>
            {
                Write(e.Line, e.IsLine);
                IsExecutionEnded = e.EndOfExecution;
            });
        }

        private void Write(string data, bool isLine)
        {
            ConsoleResult += data + (isLine ? "\n" : "");
        }

        private void ClearScreen()
        {
            ConsoleResult = string.Empty;
        }

        private void KeyUpEventCommandExecuted(KeyEventArgs obj)
        {
            if (isExecutionEnded == true && obj.Key == Key.Enter)
            {
                ClearScreen();
                FinishInteraction();
            }
        }

        #region IInteractionRequestAware

        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }
        
        #endregion
    }
}
