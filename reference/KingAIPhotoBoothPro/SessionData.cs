using System;
using System.IO;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000011 RID: 17
	public static class SessionData
	{
		// Token: 0x1700000D RID: 13
		// (get) Token: 0x0600007A RID: 122 RVA: 0x00004FAC File Offset: 0x000031AC
		public static string ApplicationDataFolderPath
		{
			get
			{
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Config.AppIdentifier);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600007B RID: 123 RVA: 0x00004FDC File Offset: 0x000031DC
		public static string UserDataFolderPath
		{
			get
			{
				string path = Path.Combine(SessionData.ApplicationDataFolderPath, SessionData.accountInfo.userHash);
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600007C RID: 124 RVA: 0x0000500E File Offset: 0x0000320E
		public static string UserEventsJsonPath
		{
			get
			{
				return Path.Combine(SessionData.UserDataFolderPath, "events.json");
			}
		}

		// Token: 0x0400006B RID: 107
		public static AccountInfo accountInfo;

		// Token: 0x0400006C RID: 108
		public static bool RunOnStartup;
	}
}
