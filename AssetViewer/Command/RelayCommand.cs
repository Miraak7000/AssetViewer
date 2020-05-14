using System;
using System.Diagnostics;

namespace VAV {

  /// <summary> Provides an implementation of the <see cref="ICommand"/> interface. </summary>
  public class RelayCommand : CommandBase {

    #region Public Properties

    /// <summary> Gets a value indicating whether the command can execute in its current state. </summary>
    public override bool CanExecute => _canExecute == null || _canExecute();

    #endregion Public Properties

    #region Public Constructors

    /// <summary> Initializes a new instance of the <see cref="RelayCommand"/> class. </summary>
    /// <param name="execute"> The action to execute. </param>
    public RelayCommand(Action execute)
        : this(execute, null) { }

    /// <summary> Initializes a new instance of the <see cref="RelayCommand"/> class. </summary>
    /// <param name="execute"> The action to execute. </param>
    /// <param name="canExecute"> The predicate to check whether the function can be executed. </param>
    /// <exception cref="T:System.ArgumentNullException"> </exception>
    public RelayCommand(Action execute, Func<bool> canExecute) {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute;
    }

    #endregion Public Constructors

    #region Protected Methods

    /// <summary> Defines the method to be called when the command is invoked. </summary>
    protected override void Execute() {
      _execute();
    }

    #endregion Protected Methods

    #region Private Fields

    private readonly Func<bool> _canExecute;
    private readonly Action _execute;

    #endregion Private Fields
  }

  /// <summary> Provides an implementation of the <see cref="ICommand"/> interface. </summary>
  /// <typeparam name="T"> The type of the command parameter. </typeparam>
  public class RelayCommand<T> : CommandBase<T> {

    #region Public Constructors

    /// <summary> Initializes a new instance of the <see cref="RelayCommand"/> class. </summary>
    /// <param name="execute"> The action to execute. </param>
    public RelayCommand(Action<T> execute)
        : this(execute, null) {
    }

    /// <summary> Initializes a new instance of the <see cref="RelayCommand"/> class. </summary>
    /// <param name="execute"> The action to execute. </param>
    /// <param name="canExecute"> The predicate to check whether the function can be executed. </param>
    public RelayCommand(Action<T> execute, Predicate<T> canExecute) {
      _execute = execute ?? throw new ArgumentNullException(nameof(execute));
      _canExecute = canExecute;
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary> Gets a value indicating whether the command can execute in its current state. </summary>
    /// <param name="parameter"> todo: describe parameter parameter on CanExecute </param>
    [DebuggerStepThrough]
    public override bool CanExecute(T parameter) {
      return _canExecute == null || _canExecute(parameter);
    }

    #endregion Public Methods

    #region Protected Methods

    /// <summary> Defines the method to be called when the command is invoked. </summary>
    /// <param name="parameter"> todo: describe parameter parameter on Execute </param>
    protected override void Execute(T parameter) {
      _execute(parameter);
    }

    #endregion Protected Methods

    #region Private Fields

    private readonly Predicate<T> _canExecute;
    private readonly Action<T> _execute;

    #endregion Private Fields
  }
}