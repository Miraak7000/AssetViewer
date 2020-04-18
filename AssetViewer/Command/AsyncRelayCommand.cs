using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VAV {

    /// <summary>Provides an async implementation of the <see cref="ICommand"/> interface.
    /// The command is inactive when the command's task is running. </summary>
    public class AsyncRelayCommand : CommandBase {

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="AsyncRelayCommand"/> class. </summary>
        /// <param name="execute">The function to execute. </param>
        public AsyncRelayCommand(Func<Task> execute)
            : this(execute, null) { }

        /// <summary>Initializes a new instance of the <see cref="AsyncRelayCommand"/> class. </summary>
        /// <param name="execute">The function. </param>
        /// <param name="canExecute">The predicate to check whether the function can be executed. </param>
        /// <exception cref="T:System.ArgumentNullException"></exception>
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute) {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region Properties

        /// <summary>Gets a value indicating whether the command can execute in its current state. </summary>
        public override bool CanExecute => !IsRunning && (_canExecute == null || _canExecute());

        /// <summary>Gets a value indicating whether the command is currently running. </summary>
        public bool IsRunning {
            get => _isRunning;
            private set {
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>Defines the method to be called when the command is invoked. </summary>
        protected override async void Execute() {
            var task = _execute();
            if (task != null) {
                IsRunning = true;
                await task;
                IsRunning = false;
            }
        }

        #endregion Methods

        #region Fields

        private readonly Func<bool> _canExecute;
        private readonly Func<Task> _execute;
        private bool _isRunning;

        #endregion Fields
    }

    /// <summary>Provides an implementation of the <see cref="ICommand"/> interface. </summary>
    /// <typeparam name="TParameter">The type of the command parameter. </typeparam>
    public class AsyncRelayCommand<TParameter> : CommandBase<TParameter> {

        #region Constructors

        /// <summary>Initializes a new instance of the <see cref="AsyncRelayCommand"/> class. </summary>
        /// <param name="execute">The function. </param>
        public AsyncRelayCommand(Func<TParameter, Task> execute)
            : this(execute, null) {
        }

        /// <summary>Initializes a new instance of the <see cref="AsyncRelayCommand"/> class. </summary>
        /// <param name="execute">The function. </param>
        /// <param name="canExecute">The predicate to check whether the function can be executed. </param>
        public AsyncRelayCommand(Func<TParameter, Task> execute, Predicate<TParameter> canExecute) {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        #endregion Constructors

        #region Properties

        /// <summary>Gets a value indicating whether the command is currently running. </summary>
        public bool IsRunning {
            get => _isRunning;
            private set {
                _isRunning = value;
                RaiseCanExecuteChanged();
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>Gets a value indicating whether the command can execute in its current state. </summary>
        /// <param name="parameter">todo: describe parameter parameter on CanExecute</param>
        [DebuggerStepThrough]
        public override bool CanExecute(TParameter parameter) {
            return !IsRunning && (_canExecute == null || _canExecute(parameter));
        }

        /// <summary>Defines the method to be called when the command is invoked. </summary>
        /// <param name="parameter">todo: describe parameter parameter on Execute</param>
        protected override async void Execute(TParameter parameter) {
            var task = _execute(parameter);
            if (task != null) {
                IsRunning = true;
                await task;
                IsRunning = false;
            }
        }

        #endregion Methods

        #region Fields

        private readonly Predicate<TParameter> _canExecute;
        private readonly Func<TParameter, Task> _execute;
        private bool _isRunning;

        #endregion Fields
    }
}