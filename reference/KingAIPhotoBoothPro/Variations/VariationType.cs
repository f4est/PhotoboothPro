using System;
using System.ComponentModel.DataAnnotations;

namespace KingAIPhotoBoothPro.Variations
{
	// Token: 0x0200001F RID: 31
	public enum VariationType
	{
		// Token: 0x04000110 RID: 272
		None,
		// Token: 0x04000111 RID: 273
		[Display(Name = "Ai Prompt")]
		AIPrompt,
		// Token: 0x04000112 RID: 274
		[Display(Name = "Face Swap")]
		AIFaceSwap,
		// Token: 0x04000113 RID: 275
		[Display(Name = "Word Portrait")]
		WordPortrait,
		// Token: 0x04000114 RID: 276
		[Display(Name = "Ai Motion")]
		AiMotion,
		// Token: 0x04000115 RID: 277
		[Display(Name = "Ai Prompt")]
		MidJourneyPrompt,
		// Token: 0x04000116 RID: 278
		[Display(Name = "Ai Effect")]
		AiEffect,
		// Token: 0x04000117 RID: 279
		[Display(Name = "Ai Beautifier")]
		AiBeautifier
	}
}
