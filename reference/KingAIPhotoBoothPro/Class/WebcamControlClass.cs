using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Accord.Video.DirectShow;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using OpenCvSharp;
using SharpDX.MediaFoundation;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000AE RID: 174
	public static class WebcamControlClass
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060009C8 RID: 2504 RVA: 0x00038A0C File Offset: 0x00036C0C
		// (set) Token: 0x060009C9 RID: 2505 RVA: 0x00038A13 File Offset: 0x00036C13
		public static List<string> CameraList { get; private set; }

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x060009CA RID: 2506 RVA: 0x00038A1C File Offset: 0x00036C1C
		// (remove) Token: 0x060009CB RID: 2507 RVA: 0x00038A50 File Offset: 0x00036C50
		public static event Action<string> PhotoCaptured;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x060009CC RID: 2508 RVA: 0x00038A84 File Offset: 0x00036C84
		// (remove) Token: 0x060009CD RID: 2509 RVA: 0x00038AB8 File Offset: 0x00036CB8
		public static event Action<List<string>> WebcamConnected;

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x060009CE RID: 2510 RVA: 0x00038AEC File Offset: 0x00036CEC
		// (remove) Token: 0x060009CF RID: 2511 RVA: 0x00038B20 File Offset: 0x00036D20
		public static event Action<List<string>> WebcamDisconnected;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x060009D0 RID: 2512 RVA: 0x00038B54 File Offset: 0x00036D54
		// (remove) Token: 0x060009D1 RID: 2513 RVA: 0x00038B88 File Offset: 0x00036D88
		public static event Action<byte[]> LiveViewImageLoaded;

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x060009D2 RID: 2514 RVA: 0x00038BBC File Offset: 0x00036DBC
		// (remove) Token: 0x060009D3 RID: 2515 RVA: 0x00038BF0 File Offset: 0x00036DF0
		public static event Action<string> VideoCaptured;

		// Token: 0x060009D4 RID: 2516 RVA: 0x00038C24 File Offset: 0x00036E24
		public static Task<int> GetCameraIdByName(string cameraName)
		{
			WebcamControlClass.<GetCameraIdByName>d__37 <GetCameraIdByName>d__;
			<GetCameraIdByName>d__.<>t__builder = AsyncTaskMethodBuilder<int>.Create();
			<GetCameraIdByName>d__.cameraName = cameraName;
			<GetCameraIdByName>d__.<>1__state = -1;
			<GetCameraIdByName>d__.<>t__builder.Start<WebcamControlClass.<GetCameraIdByName>d__37>(ref <GetCameraIdByName>d__);
			return <GetCameraIdByName>d__.<>t__builder.Task;
		}

		// Token: 0x060009D5 RID: 2517 RVA: 0x00038C68 File Offset: 0x00036E68
		public static string[] ListOfAttachedCameras()
		{
			List<string> cameras = new List<string>();
			MediaAttributes attributes = new MediaAttributes(1);
			attributes.Set<Guid>(CaptureDeviceAttributeKeys.SourceType.Guid, CaptureDeviceAttributeKeys.SourceTypeVideoCapture.Guid);
			Activate[] devices = MediaFactory.EnumDeviceSources(attributes);
			for (int i = 0; i < devices.Count<Activate>(); i++)
			{
				string friendlyName = devices[i].Get<string>(CaptureDeviceAttributeKeys.FriendlyName);
				cameras.Add(friendlyName);
			}
			return cameras.ToArray();
		}

		// Token: 0x060009D6 RID: 2518 RVA: 0x00038CD4 File Offset: 0x00036ED4
		public static VideoCaptureAPIs GetBackendFromFriendlyName(string friendlyName)
		{
			if (string.IsNullOrWhiteSpace(friendlyName))
			{
				return VideoCaptureAPIs.ANY;
			}
			friendlyName = friendlyName.ToLowerInvariant();
			if (friendlyName.Contains("logi") || friendlyName.Contains("lifecam") || friendlyName.Contains("microsoft"))
			{
				return VideoCaptureAPIs.DSHOW;
			}
			if (friendlyName.Contains("usb") || friendlyName.Contains("uvc") || friendlyName.Contains("integrated"))
			{
				return VideoCaptureAPIs.MSMF;
			}
			if (friendlyName.Contains("obs") || friendlyName.Contains("virtual") || friendlyName.Contains("snap"))
			{
				return VideoCaptureAPIs.MSMF;
			}
			return VideoCaptureAPIs.DSHOW;
		}

		// Token: 0x060009D7 RID: 2519 RVA: 0x00038D80 File Offset: 0x00036F80
		public static void WebcamInit()
		{
			if (WebcamControlClass.openCvCameraHelper == null)
			{
				WebcamControlClass.openCvCameraHelper = new OpenCvCameraHelper();
			}
			WebcamControlClass.filePathFolder = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "Captured");
			CameraControlClass.FolderCheck();
			WebcamControlClass.CameraList = new List<string>();
			WebcamControlClass.videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
			foreach (FilterInfo device in WebcamControlClass.videoDevices)
			{
				if (!device.Name.ToLower().StartsWith("intel virtual"))
				{
					WebcamControlClass.CameraList.Add(device.Name);
				}
			}
			WebcamControlClass.threadForDeviceConnect = new Thread(new ThreadStart(WebcamControlClass.DeviceConnectFunc));
			WebcamControlClass.threadForDeviceConnect.Start();
		}

		// Token: 0x060009D8 RID: 2520 RVA: 0x00038E5C File Offset: 0x0003705C
		private static void DeviceConnectFunc()
		{
			for (int count = 0; count < 5; count++)
			{
				FilterInfoCollection newList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
				if (WebcamControlClass.videoDevices.Count > newList.Count)
				{
					if (WebcamControlClass.WebcamDisconnected != null)
					{
						WebcamControlClass.WebcamDisconnected((from x in WebcamControlClass.videoDevices
						select x.Name).Except(from x in newList
						select x.Name).ToList<string>());
					}
				}
				else if (WebcamControlClass.videoDevices.Count < newList.Count && WebcamControlClass.WebcamConnected != null)
				{
					WebcamControlClass.WebcamConnected((from x in newList
					select x.Name).Except(from x in WebcamControlClass.videoDevices
					select x.Name).ToList<string>());
				}
				Thread.Sleep(1);
			}
			if (WebcamControlClass.threadForDeviceConnect != null && WebcamControlClass.threadForDeviceConnect.IsAlive)
			{
				WebcamControlClass.threadForDeviceConnect.Abort();
				Debug.Log("WebcamControlClass", "threadfordeviceconnect aborted in DeviceConnectFunc", "DeviceConnectFunc", 164);
			}
		}

		// Token: 0x060009D9 RID: 2521 RVA: 0x00038FBD File Offset: 0x000371BD
		public static void WebcamClose()
		{
			WebcamControlClass.StopWebcam();
			if (WebcamControlClass.threadForDeviceConnect != null && WebcamControlClass.threadForDeviceConnect.IsAlive)
			{
				WebcamControlClass.threadForDeviceConnect.Abort();
			}
		}

		// Token: 0x060009DA RID: 2522 RVA: 0x00038FE4 File Offset: 0x000371E4
		public static Task SelectCamera(string cameraName)
		{
			WebcamControlClass.<SelectCamera>d__44 <SelectCamera>d__;
			<SelectCamera>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SelectCamera>d__.cameraName = cameraName;
			<SelectCamera>d__.<>1__state = -1;
			<SelectCamera>d__.<>t__builder.Start<WebcamControlClass.<SelectCamera>d__44>(ref <SelectCamera>d__);
			return <SelectCamera>d__.<>t__builder.Task;
		}

		// Token: 0x060009DB RID: 2523 RVA: 0x00039027 File Offset: 0x00037227
		private static void VideoSourceErrorHandler(object sender, string eventArgs)
		{
			Debug.Log("VideoSourceError", "Video feed source error: " + eventArgs, "VideoSourceErrorHandler", 291);
			MessageBoxWindow.CreateWindow("Webcam Error", eventArgs, null, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
		}

		// Token: 0x060009DC RID: 2524 RVA: 0x00039058 File Offset: 0x00037258
		public static void WebcamThreadControl()
		{
			if (MainSettingsPage.selectedCameraType != MainSettingsPage.CameraType.Webcam)
			{
				if (WebcamControlClass.openCvCameraHelper == null)
				{
					return;
				}
				WebcamControlClass.openCvCameraHelper._isRunning = false;
			}
		}

		// Token: 0x060009DD RID: 2525 RVA: 0x00039078 File Offset: 0x00037278
		private static void TryOpenVideoWriter(string fileRecordPath, double fps, int CameraPreviewWidth, int CameraPreviewHeight)
		{
			int[] codecs = new int[]
			{
				FourCC.H264,
				FourCC.H265,
				FourCC.MP4V
			};
			foreach (int codec in codecs)
			{
				try
				{
					WebcamControlClass.staticWriter.Open(fileRecordPath, codec, fps, new OpenCvSharp.Size(CameraPreviewWidth, CameraPreviewHeight), true);
					if (WebcamControlClass.staticWriter.IsOpened())
					{
						break;
					}
				}
				catch (Exception ex)
				{
				}
			}
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x000390F8 File Offset: 0x000372F8
		public static void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			WebcamControlClass.<VideoSource_NewFrame>d__48 <VideoSource_NewFrame>d__;
			<VideoSource_NewFrame>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<VideoSource_NewFrame>d__.eventArgs = eventArgs;
			<VideoSource_NewFrame>d__.<>1__state = -1;
			<VideoSource_NewFrame>d__.<>t__builder.Start<WebcamControlClass.<VideoSource_NewFrame>d__48>(ref <VideoSource_NewFrame>d__);
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x00039130 File Offset: 0x00037330
		private static void SaveImage(byte[] imageData, string filePath)
		{
			using (MemoryStream ms = new MemoryStream(imageData))
			{
				using (Image img = Image.FromStream(ms))
				{
					img.Save(filePath, ImageFormat.Png);
				}
			}
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x0003918C File Offset: 0x0003738C
		public static void TakePicture(bool isStop)
		{
			if (WebcamControlClass.openCvCameraHelper._capture != null && WebcamControlClass.openCvCameraHelper._capture.IsOpened())
			{
				WebcamControlClass.isCapturedPhoto = true;
				WebcamControlClass.isTopStatic = isStop;
			}
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x000391B8 File Offset: 0x000373B8
		public static void TakeVideo(int recordingTime)
		{
			WebcamControlClass.fileRecordPath = Path.Combine(WebcamControlClass.filePathFolder, DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss_ffff") + ".mp4");
			if (WebcamControlClass.openCvCameraHelper._capture != null && WebcamControlClass.openCvCameraHelper._capture.IsOpened())
			{
				WebcamControlClass.staticWriter = null;
				WebcamControlClass.recordingTimeInSeconds = recordingTime;
				WebcamControlClass.videoStartTime = DateTime.Now;
				WebcamControlClass.isvideoStart = true;
			}
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0003922C File Offset: 0x0003742C
		public static Task StartWebcam()
		{
			WebcamControlClass.<StartWebcam>d__52 <StartWebcam>d__;
			<StartWebcam>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<StartWebcam>d__.<>1__state = -1;
			<StartWebcam>d__.<>t__builder.Start<WebcamControlClass.<StartWebcam>d__52>(ref <StartWebcam>d__);
			return <StartWebcam>d__.<>t__builder.Task;
		}

		// Token: 0x060009E3 RID: 2531 RVA: 0x00039268 File Offset: 0x00037468
		public static void StopWebcam()
		{
			if (WebcamControlClass.openCvCameraHelper == null)
			{
				WebcamControlClass.openCvCameraHelper = new OpenCvCameraHelper();
			}
			try
			{
				if (WebcamControlClass.openCvCameraHelper._capture != null && !WebcamControlClass.openCvCameraHelper._capture.IsOpened())
				{
					WebcamControlClass.openCvCameraHelper.Stop();
				}
			}
			catch (Exception ex)
			{
				Debug.Log("Stop Webcam", ex.ToString(), "StopWebcam", 462);
			}
		}

		// Token: 0x0400099F RID: 2463
		public static readonly object Lock = new object();

		// Token: 0x040009A0 RID: 2464
		private static FilterInfoCollection videoDevices;

		// Token: 0x040009A1 RID: 2465
		public static OpenCvCameraHelper openCvCameraHelper;

		// Token: 0x040009A2 RID: 2466
		public static bool isCapturedPhoto = false;

		// Token: 0x040009A4 RID: 2468
		private static string selectedCameraName;

		// Token: 0x040009A8 RID: 2472
		public static string FolderForTemp;

		// Token: 0x040009A9 RID: 2473
		public static bool isLiveViewActive = false;

		// Token: 0x040009AB RID: 2475
		public static Bitmap staticBitmapImage;

		// Token: 0x040009AD RID: 2477
		private static string filePathFolder;

		// Token: 0x040009AE RID: 2478
		private static Thread threadForDeviceConnect;

		// Token: 0x040009AF RID: 2479
		public static int CameraPreviewWidth = 0;

		// Token: 0x040009B0 RID: 2480
		public static int CameraPreviewHeight = 0;

		// Token: 0x040009B1 RID: 2481
		private static bool isTopStatic = false;

		// Token: 0x040009B2 RID: 2482
		[NonSerialized]
		private static VideoWriter staticWriter;

		// Token: 0x040009B3 RID: 2483
		private static DateTime videoStartTime;

		// Token: 0x040009B4 RID: 2484
		private static bool isvideoStart = false;

		// Token: 0x040009B5 RID: 2485
		private static int recordingTimeInSeconds = 0;

		// Token: 0x040009B6 RID: 2486
		private static string fileRecordPath;

		// Token: 0x040009B7 RID: 2487
		private static string cameraNameStatic;
	}
}
