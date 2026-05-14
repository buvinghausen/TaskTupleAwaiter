namespace TaskTupleAwaiter.Tests;

static class On
{
	public static IDisposable Dispose(Action action) =>
		new OnDisposeAction(action);

	sealed class OnDisposeAction(Action action) : IDisposable
	{
		Action _action = action;

		public void Dispose() =>
			Interlocked.Exchange(ref _action, null)?.Invoke();
	}
}
