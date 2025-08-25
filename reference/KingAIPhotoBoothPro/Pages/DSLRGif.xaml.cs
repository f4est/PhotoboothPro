using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using FFmpeg.NET.Events;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000037 RID: 55
	public partial class DSLRGif : Page
	{
		// Token: 0x06000355 RID: 853 RVA: 0x000122F0 File Offset: 0x000104F0
		public DSLRGif()
		{
			this.InitializeComponent();
			this.gifImageList = new List<Image>
			{
				this.Photo1Image,
				this.Photo2Image,
				this.Photo3Image,
				this.Photo4Image
			};
			DSLRGif.imageList = new List<BitmapImage>
			{
				null,
				null,
				null,
				null
			};
			DSLRGif.imageSourceList = new List<BitmapImage>
			{
				null,
				null,
				null,
				null
			};
			this.capturedFileList = new List<string>();
		}

		// Token: 0x06000356 RID: 854 RVA: 0x000123B0 File Offset: 0x000105B0
		private void PhotoFailed(string error)
		{
			DSLRGif.<PhotoFailed>d__14 <PhotoFailed>d__;
			<PhotoFailed>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PhotoFailed>d__.<>4__this = this;
			<PhotoFailed>d__.<>1__state = -1;
			<PhotoFailed>d__.<>t__builder.Start<DSLRGif.<PhotoFailed>d__14>(ref <PhotoFailed>d__);
		}

		// Token: 0x06000357 RID: 855 RVA: 0x000123E7 File Offset: 0x000105E7
		private void PhotoFailedMessageCompleted(MessageBoxWindow.MessageBoxReturn result)
		{
			this.CancelTakeOperation();
		}

		// Token: 0x06000358 RID: 856 RVA: 0x000123F0 File Offset: 0x000105F0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.isActive = true;
			this.photoNumber = 0;
			CameraControlClass.errorNumber = 0;
			if (this.isFirstWork)
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					LiveViewClass.LiveViewImageLoaded += this.LiveViewClass_LiveViewImageLoaded;
					CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Combine(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapture));
					CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Combine(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailed));
				}
				else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					WebcamControlClass.openCvCameraHelper.NewFrame += this.GifSource_NewFrame;
					WebcamControlClass.PhotoCaptured += this.PhotoCapture;
				}
				CameraControlClass.ffmpeg.Complete += this.Ffmpeg_Complete;
				CameraControlClass.ffmpeg.Error += this.Ffmpeg_Error;
				this.isFirstWork = false;
			}
			this.LiveViewImage.Visibility = Visibility.Visible;
			string[] gifFolderFiles = Directory.GetFiles(CameraControlClass.FolderForGif);
			for (int i = 0; i < gifFolderFiles.Length; i++)
			{
				try
				{
					File.Delete(gifFolderFiles[i]);
				}
				catch (Exception)
				{
				}
			}
			TimedAction.ExecuteWithDelay(delegate
			{
				this.CountDownGif_MediaEnded();
			}, TimeSpan.FromSeconds(3.0));
			this.ManagePortraitScreen();
		}

		// Token: 0x06000359 RID: 857 RVA: 0x00012540 File Offset: 0x00010740
		private void GifSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LiveViewImage.Source = eventArgs.Frame.ToBitmapSource();
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600035A RID: 858 RVA: 0x00012588 File Offset: 0x00010788
		private void Ffmpeg_Error(object sender, ConversionErrorEventArgs e)
		{
			string error = e.ToString();
		}

		// Token: 0x0600035B RID: 859 RVA: 0x0001259C File Offset: 0x0001079C
		private void Ffmpeg_Complete(object sender, ConversionCompleteEventArgs e)
		{
			if (this.isActive)
			{
				new Thread(delegate()
				{
					DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						this._firstFrameTime = null;
						FileInfo[] arrayFileInfo = new FileInfo[4];
						for (int i = 0; i < 4; i++)
						{
							arrayFileInfo[i] = new FileInfo(this.capturedFileList[i]);
						}
						File.Copy(this.capturedFileList[0], DSLRGif.CaptureResultPath.Substring(0, DSLRGif.CaptureResultPath.Length - 4) + "thumb.JPG");
						FileInfo resultFileInfo = new FileInfo(DSLRGif.CaptureResultPath);
						DSLR.eventMediaClass.mediaList.Add(new MediaClassBase
						{
							capturedFiles = arrayFileInfo,
							index = DSLR.eventMediaClass.mediaList.Count,
							mediaType = MediaType.video,
							dataType = DataType.gif,
							resultFile = resultFileInfo,
							backgroundColorHex = Settings.GetValueString("colorgreenscreen"),
							thumbnail = new FileInfo(DSLRGif.CaptureResultPath.Substring(0, DSLRGif.CaptureResultPath.Length - 4) + "thumb.JPG"),
							fileDetails = new FileInformation
							{
								Filename = resultFileInfo.Name,
								Filesize = resultFileInfo.Length,
								Width = TemplateClass.PaperWidthVideo,
								Height = TemplateClass.PaperHeightVideo
							}
						});
						AISharing.SharingMediaclass = (from x in DSLR.eventMediaClass.mediaList
						where x.index == DSLR.eventMediaClass.mediaList.Count - 1
						select x).FirstOrDefault<MediaClassBase>();
					}), DispatcherPriority.Normal, Array.Empty<object>());
					op.Wait();
					Thread.Sleep(500);
					DispatcherOperation ope = base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						this.LiveViewImage.Visibility = Visibility.Collapsed;
						ActivationKingWindow.SetPage(ApplicationPage.AiSharingPage, false);
					}), DispatcherPriority.Normal, Array.Empty<object>());
				}).Start();
			}
		}

		// Token: 0x0600035C RID: 860 RVA: 0x000125BC File Offset: 0x000107BC
		private void CountDownGif_MediaEnded()
		{
			if (!this.isFirstWork)
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					CameraControlClass.TakePicture();
					return;
				}
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					WebcamControlClass.TakePicture(false);
				}
			}
		}

		// Token: 0x0600035D RID: 861 RVA: 0x000125E4 File Offset: 0x000107E4
		private void RetakePhoto()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					CameraControlClass.StartLiveCamera();
				}
				this.LiveViewImage.Visibility = Visibility.Visible;
				TimedAction.ExecuteWithDelay(delegate
				{
					this.CountDownGif_MediaEnded();
				}, TimeSpan.FromMilliseconds(3000.0));
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600035E RID: 862 RVA: 0x00012617 File Offset: 0x00010817
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.CancelTakeOperation();
		}

		// Token: 0x0600035F RID: 863 RVA: 0x00012620 File Offset: 0x00010820
		private void CancelTakeOperation()
		{
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
			{
				LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
				CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapture));
				CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailed));
			}
			else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
			{
				WebcamControlClass.openCvCameraHelper.NewFrame -= this.GifSource_NewFrame;
				WebcamControlClass.PhotoCaptured -= this.PhotoCapture;
			}
			this.isFirstWork = true;
			ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
			this.photoNumber = 0;
			CameraControlClass.BreakCapturePhotoThread();
			ActivationKingWindow.DSLRGifPage = null;
		}

		// Token: 0x06000360 RID: 864 RVA: 0x000126D8 File Offset: 0x000108D8
		private void PhotoCapture(string filepath)
		{
			if (File.Exists(filepath))
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					DSLRGif.CapturedFilePath = filepath;
					DSLRGif.CapturedGifFilePath = Path.Combine(CameraControlClass.FolderForGif, this.photoNumber.ToString() + "_gif.png");
					ExtensionMethod.RenderAsync(new List<string>
					{
						DSLRGif.CapturedFilePath
					}, DSLRGif.CapturedGifFilePath, DSLR.isHaveGreenBox, DSLR.isHaveAIBackground, false, false, true);
					this.capturedFileList.Add(DSLRGif.CapturedGifFilePath);
					DSLRGif.imageList[this.photoNumber] = new BitmapImage();
					DSLRGif.imageList[this.photoNumber].BeginInit();
					DSLRGif.imageList[this.photoNumber].UriSource = new Uri(DSLRGif.CapturedFilePath, UriKind.Absolute);
					DSLRGif.imageList[this.photoNumber].EndInit();
					this.gifImageList[this.photoNumber].Source = DSLRGif.imageList[this.photoNumber];
					DSLRGif.imageSourceList[this.photoNumber] = new BitmapImage();
					DSLRGif.imageSourceList[this.photoNumber].BeginInit();
					DSLRGif.imageSourceList[this.photoNumber].UriSource = new Uri(DSLRGif.CapturedGifFilePath, UriKind.Absolute);
					DSLRGif.imageSourceList[this.photoNumber].EndInit();
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
				this.photoNumber++;
				if (this.photoNumber >= 4)
				{
					new Thread(delegate()
					{
						DSLRGif.CapturedFilePath = filepath;
						Thread.Sleep(3000);
						DSLRGif.CaptureResultPath = Path.Combine(CameraControlClass.FolderForResultsGif, DateTime.Now.ToString("dd.MM.yyyy_HH.mm.ss") + ".mp4");
						DispatcherOperation operation = this.Dispatcher.BeginInvoke(new Action(delegate()
						{
							DSLRGif.<>c.<<PhotoCapture>b__24_2>d <<PhotoCapture>b__24_2>d;
							<<PhotoCapture>b__24_2>d.<>t__builder = AsyncVoidMethodBuilder.Create();
							<<PhotoCapture>b__24_2>d.<>1__state = -1;
							<<PhotoCapture>b__24_2>d.<>t__builder.Start<DSLRGif.<>c.<<PhotoCapture>b__24_2>d>(ref <<PhotoCapture>b__24_2>d);
						}), DispatcherPriority.Normal, Array.Empty<object>());
						operation.Wait();
					}).Start();
					return;
				}
				DispatcherOperation ope = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
				}), DispatcherPriority.Normal, Array.Empty<object>());
				new Thread(delegate()
				{
					Thread.Sleep(1000);
					this.RetakePhoto();
				}).Start();
			}
		}

		// Token: 0x06000361 RID: 865 RVA: 0x000127A8 File Offset: 0x000109A8
		private void LiveViewClass_LiveViewImageLoaded()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LiveViewImage.Source = LiveViewClass.staticImage;
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x06000362 RID: 866 RVA: 0x000127DC File Offset: 0x000109DC
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapture));
			CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailed));
			this.isActive = false;
			this.isFirstWork = true;
		}

		// Token: 0x06000363 RID: 867 RVA: 0x00012848 File Offset: 0x00010A48
		private void pageGif_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x06000364 RID: 868 RVA: 0x00012850 File Offset: 0x00010A50
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRation = base.ActualWidth / 3840.0;
			if (isPortraitScreen)
			{
				this.gridMain.RowDefinitions[0].Height = new GridLength(180.0, GridUnitType.Star);
				this.gridMain.RowDefinitions[this.gridMain.RowDefinitions.Count - 1].Height = new GridLength(180.0, GridUnitType.Star);
				return;
			}
			this.gridMain.RowDefinitions[0].Height = new GridLength(8.0, GridUnitType.Star);
			this.gridMain.RowDefinitions[this.gridMain.RowDefinitions.Count - 1].Height = new GridLength(8.0, GridUnitType.Star);
		}

		// Token: 0x04000342 RID: 834
		private bool isActive;

		// Token: 0x04000343 RID: 835
		private bool isFirstWork = true;

		// Token: 0x04000344 RID: 836
		public static string CapturedFilePath;

		// Token: 0x04000345 RID: 837
		public static string CapturedGifFilePath;

		// Token: 0x04000346 RID: 838
		public static string CaptureResultPath;

		// Token: 0x04000347 RID: 839
		private int photoNumber;

		// Token: 0x04000348 RID: 840
		private List<string> capturedFileList;

		// Token: 0x04000349 RID: 841
		private List<Image> gifImageList;

		// Token: 0x0400034A RID: 842
		private static List<BitmapImage> imageList;

		// Token: 0x0400034B RID: 843
		private static List<BitmapImage> imageSourceList;

		// Token: 0x0400034C RID: 844
		private DateTime? _firstFrameTime;

		// Token: 0x0400034D RID: 845
		[NonSerialized]
		private VideoWriter _writer = new VideoWriter();

		// Token: 0x0400034E RID: 846
		private static bool firstTimeLoaded;
	}
}
