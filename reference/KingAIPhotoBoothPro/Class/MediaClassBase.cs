using System;
using System.IO;
using KingAIPhotoBoothPro.Modals;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B7 RID: 183
	[Serializable]
	public class MediaClassBase
	{
		// Token: 0x17000174 RID: 372
		// (get) Token: 0x06000A3A RID: 2618 RVA: 0x0003B60E File Offset: 0x0003980E
		// (set) Token: 0x06000A3B RID: 2619 RVA: 0x0003B616 File Offset: 0x00039816
		[JsonProperty("rawUrl")]
		public string RawUrl { get; set; }

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06000A3C RID: 2620 RVA: 0x0003B61F File Offset: 0x0003981F
		// (set) Token: 0x06000A3D RID: 2621 RVA: 0x0003B627 File Offset: 0x00039827
		[JsonProperty("resultUrl")]
		public string ResultUrl { get; set; }

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06000A3E RID: 2622 RVA: 0x0003B630 File Offset: 0x00039830
		// (set) Token: 0x06000A3F RID: 2623 RVA: 0x0003B638 File Offset: 0x00039838
		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl { get; set; }

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06000A40 RID: 2624 RVA: 0x0003B641 File Offset: 0x00039841
		[JsonIgnore]
		public bool isCloudSynced
		{
			get
			{
				return !string.IsNullOrEmpty(this.MediaHash);
			}
		}

		// Token: 0x04000A01 RID: 2561
		[JsonProperty("backgroundColorHex")]
		public string backgroundColorHex;

		// Token: 0x04000A02 RID: 2562
		[JsonProperty("index")]
		public int index;

		// Token: 0x04000A03 RID: 2563
		[JsonProperty("capturedFiles")]
		public FileInfo[] capturedFiles;

		// Token: 0x04000A04 RID: 2564
		[JsonProperty("resultFile")]
		public FileInfo resultFile;

		// Token: 0x04000A05 RID: 2565
		[JsonProperty("mediaType")]
		public MediaType mediaType;

		// Token: 0x04000A06 RID: 2566
		[JsonProperty("dataType")]
		public DataType dataType;

		// Token: 0x04000A07 RID: 2567
		[JsonProperty("thumbnail")]
		public FileInfo thumbnail;

		// Token: 0x04000A08 RID: 2568
		[JsonProperty("isUploadedOrginal")]
		public bool isUploadedOrginal;

		// Token: 0x04000A09 RID: 2569
		[JsonProperty("isUploaded")]
		public bool isUploaded;

		// Token: 0x04000A0A RID: 2570
		[JsonProperty("isCloudSyncing")]
		public bool isCloudSyncing;

		// Token: 0x04000A0B RID: 2571
		[JsonProperty("resultGifFile")]
		public FileInfo ResultGifFile;

		// Token: 0x04000A0C RID: 2572
		[JsonProperty("isUploadedGifFile")]
		public bool isUploadedGifFile;

		// Token: 0x04000A0D RID: 2573
		[JsonProperty("eventHash")]
		public string EventHash;

		// Token: 0x04000A0E RID: 2574
		[JsonProperty("mediaHash")]
		public string MediaHash = "";

		// Token: 0x04000A0F RID: 2575
		[JsonProperty("fileDetails")]
		public FileInformation fileDetails;

		// Token: 0x04000A10 RID: 2576
		[JsonProperty("fileDetailsVideo")]
		public FileInformation fileDetailsVideo;

		// Token: 0x04000A11 RID: 2577
		[JsonProperty("isDeleted")]
		public bool isDeleted;
	}
}
