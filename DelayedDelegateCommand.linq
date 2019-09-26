<Query Kind="Program" />

internal sealed class DelayedExecutionScheduler
{
	private const int WaiteTime = 750;

	private readonly Action executeMethod;

	private Timer timer;
	private bool isTimerStarted;

	public DelayedExecutionScheduler(Action executeMethod)
	{
		this.executeMethod = executeMethod;
	}

	public void Execute()
	{
		if (this.isTimerStarted)
		{
			this.timer.Change(WaiteTime, int.MaxValue);
			return;
		}

		this.isTimerStarted = true;
		Exception faultedException = null;

		this.timer = new Timer(
		  state =>
		  {
			  try
			  {
				  this.isTimerStarted = false;
				  this.executeMethod.Invoke();
			  }
			  catch (Exception exception)
			  {
				  faultedException = exception;
			  }
		  },
		  null,
		  WaiteTime,
		  int.MaxValue);

		if (faultedException != null)
		{
			throw faultedException;
		}
	}
}

/// <summary>
/// The delayed delegate command. Delays the execution of the command for some milliseconds.
/// </summary>
public sealed class DelayedDelegateCommand : DelegateCommandBase
{
	private readonly Func<bool> canExecuteMethod;
	private readonly DelayedExecutionScheduler delayedExecutionScheduler;

	/// <summary>
	/// Initializes a new instance of the <see cref="DelayedDelegateCommand"/> class.
	/// </summary>
	/// <param name="executeMethod">Delegate to execute when CanExecute is called on the command. This can be null..</param>
	public DelayedDelegateCommand(Action executeMethod)
	  : this(executeMethod, () => true)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DelayedDelegateCommand"/> class.
	/// </summary>
	/// <param name="executeMethod">Delegate to execute when CanExecute is called on the command. This can be null..</param>
	/// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command. This can be null.</param>
	public DelayedDelegateCommand(Action executeMethod, Func<bool> canExecuteMethod)
	  : base(o => executeMethod(), o => canExecuteMethod())
	{
		if (executeMethod == null || canExecuteMethod == null)
		{
			throw new ArgumentNullException(nameof(executeMethod), "Delegate commands can not be null.");
		}

		this.canExecuteMethod = canExecuteMethod;
		this.delayedExecutionScheduler = new DelayedExecutionScheduler(executeMethod);
	}

	/// <summary>
	/// Executes the command with the provided parameter by invoking the <see cref="T:System.Action`1" /> supplied during construction.
	/// </summary>
	/// <param name="parameter">The methods parameter. Will be ignored.</param>
	/// <returns>A completed task.</returns>
	protected override Task Execute(object parameter)
	{
		this.delayedExecutionScheduler.Execute();
		return Task.CompletedTask;
	}
}

void Main()
{

}

// Define other methods and classes here
