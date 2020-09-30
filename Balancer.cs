using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Balancer
{
	public class Balancer
	{
		private ConcurrentQueue<BalancerClient> _clientQueue = new ConcurrentQueue<BalancerClient>();
		private IEnumerable<RemoteServer> _servers;
		private int _connectionsPerServer = 0;

		public Balancer(IEnumerable<RemoteServer> servers, int connectionsPerServer = 2)
		{
			if (servers == null)
				throw new ArgumentNullException(nameof(servers));

			_servers = servers;
			_connectionsPerServer = connectionsPerServer;
		}

		public void Start()
		{
			foreach(var server in Enumerable.Range(0, _connectionsPerServer).SelectMany(i => _servers, (i, server) => server))
			{
				var client = new BalancerClient(server.Server, server.Port);
				_clientQueue.Enqueue(client);
			}
		}

		private async Task<BalancerClient> GetNext()
		{
			BalancerClient client;
			while (!_clientQueue.TryDequeue(out client))
				await Task.Delay(100);

			return client;
		}

		public async Task ProcessNext(TcpClient remoteClient)
		{
			var client = await GetNext();
			_ = client.ProcessConnection(remoteClient)
				.ContinueWith(t => _clientQueue.Enqueue(client));
		}
	}
}
