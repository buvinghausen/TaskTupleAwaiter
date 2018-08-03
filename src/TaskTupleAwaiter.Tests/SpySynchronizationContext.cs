using System.Threading;

namespace TaskTupleAwaiter.Tests
{
	internal sealed class SpySynchronizationContext : SynchronizationContext
	{
		public bool DidPost { get; private set; }

		public override void Post(SendOrPostCallback d, object state)
		{
			DidPost = true;
			base.Post(d, state);
		}
	}
}
