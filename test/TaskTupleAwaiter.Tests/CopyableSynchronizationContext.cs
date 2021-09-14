using System.Threading;

namespace TaskTupleAwaiter.Tests;

internal sealed class CopyableSynchronizationContext : SynchronizationContext
{
	public override SynchronizationContext CreateCopy() =>
		new CopyableSynchronizationContext();

	public override void Post(SendOrPostCallback d, object state) =>
		d.Invoke(state);
}
