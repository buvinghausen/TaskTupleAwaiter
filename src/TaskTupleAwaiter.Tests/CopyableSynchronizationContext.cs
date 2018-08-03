using System.Threading;

namespace TaskTupleAwaiter.Tests
{
	internal sealed class CopyableSynchronizationContext : SynchronizationContext
	{
		public override SynchronizationContext CreateCopy()
		{
			return new CopyableSynchronizationContext();
		}
	}
}
