using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals.Api;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Variations;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D2 RID: 210
	public static class BunnyCDNHelper
	{
		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06000B08 RID: 2824 RVA: 0x000405C0 File Offset: 0x0003E7C0
		// (remove) Token: 0x06000B09 RID: 2825 RVA: 0x000405F4 File Offset: 0x0003E7F4
		public static event Action<MediaClassBase> MediaLinkGenerated;

		// Token: 0x06000B0A RID: 2826 RVA: 0x00040628 File Offset: 0x0003E828
		public static string GetBunnyLink(string filepath, string userHash, string eventHash, string subFolderName = "")
		{
			return string.Format("https://cdn.activationshare.com/{0}/{1}/{3}{2}", new object[]
			{
				userHash,
				eventHash,
				Path.GetFileName(filepath),
				string.IsNullOrEmpty(subFolderName) ? "" : (subFolderName + "/")
			});
		}

		// Token: 0x06000B0B RID: 2827 RVA: 0x00040678 File Offset: 0x0003E878
		private static Task UploadFileToCDN(string filePath, string userHash, string eventHash, string suffix)
		{
			BunnyCDNHelper.<UploadFileToCDN>d__10 <UploadFileToCDN>d__;
			<UploadFileToCDN>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UploadFileToCDN>d__.filePath = filePath;
			<UploadFileToCDN>d__.userHash = userHash;
			<UploadFileToCDN>d__.eventHash = eventHash;
			<UploadFileToCDN>d__.suffix = suffix;
			<UploadFileToCDN>d__.<>1__state = -1;
			<UploadFileToCDN>d__.<>t__builder.Start<BunnyCDNHelper.<UploadFileToCDN>d__10>(ref <UploadFileToCDN>d__);
			return <UploadFileToCDN>d__.<>t__builder.Task;
		}

		// Token: 0x06000B0C RID: 2828 RVA: 0x000406D4 File Offset: 0x0003E8D4
		public static Task<BunnyCDNHelper.CDNTaskResult> CDNUploadFileAsync(string filepath, string userHash, string eventHash, Action<BunnyCDNHelper.CDNTaskResult> callback = null, string subFolderName = "")
		{
			BunnyCDNHelper.<CDNUploadFileAsync>d__11 <CDNUploadFileAsync>d__;
			<CDNUploadFileAsync>d__.<>t__builder = AsyncTaskMethodBuilder<BunnyCDNHelper.CDNTaskResult>.Create();
			<CDNUploadFileAsync>d__.filepath = filepath;
			<CDNUploadFileAsync>d__.userHash = userHash;
			<CDNUploadFileAsync>d__.eventHash = eventHash;
			<CDNUploadFileAsync>d__.callback = callback;
			<CDNUploadFileAsync>d__.subFolderName = subFolderName;
			<CDNUploadFileAsync>d__.<>1__state = -1;
			<CDNUploadFileAsync>d__.<>t__builder.Start<BunnyCDNHelper.<CDNUploadFileAsync>d__11>(ref <CDNUploadFileAsync>d__);
			return <CDNUploadFileAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000B0D RID: 2829 RVA: 0x00040738 File Offset: 0x0003E938
		public static string NormalizePath(string path, bool? isDirectory = null)
		{
			path = path.Trim().Replace("\\", "/").TrimStart(new char[]
			{
				'/'
			});
			if (!path.StartsWith(ConfigReader.GetParameterValue("CDNStorageZoneName") + "/"))
			{
				throw new Exception("Path validation failed.");
			}
			if (isDirectory != null)
			{
				if (isDirectory.Value)
				{
					path = path.TrimEnd(new char[]
					{
						'/'
					}) + "/";
				}
				else if (path.EndsWith("/"))
				{
					throw new Exception("The requested path is invalid, cannot be directory.");
				}
			}
			while (path.Contains("//"))
			{
				path = path.Replace("//", "/");
			}
			return path;
		}

		// Token: 0x06000B0E RID: 2830 RVA: 0x000407FC File Offset: 0x0003E9FC
		private static string GetBaseAddress(string mainReplicationRegion)
		{
			if (mainReplicationRegion == "" || mainReplicationRegion.ToLower() == "de")
			{
				return "https://storage.bunnycdn.com/";
			}
			return "https://" + mainReplicationRegion + ".storage.bunnycdn.com/";
		}

		// Token: 0x06000B0F RID: 2831 RVA: 0x00040834 File Offset: 0x0003EA34
		public static Task<bool> CheckFor404Async(string url)
		{
			BunnyCDNHelper.<CheckFor404Async>d__14 <CheckFor404Async>d__;
			<CheckFor404Async>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<CheckFor404Async>d__.url = url;
			<CheckFor404Async>d__.<>1__state = -1;
			<CheckFor404Async>d__.<>t__builder.Start<BunnyCDNHelper.<CheckFor404Async>d__14>(ref <CheckFor404Async>d__);
			return <CheckFor404Async>d__.<>t__builder.Task;
		}

		// Token: 0x06000B10 RID: 2832 RVA: 0x00040878 File Offset: 0x0003EA78
		private static void UploadThreadFunc()
		{
			BunnyCDNHelper.<UploadThreadFunc>d__15 <UploadThreadFunc>d__;
			<UploadThreadFunc>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UploadThreadFunc>d__.<>1__state = -1;
			<UploadThreadFunc>d__.<>t__builder.Start<BunnyCDNHelper.<UploadThreadFunc>d__15>(ref <UploadThreadFunc>d__);
		}

		// Token: 0x06000B11 RID: 2833 RVA: 0x000408A8 File Offset: 0x0003EAA8
		private static void MediaCloudSyncThreadFunc()
		{
			while (!BunnyCDNHelper.isForceQuit)
			{
				List<MediaClassBase> noDbSyncMediaList = (from x in DSLR.eventMediaClass.mediaList
				where x.isUploaded && !x.isCloudSynced && !x.isCloudSyncing
				select x).ToList<MediaClassBase>();
				if (noDbSyncMediaList.Count == 0)
				{
					Thread.Sleep(500);
				}
				else
				{
					foreach (MediaClassBase item in noDbSyncMediaList)
					{
						BunnyCDNHelper.SyncMediaToCloud(item);
					}
					DSLR.eventMediaClass.SaveToFile();
				}
				Thread.Sleep(500);
			}
		}

		// Token: 0x06000B12 RID: 2834 RVA: 0x00040960 File Offset: 0x0003EB60
		private static void VariationMediaCloudSyncThreadFunc()
		{
			BunnyCDNHelper.<VariationMediaCloudSyncThreadFunc>d__18 <VariationMediaCloudSyncThreadFunc>d__;
			<VariationMediaCloudSyncThreadFunc>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<VariationMediaCloudSyncThreadFunc>d__.<>1__state = -1;
			<VariationMediaCloudSyncThreadFunc>d__.<>t__builder.Start<BunnyCDNHelper.<VariationMediaCloudSyncThreadFunc>d__18>(ref <VariationMediaCloudSyncThreadFunc>d__);
		}

		// Token: 0x06000B13 RID: 2835 RVA: 0x00040990 File Offset: 0x0003EB90
		public static MediaSaveModal.MediaVariationSaveResponse SyncMediaVariationsToCloud(VariationMedia item)
		{
			if (item.isCloudSyncing)
			{
				return new MediaSaveModal.MediaVariationSaveResponse
				{
					Success = false,
					Message = "isCloudSyncing true"
				};
			}
			item.isCloudSyncing = true;
			MediaSaveModal.MediaVariationSaveRequest mediaSaveVariationRequest = new MediaSaveModal.MediaVariationSaveRequest
			{
				RawFileInfo = item.VariationRawFileInfo,
				RawUrl = item.VariationRawUrl,
				ResultUrl = item.ResultUrl,
				MediaHash = item.OriginalMediaHash,
				Prompt = item.Promp,
				FileInfo = item.ResultFileInfo,
				VariationOperationType = item.VariationOperation,
				AccessToken = SessionData.accountInfo.accessToken,
				ThumbnailUrl = item.ThumbnailUrl
			};
			Task<HTTPHelper.PostResult> task = HTTPHelper.PostRequestAsync<MediaSaveModal.MediaVariationSaveRequest>("api/content/SaveUploadedMediaVariation", mediaSaveVariationRequest, null, "");
			task.ConfigureAwait(false);
			if (task.Result.ServerCommunicationSuccess)
			{
				MediaSaveModal.MediaVariationSaveResponse mediaVariationSaveResponse;
				try
				{
					mediaVariationSaveResponse = JsonConvert.DeserializeObject<MediaSaveModal.MediaVariationSaveResponse>(task.Result.Message);
				}
				catch (Exception ex)
				{
					item.isCloudSyncing = false;
					Debug.Log("BunnyCDNHelper", ex.Message, "SyncMediaVariationsToCloud", 500);
					item.isCloudSyncing = false;
					return new MediaSaveModal.MediaVariationSaveResponse
					{
						Success = false,
						Message = ex.Message
					};
				}
				if (mediaVariationSaveResponse.Success && !string.IsNullOrEmpty(mediaVariationSaveResponse.MediaVariationData.MediaHash))
				{
					item.VariationMediaHash = mediaVariationSaveResponse.MediaVariationData.VariationMediaHash;
					item.isCloudSyncing = false;
					return mediaVariationSaveResponse;
				}
				item.isCloudSyncing = false;
				return mediaVariationSaveResponse;
			}
			item.isCloudSyncing = false;
			return new MediaSaveModal.MediaVariationSaveResponse
			{
				Success = false,
				Message = task.Result.Message
			};
		}

		// Token: 0x06000B14 RID: 2836 RVA: 0x00040B30 File Offset: 0x0003ED30
		private static void SyncMediaToCloud(MediaClassBase item)
		{
			BunnyCDNHelper.<SyncMediaToCloud>d__20 <SyncMediaToCloud>d__;
			<SyncMediaToCloud>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SyncMediaToCloud>d__.item = item;
			<SyncMediaToCloud>d__.<>1__state = -1;
			<SyncMediaToCloud>d__.<>t__builder.Start<BunnyCDNHelper.<SyncMediaToCloud>d__20>(ref <SyncMediaToCloud>d__);
		}

		// Token: 0x06000B15 RID: 2837 RVA: 0x00040B68 File Offset: 0x0003ED68
		private static Task<bool> SyncMediaGifToCloud(MediaClassBase item)
		{
			BunnyCDNHelper.<SyncMediaGifToCloud>d__21 <SyncMediaGifToCloud>d__;
			<SyncMediaGifToCloud>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SyncMediaGifToCloud>d__.item = item;
			<SyncMediaGifToCloud>d__.<>1__state = -1;
			<SyncMediaGifToCloud>d__.<>t__builder.Start<BunnyCDNHelper.<SyncMediaGifToCloud>d__21>(ref <SyncMediaGifToCloud>d__);
			return <SyncMediaGifToCloud>d__.<>t__builder.Task;
		}

		// Token: 0x06000B16 RID: 2838 RVA: 0x00040BAC File Offset: 0x0003EDAC
		public static void StartUploadThread()
		{
			if (BunnyCDNHelper.UploadThread == null)
			{
				BunnyCDNHelper.UploadThread = new Thread(new ThreadStart(BunnyCDNHelper.UploadThreadFunc));
				BunnyCDNHelper.UploadThread.Start();
				BunnyCDNHelper.MediaCloudSyncThread = new Thread(new ThreadStart(BunnyCDNHelper.MediaCloudSyncThreadFunc));
				BunnyCDNHelper.MediaCloudSyncThread.Start();
				BunnyCDNHelper.VariationMediaCloudSyncThread = new Thread(new ThreadStart(BunnyCDNHelper.VariationMediaCloudSyncThreadFunc));
				BunnyCDNHelper.VariationMediaCloudSyncThread.Start();
			}
		}

		// Token: 0x04000A5C RID: 2652
		public static Thread UploadThread;

		// Token: 0x04000A5D RID: 2653
		public static Thread MediaCloudSyncThread;

		// Token: 0x04000A5E RID: 2654
		public static Thread VariationMediaCloudSyncThread;

		// Token: 0x04000A5F RID: 2655
		public static bool isForceQuit;

		// Token: 0x04000A61 RID: 2657
		private static List<VariationMedia> allVariations;

		// Token: 0x020002AE RID: 686
		public class CDNTaskResult
		{
			// Token: 0x0600113B RID: 4411 RVA: 0x00065816 File Offset: 0x00063A16
			public CDNTaskResult(bool success, BunnyCDNHelper.BunnyCDNUploadResult uploadResult)
			{
				this.Success = success;
				this.UploadResult = uploadResult;
			}

			// Token: 0x0600113C RID: 4412 RVA: 0x0006582C File Offset: 0x00063A2C
			public CDNTaskResult(bool success, GenericResponse uploadResult)
			{
				this.Success = success;
				this.UploadResult = new BunnyCDNHelper.BunnyCDNUploadResult
				{
					HttpCode = (int)uploadResult.StatusCode,
					Message = uploadResult.Message
				};
			}

			// Token: 0x040011D8 RID: 4568
			public bool Success;

			// Token: 0x040011D9 RID: 4569
			public BunnyCDNHelper.BunnyCDNUploadResult UploadResult;
		}

		// Token: 0x020002AF RID: 687
		public class BunnyCDNUploadResult
		{
			// Token: 0x040011DA RID: 4570
			public int HttpCode;

			// Token: 0x040011DB RID: 4571
			public string Message;
		}
	}
}
