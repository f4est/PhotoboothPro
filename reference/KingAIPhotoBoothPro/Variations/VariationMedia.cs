using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ImageMagick;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Modals.Api;
using KingAIPhotoBoothPro.Modals.Api.AiMotion;
using KingAIPhotoBoothPro.Pages;
using Newtonsoft.Json;
using SendGrid.Helpers.Errors.Model;

namespace KingAIPhotoBoothPro.Variations
{
	// Token: 0x0200001E RID: 30
	public class VariationMedia : Base
	{
		// Token: 0x0600011D RID: 285 RVA: 0x000076F4 File Offset: 0x000058F4
		public VariationMedia()
		{
			this.StartTime = DateTime.Now;
			this._midJourneyProcess = MediaVariationModals.MidJourneyProcess.Completed;
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0000772C File Offset: 0x0000592C
		public VariationMedia(MediaClassBase mediaClassBase, VariationType variationOperation)
		{
			this.StartTime = DateTime.Now;
			if (mediaClassBase == null)
			{
				throw new ArgumentNullException("You must provide a valid original media!");
			}
			if (variationOperation == VariationType.None)
			{
				throw new BadRequestException("You must select a variation type!");
			}
			this.VariationOperation = variationOperation;
			this.LocalId = HashGenerator.GenerateHash(Guid.NewGuid().ToString());
			this.OriginalMediaHash = mediaClassBase.MediaHash;
			this.MediaOriginalRawUrl = mediaClassBase.RawUrl;
			OperationalEvent operationalEvent = (from e in EventManagementPage.Events
			where e.EventHash == mediaClassBase.EventHash
			select e).FirstOrDefault<OperationalEvent>();
			if (operationalEvent == null)
			{
				operationalEvent = EventManagementPage.GetCurrentEvent();
			}
			this.VariationRawLocalPath = this.GenerateVariationRawFilePath(variationOperation, operationalEvent, mediaClassBase);
			this.ResultLocalPath = this.GenerateResultLocalPath(variationOperation, operationalEvent, mediaClassBase);
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x0600011F RID: 287 RVA: 0x00007829 File Offset: 0x00005A29
		// (set) Token: 0x06000120 RID: 288 RVA: 0x00007831 File Offset: 0x00005A31
		[JsonProperty("resultUrl")]
		public string ResultUrl { get; set; }

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000121 RID: 289 RVA: 0x0000783A File Offset: 0x00005A3A
		// (set) Token: 0x06000122 RID: 290 RVA: 0x00007842 File Offset: 0x00005A42
		[JsonProperty("variationRawUrl")]
		public string VariationRawUrl { get; set; }

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000123 RID: 291 RVA: 0x0000784B File Offset: 0x00005A4B
		// (set) Token: 0x06000124 RID: 292 RVA: 0x00007853 File Offset: 0x00005A53
		[JsonProperty("thumbnailUrl")]
		public string ThumbnailUrl { get; set; }

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x06000125 RID: 293 RVA: 0x0000785C File Offset: 0x00005A5C
		// (set) Token: 0x06000126 RID: 294 RVA: 0x00007864 File Offset: 0x00005A64
		[JsonProperty("mediaOriginalRawUrl")]
		public string MediaOriginalRawUrl { get; set; }

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x06000127 RID: 295 RVA: 0x0000786D File Offset: 0x00005A6D
		// (set) Token: 0x06000128 RID: 296 RVA: 0x00007875 File Offset: 0x00005A75
		[JsonProperty("resultFileInfo")]
		public FileInformation ResultFileInfo { get; set; }

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x06000129 RID: 297 RVA: 0x0000787E File Offset: 0x00005A7E
		// (set) Token: 0x0600012A RID: 298 RVA: 0x00007886 File Offset: 0x00005A86
		[JsonProperty("variationRawFileInfo")]
		public FileInformation VariationRawFileInfo { get; set; }

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600012B RID: 299 RVA: 0x0000788F File Offset: 0x00005A8F
		// (set) Token: 0x0600012C RID: 300 RVA: 0x00007897 File Offset: 0x00005A97
		[JsonProperty("thumbnailFileInfo")]
		public FileInformation ThumbnailFileInfo { get; set; }

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x0600012D RID: 301 RVA: 0x000078A0 File Offset: 0x00005AA0
		// (set) Token: 0x0600012E RID: 302 RVA: 0x000078A8 File Offset: 0x00005AA8
		[JsonProperty("variationRawLocalPath")]
		public string VariationRawLocalPath { get; set; }

		// Token: 0x17000037 RID: 55
		// (get) Token: 0x0600012F RID: 303 RVA: 0x000078B1 File Offset: 0x00005AB1
		// (set) Token: 0x06000130 RID: 304 RVA: 0x000078B9 File Offset: 0x00005AB9
		[JsonProperty("resultLocalPath")]
		public string ResultLocalPath { get; set; }

		// Token: 0x17000038 RID: 56
		// (get) Token: 0x06000131 RID: 305 RVA: 0x000078C4 File Offset: 0x00005AC4
		// (set) Token: 0x06000132 RID: 306 RVA: 0x0000797B File Offset: 0x00005B7B
		public string ThumbnailLocalPath
		{
			get
			{
				if (string.IsNullOrEmpty(this.thumbnailLocalPath) || !File.Exists(this.thumbnailLocalPath))
				{
					if (!string.IsNullOrEmpty(this.AncestorVariationMediaHash))
					{
						VariationMedia AncestorVariationMedia = (from x in VariationMediaHelper.VariationMediasList
						where x.VariationMediaHash == this.AncestorVariationMediaHash
						select x).FirstOrDefault<VariationMedia>();
						if (AncestorVariationMedia != null)
						{
							this.thumbnailLocalPath = AncestorVariationMedia.ResultLocalPath;
						}
					}
					if (string.IsNullOrEmpty(this.thumbnailLocalPath) || !File.Exists(this.thumbnailLocalPath))
					{
						MediaClassBase mediaClassBase = (from x in DSLR.eventMediaClass.mediaList
						where x.MediaHash == this.OriginalMediaHash
						select x).FirstOrDefault<MediaClassBase>();
						if (mediaClassBase != null)
						{
							this.thumbnailLocalPath = mediaClassBase.thumbnail.FullName;
						}
					}
				}
				return this.thumbnailLocalPath;
			}
			set
			{
				this.thumbnailLocalPath = value;
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x06000133 RID: 307 RVA: 0x00007984 File Offset: 0x00005B84
		// (set) Token: 0x06000134 RID: 308 RVA: 0x0000798C File Offset: 0x00005B8C
		public string LocalId
		{
			get
			{
				return this.localId;
			}
			set
			{
				this.localId = value;
			}
		}

		// Token: 0x1700003A RID: 58
		// (get) Token: 0x06000135 RID: 309 RVA: 0x00007995 File Offset: 0x00005B95
		// (set) Token: 0x06000136 RID: 310 RVA: 0x0000799D File Offset: 0x00005B9D
		[JsonProperty("wordPortraitModal")]
		public WordPortraitModal wordPortraitModal { get; set; }

		// Token: 0x1700003B RID: 59
		// (get) Token: 0x06000137 RID: 311 RVA: 0x000079A6 File Offset: 0x00005BA6
		// (set) Token: 0x06000138 RID: 312 RVA: 0x000079AE File Offset: 0x00005BAE
		[JsonProperty("errorCount")]
		public int ErrorCount { get; set; }

		// Token: 0x1700003C RID: 60
		// (get) Token: 0x06000139 RID: 313 RVA: 0x000079B7 File Offset: 0x00005BB7
		// (set) Token: 0x0600013A RID: 314 RVA: 0x000079BF File Offset: 0x00005BBF
		[JsonProperty("originalMediaHash")]
		public string OriginalMediaHash { get; set; }

		// Token: 0x1700003D RID: 61
		// (get) Token: 0x0600013B RID: 315 RVA: 0x000079C8 File Offset: 0x00005BC8
		// (set) Token: 0x0600013C RID: 316 RVA: 0x000079D0 File Offset: 0x00005BD0
		[JsonProperty("variationMediaHash")]
		public string VariationMediaHash { get; set; }

		// Token: 0x1700003E RID: 62
		// (get) Token: 0x0600013D RID: 317 RVA: 0x000079D9 File Offset: 0x00005BD9
		// (set) Token: 0x0600013E RID: 318 RVA: 0x000079E1 File Offset: 0x00005BE1
		[JsonProperty("ancestorVariationMediaHash")]
		public string AncestorVariationMediaHash { get; set; }

		// Token: 0x1700003F RID: 63
		// (get) Token: 0x0600013F RID: 319 RVA: 0x000079EA File Offset: 0x00005BEA
		// (set) Token: 0x06000140 RID: 320 RVA: 0x000079F2 File Offset: 0x00005BF2
		[JsonProperty("promp")]
		public string Promp { get; set; }

		// Token: 0x17000040 RID: 64
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000079FB File Offset: 0x00005BFB
		// (set) Token: 0x06000142 RID: 322 RVA: 0x00007A03 File Offset: 0x00005C03
		[JsonProperty("midJourneyResultPhotoUrl")]
		public string MidJourneyResultPhotoUrl { get; set; }

		// Token: 0x17000041 RID: 65
		// (get) Token: 0x06000143 RID: 323 RVA: 0x00007A0C File Offset: 0x00005C0C
		// (set) Token: 0x06000144 RID: 324 RVA: 0x00007A14 File Offset: 0x00005C14
		[JsonProperty("midJourneyResultPhotoLocalPath")]
		public string MidJourneyResultPhotoLocalPath { get; set; }

		// Token: 0x17000042 RID: 66
		// (get) Token: 0x06000145 RID: 325 RVA: 0x00007A1D File Offset: 0x00005C1D
		// (set) Token: 0x06000146 RID: 326 RVA: 0x00007A25 File Offset: 0x00005C25
		[JsonProperty("midJourneyTaskId")]
		public string MidJourneyTaskId { get; set; }

		// Token: 0x17000043 RID: 67
		// (get) Token: 0x06000147 RID: 327 RVA: 0x00007A2E File Offset: 0x00005C2E
		// (set) Token: 0x06000148 RID: 328 RVA: 0x00007A36 File Offset: 0x00005C36
		[JsonProperty("midJourneyResultSeperatedPhotosUrls")]
		public List<string> MidJourneyResultSeperatedPhotosUrls { get; set; }

		// Token: 0x17000044 RID: 68
		// (get) Token: 0x06000149 RID: 329 RVA: 0x00007A3F File Offset: 0x00005C3F
		// (set) Token: 0x0600014A RID: 330 RVA: 0x00007A47 File Offset: 0x00005C47
		[JsonProperty("upscaledPath")]
		public string UpscaledPath { get; set; }

		// Token: 0x17000045 RID: 69
		// (get) Token: 0x0600014B RID: 331 RVA: 0x00007A50 File Offset: 0x00005C50
		// (set) Token: 0x0600014C RID: 332 RVA: 0x00007A58 File Offset: 0x00005C58
		[JsonProperty("faceSwapTarget")]
		public FaceSwapTarget faceSwapTarget { get; set; }

		// Token: 0x17000046 RID: 70
		// (get) Token: 0x0600014D RID: 333 RVA: 0x00007A61 File Offset: 0x00005C61
		// (set) Token: 0x0600014E RID: 334 RVA: 0x00007A69 File Offset: 0x00005C69
		[JsonProperty("participantInformation")]
		public ParticipantInformation participantInformation { get; set; }

		// Token: 0x17000047 RID: 71
		// (get) Token: 0x0600014F RID: 335 RVA: 0x00007A72 File Offset: 0x00005C72
		// (set) Token: 0x06000150 RID: 336 RVA: 0x00007A7A File Offset: 0x00005C7A
		[JsonProperty("aieffectid")]
		public string AIEffectId { get; set; }

		// Token: 0x17000048 RID: 72
		// (get) Token: 0x06000151 RID: 337 RVA: 0x00007A83 File Offset: 0x00005C83
		// (set) Token: 0x06000152 RID: 338 RVA: 0x00007A8C File Offset: 0x00005C8C
		[JsonIgnore]
		public MediaVariationModals.MidJourneyProcess midJourneyProcess
		{
			get
			{
				return this._midJourneyProcess;
			}
			set
			{
				this._midJourneyProcess = value;
				if (value == MediaVariationModals.MidJourneyProcess.Completed)
				{
					this.SetStatus("Completed");
					if (this.MediaVariationSynced)
					{
						Action<VariationMedia> variationProcessCompleted = VariationMedia.VariationProcessCompleted;
						if (variationProcessCompleted != null)
						{
							variationProcessCompleted(this);
						}
						ExtensionMethod.CopyFileToOutputFolder(this.ResultLocalPath, this.VariationOperation, false);
					}
				}
			}
		}

		// Token: 0x17000049 RID: 73
		// (get) Token: 0x06000153 RID: 339 RVA: 0x00007ADB File Offset: 0x00005CDB
		// (set) Token: 0x06000154 RID: 340 RVA: 0x00007AE3 File Offset: 0x00005CE3
		public string PreviewImagePath { get; set; }

		// Token: 0x1700004A RID: 74
		// (get) Token: 0x06000155 RID: 341 RVA: 0x00007AEC File Offset: 0x00005CEC
		// (set) Token: 0x06000156 RID: 342 RVA: 0x00007AF4 File Offset: 0x00005CF4
		private DateTime StartTime { get; set; }

		// Token: 0x1700004B RID: 75
		// (get) Token: 0x06000157 RID: 343 RVA: 0x00007AFD File Offset: 0x00005CFD
		// (set) Token: 0x06000158 RID: 344 RVA: 0x00007B05 File Offset: 0x00005D05
		[JsonIgnore]
		public bool isCloudSyncing { get; set; }

		// Token: 0x1700004C RID: 76
		// (get) Token: 0x06000159 RID: 345 RVA: 0x00007B0E File Offset: 0x00005D0E
		public bool MediaVariationSynced
		{
			get
			{
				return !string.IsNullOrEmpty(this.VariationMediaHash);
			}
		}

		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600015A RID: 346 RVA: 0x00007B1E File Offset: 0x00005D1E
		public MediaType ResultMediaType
		{
			get
			{
				return this.GetMediaType(this.ResultLocalPath);
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00007B2C File Offset: 0x00005D2C
		private MediaType GetMediaType(string filename)
		{
			string extension = Path.GetExtension(filename).ToLower();
			if (extension == ".mp4" || extension == ".mov")
			{
				return MediaType.video;
			}
			if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
			{
				return MediaType.photo;
			}
			return MediaType.unknown;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x0600015C RID: 348 RVA: 0x00007B8C File Offset: 0x00005D8C
		// (remove) Token: 0x0600015D RID: 349 RVA: 0x00007BC0 File Offset: 0x00005DC0
		public static event Action<VariationMedia> ResultLocalImageSaved;

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x0600015E RID: 350 RVA: 0x00007BF4 File Offset: 0x00005DF4
		// (remove) Token: 0x0600015F RID: 351 RVA: 0x00007C28 File Offset: 0x00005E28
		public static event Action<VariationMedia> MJFirstImageGenerated;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000160 RID: 352 RVA: 0x00007C5C File Offset: 0x00005E5C
		// (remove) Token: 0x06000161 RID: 353 RVA: 0x00007C90 File Offset: 0x00005E90
		public static event Action<string> MJDataGenerateError;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000162 RID: 354 RVA: 0x00007CC4 File Offset: 0x00005EC4
		// (remove) Token: 0x06000163 RID: 355 RVA: 0x00007CF8 File Offset: 0x00005EF8
		public static event Action<VariationMedia> MJFaceSwapError;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000164 RID: 356 RVA: 0x00007D2C File Offset: 0x00005F2C
		// (remove) Token: 0x06000165 RID: 357 RVA: 0x00007D60 File Offset: 0x00005F60
		public static event Action<string, string> StatusUpdate;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000166 RID: 358 RVA: 0x00007D94 File Offset: 0x00005F94
		// (remove) Token: 0x06000167 RID: 359 RVA: 0x00007DC8 File Offset: 0x00005FC8
		public static event Action<string> ImagineJobCanceled;

		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000168 RID: 360 RVA: 0x00007DFC File Offset: 0x00005FFC
		public int ProcessPercentage
		{
			get
			{
				if (this.midJourneyProcess == MediaVariationModals.MidJourneyProcess.Completed)
				{
					return 100;
				}
				return Math.Min((int)((DateTime.Now - this.StartTime).TotalSeconds * 100.0 / this.EstimatedCompleteTime), 100);
			}
		}

		// Token: 0x1700004F RID: 79
		// (get) Token: 0x06000169 RID: 361 RVA: 0x00007E48 File Offset: 0x00006048
		public double EstimatedCompleteTime
		{
			get
			{
				switch (this.VariationOperation)
				{
				case VariationType.AIPrompt:
					return VariationMedia.AiPromptEstimatedCompleteTime;
				case VariationType.AIFaceSwap:
					return VariationMedia.FaceSwapEstimatedCompleteTime;
				case VariationType.WordPortrait:
					return VariationMedia.WordPortraitEstimatedCompleteTime;
				case VariationType.AiMotion:
					return VariationMedia.AiMotionEstimatedCompleteTime;
				}
				return VariationMedia.AiMotionEstimatedCompleteTime;
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x00007E98 File Offset: 0x00006098
		private string GenerateResultLocalPath(VariationType variationOperationType, OperationalEvent operationalEvent, MediaClassBase mediaClassBase)
		{
			this.variationDirectoryPath = Path.Combine(operationalEvent.DirectoryPath, "Variations", this.LocalId);
			if (!Directory.Exists(this.variationDirectoryPath))
			{
				Directory.CreateDirectory(this.variationDirectoryPath);
			}
			string resultExtention = ".jpg";
			if (variationOperationType == VariationType.AiMotion)
			{
				resultExtention = ".mp4";
			}
			return Path.Combine(this.variationDirectoryPath, this.LocalId + "_result" + resultExtention);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00007F08 File Offset: 0x00006108
		private string GenerateVariationRawFilePath(VariationType variationOperationType, OperationalEvent operationalEvent, MediaClassBase mediaClassBase)
		{
			this.variationDirectoryPath = Path.Combine(operationalEvent.DirectoryPath, "Variations", this.LocalId);
			if (!Directory.Exists(this.variationDirectoryPath))
			{
				Directory.CreateDirectory(this.variationDirectoryPath);
			}
			string resultExtention = ".jpg";
			if (variationOperationType == VariationType.AiMotion)
			{
				resultExtention = ".mp4";
			}
			return Path.Combine(this.variationDirectoryPath, this.LocalId + "_raw" + resultExtention);
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00007F78 File Offset: 0x00006178
		public Task<AIEffectModals.ApplyAiEffectResponse> ApplyAIEffect()
		{
			VariationMedia.<ApplyAIEffect>d__160 <ApplyAIEffect>d__;
			<ApplyAIEffect>d__.<>t__builder = AsyncTaskMethodBuilder<AIEffectModals.ApplyAiEffectResponse>.Create();
			<ApplyAIEffect>d__.<>4__this = this;
			<ApplyAIEffect>d__.<>1__state = -1;
			<ApplyAIEffect>d__.<>t__builder.Start<VariationMedia.<ApplyAIEffect>d__160>(ref <ApplyAIEffect>d__);
			return <ApplyAIEffect>d__.<>t__builder.Task;
		}

		// Token: 0x0600016D RID: 365 RVA: 0x00007FBC File Offset: 0x000061BC
		public Task<MediaVariationModals.FaceSwapResponse> ApplyFaceSwap()
		{
			VariationMedia.<ApplyFaceSwap>d__161 <ApplyFaceSwap>d__;
			<ApplyFaceSwap>d__.<>t__builder = AsyncTaskMethodBuilder<MediaVariationModals.FaceSwapResponse>.Create();
			<ApplyFaceSwap>d__.<>4__this = this;
			<ApplyFaceSwap>d__.<>1__state = -1;
			<ApplyFaceSwap>d__.<>t__builder.Start<VariationMedia.<ApplyFaceSwap>d__161>(ref <ApplyFaceSwap>d__);
			return <ApplyFaceSwap>d__.<>t__builder.Task;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00008000 File Offset: 0x00006200
		public Task<WordPortraitModals.WordPortraitResponse> ApplyWordPortrait()
		{
			VariationMedia.<ApplyWordPortrait>d__162 <ApplyWordPortrait>d__;
			<ApplyWordPortrait>d__.<>t__builder = AsyncTaskMethodBuilder<WordPortraitModals.WordPortraitResponse>.Create();
			<ApplyWordPortrait>d__.<>4__this = this;
			<ApplyWordPortrait>d__.<>1__state = -1;
			<ApplyWordPortrait>d__.<>t__builder.Start<VariationMedia.<ApplyWordPortrait>d__162>(ref <ApplyWordPortrait>d__);
			return <ApplyWordPortrait>d__.<>t__builder.Task;
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00008044 File Offset: 0x00006244
		public Task<AiImageMotionRunResponse> RunAiMotion(string prompt)
		{
			VariationMedia.<RunAiMotion>d__163 <RunAiMotion>d__;
			<RunAiMotion>d__.<>t__builder = AsyncTaskMethodBuilder<AiImageMotionRunResponse>.Create();
			<RunAiMotion>d__.<>4__this = this;
			<RunAiMotion>d__.prompt = prompt;
			<RunAiMotion>d__.<>1__state = -1;
			<RunAiMotion>d__.<>t__builder.Start<VariationMedia.<RunAiMotion>d__163>(ref <RunAiMotion>d__);
			return <RunAiMotion>d__.<>t__builder.Task;
		}

		// Token: 0x06000170 RID: 368 RVA: 0x00008090 File Offset: 0x00006290
		public Task<AIEffectModals.AiEffectRunResponse> RunAiEffect(string effectID)
		{
			VariationMedia.<RunAiEffect>d__164 <RunAiEffect>d__;
			<RunAiEffect>d__.<>t__builder = AsyncTaskMethodBuilder<AIEffectModals.AiEffectRunResponse>.Create();
			<RunAiEffect>d__.<>4__this = this;
			<RunAiEffect>d__.effectID = effectID;
			<RunAiEffect>d__.<>1__state = -1;
			<RunAiEffect>d__.<>t__builder.Start<VariationMedia.<RunAiEffect>d__164>(ref <RunAiEffect>d__);
			return <RunAiEffect>d__.<>t__builder.Task;
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000080DC File Offset: 0x000062DC
		public Task<AIEffectModals.AiEffectResultResponse> GetResultAiEffect(string taskID, string aiEffectID)
		{
			VariationMedia.<GetResultAiEffect>d__165 <GetResultAiEffect>d__;
			<GetResultAiEffect>d__.<>t__builder = AsyncTaskMethodBuilder<AIEffectModals.AiEffectResultResponse>.Create();
			<GetResultAiEffect>d__.<>4__this = this;
			<GetResultAiEffect>d__.taskID = taskID;
			<GetResultAiEffect>d__.aiEffectID = aiEffectID;
			<GetResultAiEffect>d__.<>1__state = -1;
			<GetResultAiEffect>d__.<>t__builder.Start<VariationMedia.<GetResultAiEffect>d__165>(ref <GetResultAiEffect>d__);
			return <GetResultAiEffect>d__.<>t__builder.Task;
		}

		// Token: 0x06000172 RID: 370 RVA: 0x00008130 File Offset: 0x00006330
		public Task<AiImageMotionOutput> GetResultAiMotion(string id)
		{
			VariationMedia.<GetResultAiMotion>d__166 <GetResultAiMotion>d__;
			<GetResultAiMotion>d__.<>t__builder = AsyncTaskMethodBuilder<AiImageMotionOutput>.Create();
			<GetResultAiMotion>d__.<>4__this = this;
			<GetResultAiMotion>d__.id = id;
			<GetResultAiMotion>d__.<>1__state = -1;
			<GetResultAiMotion>d__.<>t__builder.Start<VariationMedia.<GetResultAiMotion>d__166>(ref <GetResultAiMotion>d__);
			return <GetResultAiMotion>d__.<>t__builder.Task;
		}

		// Token: 0x06000173 RID: 371 RVA: 0x0000817C File Offset: 0x0000637C
		private void SetResultParameters(VariationMediaHelper.ImagineResultResponse imagineResultResponse)
		{
			this.MidJourneyResultPhotoUrl = imagineResultResponse.ProcessedMidjourneyTask.PhotoUrl;
			this.MidJourneyResultSeperatedPhotosUrls = new List<string>
			{
				imagineResultResponse.ProcessedMidjourneyTask.U1Url,
				imagineResultResponse.ProcessedMidjourneyTask.U2Url,
				imagineResultResponse.ProcessedMidjourneyTask.U3Url,
				imagineResultResponse.ProcessedMidjourneyTask.U4Url
			};
		}

		// Token: 0x06000174 RID: 372 RVA: 0x000081EC File Offset: 0x000063EC
		public void StartProcess(int startWaitMillis = 0)
		{
			if ((DateTime.Now - this.processWatchdog).TotalSeconds > 60.0)
			{
				this.ConvertRunning = false;
			}
			if (!this.ConvertRunning)
			{
				try
				{
					Timer timer = this.timer;
					if (timer != null)
					{
						timer.Change(-1, -1);
					}
					Timer timer2 = this.timer;
					if (timer2 != null)
					{
						timer2.Dispose();
					}
				}
				catch (Exception)
				{
				}
				this.StartTime = DateTime.Now;
				this.ConvertAIImageHarikalar(null);
			}
		}

		// Token: 0x06000175 RID: 373 RVA: 0x00008278 File Offset: 0x00006478
		public void CancelImagineJob(string title, string message)
		{
			try
			{
				Timer timer = this.timer;
				if (timer != null)
				{
					timer.Change(-1, -1);
				}
				Timer timer2 = this.timer;
				if (timer2 != null)
				{
					timer2.Dispose();
				}
			}
			catch (Exception)
			{
			}
			this.SetStatus("Canceled!");
			this.CancelThisOperation(title, message);
			if (VariationMedia.ImagineJobCanceled != null)
			{
				VariationMedia.ImagineJobCanceled(this.OriginalMediaHash);
			}
		}

		// Token: 0x17000050 RID: 80
		// (get) Token: 0x06000176 RID: 374 RVA: 0x000082EC File Offset: 0x000064EC
		public bool IsUploaded
		{
			get
			{
				return !string.IsNullOrEmpty(this.ResultUrl);
			}
		}

		// Token: 0x17000051 RID: 81
		// (get) Token: 0x06000177 RID: 375 RVA: 0x000082FC File Offset: 0x000064FC
		// (set) Token: 0x06000178 RID: 376 RVA: 0x00008304 File Offset: 0x00006504
		public string CurrentStatus { get; private set; }

		// Token: 0x06000179 RID: 377 RVA: 0x00008310 File Offset: 0x00006510
		private void ConvertAIImageHarikalar(object obj)
		{
			if (this.midJourneyProcess == MediaVariationModals.MidJourneyProcess.Completed)
			{
				Timer timer = this.timer;
				if (timer != null)
				{
					timer.Change(-1, -1);
				}
				Timer timer2 = this.timer;
				if (timer2 != null)
				{
					timer2.Dispose();
				}
			}
			if (this.ConvertRunning)
			{
				return;
			}
			new Thread(delegate()
			{
				VariationMedia.<<ConvertAIImageHarikalar>b__181_0>d <<ConvertAIImageHarikalar>b__181_0>d;
				<<ConvertAIImageHarikalar>b__181_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<ConvertAIImageHarikalar>b__181_0>d.<>4__this = this;
				<<ConvertAIImageHarikalar>b__181_0>d.<>1__state = -1;
				<<ConvertAIImageHarikalar>b__181_0>d.<>t__builder.Start<VariationMedia.<<ConvertAIImageHarikalar>b__181_0>d>(ref <<ConvertAIImageHarikalar>b__181_0>d);
			}).Start();
		}

		// Token: 0x0600017A RID: 378 RVA: 0x0000836C File Offset: 0x0000656C
		private Task OldAIEffect()
		{
			VariationMedia.<OldAIEffect>d__182 <OldAIEffect>d__;
			<OldAIEffect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<OldAIEffect>d__.<>4__this = this;
			<OldAIEffect>d__.<>1__state = -1;
			<OldAIEffect>d__.<>t__builder.Start<VariationMedia.<OldAIEffect>d__182>(ref <OldAIEffect>d__);
			return <OldAIEffect>d__.<>t__builder.Task;
		}

		// Token: 0x0600017B RID: 379 RVA: 0x000083B0 File Offset: 0x000065B0
		private Task<AIBeautifierModals.AIBeautifierResponse> ApplyAIBeautifier()
		{
			VariationMedia.<ApplyAIBeautifier>d__183 <ApplyAIBeautifier>d__;
			<ApplyAIBeautifier>d__.<>t__builder = AsyncTaskMethodBuilder<AIBeautifierModals.AIBeautifierResponse>.Create();
			<ApplyAIBeautifier>d__.<>4__this = this;
			<ApplyAIBeautifier>d__.<>1__state = -1;
			<ApplyAIBeautifier>d__.<>t__builder.Start<VariationMedia.<ApplyAIBeautifier>d__183>(ref <ApplyAIBeautifier>d__);
			return <ApplyAIBeautifier>d__.<>t__builder.Task;
		}

		// Token: 0x0600017C RID: 380 RVA: 0x000083F4 File Offset: 0x000065F4
		private Task<GenericResponse> PrepareResult(MediaType mediaType)
		{
			VariationMedia.<PrepareResult>d__184 <PrepareResult>d__;
			<PrepareResult>d__.<>t__builder = AsyncTaskMethodBuilder<GenericResponse>.Create();
			<PrepareResult>d__.<>4__this = this;
			<PrepareResult>d__.mediaType = mediaType;
			<PrepareResult>d__.<>1__state = -1;
			<PrepareResult>d__.<>t__builder.Start<VariationMedia.<PrepareResult>d__184>(ref <PrepareResult>d__);
			return <PrepareResult>d__.<>t__builder.Task;
		}

		// Token: 0x0600017D RID: 381 RVA: 0x00008440 File Offset: 0x00006640
		private GenericResponse DownloadVariationRawFile()
		{
			this.SetStatus("Downloading file...");
			byte[] data = null;
			do
			{
				try
				{
					using (WebClient client = new WebClient())
					{
						data = client.DownloadData(this.VariationRawUrl);
					}
				}
				catch (Exception ex)
				{
				}
			}
			while (data == null);
			try
			{
				if (this.ResultMediaType == MediaType.photo)
				{
					using (MagickImage mImage = new MagickImage(data))
					{
						mImage.Format = MagickFormat.Jpg;
						this.VariationRawFileInfo = new FileInformation
						{
							Filename = Path.GetFileName(this.VariationRawLocalPath),
							Directory = Path.GetDirectoryName(this.VariationRawLocalPath),
							Filesize = (long)mImage.ToByteArray().Length,
							Height = (int)mImage.Height,
							Width = (int)mImage.Width
						};
						mImage.Write(this.VariationRawLocalPath);
						goto IL_119;
					}
				}
				if (this.ResultMediaType == MediaType.video)
				{
					this.VariationRawFileInfo = new FileInformation
					{
						Filename = Path.GetFileName(this.VariationRawLocalPath),
						Directory = Path.GetDirectoryName(this.VariationRawLocalPath),
						Filesize = 0L,
						Height = TemplateClass.PaperHeightPhoto,
						Width = TemplateClass.PaperWidthPhoto
					};
					File.WriteAllBytes(this.VariationRawLocalPath, data);
				}
				IL_119:;
			}
			catch (Exception ex2)
			{
				return new GenericResponse(false, ex2.Message);
			}
			return new GenericResponse(true);
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000085B8 File Offset: 0x000067B8
		public void SetStatus(string status)
		{
			this.CurrentStatus = status;
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (VariationMedia.StatusUpdate != null)
				{
					VariationMedia.StatusUpdate(status, this.OriginalMediaHash);
				}
			}), Array.Empty<object>());
		}

		// Token: 0x0600017F RID: 383 RVA: 0x00008608 File Offset: 0x00006808
		private Task MidJourneyImagineCommand()
		{
			VariationMedia.<MidJourneyImagineCommand>d__187 <MidJourneyImagineCommand>d__;
			<MidJourneyImagineCommand>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<MidJourneyImagineCommand>d__.<>4__this = this;
			<MidJourneyImagineCommand>d__.<>1__state = -1;
			<MidJourneyImagineCommand>d__.<>t__builder.Start<VariationMedia.<MidJourneyImagineCommand>d__187>(ref <MidJourneyImagineCommand>d__);
			return <MidJourneyImagineCommand>d__.<>t__builder.Task;
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0000864B File Offset: 0x0000684B
		private void RunAiCreditsLink(MessageBoxWindow.MessageBoxReturn btnReturn)
		{
			if (btnReturn == MessageBoxWindow.MessageBoxReturn.Yes)
			{
				Process.Start(AppInfo.AppClass.CreditLink);
			}
		}

		// Token: 0x06000181 RID: 385 RVA: 0x00008661 File Offset: 0x00006861
		private void CancelThisOperation(string title, string message)
		{
			MessageBoxWindow.CreateWindow(title, message, new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Continue
			}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
			VariationMediaHelper.VariationMediasList.Remove(this);
			ExtensionMethod.CreateWriteJson<List<VariationMedia>>(VariationMediaHelper.VariationMediasList, VariationMediaHelper.GetDataFilePath());
			this.midJourneyProcess = MediaVariationModals.MidJourneyProcess.Completed;
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0000869C File Offset: 0x0000689C
		private static Task<FaceSwapTarget> GenerateFaceSwapTarget(MagickImage mImage, FileInformation fileInformation)
		{
			VariationMedia.<GenerateFaceSwapTarget>d__190 <GenerateFaceSwapTarget>d__;
			<GenerateFaceSwapTarget>d__.<>t__builder = AsyncTaskMethodBuilder<FaceSwapTarget>.Create();
			<GenerateFaceSwapTarget>d__.mImage = mImage;
			<GenerateFaceSwapTarget>d__.fileInformation = fileInformation;
			<GenerateFaceSwapTarget>d__.<>1__state = -1;
			<GenerateFaceSwapTarget>d__.<>t__builder.Start<VariationMedia.<GenerateFaceSwapTarget>d__190>(ref <GenerateFaceSwapTarget>d__);
			return <GenerateFaceSwapTarget>d__.<>t__builder.Task;
		}

		// Token: 0x040000D9 RID: 217
		[JsonProperty("variationOperation")]
		public VariationType VariationOperation;

		// Token: 0x040000E3 RID: 227
		[JsonProperty("thumbnailLocalPath")]
		private string thumbnailLocalPath;

		// Token: 0x040000E4 RID: 228
		[JsonProperty("localId")]
		private string localId;

		// Token: 0x040000E6 RID: 230
		[JsonProperty("index")]
		public int index;

		// Token: 0x040000F1 RID: 241
		[JsonProperty("selectedResultIndex")]
		public int selectedResultIndex = -1;

		// Token: 0x040000F5 RID: 245
		[JsonIgnore]
		private MediaVariationModals.MidJourneyProcess _midJourneyProcess;

		// Token: 0x040000F8 RID: 248
		[JsonProperty("variationDirectoryPath")]
		private string variationDirectoryPath;

		// Token: 0x040000F9 RID: 249
		[JsonProperty("currentCDNUploadCount")]
		public static int CurrentCDNUploadCount = 0;

		// Token: 0x040000FB RID: 251
		private DiscordMessageModals.DiscordMessage imageMessage = new DiscordMessageModals.DiscordMessage();

		// Token: 0x04000102 RID: 258
		public static Action<VariationMedia> VariationProcessCompleted;

		// Token: 0x04000103 RID: 259
		public static double AiPromptEstimatedCompleteTime = 50.0;

		// Token: 0x04000104 RID: 260
		public static double FaceSwapEstimatedCompleteTime = 30.0;

		// Token: 0x04000105 RID: 261
		public static double WordPortraitEstimatedCompleteTime = 60.0;

		// Token: 0x04000106 RID: 262
		public static double AiMotionEstimatedCompleteTime = 600.0;

		// Token: 0x04000107 RID: 263
		public static double AiEffectEstimatedCompleteTime = 90.0;

		// Token: 0x04000108 RID: 264
		public static double AIBeautifierEstimatedCompleteTime = 30.0;

		// Token: 0x04000109 RID: 265
		private Timer timer;

		// Token: 0x0400010A RID: 266
		private DateTime processWatchdog = DateTime.MinValue;

		// Token: 0x0400010B RID: 267
		private CancellationToken stoppingToken;

		// Token: 0x0400010C RID: 268
		private bool ConvertRunning;

		// Token: 0x0400010E RID: 270
		private static List<string> lastAiMotionId = new List<string>();
	}
}
