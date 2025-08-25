using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000040 RID: 64
	public partial class AIEffectPage : SettingsSubPage
	{
		// Token: 0x06000489 RID: 1161 RVA: 0x00019BF8 File Offset: 0x00017DF8
		public AIEffectPage()
		{
			this.InitializeComponent();
			AIEffectPrefab.EnabledCheck += this.AIEffectPrefab_EnabledCheck;
			AIEffectPrefab.EnabledUnCheck += this.AIEffectPrefab_EnabledUnCheck;
		}

		// Token: 0x0600048A RID: 1162 RVA: 0x00019C5C File Offset: 0x00017E5C
		public static Task<List<AiEffectLocal>> GetAIEffectsLocals()
		{
			AIEffectPage.<GetAIEffectsLocals>d__3 <GetAIEffectsLocals>d__;
			<GetAIEffectsLocals>d__.<>t__builder = AsyncTaskMethodBuilder<List<AiEffectLocal>>.Create();
			<GetAIEffectsLocals>d__.<>1__state = -1;
			<GetAIEffectsLocals>d__.<>t__builder.Start<AIEffectPage.<GetAIEffectsLocals>d__3>(ref <GetAIEffectsLocals>d__);
			return <GetAIEffectsLocals>d__.<>t__builder.Task;
		}

		// Token: 0x17000076 RID: 118
		// (get) Token: 0x0600048B RID: 1163 RVA: 0x00019C98 File Offset: 0x00017E98
		public static string AIEffectFolderPath
		{
			get
			{
				string path = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "AIEffectTargets");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00019CCC File Offset: 0x00017ECC
		public static string AIEffectFolderPathByEvent(OperationalEvent operationalEvent)
		{
			string path = Path.Combine(operationalEvent.DirectoryPath, "AIEffectTargets");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		// Token: 0x17000077 RID: 119
		// (get) Token: 0x0600048D RID: 1165 RVA: 0x00019CFC File Offset: 0x00017EFC
		public static string AIEffectJsonPath
		{
			get
			{
				return Path.Combine(AIEffectPage.AIEffectFolderPath, "AIEffectTargets.json");
			}
		}

		// Token: 0x17000078 RID: 120
		// (get) Token: 0x0600048E RID: 1166 RVA: 0x00019D1C File Offset: 0x00017F1C
		public static string imageFolderPath
		{
			get
			{
				string path = Path.Combine(AIEffectPage.AIEffectFolderPath, "Images");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00019D49 File Offset: 0x00017F49
		public override string GetTitle()
		{
			return "AI Effect Gallery";
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00019D50 File Offset: 0x00017F50
		public override string GetSubTitle()
		{
			return "You can open or close AI Effect gallery in here.";
		}

		// Token: 0x06000491 RID: 1169 RVA: 0x00019D58 File Offset: 0x00017F58
		private void AIEffectPrefab_EnabledUnCheck(string id)
		{
			AiEffectLocal local = (from x in this.AllAIEffects
			where x.Id == id
			select x).FirstOrDefault<AiEffectLocal>();
			local.IsEnabled = false;
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00019D98 File Offset: 0x00017F98
		private void AIEffectPrefab_EnabledCheck(string id)
		{
			AiEffectLocal local = (from x in this.AllAIEffects
			where x.Id == id
			select x).FirstOrDefault<AiEffectLocal>();
			local.IsEnabled = true;
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00019DD8 File Offset: 0x00017FD8
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AIEffectPage.<Page_Loaded>d__23 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<AIEffectPage.<Page_Loaded>d__23>(ref <Page_Loaded>d__);
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x00019E0F File Offset: 0x0001800F
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.cancellationTokenSource != null)
			{
				this.cancellationTokenSource.Cancel();
			}
			this.ImagePickerPrefabList.Clear();
			this.AIEffectPrefabStackPanel.Children.Clear();
			ExtensionMethod.CreateWriteJson<List<AiEffectLocal>>(this.AllAIEffects, AIEffectPage.AIEffectJsonPath);
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x00019E50 File Offset: 0x00018050
		private Task<AiEffectLocal> ImageControl(AiEffectLocal aiEffectLocal)
		{
			AIEffectPage.<ImageControl>d__25 <ImageControl>d__;
			<ImageControl>d__.<>t__builder = AsyncTaskMethodBuilder<AiEffectLocal>.Create();
			<ImageControl>d__.aiEffectLocal = aiEffectLocal;
			<ImageControl>d__.<>1__state = -1;
			<ImageControl>d__.<>t__builder.Start<AIEffectPage.<ImageControl>d__25>(ref <ImageControl>d__);
			return <ImageControl>d__.<>t__builder.Task;
		}

		// Token: 0x06000496 RID: 1174 RVA: 0x00019E94 File Offset: 0x00018094
		private Task AddPrefabAsyncInitialization(AiEffectLocal aiEffectLocal)
		{
			AIEffectPage.<AddPrefabAsyncInitialization>d__26 <AddPrefabAsyncInitialization>d__;
			<AddPrefabAsyncInitialization>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddPrefabAsyncInitialization>d__.<>4__this = this;
			<AddPrefabAsyncInitialization>d__.aiEffectLocal = aiEffectLocal;
			<AddPrefabAsyncInitialization>d__.<>1__state = -1;
			<AddPrefabAsyncInitialization>d__.<>t__builder.Start<AIEffectPage.<AddPrefabAsyncInitialization>d__26>(ref <AddPrefabAsyncInitialization>d__);
			return <AddPrefabAsyncInitialization>d__.<>t__builder.Task;
		}

		// Token: 0x06000497 RID: 1175 RVA: 0x00019EE0 File Offset: 0x000180E0
		private Task<AiEffectLocal> DownloadAndSaveImage(AiEffectLocal aiEffect)
		{
			AIEffectPage.<DownloadAndSaveImage>d__28 <DownloadAndSaveImage>d__;
			<DownloadAndSaveImage>d__.<>t__builder = AsyncTaskMethodBuilder<AiEffectLocal>.Create();
			<DownloadAndSaveImage>d__.aiEffect = aiEffect;
			<DownloadAndSaveImage>d__.<>1__state = -1;
			<DownloadAndSaveImage>d__.<>t__builder.Start<AIEffectPage.<DownloadAndSaveImage>d__28>(ref <DownloadAndSaveImage>d__);
			return <DownloadAndSaveImage>d__.<>t__builder.Task;
		}

		// Token: 0x06000498 RID: 1176 RVA: 0x00019F24 File Offset: 0x00018124
		private Task<List<AiEffectLocal>> ControlList(List<AiEffectLocal> ApiList, List<AiEffectLocal> ReadList)
		{
			AIEffectPage.<ControlList>d__29 <ControlList>d__;
			<ControlList>d__.<>t__builder = AsyncTaskMethodBuilder<List<AiEffectLocal>>.Create();
			<ControlList>d__.<>4__this = this;
			<ControlList>d__.ApiList = ApiList;
			<ControlList>d__.ReadList = ReadList;
			<ControlList>d__.<>1__state = -1;
			<ControlList>d__.<>t__builder.Start<AIEffectPage.<ControlList>d__29>(ref <ControlList>d__);
			return <ControlList>d__.<>t__builder.Task;
		}

		// Token: 0x06000499 RID: 1177 RVA: 0x00019F78 File Offset: 0x00018178
		private Task GetAndControlAIEffectJson()
		{
			AIEffectPage.<GetAndControlAIEffectJson>d__30 <GetAndControlAIEffectJson>d__;
			<GetAndControlAIEffectJson>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GetAndControlAIEffectJson>d__.<>4__this = this;
			<GetAndControlAIEffectJson>d__.<>1__state = -1;
			<GetAndControlAIEffectJson>d__.<>t__builder.Start<AIEffectPage.<GetAndControlAIEffectJson>d__30>(ref <GetAndControlAIEffectJson>d__);
			return <GetAndControlAIEffectJson>d__.<>t__builder.Task;
		}

		// Token: 0x0600049A RID: 1178 RVA: 0x00019FBC File Offset: 0x000181BC
		private Task StartJsonProcessAndAddPrefabs()
		{
			AIEffectPage.<StartJsonProcessAndAddPrefabs>d__31 <StartJsonProcessAndAddPrefabs>d__;
			<StartJsonProcessAndAddPrefabs>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartJsonProcessAndAddPrefabs>d__.<>4__this = this;
			<StartJsonProcessAndAddPrefabs>d__.<>1__state = -1;
			<StartJsonProcessAndAddPrefabs>d__.<>t__builder.Start<AIEffectPage.<StartJsonProcessAndAddPrefabs>d__31>(ref <StartJsonProcessAndAddPrefabs>d__);
			return <StartJsonProcessAndAddPrefabs>d__.<>t__builder.Task;
		}

		// Token: 0x04000492 RID: 1170
		private string json = "\r\n[\r\n  {\r\n    \"Id\": \"66164e9b1f1c2e3b7c123abc\",\r\n    \"CoverImageUrl\": \"https://imgv3.fotor.com/images/share/wonderland-girl-generated-by-Fotor-ai-art-generator.jpg\",\r\n    \"Title\": \"Cyberpunk Street\",\r\n    \"Prompt\": \"A futuristic street with neon lights\",\r\n    \"IsNew\": true,\r\n    \"IsFaceSwapActive\": false,\r\n    \"IsPublished\": true,\r\n    \"GeneratedTime\": \"2025-04-10T10:00:00\",\r\n    \"ChangeTime\": \"2025-04-10T10:00:01\",\r\n    \"DeletedTime\": \"2025-04-10T10:00:00\",\r\n    \"IsDeleted\": false\r\n  },\r\n  {\r\n    \"Id\": \"66164e9b1f1c2e3b7c456def\",\r\n    \"CoverImageUrl\": \"https://www.img2go.com/assets/img/ai_images/how_to_section.jpg\",\r\n    \"Title\": \"Fantasy Castle\",\r\n    \"Prompt\": \"A majestic castle floating in the sky\",\r\n    \"IsNew\": false,\r\n    \"IsFaceSwapActive\": true,\r\n    \"IsPublished\": true,\r\n    \"GeneratedTime\": \"2025-04-10T10:01:00\",\r\n    \"ChangeTime\": \"2025-04-10T10:01:01\",\r\n    \"DeletedTime\": \"2025-04-10T10:01:00\",\r\n    \"IsDeleted\": false\r\n  },\r\n  {\r\n    \"Id\": \"66164e9b1f1c2e3b7c789ghi\",\r\n    \"CoverImageUrl\": \"https://img.freepik.com/premium-photo/beautiful-girl_634423-5324.jpg\",\r\n    \"Title\": \"Retro Sci-Fi\",\r\n    \"Prompt\": \"A 1980s style science fiction poster\",\r\n    \"IsNew\": true,\r\n    \"IsFaceSwapActive\": false,\r\n    \"IsPublished\": false,\r\n    \"GeneratedTime\": \"2025-04-10T10:02:00\",\r\n    \"ChangeTime\": \"2025-04-10T10:02:01\",\r\n    \"DeletedTime\": \"2025-04-10T10:02:00\",\r\n    \"IsDeleted\": false\r\n  }\r\n]\r\n";

		// Token: 0x04000493 RID: 1171
		private const string AIEffectFolderName = "AIEffectTargets";

		// Token: 0x04000494 RID: 1172
		private const string AIEffectJsonName = "AIEffectTargets.json";

		// Token: 0x04000495 RID: 1173
		private const string imageFolderName = "Images";

		// Token: 0x04000496 RID: 1174
		private List<AIEffectPrefab> ImagePickerPrefabList = new List<AIEffectPrefab>();

		// Token: 0x04000497 RID: 1175
		private List<AiEffectLocal> AllAIEffects = new List<AiEffectLocal>();

		// Token: 0x04000498 RID: 1176
		private bool isFirstWork = true;

		// Token: 0x04000499 RID: 1177
		private int prefabCount;

		// Token: 0x0400049A RID: 1178
		private int imgCount;

		// Token: 0x0400049B RID: 1179
		private CancellationTokenSource cancellationTokenSource;

		// Token: 0x0400049C RID: 1180
		private bool ResetWorking;
	}
}
