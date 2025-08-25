using System;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D8 RID: 216
	[Serializable]
	public class SMSInfo
	{
		// Token: 0x17000192 RID: 402
		// (get) Token: 0x06000B4A RID: 2890 RVA: 0x00042CDB File Offset: 0x00040EDB
		// (set) Token: 0x06000B4B RID: 2891 RVA: 0x00042CE3 File Offset: 0x00040EE3
		public string HostNumber { get; set; }

		// Token: 0x17000193 RID: 403
		// (get) Token: 0x06000B4C RID: 2892 RVA: 0x00042CEC File Offset: 0x00040EEC
		// (set) Token: 0x06000B4D RID: 2893 RVA: 0x00042CF4 File Offset: 0x00040EF4
		public string body { get; set; }

		// Token: 0x17000194 RID: 404
		// (get) Token: 0x06000B4E RID: 2894 RVA: 0x00042CFD File Offset: 0x00040EFD
		// (set) Token: 0x06000B4F RID: 2895 RVA: 0x00042D05 File Offset: 0x00040F05
		public string SendNumber { get; set; }

		// Token: 0x17000195 RID: 405
		// (get) Token: 0x06000B50 RID: 2896 RVA: 0x00042D0E File Offset: 0x00040F0E
		// (set) Token: 0x06000B51 RID: 2897 RVA: 0x00042D16 File Offset: 0x00040F16
		public bool isMailSend { get; set; }
	}
}
