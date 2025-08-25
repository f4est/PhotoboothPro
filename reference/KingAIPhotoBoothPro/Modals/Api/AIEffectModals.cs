using System;
using System.Collections.Generic;
using System.Net;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000087 RID: 135
	public class AIEffectModals
	{
		// Token: 0x02000223 RID: 547
		public class AiEffectResultResponse : ForgotUnlockDashboardModals.GenericResponse
		{
			// Token: 0x170001AB RID: 427
			// (get) Token: 0x06000E94 RID: 3732 RVA: 0x0005BA20 File Offset: 0x00059C20
			// (set) Token: 0x06000E95 RID: 3733 RVA: 0x0005BA28 File Offset: 0x00059C28
			public bool Finished { get; set; }

			// Token: 0x170001AC RID: 428
			// (get) Token: 0x06000E96 RID: 3734 RVA: 0x0005BA31 File Offset: 0x00059C31
			// (set) Token: 0x06000E97 RID: 3735 RVA: 0x0005BA39 File Offset: 0x00059C39
			public int Percentage { get; set; }

			// Token: 0x170001AD RID: 429
			// (get) Token: 0x06000E98 RID: 3736 RVA: 0x0005BA42 File Offset: 0x00059C42
			// (set) Token: 0x06000E99 RID: 3737 RVA: 0x0005BA4A File Offset: 0x00059C4A
			public string[] ResultImages { get; set; }

			// Token: 0x170001AE RID: 430
			// (get) Token: 0x06000E9A RID: 3738 RVA: 0x0005BA53 File Offset: 0x00059C53
			// (set) Token: 0x06000E9B RID: 3739 RVA: 0x0005BA5B File Offset: 0x00059C5B
			public bool FaceSwapApplied { get; set; }

			// Token: 0x170001AF RID: 431
			// (get) Token: 0x06000E9C RID: 3740 RVA: 0x0005BA64 File Offset: 0x00059C64
			// (set) Token: 0x06000E9D RID: 3741 RVA: 0x0005BA6C File Offset: 0x00059C6C
			[Obsolete]
			public string U1Url { get; set; }

			// Token: 0x170001B0 RID: 432
			// (get) Token: 0x06000E9E RID: 3742 RVA: 0x0005BA75 File Offset: 0x00059C75
			public int ResultCount
			{
				get
				{
					if (this.ResultImages == null)
					{
						return 0;
					}
					return this.ResultImages.Length;
				}
			}

			// Token: 0x06000E9F RID: 3743 RVA: 0x0005BA89 File Offset: 0x00059C89
			public AiEffectResultResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
			{
			}
		}

		// Token: 0x02000224 RID: 548
		public class AiEffectResultRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x170001B1 RID: 433
			// (get) Token: 0x06000EA0 RID: 3744 RVA: 0x0005BA96 File Offset: 0x00059C96
			// (set) Token: 0x06000EA1 RID: 3745 RVA: 0x0005BA9E File Offset: 0x00059C9E
			public string TaskId { get; set; }

			// Token: 0x170001B2 RID: 434
			// (get) Token: 0x06000EA2 RID: 3746 RVA: 0x0005BAA7 File Offset: 0x00059CA7
			// (set) Token: 0x06000EA3 RID: 3747 RVA: 0x0005BAAF File Offset: 0x00059CAF
			public string AiEffectId { get; set; }

			// Token: 0x170001B3 RID: 435
			// (get) Token: 0x06000EA4 RID: 3748 RVA: 0x0005BAB8 File Offset: 0x00059CB8
			// (set) Token: 0x06000EA5 RID: 3749 RVA: 0x0005BAC0 File Offset: 0x00059CC0
			public string MediaHash { get; set; }
		}

		// Token: 0x02000225 RID: 549
		public class AiEffectRunRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x170001B4 RID: 436
			// (get) Token: 0x06000EA7 RID: 3751 RVA: 0x0005BAD1 File Offset: 0x00059CD1
			// (set) Token: 0x06000EA8 RID: 3752 RVA: 0x0005BAD9 File Offset: 0x00059CD9
			public string AiEffectId { get; set; }

			// Token: 0x170001B5 RID: 437
			// (get) Token: 0x06000EA9 RID: 3753 RVA: 0x0005BAE2 File Offset: 0x00059CE2
			// (set) Token: 0x06000EAA RID: 3754 RVA: 0x0005BAEA File Offset: 0x00059CEA
			public string MediaHash { get; set; }
		}

		// Token: 0x02000226 RID: 550
		public class AiEffectRunResponse : ForgotUnlockDashboardModals.GenericResponse
		{
			// Token: 0x06000EAC RID: 3756 RVA: 0x0005BAFB File Offset: 0x00059CFB
			public AiEffectRunResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
			{
			}

			// Token: 0x170001B6 RID: 438
			// (get) Token: 0x06000EAD RID: 3757 RVA: 0x0005BB08 File Offset: 0x00059D08
			// (set) Token: 0x06000EAE RID: 3758 RVA: 0x0005BB10 File Offset: 0x00059D10
			public string TaskId { get; set; }
		}

		// Token: 0x02000227 RID: 551
		public class ApplyAiEffectResponse : ForgotUnlockDashboardModals.GenericResponse
		{
			// Token: 0x170001B7 RID: 439
			// (get) Token: 0x06000EAF RID: 3759 RVA: 0x0005BB19 File Offset: 0x00059D19
			// (set) Token: 0x06000EB0 RID: 3760 RVA: 0x0005BB21 File Offset: 0x00059D21
			public string PhotoUrl { get; set; }

			// Token: 0x170001B8 RID: 440
			// (get) Token: 0x06000EB1 RID: 3761 RVA: 0x0005BB2A File Offset: 0x00059D2A
			// (set) Token: 0x06000EB2 RID: 3762 RVA: 0x0005BB32 File Offset: 0x00059D32
			public string U1Url { get; set; }

			// Token: 0x170001B9 RID: 441
			// (get) Token: 0x06000EB3 RID: 3763 RVA: 0x0005BB3B File Offset: 0x00059D3B
			// (set) Token: 0x06000EB4 RID: 3764 RVA: 0x0005BB43 File Offset: 0x00059D43
			public string U2Url { get; set; }

			// Token: 0x170001BA RID: 442
			// (get) Token: 0x06000EB5 RID: 3765 RVA: 0x0005BB4C File Offset: 0x00059D4C
			// (set) Token: 0x06000EB6 RID: 3766 RVA: 0x0005BB54 File Offset: 0x00059D54
			public string U3Url { get; set; }

			// Token: 0x170001BB RID: 443
			// (get) Token: 0x06000EB7 RID: 3767 RVA: 0x0005BB5D File Offset: 0x00059D5D
			// (set) Token: 0x06000EB8 RID: 3768 RVA: 0x0005BB65 File Offset: 0x00059D65
			public string U4Url { get; set; }

			// Token: 0x170001BC RID: 444
			// (get) Token: 0x06000EB9 RID: 3769 RVA: 0x0005BB6E File Offset: 0x00059D6E
			// (set) Token: 0x06000EBA RID: 3770 RVA: 0x0005BB76 File Offset: 0x00059D76
			public string Prompt { get; set; }

			// Token: 0x170001BD RID: 445
			// (get) Token: 0x06000EBB RID: 3771 RVA: 0x0005BB7F File Offset: 0x00059D7F
			// (set) Token: 0x06000EBC RID: 3772 RVA: 0x0005BB87 File Offset: 0x00059D87
			public VariationMediaHelper.MidjourneyTask MidjourneyTask { get; set; }

			// Token: 0x06000EBD RID: 3773 RVA: 0x0005BB90 File Offset: 0x00059D90
			public ApplyAiEffectResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
			{
			}
		}

		// Token: 0x02000228 RID: 552
		public class GetAllAiEffectsResponse : ForgotUnlockDashboardModals.GenericResponse
		{
			// Token: 0x06000EBE RID: 3774 RVA: 0x0005BB9D File Offset: 0x00059D9D
			public GetAllAiEffectsResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
			{
			}

			// Token: 0x170001BE RID: 446
			// (get) Token: 0x06000EBF RID: 3775 RVA: 0x0005BBAA File Offset: 0x00059DAA
			// (set) Token: 0x06000EC0 RID: 3776 RVA: 0x0005BBB2 File Offset: 0x00059DB2
			public List<AiEffectLocal> AiEffects { get; set; }
		}

		// Token: 0x02000229 RID: 553
		public class GetAllAiEffectsRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
		}

		// Token: 0x0200022A RID: 554
		public class ApplyAiEffectRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x06000EC2 RID: 3778 RVA: 0x0005BBC3 File Offset: 0x00059DC3
			public ApplyAiEffectRequest(string token, string aiEffectId, string mediaHash)
			{
				base.AccessToken = token;
				this.AiEffectId = aiEffectId;
				this.MediaHash = mediaHash;
			}

			// Token: 0x170001BF RID: 447
			// (get) Token: 0x06000EC3 RID: 3779 RVA: 0x0005BBE0 File Offset: 0x00059DE0
			// (set) Token: 0x06000EC4 RID: 3780 RVA: 0x0005BBE8 File Offset: 0x00059DE8
			public string AiEffectId { get; set; }

			// Token: 0x170001C0 RID: 448
			// (get) Token: 0x06000EC5 RID: 3781 RVA: 0x0005BBF1 File Offset: 0x00059DF1
			// (set) Token: 0x06000EC6 RID: 3782 RVA: 0x0005BBF9 File Offset: 0x00059DF9
			public string MediaHash { get; set; }
		}
	}
}
