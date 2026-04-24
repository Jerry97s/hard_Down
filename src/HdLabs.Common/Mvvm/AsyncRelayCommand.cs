using System.Windows.Input;

namespace HdLabs.Common.Mvvm;

public sealed class AsyncRelayCommand : ICommand
{
    private readonly Func<Task> _execute;
    private readonly Func<bool>? _canExecute;
    private bool _isExecuting;

    public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public bool IsExecuting
    {
        get => _isExecuting;
        private set
        {
            if (_isExecuting == value) return;
            _isExecuting = value;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool CanExecute(object? parameter)
        => !IsExecuting && (_canExecute?.Invoke() ?? true);

    public async void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            return;

        try
        {
            IsExecuting = true;
            await _execute().ConfigureAwait(true);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    public event EventHandler? CanExecuteChanged;
}
