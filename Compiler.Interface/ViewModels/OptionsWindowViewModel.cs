using Compiler.Interface.Properties;
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
    public class OptionsWindowViewModel : BindableBase, IInteractionRequestAware
    {
        private bool reloadFiles;
        public bool ReloadFiles
        {
            get => reloadFiles;
            set => SetProperty(ref reloadFiles, value);
        }

        private bool autoSave;
        public bool AutoSave
        {
            get => autoSave;
            set => SetProperty(ref autoSave, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public OptionsWindowViewModel()
        {
            CancelCommand = new DelegateCommand(CancelCommandExecuted);
            SaveCommand = new DelegateCommand(SaveCommandExecuted);

            ReloadFiles = Settings.Default.ReloadFiles;
            AutoSave = Settings.Default.AutoSave;
        }

        private void SaveCommandExecuted()
        {
            try
            {
                Settings.Default.ReloadFiles = ReloadFiles;
                Settings.Default.AutoSave = AutoSave;

                Settings.Default.Save();

                FinishInteraction();
            }
            catch (Exception ex)
            {
            }
        }

        private void CancelCommandExecuted()
        {
            FinishInteraction();
        }

        #region IInteractionRequestAware
        public INotification Notification { get; set; }
        public Action FinishInteraction { get; set; }
        #endregion
    }
}
