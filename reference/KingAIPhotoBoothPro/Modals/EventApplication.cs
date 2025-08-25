using System;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200006C RID: 108
	public class EventApplication : Base
	{
		// Token: 0x170000DF RID: 223
		// (get) Token: 0x060007FA RID: 2042 RVA: 0x00033297 File Offset: 0x00031497
		// (set) Token: 0x060007FB RID: 2043 RVA: 0x0003329F File Offset: 0x0003149F
		public string LicenceId { get; set; }

		// Token: 0x170000E0 RID: 224
		// (get) Token: 0x060007FC RID: 2044 RVA: 0x000332A8 File Offset: 0x000314A8
		// (set) Token: 0x060007FD RID: 2045 RVA: 0x000332B0 File Offset: 0x000314B0
		public string EventName { get; set; }

		// Token: 0x170000E1 RID: 225
		// (get) Token: 0x060007FE RID: 2046 RVA: 0x000332B9 File Offset: 0x000314B9
		// (set) Token: 0x060007FF RID: 2047 RVA: 0x000332C1 File Offset: 0x000314C1
		public string EventMotto { get; set; }

		// Token: 0x170000E2 RID: 226
		// (get) Token: 0x06000800 RID: 2048 RVA: 0x000332CA File Offset: 0x000314CA
		// (set) Token: 0x06000801 RID: 2049 RVA: 0x000332D2 File Offset: 0x000314D2
		public string EventHash { get; set; }

		// Token: 0x170000E3 RID: 227
		// (get) Token: 0x06000802 RID: 2050 RVA: 0x000332DB File Offset: 0x000314DB
		// (set) Token: 0x06000803 RID: 2051 RVA: 0x000332E3 File Offset: 0x000314E3
		public bool IsCloudSync { get; set; }

		// Token: 0x06000804 RID: 2052 RVA: 0x000332EC File Offset: 0x000314EC
		public string GenerateEventHash()
		{
			return HashGenerator.GenerateHash(base.Id);
		}
	}
}
