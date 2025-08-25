using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Modals.Api;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Variations;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D4 RID: 212
	public static class VariationMediaHelper
	{
		// Token: 0x06000B17 RID: 2839 RVA: 0x00040C20 File Offset: 0x0003EE20
		public static string GetDataFilePath()
		{
			return Path.Combine(new string[]
			{
				Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "MJMediaData.json")
			});
		}

		// Token: 0x06000B18 RID: 2840 RVA: 0x00040C44 File Offset: 0x0003EE44
		public static void SaveFile()
		{
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
		}

		// Token: 0x06000B19 RID: 2841 RVA: 0x00040C55 File Offset: 0x0003EE55
		public static string prompControl(string promp)
		{
			return promp.Replace("--ar", "").Replace("--v", "");
		}

		// Token: 0x06000B1A RID: 2842 RVA: 0x00040C78 File Offset: 0x0003EE78
		public static VariationMedia CreateFaceSwapJob(MediaClassBase mediaClassBase, string rawUrl, string mediaURL, FaceSwapTarget faceSwapTarget, string folderPath, string mail)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string variationsNumber = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == mediaClassBase.MediaHash
			select x).Count<VariationMedia>().ToString();
			string newID = mediaClassBase.MediaHash + variationsNumber;
			string convertedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_IC.png");
			string UpscaledPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_UP.png");
			string faceSwappedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_CFS.png");
			string resultPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_Result.png");
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.AIFaceSwap);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			variationMedia.ResultLocalPath = resultPath;
			variationMedia.OriginalMediaHash = mediaClassBase.MediaHash;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.FaceSwapping;
			variationMedia.faceSwapTarget = faceSwapTarget;
			variationMedia.PreviewImagePath = Path.Combine("file://", faceSwapTarget.FileDetails.FullPath);
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B1B RID: 2843 RVA: 0x00040E08 File Offset: 0x0003F008
		public static VariationMedia CreateAIBeautifierJob(MediaClassBase mediaClassBase, string folderPath, string mail)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string variationsNumber = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == mediaClassBase.MediaHash
			select x).Count<VariationMedia>().ToString();
			string newID = mediaClassBase.MediaHash + variationsNumber;
			string convertedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_IC.png");
			string UpscaledPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_UP.png");
			string faceSwappedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_CFS.png");
			string resultPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_Result.png");
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.AiBeautifier);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			variationMedia.ResultLocalPath = resultPath;
			variationMedia.OriginalMediaHash = mediaClassBase.MediaHash;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.FaceSwapping;
			variationMedia.PreviewImagePath = Path.Combine("file://", mediaClassBase.capturedFiles[0].FullName);
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B1C RID: 2844 RVA: 0x00040F90 File Offset: 0x0003F190
		public static VariationMedia CreateFluxJob(string promp, MediaClassBase mediaClassBase, string mail)
		{
			string variationsNumber = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == mediaClassBase.MediaHash
			select x).Count<VariationMedia>().ToString();
			string imagePath = mediaClassBase.resultFile.FullName;
			Func<string, bool> <>9__2;
			SamplePrompt selectedSamplePrompt = SamplePromptsPage.SamplePromptData.SamplePrompts.FirstOrDefault(delegate(SamplePrompt samplePrompt)
			{
				IEnumerable<string> prompts = samplePrompt.Prompts;
				Func<string, bool> predicate;
				if ((predicate = <>9__2) == null)
				{
					predicate = (<>9__2 = ((string prompt) => prompt.Contains(promp)));
				}
				return prompts.Any(predicate);
			});
			if (selectedSamplePrompt != null)
			{
				imagePath = Path.Combine(new string[]
				{
					selectedSamplePrompt.FileDetails.FullPath
				});
			}
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.AIPrompt);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.VariationOperation = VariationType.AIPrompt;
			variationMedia.Promp = promp;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.SendImagineCommand;
			variationMedia.PreviewImagePath = imagePath;
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B1D RID: 2845 RVA: 0x000410B0 File Offset: 0x0003F2B0
		public static VariationMedia CreateAiMotionJob(string promp, MediaClassBase mediaClassBase, VariationMedia originalVariationMedia, string mail)
		{
			if (originalVariationMedia.VariationRawFileInfo.GetMediaType() != MediaType.photo)
			{
				return null;
			}
			string variationsNumber = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == mediaClassBase.MediaHash
			select x).Count<VariationMedia>().ToString();
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.AiMotion);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.Promp = promp;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.SendImagineCommand;
			variationMedia.MediaOriginalRawUrl = originalVariationMedia.VariationRawUrl;
			variationMedia.AncestorVariationMediaHash = originalVariationMedia.VariationMediaHash;
			variationMedia.OriginalMediaHash = mediaClassBase.MediaHash;
			variationMedia.PreviewImagePath = originalVariationMedia.ResultLocalPath;
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B1E RID: 2846 RVA: 0x000411AC File Offset: 0x0003F3AC
		public static VariationMedia CreateAiEffectJob(MediaClassBase mediaClassBase, string aiEffectID, string mail)
		{
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.AiEffect);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.VariationOperation = VariationType.AiEffect;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.SendImagineCommand;
			variationMedia.OriginalMediaHash = mediaClassBase.MediaHash;
			variationMedia.AIEffectId = aiEffectID;
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B1F RID: 2847 RVA: 0x00041240 File Offset: 0x0003F440
		public static VariationMedia CreateMidJourneyJob(string promp, string aspectRatio, MediaClassBase mediaClassBase, string mediaURL, string folderPath, string mail)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string variationsNumber = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == mediaClassBase.MediaHash
			select x).Count<VariationMedia>().ToString();
			string newID = mediaClassBase.MediaHash + variationsNumber;
			string convertedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_IC.png");
			string UpscaledPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_UP.png");
			string faceSwappedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_CFS.png");
			string resultPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_AI_" + variationsNumber + "_Result.png");
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.MidJourneyPrompt);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			variationMedia.ResultLocalPath = resultPath;
			variationMedia.OriginalMediaHash = mediaClassBase.MediaHash;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.SendImagineCommand;
			variationMedia.Promp = (VariationMediaHelper.prompControl(promp) ?? "");
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B20 RID: 2848 RVA: 0x000413C4 File Offset: 0x0003F5C4
		public static VariationMedia CreateWordPortrait(MediaClassBase mediaClassBase, string rawUrl, string mediaURL, List<string> words, string fontName, string fontColor, string backgroundColor, string folderPath, string mail)
		{
			if (!Directory.Exists(folderPath))
			{
				Directory.CreateDirectory(folderPath);
			}
			string variationsNumber = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == mediaClassBase.MediaHash
			select x).Count<VariationMedia>().ToString();
			string newID = mediaClassBase.MediaHash + variationsNumber;
			string convertedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_W_" + variationsNumber + "_IC.png");
			string UpscaledPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_W_" + variationsNumber + "_UP.png");
			string faceSwappedPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_W_" + variationsNumber + "_CFS.png");
			string resultPath = Path.Combine(folderPath, Path.GetFileNameWithoutExtension(newID) + "_W_" + variationsNumber + "_Result.png");
			VariationMedia variationMedia = new VariationMedia(mediaClassBase, VariationType.WordPortrait);
			variationMedia.index = VariationMediaHelper.VariationMediasList.Count;
			variationMedia.ResultLocalPath = resultPath;
			variationMedia.OriginalMediaHash = mediaClassBase.MediaHash;
			ParticipantInformation participantInformation;
			if (!string.IsNullOrEmpty(mail))
			{
				(participantInformation = new ParticipantInformation()).Email = mail;
			}
			else
			{
				participantInformation = null;
			}
			variationMedia.participantInformation = participantInformation;
			variationMedia.ErrorCount = 0;
			variationMedia.isCloudSyncing = false;
			variationMedia.midJourneyProcess = MediaVariationModals.MidJourneyProcess.SendImagineCommand;
			variationMedia.VariationOperation = VariationType.WordPortrait;
			variationMedia.wordPortraitModal = new WordPortraitModal
			{
				BackgroundColor = backgroundColor,
				FontColor = fontColor,
				FontName = fontName,
				Words = words,
				wordPortraitPath = convertedPath
			};
			variationMedia.PreviewImagePath = mediaClassBase.fileDetails.FullPath;
			VariationMedia refMedia = variationMedia;
			refMedia.StartProcess(0);
			VariationMediaHelper.VariationMediasList.Add(refMedia);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			return refMedia;
		}

		// Token: 0x06000B21 RID: 2849 RVA: 0x00041580 File Offset: 0x0003F780
		public static Task<VariationMediaHelper.ImagineResponse> SendImagine(VariationMediaHelper.ImagineRequest requestObject)
		{
			VariationMediaHelper.<SendImagine>d__20 <SendImagine>d__;
			<SendImagine>d__.<>t__builder = AsyncTaskMethodBuilder<VariationMediaHelper.ImagineResponse>.Create();
			<SendImagine>d__.requestObject = requestObject;
			<SendImagine>d__.<>1__state = -1;
			<SendImagine>d__.<>t__builder.Start<VariationMediaHelper.<SendImagine>d__20>(ref <SendImagine>d__);
			return <SendImagine>d__.<>t__builder.Task;
		}

		// Token: 0x06000B22 RID: 2850 RVA: 0x000415C4 File Offset: 0x0003F7C4
		public static Task<VariationMediaHelper.ImagineResultResponse> GetImagineResult(string taskId)
		{
			VariationMediaHelper.<GetImagineResult>d__21 <GetImagineResult>d__;
			<GetImagineResult>d__.<>t__builder = AsyncTaskMethodBuilder<VariationMediaHelper.ImagineResultResponse>.Create();
			<GetImagineResult>d__.taskId = taskId;
			<GetImagineResult>d__.<>1__state = -1;
			<GetImagineResult>d__.<>t__builder.Start<VariationMediaHelper.<GetImagineResult>d__21>(ref <GetImagineResult>d__);
			return <GetImagineResult>d__.<>t__builder.Task;
		}

		// Token: 0x06000B23 RID: 2851 RVA: 0x00041608 File Offset: 0x0003F808
		public static Task<float> GetAICredits(string accessToken)
		{
			VariationMediaHelper.<GetAICredits>d__22 <GetAICredits>d__;
			<GetAICredits>d__.<>t__builder = AsyncTaskMethodBuilder<float>.Create();
			<GetAICredits>d__.accessToken = accessToken;
			<GetAICredits>d__.<>1__state = -1;
			<GetAICredits>d__.<>t__builder.Start<VariationMediaHelper.<GetAICredits>d__22>(ref <GetAICredits>d__);
			return <GetAICredits>d__.<>t__builder.Task;
		}

		// Token: 0x04000A62 RID: 2658
		public static bool isForceQuit = false;

		// Token: 0x04000A63 RID: 2659
		public static List<VariationMedia> VariationMediasList;

		// Token: 0x04000A64 RID: 2660
		private static bool isFirstWork = true;

		// Token: 0x04000A65 RID: 2661
		private const string secretKey = "18405e60-8181-11ee-9f62-a5f1bd18cd6d";

		// Token: 0x020002B8 RID: 696
		public class GetApiKeyResponse
		{
			// Token: 0x17000260 RID: 608
			// (get) Token: 0x06001151 RID: 4433 RVA: 0x00066F32 File Offset: 0x00065132
			// (set) Token: 0x06001152 RID: 4434 RVA: 0x00066F3A File Offset: 0x0006513A
			public string ApiKey { get; set; }

			// Token: 0x17000261 RID: 609
			// (get) Token: 0x06001153 RID: 4435 RVA: 0x00066F43 File Offset: 0x00065143
			// (set) Token: 0x06001154 RID: 4436 RVA: 0x00066F4B File Offset: 0x0006514B
			public int ApiIndex { get; set; }
		}

		// Token: 0x020002B9 RID: 697
		public class MidjourneyTask : Base
		{
			// Token: 0x06001156 RID: 4438 RVA: 0x00066F5C File Offset: 0x0006515C
			public MidjourneyTask(string prompt, string requestIp, string taskId, string channelId, string mediaDataId)
			{
				this.Prompt = prompt;
				this.RequestIP = requestIp;
				this.Status = VariationMediaHelper.MidjourneyTask.MidjourneyTaskStatus.WaitingToStart;
				this.DiscordChannelId = channelId;
				this.TaskId = taskId;
				this.Percentage = 0;
				this.MediaDataId = mediaDataId;
			}

			// Token: 0x17000262 RID: 610
			// (get) Token: 0x06001157 RID: 4439 RVA: 0x00066F97 File Offset: 0x00065197
			// (set) Token: 0x06001158 RID: 4440 RVA: 0x00066F9F File Offset: 0x0006519F
			public string MediaDataId { get; set; }

			// Token: 0x17000263 RID: 611
			// (get) Token: 0x06001159 RID: 4441 RVA: 0x00066FA8 File Offset: 0x000651A8
			// (set) Token: 0x0600115A RID: 4442 RVA: 0x00066FB0 File Offset: 0x000651B0
			public string TaskId { get; set; }

			// Token: 0x17000264 RID: 612
			// (get) Token: 0x0600115B RID: 4443 RVA: 0x00066FB9 File Offset: 0x000651B9
			// (set) Token: 0x0600115C RID: 4444 RVA: 0x00066FC1 File Offset: 0x000651C1
			public string AfterId { get; set; }

			// Token: 0x17000265 RID: 613
			// (get) Token: 0x0600115D RID: 4445 RVA: 0x00066FCA File Offset: 0x000651CA
			// (set) Token: 0x0600115E RID: 4446 RVA: 0x00066FD2 File Offset: 0x000651D2
			public string Prompt { get; set; }

			// Token: 0x17000266 RID: 614
			// (get) Token: 0x0600115F RID: 4447 RVA: 0x00066FDB File Offset: 0x000651DB
			// (set) Token: 0x06001160 RID: 4448 RVA: 0x00066FE3 File Offset: 0x000651E3
			public string DiscordChannelId { get; set; }

			// Token: 0x17000267 RID: 615
			// (get) Token: 0x06001161 RID: 4449 RVA: 0x00066FEC File Offset: 0x000651EC
			// (set) Token: 0x06001162 RID: 4450 RVA: 0x00066FF4 File Offset: 0x000651F4
			public string PhotoUrl { get; set; }

			// Token: 0x17000268 RID: 616
			// (get) Token: 0x06001163 RID: 4451 RVA: 0x00066FFD File Offset: 0x000651FD
			// (set) Token: 0x06001164 RID: 4452 RVA: 0x00067005 File Offset: 0x00065205
			public string Error { get; set; }

			// Token: 0x17000269 RID: 617
			// (get) Token: 0x06001165 RID: 4453 RVA: 0x0006700E File Offset: 0x0006520E
			// (set) Token: 0x06001166 RID: 4454 RVA: 0x00067016 File Offset: 0x00065216
			public bool IsErrorStatus { get; set; }

			// Token: 0x1700026A RID: 618
			// (get) Token: 0x06001167 RID: 4455 RVA: 0x0006701F File Offset: 0x0006521F
			// (set) Token: 0x06001168 RID: 4456 RVA: 0x00067027 File Offset: 0x00065227
			public int Percentage { get; set; }

			// Token: 0x1700026B RID: 619
			// (get) Token: 0x06001169 RID: 4457 RVA: 0x00067030 File Offset: 0x00065230
			// (set) Token: 0x0600116A RID: 4458 RVA: 0x00067038 File Offset: 0x00065238
			public VariationMediaHelper.MidjourneyTask.MidjourneyTaskStatus Status { get; set; }

			// Token: 0x1700026C RID: 620
			// (get) Token: 0x0600116B RID: 4459 RVA: 0x00067041 File Offset: 0x00065241
			// (set) Token: 0x0600116C RID: 4460 RVA: 0x00067049 File Offset: 0x00065249
			public string RequestIP { get; set; }

			// Token: 0x1700026D RID: 621
			// (get) Token: 0x0600116D RID: 4461 RVA: 0x00067052 File Offset: 0x00065252
			// (set) Token: 0x0600116E RID: 4462 RVA: 0x0006705A File Offset: 0x0006525A
			public DateTime? ImagineFinishedTime { get; set; }

			// Token: 0x1700026E RID: 622
			// (get) Token: 0x0600116F RID: 4463 RVA: 0x00067063 File Offset: 0x00065263
			// (set) Token: 0x06001170 RID: 4464 RVA: 0x0006706B File Offset: 0x0006526B
			public string U1Url { get; set; }

			// Token: 0x1700026F RID: 623
			// (get) Token: 0x06001171 RID: 4465 RVA: 0x00067074 File Offset: 0x00065274
			// (set) Token: 0x06001172 RID: 4466 RVA: 0x0006707C File Offset: 0x0006527C
			public string U2Url { get; set; }

			// Token: 0x17000270 RID: 624
			// (get) Token: 0x06001173 RID: 4467 RVA: 0x00067085 File Offset: 0x00065285
			// (set) Token: 0x06001174 RID: 4468 RVA: 0x0006708D File Offset: 0x0006528D
			public string U3Url { get; set; }

			// Token: 0x17000271 RID: 625
			// (get) Token: 0x06001175 RID: 4469 RVA: 0x00067096 File Offset: 0x00065296
			// (set) Token: 0x06001176 RID: 4470 RVA: 0x0006709E File Offset: 0x0006529E
			public string U4Url { get; set; }

			// Token: 0x020002FE RID: 766
			public enum MidjourneyTaskStatus
			{
				// Token: 0x040013AB RID: 5035
				WaitingToStart,
				// Token: 0x040013AC RID: 5036
				Started,
				// Token: 0x040013AD RID: 5037
				Processing,
				// Token: 0x040013AE RID: 5038
				Finished
			}
		}

		// Token: 0x020002BA RID: 698
		public class ImagineRequest
		{
			// Token: 0x06001177 RID: 4471 RVA: 0x000670A7 File Offset: 0x000652A7
			public ImagineRequest(string prompt, string mediaHash, string accessToken)
			{
				this.Prompt = prompt;
				this.MediaHash = mediaHash;
				this.AccessToken = accessToken;
			}

			// Token: 0x17000272 RID: 626
			// (get) Token: 0x06001178 RID: 4472 RVA: 0x000670C4 File Offset: 0x000652C4
			// (set) Token: 0x06001179 RID: 4473 RVA: 0x000670CC File Offset: 0x000652CC
			public string Prompt { get; private set; }

			// Token: 0x17000273 RID: 627
			// (get) Token: 0x0600117A RID: 4474 RVA: 0x000670D5 File Offset: 0x000652D5
			// (set) Token: 0x0600117B RID: 4475 RVA: 0x000670DD File Offset: 0x000652DD
			public string MediaHash { get; private set; }

			// Token: 0x17000274 RID: 628
			// (get) Token: 0x0600117C RID: 4476 RVA: 0x000670E6 File Offset: 0x000652E6
			// (set) Token: 0x0600117D RID: 4477 RVA: 0x000670EE File Offset: 0x000652EE
			public string AccessToken { get; private set; }
		}

		// Token: 0x020002BB RID: 699
		public class ImagineResponse
		{
			// Token: 0x17000275 RID: 629
			// (get) Token: 0x0600117E RID: 4478 RVA: 0x000670F7 File Offset: 0x000652F7
			// (set) Token: 0x0600117F RID: 4479 RVA: 0x000670FF File Offset: 0x000652FF
			public bool Success { get; set; }

			// Token: 0x17000276 RID: 630
			// (get) Token: 0x06001180 RID: 4480 RVA: 0x00067108 File Offset: 0x00065308
			// (set) Token: 0x06001181 RID: 4481 RVA: 0x00067110 File Offset: 0x00065310
			public string TaskId { get; set; }

			// Token: 0x17000277 RID: 631
			// (get) Token: 0x06001182 RID: 4482 RVA: 0x00067119 File Offset: 0x00065319
			// (set) Token: 0x06001183 RID: 4483 RVA: 0x00067121 File Offset: 0x00065321
			public string Error { get; set; }
		}

		// Token: 0x020002BC RID: 700
		public class ImagineResultRequest
		{
			// Token: 0x17000278 RID: 632
			// (get) Token: 0x06001185 RID: 4485 RVA: 0x00067132 File Offset: 0x00065332
			// (set) Token: 0x06001186 RID: 4486 RVA: 0x0006713A File Offset: 0x0006533A
			public string TaskId { get; set; }
		}

		// Token: 0x020002BD RID: 701
		public class ImagineResultResponse
		{
			// Token: 0x06001188 RID: 4488 RVA: 0x0006714B File Offset: 0x0006534B
			public ImagineResultResponse(bool success, VariationMediaHelper.MidjourneyTask midjourneyTask)
			{
				this.Success = success;
				this.ProcessedMidjourneyTask = midjourneyTask;
			}

			// Token: 0x06001189 RID: 4489 RVA: 0x00067161 File Offset: 0x00065361
			[JsonConstructor]
			public ImagineResultResponse(bool success)
			{
				this.Success = success;
			}

			// Token: 0x17000279 RID: 633
			// (get) Token: 0x0600118A RID: 4490 RVA: 0x00067170 File Offset: 0x00065370
			// (set) Token: 0x0600118B RID: 4491 RVA: 0x00067178 File Offset: 0x00065378
			public bool Success { get; private set; }

			// Token: 0x1700027A RID: 634
			// (get) Token: 0x0600118C RID: 4492 RVA: 0x00067181 File Offset: 0x00065381
			// (set) Token: 0x0600118D RID: 4493 RVA: 0x00067189 File Offset: 0x00065389
			public VariationMediaHelper.MidjourneyTask ProcessedMidjourneyTask { get; set; }

			// Token: 0x1700027B RID: 635
			// (get) Token: 0x0600118E RID: 4494 RVA: 0x00067192 File Offset: 0x00065392
			// (set) Token: 0x0600118F RID: 4495 RVA: 0x0006719A File Offset: 0x0006539A
			public string Error { get; set; }

			// Token: 0x1700027C RID: 636
			// (get) Token: 0x06001190 RID: 4496 RVA: 0x000671A3 File Offset: 0x000653A3
			// (set) Token: 0x06001191 RID: 4497 RVA: 0x000671AB File Offset: 0x000653AB
			public string Status { get; set; }

			// Token: 0x1700027D RID: 637
			// (get) Token: 0x06001192 RID: 4498 RVA: 0x000671B4 File Offset: 0x000653B4
			// (set) Token: 0x06001193 RID: 4499 RVA: 0x000671BC File Offset: 0x000653BC
			public string EstimatedWaitingTime { get; set; }

			// Token: 0x1700027E RID: 638
			// (get) Token: 0x06001194 RID: 4500 RVA: 0x000671C5 File Offset: 0x000653C5
			// (set) Token: 0x06001195 RID: 4501 RVA: 0x000671CD File Offset: 0x000653CD
			public long OrderInQueue { get; set; }

			// Token: 0x1700027F RID: 639
			// (get) Token: 0x06001196 RID: 4502 RVA: 0x000671D6 File Offset: 0x000653D6
			// (set) Token: 0x06001197 RID: 4503 RVA: 0x000671DE File Offset: 0x000653DE
			public TimeSpan EstimatedWaitingTimeSpan { get; set; }
		}
	}
}
