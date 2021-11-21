using System.IO;
using System.Windows.Input;
using Prism.Commands;

namespace Compiler.Interface.ViewModels;

public class AubFile : BaseViewModel
{
    public string FileName
    {
        get
        {
            try { return Path.GetFileName(FilePath).Split('.')[0]; }
            catch (Exception)
            {
                return "";
            }
        }
    }

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
        set
        {
            SetProperty(ref content, value);
            IsSaved = false;
        }
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

    private bool isSaved;
    public bool IsSaved
    {
        get => isSaved;
        private set => SetProperty(ref isSaved, value);
    }

    private AubFile(string path, string content)
    {
        this.FilePath = path;
        this.Content = content;
        this.IsSaved = true;

        this.CloseFileCommand = new DelegateCommand(CloseFileCommandExecuted);
    }

    private void CloseFileCommandExecuted() => EventAggregator.GetEvent<CloseFilePubSub>().Publish(this);

    public static AubFile OpenAubFile(string path) => new(path, File.ReadAllText(path));

    public static AubFile NewAubFile(string path) => new(path, "");

    public void Save()
    {
        File.WriteAllText(FilePath, Content);
        IsSaved = true;
    }

    public ICommand CloseFileCommand { get; }

}
