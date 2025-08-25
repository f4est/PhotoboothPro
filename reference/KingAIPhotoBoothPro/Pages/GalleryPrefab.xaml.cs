using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Variations;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200003F RID: 63
	public partial class GalleryPrefab : Page, IDisposable
	{
		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06000475 RID: 1141 RVA: 0x00019618 File Offset: 0x00017818
		// (remove) Token: 0x06000476 RID: 1142 RVA: 0x00019650 File Offset: 0x00017850
		public event Action<MediaClassBase> ClickEvent;

		// Token: 0x06000477 RID: 1143 RVA: 0x00019685 File Offset: 0x00017885
		public GalleryPrefab()
		{
			base.Content = base.Content;
			this.InitializeComponent();
			this.RefreshLoadingElements();
		}

		// Token: 0x06000478 RID: 1144 RVA: 0x000196B0 File Offset: 0x000178B0
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			VariationMedia.VariationProcessCompleted = (Action<VariationMedia>)Delegate.Remove(VariationMedia.VariationProcessCompleted, new Action<VariationMedia>(this.VariationMediaProcessCompleted));
			VariationMedia.ImagineJobCanceled -= this.VariationMedia_ImagineCanceled;
		}

		// Token: 0x06000479 RID: 1145 RVA: 0x000196E3 File Offset: 0x000178E3
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			VariationMedia.VariationProcessCompleted = (Action<VariationMedia>)Delegate.Combine(VariationMedia.VariationProcessCompleted, new Action<VariationMedia>(this.VariationMediaProcessCompleted));
			VariationMedia.ImagineJobCanceled += this.VariationMedia_ImagineCanceled;
		}

		// Token: 0x0600047A RID: 1146 RVA: 0x00019716 File Offset: 0x00017916
		private void VariationMedia_ImagineCanceled(string obj)
		{
			this.RefreshLoadingElements();
		}

		// Token: 0x0600047B RID: 1147 RVA: 0x00019720 File Offset: 0x00017920
		private void VariationMediaProcessCompleted(VariationMedia media)
		{
			if (media != null && this.thisMediaClass.MediaHash == media.OriginalMediaHash)
			{
				this.LoadImage((media.ResultMediaType == MediaType.photo) ? media.ResultLocalPath : media.ThumbnailLocalPath, media.ResultMediaType, this.thisMediaClass);
				this.RefreshLoadingElements();
			}
		}

		// Token: 0x0600047C RID: 1148 RVA: 0x00019778 File Offset: 0x00017978
		public void LoadImage(string Path, MediaType mediaType, MediaClassBase thisMedia)
		{
			this.thisMediaClass = thisMedia;
			if (File.Exists(Path))
			{
				try
				{
					DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						this.imagePath = Path;
						BitmapImage bitmap = new BitmapImage();
						bitmap.BeginInit();
						bitmap.DecodePixelWidth = this.maxWidth;
						bitmap.UriSource = new Uri(Path, UriKind.Absolute);
						bitmap.EndInit();
						this.GalleryImage.Source = bitmap;
					}), DispatcherPriority.Normal, Array.Empty<object>());
				}
				catch (Exception ex)
				{
				}
			}
			DispatcherOperation ope = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.videoIcon.Visibility = ((mediaType == MediaType.video) ? Visibility.Visible : Visibility.Collapsed);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x0600047D RID: 1149 RVA: 0x00019808 File Offset: 0x00017A08
		private void RefreshLoadingElements()
		{
			if (this.refreshLoadingWorking)
			{
				return;
			}
			this.refreshLoadingWorking = true;
			try
			{
				Task.Run(delegate()
				{
					GalleryPrefab.<<RefreshLoadingElements>b__13_0>d <<RefreshLoadingElements>b__13_0>d;
					<<RefreshLoadingElements>b__13_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<RefreshLoadingElements>b__13_0>d.<>4__this = this;
					<<RefreshLoadingElements>b__13_0>d.<>1__state = -1;
					<<RefreshLoadingElements>b__13_0>d.<>t__builder.Start<GalleryPrefab.<<RefreshLoadingElements>b__13_0>d>(ref <<RefreshLoadingElements>b__13_0>d);
					return <<RefreshLoadingElements>b__13_0>d.<>t__builder.Task;
				});
			}
			catch (Exception ex)
			{
			}
			this.refreshLoadingWorking = false;
		}

		// Token: 0x0600047E RID: 1150 RVA: 0x00019854 File Offset: 0x00017A54
		private void PhotoButton_Click(object sender, RoutedEventArgs e)
		{
			this.ClickEvent(this.thisMediaClass);
		}

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x0600047F RID: 1151 RVA: 0x00019868 File Offset: 0x00017A68
		// (remove) Token: 0x06000480 RID: 1152 RVA: 0x000198A0 File Offset: 0x00017AA0
		public event Action<GalleryPrefab> DeleteEvent;

		// Token: 0x06000481 RID: 1153 RVA: 0x000198D8 File Offset: 0x00017AD8
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			int index = DSLR.eventMediaClass.mediaList.FindIndex((MediaClassBase x) => x.MediaHash == this.thisMediaClass.MediaHash);
			if (index >= 0)
			{
				DSLR.eventMediaClass.mediaList[index].isDeleted = true;
				Action<GalleryPrefab> deleteEvent = this.DeleteEvent;
				if (deleteEvent == null)
				{
					return;
				}
				deleteEvent(this);
			}
		}

		// Token: 0x06000482 RID: 1154 RVA: 0x0001992C File Offset: 0x00017B2C
		public void ToggleDeleteButtonVisibility(bool isVisible)
		{
			this.CloseButton.Visibility = (isVisible ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x06000483 RID: 1155 RVA: 0x00019940 File Offset: 0x00017B40
		public void Dispose()
		{
			VariationMedia.VariationProcessCompleted = (Action<VariationMedia>)Delegate.Remove(VariationMedia.VariationProcessCompleted, new Action<VariationMedia>(this.VariationMediaProcessCompleted));
			VariationMedia.ImagineJobCanceled -= this.VariationMedia_ImagineCanceled;
			base.Loaded -= this.Page_Loaded;
			base.Unloaded -= this.Page_Unloaded;
			this.PhotoButton.Click -= this.PhotoButton_Click;
			this.CloseButton.Click -= this.DeleteButton_Click;
			if (this.GalleryImage != null)
			{
				this.GalleryImage.Source = null;
			}
			base.Content = null;
			this.ClickEvent = null;
			this.DeleteEvent = null;
			this.refreshLoadingWorking = false;
			GC.SuppressFinalize(this);
		}

		// Token: 0x04000481 RID: 1153
		public MediaClassBase thisMediaClass;

		// Token: 0x04000483 RID: 1155
		private int maxWidth = 480;

		// Token: 0x04000484 RID: 1156
		public string imagePath;

		// Token: 0x04000485 RID: 1157
		private bool refreshLoadingWorking;
	}
}
