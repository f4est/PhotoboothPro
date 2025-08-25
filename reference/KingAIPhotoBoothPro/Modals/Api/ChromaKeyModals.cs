using System;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000089 RID: 137
	public class ChromaKeyModals
	{
		// Token: 0x0200022B RID: 555
		public class ChromaKeyRequest
		{
			// Token: 0x170001C1 RID: 449
			// (get) Token: 0x06000EC7 RID: 3783 RVA: 0x0005BC02 File Offset: 0x00059E02
			// (set) Token: 0x06000EC8 RID: 3784 RVA: 0x0005BC0A File Offset: 0x00059E0A
			public string url { get; set; }

			// Token: 0x170001C2 RID: 450
			// (get) Token: 0x06000EC9 RID: 3785 RVA: 0x0005BC13 File Offset: 0x00059E13
			// (set) Token: 0x06000ECA RID: 3786 RVA: 0x0005BC1B File Offset: 0x00059E1B
			public string color { get; set; }

			// Token: 0x170001C3 RID: 451
			// (get) Token: 0x06000ECB RID: 3787 RVA: 0x0005BC24 File Offset: 0x00059E24
			// (set) Token: 0x06000ECC RID: 3788 RVA: 0x0005BC2C File Offset: 0x00059E2C
			public int threasHold { get; set; }
		}
	}
}
