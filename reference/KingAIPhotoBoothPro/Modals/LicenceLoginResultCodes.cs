using System;
using System.ComponentModel;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000080 RID: 128
	public enum LicenceLoginResultCodes
	{
		// Token: 0x040008D3 RID: 2259
		[Description("Licence Is Valid")]
		LicenceIsValid,
		// Token: 0x040008D4 RID: 2260
		[Description("Licence Is Not Valid")]
		LicenceIsNotValid,
		// Token: 0x040008D5 RID: 2261
		[Description("Device Limit Exceeded")]
		DeviceLimitExceeded,
		// Token: 0x040008D6 RID: 2262
		[Description("Licence Time Exceeded")]
		LicenceTimeExceeded,
		// Token: 0x040008D7 RID: 2263
		[Description("Token Is Not Valid")]
		TokenIsNotValid,
		// Token: 0x040008D8 RID: 2264
		[Description("Token Time Exceeded")]
		TokenTimeExceeded,
		// Token: 0x040008D9 RID: 2265
		[Description("User does Not Exist")]
		UserDoesNotExist,
		// Token: 0x040008DA RID: 2266
		[Description("App does Not Exist")]
		AppDoesNotExist,
		// Token: 0x040008DB RID: 2267
		[Description("User Create Error")]
		UserCreateError,
		// Token: 0x040008DC RID: 2268
		[Description("Non-Trial Already Exists")]
		NonTrialAccountAlreadyExists,
		// Token: 0x040008DD RID: 2269
		[Description("Device Used Before")]
		DeviceAlreadyUsedWithLicence,
		// Token: 0x040008DE RID: 2270
		[Description("Trial Time Exceeded")]
		TrialTimeExceeded,
		// Token: 0x040008DF RID: 2271
		[Description("Licence Code Sent as Email")]
		LicenceCodeSent,
		// Token: 0x040008E0 RID: 2272
		[Description("Error Occurred While Sending Email")]
		EmailSendError,
		// Token: 0x040008E1 RID: 2273
		[Description("Unknown Error Occurred")]
		UnknownError,
		// Token: 0x040008E2 RID: 2274
		[Description("Trial User Not Created")]
		TrialLicenceError,
		// Token: 0x040008E3 RID: 2275
		[Description("Email Address Is Not Valid")]
		EmailIsNotValid,
		// Token: 0x040008E4 RID: 2276
		[Description("Email Already Sent. You Tried Too Many Times")]
		EmailSendLimitExceeded
	}
}
