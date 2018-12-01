using Compiler.Core;
using Compiler.Interface.Properties;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Unity.Attributes;

namespace Compiler.Interface.ViewModels
{
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
            SaveFileCommand = new DelegateCommand(SaveFileCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            SaveAllFilesCommand = new DelegateCommand(SaveAllFilesCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            SaveAsCommand = new DelegateCommand(SaveAsCommandExecuted, IsSelectedFile).ObservesProperty(() => SelectedFile);
            NextTabCommand = new DelegateCommand(NextTabCommandExecuted, () => SelectedFile != null && Files.Count > 1).ObservesProperty(() => SelectedFile).ObservesProperty(() => Files);
            WindowClosing = new DelegateCommand(WindowClosingExecuted);

            EventAggregator.GetEvent<CloseFilePubSub>().Subscribe(file => CloseFile(file));
        }

        private void WindowClosingExecuted()
        {
            ExitApplicationCommandExecuted();
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

        private bool IsSelectedFile() => SelectedFile != null;

        private void NewFileCommandExecuted()
        {
            try
            {
                string filePath = DialogService.SaveFileDialog();
                if (string.IsNullOrEmpty(filePath))
                    return;

                using (File.Create(filePath))
                { }

                AubFile newFile = AubFile.NewAubFile(filePath);
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
                string[] filePaths = DialogService.OpenFileDialog();
                if (filePaths == null || filePaths.Length == 0)
                    return;

                foreach (string filePath in filePaths)
                {
                    AubFile newFile = AubFile.OpenAubFile(filePath);
                    Files.Add(newFile);
                    SelectedFile = newFile;
                }
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

            CloseFile(SelectedFile);
        }

        private void CloseFile(AubFile file)
        {
            try
            {
                MessageBoxResult result = CloseFileIfUserConfirm(file);
                if (result != MessageBoxResult.Cancel)
                    Files.Remove(file);
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
                // The ToList method here is essential, do not remove it, it is fix for an exception
                // https://stackoverflow.com/a/27851493
                foreach (AubFile item in Files.ToList())
                {
                    MessageBoxResult result = CloseFileIfUserConfirm(item);
                    if (result != MessageBoxResult.Cancel)
                        Files.Remove(item);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private MessageBoxResult CloseFileIfUserConfirm(AubFile file)
        {
            if (file.IsSaved)
                return MessageBoxResult.Yes;

            if (Settings.Default.AutoSave)
            {
                file.Save();
                return MessageBoxResult.Yes;
            }

            MessageBoxResult result = DialogService.ShowConfirmation($"Save file {file.FileName} ?");
            if (result == MessageBoxResult.Yes)
                file.Save();

            return result;
        }

        private void SaveFileCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            try
            {
                SelectedFile.Save();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void SaveAllFilesCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            try
            {
                foreach (AubFile file in Files)
                    file.Save();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void SaveAsCommandExecuted()
        {
            if (SelectedFile == null)
                return;

            try
            {
                string filePath = DialogService.SaveFileDialog();
                if (string.IsNullOrEmpty(filePath))
                    return;

                using (File.Create(filePath))
                { }

                AubFile saveAsFile = AubFile.NewAubFile(filePath);
                saveAsFile.Content = SelectedFile.Content;
                saveAsFile.Save();
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
            CloseAllFilesCommandExecuted();
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
        public ICommand NextTabCommand { get; }
        public ICommand WindowClosing { get; }

        private void BuildCommandExecuted()
        {
            try
            {
                Compiler.CompileMainProgram(SelectedFile.FilePath);
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
                Compiler.CompileMainProgram(SelectedFile.FilePath);
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
