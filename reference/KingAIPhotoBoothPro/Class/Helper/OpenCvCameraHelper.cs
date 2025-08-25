using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using OpenCvSharp;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D6 RID: 214
	public class OpenCvCameraHelper
	{
		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06000B3F RID: 2879 RVA: 0x00042980 File Offset: 0x00040B80
		// (remove) Token: 0x06000B40 RID: 2880 RVA: 0x000429B8 File Offset: 0x00040BB8
		public event EventHandler<NewFrameEventArgs> NewFrame;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06000B41 RID: 2881 RVA: 0x000429F0 File Offset: 0x00040BF0
		// (remove) Token: 0x06000B42 RID: 2882 RVA: 0x00042A28 File Offset: 0x00040C28
		public event EventHandler<string> ErrorOccurred;

		// Token: 0x06000B43 RID: 2883 RVA: 0x00042A60 File Offset: 0x00040C60
		public bool OpenCamera(int cameraID, int tryCount = 0)
		{
			if (this._capture.IsOpened())
			{
				return true;
			}
			tryCount++;
			switch (tryCount)
			{
			case 1:
				this._capture = new VideoCapture(cameraID, VideoCaptureAPIs.MSMF);
				this.selectedApi = VideoCaptureAPIs.MSMF;
				return this.OpenCamera(cameraID, tryCount);
			case 2:
				this._capture = new VideoCapture(cameraID, VideoCaptureAPIs.GSTREAMER);
				this.selectedApi = VideoCaptureAPIs.GSTREAMER;
				return this.OpenCamera(cameraID, tryCount);
			case 3:
				this._capture = new VideoCapture(cameraID, VideoCaptureAPIs.WINRT);
				this.selectedApi = VideoCaptureAPIs.WINRT;
				return this.OpenCamera(cameraID, tryCount);
			case 4:
				this._capture = new VideoCapture(cameraID, VideoCaptureAPIs.DSHOW);
				this.selectedApi = VideoCaptureAPIs.DSHOW;
				return this.OpenCamera(cameraID, tryCount);
			case 5:
				this._capture = new VideoCapture(cameraID, VideoCaptureAPIs.ANY);
				this.selectedApi = VideoCaptureAPIs.ANY;
				return this.OpenCamera(cameraID, tryCount);
			case 6:
				return false;
			default:
				return false;
			}
		}

		// Token: 0x06000B44 RID: 2884 RVA: 0x00042B5C File Offset: 0x00040D5C
		public Task Start(int cameraId, VideoCaptureAPIs decodeApi)
		{
			OpenCvCameraHelper.<Start>d__12 <Start>d__;
			<Start>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.cameraId = cameraId;
			<Start>d__.decodeApi = decodeApi;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<OpenCvCameraHelper.<Start>d__12>(ref <Start>d__);
			return <Start>d__.<>t__builder.Task;
		}

		// Token: 0x06000B45 RID: 2885 RVA: 0x00042BAF File Offset: 0x00040DAF
		public void Stop()
		{
			this._isRunning = false;
			VideoCapture capture = this._capture;
			if (capture != null)
			{
				capture.Release();
			}
			VideoCapture capture2 = this._capture;
			if (capture2 != null)
			{
				capture2.Dispose();
			}
			this._capture = null;
		}

		// Token: 0x06000B46 RID: 2886 RVA: 0x00042BE4 File Offset: 0x00040DE4
		private void CaptureLoop()
		{
			try
			{
				using (Mat frame = new Mat())
				{
					while (this._isRunning)
					{
						this._capture.Read(frame);
						if (!frame.Empty())
						{
							EventHandler<NewFrameEventArgs> newFrame = this.NewFrame;
							if (newFrame != null)
							{
								newFrame(this, new NewFrameEventArgs(frame.Clone()));
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				EventHandler<string> errorOccurred = this.ErrorOccurred;
				if (errorOccurred != null)
				{
					errorOccurred(this, "Frame işleme hatası: " + ex.Message);
				}
				if (ActivationKingWindow.CurrentPage == ApplicationPage.VideoShootPage || ActivationKingWindow.CurrentPage == ApplicationPage.PhotoShootPage)
				{
					MessageBoxWindow.CreateWindow("C++ Redistributable Not Found", "Please install Redistributable via link.\nhttps://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170#visual-studio-2015-2017-2019-and-2022\n" + ex.Message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				}
			}
		}

		// Token: 0x04000A6A RID: 2666
		public VideoCapture _capture;

		// Token: 0x04000A6B RID: 2667
		private VideoCaptureAPIs selectedApi;

		// Token: 0x04000A6C RID: 2668
		public static double fps;

		// Token: 0x04000A6D RID: 2669
		private Thread _cameraThread;

		// Token: 0x04000A6E RID: 2670
		public bool _isRunning;
	}
}
