using System;
using System.Net.Sockets;
using System.Text;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D0 RID: 208
	public static class KukaUdpMessenger
	{
		// Token: 0x06000B04 RID: 2820 RVA: 0x000404C0 File Offset: 0x0003E6C0
		public static void SendMessage(string message)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(message);
			KukaUdpMessenger.udpClient.Send(bytes, bytes.Length, KukaUdpMessenger.remoteAddress, KukaUdpMessenger.port);
		}

		// Token: 0x04000A59 RID: 2649
		private static UdpClient udpClient = new UdpClient();

		// Token: 0x04000A5A RID: 2650
		private static readonly int port = 30000;

		// Token: 0x04000A5B RID: 2651
		private static readonly string remoteAddress = "172.31.1.147";
	}
}
