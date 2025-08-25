using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000077 RID: 119
	public class PrintData
	{
		// Token: 0x1700011E RID: 286
		// (get) Token: 0x06000886 RID: 2182 RVA: 0x0003388E File Offset: 0x00031A8E
		// (set) Token: 0x06000887 RID: 2183 RVA: 0x00033896 File Offset: 0x00031A96
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[Key]
		public string Id { get; set; }

		// Token: 0x1700011F RID: 287
		// (get) Token: 0x06000888 RID: 2184 RVA: 0x0003389F File Offset: 0x00031A9F
		// (set) Token: 0x06000889 RID: 2185 RVA: 0x000338A7 File Offset: 0x00031AA7
		public string EventID { get; set; }

		// Token: 0x17000120 RID: 288
		// (get) Token: 0x0600088A RID: 2186 RVA: 0x000338B0 File Offset: 0x00031AB0
		// (set) Token: 0x0600088B RID: 2187 RVA: 0x000338B8 File Offset: 0x00031AB8
		public DateTime PrintDatetime { get; set; }

		// Token: 0x17000121 RID: 289
		// (get) Token: 0x0600088C RID: 2188 RVA: 0x000338C1 File Offset: 0x00031AC1
		// (set) Token: 0x0600088D RID: 2189 RVA: 0x000338C9 File Offset: 0x00031AC9
		public FileInformation PrintFileInfo { get; set; }

		// Token: 0x17000122 RID: 290
		// (get) Token: 0x0600088E RID: 2190 RVA: 0x000338D2 File Offset: 0x00031AD2
		// (set) Token: 0x0600088F RID: 2191 RVA: 0x000338DA File Offset: 0x00031ADA
		public PrinterInfo PrinterInfo { get; set; }

		// Token: 0x17000123 RID: 291
		// (get) Token: 0x06000890 RID: 2192 RVA: 0x000338E3 File Offset: 0x00031AE3
		// (set) Token: 0x06000891 RID: 2193 RVA: 0x000338EB File Offset: 0x00031AEB
		public bool IsCloudSync { get; set; }
	}
}
