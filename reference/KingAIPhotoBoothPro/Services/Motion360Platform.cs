using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;

namespace KingAIPhotoBoothPro.Services
{
	// Token: 0x0200001D RID: 29
	public class Motion360Platform
	{
		// Token: 0x1700002C RID: 44
		// (get) Token: 0x06000109 RID: 265 RVA: 0x00007174 File Offset: 0x00005374
		public static Motion360Platform Instance
		{
			get
			{
				if (Motion360Platform._instance == null)
				{
					object @lock = Motion360Platform._lock;
					lock (@lock)
					{
						if (Motion360Platform._instance == null)
						{
							Motion360Platform._instance = new Motion360Platform();
						}
					}
				}
				return Motion360Platform._instance;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x0600010A RID: 266 RVA: 0x000071CC File Offset: 0x000053CC
		public static bool Ready
		{
			get
			{
				return Motion360Platform.Instance.IsReady;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x0600010B RID: 267 RVA: 0x000071D8 File Offset: 0x000053D8
		private bool IsReady
		{
			get
			{
				return this._serialPort != null && this._serialPort.IsOpen;
			}
		}

		// Token: 0x0600010C RID: 268 RVA: 0x000071F0 File Offset: 0x000053F0
		public Motion360Platform()
		{
			this.Initialize();
		}

		// Token: 0x0600010D RID: 269 RVA: 0x00007244 File Offset: 0x00005444
		private void Initialize()
		{
			if (this.RunPlatformCanceled)
			{
				return;
			}
			if (this._serialPort == null)
			{
				string[] ports = this.get360PortsByHardwareId();
				if (ports == null || ports.Length == 0)
				{
					return;
				}
				this._serialPort = new SerialPort(ports[0]);
				this._serialPort.WriteTimeout = 1000;
				this._serialPort.BaudRate = 9600;
				try
				{
					this._serialPort.Open();
					MessageBoxWindow.CreateWindow("360 Platform Connection Successfull", "", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Success, null, MessageBoxWindow.MessageBoxSize.Large, false);
					return;
				}
				catch (Exception ex)
				{
					MessageBoxWindow.CreateWindow("360 Platform Connection Error", ex.Message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Cancel,
						MessageBoxWindow.ButtonType.TryAgain
					}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.MessageBoxAnswered), MessageBoxWindow.MessageBoxSize.Large, false);
					this._serialPort = null;
					return;
				}
			}
			if (!this._serialPort.IsOpen)
			{
				try
				{
					this._serialPort.Open();
					MessageBoxWindow.CreateWindow("360 Platform Connection Successfull", "", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Success, null, MessageBoxWindow.MessageBoxSize.Large, false);
				}
				catch (Exception ex2)
				{
					MessageBoxWindow.CreateWindow("360 Platform Connection Error", ex2.Message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Cancel,
						MessageBoxWindow.ButtonType.TryAgain
					}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.MessageBoxAnswered), MessageBoxWindow.MessageBoxSize.Large, false);
					this._serialPort = null;
				}
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x000073A4 File Offset: 0x000055A4
		private void MessageBoxAnswered(MessageBoxWindow.MessageBoxReturn @return)
		{
			switch (@return)
			{
			case MessageBoxWindow.MessageBoxReturn.Cancel:
				this.RunPlatformCanceled = true;
				return;
			case MessageBoxWindow.MessageBoxReturn.Continue:
				break;
			case MessageBoxWindow.MessageBoxReturn.TryAgain:
				this.Initialize();
				break;
			default:
				return;
			}
		}

		// Token: 0x0600010F RID: 271 RVA: 0x000073C8 File Offset: 0x000055C8
		private string[] Get360PortsByName()
		{
			return SerialPort.GetPortNames().Where(delegate(string port)
			{
				try
				{
					using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"))
					{
						ManagementObjectCollection ports = searcher.Get();
						foreach (ManagementBaseObject portObj in ports)
						{
							string portName = portObj["Name"].ToString();
							if (this.allowedSerialNames.Any((string name) => portName.ToLower().Contains(name.ToLower())) && portName.Contains(port))
							{
								return true;
							}
						}
					}
				}
				catch (Exception ex)
				{
				}
				return false;
			}).ToArray<string>();
		}

		// Token: 0x06000110 RID: 272 RVA: 0x000073F4 File Offset: 0x000055F4
		private string[] get360PortsByHardwareId()
		{
			return SerialPort.GetPortNames().Where(delegate(string port)
			{
				try
				{
					using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%(COM%'"))
					{
						ManagementObjectCollection devices = searcher.Get();
						foreach (ManagementBaseObject device in devices)
						{
							string portName = device["Name"].ToString();
							string[] hardwareIDs = (string[])device["HardwareID"];
							if (hardwareIDs != null && this.allowedHardwareIds.Any((string allowedId) => hardwareIDs.Any((string hardwareId) => hardwareId.Contains(allowedId))) && portName.Contains(port))
							{
								return true;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Port tarama hatası: " + ex.Message);
				}
				return false;
			}).ToArray<string>();
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000741E File Offset: 0x0000561E
		public static void StartMotion()
		{
			Motion360Platform.Instance.StartMotionInternal();
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000742A File Offset: 0x0000562A
		public static void StopMotion()
		{
			Motion360Platform.Instance.StopMotionInternal();
		}

		// Token: 0x06000113 RID: 275 RVA: 0x00007436 File Offset: 0x00005636
		public static void EmergencyStopMotion()
		{
			Motion360Platform.Instance.EmergencyStopMotionInternal();
		}

		// Token: 0x06000114 RID: 276 RVA: 0x00007442 File Offset: 0x00005642
		private void StartMotionInternal()
		{
			this.WriteLine("S");
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000744F File Offset: 0x0000564F
		public void StopMotionInternal()
		{
			this.WriteLine("X");
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000745C File Offset: 0x0000565C
		public void EmergencyStopMotionInternal()
		{
			this.WriteLine("E");
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00007469 File Offset: 0x00005669
		public void SetTime(int seconds)
		{
			seconds = Math.Max(Math.Min(seconds, 9), 3);
			this.WriteLine(string.Format("V{0}", seconds));
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00007491 File Offset: 0x00005691
		public void Activate()
		{
			this.RunPlatformCanceled = false;
			this.Initialize();
		}

		// Token: 0x06000119 RID: 281 RVA: 0x000074A0 File Offset: 0x000056A0
		private void WriteLine(string command)
		{
			bool success = false;
			if (!this.IsReady)
			{
				this.Initialize();
			}
			if (this.IsReady)
			{
				try
				{
					this._serialPort.WriteLine(command);
					success = true;
				}
				catch (Exception ex)
				{
				}
				if (!success)
				{
					try
					{
						if (this._serialPort.IsOpen)
						{
							this._serialPort.Close();
						}
						this._serialPort = null;
					}
					catch (Exception)
					{
					}
				}
			}
		}

		// Token: 0x040000D3 RID: 211
		private static Motion360Platform _instance;

		// Token: 0x040000D4 RID: 212
		private static readonly object _lock = new object();

		// Token: 0x040000D5 RID: 213
		private string[] allowedSerialNames = new string[]
		{
			"arduino",
			"chr"
		};

		// Token: 0x040000D6 RID: 214
		private string[] allowedHardwareIds = new string[]
		{
			"VID_2341",
			"VID_1A86"
		};

		// Token: 0x040000D7 RID: 215
		private SerialPort _serialPort;

		// Token: 0x040000D8 RID: 216
		private bool RunPlatformCanceled;
	}
}
