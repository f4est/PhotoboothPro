using System;
using System.Net;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Modals.Api.AiMotion
{
	// Token: 0x02000097 RID: 151
	public class AiImageMotionOutput : ForgotUnlockDashboardModals.GenericResponse
	{
		// Token: 0x17000154 RID: 340
		// (get) Token: 0x06000916 RID: 2326 RVA: 0x00033FA9 File Offset: 0x000321A9
		// (set) Token: 0x06000917 RID: 2327 RVA: 0x00033FB1 File Offset: 0x000321B1
		public string Url { get; set; }

		// Token: 0x17000155 RID: 341
		// (get) Token: 0x06000918 RID: 2328 RVA: 0x00033FBA File Offset: 0x000321BA
		// (set) Token: 0x06000919 RID: 2329 RVA: 0x00033FC2 File Offset: 0x000321C2
		public AiImageMotionInput Input { get; set; }

		// Token: 0x17000156 RID: 342
		// (get) Token: 0x0600091A RID: 2330 RVA: 0x00033FCB File Offset: 0x000321CB
		// (set) Token: 0x0600091B RID: 2331 RVA: 0x00033FD3 File Offset: 0x000321D3
		[JsonProperty("status")]
		public string Status { get; set; }

		// Token: 0x0600091C RID: 2332 RVA: 0x00033FDC File Offset: 0x000321DC
		public AiImageMotionOutput() : base(false, HttpStatusCode.Created, DateTime.UtcNow, "")
		{
		}

		// Token: 0x0600091D RID: 2333 RVA: 0x00033FF4 File Offset: 0x000321F4
		public AiImageMotionOutput(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
		{
		}

		// Token: 0x0600091E RID: 2334 RVA: 0x00034001 File Offset: 0x00032201
		public AiImageMotionOutput(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, AiImageMotionInput input = null, string message = "") : base(success, httpStatusCode, dtStart, message)
		{
			this.Input = input;
		}
	}
}
