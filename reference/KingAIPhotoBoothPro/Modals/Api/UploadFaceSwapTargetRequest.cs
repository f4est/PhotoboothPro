using System;

namespace KingAIPhotoBoothPro.Modals.Api
{
	// Token: 0x02000091 RID: 145
	public class UploadFaceSwapTargetRequest
	{
		// Token: 0x1700014A RID: 330
		// (get) Token: 0x060008FC RID: 2300 RVA: 0x00033ECA File Offset: 0x000320CA
		// (set) Token: 0x060008FD RID: 2301 RVA: 0x00033ED2 File Offset: 0x000320D2
		public string AccessToken { get; set; }

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x060008FE RID: 2302 RVA: 0x00033EDB File Offset: 0x000320DB
		// (set) Token: 0x060008FF RID: 2303 RVA: 0x00033EE3 File Offset: 0x000320E3
		public string EventId { get; set; }

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06000900 RID: 2304 RVA: 0x00033EEC File Offset: 0x000320EC
		// (set) Token: 0x06000901 RID: 2305 RVA: 0x00033EF4 File Offset: 0x000320F4
		public string Filename { get; set; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06000902 RID: 2306 RVA: 0x00033EFD File Offset: 0x000320FD
		// (set) Token: 0x06000903 RID: 2307 RVA: 0x00033F05 File Offset: 0x00032105
		public byte[] ImageData { get; set; }
	}
}
