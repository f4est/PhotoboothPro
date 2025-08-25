using System;
using System.Net;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x0200008D RID: 141
	public class ForgotUnlockDashboardModals
	{
		// Token: 0x0200023B RID: 571
		public class ForgotUnlockDashboardRequest : ForgotUnlockDashboardModals.ApiRequestBase
		{
			// Token: 0x17000228 RID: 552
			// (get) Token: 0x06000FA5 RID: 4005 RVA: 0x0005C359 File Offset: 0x0005A559
			// (set) Token: 0x06000FA6 RID: 4006 RVA: 0x0005C361 File Offset: 0x0005A561
			public string PinCode { get; set; }
		}

		// Token: 0x0200023C RID: 572
		public class ApiRequestBase
		{
			// Token: 0x17000229 RID: 553
			// (get) Token: 0x06000FA8 RID: 4008 RVA: 0x0005C372 File Offset: 0x0005A572
			// (set) Token: 0x06000FA9 RID: 4009 RVA: 0x0005C37A File Offset: 0x0005A57A
			public string AccessToken { get; set; }

			// Token: 0x06000FAA RID: 4010 RVA: 0x0005C383 File Offset: 0x0005A583
			public string ToHtmlString()
			{
				return "<ul><li>Access Token: " + this.AccessToken + "</li></ul>";
			}
		}

		// Token: 0x0200023D RID: 573
		public class ForgotUnlockDashboardResponse : ForgotUnlockDashboardModals.GenericResponse
		{
			// Token: 0x06000FAC RID: 4012 RVA: 0x0005C3A2 File Offset: 0x0005A5A2
			public ForgotUnlockDashboardResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message = "") : base(success, httpStatusCode, dtStart, message)
			{
			}
		}

		// Token: 0x0200023E RID: 574
		public class GenericResponse
		{
			// Token: 0x06000FAD RID: 4013 RVA: 0x0005C3AF File Offset: 0x0005A5AF
			public GenericResponse()
			{
			}

			// Token: 0x06000FAE RID: 4014 RVA: 0x0005C3B7 File Offset: 0x0005A5B7
			public GenericResponse(bool success, HttpStatusCode httpStatusCode, DateTime dtStart, string message)
			{
				this.Success = success;
				this.StatusCode = httpStatusCode;
				this.ProcessStartedDateTime = dtStart;
				this.Message = message;
			}

			// Token: 0x1700022A RID: 554
			// (get) Token: 0x06000FAF RID: 4015 RVA: 0x0005C3DC File Offset: 0x0005A5DC
			// (set) Token: 0x06000FB0 RID: 4016 RVA: 0x0005C3E4 File Offset: 0x0005A5E4
			[JsonProperty("success")]
			public bool Success { get; set; }

			// Token: 0x1700022B RID: 555
			// (get) Token: 0x06000FB1 RID: 4017 RVA: 0x0005C3ED File Offset: 0x0005A5ED
			// (set) Token: 0x06000FB2 RID: 4018 RVA: 0x0005C3F5 File Offset: 0x0005A5F5
			[JsonProperty("message")]
			public string Message { get; set; }

			// Token: 0x1700022C RID: 556
			// (get) Token: 0x06000FB3 RID: 4019 RVA: 0x0005C3FE File Offset: 0x0005A5FE
			// (set) Token: 0x06000FB4 RID: 4020 RVA: 0x0005C406 File Offset: 0x0005A606
			[JsonProperty("statusCode")]
			public HttpStatusCode StatusCode { get; set; }

			// Token: 0x1700022D RID: 557
			// (get) Token: 0x06000FB5 RID: 4021 RVA: 0x0005C40F File Offset: 0x0005A60F
			// (set) Token: 0x06000FB6 RID: 4022 RVA: 0x0005C417 File Offset: 0x0005A617
			[JsonProperty("processingTime")]
			public string ProcessingTime { get; private set; }

			// Token: 0x1700022E RID: 558
			// (get) Token: 0x06000FB7 RID: 4023 RVA: 0x0005C420 File Offset: 0x0005A620
			// (set) Token: 0x06000FB8 RID: 4024 RVA: 0x0005C428 File Offset: 0x0005A628
			[JsonProperty("processingTimeSpan")]
			public TimeSpan ProcessingTimeSpan { get; private set; }

			// Token: 0x1700022F RID: 559
			// (get) Token: 0x06000FB9 RID: 4025 RVA: 0x0005C431 File Offset: 0x0005A631
			// (set) Token: 0x06000FBA RID: 4026 RVA: 0x0005C439 File Offset: 0x0005A639
			[JsonProperty("processStartedDateTime")]
			public DateTime ProcessStartedDateTime { get; private set; }
		}
	}
}
