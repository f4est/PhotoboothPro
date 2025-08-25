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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using FFmpeg.NET.Events;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Services;
using OpenCvSharp;
using Vlc.DotNet.Wpf;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200002E RID: 46
	public partial class GoProVideo : Page
	{
		// Token: 0x06000290 RID: 656 RVA: 0x0000D363 File Offset: 0x0000B563
		public GoProVideo.RotationDegrees GetRotationDegrees(string degressString)
		{
			if (degressString.IndexOf("90") != -1)
			{
				return GoProVideo.RotationDegrees.Rotate90;
			}
			if (degressString.IndexOf("180") != -1)
			{
				return GoProVideo.RotationDegrees.Rotate180;
			}
			if (degressString.IndexOf("270") != -1)
			{
				return GoProVideo.RotationDegrees.Rotate270;
			}
			return GoProVideo.RotationDegrees.Rotate0;
		}

		// Token: 0x06000291 RID: 657 RVA: 0x0000D398 File Offset: 0x0000B598
		public GoProVideo()
		{
			this.InitializeComponent();
		}

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000292 RID: 658 RVA: 0x0000D450 File Offset: 0x0000B650
		// (remove) Token: 0x06000293 RID: 659 RVA: 0x0000D488 File Offset: 0x0000B688
		private event Action EndVideoEvent;

		// Token: 0x06000294 RID: 660 RVA: 0x0000D4C0 File Offset: 0x0000B6C0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			CameraControlClass.FolderCheck();
			this.isFinishMusicComp = false;
			this.isActive = true;
			this.isFPSSlowed = false;
			this.VideoSecondsList = new List<double>
			{
				GoProCameraControlClass.slowMotionVideoStart,
				GoProCameraControlClass.slowMotionVideoEnd,
				(double)GoProCameraControlClass.videoLength
			};
			this.isHaveTemplate = TemplateClass.IsHaveImageTemplate(false);
			CameraControlClass.isVideo = true;
			this.isMusicEnabled = (Settings.GetValueBoolean("musiccheck") != null && Settings.GetValueBoolean("musiccheck").Value);
			if (this.isMusicEnabled)
			{
				this.MusicPath = Settings.GetValueString("musicpath");
			}
			this.slowVideoStep = 0;
			this.count = 0;
			this.countDownText.Text = "";
			this.countdownSecondsLimit = Settings.GetValueInt("countdownseconds") * 1000;
			this.remainingCountdownSeconds = this.countdownSecondsLimit / 1000;
			this.WaitingGrid.Visibility = Visibility.Collapsed;
			if (this.isFirstWork)
			{
				CameraControlClass.ffmpeg.Complete += this.Ffmpeg_Complete;
				CameraControlClass.ffmpeg.Error += this.Ffmpeg_Error;
				if (!DSLR.isImportFile)
				{
					GoProCameraControlClass.CaptureFile = (Action<string>)Delegate.Combine(GoProCameraControlClass.CaptureFile, new Action<string>(this.VideoCapture));
					GoProCameraControlClass.VideoCapturedFailedEvent = (Action<string>)Delegate.Combine(GoProCameraControlClass.VideoCapturedFailedEvent, new Action<string>(this.VideoCapturedFailed));
					this.EndVideoEvent += this.DSLRVideo_EndVideoEvent;
					GoProCameraControlClass.PreviewStart();
				}
				this.isFirstWork = false;
			}
			this.LiveViewImagePlayer.Visibility = Visibility.Visible;
			this.CountdownContainer.Visibility = Visibility.Visible;
			this.countDownText.Visibility = Visibility.Visible;
			this.timer = new DispatcherTimer();
			this.timer.Interval = TimeSpan.FromSeconds(1.0);
			this.timer.Tick += this.Timer_Tick;
			this.timer.Start();
			if (DSLR.isImportFile)
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.CountdownContainer.Visibility = Visibility.Collapsed;
					this.RecordingContainer.Visibility = Visibility.Visible;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				DispatcherOperation ope = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.RecordingContainer.Visibility = Visibility.Collapsed;
					this.WaitingGrid.Visibility = Visibility.Visible;
					this.WaitingContainer.Visibility = Visibility.Visible;
					this.WaitingGrid.Visibility = Visibility.Visible;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				this.VideoCapture(DSLR.importFilePaths[0]);
			}
			this.ManagePortraitScreen();
			if (Motion360Platform.Ready)
			{
				this.EmergencyStopButton.Visibility = Visibility.Visible;
				return;
			}
			this.EmergencyStopButton.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000295 RID: 661 RVA: 0x0000D784 File Offset: 0x0000B984
		private void Timer_Tick(object sender, EventArgs e)
		{
			int? num = this.remainingCountdownSeconds;
			int num2 = 0;
			if (num.GetValueOrDefault() <= num2 & num != null)
			{
				this.timer.Stop();
				this.AnimationFinished();
			}
			else
			{
				this.countDownText.Text = this.remainingCountdownSeconds.ToString();
				this.AnimateFontSize(this.countDownText, this.originalFontSize, this.reducedFontSize);
			}
			this.remainingCountdownSeconds--;
		}

		// Token: 0x06000296 RID: 662 RVA: 0x0000D828 File Offset: 0x0000BA28
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
			new Storyboard
			{
				Children = 
				{
					fontSizeAnimation
				}
			}.Begin();
		}

		// Token: 0x06000297 RID: 663 RVA: 0x0000D89B File Offset: 0x0000BA9B
		private void VideoCapturedFailed(string errorName)
		{
			this.CancelButton_Click(null, null);
		}

		// Token: 0x06000298 RID: 664 RVA: 0x0000D8A8 File Offset: 0x0000BAA8
		private void DSLRVideo_EndVideoEvent()
		{
			GoProCameraControlClass.GoProShutterOff_Click(false, Path.Combine(CameraControlClass.FolderForPhotos, DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_fffffff") + ".mp4"));
			DispatcherOperation ope = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.RecordingContainer.Visibility = Visibility.Collapsed;
				this.WaitingGrid.Visibility = Visibility.Visible;
				this.WaitingContainer.Visibility = Visibility.Visible;
				this.WaitingGrid.Visibility = Visibility.Visible;
			}), DispatcherPriority.Normal, Array.Empty<object>());
			Motion360Platform.StopMotion();
		}

		// Token: 0x06000299 RID: 665 RVA: 0x0000D908 File Offset: 0x0000BB08
		private void VideoCapture(string filepath)
		{
			GoProVideo.<>c__DisplayClass48_0 CS$<>8__locals1 = new GoProVideo.<>c__DisplayClass48_0();
			CS$<>8__locals1.filepath = filepath;
			CS$<>8__locals1.<>4__this = this;
			if (this.count == 0)
			{
				this.count++;
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					GoProVideo.<>c__DisplayClass48_0.<<VideoCapture>b__0>d <<VideoCapture>b__0>d;
					<<VideoCapture>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<VideoCapture>b__0>d.<>4__this = CS$<>8__locals1;
					<<VideoCapture>b__0>d.<>1__state = -1;
					<<VideoCapture>b__0>d.<>t__builder.Start<GoProVideo.<>c__DisplayClass48_0.<<VideoCapture>b__0>d>(ref <<VideoCapture>b__0>d);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
				return;
			}
			this.count++;
		}

		// Token: 0x0600029A RID: 666 RVA: 0x0000D974 File Offset: 0x0000BB74
		private void Ffmpeg_Error(object sender, ConversionErrorEventArgs e)
		{
			GoProVideo.<Ffmpeg_Error>d__49 <Ffmpeg_Error>d__;
			<Ffmpeg_Error>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Ffmpeg_Error>d__.<>4__this = this;
			<Ffmpeg_Error>d__.e = e;
			<Ffmpeg_Error>d__.<>1__state = -1;
			<Ffmpeg_Error>d__.<>t__builder.Start<GoProVideo.<Ffmpeg_Error>d__49>(ref <Ffmpeg_Error>d__);
		}

		// Token: 0x0600029B RID: 667 RVA: 0x0000D9B3 File Offset: 0x0000BBB3
		private void CancelGoProVideoEvent(MessageBoxWindow.MessageBoxReturn @return)
		{
			this.CancelGoProVideoEvent();
			switch (@return)
			{
			default:
				return;
			}
		}

		// Token: 0x0600029C RID: 668 RVA: 0x0000D9D0 File Offset: 0x0000BBD0
		private void SlowVideoFunctions(string path, int addStep = 0)
		{
			GoProVideo.<>c__DisplayClass51_0 CS$<>8__locals1 = new GoProVideo.<>c__DisplayClass51_0();
			CS$<>8__locals1.path = path;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.addStep = addStep;
			if (this.slowVideoStep == 0)
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					GoProVideo.<>c__DisplayClass51_0.<<SlowVideoFunctions>b__0>d <<SlowVideoFunctions>b__0>d;
					<<SlowVideoFunctions>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<SlowVideoFunctions>b__0>d.<>4__this = CS$<>8__locals1;
					<<SlowVideoFunctions>b__0>d.<>1__state = -1;
					<<SlowVideoFunctions>b__0>d.<>t__builder.Start<GoProVideo.<>c__DisplayClass51_0.<<SlowVideoFunctions>b__0>d>(ref <<SlowVideoFunctions>b__0>d);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
			}
		}

		// Token: 0x0600029D RID: 669 RVA: 0x0000DA28 File Offset: 0x0000BC28
		private void Ffmpeg_Complete(object sender, ConversionCompleteEventArgs e)
		{
			GoProVideo.<Ffmpeg_Complete>d__52 <Ffmpeg_Complete>d__;
			<Ffmpeg_Complete>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Ffmpeg_Complete>d__.<>4__this = this;
			<Ffmpeg_Complete>d__.<>1__state = -1;
			<Ffmpeg_Complete>d__.<>t__builder.Start<GoProVideo.<Ffmpeg_Complete>d__52>(ref <Ffmpeg_Complete>d__);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000DA60 File Offset: 0x0000BC60
		public Task ChangeSharingPage(string videoPath, int addStep = 0)
		{
			GoProVideo.<ChangeSharingPage>d__53 <ChangeSharingPage>d__;
			<ChangeSharingPage>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ChangeSharingPage>d__.<>4__this = this;
			<ChangeSharingPage>d__.videoPath = videoPath;
			<ChangeSharingPage>d__.addStep = addStep;
			<ChangeSharingPage>d__.<>1__state = -1;
			<ChangeSharingPage>d__.<>t__builder.Start<GoProVideo.<ChangeSharingPage>d__53>(ref <ChangeSharingPage>d__);
			return <ChangeSharingPage>d__.<>t__builder.Task;
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000DAB4 File Offset: 0x0000BCB4
		private void AnimationFinished()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.CountdownContainer.Visibility = Visibility.Collapsed;
				this.countDownText.Visibility = Visibility.Collapsed;
				this.RecordingContainer.Visibility = Visibility.Visible;
			}), DispatcherPriority.Normal, Array.Empty<object>());
			if (!this.isFirstWork)
			{
				GoProCameraControlClass.GoProShutterOn();
			}
			new Thread(delegate()
			{
				Thread.Sleep((int)(this.VideoSecondsList[2] * 1000.0));
				if (!this.isFirstWork)
				{
					Action endVideoEvent = this.EndVideoEvent;
					if (endVideoEvent == null)
					{
						return;
					}
					endVideoEvent();
				}
			}).Start();
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000DB04 File Offset: 0x0000BD04
		private void CancelGoProVideoEvent()
		{
			GoProCameraControlClass.CaptureFile = (Action<string>)Delegate.Remove(GoProCameraControlClass.CaptureFile, new Action<string>(this.VideoCapture));
			CameraControlClass.ffmpeg.Complete -= this.Ffmpeg_Complete;
			CameraControlClass.ffmpeg.Error -= this.Ffmpeg_Error;
			this.isFirstWork = true;
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			if (GoProCameraControlClass.isRecord)
			{
				GoProCameraControlClass.GoProShutterOff_Click(true, "");
			}
		}

		// Token: 0x060002A1 RID: 673 RVA: 0x0000DBAF File Offset: 0x0000BDAF
		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			this.CancelGoProVideoEvent();
		}

		// Token: 0x060002A2 RID: 674 RVA: 0x0000DBB8 File Offset: 0x0000BDB8
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.isActive = false;
			CameraControlClass.isVideo = false;
			GoProCameraControlClass.CaptureFile = (Action<string>)Delegate.Remove(GoProCameraControlClass.CaptureFile, new Action<string>(this.VideoCapture));
			CameraControlClass.ffmpeg.Complete -= this.Ffmpeg_Complete;
			CameraControlClass.ffmpeg.Error -= this.Ffmpeg_Error;
			this.EndVideoEvent -= this.DSLRVideo_EndVideoEvent;
			this.isFirstWork = true;
		}

		// Token: 0x060002A3 RID: 675 RVA: 0x0000DC38 File Offset: 0x0000BE38
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRation = base.ActualWidth / 3840.0;
			if (isPortraitScreen)
			{
				this.gridMain.RowDefinitions[0].Height = new GridLength(180.0, GridUnitType.Star);
				this.gridMain.RowDefinitions[this.gridMain.RowDefinitions.Count - 1].Height = new GridLength(180.0, GridUnitType.Star);
			}
			else
			{
				this.gridMain.RowDefinitions[0].Height = new GridLength(8.0, GridUnitType.Star);
				this.gridMain.RowDefinitions[this.gridMain.RowDefinitions.Count - 1].Height = new GridLength(8.0, GridUnitType.Star);
			}
			string rotation = Settings.GetValueString("rotation") ?? "0";
			if (rotation.Contains("90") || rotation.Contains("270"))
			{
				this.PhotoPanel.Width = this.PhotoPanel.ActualHeight * 0.66;
				return;
			}
			this.PhotoPanel.Height = this.PhotoPanel.ActualWidth * 0.66;
		}

		// Token: 0x060002A4 RID: 676 RVA: 0x0000DD8F File Offset: 0x0000BF8F
		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x060002A5 RID: 677 RVA: 0x0000DD97 File Offset: 0x0000BF97
		private void EmergencyStopButton_Click(object sender, RoutedEventArgs e)
		{
			Motion360Platform.EmergencyStopMotion();
		}

		// Token: 0x04000223 RID: 547
		private bool isActive;

		// Token: 0x04000224 RID: 548
		private bool isFirstWork = true;

		// Token: 0x04000225 RID: 549
		public static string CapturedVideoPath;

		// Token: 0x04000226 RID: 550
		public static string CapturedSlowVideoPath;

		// Token: 0x04000227 RID: 551
		public static string CapturedFinalVideoPath;

		// Token: 0x04000228 RID: 552
		public static string CapturedFinalRenderVideoPath;

		// Token: 0x04000229 RID: 553
		public static string CapturedFinalRenderOptimizeVideoPath;

		// Token: 0x0400022A RID: 554
		private static string CapturedRotatePath;

		// Token: 0x0400022B RID: 555
		public static string CapturedScalePath;

		// Token: 0x0400022C RID: 556
		private static string CapturedCropPath;

		// Token: 0x0400022D RID: 557
		private static string CapturedWithMusicPath;

		// Token: 0x0400022E RID: 558
		public static string CapturedWithFPSSLowPath;

		// Token: 0x0400022F RID: 559
		public static string VideoThumbPath;

		// Token: 0x04000230 RID: 560
		private string[] videoPartPaths;

		// Token: 0x04000231 RID: 561
		private string blackVideoPath;

		// Token: 0x04000232 RID: 562
		private string blackVideoResizedPath;

		// Token: 0x04000233 RID: 563
		private string TwoVideoPath;

		// Token: 0x04000234 RID: 564
		private bool isHaveTemplate;

		// Token: 0x04000235 RID: 565
		private int completeStep;

		// Token: 0x04000236 RID: 566
		private int count;

		// Token: 0x04000237 RID: 567
		private bool isMusicEnabled;

		// Token: 0x04000238 RID: 568
		private string MusicPath = "";

		// Token: 0x04000239 RID: 569
		private bool isFinishMusicComp;

		// Token: 0x0400023A RID: 570
		private string executeFfmpegString;

		// Token: 0x0400023B RID: 571
		private string executeFfmpegCropString;

		// Token: 0x0400023C RID: 572
		private Rectangle capturedVideoRectAngle;

		// Token: 0x0400023D RID: 573
		private int slowVideoStep;

		// Token: 0x0400023E RID: 574
		private List<double> VideoSecondsList = new List<double>
		{
			2.0,
			4.0,
			6.0
		};

		// Token: 0x0400023F RID: 575
		private bool isFPSSlowed;

		// Token: 0x04000240 RID: 576
		private int fps = 120;

		// Token: 0x04000241 RID: 577
		private int? remainingCountdownSeconds = new int?(8);

		// Token: 0x04000242 RID: 578
		private int? countdownSecondsLimit = new int?(8);

		// Token: 0x04000243 RID: 579
		private float SlowVideoGain = 4f;

		// Token: 0x04000244 RID: 580
		private DispatcherTimer timer;

		// Token: 0x04000245 RID: 581
		private double originalFontSize = 108.0;

		// Token: 0x04000246 RID: 582
		private double reducedFontSize = 54.0;

		// Token: 0x04000247 RID: 583
		[NonSerialized]
		private VideoWriter _writer = new VideoWriter();

		// Token: 0x02000143 RID: 323
		public enum RotationDegrees
		{
			// Token: 0x04000C11 RID: 3089
			Rotate0,
			// Token: 0x04000C12 RID: 3090
			Rotate90,
			// Token: 0x04000C13 RID: 3091
			Rotate180,
			// Token: 0x04000C14 RID: 3092
			Rotate270
		}
	}
}
