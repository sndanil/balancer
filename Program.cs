using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Balancer
{
	class Program
	{
		static async Task Main(string[] args)
		{
			var serverrs = Enumerable.Range(0, 5)
				.Select(i => new RemoteServer { Server = "127.0.0.1", Port = 4830 + i })
				.ToArray();

			var server = new TcpServer(new Balancer(serverrs), 6400);
			await server.Start();
    }
	}
}
