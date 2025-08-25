using System;

namespace KingAIPhotoBoothPro.Modals.Api.AiMotion
{
	// Token: 0x02000098 RID: 152
	public class AiImageMotionInput
	{
		// Token: 0x17000157 RID: 343
		// (get) Token: 0x0600091F RID: 2335 RVA: 0x00034016 File Offset: 0x00032216
		// (set) Token: 0x06000920 RID: 2336 RVA: 0x0003401E File Offset: 0x0003221E
		public string prompt { get; set; }

		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06000921 RID: 2337 RVA: 0x00034027 File Offset: 0x00032227
		// (set) Token: 0x06000922 RID: 2338 RVA: 0x0003402F File Offset: 0x0003222F
		public string negative_prompt { get; set; }

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x06000923 RID: 2339 RVA: 0x00034038 File Offset: 0x00032238
		// (set) Token: 0x06000924 RID: 2340 RVA: 0x00034040 File Offset: 0x00032240
		public string aspect_ratio { get; set; }

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x06000925 RID: 2341 RVA: 0x00034049 File Offset: 0x00032249
		// (set) Token: 0x06000926 RID: 2342 RVA: 0x00034051 File Offset: 0x00032251
		public string start_image { get; set; }

		// Token: 0x1700015B RID: 347
		// (get) Token: 0x06000927 RID: 2343 RVA: 0x0003405A File Offset: 0x0003225A
		// (set) Token: 0x06000928 RID: 2344 RVA: 0x00034062 File Offset: 0x00032262
		public double cfg_scale { get; set; }

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000929 RID: 2345 RVA: 0x0003406B File Offset: 0x0003226B
		// (set) Token: 0x0600092A RID: 2346 RVA: 0x00034073 File Offset: 0x00032273
		public int duration { get; set; }
	}
}
