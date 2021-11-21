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
        Message = eventArgs.Message;
        Column = eventArgs.Column;
        Line = eventArgs.Line;
        File = eventArgs.File;
        ErrorType = eventArgs.ErrorType;
    }

    public void ClearError()
    {
        Message = string.Empty;
        Column = string.Empty;
        Line = string.Empty;
        File = string.Empty;
        ErrorType = string.Empty;
    }
}
