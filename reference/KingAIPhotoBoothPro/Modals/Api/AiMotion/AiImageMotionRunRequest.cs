using System;

namespace KingAIPhotoBoothPro.Modals.Api.AiMotion
{
	// Token: 0x02000094 RID: 148
	public class AiImageMotionRunRequest : ForgotUnlockDashboardModals.ApiRequestBase
	{
		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06000907 RID: 2311 RVA: 0x00033F26 File Offset: 0x00032126
		// (set) Token: 0x06000908 RID: 2312 RVA: 0x00033F2E File Offset: 0x0003212E
		public string AncestorVariationMediaHash { get; set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06000909 RID: 2313 RVA: 0x00033F37 File Offset: 0x00032137
		// (set) Token: 0x0600090A RID: 2314 RVA: 0x00033F3F File Offset: 0x0003213F
		public string Prompt { get; set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x0600090B RID: 2315 RVA: 0x00033F48 File Offset: 0x00032148
		// (set) Token: 0x0600090C RID: 2316 RVA: 0x00033F50 File Offset: 0x00032150
		public string NegativePrompt { get; set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x0600090D RID: 2317 RVA: 0x00033F59 File Offset: 0x00032159
		// (set) Token: 0x0600090E RID: 2318 RVA: 0x00033F61 File Offset: 0x00032161
		public int Duration { get; set; }
	}
}
