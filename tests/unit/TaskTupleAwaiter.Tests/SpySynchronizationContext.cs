namespace TaskTupleAwaiter.Tests;

sealed class SpySynchronizationContext : SynchronizationContext
{
	public bool DidPost { get; private set; }

	public override void Post(SendOrPostCallback d, object state)
	{
		DidPost = true;
		base.Post(d, state);
	}
}
