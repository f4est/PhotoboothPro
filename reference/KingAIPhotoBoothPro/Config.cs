using System;

namespace KingAIPhotoBoothPro
{
	// Token: 0x0200000F RID: 15
	public static class Config
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600005F RID: 95 RVA: 0x000047C7 File Offset: 0x000029C7
		public static string AppIdentifier
		{
			get
			{
				return "com.harikalar.dslrphotobooth";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000060 RID: 96 RVA: 0x000047CE File Offset: 0x000029CE
		// (set) Token: 0x06000061 RID: 97 RVA: 0x000047D5 File Offset: 0x000029D5
		public static string HostAddress
		{
			get
			{
				return Config.hostAddress;
			}
			private set
			{
				Config.hostAddress = value;
			}
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000047DD File Offset: 0x000029DD
		public static void ChangeHostAddress(string currentAddress)
		{
			if (currentAddress == "https://v5.apiactivationking.com/")
			{
				Config.HostAddress = "https://v5backup.apiactivationking.com/";
				return;
			}
			Config.HostAddress = "https://v5.apiactivationking.com/";
		}

		// Token: 0x04000045 RID: 69
		private const string appIdentifier = "com.harikalar.dslrphotobooth";

		// Token: 0x04000046 RID: 70
		public static string DatetimeStringFormat = "dd.MM.yyyy hh:mm:ss";

		// Token: 0x04000047 RID: 71
		private const string ApiWebsite = "https://v5.apiactivationking.com/";

		// Token: 0x04000048 RID: 72
		private const string ApiBackupWebsite = "https://v5backup.apiactivationking.com/";

		// Token: 0x04000049 RID: 73
		public static string hostAddress = "https://v5.apiactivationking.com/";

		// Token: 0x0400004A RID: 74
		public const string MidjourneyHostAddress = "https://midjourney.activationking.com/";

		// Token: 0x0400004B RID: 75
		public const string ShareHostAddress = "https://activationshare.com/";

		// Token: 0x0400004C RID: 76
		public const string ContainerIp = "https://flask.activationking.com/";

		// Token: 0x0400004D RID: 77
		public const string MainWebsite = "https://activationking.com";

		// Token: 0x0400004E RID: 78
		public const string CdnAddress = "https://cdn.activationshare.com";
	}
}
