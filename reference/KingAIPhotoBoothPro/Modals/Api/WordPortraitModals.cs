using System;
using System.Collections.Generic;
using KingAIPhotoBoothPro.Class.ActivationKing;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000093 RID: 147
	public class WordPortraitModals
	{
		// Token: 0x02000248 RID: 584
		public class WordPortraitRequest
		{
			// Token: 0x1700024D RID: 589
			// (get) Token: 0x06000FFF RID: 4095 RVA: 0x0005C6A6 File Offset: 0x0005A8A6
			// (set) Token: 0x06001000 RID: 4096 RVA: 0x0005C6AE File Offset: 0x0005A8AE
			public string AccessToken { get; set; }

			// Token: 0x1700024E RID: 590
			// (get) Token: 0x06001001 RID: 4097 RVA: 0x0005C6B7 File Offset: 0x0005A8B7
			// (set) Token: 0x06001002 RID: 4098 RVA: 0x0005C6BF File Offset: 0x0005A8BF
			public string Url { get; set; }

			// Token: 0x1700024F RID: 591
			// (get) Token: 0x06001003 RID: 4099 RVA: 0x0005C6C8 File Offset: 0x0005A8C8
			// (set) Token: 0x06001004 RID: 4100 RVA: 0x0005C6D0 File Offset: 0x0005A8D0
			public List<string> Words { get; set; }

			// Token: 0x17000250 RID: 592
			// (get) Token: 0x06001005 RID: 4101 RVA: 0x0005C6D9 File Offset: 0x0005A8D9
			// (set) Token: 0x06001006 RID: 4102 RVA: 0x0005C6E1 File Offset: 0x0005A8E1
			public string FontName { get; set; }

			// Token: 0x17000251 RID: 593
			// (get) Token: 0x06001007 RID: 4103 RVA: 0x0005C6EA File Offset: 0x0005A8EA
			// (set) Token: 0x06001008 RID: 4104 RVA: 0x0005C6F2 File Offset: 0x0005A8F2
			public string FontColor { get; set; }

			// Token: 0x17000252 RID: 594
			// (get) Token: 0x06001009 RID: 4105 RVA: 0x0005C6FB File Offset: 0x0005A8FB
			// (set) Token: 0x0600100A RID: 4106 RVA: 0x0005C703 File Offset: 0x0005A903
			public string BackgroundColor { get; set; }
		}

		// Token: 0x02000249 RID: 585
		public class WordPortraitResponse : GenericResponse
		{
			// Token: 0x17000253 RID: 595
			// (get) Token: 0x0600100C RID: 4108 RVA: 0x0005C714 File Offset: 0x0005A914
			// (set) Token: 0x0600100D RID: 4109 RVA: 0x0005C71C File Offset: 0x0005A91C
			public string Url { get; set; }

			// Token: 0x0600100E RID: 4110 RVA: 0x0005C725 File Offset: 0x0005A925
			public WordPortraitResponse()
			{
			}

			// Token: 0x0600100F RID: 4111 RVA: 0x0005C72D File Offset: 0x0005A92D
			public WordPortraitResponse(bool success, string message = "") : base(success, message)
			{
			}
		}
	}
}
