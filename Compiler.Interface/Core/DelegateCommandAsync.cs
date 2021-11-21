using System.Linq.Expressions;
using System.Windows.Input;
using Prism.Commands;

namespace Compiler.Interface;

public interface IAsyncCommand : IAsyncCommand<object>
{
}

public interface IAsyncCommand<in T>
{
    Task ExecuteAsync(T obj);
}

public class DelegateCommandAsync : DelegateCommandAsync<object>
{
    public DelegateCommandAsync(Func<Task> executeMethod)
        : base(o => executeMethod())
    {
    }

    public DelegateCommandAsync(Func<Task> executeMethod, Func<bool> canExecuteMethod)
        : base(o => executeMethod(), o => canExecuteMethod())
    {
    }
}

public class DelegateCommandAsync<T> : IAsyncCommand<T>, ICommand
{
    private readonly Func<T, Task> _executeMethod;
    private readonly DelegateCommand<T> _underlyingCommand;
    private bool _isExecuting;

    public DelegateCommandAsync(Func<T, Task> executeMethod)
        : this(executeMethod, _ => true)
    {
    }

    public DelegateCommandAsync(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
    {
        _executeMethod = executeMethod;
        _underlyingCommand = new DelegateCommand<T>(x => { }, canExecuteMethod);
    }

    #region IAsyncCommand Interface
    public async Task ExecuteAsync(T obj)
    {
        try
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();
            await _executeMethod(obj);
        }
        finally
        {
            _isExecuting = false;
            RaiseCanExecuteChanged();
        }
    }
    #endregion

    #region Command Interface Members

    public bool CanExecute(object parameter) => !_isExecuting && _underlyingCommand.CanExecute((T)parameter);

    public event EventHandler CanExecuteChanged
    {
        add { _underlyingCommand.CanExecuteChanged += value; }
        remove { _underlyingCommand.CanExecuteChanged -= value; }
    }

    public async void Execute(object parameter) => await ExecuteAsync((T)parameter);

    #endregion

    public void RaiseCanExecuteChanged() => _underlyingCommand.RaiseCanExecuteChanged();

    public DelegateCommandAsync<T> ObservesCanExecute(Expression<Func<bool>> canExecuteExpression)
    {
        _underlyingCommand.ObservesCanExecute(canExecuteExpression);
        return this;
    }

    public DelegateCommandAsync<T> ObservesProperty(Expression<Func<T>> propertyExpression)
    {
        _underlyingCommand.ObservesProperty(propertyExpression);
        return this;
    }
}
