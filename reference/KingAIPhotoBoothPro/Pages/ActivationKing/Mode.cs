using System;
using System.ComponentModel.DataAnnotations;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x0200004B RID: 75
	public enum Mode
	{
		// Token: 0x040005F4 RID: 1524
		[Display(Name = "Multifunctional Photo Booth")]
		MultifunctionalPhotoBooth,
		// Token: 0x040005F5 RID: 1525
		[Display(Name = "Glambot Robotic Arm")]
		Glambot,
		// Token: 0x040005F6 RID: 1526
		[Display(Name = "Singlefunctional Photo Booth")]
		MonofunctionalPhotoBooth
	}
}
