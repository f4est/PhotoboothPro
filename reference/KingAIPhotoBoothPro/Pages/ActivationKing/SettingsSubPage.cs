using System;
using System.Windows.Controls;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000044 RID: 68
	public abstract class SettingsSubPage : Page
	{
		// Token: 0x17000082 RID: 130
		// (get) Token: 0x060004E1 RID: 1249 RVA: 0x0001B767 File Offset: 0x00019967
		// (set) Token: 0x060004E2 RID: 1250 RVA: 0x0001B76F File Offset: 0x0001996F
		public DateTime LoadedTime { get; set; } = DateTime.Now;

		// Token: 0x060004E3 RID: 1251
		public abstract string GetTitle();

		// Token: 0x060004E4 RID: 1252
		public abstract string GetSubTitle();
	}
}
