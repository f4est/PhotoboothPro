using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Xml;
using ImageMagick;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Modals.Api;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Variations;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Shell;
using Newtonsoft.Json;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using QRCoder;
using ShaderEffectLibrary;
using VideoProcessorLibrary.Models;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C5 RID: 197
	public static class ExtensionMethod
	{
		// Token: 0x06000A97 RID: 2711 RVA: 0x0003CE14 File Offset: 0x0003B014
		public static T XamlClone<T>(this T original) where T : class
		{
			if (original == null)
			{
				return default(T);
			}
			object clone;
			using (MemoryStream stream = new MemoryStream())
			{
				XamlWriter.Save(original, stream);
				stream.Seek(0L, SeekOrigin.Begin);
				clone = XamlReader.Load(stream);
			}
			if (clone is T)
			{
				return (T)((object)clone);
			}
			return default(T);
		}

		// Token: 0x06000A98 RID: 2712 RVA: 0x0003CE8C File Offset: 0x0003B08C
		public static bool IsTextAllowed(string text)
		{
			return !ExtensionMethod._regex.IsMatch(text);
		}

		// Token: 0x06000A99 RID: 2713 RVA: 0x0003CE9C File Offset: 0x0003B09C
		public static void OpenUrl(string url)
		{
			try
			{
				Process.Start(new ProcessStartInfo
				{
					FileName = url,
					UseShellExecute = true
				});
			}
			catch (Exception ex)
			{
				MessageBoxWindow.CreateWindow("URL Error", "Error Info : " + ex.Message, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x06000A9A RID: 2714 RVA: 0x0003CF04 File Offset: 0x0003B104
		public static string ChangeImageFormat(string sourcePath, ImageFormat imageFormat)
		{
			string newFileName = Path.Combine(Path.GetDirectoryName(sourcePath), Path.GetFileNameWithoutExtension(sourcePath) + ".jpg");
			using (System.Drawing.Image image = System.Drawing.Image.FromFile(sourcePath))
			{
				image.Save(newFileName, imageFormat);
			}
			return newFileName;
		}

		// Token: 0x06000A9B RID: 2715 RVA: 0x0003CF5C File Offset: 0x0003B15C
		public static void LoadResourcesFonts()
		{
			ExtensionMethod.ResourcesFonts = new List<System.Windows.Media.FontFamily>
			{
				(System.Windows.Media.FontFamily)Application.Current.Resources["Arial"],
				(System.Windows.Media.FontFamily)Application.Current.Resources["Bauhaus"],
				(System.Windows.Media.FontFamily)Application.Current.Resources["Broadway"],
				(System.Windows.Media.FontFamily)Application.Current.Resources["BrushScript"],
				(System.Windows.Media.FontFamily)Application.Current.Resources["CHILLER"],
				(System.Windows.Media.FontFamily)Application.Current.Resources["Copperplate"],
				(System.Windows.Media.FontFamily)Application.Current.Resources["PixelifySans"]
			};
		}

		// Token: 0x06000A9C RID: 2716 RVA: 0x0003D04C File Offset: 0x0003B24C
		public static System.Windows.Media.FontFamily FindFont(int Index)
		{
			if (ExtensionMethod.ResourcesFonts == null)
			{
				ExtensionMethod.LoadResourcesFonts();
			}
			return ExtensionMethod.ResourcesFonts[Index];
		}

		// Token: 0x06000A9D RID: 2717 RVA: 0x0003D068 File Offset: 0x0003B268
		public static System.Windows.Media.FontFamily FindFont(string name)
		{
			List<string> fontNames = ExtensionMethod.GetFontNames();
			int index = Math.Max(fontNames.FindIndex((string x) => x == name), 0);
			return ExtensionMethod.FindFont(index);
		}

		// Token: 0x06000A9E RID: 2718 RVA: 0x0003D0A8 File Offset: 0x0003B2A8
		public static void CreateEventXml<T>(T eventInfo, string path, string mainElement, string comment)
		{
			using (XmlTextWriter createXML = new XmlTextWriter(path, Encoding.UTF8))
			{
				createXML.WriteStartDocument();
				createXML.WriteComment(comment);
				createXML.WriteStartElement(mainElement);
				Type type = typeof(T);
				foreach (FieldInfo prop in type.GetFields())
				{
					createXML.WriteStartElement(prop.Name);
					object value = prop.GetValue(eventInfo);
					if (value != null)
					{
						if (value is DateTime)
						{
							createXML.WriteString(((DateTime)value).ToString("dd.MM.yyyy HH:mm:ss"));
						}
						else
						{
							createXML.WriteString(value.ToString());
						}
					}
					else
					{
						createXML.WriteString("");
					}
					createXML.WriteEndElement();
				}
				createXML.WriteEndElement();
				createXML.WriteEndDocument();
			}
		}

		// Token: 0x06000A9F RID: 2719 RVA: 0x0003D188 File Offset: 0x0003B388
		public static void WriteEventsDataToDatabase()
		{
			if (ExtensionMethod.EventSyncIsWorking)
			{
				return;
			}
			new Thread(delegate()
			{
				ExtensionMethod.<>c.<<WriteEventsDataToDatabase>b__14_0>d <<WriteEventsDataToDatabase>b__14_0>d;
				<<WriteEventsDataToDatabase>b__14_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<WriteEventsDataToDatabase>b__14_0>d.<>1__state = -1;
				<<WriteEventsDataToDatabase>b__14_0>d.<>t__builder.Start<ExtensionMethod.<>c.<<WriteEventsDataToDatabase>b__14_0>d>(ref <<WriteEventsDataToDatabase>b__14_0>d);
			}).Start();
		}

		// Token: 0x06000AA0 RID: 2720 RVA: 0x0003D1BC File Offset: 0x0003B3BC
		public static List<string> GetInstalledPrograms()
		{
			List<string> resultList = new List<string>();
			string registryKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKeyPath))
			{
				if (key != null)
				{
					foreach (string subkeyName in key.GetSubKeyNames())
					{
						using (RegistryKey subkey = key.OpenSubKey(subkeyName))
						{
							string programName = subkey.GetValue("DisplayName") as string;
							if (!string.IsNullOrEmpty(programName))
							{
								resultList.Add(programName);
							}
						}
					}
				}
			}
			using (RegistryKey key2 = Registry.LocalMachine.OpenSubKey(registryKeyPath.Replace("SOFTWARE", "SOFTWARE\\WOW6432Node")))
			{
				if (key2 != null)
				{
					foreach (string subkeyName2 in key2.GetSubKeyNames())
					{
						using (RegistryKey subkey2 = key2.OpenSubKey(subkeyName2))
						{
							string programName2 = subkey2.GetValue("DisplayName") as string;
							if (!string.IsNullOrEmpty(programName2))
							{
								resultList.Add(programName2);
							}
						}
					}
				}
			}
			return resultList;
		}

		// Token: 0x06000AA1 RID: 2721 RVA: 0x0003D310 File Offset: 0x0003B510
		public static List<string> GetAllStringValues(object obj)
		{
			List<string> stringValues = new List<string>();
			if (obj == null)
			{
				return stringValues;
			}
			Type type = obj.GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			foreach (PropertyInfo property in properties)
			{
				try
				{
					if (property.PropertyType == typeof(string))
					{
						string value = property.GetValue(obj) as string;
						if (value != null)
						{
							stringValues.Add(value.ToLower());
						}
					}
					else if (!property.PropertyType.IsPrimitive && property.PropertyType.IsClass)
					{
						object nestedObject = property.GetValue(obj);
						if (nestedObject != null)
						{
							stringValues.AddRange(ExtensionMethod.GetAllStringValues(nestedObject));
						}
					}
				}
				catch (Exception ex)
				{
					File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error_" + DateTime.Now.ToString(ExtensionMethod.dateTimeFormat) + ".txt"), "Error accessing property " + property.Name + ": " + ex.Message);
				}
			}
			return stringValues;
		}

		// Token: 0x06000AA2 RID: 2722 RVA: 0x0003D434 File Offset: 0x0003B634
		public static bool ContainsClass(object obj, string search)
		{
			List<string> liste = ExtensionMethod.GetAllStringValues(obj);
			return (from x in liste
			where x.Contains(search)
			select x).Any<string>();
		}

		// Token: 0x06000AA3 RID: 2723 RVA: 0x0003D46C File Offset: 0x0003B66C
		public static void ControlEOSService()
		{
			string serviceName = "EWCService";
			bool isServiceStopTry = false;
			using (ServiceController service = new ServiceController(serviceName))
			{
				try
				{
					if (service.Status == ServiceControllerStatus.Running)
					{
						isServiceStopTry = true;
						service.Stop();
						service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30.0));
						MessageBoxWindow.CreateWindow("Canon EOS Warning", "Canon EOS Stopped for use AIPhotoBoothPro", new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					}
				}
				catch (InvalidOperationException ex)
				{
					if (isServiceStopTry)
					{
						MessageBoxWindow.CreateWindow("Canon EOS Warning", "Please Uninstall Canon EOS Webcam Utility, Canon DSLR doesn't work if you run Canon EOS Webcam Utility", new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Error_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff") + ".txt"), "Error: " + ex.Message);
					}
				}
			}
		}

		// Token: 0x06000AA4 RID: 2724 RVA: 0x0003D560 File Offset: 0x0003B760
		public static bool IsProgramInstalled(string programName)
		{
			List<string> installedPrograms = (from x in ExtensionMethod.GetInstalledPrograms()
			orderby x
			select x).ToList<string>();
			foreach (string installedProgram in installedPrograms)
			{
				if (installedProgram.Contains(programName))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000AA5 RID: 2725 RVA: 0x0003D5E8 File Offset: 0x0003B7E8
		private static void SyncMailPrintPostCompleted(bool success, string content)
		{
			if (success)
			{
				MailPrintDataRequest dataClass = JsonConvert.DeserializeObject<MailPrintDataRequest>(content);
				List<MailData> mailData = ExtensionMethod.ReadJson<List<MailData>>(EventManagementPage.GetCurrentEvent().MailJsonPath);
				List<PrintData> printData = ExtensionMethod.ReadJson<List<PrintData>>(EventManagementPage.GetCurrentEvent().PrintJsonPath);
				if (File.Exists(EventManagementPage.GetCurrentEvent().MailJsonPath) && dataClass.mailData != null)
				{
					for (int i = 0; i < dataClass.mailData.Count; i++)
					{
						for (int j = 0; j < mailData.Count; j++)
						{
							if (dataClass.mailData[i].Id == mailData[j].Id)
							{
								mailData[j].IsCloudSync = true;
								break;
							}
						}
					}
					ExtensionMethod.CreateWriteJson<List<MailData>>(mailData, EventManagementPage.GetCurrentEvent().MailJsonPath);
				}
				if (File.Exists(EventManagementPage.GetCurrentEvent().PrintJsonPath) && dataClass.printData != null)
				{
					for (int k = 0; k < dataClass.printData.Count; k++)
					{
						for (int l = 0; l < printData.Count; l++)
						{
							if (dataClass.printData[k].Id == printData[l].Id)
							{
								printData[l].IsCloudSync = true;
								break;
							}
						}
					}
					ExtensionMethod.CreateWriteJson<List<PrintData>>(printData, EventManagementPage.GetCurrentEvent().PrintJsonPath);
				}
			}
		}

		// Token: 0x06000AA6 RID: 2726 RVA: 0x0003D73C File Offset: 0x0003B93C
		public static void AddFolderToZip(ZipArchive archive, string folderPath, string folderNameInZip)
		{
			DirectoryInfo folder = new DirectoryInfo(folderPath);
			foreach (FileInfo file in folder.GetFiles())
			{
				string entryName = Path.Combine(folderNameInZip, file.Name);
				ZipArchiveEntry entry = archive.CreateEntry(entryName, CompressionLevel.Fastest);
				using (Stream entryStream = entry.Open())
				{
					using (FileStream fileStream = File.OpenRead(file.FullName))
					{
						fileStream.CopyTo(entryStream);
					}
				}
			}
			foreach (DirectoryInfo subfolder in folder.GetDirectories())
			{
				string subfolderNameInZip = Path.Combine(folderNameInZip, subfolder.Name);
				ExtensionMethod.AddFolderToZip(archive, subfolder.FullName, subfolderNameInZip);
			}
		}

		// Token: 0x06000AA7 RID: 2727 RVA: 0x0003D818 File Offset: 0x0003BA18
		private static void SyncEventsPostCompleted(bool success, string content)
		{
			if (success)
			{
				EventDataResponse eventClass = JsonConvert.DeserializeObject<EventDataResponse>(content);
				if (eventClass != null && File.Exists(SessionData.UserEventsJsonPath))
				{
					List<EventApplication> events = ExtensionMethod.ReadJson<List<EventApplication>>(SessionData.UserEventsJsonPath);
					for (int i = 0; i < eventClass.Events.Count; i++)
					{
						for (int j = 0; j < events.Count; j++)
						{
							if (eventClass.Events[i].Id == events[j].Id)
							{
								events[j].IsCloudSync = true;
								break;
							}
						}
					}
					ExtensionMethod.CreateWriteJson<List<EventApplication>>(events, SessionData.UserEventsJsonPath);
				}
			}
			if ((from x in ExtensionMethod.ReadJson<List<EventApplication>>(SessionData.UserEventsJsonPath)
			where !x.IsCloudSync
			select x).ToList<EventApplication>().Count > 0)
			{
				ExtensionMethod.WriteEventsDataToDatabase();
			}
		}

		// Token: 0x06000AA8 RID: 2728 RVA: 0x0003D8F4 File Offset: 0x0003BAF4
		public static T FindChildByName<T>(DependencyObject parent, string childName) where T : DependencyObject
		{
			if (parent == null)
			{
				return default(T);
			}
			T foundChild = default(T);
			int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
			for (int i = 0; i < childrenCount; i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(parent, i);
				T typedChild = child as T;
				if (typedChild != null && child.GetValue(FrameworkElement.NameProperty) as string == childName)
				{
					foundChild = typedChild;
					break;
				}
				foundChild = ExtensionMethod.FindChildByName<T>(child, childName);
				if (foundChild != null)
				{
					break;
				}
			}
			return foundChild;
		}

		// Token: 0x06000AA9 RID: 2729 RVA: 0x0003D979 File Offset: 0x0003BB79
		public static bool TryConvertStringToEnum(string type, out MainSettingsPage.CameraType result)
		{
			return Enum.TryParse<MainSettingsPage.CameraType>(type, out result);
		}

		// Token: 0x06000AAA RID: 2730 RVA: 0x0003D984 File Offset: 0x0003BB84
		public static void CopyDirectory(string sourceDirPath, string destDirPath)
		{
			if (!Directory.Exists(sourceDirPath))
			{
				throw new DirectoryNotFoundException("Kaynak klasör bulunamadı: " + sourceDirPath);
			}
			if (!Directory.Exists(destDirPath))
			{
				Directory.CreateDirectory(destDirPath);
			}
			foreach (string file in Directory.GetFiles(sourceDirPath))
			{
				string destFile = Path.Combine(destDirPath, Path.GetFileName(file));
				File.Copy(file, destFile, true);
			}
			foreach (string subDirPath in Directory.GetDirectories(sourceDirPath))
			{
				string destSubDirPath = Path.Combine(destDirPath, Path.GetFileName(subDirPath));
				ExtensionMethod.CopyDirectory(subDirPath, destSubDirPath);
			}
		}

		// Token: 0x06000AAB RID: 2731 RVA: 0x0003DA24 File Offset: 0x0003BC24
		public static BitmapImage GenerateQr(string code)
		{
			QRCodeGenerator qrGenerator = new QRCodeGenerator();
			QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q, false, false, QRCodeGenerator.EciMode.Default, -1);
			QRCode qrCode = new QRCode(qrCodeData);
			string resourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "Photologo.png");
			BitmapImage result;
			using (Bitmap qrCodeLogo = (Bitmap)System.Drawing.Image.FromFile(resourcePath))
			{
				using (Bitmap qrCodeImage = qrCode.GetGraphic(20, System.Drawing.Color.Black, System.Drawing.Color.White, qrCodeLogo, 15, 0, true, null))
				{
					result = ExtensionMethod.ConvertBitmap2BitmapSource((Bitmap)qrCodeImage.Clone());
				}
			}
			return result;
		}

		// Token: 0x06000AAC RID: 2732 RVA: 0x0003DAE8 File Offset: 0x0003BCE8
		public static string ReChanged(TextChangedEventArgs e, TextBox txtBox)
		{
			string retunText = txtBox.Text;
			return retunText.Replace(retunText.Substring(e.Changes.First<TextChange>().Offset, e.Changes.First<TextChange>().AddedLength), "");
		}

		// Token: 0x06000AAD RID: 2733 RVA: 0x0003DB30 File Offset: 0x0003BD30
		public static string RemoveZeroString(string str, out bool isChange)
		{
			isChange = false;
			if (str.Length > 1)
			{
				if (str.IndexOf("0") == 0)
				{
					bool other = false;
					str = ExtensionMethod.RemoveZeroString(str.Substring(1, str.Length - 1), out other);
					isChange = true;
				}
				return str;
			}
			return str;
		}

		// Token: 0x06000AAE RID: 2734 RVA: 0x0003DB7C File Offset: 0x0003BD7C
		public static void WriteCameraInfo(ConnectedGoPro addGoProData)
		{
			List<ConnectedGoPro> readed = new List<ConnectedGoPro>();
			if (File.Exists(GoProCameraControlClass.connectedGoProStringsFilePath))
			{
				readed = ExtensionMethod.ReadJson<List<ConnectedGoPro>>(GoProCameraControlClass.connectedGoProStringsFilePath);
			}
			if (!readed.Contains(addGoProData))
			{
				readed.Add(addGoProData);
			}
			ExtensionMethod.CreateWriteJson<List<ConnectedGoPro>>(readed, GoProCameraControlClass.connectedGoProStringsFilePath);
		}

		// Token: 0x06000AAF RID: 2735 RVA: 0x0003DBC4 File Offset: 0x0003BDC4
		public static Bitmap CropAndResizeImage(Bitmap originalImage, double topPercentage, double rightPercentage, double bottomPercentage, double leftPercentage)
		{
			int originalWidth = originalImage.Width;
			int originalHeight = originalImage.Height;
			int cropLeft = (int)((double)originalWidth * leftPercentage / 100.0);
			int cropTop = (int)((double)originalHeight * topPercentage / 100.0);
			int cropRight = (int)((double)originalWidth * rightPercentage / 100.0);
			int cropBottom = (int)((double)originalHeight * bottomPercentage / 100.0);
			int newWidth = originalWidth - cropLeft - cropRight;
			int newHeight = originalHeight - cropTop - cropBottom;
			Bitmap result;
			using (Bitmap croppedImage = new Bitmap(newWidth, newHeight))
			{
				using (Graphics graphics = Graphics.FromImage(croppedImage))
				{
					graphics.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(cropLeft, cropTop, originalWidth - cropLeft - cropRight, originalHeight - cropTop - cropBottom), GraphicsUnit.Pixel);
				}
				result = (Bitmap)croppedImage.Clone();
			}
			return result;
		}

		// Token: 0x06000AB0 RID: 2736 RVA: 0x0003DCB8 File Offset: 0x0003BEB8
		private static List<ExtensionMethod.SystemPrinter> GetPrinters()
		{
			List<ExtensionMethod.SystemPrinter> systemPrinters = new List<ExtensionMethod.SystemPrinter>();
			ConnectionOptions options = new ConnectionOptions
			{
				Impersonation = ImpersonationLevel.Impersonate,
				EnablePrivileges = true
			};
			ManagementScope scope = new ManagementScope("\\\\.\\root\\cimv2", options);
			scope.Connect();
			ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_Printer"));
			foreach (ManagementBaseObject managementBaseObject in searcher.Get())
			{
				ManagementObject printer = (ManagementObject)managementBaseObject;
				systemPrinters.Add(new ExtensionMethod.SystemPrinter
				{
					Name = printer["Name"].ToString(),
					Active = (printer["PrinterStatus"].ToString() == "3" && printer["DetectedErrorState"].ToString() == "0")
				});
			}
			return systemPrinters;
		}

		// Token: 0x06000AB1 RID: 2737
		[DllImport("wininet.dll")]
		private static extern bool InternetGetConnectedState(out int description, int reservedValue);

		// Token: 0x06000AB2 RID: 2738 RVA: 0x0003DDAC File Offset: 0x0003BFAC
		public static bool IsInternetAvailable()
		{
			bool result = false;
			for (int i = 0; i < 4; i++)
			{
				try
				{
					using (Ping ping = new Ping())
					{
						PingReply reply = ping.Send("8.8.8.8");
						result = (reply != null && reply.Status == IPStatus.Success);
						if (result)
						{
							ActivationKingWindow.Instance.CheckInternetGrid(result);
							return result;
						}
						reply = ping.Send("1.1.1.1");
						result = (reply != null && reply.Status == IPStatus.Success);
					}
				}
				catch (Exception ex)
				{
				}
				if (!result)
				{
					try
					{
						using (WebClient client = new WebClient())
						{
							using (client.OpenRead("http://www.google.com"))
							{
								ActivationKingWindow.Instance.CheckInternetGrid(true);
								return true;
							}
						}
					}
					catch
					{
					}
					try
					{
						using (TcpClient client2 = new TcpClient())
						{
							client2.Connect("8.8.8.8", 53);
							ActivationKingWindow.Instance.CheckInternetGrid(true);
							return true;
						}
					}
					catch
					{
					}
				}
			}
			ActivationKingWindow.Instance.CheckInternetGrid(result);
			return result;
		}

		// Token: 0x06000AB3 RID: 2739 RVA: 0x0003DF18 File Offset: 0x0003C118
		public static Task UpdatePrinters()
		{
			ExtensionMethod.<UpdatePrinters>d__35 <UpdatePrinters>d__;
			<UpdatePrinters>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdatePrinters>d__.<>1__state = -1;
			<UpdatePrinters>d__.<>t__builder.Start<ExtensionMethod.<UpdatePrinters>d__35>(ref <UpdatePrinters>d__);
			return <UpdatePrinters>d__.<>t__builder.Task;
		}

		// Token: 0x06000AB4 RID: 2740 RVA: 0x0003DF54 File Offset: 0x0003C154
		public static System.Drawing.Image Render(string capturedPath)
		{
			System.Drawing.Image capturedImage = System.Drawing.Image.FromFile(capturedPath);
			System.Drawing.Image backgroundForSize = System.Drawing.Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "transparent.png"));
			string rotation = Settings.GetValueString("rotation") ?? "0";
			if (DSLR.isFileImport)
			{
				rotation = "0";
			}
			if (rotation.Contains("90"))
			{
				capturedImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
				capturedImage.Save(capturedPath);
			}
			else if (rotation.Contains("180"))
			{
				capturedImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
				capturedImage.Save(capturedPath);
			}
			else if (rotation.Contains("270"))
			{
				capturedImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
				capturedImage.Save(capturedPath);
			}
			System.Drawing.Image resultImage = new Bitmap(backgroundForSize.Width, backgroundForSize.Height);
			float scale = Math.Min((float)capturedImage.Width / (float)TemplateClass.PaperWidthPhoto, (float)capturedImage.Height / (float)TemplateClass.PaperHeightPhoto);
			int scaleWidth = (int)((float)capturedImage.Width / scale);
			int scaleHeight = (int)((float)capturedImage.Height / scale);
			Mat backgroundMat = null;
			try
			{
				byte[] cv2TempData = File.ReadAllBytes(CameraControlClass.BacgroundForTranparent);
				backgroundMat = Cv2.ImDecode(cv2TempData, ImreadModes.AnyColor);
			}
			catch (Exception ex)
			{
				MessageBoxWindow.CreateWindow("Render Error", ex.ToString(), new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			if (Settings.GetValueBoolean("greenbox").GetValueOrDefault())
			{
				capturedImage = ExtensionMethod.ColorKeyGreenEffect((Bitmap)capturedImage, backgroundMat, byte.Parse(Settings.GetValueString("greenboxthrash")));
			}
			using (System.Drawing.Image greenImage = capturedImage)
			{
				using (Graphics graph = Graphics.FromImage(resultImage))
				{
					graph.InterpolationMode = InterpolationMode.High;
					graph.CompositingQuality = CompositingQuality.HighQuality;
					graph.SmoothingMode = SmoothingMode.AntiAlias;
					using (System.Drawing.Image backgroundImage = backgroundForSize)
					{
						graph.DrawImage(backgroundImage, 0, 0, backgroundImage.Width, backgroundImage.Height);
					}
					graph.DrawImage(greenImage, new Rectangle(0, 0, scaleWidth, scaleHeight));
				}
			}
			return resultImage;
		}

		// Token: 0x06000AB5 RID: 2741 RVA: 0x0003E170 File Offset: 0x0003C370
		public static BitmapSource GetVideoThumbnail(string videoPath)
		{
			BitmapSource result;
			using (ShellFile shellFile = ShellFile.FromFilePath(videoPath))
			{
				ShellThumbnail thumbnail = shellFile.Thumbnail;
				thumbnail.FormatOption = ShellThumbnailFormatOption.ThumbnailOnly;
				BitmapSource bSource = Imaging.CreateBitmapSourceFromHBitmap(thumbnail.Bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
				result = bSource;
			}
			return result;
		}

		// Token: 0x06000AB6 RID: 2742 RVA: 0x0003E1D4 File Offset: 0x0003C3D4
		public static void AddFormattedTextToTextBlock(TextBlock textBlock, List<string> Texts, List<ExtensionMethod.FontType> fontTypes)
		{
			textBlock.Inlines.Clear();
			for (int i = 0; i < Texts.Count; i++)
			{
				Run fontText = new Run(Texts[i])
				{
					FontWeight = ((fontTypes[i] == ExtensionMethod.FontType.Bold) ? FontWeights.Bold : FontWeights.Normal)
				};
				textBlock.Inlines.Add(fontText);
			}
		}

		// Token: 0x06000AB7 RID: 2743 RVA: 0x0003E234 File Offset: 0x0003C434
		public static Task<string> RenderGifVideo(List<string> capturedPaths, int framerate = 2, int repeatCount = 3)
		{
			ExtensionMethod.<RenderGifVideo>d__41 <RenderGifVideo>d__;
			<RenderGifVideo>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<RenderGifVideo>d__.capturedPaths = capturedPaths;
			<RenderGifVideo>d__.framerate = framerate;
			<RenderGifVideo>d__.repeatCount = repeatCount;
			<RenderGifVideo>d__.<>1__state = -1;
			<RenderGifVideo>d__.<>t__builder.Start<ExtensionMethod.<RenderGifVideo>d__41>(ref <RenderGifVideo>d__);
			return <RenderGifVideo>d__.<>t__builder.Task;
		}

		// Token: 0x06000AB8 RID: 2744 RVA: 0x0003E288 File Offset: 0x0003C488
		private static void DeleteExistingGifPhotos()
		{
			string[] existingFiles = Directory.GetFiles(CameraControlClass.FolderForGif);
			foreach (string file in existingFiles)
			{
				File.Delete(file);
			}
		}

		// Token: 0x06000AB9 RID: 2745 RVA: 0x0003E2BC File Offset: 0x0003C4BC
		private static Task DeleteExistingGifPhotosAfterSeconds(int milliseconds)
		{
			ExtensionMethod.<DeleteExistingGifPhotosAfterSeconds>d__43 <DeleteExistingGifPhotosAfterSeconds>d__;
			<DeleteExistingGifPhotosAfterSeconds>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteExistingGifPhotosAfterSeconds>d__.milliseconds = milliseconds;
			<DeleteExistingGifPhotosAfterSeconds>d__.<>1__state = -1;
			<DeleteExistingGifPhotosAfterSeconds>d__.<>t__builder.Start<ExtensionMethod.<DeleteExistingGifPhotosAfterSeconds>d__43>(ref <DeleteExistingGifPhotosAfterSeconds>d__);
			return <DeleteExistingGifPhotosAfterSeconds>d__.<>t__builder.Task;
		}

		// Token: 0x06000ABA RID: 2746 RVA: 0x0003E300 File Offset: 0x0003C500
		private static Font LoadEmbeddedFont(float size)
		{
			Assembly assembly = Assembly.GetExecutingAssembly();
			string resourceName = "KingAIPhotoBoothPro.Fonts.PixelifySans-VariableFont_wght.ttf";
			PrivateFontCollection fontCollection = new PrivateFontCollection();
			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					return null;
				}
				byte[] fontData = new byte[stream.Length];
				stream.Read(fontData, 0, (int)stream.Length);
				IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
				Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
				fontCollection.AddMemoryFont(fontPtr, fontData.Length);
				Marshal.FreeCoTaskMem(fontPtr);
			}
			return new Font(fontCollection.Families[0], size);
		}

		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06000ABB RID: 2747 RVA: 0x0003E3A8 File Offset: 0x0003C5A8
		public static ImageCodecInfo ResultImageCodecInfo
		{
			get
			{
				return ExtensionMethod.GetEncoder(ImageFormat.Jpeg);
			}
		}

		// Token: 0x06000ABC RID: 2748 RVA: 0x0003E3B4 File Offset: 0x0003C5B4
		public static ImageCodecInfo GetEncoder(ImageFormat format)
		{
			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
			foreach (ImageCodecInfo codec in codecs)
			{
				if (codec.FormatID == format.Guid)
				{
					return codec;
				}
			}
			return null;
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06000ABD RID: 2749 RVA: 0x0003E3F4 File Offset: 0x0003C5F4
		public static EncoderParameters ResultImageEncoderParameters
		{
			get
			{
				System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
				EncoderParameters myEncoderParameters = new EncoderParameters(1);
				EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)AppInfo.AppClass.AppDefaultSettings.ResultImageQualityValue);
				myEncoderParameters.Param[0] = myEncoderParameter;
				return myEncoderParameters;
			}
		}

		// Token: 0x06000ABE RID: 2750 RVA: 0x0003E430 File Offset: 0x0003C630
		public static List<string> ReadWordCloud()
		{
			string wordCloudString = Settings.GetValueString("wordcloud") ?? WordCloudPage.defaultString;
			string[] words = wordCloudString.ToUpper().Split(new char[]
			{
				','
			});
			return words.ToList<string>();
		}

		// Token: 0x06000ABF RID: 2751 RVA: 0x0003E46E File Offset: 0x0003C66E
		public static string RemoveAlphaChannelFromString(string color)
		{
			if (string.IsNullOrEmpty(color) || !color.StartsWith("#"))
			{
				return "#000000";
			}
			if (color.Length == 9)
			{
				return "#" + color.Substring(3);
			}
			return color;
		}

		// Token: 0x06000AC0 RID: 2752 RVA: 0x0003E4A8 File Offset: 0x0003C6A8
		public static void ApplyJakartaFilter(string inputPath, string outputPath)
		{
			if (!File.Exists(inputPath))
			{
				throw new FileNotFoundException("Girdi dosyası bulunamadı.", inputPath);
			}
			BitmapImage original = new BitmapImage();
			using (FileStream stream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
			{
				original.BeginInit();
				original.CacheOption = BitmapCacheOption.OnLoad;
				original.StreamSource = stream;
				original.EndInit();
				original.Freeze();
			}
			BitmapSource sharpenyellowed = ExtensionMethod.ApplyJakartaFilter(original);
			BitmapEncoder encoder = new PngBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(sharpenyellowed));
			using (FileStream fileStream = new FileStream(outputPath, FileMode.Create))
			{
				encoder.Save(fileStream);
			}
		}

		// Token: 0x06000AC1 RID: 2753 RVA: 0x0003E560 File Offset: 0x0003C760
		public static BitmapSource ApplyJakartaFilter(BitmapSource original)
		{
			int width = original.PixelWidth;
			int height = original.PixelHeight;
			int stride = width * 4;
			byte[] pixels = new byte[height * stride];
			original.CopyPixels(pixels, stride, 0);
			float warmth = 0f;
			float saturation = 1.09f;
			float contrast = 1.09f;
			float brightness = 1.08f;
			float yellowOverlay = 0.05f;
			byte overlayR = byte.MaxValue;
			byte overlayG = 245;
			byte overlayB = 203;
			for (int i = 0; i < pixels.Length; i += 4)
			{
				float b = (float)pixels[i];
				float g = (float)pixels[i + 1];
				float r = (float)pixels[i + 2];
				r += warmth * (255f - r);
				g += warmth * (255f - g);
				b -= warmth * b;
				r = r * (1f - yellowOverlay) + (float)overlayR * yellowOverlay;
				g = g * (1f - yellowOverlay) + (float)overlayG * yellowOverlay;
				b = b * (1f - yellowOverlay) + (float)overlayB * yellowOverlay;
				float avg = (r + g + b) / 3f;
				r = avg + (r - avg) * saturation;
				g = avg + (g - avg) * saturation;
				b = avg + (b - avg) * saturation;
				r = (r - 128f) * contrast + 128f;
				g = (g - 128f) * contrast + 128f;
				b = (b - 128f) * contrast + 128f;
				r *= brightness;
				g *= brightness;
				b *= brightness;
				pixels[i] = ExtensionMethod.Clamp((int)b);
				pixels[i + 1] = ExtensionMethod.Clamp((int)g);
				pixels[i + 2] = ExtensionMethod.Clamp((int)r);
			}
			return BitmapSource.Create(width, height, 96.0, 96.0, PixelFormats.Bgra32, null, pixels, stride);
		}

		// Token: 0x06000AC2 RID: 2754 RVA: 0x0003E72E File Offset: 0x0003C92E
		private static byte Clamp(int val)
		{
			return (byte)((val < 0) ? 0 : ((val > 255) ? 255 : val));
		}

		// Token: 0x06000AC3 RID: 2755 RVA: 0x0003E748 File Offset: 0x0003C948
		public static Task RenderAsync(List<string> capturedPaths, string savePath, bool isHaveGreenEffect, bool isAIBackground, bool isPhotoMode, bool isForTemplate, bool isRotation = true)
		{
			ExtensionMethod.<RenderAsync>d__55 <RenderAsync>d__;
			<RenderAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<RenderAsync>d__.capturedPaths = capturedPaths;
			<RenderAsync>d__.savePath = savePath;
			<RenderAsync>d__.isHaveGreenEffect = isHaveGreenEffect;
			<RenderAsync>d__.isAIBackground = isAIBackground;
			<RenderAsync>d__.isPhotoMode = isPhotoMode;
			<RenderAsync>d__.isForTemplate = isForTemplate;
			<RenderAsync>d__.isRotation = isRotation;
			<RenderAsync>d__.<>1__state = -1;
			<RenderAsync>d__.<>t__builder.Start<ExtensionMethod.<RenderAsync>d__55>(ref <RenderAsync>d__);
			return <RenderAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000AC4 RID: 2756 RVA: 0x0003E7C0 File Offset: 0x0003C9C0
		public static List<string> GetFontNames()
		{
			if (ExtensionMethod.appFonts == null)
			{
				ExtensionMethod.appFonts = ExtensionMethod.LoadAllEmbeddedFonts();
			}
			return (from x in ExtensionMethod.appFonts.Families
			select x.Name).ToList<string>();
		}

		// Token: 0x06000AC5 RID: 2757 RVA: 0x0003E814 File Offset: 0x0003CA14
		public static EmbeddedFont GetSelectedFont()
		{
			if (ExtensionMethod.appFonts == null)
			{
				ExtensionMethod.appFonts = ExtensionMethod.LoadAllEmbeddedFonts();
			}
			string selectedFont = Settings.GetValueString("selectedfontname");
			if (string.IsNullOrEmpty(selectedFont))
			{
				return new EmbeddedFont
				{
					isFontAvailable = false
				};
			}
			System.Drawing.FontFamily fontFamily = (from x in ExtensionMethod.appFonts.Families
			where x.Name == selectedFont
			select x).FirstOrDefault<System.Drawing.FontFamily>();
			if (fontFamily != null)
			{
				return new EmbeddedFont
				{
					font = new Font(fontFamily, (float)Settings.GetValueInt("selectedfontsize").Value),
					fontSize = Settings.GetValueInt("selectedfontsize").Value,
					isFontAvailable = true
				};
			}
			return new EmbeddedFont
			{
				isFontAvailable = false
			};
		}

		// Token: 0x06000AC6 RID: 2758 RVA: 0x0003E8D8 File Offset: 0x0003CAD8
		public static PrivateFontCollection LoadAllEmbeddedFonts()
		{
			string fontFolderNamespace = "KingAIPhotoBoothPro.Fonts.Embedded";
			PrivateFontCollection fontCollection = new PrivateFontCollection();
			Assembly assembly = Assembly.GetExecutingAssembly();
			foreach (string resourceName in assembly.GetManifestResourceNames())
			{
				if (resourceName.StartsWith(fontFolderNamespace) && resourceName.ToLower().EndsWith(".ttf"))
				{
					using (Stream stream = assembly.GetManifestResourceStream(resourceName))
					{
						if (stream != null)
						{
							byte[] fontData = new byte[stream.Length];
							stream.Read(fontData, 0, fontData.Length);
							IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
							Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
							fontCollection.AddMemoryFont(fontPtr, fontData.Length);
							Marshal.FreeCoTaskMem(fontPtr);
						}
					}
				}
			}
			return fontCollection;
		}

		// Token: 0x06000AC7 RID: 2759 RVA: 0x0003E9B4 File Offset: 0x0003CBB4
		public static Task<string> ColorKeyChromaEffect(string sourcePath, string savePath)
		{
			ExtensionMethod.<ColorKeyChromaEffect>d__60 <ColorKeyChromaEffect>d__;
			<ColorKeyChromaEffect>d__.<>t__builder = AsyncTaskMethodBuilder<string>.Create();
			<ColorKeyChromaEffect>d__.sourcePath = sourcePath;
			<ColorKeyChromaEffect>d__.savePath = savePath;
			<ColorKeyChromaEffect>d__.<>1__state = -1;
			<ColorKeyChromaEffect>d__.<>t__builder.Start<ExtensionMethod.<ColorKeyChromaEffect>d__60>(ref <ColorKeyChromaEffect>d__);
			return <ColorKeyChromaEffect>d__.<>t__builder.Task;
		}

		// Token: 0x06000AC8 RID: 2760 RVA: 0x0003EA00 File Offset: 0x0003CC00
		public static void AIFaceSwapRender(byte[] dataImage, string savePath)
		{
			using (MemoryStream ms = new MemoryStream(dataImage))
			{
				using (System.Drawing.Image swapped = System.Drawing.Image.FromStream(ms))
				{
					using (System.Drawing.Image PasteArea = new Bitmap(swapped.Width, swapped.Height))
					{
						using (Graphics graph = Graphics.FromImage(PasteArea))
						{
							using (System.Drawing.Image orginal = System.Drawing.Image.FromFile(ActivationKingWindow.FaceSwapBackgroundGalleryPage.GetOrginalPathFromCDNURL()))
							{
								float xGain = (float)(swapped.Width / orginal.Width);
								graph.DrawImage(swapped, new Rectangle(0, 0, swapped.Width, swapped.Height));
								if (DSLR.NameAndSurname.Length > 0)
								{
									StringFormat format = new StringFormat();
									format.Alignment = StringAlignment.Near;
									format.LineAlignment = StringAlignment.Near;
									graph.DrawString(DSLR.NameAndSurname.Replace(" ", "\n").ToUpper(), ExtensionMethod.LoadEmbeddedFont(50f), new SolidBrush(System.Drawing.Color.White), new System.Drawing.Point(280, 1360), format);
								}
								PasteArea.Save(savePath);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000AC9 RID: 2761 RVA: 0x0003EB5C File Offset: 0x0003CD5C
		public static void AITaskRender(List<string> capturedPaths, int i, string AIBackgroundFolderPath, string saveAIBImagePath)
		{
			Task.Run(delegate()
			{
				Task<BunnyCDNHelper.CDNTaskResult> cdnTaskResult = BunnyCDNHelper.CDNUploadFileAsync(capturedPaths[i], SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, null, "");
				cdnTaskResult.Wait();
				if (!cdnTaskResult.Result.Success)
				{
					MessageBoxWindow.CreateWindow("Upload error", "error code: " + cdnTaskResult.Result.UploadResult.HttpCode.ToString() + " :" + cdnTaskResult.Result.UploadResult.Message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					throw new Exception("File couldn't be uploaded.");
				}
				string capturedUrl = BunnyCDNHelper.GetBunnyLink(capturedPaths[i], SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
				if (!Directory.Exists(AIBackgroundFolderPath))
				{
					Directory.CreateDirectory(AIBackgroundFolderPath);
				}
				saveAIBImagePath = Path.Combine(AIBackgroundFolderPath, Path.GetFileName(capturedPaths[i]));
				AIBackgroundModals.AIBackgroundRequest sendAIBackgroundRequest = new AIBackgroundModals.AIBackgroundRequest
				{
					Url = capturedUrl,
					AccessToken = SessionData.accountInfo.accessToken
				};
				Task<HTTPHelper.PostResult> postResult = HTTPHelper.PostRequestAsync<AIBackgroundModals.AIBackgroundRequest>("imageprocessing/backgroundremover", sendAIBackgroundRequest, null, "");
				postResult.Wait();
				AIBackgroundModals.AIBackgroundResponse response = JsonConvert.DeserializeObject<AIBackgroundModals.AIBackgroundResponse>(postResult.Result.Message);
				if (!response.success)
				{
					MessageBoxWindow.CreateWindow("AIBackgroundError", response.message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
					throw new Exception();
				}
				using (WebClient client = new WebClient())
				{
					byte[] data = client.DownloadData(response.url);
					File.WriteAllBytes(saveAIBImagePath, data);
				}
			}).Wait();
		}

		// Token: 0x06000ACA RID: 2762 RVA: 0x0003EBA4 File Offset: 0x0003CDA4
		public static Task<RenderResult> RenderVideo(string inputVideoPath, string outputFileName, string folderPath, int targetFps, TemplateType templateType, bool rotationActive = false)
		{
			ExtensionMethod.<RenderVideo>d__63 <RenderVideo>d__;
			<RenderVideo>d__.<>t__builder = AsyncTaskMethodBuilder<RenderResult>.Create();
			<RenderVideo>d__.inputVideoPath = inputVideoPath;
			<RenderVideo>d__.outputFileName = outputFileName;
			<RenderVideo>d__.folderPath = folderPath;
			<RenderVideo>d__.targetFps = targetFps;
			<RenderVideo>d__.templateType = templateType;
			<RenderVideo>d__.rotationActive = rotationActive;
			<RenderVideo>d__.<>1__state = -1;
			<RenderVideo>d__.<>t__builder.Start<ExtensionMethod.<RenderVideo>d__63>(ref <RenderVideo>d__);
			return <RenderVideo>d__.<>t__builder.Task;
		}

		// Token: 0x06000ACB RID: 2763 RVA: 0x0003EC14 File Offset: 0x0003CE14
		public static Task<FFmpegResult> GetThumbnailOfVideo(string inputVideoPath, string folderPath)
		{
			ExtensionMethod.<GetThumbnailOfVideo>d__64 <GetThumbnailOfVideo>d__;
			<GetThumbnailOfVideo>d__.<>t__builder = AsyncTaskMethodBuilder<FFmpegResult>.Create();
			<GetThumbnailOfVideo>d__.inputVideoPath = inputVideoPath;
			<GetThumbnailOfVideo>d__.folderPath = folderPath;
			<GetThumbnailOfVideo>d__.<>1__state = -1;
			<GetThumbnailOfVideo>d__.<>t__builder.Start<ExtensionMethod.<GetThumbnailOfVideo>d__64>(ref <GetThumbnailOfVideo>d__);
			return <GetThumbnailOfVideo>d__.<>t__builder.Task;
		}

		// Token: 0x06000ACC RID: 2764 RVA: 0x0003EC60 File Offset: 0x0003CE60
		public static bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return false;
			}
			try
			{
				email = Regex.Replace(email, "(@)(.+)$", new MatchEvaluator(ExtensionMethod.<IsValidEmail>g__DomainMapper|65_0), RegexOptions.None, TimeSpan.FromMilliseconds(200.0));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
			catch (ArgumentException)
			{
				return false;
			}
			bool result;
			try
			{
				result = Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.0));
			}
			catch (RegexMatchTimeoutException)
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000ACD RID: 2765 RVA: 0x0003ECF8 File Offset: 0x0003CEF8
		private static SolidBrush GetDrawingSolidBrush(string key)
		{
			System.Windows.Media.Color mediaColor = Settings.GetValueColor(key);
			System.Drawing.Color drawingcolor = System.Drawing.Color.FromArgb((int)mediaColor.A, (int)mediaColor.R, (int)mediaColor.G, (int)mediaColor.B);
			return new SolidBrush(drawingcolor);
		}

		// Token: 0x06000ACE RID: 2766 RVA: 0x0003ED38 File Offset: 0x0003CF38
		public static FileInformation AIRender(List<string> capturedPaths, string savePath, bool isHaveGreenEffect, bool isHaveAIBackground, bool isPhoto)
		{
			int NoIndex = (!isPhoto) ? 1 : 0;
			string watermarkValue = isPhoto ? TemplateClass.watermarkPhotoKey : TemplateClass.watermarkVideoKey;
			string backgroundValue = isPhoto ? TemplateClass.backgroundPhotoKey : TemplateClass.backgroundVideoKey;
			string watermarkPath = Settings.GetValueString(watermarkValue);
			string noBackgroundPath = Path.Combine(CameraControlClass.FolderForTemp, watermarkValue);
			System.Drawing.Image watermarkImage = null;
			List<System.Drawing.Image> capturedImages = new List<System.Drawing.Image>();
			List<int> scaleWidths = new List<int>();
			List<int> scaleHeights = new List<int>();
			List<FileInfo> fileInfos = new List<FileInfo>();
			List<string> pathForDatas = new List<string>();
			for (int i = 0; i < capturedPaths.Count; i++)
			{
				capturedImages.Add(System.Drawing.Image.FromFile(capturedPaths[i]));
				float scale = Math.Min((float)capturedImages[i].Width / (float)TemplateClass.PaperWidthPhoto, (float)capturedImages[i].Height / (float)TemplateClass.PaperHeightPhoto);
				scaleWidths.Add((int)((float)capturedImages[i].Width / scale));
				scaleHeights.Add((int)((float)capturedImages[i].Height / scale));
				fileInfos.Add(new FileInfo(capturedPaths[i]));
				pathForDatas.Add(Path.Combine(fileInfos[i].Directory.FullName, fileInfos[i].Name.Split(new char[]
				{
					'.'
				})[0] + "_1" + fileInfos[i].Extension));
			}
			if (!TemplateClass.NoWatermark[NoIndex])
			{
				watermarkImage = System.Drawing.Image.FromFile(watermarkPath);
			}
			System.Drawing.Image resultImage = new Bitmap(TemplateClass.PaperWidthPhoto, TemplateClass.PaperHeightPhoto);
			string colorString = Settings.GetValueString("colorgreenscreen");
			if (string.IsNullOrEmpty(colorString))
			{
				colorString = "#000000";
				Settings.SetValue("colorgreenscreen", colorString, true);
			}
			System.Windows.Media.Color themeColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(colorString);
			System.Drawing.Color drawingcolor = System.Drawing.Color.FromArgb((int)themeColor.A, (int)themeColor.R, (int)themeColor.G, (int)themeColor.B);
			SolidBrush brush = new SolidBrush(drawingcolor);
			if (TemplateClass.IsHaveImageTemplate(isPhoto))
			{
				if (TemplateClass.NoBackground[NoIndex])
				{
					using (System.Drawing.Image greenImage = new Bitmap(TemplateClass.PaperWidthPhoto, TemplateClass.PaperHeightPhoto))
					{
						using (Graphics graph = Graphics.FromImage(greenImage))
						{
							graph.InterpolationMode = InterpolationMode.High;
							graph.CompositingQuality = CompositingQuality.HighQuality;
							graph.SmoothingMode = SmoothingMode.AntiAlias;
							graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), new Rectangle(0, 0, greenImage.Width, greenImage.Height));
							greenImage.Save(noBackgroundPath);
						}
					}
				}
				for (int j = 0; j < capturedPaths.Count; j++)
				{
					using (Graphics graph2 = Graphics.FromImage(resultImage))
					{
						graph2.InterpolationMode = InterpolationMode.High;
						graph2.CompositingQuality = CompositingQuality.HighQuality;
						graph2.SmoothingMode = SmoothingMode.AntiAlias;
						if (isHaveGreenEffect)
						{
							graph2.FillRectangle(brush, new RectangleF(0f, 0f, (float)scaleWidths[j], (float)scaleHeights[j]));
						}
						graph2.DrawImage(TemplateClass.GetPhotoCrop(capturedImages[j], j, isPhoto, 0), TemplateClass.GetPhotoRectangle(j, isPhoto, 0));
						if (j == capturedPaths.Count - 1 && TemplateClass.ControlRawImage())
						{
							graph2.DrawImage(TemplateClass.GetRawPhotoCrop(System.Drawing.Image.FromFile(DSLRPhoto.lastCapturedPhoto)), TemplateClass.GetRawPhotoRentangle());
							if (TemplateClass.IsRawImageBack())
							{
								graph2.DrawImage(TemplateClass.GetPhotoCrop(capturedImages[j], j, isPhoto, 0), TemplateClass.GetPhotoRectangle(j, isPhoto, 0));
							}
						}
						resultImage.Save(pathForDatas[j]);
					}
					if (isHaveGreenEffect)
					{
						string backgroundPath = TemplateClass.NoBackground[NoIndex] ? noBackgroundPath : Settings.GetValueString(backgroundValue);
						ExtensionMethod.ColorKeyGreenEffect(pathForDatas[j], Cv2.ImDecode(File.ReadAllBytes(backgroundPath), ImreadModes.AnyColor), pathForDatas[j], CameraControlClass.trashOld);
					}
					if (isHaveAIBackground)
					{
						string AIBackgroundFolderPath = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "AIBAckground");
						string saveAIBImagePath = Path.Combine(AIBackgroundFolderPath, Path.GetFileName(capturedPaths[j]));
						ExtensionMethod.AITaskRender(capturedPaths, j, AIBackgroundFolderPath, saveAIBImagePath);
						capturedPaths[j] = saveAIBImagePath;
						capturedImages[j] = System.Drawing.Image.FromFile(saveAIBImagePath);
					}
				}
				using (Graphics graph3 = Graphics.FromImage(resultImage))
				{
					if (!TemplateClass.NoBackground[NoIndex])
					{
						using (System.Drawing.Image backgroundImage = System.Drawing.Image.FromFile(Settings.GetValueString(backgroundValue)))
						{
							graph3.DrawImage(backgroundImage, 0, 0, backgroundImage.Width, backgroundImage.Height);
						}
					}
					for (int k = 0; k < capturedPaths.Count; k++)
					{
						using (System.Drawing.Image greenImage2 = System.Drawing.Image.FromFile(pathForDatas[k]))
						{
							graph3.InterpolationMode = InterpolationMode.High;
							graph3.CompositingQuality = CompositingQuality.HighQuality;
							graph3.SmoothingMode = SmoothingMode.AntiAlias;
							graph3.DrawImage(greenImage2, new Rectangle(0, 0, greenImage2.Width, greenImage2.Height));
						}
						File.Delete(pathForDatas[k]);
					}
					if (!TemplateClass.NoWatermark[NoIndex])
					{
						graph3.DrawImage(watermarkImage, new Rectangle(0, 0, watermarkImage.Width, watermarkImage.Height));
					}
					if (TemplateClass.IsHaveText)
					{
						TemplateClass.WriteFontInfo fontInfo = TemplateClass.GetFontInfo;
						if (DSLR.NameAndSurname.Length > 0 && fontInfo.selectFont.isFontAvailable)
						{
							StringFormat format = new StringFormat();
							format.Alignment = StringAlignment.Near;
							format.LineAlignment = StringAlignment.Near;
							graph3.DrawString(DSLR.NameAndSurname, fontInfo.selectFont.font, ExtensionMethod.GetDrawingSolidBrush("selectedfontcolor"), fontInfo.point, format);
						}
					}
					resultImage.Save(savePath, ExtensionMethod.ResultImageCodecInfo, ExtensionMethod.ResultImageEncoderParameters);
					goto IL_635;
				}
			}
			using (Graphics graph4 = Graphics.FromImage(resultImage))
			{
				graph4.InterpolationMode = InterpolationMode.High;
				graph4.CompositingQuality = CompositingQuality.HighQuality;
				graph4.SmoothingMode = SmoothingMode.AntiAlias;
				graph4.DrawImage(capturedImages[0], new Rectangle(-((scaleWidths[0] - resultImage.Width) / 2), -((scaleHeights[0] - resultImage.Height) / 2), scaleWidths[0], scaleHeights[0]));
				graph4.DrawImage(watermarkImage, new Rectangle(0, 0, watermarkImage.Width, watermarkImage.Height));
				resultImage.Save(savePath, ExtensionMethod.ResultImageCodecInfo, ExtensionMethod.ResultImageEncoderParameters);
			}
			IL_635:
			FileInformation result;
			using (MagickImage mIamge = new MagickImage(savePath))
			{
				result = new FileInformation
				{
					Filename = Path.GetFileName(savePath),
					Directory = Path.GetDirectoryName(savePath),
					Filesize = (long)mIamge.ToByteArray().Length,
					Height = (int)mIamge.Height,
					Width = (int)mIamge.Width
				};
			}
			return result;
		}

		// Token: 0x06000ACF RID: 2767 RVA: 0x0003F4A0 File Offset: 0x0003D6A0
		public static string CMDPython(string path, string pythonName, string imagePath, string saveImageName)
		{
			Process p = new Process();
			p.StartInfo.FileName = "Python.exe";
			p.StartInfo.Arguments = string.Concat(new string[]
			{
				pythonName,
				" \"",
				imagePath,
				"\" ",
				saveImageName,
				"\""
			});
			p.StartInfo.WorkingDirectory = path;
			p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.CreateNoWindow = true;
			p.Start();
			bool isPythonRunning = true;
			while (isPythonRunning)
			{
				if (p.HasExited)
				{
					isPythonRunning = false;
					string pythonOutput = p.StandardOutput.ReadToEnd();
					string pythonError = p.StandardError.ReadToEnd();
					if (p.ExitCode == 0)
					{
						global::Debug.Log("ExtentionMethod", "CMDPython", "Python script executed successfully.", 2177);
						return pythonOutput;
					}
					global::Debug.Log("ExtentionMethod", "Failed to execute Python script.", "CMDPython", 2184);
					MessageBoxWindow.CreateWindow("Python Error", pythonError, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				}
			}
			return "pythonError";
		}

		// Token: 0x06000AD0 RID: 2768 RVA: 0x0003F5D4 File Offset: 0x0003D7D4
		public static string PrintControlScale(string path)
		{
			FileInfo pathInfo = new FileInfo(path);
			string savePath = Path.Combine(pathInfo.Directory.FullName, pathInfo.Name.Split(new char[]
			{
				'.'
			})[0] + "_p" + pathInfo.Extension);
			if (PrinterPage.printTop != 0.0 || PrinterPage.printBottom != 0.0 || PrinterPage.printLeft != 0.0 || PrinterPage.printRight != 0.0)
			{
				System.Drawing.Image returnImage;
				using (Bitmap resultOldImage = (Bitmap)System.Drawing.Image.FromFile(path))
				{
					returnImage = ExtensionMethod.CropAndResizeImage(resultOldImage, PrinterPage.printTop, PrinterPage.printRight, PrinterPage.printBottom, PrinterPage.printLeft);
				}
				returnImage.Save(savePath);
				return savePath;
			}
			return path;
		}

		// Token: 0x06000AD1 RID: 2769 RVA: 0x0003F6B0 File Offset: 0x0003D8B0
		public static void CreateWriteJson<T>(T eventData, string path)
		{
			try
			{
				string json = JsonConvert.SerializeObject(eventData);
				File.WriteAllText(path, json);
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x06000AD2 RID: 2770 RVA: 0x0003F6E8 File Offset: 0x0003D8E8
		public static T ReadJson<T>(string path)
		{
			if (File.Exists(path))
			{
				string json = File.ReadAllText(path);
				return JsonConvert.DeserializeObject<T>(json);
			}
			if (typeof(IEnumerable).IsAssignableFrom(typeof(T)))
			{
				return (T)((object)Activator.CreateInstance(typeof(T)));
			}
			return default(T);
		}

		// Token: 0x06000AD3 RID: 2771 RVA: 0x0003F744 File Offset: 0x0003D944
		public static System.Drawing.Image CropImage(System.Drawing.Image image, int Width, int Height, int StartAtX, int StartAtY)
		{
			System.Drawing.Image result;
			try
			{
				if (image.Height < Height)
				{
					Height = image.Height;
				}
				if (image.Width < Width)
				{
					Width = image.Width;
				}
				Bitmap bmPhoto = new Bitmap(Width, Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
				bmPhoto.SetResolution(72f, 72f);
				Graphics grPhoto = Graphics.FromImage(bmPhoto);
				grPhoto.SmoothingMode = SmoothingMode.AntiAlias;
				grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
				grPhoto.PixelOffsetMode = PixelOffsetMode.HighQuality;
				grPhoto.DrawImage(image, new Rectangle(0, 0, Width, Height), StartAtX, StartAtY, Width, Height, GraphicsUnit.Pixel);
				MemoryStream mm = new MemoryStream();
				bmPhoto.Save(mm, ImageFormat.Jpeg);
				image.Dispose();
				bmPhoto.Dispose();
				grPhoto.Dispose();
				using (System.Drawing.Image outimage = System.Drawing.Image.FromStream(mm))
				{
					result = (System.Drawing.Image)outimage.Clone();
				}
			}
			catch (Exception ex)
			{
				throw new Exception("Error cropping image, the error was: " + ex.Message);
			}
			return result;
		}

		// Token: 0x06000AD4 RID: 2772 RVA: 0x0003F840 File Offset: 0x0003DA40
		public static ColorKeyAlphaEffect GetTransparentEffect()
		{
			ColorKeyAlphaEffect effect = new ColorKeyAlphaEffect();
			System.Windows.Media.Brush brush = Effect.ImplicitInput;
			effect.Input = brush;
			return effect;
		}

		// Token: 0x06000AD5 RID: 2773 RVA: 0x0003F864 File Offset: 0x0003DA64
		public static BitmapImage ConvertBitmap2BitmapSource(Bitmap src)
		{
			MemoryStream ms = new MemoryStream();
			src.Save(ms, ImageFormat.Bmp);
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			ms.Seek(0L, SeekOrigin.Begin);
			image.StreamSource = ms;
			image.EndInit();
			return image;
		}

		// Token: 0x06000AD6 RID: 2774 RVA: 0x0003F8A8 File Offset: 0x0003DAA8
		public static Bitmap ColorKeyGreenEffect(Bitmap sourceBitmap, Mat Background, byte GreenScaleFactor)
		{
			Mat sourceMat = sourceBitmap.ToMat();
			Bitmap result;
			using (ReplaceGreenScreenFilter filter = new ReplaceGreenScreenFilter(Background))
			{
				filter.GreenScale = GreenScaleFactor;
				filter.Apply(sourceMat);
				result = sourceMat.ToBitmap();
			}
			return result;
		}

		// Token: 0x06000AD7 RID: 2775 RVA: 0x0003F8F8 File Offset: 0x0003DAF8
		public static void ColorKeyGreenEffect(string pathSource, Mat background, string pathSave, byte GreenScaleFactor)
		{
			Mat imageMat = Cv2.ImDecode(File.ReadAllBytes(pathSource), ImreadModes.AnyColor);
			using (ReplaceGreenScreenFilter filter = new ReplaceGreenScreenFilter(background))
			{
				filter.GreenScale = GreenScaleFactor;
				filter.Apply(imageMat);
				imageMat.SaveImage(pathSave, null);
			}
		}

		// Token: 0x06000AD8 RID: 2776 RVA: 0x0003F94C File Offset: 0x0003DB4C
		public static Mat ColorKeyGreenEffect(Mat Source, Mat Background, byte GreenScaleFactor)
		{
			using (ReplaceGreenScreenFilter filter = new ReplaceGreenScreenFilter(Background))
			{
				filter.GreenScale = GreenScaleFactor;
				filter.Apply(Source);
			}
			return Source;
		}

		// Token: 0x06000AD9 RID: 2777 RVA: 0x0003F990 File Offset: 0x0003DB90
		public static Bitmap BitmapSourceToBitmap(BitmapSource srs)
		{
			int width = srs.PixelWidth;
			int height = srs.PixelHeight;
			int stride = width * ((srs.Format.BitsPerPixel + 7) / 8);
			IntPtr ptr = IntPtr.Zero;
			Bitmap result;
			try
			{
				ptr = Marshal.AllocHGlobal(height * stride);
				srs.CopyPixels(new Int32Rect(0, 0, width, height), ptr, height * stride, stride);
				using (Bitmap btm = new Bitmap(width, height, stride, System.Drawing.Imaging.PixelFormat.Format1bppIndexed, ptr))
				{
					result = new Bitmap(btm);
				}
			}
			finally
			{
				if (ptr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(ptr);
				}
			}
			return result;
		}

		// Token: 0x06000ADA RID: 2778 RVA: 0x0003FA40 File Offset: 0x0003DC40
		public static Bitmap GetBitmap(BitmapSource source)
		{
			Bitmap bmp = new Bitmap(source.PixelWidth, source.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			BitmapData data = bmp.LockBits(new Rectangle(System.Drawing.Point.Empty, bmp.Size), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
			source.CopyPixels(Int32Rect.Empty, data.Scan0, data.Height * data.Stride, data.Stride);
			bmp.UnlockBits(data);
			return bmp;
		}

		// Token: 0x06000ADB RID: 2779 RVA: 0x0003FAB0 File Offset: 0x0003DCB0
		public static void SetElementSizeToFitInsideArea(ref Grid elementToResize, FrameworkElement containerElement, double referenceWidth, double referenceHeight)
		{
			double canvasScale = Math.Min(containerElement.ActualWidth / referenceWidth, containerElement.ActualHeight / referenceHeight);
			elementToResize.Width = canvasScale * referenceWidth;
			elementToResize.Height = canvasScale * referenceHeight;
		}

		// Token: 0x06000ADC RID: 2780 RVA: 0x0003FAE8 File Offset: 0x0003DCE8
		public static void LoadLanguageString(string sourceFromSettings, object anyObject)
		{
			if (!string.IsNullOrEmpty(sourceFromSettings))
			{
				PropertyInfo contentProperty = anyObject.GetType().GetProperty("Content");
				if (contentProperty != null && contentProperty.CanWrite)
				{
					contentProperty.SetValue(anyObject, sourceFromSettings);
					return;
				}
				PropertyInfo textProperty = anyObject.GetType().GetProperty("Text");
				if (textProperty != null && textProperty.CanWrite)
				{
					textProperty.SetValue(anyObject, sourceFromSettings);
				}
			}
		}

		// Token: 0x06000ADD RID: 2781 RVA: 0x0003FB54 File Offset: 0x0003DD54
		public static Task CopyFileToOutputFolder(string sourceFilePath, VariationType variationType, bool isRaw = false)
		{
			ExtensionMethod.<CopyFileToOutputFolder>d__82 <CopyFileToOutputFolder>d__;
			<CopyFileToOutputFolder>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CopyFileToOutputFolder>d__.sourceFilePath = sourceFilePath;
			<CopyFileToOutputFolder>d__.variationType = variationType;
			<CopyFileToOutputFolder>d__.isRaw = isRaw;
			<CopyFileToOutputFolder>d__.<>1__state = -1;
			<CopyFileToOutputFolder>d__.<>t__builder.Start<ExtensionMethod.<CopyFileToOutputFolder>d__82>(ref <CopyFileToOutputFolder>d__);
			return <CopyFileToOutputFolder>d__.<>t__builder.Task;
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x0003FBA8 File Offset: 0x0003DDA8
		public static void DeleteAllFilesInFolder(string folderPath)
		{
			if (Directory.Exists(folderPath))
			{
				try
				{
					foreach (string file in Directory.GetFiles(folderPath))
					{
						File.Delete(file);
					}
					foreach (string directory in Directory.GetDirectories(folderPath))
					{
						Directory.Delete(directory, true);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error deleting files in folder " + folderPath + ": " + ex.Message);
				}
			}
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x0003FC38 File Offset: 0x0003DE38
		public static Task DeleteFilesWithSameDateInNameAsync(string filePath, int delayMilliseconds, string[] extensionsToDelete = null, string[] extraFolders = null, int maxRetryCount = 3)
		{
			ExtensionMethod.<DeleteFilesWithSameDateInNameAsync>d__84 <DeleteFilesWithSameDateInNameAsync>d__;
			<DeleteFilesWithSameDateInNameAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<DeleteFilesWithSameDateInNameAsync>d__.filePath = filePath;
			<DeleteFilesWithSameDateInNameAsync>d__.delayMilliseconds = delayMilliseconds;
			<DeleteFilesWithSameDateInNameAsync>d__.extensionsToDelete = extensionsToDelete;
			<DeleteFilesWithSameDateInNameAsync>d__.extraFolders = extraFolders;
			<DeleteFilesWithSameDateInNameAsync>d__.maxRetryCount = maxRetryCount;
			<DeleteFilesWithSameDateInNameAsync>d__.<>1__state = -1;
			<DeleteFilesWithSameDateInNameAsync>d__.<>t__builder.Start<ExtensionMethod.<DeleteFilesWithSameDateInNameAsync>d__84>(ref <DeleteFilesWithSameDateInNameAsync>d__);
			return <DeleteFilesWithSameDateInNameAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x0003FC9C File Offset: 0x0003DE9C
		private static bool IsFileLocked(string filePath)
		{
			try
			{
				using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					stream.Close();
				}
			}
			catch (IOException)
			{
				return true;
			}
			return false;
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x0003FD14 File Offset: 0x0003DF14
		[CompilerGenerated]
		internal static string <IsValidEmail>g__DomainMapper|65_0(Match match)
		{
			IdnMapping idn = new IdnMapping();
			string domainName = idn.GetAscii(match.Groups[2].Value);
			return match.Groups[1].Value + domainName;
		}

		// Token: 0x04000A3C RID: 2620
		public static string dateTimeFormat = "dd.MM.yyyy_HH.mm.ss";

		// Token: 0x04000A3D RID: 2621
		public const string dateTimeEventFormat = "dd.MM.yyyy HH:mm:ss";

		// Token: 0x04000A3E RID: 2622
		private static readonly Regex _regex = new Regex("[^0-9.-]+");

		// Token: 0x04000A3F RID: 2623
		public static int printScaleOffset = 25;

		// Token: 0x04000A40 RID: 2624
		private static bool EventSyncIsWorking = false;

		// Token: 0x04000A41 RID: 2625
		private static List<System.Windows.Media.FontFamily> ResourcesFonts;

		// Token: 0x04000A42 RID: 2626
		public static List<ExtensionMethod.SystemPrinter> printers;

		// Token: 0x04000A43 RID: 2627
		public static PrivateFontCollection appFonts;

		// Token: 0x02000298 RID: 664
		public class SystemPrinter
		{
			// Token: 0x1700025E RID: 606
			// (get) Token: 0x06001102 RID: 4354 RVA: 0x0006362E File Offset: 0x0006182E
			// (set) Token: 0x06001103 RID: 4355 RVA: 0x00063636 File Offset: 0x00061836
			public string Name { get; set; }

			// Token: 0x1700025F RID: 607
			// (get) Token: 0x06001104 RID: 4356 RVA: 0x0006363F File Offset: 0x0006183F
			// (set) Token: 0x06001105 RID: 4357 RVA: 0x00063647 File Offset: 0x00061847
			public bool Active { get; set; }
		}

		// Token: 0x02000299 RID: 665
		public enum FontType
		{
			// Token: 0x0400114C RID: 4428
			Normal,
			// Token: 0x0400114D RID: 4429
			Bold
		}
	}
}
