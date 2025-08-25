using System;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200006A RID: 106
	public class Base
	{
		// Token: 0x060007E8 RID: 2024 RVA: 0x000331C8 File Offset: 0x000313C8
		public Base()
		{
			this.GeneratedTime = DateTime.Now;
			this.ChangeTime = new DateTime?(DateTime.Now);
			this.DeletedTime = new DateTime?(DateTime.Now);
			this.IsDeleted = new bool?(false);
		}

		// Token: 0x170000D7 RID: 215
		// (get) Token: 0x060007E9 RID: 2025 RVA: 0x00033207 File Offset: 0x00031407
		// (set) Token: 0x060007EA RID: 2026 RVA: 0x0003320F File Offset: 0x0003140F
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		[Key]
		public string Id { get; set; }

		// Token: 0x170000D8 RID: 216
		// (get) Token: 0x060007EB RID: 2027 RVA: 0x00033218 File Offset: 0x00031418
		// (set) Token: 0x060007EC RID: 2028 RVA: 0x00033220 File Offset: 0x00031420
		public DateTime GeneratedTime { get; set; }

		// Token: 0x170000D9 RID: 217
		// (get) Token: 0x060007ED RID: 2029 RVA: 0x00033229 File Offset: 0x00031429
		// (set) Token: 0x060007EE RID: 2030 RVA: 0x00033231 File Offset: 0x00031431
		public DateTime? ChangeTime { get; set; }

		// Token: 0x170000DA RID: 218
		// (get) Token: 0x060007EF RID: 2031 RVA: 0x0003323A File Offset: 0x0003143A
		// (set) Token: 0x060007F0 RID: 2032 RVA: 0x00033242 File Offset: 0x00031442
		public bool? IsDeleted { get; set; }

		// Token: 0x170000DB RID: 219
		// (get) Token: 0x060007F1 RID: 2033 RVA: 0x0003324B File Offset: 0x0003144B
		// (set) Token: 0x060007F2 RID: 2034 RVA: 0x00033253 File Offset: 0x00031453
		public DateTime? DeletedTime { get; set; }
	}
}
