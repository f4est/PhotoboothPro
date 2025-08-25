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
using System.Xml.Serialization;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000043 RID: 67
	public partial class SamplePromptsPage : SettingsSubPage
	{
		// Token: 0x1700007D RID: 125
		// (get) Token: 0x060004C8 RID: 1224 RVA: 0x0001AE40 File Offset: 0x00019040
		public static SamplePromptData SamplePromptData
		{
			get
			{
				if (File.Exists(SamplePromptsPage.samplePromptsXMLPath))
				{
					SamplePromptsPage.samplePromptData = SamplePromptsPage.LoadAndRemoveOldXMLData(SamplePromptsPage.samplePromptData);
				}
				if (SamplePromptsPage.samplePromptData == null || SamplePromptsPage.samplePromptData.SamplePrompts.Count == 0)
				{
					SamplePromptsPage.samplePromptData = ExtensionMethod.ReadJson<SamplePromptData>(SamplePromptsPage.samplePromptsJsonPath);
				}
				return SamplePromptsPage.samplePromptData;
			}
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x0001AE94 File Offset: 0x00019094
		private static SamplePromptDataXML DeserializeFromXml(string filePath)
		{
			SamplePromptDataXML data = null;
			XmlSerializer serializer = new XmlSerializer(typeof(SamplePromptDataXML));
			using (StreamReader reader = new StreamReader(filePath))
			{
				data = (SamplePromptDataXML)serializer.Deserialize(reader);
			}
			return data;
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x0001AEE4 File Offset: 0x000190E4
		private static SamplePromptData LoadAndRemoveOldXMLData(SamplePromptData promptData)
		{
			bool dataEmpty = true;
			try
			{
				SamplePromptDataXML xmlsamplePromptData = SamplePromptsPage.DeserializeFromXml(SamplePromptsPage.samplePromptsXMLPath);
				if (xmlsamplePromptData == null)
				{
					File.Delete(SamplePromptsPage.samplePromptsXMLPath);
					return promptData;
				}
				if (File.Exists(SamplePromptsPage.samplePromptsJsonPath))
				{
					promptData = ExtensionMethod.ReadJson<SamplePromptData>(SamplePromptsPage.samplePromptsJsonPath);
					dataEmpty = false;
				}
				if (promptData == null || promptData.SamplePrompts == null)
				{
					promptData = new SamplePromptData
					{
						SamplePrompts = new List<SamplePrompt>()
					};
				}
				int i = 0;
				Predicate<SamplePrompt> <>9__0;
				while (i < xmlsamplePromptData.SamplePrompts.Count)
				{
					if (dataEmpty)
					{
						goto IL_A6;
					}
					List<SamplePrompt> samplePrompts = promptData.SamplePrompts;
					Predicate<SamplePrompt> match;
					if ((match = <>9__0) == null)
					{
						match = (<>9__0 = ((SamplePrompt x) => x.Prompts[0] == xmlsamplePromptData.SamplePrompts[i].Prompts[0]));
					}
					if (!samplePrompts.Exists(match))
					{
						goto IL_A6;
					}
					IL_1F1:
					int j = i;
					i = j + 1;
					continue;
					IL_A6:
					promptData.SamplePrompts.Add(new SamplePrompt
					{
						FileDetails = new FileInformation
						{
							Filename = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "SamplePrompts", xmlsamplePromptData.ImageFolder, xmlsamplePromptData.SamplePrompts[i].FileDetails.Filename),
							Directory = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "SamplePrompts", xmlsamplePromptData.ImageFolder),
							Width = xmlsamplePromptData.SamplePrompts[i].FileDetails.Width,
							Height = xmlsamplePromptData.SamplePrompts[i].FileDetails.Height,
							Filesize = (long)xmlsamplePromptData.SamplePrompts[i].FileDetails.Filesize
						},
						Index = promptData.SamplePrompts.Count,
						Prompts = xmlsamplePromptData.SamplePrompts[i].Prompts,
						Title = xmlsamplePromptData.SamplePrompts[i].Title
					});
					goto IL_1F1;
				}
				File.Delete(SamplePromptsPage.samplePromptsXMLPath);
			}
			catch (Exception ex)
			{
				MessageBoxWindow.CreateWindow("AI Prompts File Broken", "Please Return Settings Menu and recreate AIPrompts.", null, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				File.Delete(SamplePromptsPage.samplePromptsXMLPath);
			}
			ExtensionMethod.CreateWriteJson<SamplePromptData>(promptData, SamplePromptsPage.samplePromptsJsonPath);
			return promptData;
		}

		// Token: 0x1700007E RID: 126
		// (get) Token: 0x060004CB RID: 1227 RVA: 0x0001B168 File Offset: 0x00019368
		public static string samplePromptsFolderPath
		{
			get
			{
				string path = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "SamplePrompts");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x0001B19C File Offset: 0x0001939C
		public static string samplePromptsFolderPathByEvent(OperationalEvent operationalEvent)
		{
			string path = Path.Combine(operationalEvent.DirectoryPath, "SamplePrompts");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		// Token: 0x1700007F RID: 127
		// (get) Token: 0x060004CD RID: 1229 RVA: 0x0001B1CC File Offset: 0x000193CC
		public static string samplePromptsJsonPath
		{
			get
			{
				return Path.Combine(SamplePromptsPage.samplePromptsFolderPath, "SamplePromptData.json");
			}
		}

		// Token: 0x17000080 RID: 128
		// (get) Token: 0x060004CE RID: 1230 RVA: 0x0001B1EC File Offset: 0x000193EC
		public static string samplePromptsXMLPath
		{
			get
			{
				return Path.Combine(SamplePromptsPage.samplePromptsFolderPath, "SamplePromptData.xml");
			}
		}

		// Token: 0x17000081 RID: 129
		// (get) Token: 0x060004CF RID: 1231 RVA: 0x0001B20C File Offset: 0x0001940C
		public static string imageFolderPath
		{
			get
			{
				string path = Path.Combine(SamplePromptsPage.samplePromptsFolderPath, "SamplePromptImages");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x0001B23C File Offset: 0x0001943C
		public SamplePromptsPage()
		{
			this.InitializeComponent();
			SamplePromptPickerPrefab.imageLoaded += this.SamplePromptPickerPrefab_imageLoaded;
			SamplePromptPickerPrefab.deleteClicked += this.DeleteSamplePromptPrefab;
			SamplePromptPickerPrefab.newSamplePromptAdded += this.SamplePromptPickerPrefab_newSamplePromptAdded;
			SamplePromptPickerPrefab.titleChanged += this.SamplePromptPickerPrefab_titleChanged;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x0001B2AB File Offset: 0x000194AB
		private void SamplePromptPickerPrefab_titleChanged(string title, int index)
		{
			if (SamplePromptsPage.samplePromptData != null && SamplePromptsPage.samplePromptData.SamplePrompts.Count > index)
			{
				SamplePromptsPage.samplePromptData.SamplePrompts[index].Title = title;
			}
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x0001B2DC File Offset: 0x000194DC
		private void SamplePromptPickerPrefab_newSamplePromptAdded()
		{
			Application.Current.Dispatcher.Invoke(delegate()
			{
				List<SamplePromptPickerPrefab> currentEmptyPrefabs = (from x in this.ImagePickerPrefabList
				where x.prefabImage.Source == null
				select x).ToList<SamplePromptPickerPrefab>();
				if (currentEmptyPrefabs.Count >= 1)
				{
					this.AddEmptyPrefabAsync(null, null);
				}
			});
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x0001B2F9 File Offset: 0x000194F9
		public override string GetTitle()
		{
			return "AI Prompt Gallery";
		}

		// Token: 0x060004D4 RID: 1236 RVA: 0x0001B300 File Offset: 0x00019500
		public override string GetSubTitle()
		{
			return "You can create, delete or edit your prompt gallery in here.";
		}

		// Token: 0x060004D5 RID: 1237 RVA: 0x0001B308 File Offset: 0x00019508
		public static SamplePromptData LoadSamplePromptData(string filePath)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					MessageBoxWindow.CreateWindow("SamplePrompts Error", "JSON dosyası bulunamadı -> " + filePath, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("Error_{0:yyyy_MM_dd_HH_mm_ss_ffff}.txt", DateTime.Now)), "Örnek bir JSON dosyası oluşturun veya kontrol edin -> " + filePath);
					return null;
				}
				return ExtensionMethod.ReadJson<SamplePromptData>(filePath);
			}
			catch (IOException)
			{
				MessageBoxWindow.CreateWindow("SamplePrompts Error", "Dosya erişimi sırasında bir I/O hatası oluştu -> " + filePath, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			catch (Exception ex)
			{
				MessageBoxWindow.CreateWindow("SamplePrompts Error", ex.Message, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			return null;
		}

		// Token: 0x060004D6 RID: 1238 RVA: 0x0001B3F0 File Offset: 0x000195F0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			SamplePromptsPage.<Page_Loaded>d__29 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<SamplePromptsPage.<Page_Loaded>d__29>(ref <Page_Loaded>d__);
		}

		// Token: 0x060004D7 RID: 1239 RVA: 0x0001B427 File Offset: 0x00019627
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			ExtensionMethod.CreateWriteJson<SamplePromptData>(SamplePromptsPage.samplePromptData, SamplePromptsPage.samplePromptsJsonPath);
			if (this.cancellationTokenSource != null)
			{
				this.cancellationTokenSource.Cancel();
			}
			this.ImagePickerPrefabList.Clear();
			this.SamplePromptPrefabStackPanel.Children.Clear();
		}

		// Token: 0x060004D8 RID: 1240 RVA: 0x0001B468 File Offset: 0x00019668
		public void SamplePromptPickerPrefab_imageLoaded(SamplePromptPickerPrefab samplePromptPickerPrefab)
		{
			if (samplePromptPickerPrefab.index > SamplePromptsPage.samplePromptData.SamplePrompts.Count - 1)
			{
				SamplePromptsPage.samplePromptData.SamplePrompts.Add(samplePromptPickerPrefab.SamplePrompt);
				if (!this.isFirstWork)
				{
					this.AddEmptyPrefabAsync(null, null);
				}
				if (!this.ImagePickerPrefabList.Exists((SamplePromptPickerPrefab x) => x.prefabImage.Source == null))
				{
					this.AddEmptyPrefabAsync(null, null);
				}
			}
			else
			{
				SamplePromptsPage.samplePromptData.SamplePrompts[samplePromptPickerPrefab.index] = samplePromptPickerPrefab.SamplePrompt;
			}
			Application.Current.Dispatcher.Invoke(delegate()
			{
				ExtensionMethod.CreateWriteJson<SamplePromptData>(SamplePromptsPage.samplePromptData, SamplePromptsPage.samplePromptsJsonPath);
			});
			if (this.isFirstWork)
			{
				this.scrollPanel.ScrollToEnd();
			}
		}

		// Token: 0x060004D9 RID: 1241 RVA: 0x0001B548 File Offset: 0x00019748
		private Task AddEmptyPrefabAsync(object sender, RoutedEventArgs e)
		{
			SamplePromptsPage.<AddEmptyPrefabAsync>d__32 <AddEmptyPrefabAsync>d__;
			<AddEmptyPrefabAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddEmptyPrefabAsync>d__.<>4__this = this;
			<AddEmptyPrefabAsync>d__.<>1__state = -1;
			<AddEmptyPrefabAsync>d__.<>t__builder.Start<SamplePromptsPage.<AddEmptyPrefabAsync>d__32>(ref <AddEmptyPrefabAsync>d__);
			return <AddEmptyPrefabAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060004DA RID: 1242 RVA: 0x0001B58C File Offset: 0x0001978C
		private Task AddPrefabAsyncInitialization(bool Empty = false)
		{
			SamplePromptsPage.<AddPrefabAsyncInitialization>d__33 <AddPrefabAsyncInitialization>d__;
			<AddPrefabAsyncInitialization>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddPrefabAsyncInitialization>d__.<>4__this = this;
			<AddPrefabAsyncInitialization>d__.Empty = Empty;
			<AddPrefabAsyncInitialization>d__.<>1__state = -1;
			<AddPrefabAsyncInitialization>d__.<>t__builder.Start<SamplePromptsPage.<AddPrefabAsyncInitialization>d__33>(ref <AddPrefabAsyncInitialization>d__);
			return <AddPrefabAsyncInitialization>d__.<>t__builder.Task;
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x0001B5D8 File Offset: 0x000197D8
		private Task StartXMLProcessAndAddPrefabs()
		{
			SamplePromptsPage.<StartXMLProcessAndAddPrefabs>d__34 <StartXMLProcessAndAddPrefabs>d__;
			<StartXMLProcessAndAddPrefabs>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartXMLProcessAndAddPrefabs>d__.<>4__this = this;
			<StartXMLProcessAndAddPrefabs>d__.<>1__state = -1;
			<StartXMLProcessAndAddPrefabs>d__.<>t__builder.Start<SamplePromptsPage.<StartXMLProcessAndAddPrefabs>d__34>(ref <StartXMLProcessAndAddPrefabs>d__);
			return <StartXMLProcessAndAddPrefabs>d__.<>t__builder.Task;
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0001B61C File Offset: 0x0001981C
		private void DeleteSamplePromptPrefab(SamplePromptPickerPrefab obj)
		{
			Func<Frame, bool> <>9__1;
			Application.Current.Dispatcher.Invoke(delegate()
			{
				IEnumerable<Frame> source = this.SamplePromptPrefabStackPanel.Children.OfType<Frame>();
				Func<Frame, bool> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((Frame x) => x.Content == obj));
				}
				Frame frameToRemove = source.FirstOrDefault(predicate);
				if (frameToRemove != null)
				{
					this.SamplePromptPrefabStackPanel.Children.Remove(frameToRemove);
				}
				int deletedIndex = obj.index;
				this.ImagePickerPrefabList.Remove(obj);
				if (SamplePromptsPage.samplePromptData != null && SamplePromptsPage.samplePromptData.SamplePrompts.Count > deletedIndex)
				{
					SamplePromptsPage.samplePromptData.SamplePrompts.RemoveAt(deletedIndex);
				}
				for (int i = deletedIndex; i < this.ImagePickerPrefabList.Count; i++)
				{
					this.ImagePickerPrefabList[i].index = i;
					if (SamplePromptsPage.samplePromptData != null && SamplePromptsPage.samplePromptData.SamplePrompts.Count > i)
					{
						SamplePromptsPage.samplePromptData.SamplePrompts[i].Index = i;
					}
				}
				foreach (SamplePromptPickerPrefab prefab in this.ImagePickerPrefabList)
				{
					prefab.Load();
				}
				this.imgCount--;
				this.prefabCount--;
				List<SamplePromptPickerPrefab> currentEmptyPrefabs = (from x in this.ImagePickerPrefabList
				where x.prefabImage.Source == null
				select x).ToList<SamplePromptPickerPrefab>();
				if (currentEmptyPrefabs.Count == 0)
				{
					this.AddEmptyPrefabAsync(null, null);
				}
				ExtensionMethod.CreateWriteJson<SamplePromptData>(SamplePromptsPage.samplePromptData, SamplePromptsPage.samplePromptsJsonPath);
			});
		}

		// Token: 0x040004CA RID: 1226
		private const string samplePromptsFolderName = "SamplePrompts";

		// Token: 0x040004CB RID: 1227
		public static SamplePromptData samplePromptData;

		// Token: 0x040004CC RID: 1228
		private const string samplePromptsJsonName = "SamplePromptData.json";

		// Token: 0x040004CD RID: 1229
		private const string samplePromptsXMLName = "SamplePromptData.xml";

		// Token: 0x040004CE RID: 1230
		private const string imageFolderName = "SamplePromptImages";

		// Token: 0x040004CF RID: 1231
		private List<SamplePromptPickerPrefab> ImagePickerPrefabList = new List<SamplePromptPickerPrefab>();

		// Token: 0x040004D0 RID: 1232
		private bool isFirstWork = true;

		// Token: 0x040004D1 RID: 1233
		private int prefabCount;

		// Token: 0x040004D2 RID: 1234
		private int imgCount;

		// Token: 0x040004D3 RID: 1235
		private CancellationTokenSource cancellationTokenSource;
	}
}
