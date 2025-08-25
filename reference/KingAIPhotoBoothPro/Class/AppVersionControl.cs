using System;
using System.Reflection;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B0 RID: 176
	public static class AppVersionControl
	{
		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060009E8 RID: 2536 RVA: 0x00039378 File Offset: 0x00037578
		// (set) Token: 0x060009E9 RID: 2537 RVA: 0x0003937F File Offset: 0x0003757F
		public static string NewVersionName { get; set; }

		// Token: 0x060009EA RID: 2538 RVA: 0x00039388 File Offset: 0x00037588
		public static bool CheckUpdate(string versionControl)
		{
			if (string.IsNullOrEmpty(versionControl))
			{
				return false;
			}
			versionControl = versionControl.Replace("V", "").Replace("v", "");
			Version versionApp = Assembly.GetExecutingAssembly().GetName().Version;
			Version versionToCheck = new Version(versionControl);
			AppVersionControl.NewVersionName = versionControl;
			return versionApp < versionToCheck;
		}
	}
}
