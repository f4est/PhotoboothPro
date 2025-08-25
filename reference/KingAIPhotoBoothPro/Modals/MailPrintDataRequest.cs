using System;
using System.Collections.Generic;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000072 RID: 114
	public class MailPrintDataRequest
	{
		// Token: 0x170000FE RID: 254
		// (get) Token: 0x06000842 RID: 2114 RVA: 0x0003364E File Offset: 0x0003184E
		// (set) Token: 0x06000843 RID: 2115 RVA: 0x00033656 File Offset: 0x00031856
		public List<PrintData> printData { get; set; }

		// Token: 0x170000FF RID: 255
		// (get) Token: 0x06000844 RID: 2116 RVA: 0x0003365F File Offset: 0x0003185F
		// (set) Token: 0x06000845 RID: 2117 RVA: 0x00033667 File Offset: 0x00031867
		public List<MailData> mailData { get; set; }
	}
}
