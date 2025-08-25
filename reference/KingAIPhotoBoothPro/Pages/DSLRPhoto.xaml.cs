using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Variations;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000038 RID: 56
	public partial class DSLRPhoto : Page
	{
		// Token: 0x0600036E RID: 878 RVA: 0x00012D84 File Offset: 0x00010F84
		public DSLRPhoto()
		{
			this.InitializeComponent();
			DSLRPhoto.elapsedSeconds = 15;
		}

		// Token: 0x0600036F RID: 879 RVA: 0x00012DF0 File Offset: 0x00010FF0
		private void PhotoCapturedMain(string filepath)
		{
			DSLRPhoto.<PhotoCapturedMain>d__15 <PhotoCapturedMain>d__;
			<PhotoCapturedMain>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PhotoCapturedMain>d__.<>4__this = this;
			<PhotoCapturedMain>d__.filepath = filepath;
			<PhotoCapturedMain>d__.<>1__state = -1;
			<PhotoCapturedMain>d__.<>t__builder.Start<DSLRPhoto.<PhotoCapturedMain>d__15>(ref <PhotoCapturedMain>d__);
		}

		// Token: 0x06000370 RID: 880 RVA: 0x00012E30 File Offset: 0x00011030
		private Task<MonoFunctionPhotoBoothController.AIInfo> GetAIInfo(string orginalPhotoPath)
		{
			DSLRPhoto.<GetAIInfo>d__16 <GetAIInfo>d__;
			<GetAIInfo>d__.<>t__builder = AsyncTaskMethodBuilder<MonoFunctionPhotoBoothController.AIInfo>.Create();
			<GetAIInfo>d__.orginalPhotoPath = orginalPhotoPath;
			<GetAIInfo>d__.<>1__state = -1;
			<GetAIInfo>d__.<>t__builder.Start<DSLRPhoto.<GetAIInfo>d__16>(ref <GetAIInfo>d__);
			return <GetAIInfo>d__.<>t__builder.Task;
		}

		// Token: 0x06000371 RID: 881 RVA: 0x00012E74 File Offset: 0x00011074
		private void LiveViewClass_LiveViewImageLoaded()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LiveViewImage.Source = LiveViewClass.staticImage;
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x06000372 RID: 882 RVA: 0x00012EA8 File Offset: 0x000110A8
		private void PhotoFailedAsync(string filepath)
		{
			DSLRPhoto.<PhotoFailedAsync>d__18 <PhotoFailedAsync>d__;
			<PhotoFailedAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PhotoFailedAsync>d__.<>4__this = this;
			<PhotoFailedAsync>d__.<>1__state = -1;
			<PhotoFailedAsync>d__.<>t__builder.Start<DSLRPhoto.<PhotoFailedAsync>d__18>(ref <PhotoFailedAsync>d__);
		}

		// Token: 0x06000373 RID: 883 RVA: 0x00012EDF File Offset: 0x000110DF
		private void CancelPhotoEvent(MessageBoxWindow.MessageBoxReturn @return)
		{
			this.CancelPhotoEvent();
			switch (@return)
			{
			default:
				return;
			}
		}

		// Token: 0x06000374 RID: 884 RVA: 0x00012EF9 File Offset: 0x000110F9
		private void SkipVideoButton_Click(object sender, MouseButtonEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
		}

		// Token: 0x06000375 RID: 885 RVA: 0x00012F1B File Offset: 0x0001111B
		private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
			this.isMirrorVideoEnabled = false;
		}

		// Token: 0x06000376 RID: 886 RVA: 0x00012F44 File Offset: 0x00011144
		public void Page_LoadedAsync(object sender, RoutedEventArgs e)
		{
			DSLRPhoto.<Page_LoadedAsync>d__23 <Page_LoadedAsync>d__;
			<Page_LoadedAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_LoadedAsync>d__.<>4__this = this;
			<Page_LoadedAsync>d__.<>1__state = -1;
			<Page_LoadedAsync>d__.<>t__builder.Start<DSLRPhoto.<Page_LoadedAsync>d__23>(ref <Page_LoadedAsync>d__);
		}

		// Token: 0x06000377 RID: 887 RVA: 0x00012F7B File Offset: 0x0001117B
		private void VariationMedia_ImagineJobCanceled(string obj)
		{
			this.CancelPhotoEvent();
		}

		// Token: 0x06000378 RID: 888 RVA: 0x00012F83 File Offset: 0x00011183
		private void BunnyCDNHelper_MediaLinkGenerated(MediaClassBase obj)
		{
			if (AISharing.SharingMediaclass.resultFile == obj.resultFile)
			{
				AISharing.SharingMediaclass = obj;
			}
		}

		// Token: 0x06000379 RID: 889 RVA: 0x00012FA0 File Offset: 0x000111A0
		private void ManageMirrorBoothVideo()
		{
			string videoPath = Settings.GetValueString("waitvideo") ?? "0.xyz";
			bool isVideoEnabled = Settings.GetValueBoolean("waitvideocheck").GetValueOrDefault();
			if (File.Exists(videoPath) && isVideoEnabled)
			{
				this.isMirrorVideoEnabled = true;
				this.videoPlayer.MediaEnded += this.VideoPlayer_MediaEnded;
				this.videoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
				this.adsVideoGrid.Visibility = Visibility.Visible;
				this.videoPlayer.Volume = 100.0;
				return;
			}
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
		}

		// Token: 0x0600037A RID: 890 RVA: 0x00013050 File Offset: 0x00011250
		private Task StartCountdown()
		{
			DSLRPhoto.<StartCountdown>d__27 <StartCountdown>d__;
			<StartCountdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartCountdown>d__.<>4__this = this;
			<StartCountdown>d__.<>1__state = -1;
			<StartCountdown>d__.<>t__builder.Start<DSLRPhoto.<StartCountdown>d__27>(ref <StartCountdown>d__);
			return <StartCountdown>d__.<>t__builder.Task;
		}

		// Token: 0x0600037B RID: 891 RVA: 0x00013093 File Offset: 0x00011293
		private void LoadLanguageStrings()
		{
			ExtensionMethod.LoadLanguageString(Settings.GetValueString("lang_cancel"), this.CloseFullscreenButton);
			ExtensionMethod.LoadLanguageString(Settings.GetValueString("lang_remainingseconds"), this.loadingCountDown);
		}

		// Token: 0x0600037C RID: 892 RVA: 0x000130C0 File Offset: 0x000112C0
		private void WebcamControlClass_LiveViewImageLoaded(byte[] imageData)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				BitmapImage bitmapImage = new BitmapImage();
				using (MemoryStream ms = new MemoryStream(imageData))
				{
					bitmapImage.BeginInit();
					bitmapImage.StreamSource = ms;
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.EndInit();
				}
				bitmapImage.Freeze();
				string rotation = Settings.GetValueString("rotation") ?? "0";
				if (rotation.Contains("90"))
				{
					TransformedBitmap rotatedBitmap = new TransformedBitmap(bitmapImage, new RotateTransform(90.0));
					this.LiveViewImage.Source = rotatedBitmap;
					return;
				}
				if (rotation.Contains("180"))
				{
					TransformedBitmap rotatedBitmap2 = new TransformedBitmap(bitmapImage, new RotateTransform(180.0));
					this.LiveViewImage.Source = rotatedBitmap2;
					return;
				}
				if (rotation.Contains("270"))
				{
					TransformedBitmap rotatedBitmap3 = new TransformedBitmap(bitmapImage, new RotateTransform(270.0));
					this.LiveViewImage.Source = rotatedBitmap3;
					return;
				}
				this.LiveViewImage.Source = bitmapImage;
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600037D RID: 893 RVA: 0x00013108 File Offset: 0x00011308
		private void AnimateFontSize(TextBlock textBlock, double fromSize, double toSize)
		{
			DoubleAnimation fontSizeAnimation = new DoubleAnimation
			{
				From = new double?(fromSize),
				To = new double?(toSize),
				Duration = TimeSpan.FromSeconds(1.0)
			};
			Storyboard.SetTarget(fontSizeAnimation, textBlock);
			Storyboard.SetTargetProperty(fontSizeAnimation, new PropertyPath(TextBlock.FontSizeProperty));
			Storyboard fontSizeStoryboard = new Storyboard();
			fontSizeStoryboard.Children.Clear();
			fontSizeStoryboard.Children.Add(fontSizeAnimation);
			fontSizeStoryboard.Begin();
		}

		// Token: 0x0600037E RID: 894 RVA: 0x00013188 File Offset: 0x00011388
		private void RetakePhoto()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				DSLRPhoto.<<RetakePhoto>b__33_0>d <<RetakePhoto>b__33_0>d;
				<<RetakePhoto>b__33_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<RetakePhoto>b__33_0>d.<>4__this = this;
				<<RetakePhoto>b__33_0>d.<>1__state = -1;
				<<RetakePhoto>b__33_0>d.<>t__builder.Start<DSLRPhoto.<<RetakePhoto>b__33_0>d>(ref <<RetakePhoto>b__33_0>d);
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600037F RID: 895 RVA: 0x000131BC File Offset: 0x000113BC
		private void CountDownGif_MediaEnded()
		{
			if (!this.isFirstWork)
			{
				this.countdownContainer.Visibility = Visibility.Collapsed;
				this.countDownText.Visibility = Visibility.Collapsed;
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					CameraControlClass.TakePicture();
					int? num = this.countdownSecondsLimit;
					this.remainingCountdownSeconds = ((num != null) ? new float?((float)(num.GetValueOrDefault() / 1000)) : null);
					return;
				}
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					WebcamControlClass.TakePicture(this.capturedPhotoNumber >= TemplateClass.capturePhotoCount - 1);
					int? num = this.countdownSecondsLimit;
					this.remainingCountdownSeconds = ((num != null) ? new float?((float)(num.GetValueOrDefault() / 1000)) : null);
				}
			}
		}

		// Token: 0x06000380 RID: 896 RVA: 0x00013280 File Offset: 0x00011480
		private void CancelPhotoEvent()
		{
			this.closeButtonPressed = true;
			LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			if (WebcamControlClass.openCvCameraHelper._capture != null)
			{
				WebcamControlClass.LiveViewImageLoaded -= this.WebcamControlClass_LiveViewImageLoaded;
			}
			WebcamControlClass.PhotoCaptured -= this.PhotoCapturedMain;
			CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapturedMain));
			CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailedAsync));
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			CameraControlClass.BreakCapturePhotoThread();
		}

		// Token: 0x06000381 RID: 897 RVA: 0x00013351 File Offset: 0x00011551
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.CancelPhotoEvent();
		}

		// Token: 0x06000382 RID: 898 RVA: 0x0001335C File Offset: 0x0001155C
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			if (WebcamControlClass.openCvCameraHelper._capture != null)
			{
				WebcamControlClass.LiveViewImageLoaded -= this.WebcamControlClass_LiveViewImageLoaded;
			}
			CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapturedMain));
			CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailedAsync));
			WebcamControlClass.PhotoCaptured -= this.PhotoCapturedMain;
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				BunnyCDNHelper.MediaLinkGenerated -= this.BunnyCDNHelper_MediaLinkGenerated;
				VariationMedia.ImagineJobCanceled += this.VariationMedia_ImagineJobCanceled;
			}
		}

		// Token: 0x06000383 RID: 899 RVA: 0x00013412 File Offset: 0x00011612
		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x06000384 RID: 900 RVA: 0x00013414 File Offset: 0x00011614
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRation = base.ActualWidth / 3840.0;
			string rotation = Settings.GetValueString("rotation") ?? "0";
			if (rotation.Contains("90") || rotation.Contains("270"))
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					ExtensionMethod.SetElementSizeToFitInsideArea(ref this.PhotoPanel, this.PhotoPanelContainer, 10.0, 16.0);
					return;
				}
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					ExtensionMethod.SetElementSizeToFitInsideArea(ref this.PhotoPanel, this.PhotoPanelContainer, (double)WebcamControlClass.CameraPreviewHeight, (double)WebcamControlClass.CameraPreviewWidth);
					return;
				}
			}
			else
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					ExtensionMethod.SetElementSizeToFitInsideArea(ref this.PhotoPanel, this.PhotoPanelContainer, 16.0, 10.0);
					return;
				}
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					ExtensionMethod.SetElementSizeToFitInsideArea(ref this.PhotoPanel, this.PhotoPanelContainer, (double)WebcamControlClass.CameraPreviewWidth, (double)WebcamControlClass.CameraPreviewHeight);
				}
			}
		}

		// Token: 0x04000364 RID: 868
		private bool closeButtonPressed;

		// Token: 0x04000365 RID: 869
		private bool isFirstWork = true;

		// Token: 0x04000366 RID: 870
		public static int elapsedSeconds;

		// Token: 0x04000367 RID: 871
		public static string resultFilePath;

		// Token: 0x04000368 RID: 872
		public static string CapturedFilePath;

		// Token: 0x04000369 RID: 873
		private int capturedPhotoNumber;

		// Token: 0x0400036A RID: 874
		private List<string> capturedFilePaths;

		// Token: 0x0400036B RID: 875
		private Timer timer;

		// Token: 0x0400036C RID: 876
		private float? remainingCountdownSeconds = new float?((float)8);

		// Token: 0x0400036D RID: 877
		private int? countdownSecondsLimit = new int?(8);

		// Token: 0x0400036E RID: 878
		private double originalFontSize = 108.0;

		// Token: 0x0400036F RID: 879
		private double reducedFontSize = 54.0;

		// Token: 0x04000370 RID: 880
		public static string lastCapturedPhoto;

		// Token: 0x04000371 RID: 881
		private bool isMirrorVideoEnabled;

		// Token: 0x04000372 RID: 882
		public static int funcNumber;

		// Token: 0x04000373 RID: 883
		private bool isFirstFrame1 = true;

		// Token: 0x04000374 RID: 884
		private bool isFirstFrame2 = true;
	}
}
