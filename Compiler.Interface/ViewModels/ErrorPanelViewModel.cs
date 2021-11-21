using Compiler.Core;

namespace Compiler.Interface.ViewModels;

public class ErrorPanelViewModel : BaseViewModel
{
    private string message;
    private string column;
    private string line;
    private string file;
    private string errorType;

    public string Message
    {
        get => message;
        set => SetProperty(ref message, value);
    }

    public string Column
    {
        get => column;
        set => SetProperty(ref column, value);
    }

    public string Line
    {
        get => line;
        set => SetProperty(ref line, value);
    }

    public string File
    {
        get => file;
        set => SetProperty(ref file, value);
    }

    public string ErrorType
    {
        get => errorType;
        set => SetProperty(ref errorType, value);
    }

    public ErrorPanelViewModel()
    {
        EventAggregator.GetEvent<CompileFailEventPubSub>().Subscribe((e) => ShowError(e));
        EventAggregator.GetEvent<CompileSuccessEventPubSub>().Subscribe(() => ClearError());
    }

    public void ShowError(ShowErrorEventArgs eventArgs)
    {
        this.Message = eventArgs.Message;
        this.Column = eventArgs.Column;
        this.Line = eventArgs.Line;
        this.File = eventArgs.File;
        this.ErrorType = eventArgs.ErrorType;
    }

    public void ClearError()
    {
        this.Message = string.Empty;
        this.Column = string.Empty;
        this.Line = string.Empty;
        this.File = string.Empty;
        this.ErrorType = string.Empty;
    }
}
