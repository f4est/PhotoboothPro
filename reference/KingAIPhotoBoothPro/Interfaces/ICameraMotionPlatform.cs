using System;

namespace KingAIPhotoBoothPro.Interfaces
{
	// Token: 0x02000020 RID: 32
	public interface ICameraMotionPlatform
	{
		// Token: 0x06000188 RID: 392
		void StartMotion();

		// Token: 0x06000189 RID: 393
		void StopMotion();

		// Token: 0x0600018A RID: 394
		void EmergencyStopMotion();
	}
}
