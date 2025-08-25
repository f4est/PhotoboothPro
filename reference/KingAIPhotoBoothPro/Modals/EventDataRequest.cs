using System;
using System.Collections.Generic;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200006D RID: 109
	public class EventDataRequest
	{
		// Token: 0x170000E4 RID: 228
		// (get) Token: 0x06000806 RID: 2054 RVA: 0x00033301 File Offset: 0x00031501
		// (set) Token: 0x06000807 RID: 2055 RVA: 0x00033309 File Offset: 0x00031509
		public string Token { get; set; }

		// Token: 0x170000E5 RID: 229
		// (get) Token: 0x06000808 RID: 2056 RVA: 0x00033312 File Offset: 0x00031512
		// (set) Token: 0x06000809 RID: 2057 RVA: 0x0003331A File Offset: 0x0003151A
		public List<EventApplication> Events { get; set; }
	}
}
