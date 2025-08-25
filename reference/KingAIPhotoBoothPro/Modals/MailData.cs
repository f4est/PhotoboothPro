using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Pages;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000070 RID: 112
	public class MailData
	{
		// Token: 0x170000EF RID: 239
		// (get) Token: 0x06000820 RID: 2080 RVA: 0x00033500 File Offset: 0x00031700
		// (set) Token: 0x06000821 RID: 2081 RVA: 0x00033508 File Offset: 0x00031708
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[Key]
		public string Id { get; set; }

		// Token: 0x170000F0 RID: 240
		// (get) Token: 0x06000822 RID: 2082 RVA: 0x00033511 File Offset: 0x00031711
		// (set) Token: 0x06000823 RID: 2083 RVA: 0x00033519 File Offset: 0x00031719
		public string EventID { get; set; }

		// Token: 0x170000F1 RID: 241
		// (get) Token: 0x06000824 RID: 2084 RVA: 0x00033522 File Offset: 0x00031722
		// (set) Token: 0x06000825 RID: 2085 RVA: 0x0003352A File Offset: 0x0003172A
		public DateTime MailDatetime { get; set; }

		// Token: 0x170000F2 RID: 242
		// (get) Token: 0x06000826 RID: 2086 RVA: 0x00033533 File Offset: 0x00031733
		// (set) Token: 0x06000827 RID: 2087 RVA: 0x0003353B File Offset: 0x0003173B
		public FileInformation FileInf { get; set; }

		// Token: 0x170000F3 RID: 243
		// (get) Token: 0x06000828 RID: 2088 RVA: 0x00033544 File Offset: 0x00031744
		// (set) Token: 0x06000829 RID: 2089 RVA: 0x0003354C File Offset: 0x0003174C
		public MailInfo mailInfo { get; set; }

		// Token: 0x170000F4 RID: 244
		// (get) Token: 0x0600082A RID: 2090 RVA: 0x00033555 File Offset: 0x00031755
		// (set) Token: 0x0600082B RID: 2091 RVA: 0x0003355D File Offset: 0x0003175D
		public bool IsCloudSync { get; set; }

		// Token: 0x170000F5 RID: 245
		// (get) Token: 0x0600082C RID: 2092 RVA: 0x00033566 File Offset: 0x00031766
		// (set) Token: 0x0600082D RID: 2093 RVA: 0x0003356E File Offset: 0x0003176E
		public bool isMailSend { get; set; }

		// Token: 0x170000F6 RID: 246
		// (get) Token: 0x0600082E RID: 2094 RVA: 0x00033577 File Offset: 0x00031777
		// (set) Token: 0x0600082F RID: 2095 RVA: 0x0003357F File Offset: 0x0003177F
		public string Message { get; set; }

		// Token: 0x06000830 RID: 2096 RVA: 0x00033588 File Offset: 0x00031788
		public MediaClassBase GetMedia()
		{
			return (from x in DSLR.eventMediaClass.mediaList
			where x.resultFile.Name == this.FileInf.Filename
			select x).FirstOrDefault<MediaClassBase>();
		}
	}
}
