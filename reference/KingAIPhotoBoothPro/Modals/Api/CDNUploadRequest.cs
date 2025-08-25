using System;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000088 RID: 136
	public class CDNUploadRequest : ForgotUnlockDashboardModals.ApiRequestBase
	{
		// Token: 0x060008E7 RID: 2279 RVA: 0x00033DEF File Offset: 0x00031FEF
		public CDNUploadRequest(byte[] fileByte, string userhash, string eventhash, string prefix, string suffix, string accesToken)
		{
			this.File = fileByte;
			this.UserHash = userhash;
			this.EventHash = eventhash;
			this.Prefix = prefix;
			this.Suffix = suffix;
			base.AccessToken = accesToken;
		}

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x060008E8 RID: 2280 RVA: 0x00033E24 File Offset: 0x00032024
		// (set) Token: 0x060008E9 RID: 2281 RVA: 0x00033E2C File Offset: 0x0003202C
		public byte[] File { get; set; }

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x060008EA RID: 2282 RVA: 0x00033E35 File Offset: 0x00032035
		// (set) Token: 0x060008EB RID: 2283 RVA: 0x00033E3D File Offset: 0x0003203D
		public string UserHash { get; set; }

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x060008EC RID: 2284 RVA: 0x00033E46 File Offset: 0x00032046
		// (set) Token: 0x060008ED RID: 2285 RVA: 0x00033E4E File Offset: 0x0003204E
		public string EventHash { get; set; }

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x060008EE RID: 2286 RVA: 0x00033E57 File Offset: 0x00032057
		// (set) Token: 0x060008EF RID: 2287 RVA: 0x00033E5F File Offset: 0x0003205F
		public string Prefix { get; set; }

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x060008F0 RID: 2288 RVA: 0x00033E68 File Offset: 0x00032068
		// (set) Token: 0x060008F1 RID: 2289 RVA: 0x00033E70 File Offset: 0x00032070
		public string Suffix { get; set; }
	}
}
