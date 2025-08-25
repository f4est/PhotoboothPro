using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Variations;
using WebSocketSharp;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200003E RID: 62
	public partial class Gallery : Page
	{
		// Token: 0x17000075 RID: 117
		// (get) Token: 0x0600045C RID: 1116 RVA: 0x000188D1 File Offset: 0x00016AD1
		// (set) Token: 0x0600045D RID: 1117 RVA: 0x000188D9 File Offset: 0x00016AD9
		public ObservableCollection<Frame> Items
		{
			get
			{
				return this.items;
			}
			set
			{
				this.items = value;
			}
		}

		// Token: 0x0600045E RID: 1118 RVA: 0x000188E4 File Offset: 0x00016AE4
		public Gallery()
		{
			this.InitializeComponent();
			this.videoPlayer.MediaEnded += this.VideoPlayer_MediaEnded;
			EventManagementPage.onEventChanged += this.EventManagementPage_onEventChanged;
		}

		// Token: 0x0600045F RID: 1119 RVA: 0x00018930 File Offset: 0x00016B30
		private void EventManagementPage_onEventChanged(OperationalEvent newEvent)
		{
			if (this.files != null)
			{
				this.files.Clear();
				if (this.GalleryPrefabs != null)
				{
					for (int i = 0; i < this.GalleryPrefabs.Count; i++)
					{
						this.GalleryPrefabs[i].Dispose();
					}
					this.GalleryPrefabs.Clear();
				}
			}
		}

		// Token: 0x06000460 RID: 1120 RVA: 0x0001898C File Offset: 0x00016B8C
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.pageNumber = 0;
			this.PageNumberText.Content = (this.pageNumber + 1).ToString();
			this.EventNameText.Content = EventManagementPage.GetCurrentEvent().EventName;
			if (this.GalleryPrefabs == null)
			{
				this.GalleryPrefabs = new List<GalleryPrefab>();
			}
			this.galleryThread = new Thread(new ThreadStart(this.CheckAndAddGalleryPrefabThreadFunc));
			this.files = new List<MediaClassBase>();
			int k;
			int j;
			for (k = 0; k < DSLR.eventMediaClass.mediaList.Count; k = j + 1)
			{
				if (File.Exists(DSLR.eventMediaClass.mediaList[k].resultFile.FullName))
				{
					if (!Array.Exists<MediaClassBase>(this.files.ToArray(), (MediaClassBase x) => x.resultFile.FullName == DSLR.eventMediaClass.mediaList[k].resultFile.FullName) && !DSLR.eventMediaClass.mediaList[k].isDeleted)
					{
						this.files.Add(DSLR.eventMediaClass.mediaList[k]);
					}
				}
				else
				{
					DSLR.eventMediaClass.mediaList.Remove(DSLR.eventMediaClass.mediaList[k]);
					j = k;
					k = j - 1;
				}
				j = k;
			}
			int l;
			for (l = 0; l < DSLR.eventMediaClass.mediaList.Count; l = j + 1)
			{
				if (File.Exists(DSLR.eventMediaClass.mediaList[l].resultFile.FullName))
				{
					if (!Array.Exists<MediaClassBase>(this.files.ToArray(), (MediaClassBase x) => x.resultFile.FullName == DSLR.eventMediaClass.mediaList[l].resultFile.FullName) && !DSLR.eventMediaClass.mediaList[l].isDeleted)
					{
						this.files.Add(DSLR.eventMediaClass.mediaList[l]);
					}
				}
				else
				{
					DSLR.eventMediaClass.mediaList.Remove(DSLR.eventMediaClass.mediaList[l]);
					j = l;
					l = j - 1;
				}
				j = l;
			}
			int i;
			for (i = 0; i < DSLR.eventMediaClass.mediaList.Count; i = j + 1)
			{
				if (File.Exists(DSLR.eventMediaClass.mediaList[i].resultFile.FullName))
				{
					if (!Array.Exists<MediaClassBase>(this.files.ToArray(), (MediaClassBase x) => x.resultFile.FullName == DSLR.eventMediaClass.mediaList[i].resultFile.FullName) && !DSLR.eventMediaClass.mediaList[i].isDeleted)
					{
						this.files.Add(DSLR.eventMediaClass.mediaList[i]);
					}
				}
				else
				{
					DSLR.eventMediaClass.mediaList.Remove(DSLR.eventMediaClass.mediaList[i]);
					j = i;
					i = j - 1;
				}
				j = i;
			}
			this.files = (from x in this.files
			orderby x.resultFile.CreationTime descending
			select x).ToList<MediaClassBase>();
			this.galleryThread.Start();
			this.ManagePortraitScreen();
			this.LoadLanguage();
		}

		// Token: 0x06000461 RID: 1121 RVA: 0x00018D2A File Offset: 0x00016F2A
		private void LoadLanguage()
		{
			this.DeleteButton.Content = Settings.GetValueString("lang_delete", true);
		}

		// Token: 0x06000462 RID: 1122 RVA: 0x00018D42 File Offset: 0x00016F42
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
		}

		// Token: 0x06000463 RID: 1123 RVA: 0x00018D4C File Offset: 0x00016F4C
		private void CreateGalleryItem(MediaClassBase MediaInfo, bool isVideo)
		{
			MediaInfo.MediaHash == "c2jrsl";
			Func<VariationMedia, bool> <>9__1;
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				GalleryPrefab newGalleryPrefab = new GalleryPrefab();
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.Height = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)MediaInfo.fileDetails.Height) / (double)((float)MediaInfo.fileDetails.Width);
				this.Items.Add(newFrame);
				if (isVideo)
				{
					newGalleryPrefab.LoadImage(MediaInfo.thumbnail.FullName, MediaType.video, MediaInfo);
				}
				else
				{
					IEnumerable<VariationMedia> variationMediasList = VariationMediaHelper.VariationMediasList;
					Func<VariationMedia, bool> predicate;
					if ((predicate = <>9__1) == null)
					{
						predicate = (<>9__1 = ((VariationMedia x) => x.OriginalMediaHash == MediaInfo.MediaHash && x.MediaVariationSynced));
					}
					VariationMedia refMedia = variationMediasList.Where(predicate).LastOrDefault<VariationMedia>();
					if (refMedia != null && File.Exists(refMedia.ResultLocalPath))
					{
						if (refMedia.ResultMediaType == MediaType.photo)
						{
							newGalleryPrefab.LoadImage(refMedia.ResultLocalPath, refMedia.ResultMediaType, MediaInfo);
						}
						else
						{
							newGalleryPrefab.LoadImage(refMedia.ThumbnailLocalPath, refMedia.ResultMediaType, MediaInfo);
						}
					}
					else
					{
						newGalleryPrefab.LoadImage(MediaInfo.resultFile.FullName, MediaType.photo, MediaInfo);
					}
				}
				newFrame.Content = newGalleryPrefab;
				newGalleryPrefab.ClickEvent += this.NewGalleryPrefab_ClickEvent;
				this.GalleryPrefabs.Add(newGalleryPrefab);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000464 RID: 1124 RVA: 0x00018DA9 File Offset: 0x00016FA9
		private void NewGalleryPrefab_ClickEvent(MediaClassBase mediaBase)
		{
			AISharing.SharingMediaclass = mediaBase;
			ActivationKingWindow.SetPage(ApplicationPage.AiSharingPage, false);
		}

		// Token: 0x06000465 RID: 1125 RVA: 0x00018DB8 File Offset: 0x00016FB8
		private void CheckAndAddGalleryPrefabThreadFunc()
		{
			Gallery.<>c__DisplayClass18_0 CS$<>8__locals1 = new Gallery.<>c__DisplayClass18_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.filterMedia = this.files.Skip(this.pageNumber * Gallery.pageSize).Take(Gallery.pageSize).ToList<MediaClassBase>();
			base.Dispatcher.Invoke(delegate()
			{
				CS$<>8__locals1.<>4__this.Items.Clear();
				foreach (GalleryPrefab item in CS$<>8__locals1.<>4__this.GalleryPrefabs)
				{
					item.Dispose();
				}
				CS$<>8__locals1.<>4__this.GalleryPrefabs.Clear();
			}, DispatcherPriority.Normal);
			int i;
			int j;
			for (i = 0; i < CS$<>8__locals1.filterMedia.Count; i = j + 1)
			{
				if (this.GalleryPrefabs.Count == 0 || (from x in this.GalleryPrefabs
				where x.imagePath == CS$<>8__locals1.filterMedia[i].resultFile.FullName
				select x).FirstOrDefault<GalleryPrefab>() == null)
				{
					this.CreateGalleryItem(CS$<>8__locals1.filterMedia[i], CS$<>8__locals1.filterMedia[i].mediaType == MediaType.video);
				}
				j = i;
			}
			DSLR.eventMediaClass.SaveToFile();
		}

		// Token: 0x06000466 RID: 1126 RVA: 0x00018EC8 File Offset: 0x000170C8
		private void NextGalleryButton_Click(object sender, RoutedEventArgs e)
		{
			int maxLimit = this.files.Count / Gallery.pageSize;
			if (this.pageNumber < maxLimit)
			{
				this.pageNumber++;
			}
			this.PageNumberText.Content = (this.pageNumber + 1).ToString();
			this.CheckAndAddGalleryPrefabThreadFunc();
		}

		// Token: 0x06000467 RID: 1127 RVA: 0x00018F20 File Offset: 0x00017120
		private void PreviousGalleryButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.pageNumber > 0)
			{
				this.pageNumber--;
			}
			this.CheckAndAddGalleryPrefabThreadFunc();
			this.PageNumberText.Content = (this.pageNumber + 1).ToString();
		}

		// Token: 0x06000468 RID: 1128 RVA: 0x00018F65 File Offset: 0x00017165
		private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.videoPlayer.Position = TimeSpan.Zero;
		}

		// Token: 0x06000469 RID: 1129 RVA: 0x00018F78 File Offset: 0x00017178
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.previewGrid.Visibility = Visibility.Collapsed;
			try
			{
				this.videoPlayer.Volume = 0.0;
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600046A RID: 1130 RVA: 0x00018FBC File Offset: 0x000171BC
		private void previewGrid_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x0600046B RID: 1131 RVA: 0x00018FC0 File Offset: 0x000171C0
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRation = base.ActualWidth / 3840.0;
			if (isPortraitScreen)
			{
				this.gridPreview.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Star);
				this.gridPreview.RowDefinitions[1].Height = new GridLength(2.0, GridUnitType.Star);
				this.gridPreview.RowDefinitions[2].Height = new GridLength(3.0, GridUnitType.Star);
				this.gridPreview.ColumnDefinitions[0].Width = new GridLength(1.0, GridUnitType.Star);
				this.gridPreview.ColumnDefinitions[1].Width = new GridLength(10.0, GridUnitType.Star);
				this.gridPreview.ColumnDefinitions[2].Width = new GridLength(1.0, GridUnitType.Star);
				return;
			}
			this.gridPreview.RowDefinitions[0].Height = new GridLength(1.0, GridUnitType.Star);
			this.gridPreview.RowDefinitions[1].Height = new GridLength(2.0, GridUnitType.Star);
			this.gridPreview.RowDefinitions[2].Height = new GridLength(1.0, GridUnitType.Star);
			this.gridPreview.ColumnDefinitions[0].Width = new GridLength(4.0, GridUnitType.Star);
			this.gridPreview.ColumnDefinitions[1].Width = new GridLength(7.0, GridUnitType.Star);
			this.gridPreview.ColumnDefinitions[2].Width = new GridLength(4.0, GridUnitType.Star);
		}

		// Token: 0x0600046C RID: 1132 RVA: 0x000191B0 File Offset: 0x000173B0
		private void Galleri_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x0600046D RID: 1133 RVA: 0x000191B8 File Offset: 0x000173B8
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (!this.isDeleteButtonClicked)
			{
				this.StartDeleteProcess();
				this.isDeleteButtonClicked = true;
				return;
			}
			this.StopDeleteProcess();
			this.isDeleteButtonClicked = false;
		}

		// Token: 0x0600046E RID: 1134 RVA: 0x000191E0 File Offset: 0x000173E0
		private void StartDeleteProcess()
		{
			foreach (GalleryPrefab galleryPrefab in this.GalleryPrefabs)
			{
				galleryPrefab.ToggleDeleteButtonVisibility(true);
				galleryPrefab.DeleteEvent += this.GalleryPrefab_DeleteEvent;
			}
		}

		// Token: 0x0600046F RID: 1135 RVA: 0x00019248 File Offset: 0x00017448
		private void StopDeleteProcess()
		{
			foreach (GalleryPrefab galleryPrefab in this.GalleryPrefabs)
			{
				galleryPrefab.ToggleDeleteButtonVisibility(false);
				galleryPrefab.DeleteEvent += this.GalleryPrefab_DeleteEvent;
			}
		}

		// Token: 0x06000470 RID: 1136 RVA: 0x000192B0 File Offset: 0x000174B0
		private void GalleryPrefab_DeleteEvent(GalleryPrefab galleryPrefab)
		{
			this.Items.Remove(this.Items.FirstOrDefault((Frame frame) => frame.Content == galleryPrefab));
			this.GalleryPrefabs.Remove(galleryPrefab);
			this.files.Remove(this.files.FirstOrDefault((MediaClassBase f) => !f.MediaHash.IsNullOrEmpty() && f.MediaHash == galleryPrefab.thisMediaClass.MediaHash));
			galleryPrefab.DeleteEvent -= this.GalleryPrefab_DeleteEvent;
		}

		// Token: 0x06000471 RID: 1137 RVA: 0x0001933C File Offset: 0x0001753C
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			foreach (GalleryPrefab galleryPrefab in this.GalleryPrefabs)
			{
				galleryPrefab.DeleteEvent -= this.GalleryPrefab_DeleteEvent;
			}
		}

		// Token: 0x04000464 RID: 1124
		public static BitmapImage bitmapFile = null;

		// Token: 0x04000465 RID: 1125
		public List<MediaClassBase> files;

		// Token: 0x04000466 RID: 1126
		public List<GalleryPrefab> GalleryPrefabs;

		// Token: 0x04000467 RID: 1127
		private Thread galleryThread;

		// Token: 0x04000468 RID: 1128
		private ObservableCollection<Frame> items = new ObservableCollection<Frame>();

		// Token: 0x04000469 RID: 1129
		private int pageNumber;

		// Token: 0x0400046A RID: 1130
		private static int pageSize = 12;

		// Token: 0x0400046B RID: 1131
		private static int MaxPrefabs = 50;

		// Token: 0x0400046C RID: 1132
		private bool isDeleteButtonClicked;
	}
}
