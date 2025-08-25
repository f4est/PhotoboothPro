using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000BC RID: 188
	public class Webcam
	{
		// Token: 0x1400002B RID: 43
		// (add) Token: 0x06000A57 RID: 2647 RVA: 0x0003C10C File Offset: 0x0003A30C
		// (remove) Token: 0x06000A58 RID: 2648 RVA: 0x0003C140 File Offset: 0x0003A340
		public static event Action<BitmapImage> CameraPhotoReceived;

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x06000A59 RID: 2649 RVA: 0x0003C174 File Offset: 0x0003A374
		// (remove) Token: 0x06000A5A RID: 2650 RVA: 0x0003C1A8 File Offset: 0x0003A3A8
		public static event Action<BitmapImage> CameraVideoReceived;

		// Token: 0x06000A5B RID: 2651 RVA: 0x0003C1DB File Offset: 0x0003A3DB
		public static void InitializeCamera()
		{
			Webcam.capCamera = new VideoCapture(0, VideoCaptureAPIs.ANY);
			CameraControlClass.LoadBackground(Settings.GetValueString("backgroundImage"));
		}

		// Token: 0x06000A5C RID: 2652 RVA: 0x0003C1F8 File Offset: 0x0003A3F8
		private void InitializeVideo()
		{
			string filePath = "D:\\temp\\200323.mp4";
			this.capVideo = new VideoCapture(filePath, VideoCaptureAPIs.ANY);
			this.sleepTime = (int)Math.Round(1000.0 / this.capVideo.Fps);
		}

		// Token: 0x06000A5D RID: 2653 RVA: 0x0003C239 File Offset: 0x0003A439
		public static void PlayCameraStart()
		{
			new Thread(new ThreadStart(Webcam.PlayCamera)).Start();
		}

		// Token: 0x06000A5E RID: 2654 RVA: 0x0003C254 File Offset: 0x0003A454
		private static void PlayCamera()
		{
			while (!Webcam.capCamera.IsDisposed)
			{
				Webcam.capCamera.Read(Webcam.matImage);
				if (Webcam.matImage.Empty())
				{
					break;
				}
				Webcam.matImage = ExtensionMethod.ColorKeyGreenEffect(Webcam.matImage, CameraControlClass.backgroundMat, CameraControlClass.trashOld);
				BitmapImage converted = ExtensionMethod.ConvertBitmap2BitmapSource(Webcam.matImage.ToBitmap());
				converted.Freeze();
				if (Webcam.CameraPhotoReceived != null)
				{
					Webcam.CameraPhotoReceived(converted);
				}
			}
		}

		// Token: 0x06000A5F RID: 2655 RVA: 0x0003C2CC File Offset: 0x0003A4CC
		private void PlayVideo()
		{
			while (!this.capVideo.IsDisposed)
			{
				this.capVideo.Read(Webcam.matImage);
				if (Webcam.matImage.Empty())
				{
					break;
				}
				Thread.Sleep(this.sleepTime);
				BitmapImage converted = ExtensionMethod.ConvertBitmap2BitmapSource(Webcam.matImage.ToBitmap());
				if (Webcam.CameraVideoReceived != null)
				{
					Webcam.CameraVideoReceived(converted);
				}
			}
		}

		// Token: 0x06000A60 RID: 2656 RVA: 0x0003C332 File Offset: 0x0003A532
		private void StopCamera(object sender, RoutedEventArgs e)
		{
			if (this.capVideo.IsOpened())
			{
				this.capVideo.Dispose();
			}
			if (Webcam.capCamera.IsOpened())
			{
				Webcam.capCamera.Dispose();
			}
		}

		// Token: 0x04000A18 RID: 2584
		private VideoCapture capVideo;

		// Token: 0x04000A19 RID: 2585
		private static VideoCapture capCamera;

		// Token: 0x04000A1C RID: 2588
		private int sleepTime;

		// Token: 0x04000A1D RID: 2589
		private static Mat matImage = new Mat();
	}
}
