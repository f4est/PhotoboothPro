using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;
using System.Windows.Media;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B4 RID: 180
	internal class LiveViewClass
	{
		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06000A26 RID: 2598 RVA: 0x0003B010 File Offset: 0x00039210
		// (remove) Token: 0x06000A27 RID: 2599 RVA: 0x0003B044 File Offset: 0x00039244
		public static event Action<bool> ReceiveLiveImage;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06000A28 RID: 2600 RVA: 0x0003B078 File Offset: 0x00039278
		// (remove) Token: 0x06000A29 RID: 2601 RVA: 0x0003B0AC File Offset: 0x000392AC
		public static event Action LiveViewImageLoaded;

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x06000A2A RID: 2602 RVA: 0x0003B0DF File Offset: 0x000392DF
		// (set) Token: 0x06000A2B RID: 2603 RVA: 0x0003B0E7 File Offset: 0x000392E7
		public ICameraDevice CameraDevice { get; set; }

		// Token: 0x06000A2C RID: 2604
		[DllImport("gdi32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DeleteObject([In] IntPtr hObject);

		// Token: 0x17000171 RID: 369
		// (get) Token: 0x06000A2D RID: 2605 RVA: 0x0003B0F0 File Offset: 0x000392F0
		// (set) Token: 0x06000A2E RID: 2606 RVA: 0x0003B0F7 File Offset: 0x000392F7
		public static ImageSource staticImage { get; set; }

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x06000A2F RID: 2607 RVA: 0x0003B0FF File Offset: 0x000392FF
		public static int CameraPreviewWidth
		{
			get
			{
				return LiveViewClass.saveBitmap.Width;
			}
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x06000A30 RID: 2608 RVA: 0x0003B10B File Offset: 0x0003930B
		public static int CameraPreviewHeight
		{
			get
			{
				return LiveViewClass.saveBitmap.Height;
			}
		}

		// Token: 0x06000A31 RID: 2609 RVA: 0x0003B118 File Offset: 0x00039318
		public void LiveViewStart(ICameraDevice cameraDevice)
		{
			LiveViewClass.isHaveGreenBox = Settings.GetValueBoolean("greenbox").Value;
			LiveViewClass.isLivePreviewGreenBoxActive = true;
			if (this._liveViewTimer != null && this._liveViewTimer.IsAlive)
			{
				this._liveViewTimer.Abort();
			}
			this._liveViewTimer = new Thread(new ThreadStart(this._liveViewTimer_Tick));
			this.LastCapturedTime = DateTime.Now;
			this.CameraDevice = cameraDevice;
			this.CameraDevice.CameraDisconnected += this.CameraDevice_CameraDisconnected;
			this.start_LiewView();
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x0003B1A8 File Offset: 0x000393A8
		private void start_LiewView()
		{
			if (this._StartLiveViewThread == null || !this._StartLiveViewThread.IsAlive)
			{
				this._StartLiveViewThread = new Thread(new ThreadStart(this.StartLiveView));
				this._StartLiveViewThread.Start();
			}
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x0003B1E1 File Offset: 0x000393E1
		private void CameraDevice_CameraDisconnected(object sender, DisconnectCameraEventArgs eventArgs)
		{
			this._liveViewTimer.Abort();
			LiveViewClass.liveActive = false;
			Thread.Sleep(100);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x0003B1FC File Offset: 0x000393FC
		private void StartLiveView()
		{
			int retryCount = 0;
			bool retry;
			do
			{
				retry = false;
				try
				{
					if (!CameraControlClass.IsNikonLiveViewActive(this.CameraDevice))
					{
						this.CameraDevice.StartLiveView();
					}
					LiveViewClass.IsLiveViewTimerEnabled = true;
				}
				catch (DeviceException exception)
				{
					if (exception.ErrorCode == 8217U || exception.ErrorCode == 2147942570U)
					{
						Thread.Sleep(100);
						retry = (retryCount <= 4);
						retryCount++;
						MessageBoxWindow.CreateWindow("Live View Error", exception.Message, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					}
					else
					{
						MessageBoxWindow.CreateWindow("Live View Error", exception.Message, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						CameraControlClass.SetDefaultSettingForCamera();
					}
				}
				catch (Exception ex)
				{
				}
			}
			while (retry);
			if (!this._liveViewTimer.IsAlive)
			{
				try
				{
					this._liveViewTimer.Start();
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x0003B2F4 File Offset: 0x000394F4
		public void StopLiveView()
		{
			bool retry;
			do
			{
				retry = false;
				try
				{
					if (this._liveViewTimer != null && this._liveViewTimer.IsAlive)
					{
						this._liveViewTimer.Abort();
						LiveViewClass.IsLiveViewTimerEnabled = false;
						Thread.Sleep(500);
						if (!CameraControlClass.IsNikon(CameraControlClass.DeviceManager.SelectedCameraDevice))
						{
							CameraControlClass.DeviceManager.SelectedCameraDevice.StopLiveView();
						}
					}
				}
				catch (DeviceException exception)
				{
					if (exception.ErrorCode == 2147942570U)
					{
						Thread.Sleep(100);
						retry = true;
					}
					else if (exception.ErrorCode != 8217U)
					{
						MessageBoxWindow.CreateWindow("Live View Error", exception.Message, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						CameraControlClass.SetDefaultSettingForCamera();
					}
				}
			}
			while (retry);
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x0003B3B8 File Offset: 0x000395B8
		private void RestartLiveView()
		{
			CameraControlClass.DeviceManager.SelectedCameraDevice.StopLiveView();
			this.StartLiveView();
			this.LastCapturedTime = DateTime.Now;
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x0003B3DC File Offset: 0x000395DC
		private void _liveViewTimer_Tick()
		{
			LiveViewData liveViewData = null;
			for (;;)
			{
				try
				{
					if (LiveViewClass.IsLiveViewTimerEnabled)
					{
						liveViewData = this.CameraDevice.GetLiveViewImage();
					}
				}
				catch (Exception ex)
				{
					continue;
				}
				if (liveViewData == null || liveViewData.ImageData == null)
				{
					if ((DateTime.Now - this.LastCapturedTime).Seconds > 7 && LiveViewClass.IsLiveViewTimerEnabled)
					{
						this.RestartLiveView();
					}
				}
				else
				{
					this.LastCapturedTime = DateTime.Now;
					try
					{
						LiveViewClass.saveBitmap = new Bitmap(new MemoryStream(liveViewData.ImageData, liveViewData.ImageDataPosition, liveViewData.ImageData.Length - liveViewData.ImageDataPosition));
						if (LiveViewClass.ReceiveLiveImage != null)
						{
							LiveViewClass.ReceiveLiveImage(LiveViewClass.isFirstReceive);
							LiveViewClass.isFirstReceive = true;
						}
						if (LiveViewClass.LiveViewImageLoaded != null)
						{
							if (CameraControlClass.isHaveBackground && LiveViewClass.isLivePreviewGreenBoxActive && LiveViewClass.isHaveGreenBox && !TemplateClass.NoBackground[0])
							{
								LiveViewClass.saveBitmap = ExtensionMethod.ColorKeyGreenEffect(LiveViewClass.saveBitmap, CameraControlClass.backgroundMat, CameraControlClass.trashOld);
							}
							string rotation = Settings.GetValueString("rotation") ?? "0";
							if (rotation.Contains("90"))
							{
								LiveViewClass.saveBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
							}
							else if (rotation.Contains("180"))
							{
								LiveViewClass.saveBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
							}
							else if (rotation.Contains("270"))
							{
								LiveViewClass.saveBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
							}
							LiveViewClass.staticImage = ExtensionMethod.ConvertBitmap2BitmapSource(LiveViewClass.saveBitmap);
							LiveViewClass.staticImage.Freeze();
							GC.Collect();
							GC.WaitForPendingFinalizers();
							if (LiveViewClass.LiveViewImageLoaded != null)
							{
								LiveViewClass.LiveViewImageLoaded();
							}
						}
					}
					catch (Exception ex2)
					{
						ex2.Source == "System.Drawing";
						Console.WriteLine(ex2.ToString());
						Thread.Sleep(1000);
						break;
					}
					Thread.Sleep(this.delay);
				}
			}
		}

		// Token: 0x040009E9 RID: 2537
		private static bool isFirstReceive = true;

		// Token: 0x040009EB RID: 2539
		private Thread _liveViewTimer;

		// Token: 0x040009EC RID: 2540
		private Thread _StartLiveViewThread;

		// Token: 0x040009ED RID: 2541
		public static bool liveActive = false;

		// Token: 0x040009EE RID: 2542
		private int delay = 1;

		// Token: 0x040009EF RID: 2543
		public static Bitmap saveBitmap;

		// Token: 0x040009F0 RID: 2544
		private static bool isHaveGreenBox = false;

		// Token: 0x040009F1 RID: 2545
		private static bool isLivePreviewGreenBoxActive = false;

		// Token: 0x040009F3 RID: 2547
		public static bool IsLiveViewTimerEnabled = false;

		// Token: 0x040009F4 RID: 2548
		private System.Timers.Timer LiveKeepAliveTimer;

		// Token: 0x040009F5 RID: 2549
		private DateTime LastCapturedTime = DateTime.Now;
	}
}
