using System;
using KingAIPhotoBoothPro.Class.ActivationKing;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000086 RID: 134
	public class AIBeautifierModals
	{
		// Token: 0x02000221 RID: 545
		public class AIBeautifierApiRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x06000E8B RID: 3723 RVA: 0x0005B9C5 File Offset: 0x00059BC5
			public AIBeautifierApiRequest(string sourceImage, string accessToken)
			{
				base.AccessToken = accessToken;
				this.SourceImg = sourceImage;
			}

			// Token: 0x170001A8 RID: 424
			// (get) Token: 0x06000E8C RID: 3724 RVA: 0x0005B9DB File Offset: 0x00059BDB
			// (set) Token: 0x06000E8D RID: 3725 RVA: 0x0005B9E3 File Offset: 0x00059BE3
			public string SourceImg { get; set; }
		}

		// Token: 0x02000222 RID: 546
		public class AIBeautifierResponse : GenericResponse
		{
			// Token: 0x170001A9 RID: 425
			// (get) Token: 0x06000E8E RID: 3726 RVA: 0x0005B9EC File Offset: 0x00059BEC
			// (set) Token: 0x06000E8F RID: 3727 RVA: 0x0005B9F4 File Offset: 0x00059BF4
			public string ResultImageUrl { get; set; }

			// Token: 0x170001AA RID: 426
			// (get) Token: 0x06000E90 RID: 3728 RVA: 0x0005B9FD File Offset: 0x00059BFD
			// (set) Token: 0x06000E91 RID: 3729 RVA: 0x0005BA05 File Offset: 0x00059C05
			public int FaceSwapCount { get; set; }

			// Token: 0x06000E92 RID: 3730 RVA: 0x0005BA0E File Offset: 0x00059C0E
			public AIBeautifierResponse()
			{
			}

			// Token: 0x06000E93 RID: 3731 RVA: 0x0005BA16 File Offset: 0x00059C16
			public AIBeautifierResponse(bool success, string message = "") : base(success, message)
			{
			}
		}
	}
}
