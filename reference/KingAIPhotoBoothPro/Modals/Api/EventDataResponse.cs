using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using KingAIPhotoBoothPro.Class.ActivationKing;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x0200008B RID: 139
	public class EventDataResponse : GenericResponse
	{
		// Token: 0x17000149 RID: 329
		// (get) Token: 0x060008F4 RID: 2292 RVA: 0x00033E89 File Offset: 0x00032089
		// (set) Token: 0x060008F5 RID: 2293 RVA: 0x00033E91 File Offset: 0x00032091
		[JsonPropertyName("Events")]
		public List<EventApplication> Events { get; set; }
	}
}
