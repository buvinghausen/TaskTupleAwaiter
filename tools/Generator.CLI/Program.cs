using System;

namespace Generator.CLI
{
	class Program
	{
		static int Main(string[] args)
		{
			var runner = new CommandDotNet.AppRunner<ConsoleShell>();
#if (DEBUG)
			runner.Run(args);
			while (true)
			{
				var input = Console.ReadLine();
				if (string.IsNullOrEmpty(input))
					return 0;

				args = input.Split(' ');
				Console.Clear();
				var state = runner.Run(args);
				Console.WriteLine($"Application finished with {state}.");
			}
#else
			return runner.Run(args);
#endif

		}
	}
}
