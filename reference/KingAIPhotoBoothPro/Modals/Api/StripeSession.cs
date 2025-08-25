using System;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000084 RID: 132
	public class StripeSession : Base
	{
		// Token: 0x1700013D RID: 317
		// (get) Token: 0x060008D5 RID: 2261 RVA: 0x00033D27 File Offset: 0x00031F27
		// (set) Token: 0x060008D6 RID: 2262 RVA: 0x00033D2F File Offset: 0x00031F2F
		public string sessionID { get; set; }

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x060008D7 RID: 2263 RVA: 0x00033D38 File Offset: 0x00031F38
		// (set) Token: 0x060008D8 RID: 2264 RVA: 0x00033D40 File Offset: 0x00031F40
		public string paymentIntentID { get; set; }

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x060008D9 RID: 2265 RVA: 0x00033D49 File Offset: 0x00031F49
		// (set) Token: 0x060008DA RID: 2266 RVA: 0x00033D51 File Offset: 0x00031F51
		public string amountReceived { get; set; }

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x060008DB RID: 2267 RVA: 0x00033D5A File Offset: 0x00031F5A
		// (set) Token: 0x060008DC RID: 2268 RVA: 0x00033D62 File Offset: 0x00031F62
		public string receiptUrl { get; set; }

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x060008DD RID: 2269 RVA: 0x00033D6B File Offset: 0x00031F6B
		// (set) Token: 0x060008DE RID: 2270 RVA: 0x00033D73 File Offset: 0x00031F73
		public string currency { get; set; }

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x060008DF RID: 2271 RVA: 0x00033D7C File Offset: 0x00031F7C
		// (set) Token: 0x060008E0 RID: 2272 RVA: 0x00033D84 File Offset: 0x00031F84
		public string email { get; set; }

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x060008E1 RID: 2273 RVA: 0x00033D8D File Offset: 0x00031F8D
		// (set) Token: 0x060008E2 RID: 2274 RVA: 0x00033D95 File Offset: 0x00031F95
		public bool success { get; set; }

		// Token: 0x060008E3 RID: 2275 RVA: 0x00033D9E File Offset: 0x00031F9E
		public StripeSession(bool success, string SessionID, string Currency, string AmountReceived, string ReceiptUrl, string Email, string Error = "")
		{
			if (!success)
			{
				this.error = Error;
			}
			this.sessionID = SessionID;
			this.currency = Currency;
			this.amountReceived = AmountReceived;
			this.receiptUrl = ReceiptUrl;
			this.email = Email;
		}

		// Token: 0x040008F5 RID: 2293
		public string error;
	}
}
