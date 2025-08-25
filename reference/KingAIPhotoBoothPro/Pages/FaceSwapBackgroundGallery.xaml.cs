using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200002D RID: 45
	public partial class FaceSwapBackgroundGallery : Page
	{
		// Token: 0x06000283 RID: 643 RVA: 0x0000D013 File Offset: 0x0000B213
		public string GetOrginalPathFromCDNURL()
		{
			return (from x in FaceSwap.faceSwapData.FaceSwapTargets
			where x.CdnUrl == FaceSwapBackgroundGallery.selectedFaceSwapGalleryItem.CdnUrl
			select x).FirstOrDefault<FaceSwapTarget>().FileDetails.FullPath;
		}

		// Token: 0x06000284 RID: 644 RVA: 0x0000D052 File Offset: 0x0000B252
		public FaceSwapBackgroundGallery()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000285 RID: 645 RVA: 0x0000D086 File Offset: 0x0000B286
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
				return;
			}
			ActivationKingWindow.SetPage(ApplicationPage.AiSharingPage, false);
		}

		// Token: 0x06000286 RID: 646 RVA: 0x0000D09F File Offset: 0x0000B29F
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.UniformGridContext.ItemsSource = this.FaceSwapItems;
			this.LoadFaceSwapTargets();
			if (FaceSwap.faceSwapData.FaceSwapTargets.Count > 0)
			{
				this.CheckAndAddFaceSwapPrefabThreadFunc();
			}
			this.LoadLanguageStrings();
		}

		// Token: 0x06000287 RID: 647 RVA: 0x0000D0D8 File Offset: 0x0000B2D8
		private void LoadLanguageStrings()
		{
			string faceSwapBackgroundLabelString = Settings.GetValueString("lang_selectfacebackground");
			if (!string.IsNullOrEmpty(faceSwapBackgroundLabelString))
			{
				this.FaceSwapBackgroundGridLabel.Content = faceSwapBackgroundLabelString;
			}
		}

		// Token: 0x06000288 RID: 648 RVA: 0x0000D104 File Offset: 0x0000B304
		private void CheckAndAddFaceSwapPrefabThreadFunc()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.FaceSwapItems.Clear();
				this.FaceSwapPrefabs.Clear();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			for (int i = 0; i < FaceSwap.faceSwapData.FaceSwapTargets.Count; i++)
			{
				this.CreateFaceSwapGalleryItem(FaceSwap.faceSwapData.FaceSwapTargets[i]);
			}
		}

		// Token: 0x06000289 RID: 649 RVA: 0x0000D168 File Offset: 0x0000B368
		private void LoadFaceSwapTargets()
		{
			string eventAIBackgroundPath = FaceSwap.faceSwapJSONPath;
			FaceSwap.faceSwapData = this.LoadFaceSwapData(eventAIBackgroundPath);
			if (FaceSwap.faceSwapData != null)
			{
				FaceSwap.faceSwapData.FaceSwapTargets = (from x in FaceSwap.faceSwapData.FaceSwapTargets
				orderby x.Index
				select x).ToList<FaceSwapTarget>();
			}
		}

		// Token: 0x0600028A RID: 650 RVA: 0x0000D1CB File Offset: 0x0000B3CB
		public FaceSwapBackgroundGallery.FaceSwapData LoadFaceSwapData(string filePath)
		{
			return ExtensionMethod.ReadJson<FaceSwapBackgroundGallery.FaceSwapData>(filePath);
		}

		// Token: 0x0600028B RID: 651 RVA: 0x0000D1D3 File Offset: 0x0000B3D3
		private void FaceSwapGalleryItem_ClickEvent(FaceSwapTarget obj)
		{
			FaceSwapBackgroundGallery.selectedTarget = true;
			FaceSwapBackgroundGallery.selectedFaceSwapGalleryItem = obj;
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				ActivationKingWindow.SetPage(ApplicationPage.PhotoShootPage, false);
				return;
			}
			ActivationKingWindow.SetPage(ApplicationPage.AiSharingPage, false);
		}

		// Token: 0x0600028C RID: 652 RVA: 0x0000D1F8 File Offset: 0x0000B3F8
		private void CreateFaceSwapGalleryItem(FaceSwapTarget aIBackgroundref)
		{
			if (!File.Exists(aIBackgroundref.FileDetails.FullPath))
			{
				return;
			}
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				AIBackgroundPrefab faceSwapGalleryItem = new AIBackgroundPrefab();
				faceSwapGalleryItem.ClickEvent += this.FaceSwapGalleryItem_ClickEvent;
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.Height = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)aIBackgroundref.FileDetails.Height) / (double)((float)aIBackgroundref.FileDetails.Width);
				this.FaceSwapItems.Add(newFrame);
				try
				{
					faceSwapGalleryItem.LoadImage(aIBackgroundref.FileDetails.FullPath, aIBackgroundref);
				}
				catch (Exception)
				{
					return;
				}
				faceSwapGalleryItem.AIBackgroundTitle.Text = aIBackgroundref.Title;
				faceSwapGalleryItem.AIBackgroundTitle.TextWrapping = TextWrapping.Wrap;
				newFrame.Content = faceSwapGalleryItem;
				this.FaceSwapPrefabs.Add(faceSwapGalleryItem);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x04000216 RID: 534
		public ObservableCollection<Frame> FaceSwapItems = new ObservableCollection<Frame>();

		// Token: 0x04000217 RID: 535
		public List<AIBackgroundPrefab> FaceSwapPrefabs = new List<AIBackgroundPrefab>();

		// Token: 0x04000218 RID: 536
		public static bool selectedTarget;

		// Token: 0x04000219 RID: 537
		private string appPath = AppDomain.CurrentDomain.BaseDirectory;

		// Token: 0x0400021A RID: 538
		public static FaceSwapTarget selectedFaceSwapGalleryItem;

		// Token: 0x02000140 RID: 320
		public class FaceSwapData
		{
			// Token: 0x17000196 RID: 406
			// (get) Token: 0x06000C48 RID: 3144 RVA: 0x0004BBDE File Offset: 0x00049DDE
			// (set) Token: 0x06000C49 RID: 3145 RVA: 0x0004BBE6 File Offset: 0x00049DE6
			public List<FaceSwapTarget> FaceSwapTargets { get; set; }
		}
	}
}
