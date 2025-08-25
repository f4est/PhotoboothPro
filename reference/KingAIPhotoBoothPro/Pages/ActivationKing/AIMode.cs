using System;
using System.ComponentModel.DataAnnotations;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x0200004C RID: 76
	public enum AIMode
	{
		// Token: 0x040005F8 RID: 1528
		[Display(Name = "AI Prompt")]
		AIPromptTheme,
		// Token: 0x040005F9 RID: 1529
		[Display(Name = "AI Face Swap")]
		AIFaceSwap,
		// Token: 0x040005FA RID: 1530
		[Display(Name = "AI Effect")]
		AIEffect,
		// Token: 0x040005FB RID: 1531
		[Display(Name = "Word Portrait")]
		WordPortrait,
		// Token: 0x040005FC RID: 1532
		[Display(Name = "AI Beautifier")]
		AIBeautifier
	}
}
