using System;
using System.Collections.Generic;
using Amazon.Rekognition.Model;
using KingAIPhotoBoothPro.Class.ActivationKing;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x0200008F RID: 143
	public class MediaVariationModals
	{
		// Token: 0x02000243 RID: 579
		public class AiPromptResponse : GenericResponse
		{
			// Token: 0x17000243 RID: 579
			// (get) Token: 0x06000FE6 RID: 4070 RVA: 0x0005C5B2 File Offset: 0x0005A7B2
			// (set) Token: 0x06000FE7 RID: 4071 RVA: 0x0005C5BA File Offset: 0x0005A7BA
			public int FaceSwapCount { get; set; }

			// Token: 0x17000244 RID: 580
			// (get) Token: 0x06000FE8 RID: 4072 RVA: 0x0005C5C3 File Offset: 0x0005A7C3
			// (set) Token: 0x06000FE9 RID: 4073 RVA: 0x0005C5CB File Offset: 0x0005A7CB
			public string ResultImageUrl { get; set; }
		}

		// Token: 0x02000244 RID: 580
		public enum MidJourneyProcess
		{
			// Token: 0x04000FA6 RID: 4006
			SendImagineCommand,
			// Token: 0x04000FA7 RID: 4007
			ImagineResultDownload,
			// Token: 0x04000FA8 RID: 4008
			WaitingForVariationSelection,
			// Token: 0x04000FA9 RID: 4009
			SelectedVariationDownload,
			// Token: 0x04000FAA RID: 4010
			FaceSwapping,
			// Token: 0x04000FAB RID: 4011
			ResultPhotoRender,
			// Token: 0x04000FAC RID: 4012
			Completed
		}

		// Token: 0x02000245 RID: 581
		public class FaceSwapApiRequest
		{
			// Token: 0x06000FEB RID: 4075 RVA: 0x0005C5DC File Offset: 0x0005A7DC
			public FaceSwapApiRequest(string sourceImage, MediaVariationModals.FaceSwapRequestImage targetImage, string accessToken)
			{
				this.AccessToken = accessToken;
				this.SourceImg = sourceImage;
				this.TargetImg = targetImage;
			}

			// Token: 0x17000245 RID: 581
			// (get) Token: 0x06000FEC RID: 4076 RVA: 0x0005C5F9 File Offset: 0x0005A7F9
			// (set) Token: 0x06000FED RID: 4077 RVA: 0x0005C601 File Offset: 0x0005A801
			public string SourceImg { get; set; }

			// Token: 0x17000246 RID: 582
			// (get) Token: 0x06000FEE RID: 4078 RVA: 0x0005C60A File Offset: 0x0005A80A
			// (set) Token: 0x06000FEF RID: 4079 RVA: 0x0005C612 File Offset: 0x0005A812
			public MediaVariationModals.FaceSwapRequestImage TargetImg { get; set; }

			// Token: 0x17000247 RID: 583
			// (get) Token: 0x06000FF0 RID: 4080 RVA: 0x0005C61B File Offset: 0x0005A81B
			// (set) Token: 0x06000FF1 RID: 4081 RVA: 0x0005C623 File Offset: 0x0005A823
			public string AccessToken { get; set; }
		}

		// Token: 0x02000246 RID: 582
		public class FaceSwapRequestImage
		{
			// Token: 0x17000248 RID: 584
			// (get) Token: 0x06000FF2 RID: 4082 RVA: 0x0005C62C File Offset: 0x0005A82C
			// (set) Token: 0x06000FF3 RID: 4083 RVA: 0x0005C634 File Offset: 0x0005A834
			public string Url { get; set; }

			// Token: 0x17000249 RID: 585
			// (get) Token: 0x06000FF4 RID: 4084 RVA: 0x0005C63D File Offset: 0x0005A83D
			// (set) Token: 0x06000FF5 RID: 4085 RVA: 0x0005C645 File Offset: 0x0005A845
			public List<int> DisabledFaceIndexes { get; set; } = new List<int>();

			// Token: 0x1700024A RID: 586
			// (get) Token: 0x06000FF6 RID: 4086 RVA: 0x0005C64E File Offset: 0x0005A84E
			// (set) Token: 0x06000FF7 RID: 4087 RVA: 0x0005C656 File Offset: 0x0005A856
			public List<FaceDetail> FaceDetails { get; set; }
		}

		// Token: 0x02000247 RID: 583
		public class FaceSwapResponse : GenericResponse
		{
			// Token: 0x1700024B RID: 587
			// (get) Token: 0x06000FF9 RID: 4089 RVA: 0x0005C672 File Offset: 0x0005A872
			// (set) Token: 0x06000FFA RID: 4090 RVA: 0x0005C67A File Offset: 0x0005A87A
			public string ResultImageUrl { get; set; }

			// Token: 0x1700024C RID: 588
			// (get) Token: 0x06000FFB RID: 4091 RVA: 0x0005C683 File Offset: 0x0005A883
			// (set) Token: 0x06000FFC RID: 4092 RVA: 0x0005C68B File Offset: 0x0005A88B
			public int FaceSwapCount { get; set; }

			// Token: 0x06000FFD RID: 4093 RVA: 0x0005C694 File Offset: 0x0005A894
			public FaceSwapResponse()
			{
			}

			// Token: 0x06000FFE RID: 4094 RVA: 0x0005C69C File Offset: 0x0005A89C
			public FaceSwapResponse(bool success, string message = "") : base(success, message)
			{
			}
		}
	}
}
