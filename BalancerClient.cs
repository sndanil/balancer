using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Balancer
{
	public class BalancerClient
	{
		private string _remoteServer;
		private int _remoteServerPort;

		public BalancerClient(string remoteServer, int remoteServerPort)
		{
			_remoteServer = remoteServer;
			_remoteServerPort = remoteServerPort;
		}

		public async Task ProcessConnection(TcpClient remoteClient)
		{
			var guid = Guid.NewGuid();
			Console.WriteLine($"[{DateTime.Now}] Established {_remoteServer}:{_remoteServerPort} {guid}");
			try
			{
				using (var client = new TcpClient(_remoteServer, _remoteServerPort))
				using (remoteClient)
				{
					client.NoDelay = true;

					var serverStream = client.GetStream();
					var remoteStream = remoteClient.GetStream();

					await Task.WhenAny(remoteStream.CopyToAsync(serverStream), serverStream.CopyToAsync(remoteStream));
				}
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"{guid} {ex.Message}");
				Console.ResetColor();
			}
			finally
			{
				Console.WriteLine($"[{DateTime.Now}] Closed {guid}");
			}
		}
	}
}
