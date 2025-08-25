using System;
using System.Collections.Generic;
using Amazon.Rekognition.Model;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200006E RID: 110
	public class FaceSwapTarget : Base
	{
		// Token: 0x170000E6 RID: 230
		// (get) Token: 0x0600080B RID: 2059 RVA: 0x0003332B File Offset: 0x0003152B
		// (set) Token: 0x0600080C RID: 2060 RVA: 0x00033333 File Offset: 0x00031533
		[JsonProperty("index")]
		public int Index { get; set; }

		// Token: 0x170000E7 RID: 231
		// (get) Token: 0x0600080D RID: 2061 RVA: 0x0003333C File Offset: 0x0003153C
		// (set) Token: 0x0600080E RID: 2062 RVA: 0x00033344 File Offset: 0x00031544
		[JsonProperty("title")]
		public string Title { get; set; }

		// Token: 0x170000E8 RID: 232
		// (get) Token: 0x0600080F RID: 2063 RVA: 0x0003334D File Offset: 0x0003154D
		// (set) Token: 0x06000810 RID: 2064 RVA: 0x00033355 File Offset: 0x00031555
		[JsonProperty("filename")]
		[Obsolete]
		public string Filename { get; set; }

		// Token: 0x170000E9 RID: 233
		// (get) Token: 0x06000811 RID: 2065 RVA: 0x00033360 File Offset: 0x00031560
		public string CdnUrl
		{
			get
			{
				if (string.IsNullOrEmpty(this.RelativeUrl))
				{
					return null;
				}
				Uri baseUri = new Uri("https://cdn.activationshare.com");
				Uri fullUri = new Uri(baseUri, this.RelativeUrl);
				return fullUri.ToString();
			}
		}

		// Token: 0x170000EA RID: 234
		// (get) Token: 0x06000812 RID: 2066 RVA: 0x0003339C File Offset: 0x0003159C
		// (set) Token: 0x06000813 RID: 2067 RVA: 0x000333A4 File Offset: 0x000315A4
		[JsonProperty("eventId")]
		public string EventId { get; set; }

		// Token: 0x170000EB RID: 235
		// (get) Token: 0x06000814 RID: 2068 RVA: 0x000333AD File Offset: 0x000315AD
		// (set) Token: 0x06000815 RID: 2069 RVA: 0x000333B5 File Offset: 0x000315B5
		[JsonProperty("relativeUrl")]
		public string RelativeUrl { get; set; }

		// Token: 0x170000EC RID: 236
		// (get) Token: 0x06000816 RID: 2070 RVA: 0x000333BE File Offset: 0x000315BE
		// (set) Token: 0x06000817 RID: 2071 RVA: 0x000333C6 File Offset: 0x000315C6
		public List<FaceDetail> FaceDetails { get; set; }

		// Token: 0x170000ED RID: 237
		// (get) Token: 0x06000818 RID: 2072 RVA: 0x000333CF File Offset: 0x000315CF
		// (set) Token: 0x06000819 RID: 2073 RVA: 0x000333D7 File Offset: 0x000315D7
		[JsonProperty("disabledFaceIndexes")]
		public List<int> DisabledFaceIndexes { get; set; } = new List<int>();

		// Token: 0x0600081A RID: 2074 RVA: 0x000333E0 File Offset: 0x000315E0
		internal void SetCloudParameters(FaceSwapTarget faceSwapTargetImage)
		{
			base.Id = faceSwapTargetImage.Id;
			base.ChangeTime = faceSwapTargetImage.ChangeTime;
			base.IsDeleted = faceSwapTargetImage.IsDeleted;
			base.GeneratedTime = faceSwapTargetImage.GeneratedTime;
			base.DeletedTime = faceSwapTargetImage.DeletedTime;
			this.EventId = faceSwapTargetImage.EventId;
			this.RelativeUrl = faceSwapTargetImage.RelativeUrl;
			this.FaceDetails = faceSwapTargetImage.FaceDetails;
			this.DisabledFaceIndexes = faceSwapTargetImage.DisabledFaceIndexes;
		}

		// Token: 0x0600081B RID: 2075 RVA: 0x00033459 File Offset: 0x00031659
		public void SetLocalParameters(FileInformation fileDetails)
		{
			this.FileDetails = fileDetails;
		}

		// Token: 0x04000867 RID: 2151
		[JsonProperty("fileDetails")]
		public FileInformation FileDetails;
	}
}
