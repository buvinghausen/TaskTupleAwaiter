namespace TaskTupleAwaiter.Tests;

internal static class On
{
	public static IDisposable Dispose(Action action) =>
		new OnDisposeAction(action);

	private sealed class OnDisposeAction(Action action) : IDisposable
	{
		public void Dispose() =>
			Interlocked.Exchange(ref action, null)?.Invoke();
	}
}
