using System;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000071 RID: 113
	public class MailInfo
	{
		// Token: 0x170000F7 RID: 247
		// (get) Token: 0x06000833 RID: 2099 RVA: 0x000335CF File Offset: 0x000317CF
		// (set) Token: 0x06000834 RID: 2100 RVA: 0x000335D7 File Offset: 0x000317D7
		public string HostMailAddress { get; set; }

		// Token: 0x170000F8 RID: 248
		// (get) Token: 0x06000835 RID: 2101 RVA: 0x000335E0 File Offset: 0x000317E0
		// (set) Token: 0x06000836 RID: 2102 RVA: 0x000335E8 File Offset: 0x000317E8
		public string SmtpPassword { get; set; }

		// Token: 0x170000F9 RID: 249
		// (get) Token: 0x06000837 RID: 2103 RVA: 0x000335F1 File Offset: 0x000317F1
		// (set) Token: 0x06000838 RID: 2104 RVA: 0x000335F9 File Offset: 0x000317F9
		public string SmtpHost { get; set; }

		// Token: 0x170000FA RID: 250
		// (get) Token: 0x06000839 RID: 2105 RVA: 0x00033602 File Offset: 0x00031802
		// (set) Token: 0x0600083A RID: 2106 RVA: 0x0003360A File Offset: 0x0003180A
		public int SmtpPort { get; set; }

		// Token: 0x170000FB RID: 251
		// (get) Token: 0x0600083B RID: 2107 RVA: 0x00033613 File Offset: 0x00031813
		// (set) Token: 0x0600083C RID: 2108 RVA: 0x0003361B File Offset: 0x0003181B
		public string Subject { get; set; }

		// Token: 0x170000FC RID: 252
		// (get) Token: 0x0600083D RID: 2109 RVA: 0x00033624 File Offset: 0x00031824
		// (set) Token: 0x0600083E RID: 2110 RVA: 0x0003362C File Offset: 0x0003182C
		public string Body { get; set; }

		// Token: 0x170000FD RID: 253
		// (get) Token: 0x0600083F RID: 2111 RVA: 0x00033635 File Offset: 0x00031835
		// (set) Token: 0x06000840 RID: 2112 RVA: 0x0003363D File Offset: 0x0003183D
		public string MailAddress { get; set; }
	}
}
