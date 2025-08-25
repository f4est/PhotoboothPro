using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KingAIPhotoBoothPro.Modals.Api;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000CF RID: 207
	public static class FluxAIHelper
	{
		// Token: 0x06000B03 RID: 2819 RVA: 0x0004047C File Offset: 0x0003E67C
		public static Task<FluxAIModals.FluxAIResponse> SendImagine(FluxAIModals.FluxAIRequest requestObject)
		{
			FluxAIHelper.<SendImagine>d__0 <SendImagine>d__;
			<SendImagine>d__.<>t__builder = AsyncTaskMethodBuilder<FluxAIModals.FluxAIResponse>.Create();
			<SendImagine>d__.requestObject = requestObject;
			<SendImagine>d__.<>1__state = -1;
			<SendImagine>d__.<>t__builder.Start<FluxAIHelper.<SendImagine>d__0>(ref <SendImagine>d__);
			return <SendImagine>d__.<>t__builder.Task;
		}
	}
}
