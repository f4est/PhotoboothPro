using System;
using System.Collections.Generic;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000AA RID: 170
	public class TemplateJsonObject
	{
		// Token: 0x04000987 RID: 2439
		public string PageInfo;

		// Token: 0x04000988 RID: 2440
		public int PageWidthPhoto = 1200;

		// Token: 0x04000989 RID: 2441
		public int PageHeightPhoto = 1800;

		// Token: 0x0400098A RID: 2442
		public int PageWidthVideo = 1080;

		// Token: 0x0400098B RID: 2443
		public int PageHeightVideo = 1920;

		// Token: 0x0400098C RID: 2444
		public int DPI = 300;

		// Token: 0x0400098D RID: 2445
		public List<TemplateObject> templatePhotoObjects;

		// Token: 0x0400098E RID: 2446
		public List<TemplateObject> templateVideoObjects;
	}
}
