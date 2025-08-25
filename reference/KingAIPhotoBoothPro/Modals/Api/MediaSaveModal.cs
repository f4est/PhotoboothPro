using System;
using System.Net;
using KingAIPhotoBoothPro.Variations;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x0200008E RID: 142
	public class MediaSaveModal
	{
		// Token: 0x0200023F RID: 575
		public class MediaSaveRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x17000230 RID: 560
			// (get) Token: 0x06000FBB RID: 4027 RVA: 0x0005C442 File Offset: 0x0005A642
			// (set) Token: 0x06000FBC RID: 4028 RVA: 0x0005C44A File Offset: 0x0005A64A
			public string EventId { get; set; }

			// Token: 0x17000231 RID: 561
			// (get) Token: 0x06000FBD RID: 4029 RVA: 0x0005C453 File Offset: 0x0005A653
			// (set) Token: 0x06000FBE RID: 4030 RVA: 0x0005C45B File Offset: 0x0005A65B
			public string RawUrl { get; set; }

			// Token: 0x17000232 RID: 562
			// (get) Token: 0x06000FBF RID: 4031 RVA: 0x0005C464 File Offset: 0x0005A664
			// (set) Token: 0x06000FC0 RID: 4032 RVA: 0x0005C46C File Offset: 0x0005A66C
			public string ResultUrl { get; set; }

			// Token: 0x17000233 RID: 563
			// (get) Token: 0x06000FC1 RID: 4033 RVA: 0x0005C475 File Offset: 0x0005A675
			// (set) Token: 0x06000FC2 RID: 4034 RVA: 0x0005C47D File Offset: 0x0005A67D
			public string ThumbnailUrl { get; set; }

			// Token: 0x17000234 RID: 564
			// (get) Token: 0x06000FC3 RID: 4035 RVA: 0x0005C486 File Offset: 0x0005A686
			// (set) Token: 0x06000FC4 RID: 4036 RVA: 0x0005C48E File Offset: 0x0005A68E
			public FileInformation RawFileInfo { get; set; }

			// Token: 0x17000235 RID: 565
			// (get) Token: 0x06000FC5 RID: 4037 RVA: 0x0005C497 File Offset: 0x0005A697
			// (set) Token: 0x06000FC6 RID: 4038 RVA: 0x0005C49F File Offset: 0x0005A69F
			public FileInformation FileInfo { get; set; }

			// Token: 0x17000236 RID: 566
			// (get) Token: 0x06000FC7 RID: 4039 RVA: 0x0005C4A8 File Offset: 0x0005A6A8
			// (set) Token: 0x06000FC8 RID: 4040 RVA: 0x0005C4B0 File Offset: 0x0005A6B0
			public ParticipantInformation ParticipantInfo { get; set; }
		}

		// Token: 0x02000240 RID: 576
		public class MediaSaveResponse
		{
			// Token: 0x17000237 RID: 567
			// (get) Token: 0x06000FCA RID: 4042 RVA: 0x0005C4C1 File Offset: 0x0005A6C1
			// (set) Token: 0x06000FCB RID: 4043 RVA: 0x0005C4C9 File Offset: 0x0005A6C9
			public bool Success { get; set; }

			// Token: 0x17000238 RID: 568
			// (get) Token: 0x06000FCC RID: 4044 RVA: 0x0005C4D2 File Offset: 0x0005A6D2
			// (set) Token: 0x06000FCD RID: 4045 RVA: 0x0005C4DA File Offset: 0x0005A6DA
			public string Message { get; set; }

			// Token: 0x17000239 RID: 569
			// (get) Token: 0x06000FCE RID: 4046 RVA: 0x0005C4E3 File Offset: 0x0005A6E3
			// (set) Token: 0x06000FCF RID: 4047 RVA: 0x0005C4EB File Offset: 0x0005A6EB
			public MediaData MediaData { get; set; }
		}

		// Token: 0x02000241 RID: 577
		public class MediaVariationSaveRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x1700023A RID: 570
			// (get) Token: 0x06000FD1 RID: 4049 RVA: 0x0005C4FC File Offset: 0x0005A6FC
			// (set) Token: 0x06000FD2 RID: 4050 RVA: 0x0005C504 File Offset: 0x0005A704
			public string MediaHash { get; set; }

			// Token: 0x1700023B RID: 571
			// (get) Token: 0x06000FD3 RID: 4051 RVA: 0x0005C50D File Offset: 0x0005A70D
			// (set) Token: 0x06000FD4 RID: 4052 RVA: 0x0005C515 File Offset: 0x0005A715
			public FileInformation FileInfo { get; set; }

			// Token: 0x1700023C RID: 572
			// (get) Token: 0x06000FD5 RID: 4053 RVA: 0x0005C51E File Offset: 0x0005A71E
			// (set) Token: 0x06000FD6 RID: 4054 RVA: 0x0005C526 File Offset: 0x0005A726
			public FileInformation RawFileInfo { get; set; }

			// Token: 0x1700023D RID: 573
			// (get) Token: 0x06000FD7 RID: 4055 RVA: 0x0005C52F File Offset: 0x0005A72F
			// (set) Token: 0x06000FD8 RID: 4056 RVA: 0x0005C537 File Offset: 0x0005A737
			public string Prompt { get; set; }

			// Token: 0x1700023E RID: 574
			// (get) Token: 0x06000FD9 RID: 4057 RVA: 0x0005C540 File Offset: 0x0005A740
			// (set) Token: 0x06000FDA RID: 4058 RVA: 0x0005C548 File Offset: 0x0005A748
			public string RawUrl { get; set; }

			// Token: 0x1700023F RID: 575
			// (get) Token: 0x06000FDB RID: 4059 RVA: 0x0005C551 File Offset: 0x0005A751
			// (set) Token: 0x06000FDC RID: 4060 RVA: 0x0005C559 File Offset: 0x0005A759
			public string ResultUrl { get; set; }

			// Token: 0x17000240 RID: 576
			// (get) Token: 0x06000FDD RID: 4061 RVA: 0x0005C562 File Offset: 0x0005A762
			// (set) Token: 0x06000FDE RID: 4062 RVA: 0x0005C56A File Offset: 0x0005A76A
			public string ThumbnailUrl { get; set; }

			// Token: 0x17000241 RID: 577
			// (get) Token: 0x06000FDF RID: 4063 RVA: 0x0005C573 File Offset: 0x0005A773
			// (set) Token: 0x06000FE0 RID: 4064 RVA: 0x0005C57B File Offset: 0x0005A77B
			public VariationType VariationOperationType { get; set; }
		}

		// Token: 0x02000242 RID: 578
		public class MediaVariationSaveResponse : ForgotUnlockDashboardModals.GenericResponse
		{
			// Token: 0x06000FE2 RID: 4066 RVA: 0x0005C58C File Offset: 0x0005A78C
			public MediaVariationSaveResponse()
			{
			}

			// Token: 0x06000FE3 RID: 4067 RVA: 0x0005C594 File Offset: 0x0005A794
			public MediaVariationSaveResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message) : base(success, httpStatusCode, dtStart, message)
			{
			}

			// Token: 0x17000242 RID: 578
			// (get) Token: 0x06000FE4 RID: 4068 RVA: 0x0005C5A1 File Offset: 0x0005A7A1
			// (set) Token: 0x06000FE5 RID: 4069 RVA: 0x0005C5A9 File Offset: 0x0005A7A9
			public MediaVariationData MediaVariationData { get; set; }
		}
	}
}
