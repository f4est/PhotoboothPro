using System;

namespace KingAIPhotoBoothPro.Services
{
	// Token: 0x0200001C RID: 28
	public class DobotRoboticArmService
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x06000106 RID: 262 RVA: 0x00007159 File Offset: 0x00005359
		public static DobotRoboticArmService Instance
		{
			get
			{
				return DobotRoboticArmService._instance;
			}
		}

		// Token: 0x06000107 RID: 263 RVA: 0x00007160 File Offset: 0x00005360
		public DobotRoboticArmService(string ipAddress)
		{
		}

		// Token: 0x040000D0 RID: 208
		private readonly string _ipAddress;

		// Token: 0x040000D1 RID: 209
		private static readonly object _lock = new object();

		// Token: 0x040000D2 RID: 210
		private static DobotRoboticArmService _instance;
	}
}
