namespace TaskTupleAwaiter.Tests;

static class On
{
	public static IDisposable Dispose(Action action) =>
		new OnDisposeAction(action);

	private sealed class OnDisposeAction(Action action) : IDisposable
	{
		private Action _action = action;

		public void Dispose() =>
			Interlocked.Exchange(ref _action, null)?.Invoke();
	}
}
