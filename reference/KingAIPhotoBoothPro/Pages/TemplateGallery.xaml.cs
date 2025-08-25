using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Services;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000031 RID: 49
	public partial class TemplateGallery : Page
	{
		// Token: 0x060002C5 RID: 709 RVA: 0x0000E620 File Offset: 0x0000C820
		public TemplateGallery()
		{
			this.InitializeComponent();
		}

		// Token: 0x060002C6 RID: 710 RVA: 0x0000E650 File Offset: 0x0000C850
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			TemplateGallery.<Page_Loaded>d__4 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<TemplateGallery.<Page_Loaded>d__4>(ref <Page_Loaded>d__);
		}

		// Token: 0x060002C7 RID: 711 RVA: 0x0000E688 File Offset: 0x0000C888
		private void CheckAndAddTemplateGalleryPrefabThreadFunc()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.TemplateItems.Clear();
				this.TemplateGalleryPrefabs.Clear();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			for (int i = 0; i < this.TemplateTargets.Count; i++)
			{
				this.CreateFaceSwapGalleryItem(this.TemplateTargets[i]);
			}
		}

		// Token: 0x060002C8 RID: 712 RVA: 0x0000E6E4 File Offset: 0x0000C8E4
		private void CreateFaceSwapGalleryItem(TemplateGalleryTarget target)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				TemplateGalleryPrefab TemplateGalleryItem = new TemplateGalleryPrefab();
				TemplateGalleryItem.ClickEvent += this.TemplateGalleryItem_ClickEvent;
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(15.0);
				int pageHeight;
				int pageWidth;
				if (DSLR.isPhoto)
				{
					pageHeight = target.ThisTemplate.PageHeightPhoto;
					pageWidth = target.ThisTemplate.PageWidthPhoto;
				}
				else
				{
					pageHeight = target.ThisTemplate.PageHeightVideo;
					pageWidth = target.ThisTemplate.PageWidthVideo;
				}
				if (this.ActualHeight > this.ActualWidth)
				{
					newFrame.Height = this.UniformGridContext.ActualHeight * 0.22 * (double)((float)pageHeight) / (double)((float)pageWidth);
					newFrame.Width = newFrame.Height * (double)((float)pageWidth / (float)pageHeight);
				}
				else
				{
					newFrame.Width = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)pageWidth) / (double)((float)pageHeight);
					newFrame.Height = newFrame.Width * (double)((float)pageHeight / (float)pageWidth);
				}
				this.TemplateItems.Add(newFrame);
				TemplateGalleryItem.LoadImage(target.photoPath, target.Index);
				newFrame.Content = TemplateGalleryItem;
				this.TemplateGalleryPrefabs.Add(TemplateGalleryItem);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060002C9 RID: 713 RVA: 0x0000E724 File Offset: 0x0000C924
		private void TemplateGalleryItem_ClickEvent(int ID)
		{
			if (DSLR.isPhoto)
			{
				TemplateClass.selectedCanvasID = ID;
			}
			else
			{
				TemplateClass.selectedCanvasVideoID = ID;
			}
			TemplateClass.UpdateTemplate();
			TemplateClass.SaveBackgroundAndWatermark(DSLR.isPhoto);
			ActivationKingWindow.SetPage(this.SelectGoPage(), false);
		}

		// Token: 0x060002CA RID: 714 RVA: 0x0000E756 File Offset: 0x0000C956
		private ApplicationPage SelectGoPage()
		{
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				return MainSettingsPage.SelectAIPage();
			}
			if (DSLR.isPhoto)
			{
				return ApplicationPage.PhotoShootPage;
			}
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				Motion360Platform.StartMotion();
				return ApplicationPage.GoProShootPage;
			}
			return ApplicationPage.VideoShootPage;
		}

		// Token: 0x060002CB RID: 715 RVA: 0x0000E780 File Offset: 0x0000C980
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.UniformGridContext.ItemsSource = this.TemplateItems;
			ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
		}

		// Token: 0x060002CC RID: 716 RVA: 0x0000E79C File Offset: 0x0000C99C
		private Task LoadTemplateTargets()
		{
			TemplateGallery.<LoadTemplateTargets>d__10 <LoadTemplateTargets>d__;
			<LoadTemplateTargets>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadTemplateTargets>d__.<>4__this = this;
			<LoadTemplateTargets>d__.<>1__state = -1;
			<LoadTemplateTargets>d__.<>t__builder.Start<TemplateGallery.<LoadTemplateTargets>d__10>(ref <LoadTemplateTargets>d__);
			return <LoadTemplateTargets>d__.<>t__builder.Task;
		}

		// Token: 0x04000267 RID: 615
		public ObservableCollection<Frame> TemplateItems = new ObservableCollection<Frame>();

		// Token: 0x04000268 RID: 616
		public List<TemplateGalleryPrefab> TemplateGalleryPrefabs = new List<TemplateGalleryPrefab>();

		// Token: 0x04000269 RID: 617
		public List<TemplateGalleryTarget> TemplateTargets = new List<TemplateGalleryTarget>();
	}
}
