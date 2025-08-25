using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SimpleWifi;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.WiFi;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x0200009C RID: 156
	public static class GoProCameraControlClass
	{
		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06000935 RID: 2357 RVA: 0x00034228 File Offset: 0x00032428
		// (remove) Token: 0x06000936 RID: 2358 RVA: 0x0003425C File Offset: 0x0003245C
		public static event Action<bool, bool> returnWifiConnected;

		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06000937 RID: 2359 RVA: 0x00034290 File Offset: 0x00032490
		// (remove) Token: 0x06000938 RID: 2360 RVA: 0x000342C4 File Offset: 0x000324C4
		public static event Action<bool> scanCompleted;

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000939 RID: 2361 RVA: 0x000342F7 File Offset: 0x000324F7
		// (set) Token: 0x0600093A RID: 2362 RVA: 0x000342FE File Offset: 0x000324FE
		public static ObservableCollection<GoProCameraControlClass.GDeviceInformation> Devices { get; set; } = new ObservableCollection<GoProCameraControlClass.GDeviceInformation>();

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x0600093B RID: 2363 RVA: 0x00034306 File Offset: 0x00032506
		// (set) Token: 0x0600093C RID: 2364 RVA: 0x0003430D File Offset: 0x0003250D
		public static bool Encoding
		{
			get
			{
				return GoProCameraControlClass.mEncoding;
			}
			set
			{
				GoProCameraControlClass.mEncoding = value;
				if (GoProCameraControlClass.PropertyChanged != null)
				{
					GoProCameraControlClass.PropertyChanged(GoProCameraControlClass.PropertyChanged, new PropertyChangedEventArgs("Encoding"));
				}
			}
		}

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x0600093D RID: 2365 RVA: 0x00034335 File Offset: 0x00032535
		public static bool IsCameraReady
		{
			get
			{
				return GoProCameraControlClass.isGoProWifiConnectedBool;
			}
		}

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x0600093E RID: 2366 RVA: 0x0003433C File Offset: 0x0003253C
		// (set) Token: 0x0600093F RID: 2367 RVA: 0x00034343 File Offset: 0x00032543
		public static int BatteryLevel
		{
			get
			{
				return GoProCameraControlClass.mBatterylevel;
			}
			set
			{
				GoProCameraControlClass.mBatterylevel = value;
				if (GoProCameraControlClass.PropertyChanged != null)
				{
					GoProCameraControlClass.PropertyChanged(GoProCameraControlClass.PropertyChanged, new PropertyChangedEventArgs("BatteryLevel"));
				}
			}
		}

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000940 RID: 2368 RVA: 0x0003436B File Offset: 0x0003256B
		// (set) Token: 0x06000941 RID: 2369 RVA: 0x00034372 File Offset: 0x00032572
		public static bool WifiOn
		{
			get
			{
				return GoProCameraControlClass.mWifiOn;
			}
			set
			{
				GoProCameraControlClass.mWifiOn = value;
				if (GoProCameraControlClass.PropertyChanged == null)
				{
					return;
				}
				GoProCameraControlClass.PropertyChanged(GoProCameraControlClass.PropertyChanged, new PropertyChangedEventArgs("WifiOn"));
			}
		}

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06000942 RID: 2370 RVA: 0x0003439C File Offset: 0x0003259C
		// (remove) Token: 0x06000943 RID: 2371 RVA: 0x000343D0 File Offset: 0x000325D0
		public static event PropertyChangedEventHandler PropertyChanged;

		// Token: 0x06000944 RID: 2372 RVA: 0x00034404 File Offset: 0x00032604
		public static bool GetCurrentWifiSSIDCheck(string wifiName)
		{
			Wifi wifi = new Wifi();
			IEnumerable<AccessPoint> accessPoints = from ap in wifi.GetAccessPoints()
			orderby ap.SignalStrength descending
			select ap;
			int i = 0;
			List<string> Liste = new List<string>();
			foreach (AccessPoint ap2 in accessPoints)
			{
				Liste.Add(string.Format("{0}. {1} {2}% Connected: {3}", new object[]
				{
					i++,
					ap2.Name,
					ap2.SignalStrength,
					ap2.IsConnected
				}));
			}
			return (from x in accessPoints
			where x.Name == wifiName && x.IsConnected
			select x).FirstOrDefault<AccessPoint>() != null;
		}

		// Token: 0x06000945 RID: 2373 RVA: 0x000344F8 File Offset: 0x000326F8
		public static List<string> GetCurrentWifiSSID()
		{
			Wifi wifi = new Wifi();
			List<string> result;
			try
			{
				IEnumerable<AccessPoint> accessPoints = from ap in wifi.GetAccessPoints()
				orderby ap.SignalStrength descending
				select ap;
				List<string> Liste = new List<string>();
				foreach (AccessPoint ap2 in accessPoints)
				{
					if (ap2.IsConnected)
					{
						Liste.Add(ap2.Name);
					}
				}
				result = Liste;
			}
			catch (Exception ex)
			{
				MainSettingsPage.StatusOutput("Cant Access Wifi. Check Your Wifi Settings and Restart the Application.", MainSettingsPage.StatusOutputColor.Warning);
				MessageBoxWindow.CreateWindow("Wifi Error", "Please Open Your Wifi", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
				List<string> Liste2 = new List<string>
				{
					"Error"
				};
				result = Liste2;
			}
			return result;
		}

		// Token: 0x06000946 RID: 2374 RVA: 0x000345E8 File Offset: 0x000327E8
		public static Task GoProScanAsync()
		{
			GoProCameraControlClass.<GoProScanAsync>d__70 <GoProScanAsync>d__;
			<GoProScanAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GoProScanAsync>d__.<>1__state = -1;
			<GoProScanAsync>d__.<>t__builder.Start<GoProCameraControlClass.<GoProScanAsync>d__70>(ref <GoProScanAsync>d__);
			return <GoProScanAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000947 RID: 2375 RVA: 0x00034624 File Offset: 0x00032824
		public static void GoProPair(GoProCameraControlClass.GDeviceInformation device)
		{
			GoProCameraControlClass.<GoProPair>d__71 <GoProPair>d__;
			<GoProPair>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GoProPair>d__.device = device;
			<GoProPair>d__.<>1__state = -1;
			<GoProPair>d__.<>t__builder.Start<GoProCameraControlClass.<GoProPair>d__71>(ref <GoProPair>d__);
		}

		// Token: 0x06000948 RID: 2376 RVA: 0x0003465C File Offset: 0x0003285C
		public static void GoProConnectBLE(GoProCameraControlClass.GDeviceInformation device)
		{
			GoProCameraControlClass.<GoProConnectBLE>d__72 <GoProConnectBLE>d__;
			<GoProConnectBLE>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GoProConnectBLE>d__.device = device;
			<GoProConnectBLE>d__.<>1__state = -1;
			<GoProConnectBLE>d__.<>t__builder.Start<GoProCameraControlClass.<GoProConnectBLE>d__72>(ref <GoProConnectBLE>d__);
		}

		// Token: 0x06000949 RID: 2377 RVA: 0x00034693 File Offset: 0x00032893
		public static void GoProShutterOn()
		{
			GoProCameraControlClass.isRecord = !GoProCameraControlClass.isRecord;
			GoProCameraControlClass.Shutter_OnOff_HTTP("start");
		}

		// Token: 0x0600094A RID: 2378 RVA: 0x000346AC File Offset: 0x000328AC
		public static void GoProShutterOff_Click(bool isCancel, string path)
		{
			GoProCameraControlClass.Shutter_OnOff_HTTP("stop");
			GoProCameraControlClass.PreviewStop();
			new Thread(delegate()
			{
				Thread.Sleep(1000);
				if (!isCancel)
				{
					GoProCameraControlClass.Download_Media(path);
				}
			}).Start();
		}

		// Token: 0x0600094B RID: 2379 RVA: 0x000346F4 File Offset: 0x000328F4
		private static void ToggleShutter(int onOff)
		{
			GoProCameraControlClass.<ToggleShutter>d__75 <ToggleShutter>d__;
			<ToggleShutter>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ToggleShutter>d__.onOff = onOff;
			<ToggleShutter>d__.<>1__state = -1;
			<ToggleShutter>d__.<>t__builder.Start<GoProCameraControlClass.<ToggleShutter>d__75>(ref <ToggleShutter>d__);
		}

		// Token: 0x0600094C RID: 2380 RVA: 0x0003472C File Offset: 0x0003292C
		private static void Shutter_OnOff_HTTP(string command)
		{
			try
			{
				string url = "http://10.5.5.9/gopro/camera/shutter/" + command;
				WebRequest request = WebRequest.Create(url);
				request.Method = "GET";
				request.Timeout = 1000;
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader reader = new StreamReader(webStream);
				string data = reader.ReadToEnd();
			}
			catch (WebException ex)
			{
				Console.WriteLine("İstek gönderilirken bir hata oluştu: " + ex.Message);
				if (command == "stop")
				{
					GoProCameraControlClass.Shutter_OnOff_HTTP("stop");
				}
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Beklenmeyen bir hata oluştu: " + ex2.Message);
				if (command == "stop")
				{
					GoProCameraControlClass.Shutter_OnOff_HTTP("stop");
				}
			}
		}

		// Token: 0x0600094D RID: 2381 RVA: 0x00034804 File Offset: 0x00032A04
		public static void PreviewStart()
		{
			try
			{
				string url = "http://10.5.5.9:8080/gopro/camera/stream/start";
				WebRequest request = WebRequest.Create(url);
				request.Method = "GET";
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader reader = new StreamReader(webStream);
				string data = reader.ReadToEnd();
			}
			catch (WebException ex)
			{
				Console.WriteLine("İstek gönderilirken bir hata oluştu: " + ex.Message);
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Beklenmeyen bir hata oluştu: " + ex2.Message);
			}
		}

		// Token: 0x0600094E RID: 2382 RVA: 0x0003489C File Offset: 0x00032A9C
		public static void PreviewStop()
		{
			try
			{
				string url = "http://10.5.5.9:8080/gopro/camera/stream/stop";
				WebRequest request = WebRequest.Create(url);
				request.Method = "GET";
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader reader = new StreamReader(webStream);
				string data = reader.ReadToEnd();
			}
			catch (WebException ex)
			{
				Console.WriteLine("İstek gönderilirken bir hata oluştu: " + ex.Message);
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Beklenmeyen bir hata oluştu: " + ex2.Message);
			}
		}

		// Token: 0x0600094F RID: 2383 RVA: 0x00034934 File Offset: 0x00032B34
		public static void GoProSetConfigurations()
		{
		}

		// Token: 0x06000950 RID: 2384 RVA: 0x00034938 File Offset: 0x00032B38
		public static void GoProSetVideoMode()
		{
			try
			{
				string url = "http://10.5.5.9:8080/gopro/camera/presets/set_group?id=1000";
				WebRequest request = WebRequest.Create(url);
				request.Method = "GET";
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader reader = new StreamReader(webStream);
				string data = reader.ReadToEnd();
			}
			catch (WebException ex)
			{
				Console.WriteLine("İstek gönderilirken bir hata oluştu: " + ex.Message);
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Beklenmeyen bir hata oluştu: " + ex2.Message);
			}
		}

		// Token: 0x06000951 RID: 2385 RVA: 0x000349D0 File Offset: 0x00032BD0
		public static void GoProSet120FPS()
		{
			try
			{
				string url = "http://10.5.5.9:8080/gopro/camera/setting?setting=3&option=1";
				WebRequest request = WebRequest.Create(url);
				request.Method = "GET";
				request.UseDefaultCredentials = true;
				request.Timeout = 1500;
				WebResponse webResponse = request.GetResponse();
				Stream webStream = webResponse.GetResponseStream();
				StreamReader reader = new StreamReader(webStream);
				string data = reader.ReadToEnd();
			}
			catch (WebException ex)
			{
				if (ex.Status == WebExceptionStatus.Timeout)
				{
					Debug.Log("GoProSet120FPS", "İstek zaman aşımına uğradı: " + ex.Message, "GoProSet120FPS", 754);
				}
				else
				{
					Debug.Log("GoProSet120FPS", "İstek gönderilirken bir hata oluştu: " + ex.Message, "GoProSet120FPS", 760);
				}
			}
			catch (Exception ex2)
			{
				Debug.Log("GoProSet120FPS", "Beklenmeyen bir hata oluştu: " + ex2.Message, "GoProSet120FPS", 767);
			}
		}

		// Token: 0x06000952 RID: 2386 RVA: 0x00034ACC File Offset: 0x00032CCC
		private static void MDeviceWatcher_Stopped(DeviceWatcher sender, object args)
		{
			MainSettingsPage.StatusOutput("Scanning Completed. Select Your GoPro!", MainSettingsPage.StatusOutputColor.Success);
			GoProCameraControlClass.isScanning = false;
			SettingsPage.mainSettingsPage.UpdateCameraListBox();
		}

		// Token: 0x06000953 RID: 2387 RVA: 0x00034AE9 File Offset: 0x00032CE9
		private static void MDeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
		{
			GoProCameraControlClass.mDeviceWatcher.Stop();
			MainSettingsPage.StatusOutput("Scanning Completed", MainSettingsPage.StatusOutputColor.Success);
			GoProCameraControlClass.scanCompleted(true);
			GoProCameraControlClass.isScanning = false;
		}

		// Token: 0x06000954 RID: 2388 RVA: 0x00034B14 File Offset: 0x00032D14
		private static void MDeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			int i;
			int j;
			for (i = 0; i < GoProCameraControlClass.Devices.Count; i = j + 1)
			{
				if (GoProCameraControlClass.Devices[i].DeviceInfo.Id == args.Id)
				{
					Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
					{
						GoProCameraControlClass.Devices.RemoveAt(i);
					}), Array.Empty<object>());
					return;
				}
				j = i;
			}
		}

		// Token: 0x06000955 RID: 2389 RVA: 0x00034B9C File Offset: 0x00032D9C
		private static void MDeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
		{
			GoProCameraControlClass.<>c__DisplayClass85_0 CS$<>8__locals1 = new GoProCameraControlClass.<>c__DisplayClass85_0();
			CS$<>8__locals1.args = args;
			CS$<>8__locals1.isPresent = false;
			CS$<>8__locals1.isConnected = false;
			bool found = false;
			if (CS$<>8__locals1.args.Properties.ContainsKey("System.Devices.Aep.Bluetooth.Le.IsConnectable"))
			{
				CS$<>8__locals1.isPresent = (bool)CS$<>8__locals1.args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"];
			}
			if (CS$<>8__locals1.args.Properties.ContainsKey("System.Devices.Aep.IsConnected"))
			{
				CS$<>8__locals1.isConnected = (bool)CS$<>8__locals1.args.Properties["System.Devices.Aep.IsConnected"];
			}
			int i;
			int j;
			for (i = 0; i < GoProCameraControlClass.Devices.Count; i = j + 1)
			{
				if (GoProCameraControlClass.Devices[i].DeviceInfo.Id == CS$<>8__locals1.args.Id)
				{
					found = true;
					Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
					{
						GoProCameraControlClass.Devices[i].DeviceInfo.Update(CS$<>8__locals1.args);
						GoProCameraControlClass.Devices[i].IsPresent = CS$<>8__locals1.isPresent;
						GoProCameraControlClass.Devices[i].IsConnected = CS$<>8__locals1.isConnected;
					}), Array.Empty<object>());
					break;
				}
				j = i;
			}
			if (!found && (CS$<>8__locals1.isPresent | CS$<>8__locals1.isConnected) && GoProCameraControlClass.mAllDevices.ContainsKey(CS$<>8__locals1.args.Id))
			{
				GoProCameraControlClass.mAllDevices[CS$<>8__locals1.args.Id].Update(CS$<>8__locals1.args);
				Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
				{
					GoProCameraControlClass.Devices.Add(new GoProCameraControlClass.GDeviceInformation(GoProCameraControlClass.mAllDevices[CS$<>8__locals1.args.Id], CS$<>8__locals1.isPresent, CS$<>8__locals1.isConnected));
					if (!GoProCameraControlClass.DeviceList.Contains(GoProCameraControlClass.Devices[GoProCameraControlClass.Devices.Count - 1].DeviceInfo.Name))
					{
						GoProCameraControlClass.DeviceList.Add(GoProCameraControlClass.Devices[GoProCameraControlClass.Devices.Count - 1].DeviceInfo.Name);
					}
				}), Array.Empty<object>());
			}
		}

		// Token: 0x06000956 RID: 2390 RVA: 0x00034D2C File Offset: 0x00032F2C
		private static void MDeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
		{
			bool isPresent = false;
			bool isConnected = false;
			if (args.Properties.ContainsKey("System.Devices.Aep.Bluetooth.Le.IsConnectable"))
			{
				isPresent = (bool)args.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"];
			}
			if (args.Properties.ContainsKey("System.Devices.Aep.IsConnected"))
			{
				isConnected = (bool)args.Properties["System.Devices.Aep.IsConnected"];
			}
			if (args.Name != "" && args.Name.Contains("GoPro"))
			{
				bool found = false;
				if (!GoProCameraControlClass.mAllDevices.ContainsKey(args.Id))
				{
					GoProCameraControlClass.mAllDevices.Add(args.Id, args);
				}
				int j;
				int i;
				for (i = 0; i < GoProCameraControlClass.Devices.Count; i = j + 1)
				{
					if (GoProCameraControlClass.Devices[i].DeviceInfo.Id == args.Id)
					{
						found = true;
						Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
						{
							GoProCameraControlClass.Devices[i].DeviceInfo = args;
							GoProCameraControlClass.Devices[i].IsPresent = isPresent;
							GoProCameraControlClass.Devices[i].IsConnected = isConnected;
						}), Array.Empty<object>());
						break;
					}
					j = i;
				}
				if (!found && (isPresent | isConnected))
				{
					Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
					{
						GoProCameraControlClass.Devices.Add(new GoProCameraControlClass.GDeviceInformation(args, isPresent, isConnected));
						if (!GoProCameraControlClass.DeviceList.Contains(GoProCameraControlClass.Devices[GoProCameraControlClass.Devices.Count - 1].DeviceInfo.Name))
						{
							GoProCameraControlClass.DeviceList.Add(GoProCameraControlClass.Devices[GoProCameraControlClass.Devices.Count - 1].DeviceInfo.Name);
						}
					}), Array.Empty<object>());
				}
			}
		}

		// Token: 0x06000957 RID: 2391 RVA: 0x00034EEB File Offset: 0x000330EB
		private static void Custom_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
		{
			args.Accept();
		}

		// Token: 0x06000958 RID: 2392 RVA: 0x00034EF4 File Offset: 0x000330F4
		private static void SetThirdPartySource()
		{
			GoProCameraControlClass.<SetThirdPartySource>d__88 <SetThirdPartySource>d__;
			<SetThirdPartySource>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SetThirdPartySource>d__.<>1__state = -1;
			<SetThirdPartySource>d__.<>t__builder.Start<GoProCameraControlClass.<SetThirdPartySource>d__88>(ref <SetThirdPartySource>d__);
		}

		// Token: 0x06000959 RID: 2393 RVA: 0x00034F24 File Offset: 0x00033124
		private static void MNotifyQueryResp_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			DataReader reader = DataReader.FromBuffer(args.CharacteristicValue);
			byte[] myBytes = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(myBytes);
			int newLength = GoProCameraControlClass.ReadBytesIntoBuffer(myBytes, GoProCameraControlClass.mBufQ);
			if (newLength > 0)
			{
				GoProCameraControlClass.mExpectedLengthQ = newLength;
			}
			if (GoProCameraControlClass.mExpectedLengthQ == GoProCameraControlClass.mBufQ.Count)
			{
				if ((GoProCameraControlClass.mBufQ[0] == 83 || GoProCameraControlClass.mBufQ[0] == 147) && GoProCameraControlClass.mBufQ[1] == 0)
				{
					for (int i = 0; i < GoProCameraControlClass.mBufQ.Count; i += (int)(2 + GoProCameraControlClass.mBufQ[i + 1]))
					{
						if (GoProCameraControlClass.mBufQ[i] == 10)
						{
							GoProCameraControlClass.Encoding = (GoProCameraControlClass.mBufQ[i + 2] > 0);
						}
						if (GoProCameraControlClass.mBufQ[i] == 70)
						{
							GoProCameraControlClass.BatteryLevel = (int)GoProCameraControlClass.mBufQ[i + 2];
						}
						if (GoProCameraControlClass.mBufQ[i] == 69)
						{
							GoProCameraControlClass.WifiOn = (GoProCameraControlClass.mBufQ[i + 2] == 1);
						}
					}
				}
				GoProCameraControlClass.mBufQ.Clear();
				GoProCameraControlClass.mExpectedLengthQ = 0;
			}
		}

		// Token: 0x0600095A RID: 2394 RVA: 0x00035050 File Offset: 0x00033250
		private static void MNotifySettings_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			DataReader reader = DataReader.FromBuffer(args.CharacteristicValue);
			byte[] myBytes = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(myBytes);
			int newLength = GoProCameraControlClass.ReadBytesIntoBuffer(myBytes, GoProCameraControlClass.mBufSet);
			if (newLength > 0)
			{
				GoProCameraControlClass.mExpectedLengthSet = newLength;
			}
			if (GoProCameraControlClass.mExpectedLengthSet == GoProCameraControlClass.mBufSet.Count)
			{
				GoProCameraControlClass.mBufSet.Clear();
			}
		}

		// Token: 0x0600095B RID: 2395 RVA: 0x000350B0 File Offset: 0x000332B0
		private static void MNotifyCmds_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
		{
			DataReader reader = DataReader.FromBuffer(args.CharacteristicValue);
			byte[] myBytes = new byte[reader.UnconsumedBufferLength];
			reader.ReadBytes(myBytes);
			int newLength = GoProCameraControlClass.ReadBytesIntoBuffer(myBytes, GoProCameraControlClass.mBufCmd);
			if (newLength > 0)
			{
				GoProCameraControlClass.mExpectedLengthCmd = newLength;
			}
			if (GoProCameraControlClass.mExpectedLengthCmd == GoProCameraControlClass.mBufCmd.Count)
			{
				GoProCameraControlClass.mBufCmd.Clear();
			}
		}

		// Token: 0x0600095C RID: 2396 RVA: 0x00035110 File Offset: 0x00033310
		private static int ReadBytesIntoBuffer(byte[] bytes, List<byte> mBuf)
		{
			int returnLength = -1;
			int startbyte = 1;
			int theseBytes = bytes.Length;
			if ((bytes[0] & 32) > 0)
			{
				startbyte = 2;
				int len = (int)(bytes[0] & 15) << 8 | (int)bytes[1];
				returnLength = len;
			}
			else if ((bytes[0] & 64) > 0)
			{
				startbyte = 3;
				int len2 = (int)bytes[1] << 8 | (int)bytes[2];
				returnLength = len2;
			}
			else if ((bytes[0] & 128) <= 0)
			{
				returnLength = (int)bytes[0];
			}
			for (int i = startbyte; i < theseBytes; i++)
			{
				mBuf.Add(bytes[i]);
			}
			return returnLength;
		}

		// Token: 0x0600095D RID: 2397 RVA: 0x00035188 File Offset: 0x00033388
		private static void MBLED_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
		{
			if (sender.ConnectionStatus == 1)
			{
				GoProCameraControlClass.isBLEConnected = true;
				MainSettingsPage.StatusOutput("BLE baglantisi kuruldu.", MainSettingsPage.StatusOutputColor.Success);
				Application.Current.Dispatcher.Invoke(delegate()
				{
					SettingsPage.mainSettingsPage.TestPhotoButton.IsEnabled = true;
				});
				GoProCameraControlClass.TurnGoProWifiOnAndConnect(0);
			}
			else
			{
				MainSettingsPage.StatusOutput("BLE DISCONNECTED", MainSettingsPage.StatusOutputColor.Error);
				GoProCameraControlClass.isBLEConnected = false;
			}
			GoProCameraControlClass.isConnectingBLE = false;
		}

		// Token: 0x0600095E RID: 2398 RVA: 0x000351FC File Offset: 0x000333FC
		private static Task<string> GetAdapterNameById(string adapterId)
		{
			GoProCameraControlClass.<GetAdapterNameById>d__100 <GetAdapterNameById>d__;
			<GetAdapterNameById>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<GetAdapterNameById>d__.adapterId = adapterId;
			<GetAdapterNameById>d__.<>1__state = -1;
			<GetAdapterNameById>d__.<>t__builder.Start<GoProCameraControlClass.<GetAdapterNameById>d__100>(ref <GetAdapterNameById>d__);
			return <GetAdapterNameById>d__.<>t__builder.Task;
		}

		// Token: 0x0600095F RID: 2399 RVA: 0x00035240 File Offset: 0x00033440
		public static Task IsGoProWifiConnected(ConnectedGoPro connectedGoPro)
		{
			GoProCameraControlClass.<IsGoProWifiConnected>d__101 <IsGoProWifiConnected>d__;
			<IsGoProWifiConnected>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<IsGoProWifiConnected>d__.connectedGoPro = connectedGoPro;
			<IsGoProWifiConnected>d__.<>1__state = -1;
			<IsGoProWifiConnected>d__.<>t__builder.Start<GoProCameraControlClass.<IsGoProWifiConnected>d__101>(ref <IsGoProWifiConnected>d__);
			return <IsGoProWifiConnected>d__.<>t__builder.Task;
		}

		// Token: 0x06000960 RID: 2400 RVA: 0x00035284 File Offset: 0x00033484
		public static Task<WiFiConnectionResult> GoProWifiConnectAsyncWithTimeout(double time, IAsyncOperation<WiFiConnectionResult> selectTask)
		{
			GoProCameraControlClass.<GoProWifiConnectAsyncWithTimeout>d__102 <GoProWifiConnectAsyncWithTimeout>d__;
			<GoProWifiConnectAsyncWithTimeout>d__.<>t__builder = AsyncTaskMethodBuilder<WiFiConnectionResult>.Create();
			<GoProWifiConnectAsyncWithTimeout>d__.time = time;
			<GoProWifiConnectAsyncWithTimeout>d__.selectTask = selectTask;
			<GoProWifiConnectAsyncWithTimeout>d__.<>1__state = -1;
			<GoProWifiConnectAsyncWithTimeout>d__.<>t__builder.Start<GoProCameraControlClass.<GoProWifiConnectAsyncWithTimeout>d__102>(ref <GoProWifiConnectAsyncWithTimeout>d__);
			return <GoProWifiConnectAsyncWithTimeout>d__.<>t__builder.Task;
		}

		// Token: 0x06000961 RID: 2401 RVA: 0x000352D0 File Offset: 0x000334D0
		public static Task DoTaskWithTimeOut(double time, Task selectTask)
		{
			GoProCameraControlClass.<DoTaskWithTimeOut>d__103 <DoTaskWithTimeOut>d__;
			<DoTaskWithTimeOut>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DoTaskWithTimeOut>d__.time = time;
			<DoTaskWithTimeOut>d__.selectTask = selectTask;
			<DoTaskWithTimeOut>d__.<>1__state = -1;
			<DoTaskWithTimeOut>d__.<>t__builder.Start<GoProCameraControlClass.<DoTaskWithTimeOut>d__103>(ref <DoTaskWithTimeOut>d__);
			return <DoTaskWithTimeOut>d__.<>t__builder.Task;
		}

		// Token: 0x06000962 RID: 2402 RVA: 0x0003531C File Offset: 0x0003351C
		private static void TurnGoProWifiOnAndConnect(int count)
		{
			GoProCameraControlClass.<TurnGoProWifiOnAndConnect>d__104 <TurnGoProWifiOnAndConnect>d__;
			<TurnGoProWifiOnAndConnect>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TurnGoProWifiOnAndConnect>d__.count = count;
			<TurnGoProWifiOnAndConnect>d__.<>1__state = -1;
			<TurnGoProWifiOnAndConnect>d__.<>t__builder.Start<GoProCameraControlClass.<TurnGoProWifiOnAndConnect>d__104>(ref <TurnGoProWifiOnAndConnect>d__);
		}

		// Token: 0x06000963 RID: 2403 RVA: 0x00035354 File Offset: 0x00033554
		private static void GoProWifiConnectAsync(string wifiName, string wifiPass)
		{
			GoProCameraControlClass.<GoProWifiConnectAsync>d__105 <GoProWifiConnectAsync>d__;
			<GoProWifiConnectAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GoProWifiConnectAsync>d__.wifiName = wifiName;
			<GoProWifiConnectAsync>d__.wifiPass = wifiPass;
			<GoProWifiConnectAsync>d__.<>1__state = -1;
			<GoProWifiConnectAsync>d__.<>t__builder.Start<GoProCameraControlClass.<GoProWifiConnectAsync>d__105>(ref <GoProWifiConnectAsync>d__);
		}

		// Token: 0x06000964 RID: 2404 RVA: 0x00035394 File Offset: 0x00033594
		public static Task GoProRetryWifiConnectAsync()
		{
			GoProCameraControlClass.<GoProRetryWifiConnectAsync>d__106 <GoProRetryWifiConnectAsync>d__;
			<GoProRetryWifiConnectAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GoProRetryWifiConnectAsync>d__.<>1__state = -1;
			<GoProRetryWifiConnectAsync>d__.<>t__builder.Start<GoProCameraControlClass.<GoProRetryWifiConnectAsync>d__106>(ref <GoProRetryWifiConnectAsync>d__);
			return <GoProRetryWifiConnectAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000965 RID: 2405 RVA: 0x000353D0 File Offset: 0x000335D0
		private static Task<bool> ToggleGoProWifiAP(int onOff)
		{
			GoProCameraControlClass.<ToggleGoProWifiAP>d__107 <ToggleGoProWifiAP>d__;
			<ToggleGoProWifiAP>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<ToggleGoProWifiAP>d__.onOff = onOff;
			<ToggleGoProWifiAP>d__.<>1__state = -1;
			<ToggleGoProWifiAP>d__.<>t__builder.Start<GoProCameraControlClass.<ToggleGoProWifiAP>d__107>(ref <ToggleGoProWifiAP>d__);
			return <ToggleGoProWifiAP>d__.<>t__builder.Task;
		}

		// Token: 0x06000966 RID: 2406 RVA: 0x00035414 File Offset: 0x00033614
		public static Task<WiFiAdapter> GetWiFiAdapter(string indexString = null)
		{
			GoProCameraControlClass.<GetWiFiAdapter>d__108 <GetWiFiAdapter>d__;
			<GetWiFiAdapter>d__.<>t__builder = AsyncTaskMethodBuilder<WiFiAdapter>.Create();
			<GetWiFiAdapter>d__.indexString = indexString;
			<GetWiFiAdapter>d__.<>1__state = -1;
			<GetWiFiAdapter>d__.<>t__builder.Start<GoProCameraControlClass.<GetWiFiAdapter>d__108>(ref <GetWiFiAdapter>d__);
			return <GetWiFiAdapter>d__.<>t__builder.Task;
		}

		// Token: 0x06000967 RID: 2407 RVA: 0x00035458 File Offset: 0x00033658
		public static List<string> GetAvailableWifiNetworks()
		{
			List<string> wifiNetworks = new List<string>();
			ConnectionOptions options = new ConnectionOptions
			{
				Impersonation = ImpersonationLevel.Impersonate,
				EnablePrivileges = true
			};
			ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2", options);
			scope.Connect();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery("root\\WMI", "SELECT * FROM MSNdis_80211_ServiceSetIdentifier"));
			foreach (ManagementBaseObject managementBaseObject in searcher.Get())
			{
				ManagementObject queryObj = (ManagementObject)managementBaseObject;
				byte[] ssidBytes = (byte[])queryObj["Ndis80211SsId"];
				string ssid = System.Text.Encoding.ASCII.GetString(ssidBytes).Trim(new char[1]);
				wifiNetworks.Add(ssid);
			}
			return wifiNetworks;
		}

		// Token: 0x06000968 RID: 2408 RVA: 0x00035520 File Offset: 0x00033720
		public static bool IsWifiConnected()
		{
			NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in interfaces)
			{
				if (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 && networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000969 RID: 2409 RVA: 0x00035560 File Offset: 0x00033760
		private static void Download_Media(string CapturePath)
		{
			if (GoProCameraControlClass.isDownloading)
			{
				return;
			}
			GoProCameraControlClass.isDownloading = true;
			GoProCameraControlClass.downloadFilePath = CapturePath;
			int maxAttempt = 5;
			int attemptCount = 0;
			bool isSucces = false;
			while (attemptCount < maxAttempt && !isSucces)
			{
				try
				{
					attemptCount++;
					string url = "http://10.5.5.9:8080/videos/DCIM/";
					string urlMediaList = "http://10.5.5.9:8080/gopro/media/list";
					WebRequest request = WebRequest.Create(urlMediaList);
					request.Method = "GET";
					request.Timeout = 2000;
					string json;
					using (WebResponse webResponse = request.GetResponse())
					{
						using (Stream webStream = webResponse.GetResponseStream())
						{
							using (StreamReader reader = new StreamReader(webStream))
							{
								json = reader.ReadToEnd();
							}
						}
					}
					GoProCameraControlClass.MediaList mediaList = JsonConvert.DeserializeObject<GoProCameraControlClass.MediaList>(json, new JsonSerializerSettings
					{
						MissingMemberHandling = MissingMemberHandling.Ignore
					});
					Console.WriteLine(mediaList.id);
					url += mediaList.media[0].d;
					url += "/";
					string fileName = mediaList.media[0].fs[mediaList.media[0].fs.Length - 1].n;
					url += fileName;
					Console.WriteLine(url);
					using (WebClient client = new WebClient())
					{
						client.DownloadFileCompleted += GoProCameraControlClass.Client_DownloadFileCompleted;
						client.DownloadFileAsync(new Uri(url), GoProCameraControlClass.downloadFilePath);
						Console.WriteLine("Download basladi");
					}
					isSucces = true;
				}
				catch (WebException webEx)
				{
					GoProCameraControlClass.currentState = false;
					string errorResponse = "";
					if (webEx.Response != null)
					{
						errorResponse = new StreamReader(webEx.Response.GetResponseStream()).ReadToEnd();
					}
					Debug.Log("Download Media Issue Web Hatası:", webEx.Message + "\n" + errorResponse, "Download_Media", 1742);
					isSucces = false;
					while (!GoProCameraControlClass.currentState)
					{
						Thread.Sleep(2000);
						Console.WriteLine("Bekliyoruz");
					}
					GoProCameraControlClass.Download_Media(GoProCameraControlClass.downloadFilePath);
					Console.WriteLine("Redownload webex: " + GoProCameraControlClass.isGoProWifiConnectedBool.ToString());
				}
				catch (JsonException jsonEx)
				{
					MessageBoxWindow.CreateWindow("Download Media Issue", "Couldn't download media from GoPro. Please Restart the Application.", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					Console.WriteLine("JSON hatası: " + jsonEx.Message);
					isSucces = false;
				}
				catch (Exception ex)
				{
					MessageBoxWindow.CreateWindow("Download Media Issue", "Couldn't download media from GoPro. Please Restart the Application.", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					Console.WriteLine("Bir hata oluştu: " + ex.Message);
					isSucces = false;
				}
			}
			GoProCameraControlClass.isDownloading = false;
		}

		// Token: 0x0600096A RID: 2410 RVA: 0x000358A4 File Offset: 0x00033AA4
		private static void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			Console.WriteLine("download completed");
			if (string.IsNullOrEmpty(GoProCameraControlClass.downloadFilePath))
			{
				Console.WriteLine("DownloadFilePath null");
				if (GoProCameraControlClass.VideoCapturedFailedEvent != null)
				{
					GoProCameraControlClass.VideoCapturedFailedEvent(e.Error.ToString());
					return;
				}
			}
			else
			{
				if (File.Exists(GoProCameraControlClass.downloadFilePath) && new FileInfo(GoProCameraControlClass.downloadFilePath).Length > 0L && e.Error == null && !e.Cancelled)
				{
					Action<string> captureFile = GoProCameraControlClass.CaptureFile;
					if (captureFile != null)
					{
						captureFile(GoProCameraControlClass.downloadFilePath);
					}
					Console.WriteLine("Download basarili");
					return;
				}
				GoProCameraControlClass.Download_Media(GoProCameraControlClass.downloadFilePath);
				Console.WriteLine("ReDownload" + DateTime.Now.ToString());
			}
		}

		// Token: 0x0600096B RID: 2411 RVA: 0x00035968 File Offset: 0x00033B68
		public static bool IsSendKeepAlive()
		{
			for (int i = 0; i < 3; i++)
			{
				using (HttpClient httpClient = new HttpClient())
				{
					string goproIpAddress = "10.5.5.9:8080";
					string apiEndpoint = "http://" + goproIpAddress + "/gopro/camera/keep_alive";
					httpClient.Timeout = TimeSpan.FromSeconds(2.0);
					try
					{
						HttpResponseMessage response = httpClient.GetAsync(apiEndpoint).Result;
						if (response.IsSuccessStatusCode)
						{
							return true;
						}
						return false;
					}
					catch (Exception ex)
					{
						Console.WriteLine("Hata: " + ex.Message);
					}
					Thread.Sleep(500);
				}
			}
			return false;
		}

		// Token: 0x0600096C RID: 2412 RVA: 0x00035A2C File Offset: 0x00033C2C
		public static Task SendKeepAlive()
		{
			GoProCameraControlClass.<SendKeepAlive>d__115 <SendKeepAlive>d__;
			<SendKeepAlive>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendKeepAlive>d__.<>1__state = -1;
			<SendKeepAlive>d__.<>t__builder.Start<GoProCameraControlClass.<SendKeepAlive>d__115>(ref <SendKeepAlive>d__);
			return <SendKeepAlive>d__.<>t__builder.Task;
		}

		// Token: 0x0600096D RID: 2413 RVA: 0x00035A68 File Offset: 0x00033C68
		public static void StopKeepAliveThread()
		{
			GoProCameraControlClass.isSendingKeepAlive = false;
			if (GoProCameraControlClass.keepAliveThread != null && GoProCameraControlClass.keepAliveThread.IsAlive)
			{
				GoProCameraControlClass.keepAliveThread.Join();
			}
			GoProCameraControlClass.keepAliveThread = null;
			Debug.Log("StopKeepAliveThread", "KeepAliveThread ended.", "StopKeepAliveThread", 1934);
		}

		// Token: 0x0600096E RID: 2414 RVA: 0x00035AB8 File Offset: 0x00033CB8
		private static Task TryReconnectStepsAsync()
		{
			GoProCameraControlClass.<TryReconnectStepsAsync>d__118 <TryReconnectStepsAsync>d__;
			<TryReconnectStepsAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<TryReconnectStepsAsync>d__.<>1__state = -1;
			<TryReconnectStepsAsync>d__.<>t__builder.Start<GoProCameraControlClass.<TryReconnectStepsAsync>d__118>(ref <TryReconnectStepsAsync>d__);
			return <TryReconnectStepsAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600096F RID: 2415 RVA: 0x00035AF4 File Offset: 0x00033CF4
		public static Task<bool> IsGoProWifiConnected()
		{
			GoProCameraControlClass.<IsGoProWifiConnected>d__119 <IsGoProWifiConnected>d__;
			<IsGoProWifiConnected>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<IsGoProWifiConnected>d__.<>1__state = -1;
			<IsGoProWifiConnected>d__.<>t__builder.Start<GoProCameraControlClass.<IsGoProWifiConnected>d__119>(ref <IsGoProWifiConnected>d__);
			return <IsGoProWifiConnected>d__.<>t__builder.Task;
		}

		// Token: 0x06000970 RID: 2416 RVA: 0x00035B30 File Offset: 0x00033D30
		private static Task RemoveBluetoothDeviceAsync(string targetDeviceName)
		{
			GoProCameraControlClass.<RemoveBluetoothDeviceAsync>d__120 <RemoveBluetoothDeviceAsync>d__;
			<RemoveBluetoothDeviceAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RemoveBluetoothDeviceAsync>d__.targetDeviceName = targetDeviceName;
			<RemoveBluetoothDeviceAsync>d__.<>1__state = -1;
			<RemoveBluetoothDeviceAsync>d__.<>t__builder.Start<GoProCameraControlClass.<RemoveBluetoothDeviceAsync>d__120>(ref <RemoveBluetoothDeviceAsync>d__);
			return <RemoveBluetoothDeviceAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000971 RID: 2417 RVA: 0x00035B74 File Offset: 0x00033D74
		public static Task<bool> CheckBluetoothStatusAsync()
		{
			GoProCameraControlClass.<CheckBluetoothStatusAsync>d__121 <CheckBluetoothStatusAsync>d__;
			<CheckBluetoothStatusAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<CheckBluetoothStatusAsync>d__.<>1__state = -1;
			<CheckBluetoothStatusAsync>d__.<>t__builder.Start<GoProCameraControlClass.<CheckBluetoothStatusAsync>d__121>(ref <CheckBluetoothStatusAsync>d__);
			return <CheckBluetoothStatusAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000972 RID: 2418 RVA: 0x00035BB0 File Offset: 0x00033DB0
		public static Task<List<DeviceInformation>> GetPairedBleDevicesAsync()
		{
			GoProCameraControlClass.<GetPairedBleDevicesAsync>d__122 <GetPairedBleDevicesAsync>d__;
			<GetPairedBleDevicesAsync>d__.<>t__builder = AsyncTaskMethodBuilder<List<DeviceInformation>>.Create();
			<GetPairedBleDevicesAsync>d__.<>1__state = -1;
			<GetPairedBleDevicesAsync>d__.<>t__builder.Start<GoProCameraControlClass.<GetPairedBleDevicesAsync>d__122>(ref <GetPairedBleDevicesAsync>d__);
			return <GetPairedBleDevicesAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0400091B RID: 2331
		public static bool isRecord = false;

		// Token: 0x0400091C RID: 2332
		public static string downloadFilePath;

		// Token: 0x0400091D RID: 2333
		public static Action<string> CaptureFile;

		// Token: 0x0400091E RID: 2334
		public static Action<string> VideoCapturedFailedEvent;

		// Token: 0x0400091F RID: 2335
		public static List<string> DeviceList = new List<string>();

		// Token: 0x04000922 RID: 2338
		public static string selectedGoProName;

		// Token: 0x04000923 RID: 2339
		public static string connectedGoProStringsFilePath;

		// Token: 0x04000924 RID: 2340
		public static int videoLength = 6;

		// Token: 0x04000925 RID: 2341
		public static double slowMotionVideoStart = 2.0;

		// Token: 0x04000926 RID: 2342
		public static double slowMotionVideoEnd = 4.0;

		// Token: 0x04000927 RID: 2343
		public static bool isSlowMotion = false;

		// Token: 0x04000928 RID: 2344
		public static bool isScanning = false;

		// Token: 0x04000929 RID: 2345
		public static bool isPairing = false;

		// Token: 0x0400092A RID: 2346
		public static bool isConnectingBLE = false;

		// Token: 0x0400092B RID: 2347
		public static bool isBusy = false;

		// Token: 0x0400092C RID: 2348
		public static ConnectedGoPro currentGoPro = null;

		// Token: 0x0400092D RID: 2349
		public static string selectedWifiAdapterName;

		// Token: 0x0400092E RID: 2350
		private static WiFiAdapter selectedWifiAdapter = null;

		// Token: 0x0400092F RID: 2351
		public static WiFiAvailableNetwork goProNetwork = null;

		// Token: 0x04000930 RID: 2352
		public static bool isConnectingWifi;

		// Token: 0x04000931 RID: 2353
		public static bool isGoProWifiConnectedBool = false;

		// Token: 0x04000932 RID: 2354
		public static bool currentState = false;

		// Token: 0x04000933 RID: 2355
		public static bool isWifiError = false;

		// Token: 0x04000934 RID: 2356
		private static bool isSendingKeepAlive = false;

		// Token: 0x04000935 RID: 2357
		public static bool isBLEConnected = false;

		// Token: 0x04000936 RID: 2358
		private static Thread keepAliveThread = null;

		// Token: 0x04000938 RID: 2360
		private static bool mEncoding = false;

		// Token: 0x04000939 RID: 2361
		private static int mBatterylevel = 0;

		// Token: 0x0400093A RID: 2362
		private static bool mWifiOn = false;

		// Token: 0x0400093B RID: 2363
		public static BluetoothLEDevice mBLED = null;

		// Token: 0x0400093C RID: 2364
		public static GattCharacteristic mNotifyCmds = null;

		// Token: 0x0400093D RID: 2365
		public static GattCharacteristic mSendCmds = null;

		// Token: 0x0400093E RID: 2366
		public static GattCharacteristic mSetSettings = null;

		// Token: 0x0400093F RID: 2367
		public static GattCharacteristic mNotifySettings = null;

		// Token: 0x04000940 RID: 2368
		public static GattCharacteristic mSendQueries = null;

		// Token: 0x04000941 RID: 2369
		public static GattCharacteristic mNotifyQueryResp = null;

		// Token: 0x04000942 RID: 2370
		public static GattCharacteristic mReadAPName = null;

		// Token: 0x04000943 RID: 2371
		public static GattCharacteristic mReadAPPass = null;

		// Token: 0x04000945 RID: 2373
		public static DeviceWatcher mDeviceWatcher = null;

		// Token: 0x04000946 RID: 2374
		private static readonly Dictionary<string, DeviceInformation> mAllDevices = new Dictionary<string, DeviceInformation>();

		// Token: 0x04000947 RID: 2375
		private static readonly List<byte> mBufQ = new List<byte>();

		// Token: 0x04000948 RID: 2376
		private static int mExpectedLengthQ = 0;

		// Token: 0x04000949 RID: 2377
		private static readonly List<byte> mBufSet = new List<byte>();

		// Token: 0x0400094A RID: 2378
		private static int mExpectedLengthSet = 0;

		// Token: 0x0400094B RID: 2379
		private static readonly List<byte> mBufCmd = new List<byte>();

		// Token: 0x0400094C RID: 2380
		private static int mExpectedLengthCmd = 0;

		// Token: 0x0400094D RID: 2381
		private static bool isDownloading = false;

		// Token: 0x0400094E RID: 2382
		private static bool isReconnecting = false;

		// Token: 0x0200024A RID: 586
		[BsonIgnoreExtraElements]
		[Serializable]
		public class MediaList
		{
			// Token: 0x04000FBC RID: 4028
			public string id;

			// Token: 0x04000FBD RID: 4029
			public GoProCameraControlClass.Media[] media;
		}

		// Token: 0x0200024B RID: 587
		[BsonIgnoreExtraElements]
		[Serializable]
		public class Media
		{
			// Token: 0x04000FBE RID: 4030
			public string d;

			// Token: 0x04000FBF RID: 4031
			public GoProCameraControlClass.MediaFile[] fs;
		}

		// Token: 0x0200024C RID: 588
		[BsonIgnoreExtraElements]
		[Serializable]
		public class MediaFile
		{
			// Token: 0x04000FC0 RID: 4032
			public string n;

			// Token: 0x04000FC1 RID: 4033
			public string cre;

			// Token: 0x04000FC2 RID: 4034
			public string mod;

			// Token: 0x04000FC3 RID: 4035
			public string glrv;

			// Token: 0x04000FC4 RID: 4036
			public string ls;

			// Token: 0x04000FC5 RID: 4037
			public string s;
		}

		// Token: 0x0200024D RID: 589
		public class GDeviceInformation
		{
			// Token: 0x06001013 RID: 4115 RVA: 0x0005C74F File Offset: 0x0005A94F
			public GDeviceInformation(DeviceInformation inDeviceInformation, bool inPresent, bool inConnected)
			{
				this.DeviceInfo = inDeviceInformation;
				this.IsPresent = inPresent;
				this.IsConnected = inConnected;
			}

			// Token: 0x17000254 RID: 596
			// (get) Token: 0x06001014 RID: 4116 RVA: 0x0005C76C File Offset: 0x0005A96C
			// (set) Token: 0x06001015 RID: 4117 RVA: 0x0005C774 File Offset: 0x0005A974
			public DeviceInformation DeviceInfo { get; set; }

			// Token: 0x17000255 RID: 597
			// (get) Token: 0x06001016 RID: 4118 RVA: 0x0005C77D File Offset: 0x0005A97D
			// (set) Token: 0x06001017 RID: 4119 RVA: 0x0005C785 File Offset: 0x0005A985
			public bool IsPresent { get; set; }

			// Token: 0x17000256 RID: 598
			// (get) Token: 0x06001018 RID: 4120 RVA: 0x0005C78E File Offset: 0x0005A98E
			// (set) Token: 0x06001019 RID: 4121 RVA: 0x0005C796 File Offset: 0x0005A996
			public bool IsConnected { get; set; }

			// Token: 0x17000257 RID: 599
			// (get) Token: 0x0600101A RID: 4122 RVA: 0x0005C79F File Offset: 0x0005A99F
			public bool IsVisible
			{
				get
				{
					return this.IsPresent || this.IsConnected;
				}
			}

			// Token: 0x0600101B RID: 4123 RVA: 0x0005C7B1 File Offset: 0x0005A9B1
			private GDeviceInformation()
			{
			}
		}
	}
}
