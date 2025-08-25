using System;
using System.Net;

namespace KingAIPhotoBoothPro.Modals.Api.AiMotion
{
	// Token: 0x02000095 RID: 149
	public class AiImageMotionRunResponse : ForgotUnlockDashboardModals.GenericResponse
	{
		// Token: 0x06000910 RID: 2320 RVA: 0x00033F72 File Offset: 0x00032172
		public AiImageMotionRunResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
		{
		}

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06000911 RID: 2321 RVA: 0x00033F7F File Offset: 0x0003217F
		// (set) Token: 0x06000912 RID: 2322 RVA: 0x00033F87 File Offset: 0x00032187
		public string Id { get; set; }
	}
}
