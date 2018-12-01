using CommonServiceLocator;
using Compiler.Core;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity.Attributes;

namespace Compiler.Interface.ViewModels
{
    public class AubFile : BindableBase
    {
        public string FileName => Path.GetFileName(FilePath);

        private string filePath;
        public string FilePath
        {
            get => filePath;
            set
            {
                SetProperty(ref filePath, value);
                RaisePropertyChanged(nameof(FileName));
            }
        }

        private string content;
        public string Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        private bool cutTrigger;
        public bool CutTrigger
        {
            get => cutTrigger;
            set => SetProperty(ref cutTrigger, value);
        }

        private bool copyTrigger;
        public bool CopyTrigger
        {
            get => copyTrigger;
            set => SetProperty(ref copyTrigger, value);
        }

        private bool pasteTrigger;
        public bool PasteTrigger
        {
            get => pasteTrigger;
            set => SetProperty(ref pasteTrigger, value);
        }

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

        public AubFile(string path)
        {
            this.FilePath = path;
            this.Content = string.Empty;
        }
    }

    public class MainWindowViewModel : BaseViewModel
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
            set => SetProperty(ref selectedFile, value);
        }

        [Dependency]
        internal ICompiler Compiler { get; set; }

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
            Files = new ObservableCollection<AubFile>();

            BuildCommand = new DelegateCommand(BuildCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            ExecuteCommand = new DelegateCommand(ExecuteCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            ExecuteFromObjFileCommand = new DelegateCommand(ExecuteFromObjFileCommandExecuted);

            AboutCommand = new DelegateCommand(AboutCommandExecuted);
            OptionsCommand = new DelegateCommand(OptionsCommandExecuted);
            ExitApplicationCommand = new DelegateCommand(ExitApplicationCommandExecuted);

            UndoCommand = new DelegateCommand(UndoCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            RedoCommand = new DelegateCommand(RedoCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            SelectAllCommand = new DelegateCommand(SelectAllCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            DeselectCommand = new DelegateCommand(DeselectCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            ClearCommand = new DelegateCommand(ClearCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            CutCommand = new DelegateCommand(CutCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            CopyCommand = new DelegateCommand(CopyCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            PasteCommand = new DelegateCommand(PasteCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);

            NewFileCommand = new DelegateCommand(NewFileCommandExecuted);
            OpenFileCommand = new DelegateCommand(OpenFileCommandExecuted);
            CloseFileCommand = new DelegateCommand(CloseFileCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            CloseAllFilesCommand = new DelegateCommand(CloseAllFilesCommandExecuted);
            SaveFileCommand = new DelegateCommand(SaveFileCommandExecuted);
            SaveAllFilesCommand = new DelegateCommand(SaveAllFilesCommandExecuted);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecuted);
            PrintFileCommand = new DelegateCommand(PrintFileCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
        }

        private bool IsSelectedFile() => SelectedFile != null;

        private void NewFileCommandExecuted()
        {
            try
            {
                string filePath = DialogService.SaveFileDialog();
                if (string.IsNullOrEmpty(filePath))
                    return;

                AubFile newFile = new AubFile(filePath);
                Files.Add(newFile);
                SelectedFile = newFile;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void OpenFileCommandExecuted()
        {
            try
            {
                string filePath = DialogService.OpenFileDialog();
                if (string.IsNullOrEmpty(filePath))
                    return;

                AubFile newFile = new AubFile(filePath);
                newFile.Content = File.ReadAllText(filePath);
                Files.Add(newFile);
                SelectedFile = newFile;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void CloseFileCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            try
            {
                Files.Remove(SelectedFile);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void CloseAllFilesCommandExecuted()
        {
            try
            {
                Files.Clear();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void SaveFileCommandExecuted()
        {
            throw new NotImplementedException();
        }

        private void SaveAllFilesCommandExecuted()
        {
            throw new NotImplementedException();
        }

        private void SaveAsCommandExecuted()
        {
            throw new NotImplementedException();
        }

        private void PrintFileCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            try
            {
                DialogService.PrintFileDialog();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void PasteCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.PasteTrigger = true;
            SelectedFile.PasteTrigger = false;
        }

        private void CopyCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.CopyTrigger = true;
            SelectedFile.CopyTrigger = false;
        }

        private void CutCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.CutTrigger = true;
            SelectedFile.CutTrigger = false;
        }

        private void ClearCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.ClearTextTrigger = true;
        }

        private void DeselectCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.DeselectTrigger = true;
        }

        private void SelectAllCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.SelectTrigger = true;
        }

        private void RedoCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.RedoTrigger = true;
            SelectedFile.RedoTrigger = false;
        }

        private void UndoCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            SelectedFile.UndoTrigger = true;
            SelectedFile.UndoTrigger = false;
        }

        private void ExitApplicationCommandExecuted()
        {
            Application.Current.Shutdown();
        }

        #endregion

        public ICommand NewFileCommand { get; }
        public ICommand OpenFileCommand { get; }
        public ICommand CloseFileCommand { get; }
        public ICommand CloseAllFilesCommand { get; }

        public ICommand SaveFileCommand { get; }
        public ICommand SaveAllFilesCommand { get; }
        public ICommand SaveAsCommand { get; }

        public ICommand PrintFileCommand { get; }

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
                Compiler.CompileMainProgram("file1");
            }
            catch (Exception ex)
            {
                HandleException(ex);
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
                HandleException(ex);
            }
        }

        private void ExecuteFromObjFileCommandExecuted()
        {
            try
            {

            }
            catch (Exception ex)
            {
                HandleException(ex);
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
