using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using CameraControl.Devices.Nikon;
using FFmpeg.NET;
using ImageMagick;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages;
using OpenCvSharp;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B1 RID: 177
	public static class CameraControlClass
	{
		// Token: 0x14000027 RID: 39
		// (add) Token: 0x060009EB RID: 2539 RVA: 0x000393E4 File Offset: 0x000375E4
		// (remove) Token: 0x060009EC RID: 2540 RVA: 0x00039418 File Offset: 0x00037618
		public static event Action CameraConnectLoadCompleted;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x060009ED RID: 2541 RVA: 0x0003944C File Offset: 0x0003764C
		// (remove) Token: 0x060009EE RID: 2542 RVA: 0x00039480 File Offset: 0x00037680
		public static event Action ReconnectedCameraEvent;

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060009EF RID: 2543 RVA: 0x000394B3 File Offset: 0x000376B3
		// (set) Token: 0x060009F0 RID: 2544 RVA: 0x000394BA File Offset: 0x000376BA
		public static CameraDeviceManager DeviceManager { get; set; }

		// Token: 0x060009F1 RID: 2545 RVA: 0x000394C2 File Offset: 0x000376C2
		public static void LoadBackground(string filePath)
		{
			if (File.Exists(filePath))
			{
				CameraControlClass.backgroundMat = Cv2.ImRead(filePath, ImreadModes.Color);
			}
		}

		// Token: 0x060009F2 RID: 2546 RVA: 0x000394D8 File Offset: 0x000376D8
		public static Color GetPositionColor(System.Drawing.Point loc, Bitmap image, System.Drawing.Size objectSize)
		{
			double scaleX = (double)image.Width / (double)objectSize.Width;
			double scaleY = (double)image.Height / (double)objectSize.Height;
			double photoScale = Math.Min(scaleX, scaleY);
			objectSize.Width = (int)(photoScale * (double)image.Width);
			objectSize.Height = (int)(photoScale * (double)image.Height);
			loc = new System.Drawing.Point((int)((double)loc.X * scaleX), (int)((double)loc.Y * scaleY));
			return image.GetPixel(loc.X, loc.Y);
		}

		// Token: 0x060009F3 RID: 2547 RVA: 0x00039564 File Offset: 0x00037764
		public static void SetCameraBrightness(List<string> values)
		{
			ICameraDevice thisCameraDevice = CameraControlClass.DeviceManager.SelectedCameraDevice;
			thisCameraDevice.IsoNumber.Value = values[0];
			thisCameraDevice.ShutterSpeed.Value = values[1];
			thisCameraDevice.FNumber.Value = values[2];
		}

		// Token: 0x060009F4 RID: 2548 RVA: 0x000395B4 File Offset: 0x000377B4
		public static void SetCameraBrightness(double brightness)
		{
			ICameraDevice thisCamera = CameraControlClass.DeviceManager.SelectedCameraDevice;
			int isoIndex = -1;
			int shutterIndex = -1;
			int fstopIndex = -1;
			if (thisCamera.IsoNumber.IsEnabled)
			{
				isoIndex = (int)(brightness / 100.0 * (double)(thisCamera.IsoNumber.Values.Count - 1));
			}
			if (thisCamera.ShutterSpeed.IsEnabled)
			{
				shutterIndex = thisCamera.ShutterSpeed.Values.Count - 1 - (int)(brightness / 100.0 * (double)(thisCamera.ShutterSpeed.Values.Count - 1));
			}
			if (thisCamera.FNumber.IsEnabled)
			{
				fstopIndex = thisCamera.FNumber.Values.Count - 1 - (int)(brightness / 100.0 * (double)(thisCamera.FNumber.Values.Count - 1));
			}
			if (isoIndex != -1)
			{
				thisCamera.IsoNumber.Value = thisCamera.IsoNumber.Values[isoIndex];
			}
			if (shutterIndex != -1)
			{
				thisCamera.ShutterSpeed.Value = thisCamera.ShutterSpeed.Values[shutterIndex];
			}
			if (fstopIndex != -1)
			{
				thisCamera.FNumber.Value = thisCamera.FNumber.Values[fstopIndex];
			}
		}

		// Token: 0x060009F5 RID: 2549 RVA: 0x000396E4 File Offset: 0x000378E4
		public static List<string> GetCameraBrightness(ICameraDevice thisCameraDevice = null)
		{
			List<string> CameraSettingsValues = new List<string>();
			if (thisCameraDevice == null)
			{
				thisCameraDevice = CameraControlClass.DeviceManager.SelectedCameraDevice;
			}
			CameraSettingsValues.Add(thisCameraDevice.IsoNumber.Value);
			CameraSettingsValues.Add(thisCameraDevice.ShutterSpeed.Value);
			CameraSettingsValues.Add(thisCameraDevice.FNumber.Value);
			return CameraSettingsValues;
		}

		// Token: 0x060009F6 RID: 2550 RVA: 0x0003973C File Offset: 0x0003793C
		public static Task SaveVideoWithBackground(string filePath, string savePath)
		{
			CameraControlClass.<SaveVideoWithBackground>d__54 <SaveVideoWithBackground>d__;
			<SaveVideoWithBackground>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SaveVideoWithBackground>d__.filePath = filePath;
			<SaveVideoWithBackground>d__.savePath = savePath;
			<SaveVideoWithBackground>d__.<>1__state = -1;
			<SaveVideoWithBackground>d__.<>t__builder.Start<CameraControlClass.<SaveVideoWithBackground>d__54>(ref <SaveVideoWithBackground>d__);
			return <SaveVideoWithBackground>d__.<>t__builder.Task;
		}

		// Token: 0x060009F7 RID: 2551 RVA: 0x00039788 File Offset: 0x00037988
		public static void Load()
		{
			if (CameraControlClass.watermarkHeight == 0 && !TemplateClass.NoWatermark[1])
			{
				Image bitmapwatermark = Image.FromFile(KingAIPhotoBoothPro.Class.Helper.Settings.GetValueString(TemplateClass.watermarkVideoKey));
				CameraControlClass.watermarkHeight = bitmapwatermark.Height;
				CameraControlClass.watermarkWidth = bitmapwatermark.Width;
			}
		}

		// Token: 0x060009F8 RID: 2552 RVA: 0x000397CB File Offset: 0x000379CB
		public static void CameraCompleteManuelEvent()
		{
			if (CameraControlClass.CameraConnectLoadCompleted != null)
			{
				CameraControlClass.CameraConnectLoadCompleted();
			}
		}

		// Token: 0x060009F9 RID: 2553 RVA: 0x000397E0 File Offset: 0x000379E0
		public static string GetCameraProperties()
		{
			if (CameraControlClass.DeviceManager.SelectedCameraDevice != null)
			{
				ICameraDevice cam = CameraControlClass.DeviceManager.SelectedCameraDevice;
				return string.Format("Device Name : {0}\nDevice Display Name : {1}\nISO : {2}\nShutterSpeed : {3}\nWhiteBalance : {4}", new object[]
				{
					cam.DeviceName,
					cam.DisplayName,
					cam.IsoNumber.Value,
					cam.ShutterSpeed.Value,
					cam.WhiteBalance.Value
				});
			}
			return "Device Not Connected";
		}

		// Token: 0x060009FA RID: 2554 RVA: 0x00039858 File Offset: 0x00037A58
		public static void CloseDSLR()
		{
			if (CameraControlClass.DeviceManager != null)
			{
				CameraControlClass.DeviceManager.CameraSelected -= CameraControlClass.DeviceManager_CameraSelected;
				CameraControlClass.DeviceManager.CameraConnected -= CameraControlClass.DeviceManager_CameraConnected;
				CameraControlClass.DeviceManager.CameraDisconnected -= CameraControlClass.DeviceManager_CameraDisconnected;
				CameraControlClass.DeviceManager.CloseAll();
				CameraControlClass.DeviceManager = null;
			}
		}

		// Token: 0x060009FB RID: 2555 RVA: 0x000398C0 File Offset: 0x00037AC0
		public static void FolderCheck()
		{
			CameraControlClass.ffmpeg = new Engine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg", "ffmpeg.exe"));
			CameraControlClass.FolderForPhotos = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "Captured");
			CameraControlClass.FolderForRender = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "Render");
			CameraControlClass.FolderForCopy = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "Copy");
			CameraControlClass.FolderForGif = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "GifStatic");
			CameraControlClass.FolderForResultsGif = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "Gifs");
			CameraControlClass.FolderForVideo = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "Video");
			CameraControlClass.FolderForTemp = Path.Combine(KingAIPhotoBoothPro.Class.Helper.Settings.GetCurrentEventFolderPath(), "Temp");
			CameraControlClass.BacgroundForTranparent = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "transparent.png");
			if (!Directory.Exists(CameraControlClass.FolderForPhotos))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForPhotos);
			}
			if (!Directory.Exists(CameraControlClass.FolderForRender))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForRender);
			}
			if (!Directory.Exists(CameraControlClass.FolderForCopy))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForCopy);
			}
			if (!Directory.Exists(CameraControlClass.FolderForGif))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForGif);
			}
			if (!Directory.Exists(CameraControlClass.FolderForResultsGif))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForResultsGif);
			}
			if (!Directory.Exists(CameraControlClass.FolderForVideo))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForVideo);
			}
			if (!Directory.Exists(CameraControlClass.FolderForTemp))
			{
				Directory.CreateDirectory(CameraControlClass.FolderForTemp);
			}
		}

		// Token: 0x060009FC RID: 2556 RVA: 0x00039A3C File Offset: 0x00037C3C
		public static void ConnectCamera()
		{
			CameraControlClass.FolderCheck();
			if (CameraControlClass.DeviceManager == null)
			{
				ServiceProvider.DeviceManager = new CameraDeviceManager(null);
				CameraControlClass.DeviceManager = ServiceProvider.DeviceManager;
				CameraControlClass.DeviceManager.DetectWebcams = false;
				CameraControlClass.DeviceManager.CameraSelected += CameraControlClass.DeviceManager_CameraSelected;
				CameraControlClass.DeviceManager.CameraConnected += CameraControlClass.DeviceManager_CameraConnected;
				CameraControlClass.DeviceManager.CameraDisconnected += CameraControlClass.DeviceManager_CameraDisconnected;
				CameraControlClass.DeviceManager.UseExperimentalDrivers = true;
				CameraControlClass.DeviceManager.DisableNativeDrivers = false;
				if (ServiceProvider.Branding == null)
				{
					ServiceProvider.Branding = Branding.LoadBranding();
				}
				ServiceProvider.Settings = new CameraControl.Core.Classes.Settings();
				ServiceProvider.Settings = ServiceProvider.Settings.Load();
				ServiceProvider.Settings.DisableNativeDrivers = false;
			}
			CameraControlClass.isConnectedCamera = true;
			if (CameraControlClass.CameraConnectLoadCompleted != null)
			{
				CameraControlClass.CameraConnectLoadCompleted();
			}
		}

		// Token: 0x060009FD RID: 2557 RVA: 0x00039B1C File Offset: 0x00037D1C
		public static bool NikonCameraIsBusy()
		{
			return (from x in CameraControlClass.DeviceManager.ConnectedDevices
			where x.Manufacturer != null && x.Manufacturer.ToLower().Contains("nikon")
			select x).FirstOrDefault<ICameraDevice>().IsBusy;
		}

		// Token: 0x060009FE RID: 2558 RVA: 0x00039B58 File Offset: 0x00037D58
		public static bool IsNikonCamera()
		{
			try
			{
				if (CameraControlClass.DeviceManager == null)
				{
					CameraControlClass.ConnectCamera();
				}
				CameraControlClass.DeviceManager.ConnectToCamera();
				CameraControlClass.isError = false;
			}
			catch (Exception ex)
			{
				MessageBoxWindow.CreateWindow("Camera Error", "Could not connect to camera!", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				ActivationKingWindow.SettingsPage.HideOrOpenButtons();
				CameraControlClass.isError = true;
			}
			return (from x in CameraControlClass.DeviceManager.ConnectedDevices
			where x.Manufacturer != null && x.Manufacturer.ToLower().Contains("nikon")
			select x).Count<ICameraDevice>() > 0;
		}

		// Token: 0x060009FF RID: 2559 RVA: 0x00039BFC File Offset: 0x00037DFC
		public static void LoadFinalCameraFunction()
		{
			if (!CameraControlClass.isConnectedCamera && !CameraControlClass.isError)
			{
				return;
			}
			if (CameraControlClass.isFirsWork)
			{
				CameraControlClass.DeviceManager.PhotoCaptured += CameraControlClass.DeviceManager_PhotoCaptured;
				CameraControlClass.dispatcherTimerDownloadQueue = new DispatcherTimer();
				CameraControlClass.dispatcherTimerDownloadQueue.Interval = new TimeSpan(0, 0, 0, 0, 250);
				CameraControlClass.dispatcherTimerDownloadQueue.Tick += CameraControlClass.DispatcherTimerDownloadQueue_Tick;
				CameraControlClass.dispatcherTimerDownloadQueue.Start();
			}
			CameraControlClass.DeviceList.Clear();
			foreach (ICameraDevice cameraDevice in CameraControlClass.DeviceManager.ConnectedDevices)
			{
				if (cameraDevice.IsoNumber != null && cameraDevice.IsoNumber.Value != null)
				{
					CameraControlClass.DeviceList.Add(cameraDevice.DeviceName + (cameraDevice.IsBusy ? "(unavailable)" : ""));
				}
				else
				{
					bool isCanon = ExtensionMethod.ContainsClass(cameraDevice, "canon");
					if (isCanon || (cameraDevice.Manufacturer != null && cameraDevice.Manufacturer.ToLower().Contains("nikon")))
					{
						if (isCanon && cameraDevice.IsBusy)
						{
							ExtensionMethod.ControlEOSService();
						}
						CameraControlClass.DeviceList.Add(cameraDevice.DeviceName + (cameraDevice.IsBusy ? "(unavailable)" : ""));
					}
				}
			}
			CameraControlClass.isFirsWork = false;
		}

		// Token: 0x06000A00 RID: 2560 RVA: 0x00039D74 File Offset: 0x00037F74
		private static void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
		{
			DSLR.IsLoadCamera = false;
			CameraControlClass.isCameraDisconnect = true;
			CameraControlClass.isConnectedCamera = false;
			CameraControlClass.setCameraOption = false;
			CameraControlClass.connectCameraID = cameraDevice.SerialNumber;
		}

		// Token: 0x06000A01 RID: 2561 RVA: 0x00039D99 File Offset: 0x00037F99
		private static void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
		{
			LiveViewClass.liveActive = false;
			CameraControlClass.photoDownloadQueue.Add(eventArgs);
		}

		// Token: 0x06000A02 RID: 2562 RVA: 0x00039DAC File Offset: 0x00037FAC
		private static void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
		{
			CameraControlClass.CameraName cameraName = null;
			if (CameraControlClass.cameraNames != null)
			{
				cameraName = CameraControlClass.cameraNames.Find((CameraControlClass.CameraName x) => x.SerialNumber == cameraDevice.SerialNumber);
			}
			else
			{
				CameraControlClass.cameraNames = new List<CameraControlClass.CameraName>();
			}
			if (cameraName == null)
			{
				cameraName = new CameraControlClass.CameraName();
				cameraName.SerialNumber = cameraDevice.SerialNumber;
				cameraName.Name = cameraDevice.DisplayName;
				cameraName.Connected = true;
				CameraControlClass.cameraNames.Add(cameraName);
			}
			else
			{
				cameraName.Connected = true;
			}
			if (CameraControlClass.connectCameraID != null && CameraControlClass.connectCameraID == cameraDevice.SerialNumber)
			{
				CameraControlClass.DeviceManager.SelectedCameraDevice = cameraDevice;
				CameraControlClass.DeviceManager.ConnectToCamera();
				if (CameraControlClass.ReconnectedCameraEvent != null)
				{
					CameraControlClass.ReconnectedCameraEvent();
				}
			}
		}

		// Token: 0x06000A03 RID: 2563 RVA: 0x00039E84 File Offset: 0x00038084
		public static void DeviceManager_CameraSelected(ICameraDevice oldcameraDevice, ICameraDevice newcameraDevice)
		{
		}

		// Token: 0x06000A04 RID: 2564 RVA: 0x00039E86 File Offset: 0x00038086
		public static void StartLiveCamera()
		{
			CameraControlClass.LiveViewConnect.LiveViewStart(CameraControlClass.DeviceManager.SelectedCameraDevice);
			LiveViewClass.liveActive = true;
		}

		// Token: 0x06000A05 RID: 2565 RVA: 0x00039EA2 File Offset: 0x000380A2
		public static void StopLiveCamera()
		{
			LiveViewClass.liveActive = false;
		}

		// Token: 0x06000A06 RID: 2566 RVA: 0x00039EAA File Offset: 0x000380AA
		public static void StopRecordVideo()
		{
			new Thread(delegate()
			{
				CameraControlClass.StopRecordVideoThread();
			}).Start();
		}

		// Token: 0x06000A07 RID: 2567 RVA: 0x00039ED8 File Offset: 0x000380D8
		private static void StopRecordVideoThread()
		{
			bool retry;
			do
			{
				retry = false;
				try
				{
					CameraControlClass.DeviceManager.SelectedCameraDevice.StopRecordMovie();
					if (CameraControlClass.IsNikon(CameraControlClass.DeviceManager.SelectedCameraDevice))
					{
						LiveViewClass.IsLiveViewTimerEnabled = false;
					}
				}
				catch (DeviceException exception)
				{
					if (exception.ErrorCode == 8217U || exception.ErrorCode == 2147942570U)
					{
						Thread.Sleep(100);
						retry = true;
					}
					else if (exception.Message.IndexOf("8D01") != -1)
					{
						int num = CameraControlClass.errorNumber;
						CameraControlClass.errorNumber = 1;
					}
				}
				catch (Exception ex)
				{
					if (ex.ToString().IndexOf("propertyId 1296") != -1)
					{
						string title = "Camera Error ";
						string desc = "No SD Card or full. \n The application will be restarted.";
						List<MessageBoxWindow.ButtonType> list = new List<MessageBoxWindow.ButtonType>();
						list.Add(MessageBoxWindow.ButtonType.Continue);
						MessageBoxWindow.CreateWindow(title, desc, list, MessageBoxWindow.MessageIcon.Error, delegate(MessageBoxWindow.MessageBoxReturn result)
						{
							if (result == MessageBoxWindow.MessageBoxReturn.Continue)
							{
								string batFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "restart.bat");
								new Process
								{
									StartInfo = 
									{
										FileName = batFilePath,
										UseShellExecute = false,
										CreateNoWindow = true,
										WindowStyle = ProcessWindowStyle.Hidden,
										RedirectStandardOutput = true
									}
								}.Start();
								Application.Current.Shutdown();
							}
						}, MessageBoxWindow.MessageBoxSize.Large, false);
					}
					else
					{
						MessageBoxWindow.CreateWindow("Camera Error ", "Error occurred :" + ex.Message, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
					}
					if (CameraControlClass.VideoCapturedFailedEvent != null)
					{
						CameraControlClass.VideoCapturedFailedEvent(ex.ToString());
					}
				}
			}
			while (retry);
		}

		// Token: 0x06000A08 RID: 2568 RVA: 0x0003A018 File Offset: 0x00038218
		public static void SetCamera(bool isAutomaticSelect = true, string deviceName = "", bool isStartLiveViewCamera = true)
		{
			if (CameraControlClass.DeviceManager.SelectedCameraDevice == null)
			{
				return;
			}
			if (!CameraControlClass.setCameraOption)
			{
				CameraControlClass.setCameraOption = true;
				if (isAutomaticSelect)
				{
					CameraControlClass.DeviceManager.SelectedCameraDevice = (from x in CameraControlClass.DeviceManager.ConnectedDevices
					where x.DeviceName != null
					select x).FirstOrDefault<ICameraDevice>();
				}
				else if (CameraControlClass.DeviceManager.SelectedCameraDevice == null || CameraControlClass.DeviceManager.SelectedCameraDevice.DeviceName != deviceName || CameraControlClass.firstSelectedCamera)
				{
					CameraControlClass.DeviceManager.SelectedCameraDevice = (from x in CameraControlClass.DeviceManager.ConnectedDevices
					where x.DeviceName == deviceName
					select x).FirstOrDefault<ICameraDevice>();
				}
				try
				{
					CameraControlClass.firstSelectedCamera = false;
					if (CameraControlClass.DeviceManager.SelectedCameraDevice.Manufacturer != null)
					{
						if (!CameraControlClass.DeviceManager.SelectedCameraDevice.Manufacturer.ToLower().Contains("nikon"))
						{
							CameraControlClass.Focus();
						}
					}
					else
					{
						CameraControlClass.Focus();
					}
				}
				catch (Exception ex)
				{
				}
				CameraControlClass.setCameraOption = false;
			}
			if (isStartLiveViewCamera)
			{
				CameraControlClass.StartLiveCamera();
			}
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x0003A154 File Offset: 0x00038354
		public static bool Focus()
		{
			if (CameraControlClass.DeviceManager != null && CameraControlClass.DeviceManager.SelectedCameraDevice != null)
			{
				CameraControlClass.DeviceManager.SelectedCameraDevice.AutoFocus();
				return true;
			}
			return false;
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x0003A17B File Offset: 0x0003837B
		public static void TakePicture()
		{
			CameraControlClass.CapturePhotoThread = new Thread(new ThreadStart(CameraControlClass.CaptureAsync));
			CameraControlClass.CapturePhotoThread.Start();
		}

		// Token: 0x06000A0B RID: 2571 RVA: 0x0003A19D File Offset: 0x0003839D
		public static void RecordVideo()
		{
			CameraControlClass.CaptureVideoThread = new Thread(delegate()
			{
				CameraControlClass.RecordVideoThread();
			});
			CameraControlClass.CaptureVideoThread.Start();
		}

		// Token: 0x06000A0C RID: 2572 RVA: 0x0003A1D4 File Offset: 0x000383D4
		private static void RecordVideoThread()
		{
			bool retry;
			do
			{
				retry = false;
				try
				{
					CameraControlClass.DeviceManager.SelectedCameraDevice.StartRecordMovie();
				}
				catch (DeviceException exception)
				{
					if (exception.ErrorCode == 8217U || exception.ErrorCode == 2147942570U)
					{
						Thread.Sleep(100);
						retry = true;
					}
					else if (exception.Message.IndexOf("8D01") != -1)
					{
						int num = CameraControlClass.errorNumber;
						CameraControlClass.errorNumber = 1;
					}
				}
				catch (Exception ex)
				{
					MessageBoxWindow.CreateWindow("Camera Error ", "Error occurred :" + ex.Message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
				}
			}
			while (retry);
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x0003A28C File Offset: 0x0003848C
		public static bool IsNikon(ICameraDevice device)
		{
			return device is NikonBase;
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x0003A2A4 File Offset: 0x000384A4
		public static bool IsNikonLiveViewActive(ICameraDevice device)
		{
			bool IsNikon = device is NikonBase;
			return IsNikon && (device as NikonBase).LiveViewMovieOn;
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x0003A2CC File Offset: 0x000384CC
		private static Task TimerControl(ICameraDevice camera)
		{
			CameraControlClass.<TimerControl>d__82 <TimerControl>d__;
			<TimerControl>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<TimerControl>d__.camera = camera;
			<TimerControl>d__.<>1__state = -1;
			<TimerControl>d__.<>t__builder.Start<CameraControlClass.<TimerControl>d__82>(ref <TimerControl>d__);
			return <TimerControl>d__.<>t__builder.Task;
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x0003A310 File Offset: 0x00038510
		private static Task WaitLiveViewClose()
		{
			CameraControlClass.<WaitLiveViewClose>d__83 <WaitLiveViewClose>d__;
			<WaitLiveViewClose>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<WaitLiveViewClose>d__.<>1__state = -1;
			<WaitLiveViewClose>d__.<>t__builder.Start<CameraControlClass.<WaitLiveViewClose>d__83>(ref <WaitLiveViewClose>d__);
			return <WaitLiveViewClose>d__.<>t__builder.Task;
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x0003A34C File Offset: 0x0003854C
		private static void CaptureAsync()
		{
			CameraControlClass.<CaptureAsync>d__84 <CaptureAsync>d__;
			<CaptureAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CaptureAsync>d__.<>1__state = -1;
			<CaptureAsync>d__.<>t__builder.Start<CameraControlClass.<CaptureAsync>d__84>(ref <CaptureAsync>d__);
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x0003A37C File Offset: 0x0003857C
		private static void MessageBoxButtonPressed(MessageBoxWindow.MessageBoxReturn @return)
		{
			try
			{
				if (!CameraControlClass.DeviceManager.SelectedCameraDevice.Manufacturer.ToLower().Contains("nikon"))
				{
					CameraControlClass.Focus();
				}
			}
			catch (Exception)
			{
				MessageBoxWindow.CreateWindow("Camera Error ", "Please Control Camera", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x0003A3E4 File Offset: 0x000385E4
		public static void BreakCapturePhotoThread()
		{
			LiveViewClass.IsLiveViewTimerEnabled = false;
			if (CameraControlClass.CapturePhotoThread != null && CameraControlClass.CapturePhotoThread.IsAlive)
			{
				CameraControlClass.CapturePhotoThread.Abort();
			}
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x0003A409 File Offset: 0x00038609
		public static void BreakCaptureVideoThread()
		{
			if (CameraControlClass.CaptureVideoThread != null && CameraControlClass.CaptureVideoThread.IsAlive)
			{
				CameraControlClass.CaptureVideoThread.Abort();
			}
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x0003A428 File Offset: 0x00038628
		private static void DispatcherTimerDownloadQueue_Tick(object sender, EventArgs e)
		{
			if (CameraControlClass.photoDownloadQueue.Count > 0 && !CameraControlClass.downloadWorking)
			{
				Thread thread = new Thread(new ParameterizedThreadStart(CameraControlClass.PhotoCaptured));
				thread.Start(CameraControlClass.photoDownloadQueue[0]);
				CameraControlClass.currentDownloadProcess = CameraControlClass.photoDownloadQueue[0];
			}
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x0003A47C File Offset: 0x0003867C
		private static void PhotoCaptured(object o)
		{
			CameraControlClass.downloadWorking = true;
			PhotoCapturedEventArgs eventArgs = o as PhotoCapturedEventArgs;
			if (eventArgs == null)
			{
				return;
			}
			try
			{
				string filePath = Path.Combine(CameraControlClass.FolderForPhotos, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffffff") + "_" + Path.GetFileName(eventArgs.FileName));
				if (!Directory.Exists(Path.GetDirectoryName(filePath)))
				{
					Directory.CreateDirectory(Path.GetDirectoryName(filePath));
				}
				eventArgs.CameraDevice.TransferFile(eventArgs.Handle, filePath);
				eventArgs.CameraDevice.IsBusy = false;
				string fileExtension = Path.GetExtension(filePath);
				if (CameraControlClass.IsValidFormat(fileExtension))
				{
					try
					{
						using (MagickImage mImage = new MagickImage(filePath))
						{
							mImage.Format = MagickFormat.Jpg;
							if (mImage.Height > 2160U)
							{
								double ratio = 2160.0 / mImage.Height;
								mImage.Resize((uint)(mImage.Width * ratio), 2160U);
								string newFilePath = Path.Combine(CameraControlClass.FolderForPhotos, DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffffff") + "_" + Path.GetFileNameWithoutExtension(eventArgs.FileName) + ".jpg");
								mImage.Write(newFilePath);
								File.Delete(filePath);
								filePath = newFilePath;
							}
						}
					}
					catch (Exception exception)
					{
						MessageBoxWindow.CreateWindow("Camera Photo Error ", "Error download photo from camera \n + " + exception.Message, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					}
					if (CameraControlClass.PhotoCapturedEvent != null)
					{
						CameraControlClass.PhotoCapturedEvent(filePath);
					}
					Thread.Sleep(250);
					if (LiveViewClass.liveActive)
					{
						CameraControlClass.LiveViewConnect.LiveViewStart(CameraControlClass.DeviceManager.SelectedCameraDevice);
					}
				}
				else
				{
					if (CameraControlClass.VideoCapturedEvent != null)
					{
						CameraControlClass.VideoCapturedEvent(filePath);
					}
					Thread.Sleep(250);
				}
				CameraControlClass.photoDownloadQueue.Remove(CameraControlClass.currentDownloadProcess);
			}
			catch (Exception exception2)
			{
				eventArgs.CameraDevice.IsBusy = false;
				MessageBoxWindow.CreateWindow("Camera Photo Error ", "Error download photo from camera \n + " + exception2.Message, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			CameraControlClass.downloadWorking = false;
		}

		// Token: 0x06000A17 RID: 2583 RVA: 0x0003A6E0 File Offset: 0x000388E0
		public static bool IsValidFormat(string fileExtension)
		{
			fileExtension = fileExtension.TrimStart(new char[]
			{
				'.'
			}).ToLower();
			List<string> validFormats = (from f in Enum.GetNames(typeof(MagickFormat))
			select f.ToLower()).ToList<string>();
			return validFormats.Contains(fileExtension);
		}

		// Token: 0x06000A18 RID: 2584 RVA: 0x0003A748 File Offset: 0x00038948
		public static void SetDefaultSettingForCamera()
		{
			CameraControlClass.DeviceManager.SelectedCameraDevice.FocusMode.SetValue("FocusMode", true);
			CameraControlClass.DeviceManager.SelectedCameraDevice.IsoNumber.SetValue("100", true);
			CameraControlClass.DeviceManager.SelectedCameraDevice.WhiteBalance.SetValue("Auto", true);
			CameraControlClass.DeviceManager.SelectedCameraDevice.Mode.SetValue("Single", true);
			CameraControlClass.DeviceManager.SelectedCameraDevice.StopLiveView();
		}

		// Token: 0x040009B9 RID: 2489
		private static bool isFirsWork = true;

		// Token: 0x040009BA RID: 2490
		public static bool isVideo = false;

		// Token: 0x040009BB RID: 2491
		public static Mat backgroundMat = null;

		// Token: 0x040009BD RID: 2493
		public static Action<string> PhotoCapturedEvent;

		// Token: 0x040009BE RID: 2494
		public static Action<string> VideoCapturedEvent;

		// Token: 0x040009BF RID: 2495
		public static Action<string> PhotoCapturedFailedEvent;

		// Token: 0x040009C0 RID: 2496
		public static Action<string> VideoCapturedFailedEvent;

		// Token: 0x040009C3 RID: 2499
		private static List<CameraControlClass.CameraName> cameraNames = new List<CameraControlClass.CameraName>();

		// Token: 0x040009C4 RID: 2500
		public static byte trashOld = 30;

		// Token: 0x040009C5 RID: 2501
		public static string selectedCameraDeviceName;

		// Token: 0x040009C6 RID: 2502
		public static string FolderForPhotos;

		// Token: 0x040009C7 RID: 2503
		public static int errorNumber = 0;

		// Token: 0x040009C8 RID: 2504
		private static DispatcherTimer dispatcherTimerDownloadQueue;

		// Token: 0x040009C9 RID: 2505
		private static LiveViewClass LiveViewConnect = new LiveViewClass();

		// Token: 0x040009CA RID: 2506
		private static bool firstSelectedCamera = true;

		// Token: 0x040009CB RID: 2507
		public static List<string> DeviceList = new List<string>();

		// Token: 0x040009CC RID: 2508
		public static bool isConnectedCamera = false;

		// Token: 0x040009CD RID: 2509
		private static List<PhotoCapturedEventArgs> photoDownloadQueue = new List<PhotoCapturedEventArgs>();

		// Token: 0x040009CE RID: 2510
		private static bool downloadWorking = false;

		// Token: 0x040009CF RID: 2511
		private static PhotoCapturedEventArgs currentDownloadProcess;

		// Token: 0x040009D0 RID: 2512
		public static bool isCameraDisconnect = false;

		// Token: 0x040009D1 RID: 2513
		private static string connectCameraID = null;

		// Token: 0x040009D2 RID: 2514
		public static string FolderForRender;

		// Token: 0x040009D3 RID: 2515
		public static string FolderForCopy;

		// Token: 0x040009D4 RID: 2516
		public static string FolderForGif;

		// Token: 0x040009D5 RID: 2517
		public static string FolderForResultsGif;

		// Token: 0x040009D6 RID: 2518
		public static string FolderForVideo;

		// Token: 0x040009D7 RID: 2519
		public static string FolderForCaptured;

		// Token: 0x040009D8 RID: 2520
		public static string FolderForTemp;

		// Token: 0x040009D9 RID: 2521
		public static string BacgroundForTranparent;

		// Token: 0x040009DA RID: 2522
		public static Engine ffmpeg;

		// Token: 0x040009DB RID: 2523
		public static int watermarkWidth = 0;

		// Token: 0x040009DC RID: 2524
		public static int watermarkHeight = 0;

		// Token: 0x040009DD RID: 2525
		public static bool isHaveBackground = false;

		// Token: 0x040009DE RID: 2526
		public static bool isHaveWatermark = false;

		// Token: 0x040009DF RID: 2527
		public static string BackgroundPath;

		// Token: 0x040009E0 RID: 2528
		public static bool isError = false;

		// Token: 0x040009E1 RID: 2529
		[NonSerialized]
		private static VideoWriter _videoWriter = new VideoWriter();

		// Token: 0x040009E2 RID: 2530
		private static bool setCameraOption = false;

		// Token: 0x040009E3 RID: 2531
		private static Thread CapturePhotoThread;

		// Token: 0x040009E4 RID: 2532
		private static Thread CaptureVideoThread;

		// Token: 0x02000282 RID: 642
		[Serializable]
		public class CameraName
		{
			// Token: 0x040010CB RID: 4299
			public string SerialNumber;

			// Token: 0x040010CC RID: 4300
			public string Name;

			// Token: 0x040010CD RID: 4301
			public bool Connected;
		}
	}
}
