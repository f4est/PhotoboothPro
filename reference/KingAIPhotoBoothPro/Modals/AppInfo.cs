using System;
using System.Windows;
using System.Windows.Media;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200007E RID: 126
	public static class AppInfo
	{
		// Token: 0x17000134 RID: 308
		// (get) Token: 0x060008B9 RID: 2233 RVA: 0x00033A4C File Offset: 0x00031C4C
		// (set) Token: 0x060008BA RID: 2234 RVA: 0x00033ABC File Offset: 0x00031CBC
		public static App AppClass
		{
			get
			{
				if (AppInfo.app == null)
				{
					AppInfo.app = new App
					{
						Name = "KingAIPhotoboothPro",
						Category = "Default",
						SupportLink = "harikalar.com",
						ThemeColorHexString = "#129900",
						Description = "Test App",
						DisplayName = "Activation<b>King</b>",
						InfoLink = "harikalar.com"
					};
				}
				return AppInfo.app;
			}
			set
			{
				AppInfo.app = value;
			}
		}

		// Token: 0x17000135 RID: 309
		// (get) Token: 0x060008BB RID: 2235 RVA: 0x00033AC4 File Offset: 0x00031CC4
		public static string Identifier
		{
			get
			{
				return Application.Current.MainWindow.GetType().Assembly.GetName().Name;
			}
		}

		// Token: 0x17000136 RID: 310
		// (get) Token: 0x060008BC RID: 2236 RVA: 0x00033AE4 File Offset: 0x00031CE4
		// (set) Token: 0x060008BD RID: 2237 RVA: 0x00033B46 File Offset: 0x00031D46
		public static Color ThemeColor
		{
			get
			{
				Color themeColor = new Color
				{
					R = 18,
					G = 153,
					B = 0,
					A = byte.MaxValue
				};
				if (AppInfo.AppClass.ThemeColorHexString != null)
				{
					themeColor = (Color)ColorConverter.ConvertFromString(AppInfo.AppClass.ThemeColorHexString);
				}
				return themeColor;
			}
			set
			{
				if (AppInfo.OnThemeColorChanged != null)
				{
					AppInfo.OnThemeColorChanged(value);
				}
			}
		}

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x060008BE RID: 2238 RVA: 0x00033B5C File Offset: 0x00031D5C
		// (remove) Token: 0x060008BF RID: 2239 RVA: 0x00033B90 File Offset: 0x00031D90
		public static event AppInfo.ThemeColorChangedAction OnThemeColorChanged;

		// Token: 0x060008C0 RID: 2240 RVA: 0x00033BC4 File Offset: 0x00031DC4
		public static void SetAppInfoWithJsonString(string str)
		{
			AppInfo.AppClass = JsonConvert.DeserializeObject<App>(str);
			Color color = AppInfo.ThemeColor;
			color = (Color)ColorConverter.ConvertFromString(AppInfo.AppClass.ThemeColorHexString);
			AppInfo.ThemeColor = color;
		}

		// Token: 0x060008C1 RID: 2241 RVA: 0x00033C00 File Offset: 0x00031E00
		public static void SetAppInfoWithClass(App app)
		{
			AppInfo.AppClass = app;
			Color color = AppInfo.ThemeColor;
			color = (Color)ColorConverter.ConvertFromString(AppInfo.AppClass.ThemeColorHexString);
			AppInfo.ThemeColor = color;
		}

		// Token: 0x040008C8 RID: 2248
		public static string CurrentBuildVersion = "V1.0";

		// Token: 0x040008C9 RID: 2249
		private static App app;

		// Token: 0x0200021D RID: 541
		// (Invoke) Token: 0x06000E6B RID: 3691
		public delegate void ThemeColorChangedAction(Color themeColor);
	}
}
