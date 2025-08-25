using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Variations;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x0200009E RID: 158
	public class MonoFunctionPhotoBoothController
	{
		// Token: 0x06000977 RID: 2423 RVA: 0x00035D97 File Offset: 0x00033F97
		public MonoFunctionPhotoBoothController()
		{
			VariationMedia.VariationProcessCompleted = (Action<VariationMedia>)Delegate.Combine(VariationMedia.VariationProcessCompleted, new Action<VariationMedia>(this.VariationMediaProcessCompleted));
		}

		// Token: 0x06000978 RID: 2424 RVA: 0x00035DBF File Offset: 0x00033FBF
		private void VariationMediaProcessCompleted(VariationMedia media)
		{
			this.thisMediaClassBase = media;
			this.isProcessing = false;
			VariationMedia.VariationProcessCompleted = (Action<VariationMedia>)Delegate.Remove(VariationMedia.VariationProcessCompleted, new Action<VariationMedia>(this.VariationMediaProcessCompleted));
		}

		// Token: 0x06000979 RID: 2425 RVA: 0x00035DF0 File Offset: 0x00033FF0
		public Task<VariationMedia> AIRender(MonoFunctionPhotoBoothController.AIInfo aiInfo, MediaClassBase mediaClassBase)
		{
			MonoFunctionPhotoBoothController.<AIRender>d__5 <AIRender>d__;
			<AIRender>d__.<>t__builder = AsyncTaskMethodBuilder<VariationMedia>.Create();
			<AIRender>d__.<>4__this = this;
			<AIRender>d__.aiInfo = aiInfo;
			<AIRender>d__.mediaClassBase = mediaClassBase;
			<AIRender>d__.<>1__state = -1;
			<AIRender>d__.<>t__builder.Start<MonoFunctionPhotoBoothController.<AIRender>d__5>(ref <AIRender>d__);
			return <AIRender>d__.<>t__builder.Task;
		}

		// Token: 0x0400094F RID: 2383
		private bool isProcessing;

		// Token: 0x04000950 RID: 2384
		private VariationMedia thisMediaClassBase;

		// Token: 0x0200026D RID: 621
		public class AIInfo
		{
			// Token: 0x17000258 RID: 600
			// (get) Token: 0x0600106F RID: 4207 RVA: 0x0006075E File Offset: 0x0005E95E
			// (set) Token: 0x06001070 RID: 4208 RVA: 0x00060766 File Offset: 0x0005E966
			public AIMode aiMode { get; set; }

			// Token: 0x17000259 RID: 601
			// (get) Token: 0x06001071 RID: 4209 RVA: 0x0006076F File Offset: 0x0005E96F
			// (set) Token: 0x06001072 RID: 4210 RVA: 0x00060777 File Offset: 0x0005E977
			public string promp { get; set; }

			// Token: 0x1700025A RID: 602
			// (get) Token: 0x06001073 RID: 4211 RVA: 0x00060780 File Offset: 0x0005E980
			// (set) Token: 0x06001074 RID: 4212 RVA: 0x00060788 File Offset: 0x0005E988
			public FaceSwapTarget faceSwapTarget { get; set; }

			// Token: 0x1700025B RID: 603
			// (get) Token: 0x06001075 RID: 4213 RVA: 0x00060791 File Offset: 0x0005E991
			// (set) Token: 0x06001076 RID: 4214 RVA: 0x00060799 File Offset: 0x0005E999
			public string aiEffectID { get; set; }

			// Token: 0x04001060 RID: 4192
			public string orginalPhotoPath;

			// Token: 0x04001064 RID: 4196
			public string rawURl;

			// Token: 0x04001065 RID: 4197
			public string mediaURl;
		}
	}
}
