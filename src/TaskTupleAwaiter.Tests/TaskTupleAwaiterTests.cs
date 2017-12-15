using System;
using System.Threading.Tasks;
using Xunit;

namespace TaskTupleAwaiter.Tests
{
	public class TaskTupleAwaiterTests
	{
		[Fact]
		public async Task CanAwaitTwoTasksWithNewSyntax()
		{
			var (a, b) = await (GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
		}

		[Fact]
		public async Task CanAwaitThreeTasksWithNewSyntax()
		{
			var (a, b, c) = await (GetStringAsync(), GetGuidAsync(), GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
		}

		[Fact]
		public async Task CanAwaitFourTasksWithNewSyntax()
		{
			var (a, b, c, d) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
		}

		[Fact]
		public async Task CanAwaitFiveTasksWithNewSyntax()
		{
			var (a, b, c, d, e) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
		}

		[Fact]
		public async Task CanAwaitSixTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
		}

		[Fact]
		public async Task CanAwaitSevenTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
		}

		[Fact]
		public async Task CanAwaitEightTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g, h) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
			Assert.IsType<Guid>(h);
		}

		[Fact]
		public async Task CanAwaitNineTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g, h, i) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
			Assert.IsType<Guid>(h);
			Assert.IsType<string>(i);
		}

		[Fact]
		public async Task CanAwaitTenTasksWithNewSyntax()
		{
			var (a, b, c, d, e, f, g, h, i, j) =
				await (GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync(), GetStringAsync(), GetGuidAsync(),
					GetStringAsync(), GetGuidAsync());
			Assert.IsType<string>(a);
			Assert.IsType<Guid>(b);
			Assert.IsType<string>(c);
			Assert.IsType<Guid>(d);
			Assert.IsType<string>(e);
			Assert.IsType<Guid>(f);
			Assert.IsType<string>(g);
			Assert.IsType<Guid>(h);
			Assert.IsType<string>(i);
			Assert.IsType<Guid>(j);
		}

		private static async Task<string> GetStringAsync()
		{
			await Task.Delay(500);
			return Guid.NewGuid().ToString();
		}

		private static async Task<Guid> GetGuidAsync()
		{
			await Task.Delay(500);
			return Guid.NewGuid();
		}
	}
}
