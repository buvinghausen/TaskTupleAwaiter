using System;
using System.Threading;

namespace TaskTupleAwaiter.Tests
{
	internal static class On
	{
		public static IDisposable Dispose(Action action) => new OnDisposeAction(action);

		private sealed class OnDisposeAction : IDisposable
		{
			private Action _action;

			public OnDisposeAction(Action action) => _action = action;

			public void Dispose() => Interlocked.Exchange(ref _action, null)?.Invoke();
		}
	}
}
