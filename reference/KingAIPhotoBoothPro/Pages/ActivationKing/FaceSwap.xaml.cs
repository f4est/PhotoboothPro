using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000041 RID: 65
	public partial class FaceSwap : SettingsSubPage
	{
		// Token: 0x17000079 RID: 121
		// (get) Token: 0x0600049E RID: 1182 RVA: 0x0001A0C0 File Offset: 0x000182C0
		public static string faceSwapFolderPath
		{
			get
			{
				string path = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "FaceSwap");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x0600049F RID: 1183 RVA: 0x0001A0F4 File Offset: 0x000182F4
		public static bool IsLoadAllFaceSwap()
		{
			if (FaceSwap.faceSwapData == null || FaceSwap.faceSwapData.FaceSwapTargets == null)
			{
				FaceSwap.faceSwapData = ExtensionMethod.ReadJson<FaceSwapBackgroundGallery.FaceSwapData>(FaceSwap.faceSwapJSONPath);
			}
			if (FaceSwap.faceSwapData.FaceSwapTargets == null)
			{
				return false;
			}
			return !(from x in FaceSwap.faceSwapData.FaceSwapTargets
			where x.FaceDetails == null || string.IsNullOrEmpty(x.CdnUrl)
			select x).Any<FaceSwapTarget>();
		}

		// Token: 0x060004A0 RID: 1184 RVA: 0x0001A168 File Offset: 0x00018368
		public static string faceSwapFolderPathByEvent(OperationalEvent operationalEvent)
		{
			string path = Path.Combine(operationalEvent.DirectoryPath, "FaceSwap");
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			return path;
		}

		// Token: 0x1700007A RID: 122
		// (get) Token: 0x060004A1 RID: 1185 RVA: 0x0001A198 File Offset: 0x00018398
		public static string faceSwapJSONPath
		{
			get
			{
				return Path.Combine(FaceSwap.faceSwapFolderPath, "FaceSwapData.json");
			}
		}

		// Token: 0x1700007B RID: 123
		// (get) Token: 0x060004A2 RID: 1186 RVA: 0x0001A1B8 File Offset: 0x000183B8
		public static string imageFolderPath
		{
			get
			{
				string path = Path.Combine(FaceSwap.faceSwapFolderPath, "TargetImages");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x1700007C RID: 124
		// (get) Token: 0x060004A3 RID: 1187 RVA: 0x0001A1E5 File Offset: 0x000183E5
		// (set) Token: 0x060004A4 RID: 1188 RVA: 0x0001A1ED File Offset: 0x000183ED
		public bool FirstLoadingActive { get; private set; }

		// Token: 0x060004A5 RID: 1189 RVA: 0x0001A1F8 File Offset: 0x000183F8
		public FaceSwap()
		{
			this.InitializeComponent();
			FaceSwapImagePickerPrefab.ImageLoaded += this.FaceSwapImagePickerPrefab_imageLoaded;
			FaceSwapImagePickerPrefab.titleChanged += this.FaceSwapImagePickerPrefab_titleChanged;
			FaceSwapImagePickerPrefab.DeleteClicked += this.FaceSwapImagePickerPrefab_deleteClicked;
			EventManagementPage.onEventChanged += this.EventManagementPage_onEventChanged;
			Task.Factory.StartNew<Task>(delegate()
			{
				FaceSwap.<<-ctor>b__23_0>d <<-ctor>b__23_0>d;
				<<-ctor>b__23_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<-ctor>b__23_0>d.<>4__this = this;
				<<-ctor>b__23_0>d.<>1__state = -1;
				<<-ctor>b__23_0>d.<>t__builder.Start<FaceSwap.<<-ctor>b__23_0>d>(ref <<-ctor>b__23_0>d);
				return <<-ctor>b__23_0>d.<>t__builder.Task;
			});
		}

		// Token: 0x060004A6 RID: 1190 RVA: 0x0001A27E File Offset: 0x0001847E
		private void EventManagementPage_onEventChanged(OperationalEvent newEvent)
		{
			this.isFirstWork = true;
			FaceSwap.faceSwapData = null;
			Application.Current.Dispatcher.Invoke(delegate()
			{
				FaceSwapBackgroundGallery.FaceSwapData faceSwapData = FaceSwap.faceSwapData;
				if (faceSwapData != null)
				{
					List<FaceSwapTarget> faceSwapTargets = faceSwapData.FaceSwapTargets;
					if (faceSwapTargets != null)
					{
						faceSwapTargets.Clear();
					}
				}
				this.ImagePickerPrefabList.Clear();
				this.FaceSwapImagesStackPanel.Children.Clear();
			});
		}

		// Token: 0x060004A7 RID: 1191 RVA: 0x0001A2A8 File Offset: 0x000184A8
		private void FaceSwapImagePickerPrefab_titleChanged(string title, int index)
		{
			FaceSwap.<FaceSwapImagePickerPrefab_titleChanged>d__25 <FaceSwapImagePickerPrefab_titleChanged>d__;
			<FaceSwapImagePickerPrefab_titleChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<FaceSwapImagePickerPrefab_titleChanged>d__.title = title;
			<FaceSwapImagePickerPrefab_titleChanged>d__.index = index;
			<FaceSwapImagePickerPrefab_titleChanged>d__.<>1__state = -1;
			<FaceSwapImagePickerPrefab_titleChanged>d__.<>t__builder.Start<FaceSwap.<FaceSwapImagePickerPrefab_titleChanged>d__25>(ref <FaceSwapImagePickerPrefab_titleChanged>d__);
		}

		// Token: 0x060004A8 RID: 1192 RVA: 0x0001A2E8 File Offset: 0x000184E8
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			FaceSwap.<Page_Loaded>d__26 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<FaceSwap.<Page_Loaded>d__26>(ref <Page_Loaded>d__);
		}

		// Token: 0x060004A9 RID: 1193 RVA: 0x0001A31F File Offset: 0x0001851F
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.canWriteJson)
			{
				ExtensionMethod.CreateWriteJson<FaceSwapBackgroundGallery.FaceSwapData>(FaceSwap.faceSwapData, FaceSwap.faceSwapJSONPath);
			}
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		// Token: 0x060004AA RID: 1194 RVA: 0x0001A344 File Offset: 0x00018544
		private Task StartJsonProcessAndAddPrefabsAsync()
		{
			FaceSwap.<StartJsonProcessAndAddPrefabsAsync>d__28 <StartJsonProcessAndAddPrefabsAsync>d__;
			<StartJsonProcessAndAddPrefabsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartJsonProcessAndAddPrefabsAsync>d__.<>4__this = this;
			<StartJsonProcessAndAddPrefabsAsync>d__.<>1__state = -1;
			<StartJsonProcessAndAddPrefabsAsync>d__.<>t__builder.Start<FaceSwap.<StartJsonProcessAndAddPrefabsAsync>d__28>(ref <StartJsonProcessAndAddPrefabsAsync>d__);
			return <StartJsonProcessAndAddPrefabsAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060004AB RID: 1195 RVA: 0x0001A388 File Offset: 0x00018588
		private void FaceSwapImagePickerPrefab_deleteClicked(FaceSwapImagePickerPrefab obj)
		{
			if (obj == null)
			{
				return;
			}
			if (obj.FaceSwapTargetImage == null || obj.FaceSwapTargetImage.FileDetails == null || string.IsNullOrEmpty(obj.FaceSwapTargetImage.FileDetails.FullPath))
			{
				return;
			}
			Func<Frame, bool> <>9__1;
			Application.Current.Dispatcher.Invoke(delegate()
			{
				IEnumerable<Frame> source = this.FaceSwapImagesStackPanel.Children.OfType<Frame>();
				Func<Frame, bool> predicate;
				if ((predicate = <>9__1) == null)
				{
					predicate = (<>9__1 = ((Frame x) => x.Content == obj));
				}
				Frame frameToRemove = source.FirstOrDefault(predicate);
				if (frameToRemove != null)
				{
					this.FaceSwapImagesStackPanel.Children.Remove(frameToRemove);
				}
				int deletedIndex = obj.index;
				this.ImagePickerPrefabList.Remove(obj);
				FaceSwap.faceSwapData.FaceSwapTargets.RemoveAt(deletedIndex);
				this.ImagePickerPrefabList = (from x in this.ImagePickerPrefabList
				orderby x.index
				select x).ToList<FaceSwapImagePickerPrefab>();
				for (int i = 0; i < this.ImagePickerPrefabList.Count; i++)
				{
					this.ImagePickerPrefabList[i].index = i;
				}
				FaceSwap.faceSwapData.FaceSwapTargets = (from x in FaceSwap.faceSwapData.FaceSwapTargets
				orderby x.Index
				select x).ToList<FaceSwapTarget>();
				for (int j = 0; j < FaceSwap.faceSwapData.FaceSwapTargets.Count; j++)
				{
					FaceSwap.faceSwapData.FaceSwapTargets[j].Index = j;
				}
				for (int k = 0; k < this.ImagePickerPrefabList.Count; k++)
				{
					this.ImagePickerPrefabList[k].Load();
				}
				this.imgCount--;
				this.prefabCount--;
				List<FaceSwapImagePickerPrefab> currentEmptyPrefabs = (from x in this.ImagePickerPrefabList
				where x.prefabImage.Source == null
				select x).ToList<FaceSwapImagePickerPrefab>();
				if (currentEmptyPrefabs.Count == 0)
				{
					this.AddEmptyPrefab(null, null);
				}
				ExtensionMethod.CreateWriteJson<FaceSwapBackgroundGallery.FaceSwapData>(FaceSwap.faceSwapData, FaceSwap.faceSwapJSONPath);
			});
		}

		// Token: 0x060004AC RID: 1196 RVA: 0x0001A40C File Offset: 0x0001860C
		private void DeleteFile(string filePath)
		{
			try
			{
				string directoryPath = Path.Combine(new string[]
				{
					FaceSwap.imageFolderPath
				});
				if (Directory.Exists(directoryPath))
				{
					DirectoryInfo di = new DirectoryInfo(directoryPath);
					foreach (FileInfo file in di.GetFiles())
					{
						try
						{
							if (Path.GetFileName(filePath) == file.Name)
							{
								file.Delete();
								break;
							}
						}
						catch (Exception e)
						{
							MessageBoxWindow.CreateWindow("Couldn't delete files in folder", "Couldn't delete files in " + directoryPath, new List<MessageBoxWindow.ButtonType>
							{
								MessageBoxWindow.ButtonType.Continue
							}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						}
					}
				}
			}
			catch (IOException)
			{
			}
		}

		// Token: 0x060004AD RID: 1197 RVA: 0x0001A4C0 File Offset: 0x000186C0
		private void AddPrefabFromJson(int index)
		{
			if (this.ImagePickerPrefabList != null)
			{
				this.prefabCount = this.ImagePickerPrefabList.Count;
			}
			Application.Current.Dispatcher.Invoke(delegate()
			{
				if (FaceSwap.faceSwapData.FaceSwapTargets.Count <= index)
				{
					return;
				}
				FaceSwapTarget faceSwapTarget = FaceSwap.faceSwapData.FaceSwapTargets[index];
				FaceSwapImagePickerPrefab faceSwapImagePickerPrefab = new FaceSwapImagePickerPrefab(index, ref faceSwapTarget);
				faceSwapImagePickerPrefab.index = this.prefabCount;
				Frame frame = new Frame();
				frame.Content = faceSwapImagePickerPrefab;
				frame.Margin = new Thickness(20.0, 20.0, 20.0, 20.0);
				this.FaceSwapImagesStackPanel.Children.Add(frame);
				this.ImagePickerPrefabList.Add(faceSwapImagePickerPrefab);
			});
		}

		// Token: 0x060004AE RID: 1198 RVA: 0x0001A518 File Offset: 0x00018718
		private void AddEmptyPrefab(object sender, RoutedEventArgs e)
		{
			if (this.ImagePickerPrefabList != null)
			{
				this.prefabCount = this.ImagePickerPrefabList.Count;
			}
			if (this.ImagePickerPrefabList.Exists((FaceSwapImagePickerPrefab x) => x.IsEmpty))
			{
				return;
			}
			if (this.prefabCount == 100)
			{
				MessageBoxWindow.CreateWindow("Face Swap Limit Exceeded", string.Format("The maximum Faceswap Image limit is {0}, please open a new event to add more.", 100), new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			Application.Current.Dispatcher.Invoke(delegate()
			{
				FaceSwapTarget faceSwapTarget = null;
				FaceSwapImagePickerPrefab faceSwapImagePickerPrefab = new FaceSwapImagePickerPrefab(this.prefabCount, ref faceSwapTarget);
				faceSwapImagePickerPrefab.index = this.prefabCount;
				if (FaceSwap.addedNewPath.Count > 0)
				{
					this.loadingGifGrid.Visibility = Visibility.Visible;
					this.loadingText.Text = string.Format("Loading {0} items left", FaceSwap.addedNewPath.Count);
					faceSwapImagePickerPrefab.AddFromMultiSelect(FaceSwap.addedNewPath[0]);
					FaceSwap.addedNewPath.RemoveAt(0);
					this.scroll.ScrollToEnd();
				}
				else
				{
					this.loadingGifGrid.Visibility = Visibility.Hidden;
				}
				Frame frame = new Frame();
				frame.Content = faceSwapImagePickerPrefab;
				frame.Margin = new Thickness(20.0, 20.0, 20.0, 20.0);
				this.FaceSwapImagesStackPanel.Children.Add(frame);
				this.ImagePickerPrefabList.Add(faceSwapImagePickerPrefab);
			});
		}

		// Token: 0x060004AF RID: 1199 RVA: 0x0001A5BC File Offset: 0x000187BC
		public void FaceSwapImagePickerPrefab_imageLoaded(FaceSwapImagePickerPrefab faceSwapImagePickerPrefab)
		{
			if (faceSwapImagePickerPrefab.index > FaceSwap.faceSwapData.FaceSwapTargets.Count - 1)
			{
				FaceSwap.faceSwapData.FaceSwapTargets.Add(faceSwapImagePickerPrefab.FaceSwapTargetImage);
				if (!this.FirstLoadingActive)
				{
					this.AddEmptyPrefab(null, null);
				}
				if (!this.ImagePickerPrefabList.Exists((FaceSwapImagePickerPrefab x) => x.IsEmpty))
				{
					this.AddEmptyPrefab(null, null);
				}
			}
			else
			{
				FaceSwap.faceSwapData.FaceSwapTargets[faceSwapImagePickerPrefab.index] = faceSwapImagePickerPrefab.FaceSwapTargetImage;
			}
			Application.Current.Dispatcher.Invoke(delegate()
			{
				ExtensionMethod.CreateWriteJson<FaceSwapBackgroundGallery.FaceSwapData>(FaceSwap.faceSwapData, FaceSwap.faceSwapJSONPath);
			});
			if (this.FirstLoadingActive)
			{
				this.scroll.ScrollToEnd();
			}
		}

		// Token: 0x060004B0 RID: 1200 RVA: 0x0001A699 File Offset: 0x00018899
		public override string GetTitle()
		{
			return "Face Swap Gallery";
		}

		// Token: 0x060004B1 RID: 1201 RVA: 0x0001A6A0 File Offset: 0x000188A0
		public override string GetSubTitle()
		{
			return "Here you can set your Face Swap gallery! Be sure to upload your photos in JPG format in right resolution and min MB possible to have faster results.";
		}

		// Token: 0x060004B2 RID: 1202 RVA: 0x0001A6A7 File Offset: 0x000188A7
		private void AddImageButton_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060004B3 RID: 1203 RVA: 0x0001A6A9 File Offset: 0x000188A9
		private void AddImageButton_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060004B4 RID: 1204 RVA: 0x0001A6AB File Offset: 0x000188AB
		private void BuyNewAssetButton_Click(object sender, RoutedEventArgs e)
		{
			ExtensionMethod.OpenUrl("https://activationking.com/product-category/ai-face-swap-assets/");
		}

		// Token: 0x040004A4 RID: 1188
		public const int FaceSwapLimit = 100;

		// Token: 0x040004A5 RID: 1189
		private const string faceSwapFolderName = "FaceSwap";

		// Token: 0x040004A6 RID: 1190
		private bool canWriteJson;

		// Token: 0x040004A7 RID: 1191
		public static List<string> addedNewPath = new List<string>();

		// Token: 0x040004A8 RID: 1192
		private const string faceSwapJSONName = "FaceSwapData.json";

		// Token: 0x040004A9 RID: 1193
		private const string imageFolderName = "TargetImages";

		// Token: 0x040004AB RID: 1195
		private List<FaceSwapImagePickerPrefab> ImagePickerPrefabList = new List<FaceSwapImagePickerPrefab>();

		// Token: 0x040004AC RID: 1196
		private bool isFirstWork = true;

		// Token: 0x040004AD RID: 1197
		private int prefabCount;

		// Token: 0x040004AE RID: 1198
		private int imgCount;

		// Token: 0x040004AF RID: 1199
		public static FaceSwapBackgroundGallery.FaceSwapData faceSwapData = new FaceSwapBackgroundGallery.FaceSwapData();
	}
}
