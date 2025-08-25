using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B3 RID: 179
	public static class HTTPHelper
	{
		// Token: 0x06000A1E RID: 2590 RVA: 0x0003ADB8 File Offset: 0x00038FB8
		public static Task<HTTPHelper.PostResultByte> GetRequestAsyncParamsByte(string baseUrl, string param, List<string> paramNames = null, List<string> paramValues = null, Action<bool, byte[]> callback = null, string bearerToken = "")
		{
			HTTPHelper.<GetRequestAsyncParamsByte>d__0 <GetRequestAsyncParamsByte>d__;
			<GetRequestAsyncParamsByte>d__.<>t__builder = AsyncTaskMethodBuilder<HTTPHelper.PostResultByte>.Create();
			<GetRequestAsyncParamsByte>d__.baseUrl = baseUrl;
			<GetRequestAsyncParamsByte>d__.param = param;
			<GetRequestAsyncParamsByte>d__.paramNames = paramNames;
			<GetRequestAsyncParamsByte>d__.paramValues = paramValues;
			<GetRequestAsyncParamsByte>d__.callback = callback;
			<GetRequestAsyncParamsByte>d__.bearerToken = bearerToken;
			<GetRequestAsyncParamsByte>d__.<>1__state = -1;
			<GetRequestAsyncParamsByte>d__.<>t__builder.Start<HTTPHelper.<GetRequestAsyncParamsByte>d__0>(ref <GetRequestAsyncParamsByte>d__);
			return <GetRequestAsyncParamsByte>d__.<>t__builder.Task;
		}

		// Token: 0x06000A1F RID: 2591 RVA: 0x0003AE25 File Offset: 0x00039025
		public static string ConvertUnicodeCharacters(string input)
		{
			return input.Replace("u0022", "'");
		}

		// Token: 0x06000A20 RID: 2592 RVA: 0x0003AE38 File Offset: 0x00039038
		public static Task<HTTPHelper.PostResult> GetRequestAsync(string baseUrl, string param, Action<bool, string> callback = null, string Authorization = "")
		{
			HTTPHelper.<GetRequestAsync>d__2 <GetRequestAsync>d__;
			<GetRequestAsync>d__.<>t__builder = AsyncTaskMethodBuilder<HTTPHelper.PostResult>.Create();
			<GetRequestAsync>d__.baseUrl = baseUrl;
			<GetRequestAsync>d__.param = param;
			<GetRequestAsync>d__.callback = callback;
			<GetRequestAsync>d__.Authorization = Authorization;
			<GetRequestAsync>d__.<>1__state = -1;
			<GetRequestAsync>d__.<>t__builder.Start<HTTPHelper.<GetRequestAsync>d__2>(ref <GetRequestAsync>d__);
			return <GetRequestAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000A21 RID: 2593 RVA: 0x0003AE94 File Offset: 0x00039094
		public static Task<HTTPHelper.PostResultByte> PostRequestByteAsync<T>(string baseUrl, string param, T ObjectInfo, Action<bool, byte[]> callback = null, string Authorization = "")
		{
			HTTPHelper.<PostRequestByteAsync>d__3<T> <PostRequestByteAsync>d__;
			<PostRequestByteAsync>d__.<>t__builder = AsyncTaskMethodBuilder<HTTPHelper.PostResultByte>.Create();
			<PostRequestByteAsync>d__.baseUrl = baseUrl;
			<PostRequestByteAsync>d__.param = param;
			<PostRequestByteAsync>d__.ObjectInfo = ObjectInfo;
			<PostRequestByteAsync>d__.callback = callback;
			<PostRequestByteAsync>d__.Authorization = Authorization;
			<PostRequestByteAsync>d__.<>1__state = -1;
			<PostRequestByteAsync>d__.<>t__builder.Start<HTTPHelper.<PostRequestByteAsync>d__3<T>>(ref <PostRequestByteAsync>d__);
			return <PostRequestByteAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000A22 RID: 2594 RVA: 0x0003AEF8 File Offset: 0x000390F8
		public static Task<HTTPHelper.PostResult> PostRequestAsync<T>(string param, T ObjectInfo, Action<bool, string> callback = null, string Authorization = "")
		{
			HTTPHelper.<PostRequestAsync>d__6<T> <PostRequestAsync>d__;
			<PostRequestAsync>d__.<>t__builder = AsyncTaskMethodBuilder<HTTPHelper.PostResult>.Create();
			<PostRequestAsync>d__.param = param;
			<PostRequestAsync>d__.ObjectInfo = ObjectInfo;
			<PostRequestAsync>d__.callback = callback;
			<PostRequestAsync>d__.Authorization = Authorization;
			<PostRequestAsync>d__.<>1__state = -1;
			<PostRequestAsync>d__.<>t__builder.Start<HTTPHelper.<PostRequestAsync>d__6<T>>(ref <PostRequestAsync>d__);
			return <PostRequestAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000A23 RID: 2595 RVA: 0x0003AF54 File Offset: 0x00039154
		public static Task<HTTPHelper.PostResult> PostRequestMultipartAsync<T>(string param, string filename, T objectInfo, Action<bool, string> callback = null, string authorization = "")
		{
			HTTPHelper.<PostRequestMultipartAsync>d__7<T> <PostRequestMultipartAsync>d__;
			<PostRequestMultipartAsync>d__.<>t__builder = AsyncTaskMethodBuilder<HTTPHelper.PostResult>.Create();
			<PostRequestMultipartAsync>d__.param = param;
			<PostRequestMultipartAsync>d__.filename = filename;
			<PostRequestMultipartAsync>d__.objectInfo = objectInfo;
			<PostRequestMultipartAsync>d__.callback = callback;
			<PostRequestMultipartAsync>d__.authorization = authorization;
			<PostRequestMultipartAsync>d__.<>1__state = -1;
			<PostRequestMultipartAsync>d__.<>t__builder.Start<HTTPHelper.<PostRequestMultipartAsync>d__7<T>>(ref <PostRequestMultipartAsync>d__);
			return <PostRequestMultipartAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000A24 RID: 2596 RVA: 0x0003AFB8 File Offset: 0x000391B8
		public static Task<bool> IsUrl404Async(string url)
		{
			HTTPHelper.<IsUrl404Async>d__8 <IsUrl404Async>d__;
			<IsUrl404Async>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<IsUrl404Async>d__.url = url;
			<IsUrl404Async>d__.<>1__state = -1;
			<IsUrl404Async>d__.<>t__builder.Start<HTTPHelper.<IsUrl404Async>d__8>(ref <IsUrl404Async>d__);
			return <IsUrl404Async>d__.<>t__builder.Task;
		}

		// Token: 0x040009E5 RID: 2533
		private static bool isNoInternetWarningShown = false;

		// Token: 0x040009E6 RID: 2534
		private static DateTime lastResquestSendTime = DateTime.MinValue;

		// Token: 0x0200028B RID: 651
		public class PostResult
		{
			// Token: 0x060010E4 RID: 4324 RVA: 0x00061F7B File Offset: 0x0006017B
			public PostResult(bool serverCommunicationSuccess, string message)
			{
				this.ServerCommunicationSuccess = serverCommunicationSuccess;
				this.Message = message;
			}

			// Token: 0x040010F6 RID: 4342
			public bool ServerCommunicationSuccess;

			// Token: 0x040010F7 RID: 4343
			public string Message;
		}

		// Token: 0x0200028C RID: 652
		public class PostResultByte
		{
			// Token: 0x060010E5 RID: 4325 RVA: 0x00061F91 File Offset: 0x00060191
			public PostResultByte(bool success, byte[] messagebyteData)
			{
				this.Success = success;
				this.MessagebyteData = messagebyteData;
			}

			// Token: 0x040010F8 RID: 4344
			public bool Success;

			// Token: 0x040010F9 RID: 4345
			public byte[] MessagebyteData;
		}
	}
}
