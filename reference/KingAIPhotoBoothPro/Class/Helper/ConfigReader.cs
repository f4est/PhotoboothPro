using System;
using System.Configuration;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000CE RID: 206
	public class ConfigReader
	{
		// Token: 0x06000B01 RID: 2817 RVA: 0x00040464 File Offset: 0x0003E664
		public static string GetParameterValue(string key)
		{
			return ConfigurationManager.AppSettings[key];
		}

		// Token: 0x04000A57 RID: 2647
		public const string KeyStorageZoneName = "CDNStorageZoneName";

		// Token: 0x04000A58 RID: 2648
		public const string KeyStorageApiAccessKey = "CDNApiAccessKey";
	}
}
