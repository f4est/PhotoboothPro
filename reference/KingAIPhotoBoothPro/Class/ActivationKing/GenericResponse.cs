using System;
using System.Net;
using System.Text.Json.Serialization;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C1 RID: 193
	[Serializable]
	public class GenericResponse
	{
		// Token: 0x06000A7D RID: 2685 RVA: 0x0003C8DD File Offset: 0x0003AADD
		public GenericResponse()
		{
		}

		// Token: 0x06000A7E RID: 2686 RVA: 0x0003C8E5 File Offset: 0x0003AAE5
		public GenericResponse(bool success)
		{
			this.Success = success;
		}

		// Token: 0x06000A7F RID: 2687 RVA: 0x0003C8F4 File Offset: 0x0003AAF4
		public GenericResponse(bool success, string message)
		{
			this.Success = success;
			this.Message = message;
		}

		// Token: 0x06000A80 RID: 2688 RVA: 0x0003C90C File Offset: 0x0003AB0C
		public GenericResponse(bool success, HttpStatusCode statusCode, DateTime dtStart, string message = "")
		{
			this.Success = success;
			this.ProcessingTime = (DateTime.UtcNow - dtStart).TotalSeconds.ToString("0.000") + " seconds.";
			this.ProcessingTimeSpan = DateTime.UtcNow - dtStart;
			this.ProcessStartedDateTime = dtStart;
			this.Message = message;
			this.StatusCode = statusCode;
			if (success)
			{
				this.StatusCode = HttpStatusCode.OK;
			}
		}

		// Token: 0x04000A2C RID: 2604
		[JsonPropertyName("Success")]
		public bool Success;

		// Token: 0x04000A2D RID: 2605
		[JsonPropertyName("Message")]
		public string Message;

		// Token: 0x04000A2E RID: 2606
		[JsonPropertyName("Statuscode")]
		public HttpStatusCode StatusCode;

		// Token: 0x04000A2F RID: 2607
		public string ProcessingTime;

		// Token: 0x04000A30 RID: 2608
		public TimeSpan ProcessingTimeSpan;

		// Token: 0x04000A31 RID: 2609
		public DateTime ProcessStartedDateTime;
	}
}
