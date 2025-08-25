using System;
using KingAIPhotoBoothPro.Helper;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000081 RID: 129
	[Serializable]
	public class LicenceLoginResponse
	{
		// Token: 0x060008D0 RID: 2256 RVA: 0x00033CE3 File Offset: 0x00031EE3
		public string GetExpirationTimeString()
		{
			if (this.ExpirationTime.Year < 1900 || this.ExpirationTime.Year > 3000)
			{
				return "Unlimited";
			}
			return this.ExpirationTime.ToString("dd MMMM yyyy");
		}

		// Token: 0x040008E5 RID: 2277
		public bool LicenceValid;

		// Token: 0x040008E6 RID: 2278
		public string AccountUsername;

		// Token: 0x040008E7 RID: 2279
		[JsonConverter(typeof(SafeDateTimeConverter))]
		public DateTime ExpirationTime;

		// Token: 0x040008E8 RID: 2280
		public string Message;

		// Token: 0x040008E9 RID: 2281
		public string AccessToken;

		// Token: 0x040008EA RID: 2282
		public int ResultCode;

		// Token: 0x040008EB RID: 2283
		public string UserHash;

		// Token: 0x040008EC RID: 2284
		public bool IsTrial;

		// Token: 0x040008ED RID: 2285
		public bool IsTesterUser;
	}
}
