using System;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000085 RID: 133
	public class AIBackgroundModals
	{
		// Token: 0x0200021E RID: 542
		public class AIBackgroundRequest
		{
			// Token: 0x1700019B RID: 411
			// (get) Token: 0x06000E6E RID: 3694 RVA: 0x0005B8D0 File Offset: 0x00059AD0
			// (set) Token: 0x06000E6F RID: 3695 RVA: 0x0005B8D8 File Offset: 0x00059AD8
			public string AccessToken { get; set; }

			// Token: 0x1700019C RID: 412
			// (get) Token: 0x06000E70 RID: 3696 RVA: 0x0005B8E1 File Offset: 0x00059AE1
			// (set) Token: 0x06000E71 RID: 3697 RVA: 0x0005B8E9 File Offset: 0x00059AE9
			public string Url { get; set; }
		}

		// Token: 0x0200021F RID: 543
		public class AIBackgroundResponse
		{
			// Token: 0x1700019D RID: 413
			// (get) Token: 0x06000E73 RID: 3699 RVA: 0x0005B8FA File Offset: 0x00059AFA
			// (set) Token: 0x06000E74 RID: 3700 RVA: 0x0005B902 File Offset: 0x00059B02
			public string url { get; set; }

			// Token: 0x1700019E RID: 414
			// (get) Token: 0x06000E75 RID: 3701 RVA: 0x0005B90B File Offset: 0x00059B0B
			// (set) Token: 0x06000E76 RID: 3702 RVA: 0x0005B913 File Offset: 0x00059B13
			public bool success { get; set; }

			// Token: 0x1700019F RID: 415
			// (get) Token: 0x06000E77 RID: 3703 RVA: 0x0005B91C File Offset: 0x00059B1C
			// (set) Token: 0x06000E78 RID: 3704 RVA: 0x0005B924 File Offset: 0x00059B24
			public string message { get; set; }

			// Token: 0x170001A0 RID: 416
			// (get) Token: 0x06000E79 RID: 3705 RVA: 0x0005B92D File Offset: 0x00059B2D
			// (set) Token: 0x06000E7A RID: 3706 RVA: 0x0005B935 File Offset: 0x00059B35
			public int statusCode { get; set; }

			// Token: 0x170001A1 RID: 417
			// (get) Token: 0x06000E7B RID: 3707 RVA: 0x0005B93E File Offset: 0x00059B3E
			// (set) Token: 0x06000E7C RID: 3708 RVA: 0x0005B946 File Offset: 0x00059B46
			public string processingTime { get; set; }

			// Token: 0x170001A2 RID: 418
			// (get) Token: 0x06000E7D RID: 3709 RVA: 0x0005B94F File Offset: 0x00059B4F
			// (set) Token: 0x06000E7E RID: 3710 RVA: 0x0005B957 File Offset: 0x00059B57
			public string processingTimeSpan { get; set; }

			// Token: 0x170001A3 RID: 419
			// (get) Token: 0x06000E7F RID: 3711 RVA: 0x0005B960 File Offset: 0x00059B60
			// (set) Token: 0x06000E80 RID: 3712 RVA: 0x0005B968 File Offset: 0x00059B68
			public DateTime processStartedDateTime { get; set; }
		}

		// Token: 0x02000220 RID: 544
		public class ChromaKeyBackgroundRequest
		{
			// Token: 0x170001A4 RID: 420
			// (get) Token: 0x06000E82 RID: 3714 RVA: 0x0005B979 File Offset: 0x00059B79
			// (set) Token: 0x06000E83 RID: 3715 RVA: 0x0005B981 File Offset: 0x00059B81
			public string AccessToken { get; set; }

			// Token: 0x170001A5 RID: 421
			// (get) Token: 0x06000E84 RID: 3716 RVA: 0x0005B98A File Offset: 0x00059B8A
			// (set) Token: 0x06000E85 RID: 3717 RVA: 0x0005B992 File Offset: 0x00059B92
			public string Url { get; set; }

			// Token: 0x170001A6 RID: 422
			// (get) Token: 0x06000E86 RID: 3718 RVA: 0x0005B99B File Offset: 0x00059B9B
			// (set) Token: 0x06000E87 RID: 3719 RVA: 0x0005B9A3 File Offset: 0x00059BA3
			public string Color { get; set; }

			// Token: 0x170001A7 RID: 423
			// (get) Token: 0x06000E88 RID: 3720 RVA: 0x0005B9AC File Offset: 0x00059BAC
			// (set) Token: 0x06000E89 RID: 3721 RVA: 0x0005B9B4 File Offset: 0x00059BB4
			public int ThreasHold { get; set; }
		}
	}
}
