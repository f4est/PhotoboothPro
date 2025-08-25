using System;
using KingAIPhotoBoothPro.Class;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200007A RID: 122
	public class TemplateGalleryTarget
	{
		// Token: 0x1700012A RID: 298
		// (get) Token: 0x060008A1 RID: 2209 RVA: 0x00033972 File Offset: 0x00031B72
		// (set) Token: 0x060008A2 RID: 2210 RVA: 0x0003397A File Offset: 0x00031B7A
		public int Index { get; set; }

		// Token: 0x1700012B RID: 299
		// (get) Token: 0x060008A3 RID: 2211 RVA: 0x00033983 File Offset: 0x00031B83
		// (set) Token: 0x060008A4 RID: 2212 RVA: 0x0003398B File Offset: 0x00031B8B
		public string photoPath { get; set; }

		// Token: 0x1700012C RID: 300
		// (get) Token: 0x060008A5 RID: 2213 RVA: 0x00033994 File Offset: 0x00031B94
		// (set) Token: 0x060008A6 RID: 2214 RVA: 0x0003399C File Offset: 0x00031B9C
		public TemplateJsonObject ThisTemplate { get; set; }
	}
}
