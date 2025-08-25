using System;
using System.Collections.Generic;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200007B RID: 123
	public class WordPortraitModal
	{
		// Token: 0x1700012D RID: 301
		// (get) Token: 0x060008A8 RID: 2216 RVA: 0x000339AD File Offset: 0x00031BAD
		// (set) Token: 0x060008A9 RID: 2217 RVA: 0x000339B5 File Offset: 0x00031BB5
		public List<string> Words { get; set; }

		// Token: 0x1700012E RID: 302
		// (get) Token: 0x060008AA RID: 2218 RVA: 0x000339BE File Offset: 0x00031BBE
		// (set) Token: 0x060008AB RID: 2219 RVA: 0x000339C6 File Offset: 0x00031BC6
		public string FontName { get; set; }

		// Token: 0x1700012F RID: 303
		// (get) Token: 0x060008AC RID: 2220 RVA: 0x000339CF File Offset: 0x00031BCF
		// (set) Token: 0x060008AD RID: 2221 RVA: 0x000339D7 File Offset: 0x00031BD7
		public string FontColor { get; set; }

		// Token: 0x17000130 RID: 304
		// (get) Token: 0x060008AE RID: 2222 RVA: 0x000339E0 File Offset: 0x00031BE0
		// (set) Token: 0x060008AF RID: 2223 RVA: 0x000339E8 File Offset: 0x00031BE8
		public string BackgroundColor { get; set; }

		// Token: 0x17000131 RID: 305
		// (get) Token: 0x060008B0 RID: 2224 RVA: 0x000339F1 File Offset: 0x00031BF1
		// (set) Token: 0x060008B1 RID: 2225 RVA: 0x000339F9 File Offset: 0x00031BF9
		public string wordPortraitPath { get; set; }
	}
}
