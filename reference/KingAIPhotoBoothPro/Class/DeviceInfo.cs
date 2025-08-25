using System;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B2 RID: 178
	internal class DeviceInfo
	{
		// Token: 0x06000A1A RID: 2586 RVA: 0x0003A86C File Offset: 0x00038A6C
		public static string GetDeviceUniqueIdentifier()
		{
			string ret = string.Empty;
			string concatStr = string.Empty;
			try
			{
				ConnectionOptions options = new ConnectionOptions
				{
					Impersonation = ImpersonationLevel.Impersonate,
					EnablePrivileges = true
				};
				ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2", options);
				scope.Connect();
				ManagementObjectSearcher searcherBb = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_BaseBoard"));
				foreach (ManagementBaseObject obj in searcherBb.Get())
				{
					concatStr += (((string)obj.Properties["SerialNumber"].Value) ?? string.Empty);
				}
				ManagementObjectSearcher searcherBios = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_BIOS"));
				foreach (ManagementBaseObject obj2 in searcherBios.Get())
				{
					concatStr += (((string)obj2.Properties["SerialNumber"].Value) ?? string.Empty);
				}
				ManagementObjectSearcher searcherOs = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_OperatingSystem"));
				foreach (ManagementBaseObject obj3 in searcherOs.Get())
				{
					concatStr += (((string)obj3.Properties["SerialNumber"].Value) ?? string.Empty);
				}
				using (SHA1 sha = SHA1.Create())
				{
					ret = string.Concat(from b in sha.ComputeHash(Encoding.UTF8.GetBytes(concatStr))
					select b.ToString("x2"));
				}
			}
			catch (Exception e)
			{
				File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff") + ".txt"), e.ToString());
			}
			try
			{
				if (string.IsNullOrEmpty(ret))
				{
					concatStr = DeviceInfo.GetMotherboardUUID();
					using (SHA1 sha2 = SHA1.Create())
					{
						ret = string.Concat(from b in sha2.ComputeHash(Encoding.UTF8.GetBytes(concatStr))
						select b.ToString("x2"));
					}
				}
			}
			catch (Exception e2)
			{
				File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff") + ".txt"), e2.ToString());
			}
			try
			{
				if (string.IsNullOrEmpty(ret))
				{
					concatStr = DeviceInfo.GetMacAddress();
					using (SHA1 sha3 = SHA1.Create())
					{
						ret = string.Concat(from b in sha3.ComputeHash(Encoding.UTF8.GetBytes(concatStr))
						select b.ToString("x2"));
					}
				}
			}
			catch (Exception e3)
			{
				File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff") + ".txt"), e3.ToString());
			}
			return ret;
		}

		// Token: 0x06000A1B RID: 2587 RVA: 0x0003ACAC File Offset: 0x00038EAC
		public static string GetMotherboardUUID()
		{
			string uuid = string.Empty;
			ConnectionOptions options = new ConnectionOptions
			{
				Impersonation = ImpersonationLevel.Impersonate,
				EnablePrivileges = true
			};
			ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2", options);
			scope.Connect();
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT UUID FROM Win32_ComputerSystemProduct")))
			{
				using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = searcher.Get().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ManagementBaseObject obj = enumerator.Current;
						object obj2 = obj["UUID"];
						uuid = ((obj2 != null) ? obj2.ToString() : null);
					}
				}
			}
			return uuid;
		}

		// Token: 0x06000A1C RID: 2588 RVA: 0x0003AD68 File Offset: 0x00038F68
		public static string GetMacAddress()
		{
			NetworkInterface nic = NetworkInterface.GetAllNetworkInterfaces().FirstOrDefault((NetworkInterface n) => n.OperationalStatus == OperationalStatus.Up);
			if (nic == null)
			{
				return null;
			}
			return nic.GetPhysicalAddress().ToString();
		}
	}
}
