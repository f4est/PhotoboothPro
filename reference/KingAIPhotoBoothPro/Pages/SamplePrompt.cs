using System;
using System.Collections.Generic;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000028 RID: 40
	public class SamplePrompt
	{
		// Token: 0x1700005B RID: 91
		// (get) Token: 0x0600025E RID: 606 RVA: 0x0000CEDB File Offset: 0x0000B0DB
		// (set) Token: 0x0600025F RID: 607 RVA: 0x0000CEE3 File Offset: 0x0000B0E3
		public int Index { get; set; }

		// Token: 0x1700005C RID: 92
		// (get) Token: 0x06000260 RID: 608 RVA: 0x0000CEEC File Offset: 0x0000B0EC
		// (set) Token: 0x06000261 RID: 609 RVA: 0x0000CEF4 File Offset: 0x0000B0F4
		public string Title { get; set; }

		// Token: 0x1700005D RID: 93
		// (get) Token: 0x06000262 RID: 610 RVA: 0x0000CEFD File Offset: 0x0000B0FD
		// (set) Token: 0x06000263 RID: 611 RVA: 0x0000CF05 File Offset: 0x0000B105
		public List<string> Prompts { get; set; }

		// Token: 0x1700005E RID: 94
		// (get) Token: 0x06000264 RID: 612 RVA: 0x0000CF0E File Offset: 0x0000B10E
		// (set) Token: 0x06000265 RID: 613 RVA: 0x0000CF16 File Offset: 0x0000B116
		public FileInformation FileDetails { get; set; }
	}
}
