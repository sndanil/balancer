using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Balancer
{
	public class TcpServer
	{
		private TcpListener _listener;
		private Balancer _balancer;

		public TcpServer(Balancer balancer, int port)
		{
			if (balancer == null)
				throw new ArgumentNullException(nameof(balancer));

			_listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
			_listener.Server.NoDelay = true;

			_balancer = balancer;
		}

		public async Task Start()
		{
			_listener.Start();
			_balancer.Start();

			Console.WriteLine($"Server started {_listener.LocalEndpoint}");

			while (true)
			{

				try
				{
					var remoteClient = await _listener.AcceptTcpClientAsync();
					remoteClient.NoDelay = true;
					_ = _balancer.ProcessNext(remoteClient);
				}
				catch (Exception ex)
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine($"{ex.Message}");
					Console.ResetColor();
				}

			}
		}
	}
}
