using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Compiler.Interface.ViewModels
{
    public class FilesViewModel : BaseViewModel
    {
        private ObservableCollection<AubFile> files;
        public ObservableCollection<AubFile> Files
        {
            get => files;
            set => SetProperty(ref files, value);
        }

        private AubFile selectedFile;
        public AubFile SelectedFile
        {
            get => selectedFile;
            set
            {
                SetProperty(ref selectedFile, value);
                //GetEvent<SelectedFileChanged>().Publish(value);
            }
        }

        public ICommand NextTabCommand { get; }

        public FilesViewModel()
        {
            Files = new ObservableCollection<AubFile>();
            NextTabCommand = new DelegateCommand(NextTabCommandExecuted, () => SelectedFile != null && Files.Count > 1).ObservesProperty(() => SelectedFile).ObservesProperty(() => Files);
        }

        private void NextTabCommandExecuted()
        {
            if (SelectedFile == null || Files.Count <= 1)
                return;

            try
            {
                int index = Files.IndexOf(SelectedFile);

                int nextTabIndex = index + 1;

                if (nextTabIndex == Files.Count)
                    nextTabIndex = 0;

                SelectedFile = Files[nextTabIndex];
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }
    }
}
