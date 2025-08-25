using System;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x0200008C RID: 140
	public class FluxAIModals
	{
		// Token: 0x02000239 RID: 569
		public class FluxAIRequest
		{
			// Token: 0x1700021B RID: 539
			// (get) Token: 0x06000F89 RID: 3977 RVA: 0x0005C26C File Offset: 0x0005A46C
			// (set) Token: 0x06000F8A RID: 3978 RVA: 0x0005C274 File Offset: 0x0005A474
			public string prompt { get; set; }

			// Token: 0x1700021C RID: 540
			// (get) Token: 0x06000F8B RID: 3979 RVA: 0x0005C27D File Offset: 0x0005A47D
			// (set) Token: 0x06000F8C RID: 3980 RVA: 0x0005C285 File Offset: 0x0005A485
			public string MediaHash { get; set; }

			// Token: 0x1700021D RID: 541
			// (get) Token: 0x06000F8D RID: 3981 RVA: 0x0005C28E File Offset: 0x0005A48E
			// (set) Token: 0x06000F8E RID: 3982 RVA: 0x0005C296 File Offset: 0x0005A496
			public string accessToken { get; set; }

			// Token: 0x1700021E RID: 542
			// (get) Token: 0x06000F8F RID: 3983 RVA: 0x0005C29F File Offset: 0x0005A49F
			// (set) Token: 0x06000F90 RID: 3984 RVA: 0x0005C2A7 File Offset: 0x0005A4A7
			public bool Ultra { get; set; }

			// Token: 0x1700021F RID: 543
			// (get) Token: 0x06000F91 RID: 3985 RVA: 0x0005C2B0 File Offset: 0x0005A4B0
			// (set) Token: 0x06000F92 RID: 3986 RVA: 0x0005C2B8 File Offset: 0x0005A4B8
			public bool Raw { get; set; }

			// Token: 0x17000220 RID: 544
			// (get) Token: 0x06000F93 RID: 3987 RVA: 0x0005C2C1 File Offset: 0x0005A4C1
			// (set) Token: 0x06000F94 RID: 3988 RVA: 0x0005C2C9 File Offset: 0x0005A4C9
			public bool Faceswap { get; set; }
		}

		// Token: 0x0200023A RID: 570
		public class FluxAIResponse
		{
			// Token: 0x17000221 RID: 545
			// (get) Token: 0x06000F96 RID: 3990 RVA: 0x0005C2DA File Offset: 0x0005A4DA
			// (set) Token: 0x06000F97 RID: 3991 RVA: 0x0005C2E2 File Offset: 0x0005A4E2
			public string imageUrl { get; set; }

			// Token: 0x17000222 RID: 546
			// (get) Token: 0x06000F98 RID: 3992 RVA: 0x0005C2EB File Offset: 0x0005A4EB
			// (set) Token: 0x06000F99 RID: 3993 RVA: 0x0005C2F3 File Offset: 0x0005A4F3
			public bool success { get; set; }

			// Token: 0x17000223 RID: 547
			// (get) Token: 0x06000F9A RID: 3994 RVA: 0x0005C2FC File Offset: 0x0005A4FC
			// (set) Token: 0x06000F9B RID: 3995 RVA: 0x0005C304 File Offset: 0x0005A504
			public string message { get; set; }

			// Token: 0x17000224 RID: 548
			// (get) Token: 0x06000F9C RID: 3996 RVA: 0x0005C30D File Offset: 0x0005A50D
			// (set) Token: 0x06000F9D RID: 3997 RVA: 0x0005C315 File Offset: 0x0005A515
			public int statusCode { get; set; }

			// Token: 0x17000225 RID: 549
			// (get) Token: 0x06000F9E RID: 3998 RVA: 0x0005C31E File Offset: 0x0005A51E
			// (set) Token: 0x06000F9F RID: 3999 RVA: 0x0005C326 File Offset: 0x0005A526
			public string processingTime { get; set; }

			// Token: 0x17000226 RID: 550
			// (get) Token: 0x06000FA0 RID: 4000 RVA: 0x0005C32F File Offset: 0x0005A52F
			// (set) Token: 0x06000FA1 RID: 4001 RVA: 0x0005C337 File Offset: 0x0005A537
			public string processingTimeSpan { get; set; }

			// Token: 0x17000227 RID: 551
			// (get) Token: 0x06000FA2 RID: 4002 RVA: 0x0005C340 File Offset: 0x0005A540
			// (set) Token: 0x06000FA3 RID: 4003 RVA: 0x0005C348 File Offset: 0x0005A548
			public DateTime processStartedDateTime { get; set; }
		}
	}
}
