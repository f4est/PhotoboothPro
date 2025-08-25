using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FFmpeg.NET.Events;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.WpfExtensions;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200003D RID: 61
	public partial class DSLRVideo : Page
	{
		// Token: 0x06000430 RID: 1072 RVA: 0x00017BDA File Offset: 0x00015DDA
		public DSLRVideo.RotationDegrees GetRotationDegrees(string degressString)
		{
			if (degressString.IndexOf("90") != -1)
			{
				return DSLRVideo.RotationDegrees.Rotate90;
			}
			if (degressString.IndexOf("180") != -1)
			{
				return DSLRVideo.RotationDegrees.Rotate180;
			}
			if (degressString.IndexOf("270") != -1)
			{
				return DSLRVideo.RotationDegrees.Rotate270;
			}
			return DSLRVideo.RotationDegrees.Rotate0;
		}

		// Token: 0x06000431 RID: 1073 RVA: 0x00017C10 File Offset: 0x00015E10
		public DSLRVideo()
		{
			this.InitializeComponent();
		}

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000432 RID: 1074 RVA: 0x00017CD8 File Offset: 0x00015ED8
		// (remove) Token: 0x06000433 RID: 1075 RVA: 0x00017D10 File Offset: 0x00015F10
		private event Action EndVideoEvent;

		// Token: 0x06000434 RID: 1076 RVA: 0x00017D45 File Offset: 0x00015F45
		private void ADSVideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
			this.isMirrorVideoEnabled = false;
		}

		// Token: 0x06000435 RID: 1077 RVA: 0x00017D70 File Offset: 0x00015F70
		private void ManageMirrorBoothVideo()
		{
			string videoPath = Settings.GetValueString("waitvideo") ?? "0.xyz";
			bool isVideoEnabled = Settings.GetValueBoolean("waitvideocheck").GetValueOrDefault();
			if (File.Exists(videoPath) && isVideoEnabled)
			{
				this.isMirrorVideoEnabled = true;
				this.videoPlayer.MediaEnded += this.ADSVideoPlayer_MediaEnded;
				this.videoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
				this.adsVideoGrid.Visibility = Visibility.Visible;
				this.videoPlayer.Volume = 100.0;
				return;
			}
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
		}

		// Token: 0x06000436 RID: 1078 RVA: 0x00017E20 File Offset: 0x00016020
		private void Page_LoadedAsync(object sender, RoutedEventArgs e)
		{
			DSLRVideo.<Page_LoadedAsync>d__50 <Page_LoadedAsync>d__;
			<Page_LoadedAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_LoadedAsync>d__.<>4__this = this;
			<Page_LoadedAsync>d__.<>1__state = -1;
			<Page_LoadedAsync>d__.<>t__builder.Start<DSLRVideo.<Page_LoadedAsync>d__50>(ref <Page_LoadedAsync>d__);
		}

		// Token: 0x06000437 RID: 1079 RVA: 0x00017E58 File Offset: 0x00016058
		private Task StartCountdown()
		{
			DSLRVideo.<StartCountdown>d__51 <StartCountdown>d__;
			<StartCountdown>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartCountdown>d__.<>4__this = this;
			<StartCountdown>d__.<>1__state = -1;
			<StartCountdown>d__.<>t__builder.Start<DSLRVideo.<StartCountdown>d__51>(ref <StartCountdown>d__);
			return <StartCountdown>d__.<>t__builder.Task;
		}

		// Token: 0x06000438 RID: 1080 RVA: 0x00017E9B File Offset: 0x0001609B
		private void SkipVideoButton_Click(object sender, MouseButtonEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
		}

		// Token: 0x06000439 RID: 1081 RVA: 0x00017EC0 File Offset: 0x000160C0
		private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				string rotation = Settings.GetValueString("rotation") ?? "0";
				if (rotation.Contains("90"))
				{
					eventArgs.Frame.ToBitmap().RotateFlip(RotateFlipType.Rotate90FlipNone);
				}
				else if (rotation.Contains("180"))
				{
					eventArgs.Frame.ToBitmap().RotateFlip(RotateFlipType.Rotate180FlipNone);
				}
				else if (rotation.Contains("270"))
				{
					eventArgs.Frame.ToBitmap().RotateFlip(RotateFlipType.Rotate270FlipNone);
				}
				this.LiveViewImage.Source = eventArgs.Frame.ToBitmapSource();
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600043A RID: 1082 RVA: 0x00017F07 File Offset: 0x00016107
		private void VideoCapturedFailed(string errorName)
		{
			this.CancelVideoEvent();
		}

		// Token: 0x0600043B RID: 1083 RVA: 0x00017F10 File Offset: 0x00016110
		private void DSLRVideo_EndVideoEvent()
		{
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
			{
				CameraControlClass.StopRecordVideo();
			}
			DispatcherOperation ope = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.RecordingAnimContainer.Visibility = Visibility.Collapsed;
				this.WaitingGrid.Visibility = Visibility.Visible;
				this.WaitingContainer.Visibility = Visibility.Visible;
				this.WaitingGrid.Visibility = Visibility.Visible;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x0600043C RID: 1084 RVA: 0x00017F48 File Offset: 0x00016148
		private void VideoCapture(string filepath)
		{
			DSLRVideo.<>c__DisplayClass56_0 CS$<>8__locals1 = new DSLRVideo.<>c__DisplayClass56_0();
			CS$<>8__locals1.filepath = filepath;
			CS$<>8__locals1.<>4__this = this;
			if (this.count == 0)
			{
				this.count++;
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					DSLRVideo.<>c__DisplayClass56_0.<<VideoCapture>b__0>d <<VideoCapture>b__0>d;
					<<VideoCapture>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<VideoCapture>b__0>d.<>4__this = CS$<>8__locals1;
					<<VideoCapture>b__0>d.<>1__state = -1;
					<<VideoCapture>b__0>d.<>t__builder.Start<DSLRVideo.<>c__DisplayClass56_0.<<VideoCapture>b__0>d>(ref <<VideoCapture>b__0>d);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
				return;
			}
			this.count++;
		}

		// Token: 0x0600043D RID: 1085 RVA: 0x00017FB4 File Offset: 0x000161B4
		private void Ffmpeg_Error(object sender, ConversionErrorEventArgs e)
		{
			DSLRVideo.<Ffmpeg_Error>d__57 <Ffmpeg_Error>d__;
			<Ffmpeg_Error>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Ffmpeg_Error>d__.<>4__this = this;
			<Ffmpeg_Error>d__.e = e;
			<Ffmpeg_Error>d__.<>1__state = -1;
			<Ffmpeg_Error>d__.<>t__builder.Start<DSLRVideo.<Ffmpeg_Error>d__57>(ref <Ffmpeg_Error>d__);
		}

		// Token: 0x0600043E RID: 1086 RVA: 0x00017FF4 File Offset: 0x000161F4
		private void SlowVideoFunctions(string path)
		{
			DSLRVideo.<>c__DisplayClass58_0 CS$<>8__locals1 = new DSLRVideo.<>c__DisplayClass58_0();
			CS$<>8__locals1.path = path;
			CS$<>8__locals1.<>4__this = this;
			if (this.slowVideoStep == 0)
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					DSLRVideo.<>c__DisplayClass58_0.<<SlowVideoFunctions>b__0>d <<SlowVideoFunctions>b__0>d;
					<<SlowVideoFunctions>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<SlowVideoFunctions>b__0>d.<>4__this = CS$<>8__locals1;
					<<SlowVideoFunctions>b__0>d.<>1__state = -1;
					<<SlowVideoFunctions>b__0>d.<>t__builder.Start<DSLRVideo.<>c__DisplayClass58_0.<<SlowVideoFunctions>b__0>d>(ref <<SlowVideoFunctions>b__0>d);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
			}
		}

		// Token: 0x0600043F RID: 1087 RVA: 0x00018044 File Offset: 0x00016244
		public Task ChangeSharingPage(string videoPath)
		{
			DSLRVideo.<ChangeSharingPage>d__59 <ChangeSharingPage>d__;
			<ChangeSharingPage>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ChangeSharingPage>d__.<>4__this = this;
			<ChangeSharingPage>d__.videoPath = videoPath;
			<ChangeSharingPage>d__.<>1__state = -1;
			<ChangeSharingPage>d__.<>t__builder.Start<DSLRVideo.<ChangeSharingPage>d__59>(ref <ChangeSharingPage>d__);
			return <ChangeSharingPage>d__.<>t__builder.Task;
		}

		// Token: 0x06000440 RID: 1088 RVA: 0x00018090 File Offset: 0x00016290
		private void Ffmpeg_Complete(object sendera, ConversionCompleteEventArgs ea)
		{
			DSLRVideo.<Ffmpeg_Complete>d__60 <Ffmpeg_Complete>d__;
			<Ffmpeg_Complete>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Ffmpeg_Complete>d__.<>4__this = this;
			<Ffmpeg_Complete>d__.<>1__state = -1;
			<Ffmpeg_Complete>d__.<>t__builder.Start<DSLRVideo.<Ffmpeg_Complete>d__60>(ref <Ffmpeg_Complete>d__);
		}

		// Token: 0x06000441 RID: 1089 RVA: 0x000180C8 File Offset: 0x000162C8
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

		// Token: 0x06000442 RID: 1090 RVA: 0x000180FC File Offset: 0x000162FC
		private void AnimationFinished()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.CountdownContainer.Visibility = Visibility.Collapsed;
				this.RecordingAnimContainer.Visibility = Visibility.Visible;
			}), DispatcherPriority.Normal, Array.Empty<object>());
			if (!this.isFirstWork)
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					CameraControlClass.RecordVideo();
				}
				else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					WebcamControlClass.TakeVideo(this.VideoTime);
				}
			}
			new Thread(delegate()
			{
				Thread.Sleep(this.VideoTime * 1000);
				if (!this.isFirstWork)
				{
					this.EndVideoEvent();
				}
			}).Start();
		}

		// Token: 0x06000443 RID: 1091 RVA: 0x00018168 File Offset: 0x00016368
		private void CancelVideoEvent()
		{
			this.closeButtonPressed = true;
			LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			CameraControlClass.VideoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.VideoCapturedEvent, new Action<string>(this.VideoCapture));
			CameraControlClass.ffmpeg.Complete -= this.Ffmpeg_Complete;
			CameraControlClass.ffmpeg.Error -= this.Ffmpeg_Error;
			this.isFirstWork = true;
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			CameraControlClass.BreakCaptureVideoThread();
		}

		// Token: 0x06000444 RID: 1092 RVA: 0x0001821E File Offset: 0x0001641E
		private void CancelVideoEvent(MessageBoxWindow.MessageBoxReturn @return)
		{
			this.CancelVideoEvent();
			switch (@return)
			{
			default:
				return;
			}
		}

		// Token: 0x06000445 RID: 1093 RVA: 0x00018238 File Offset: 0x00016438
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.CancelVideoEvent();
		}

		// Token: 0x06000446 RID: 1094 RVA: 0x00018240 File Offset: 0x00016440
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.isActive = false;
			CameraControlClass.isVideo = false;
			LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			CameraControlClass.VideoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.VideoCapturedEvent, new Action<string>(this.VideoCapture));
			CameraControlClass.ffmpeg.Complete -= this.Ffmpeg_Complete;
			CameraControlClass.ffmpeg.Error -= this.Ffmpeg_Error;
			this.EndVideoEvent -= this.DSLRVideo_EndVideoEvent;
			this.isFirstWork = true;
		}

		// Token: 0x06000447 RID: 1095 RVA: 0x000182D0 File Offset: 0x000164D0
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

		// Token: 0x06000448 RID: 1096 RVA: 0x000183CC File Offset: 0x000165CC
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

		// Token: 0x06000449 RID: 1097 RVA: 0x0001844C File Offset: 0x0001664C
		private void CountDownGif_MediaEnded()
		{
			if (!this.isFirstWork)
			{
				this.CountdownContainer.Visibility = Visibility.Collapsed;
				this.RecordingAnimContainer.Visibility = Visibility.Visible;
				this.countDownText.Visibility = Visibility.Collapsed;
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					CameraControlClass.RecordVideo();
				}
				else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					WebcamControlClass.TakeVideo(this.VideoTime);
				}
			}
			new Thread(delegate()
			{
				Thread.Sleep(this.VideoTime * 1000);
				if (!this.isFirstWork)
				{
					this.EndVideoEvent();
				}
			}).Start();
		}

		// Token: 0x0600044A RID: 1098 RVA: 0x000184BC File Offset: 0x000166BC
		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x0400042A RID: 1066
		private bool closeButtonPressed;

		// Token: 0x0400042B RID: 1067
		private bool isActive;

		// Token: 0x0400042C RID: 1068
		private bool isFirstWork = true;

		// Token: 0x0400042D RID: 1069
		public static string CapturedVideoPath;

		// Token: 0x0400042E RID: 1070
		public static string CapturedSlowVideoPath;

		// Token: 0x0400042F RID: 1071
		public static string CapturedFinalVideoPath;

		// Token: 0x04000430 RID: 1072
		public static string CapturedFinalRenderVideoPath;

		// Token: 0x04000431 RID: 1073
		public static string CapturedFinalRenderOptimizeVideoPath;

		// Token: 0x04000432 RID: 1074
		private static string CapturedRotatePath;

		// Token: 0x04000433 RID: 1075
		public static string CapturedScalePath;

		// Token: 0x04000434 RID: 1076
		private static string CapturedCropPath;

		// Token: 0x04000435 RID: 1077
		private static string CapturedWithMusicPath;

		// Token: 0x04000436 RID: 1078
		private static string CapturedMirrorPath;

		// Token: 0x04000437 RID: 1079
		private static string CapturedWithFPSSLowPath;

		// Token: 0x04000438 RID: 1080
		public static string VideoThumbPath;

		// Token: 0x04000439 RID: 1081
		private string blackVideoPath;

		// Token: 0x0400043A RID: 1082
		private string blackVideoResizedPath;

		// Token: 0x0400043B RID: 1083
		private string TwoVideoPath;

		// Token: 0x0400043C RID: 1084
		private bool isFPSSlowed;

		// Token: 0x0400043D RID: 1085
		private bool isMusicEnabled;

		// Token: 0x0400043E RID: 1086
		private bool isSlowVideo;

		// Token: 0x0400043F RID: 1087
		private int VideoTime = 6;

		// Token: 0x04000440 RID: 1088
		private string MusicPath = "";

		// Token: 0x04000441 RID: 1089
		private bool isFinishMusicComp;

		// Token: 0x04000442 RID: 1090
		private bool isHaveTemplate;

		// Token: 0x04000443 RID: 1091
		private int completeStep;

		// Token: 0x04000444 RID: 1092
		private int count;

		// Token: 0x04000445 RID: 1093
		private int slowVideoStep;

		// Token: 0x04000446 RID: 1094
		private string executeFfmpegString;

		// Token: 0x04000447 RID: 1095
		private string executeFfmpegCropString;

		// Token: 0x04000448 RID: 1096
		private float SlowVideoGain = 4f;

		// Token: 0x04000449 RID: 1097
		private Rectangle capturedVideoRectAngle;

		// Token: 0x0400044A RID: 1098
		[NonSerialized]
		private VideoWriter _writer = new VideoWriter();

		// Token: 0x0400044B RID: 1099
		private List<double> VideoSecondsList = new List<double>
		{
			2.0,
			4.0,
			6.0
		};

		// Token: 0x0400044C RID: 1100
		private bool isMirrorVideoEnabled;

		// Token: 0x0400044D RID: 1101
		private bool isCaptureMirrorVideoEnabled;

		// Token: 0x0400044E RID: 1102
		public static int FfmpegCRFValue = 10;

		// Token: 0x0400044F RID: 1103
		private double fps = 30.0;

		// Token: 0x04000450 RID: 1104
		private float? remainingCountdownSeconds = new float?((float)5);

		// Token: 0x04000451 RID: 1105
		private int? countdownSecondsLimit = new int?(5);

		// Token: 0x04000452 RID: 1106
		private double originalFontSize = 108.0;

		// Token: 0x04000453 RID: 1107
		private double reducedFontSize = 54.0;

		// Token: 0x0200019F RID: 415
		public enum RotationDegrees
		{
			// Token: 0x04000D52 RID: 3410
			Rotate0,
			// Token: 0x04000D53 RID: 3411
			Rotate90,
			// Token: 0x04000D54 RID: 3412
			Rotate180,
			// Token: 0x04000D55 RID: 3413
			Rotate270
		}
	}
}
