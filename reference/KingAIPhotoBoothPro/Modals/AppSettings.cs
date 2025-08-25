using System;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200007D RID: 125
	[Serializable]
	public class AppSettings
	{
		// Token: 0x17000132 RID: 306
		// (get) Token: 0x060008B4 RID: 2228 RVA: 0x00033A12 File Offset: 0x00031C12
		// (set) Token: 0x060008B5 RID: 2229 RVA: 0x00033A1A File Offset: 0x00031C1A
		public int ResultImageQualityValue { get; set; } = 70;

		// Token: 0x17000133 RID: 307
		// (get) Token: 0x060008B6 RID: 2230 RVA: 0x00033A23 File Offset: 0x00031C23
		// (set) Token: 0x060008B7 RID: 2231 RVA: 0x00033A2B File Offset: 0x00031C2B
		public int WaitMessageTime { get; set; } = 30;
	}
}
