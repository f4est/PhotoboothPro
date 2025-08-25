using System;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000036 RID: 54
	public class PaymentSession
	{
		// Token: 0x1700006D RID: 109
		// (get) Token: 0x0600034E RID: 846 RVA: 0x0001227A File Offset: 0x0001047A
		// (set) Token: 0x0600034F RID: 847 RVA: 0x00012282 File Offset: 0x00010482
		public int printCredit { get; set; }

		// Token: 0x1700006E RID: 110
		// (get) Token: 0x06000350 RID: 848 RVA: 0x0001228B File Offset: 0x0001048B
		// (set) Token: 0x06000351 RID: 849 RVA: 0x00012293 File Offset: 0x00010493
		public int downloadCredit { get; set; }

		// Token: 0x1700006F RID: 111
		// (get) Token: 0x06000352 RID: 850 RVA: 0x0001229C File Offset: 0x0001049C
		public bool IsHaveCredit
		{
			get
			{
				return this.printCredit > 0 || this.downloadCredit > 0;
			}
		}

		// Token: 0x06000353 RID: 851 RVA: 0x000122B2 File Offset: 0x000104B2
		public PaymentSession(int PrintCredit, int DownloadCredit)
		{
			this.printCredit = PrintCredit;
			this.downloadCredit = DownloadCredit;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x000122C8 File Offset: 0x000104C8
		public void Add(PaymentSession other)
		{
			this.printCredit += other.printCredit;
			this.downloadCredit += other.downloadCredit;
		}
	}
}
