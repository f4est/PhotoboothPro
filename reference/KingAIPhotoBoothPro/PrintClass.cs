using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Xps;
using ImageMagick;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages;
using MongoDB.Bson;

// Token: 0x0200000B RID: 11
public static class PrintClass
{
	// Token: 0x0600002F RID: 47 RVA: 0x00003030 File Offset: 0x00001230
	public static int GetPrintNumber()
	{
		if (EventManagementPage.GetCurrentEvent() == null)
		{
			return 0;
		}
		string eventPath = EventManagementPage.GetCurrentEvent().DirectoryPath;
		PrintClass.GenerateDataDir(eventPath);
		string dataDir = Path.Combine(eventPath, "Data");
		PrintClass.localDate = DateTime.Now;
		string filePath = Path.Combine(dataDir, "Print_data.csv");
		if (!File.Exists(filePath))
		{
			return 0;
		}
		return File.ReadAllText(filePath).Split(new char[]
		{
			'\n'
		}).Length - 1;
	}

	// Token: 0x06000030 RID: 48 RVA: 0x000030A0 File Offset: 0x000012A0
	public static int GetPrintNumber(OperationalEvent operationalEvent)
	{
		if (operationalEvent == null)
		{
			return 0;
		}
		string eventPath = operationalEvent.DirectoryPath;
		PrintClass.GenerateDataDir(eventPath);
		string dataDir = Path.Combine(eventPath, "Data");
		PrintClass.localDate = DateTime.Now;
		string filePath = Path.Combine(dataDir, "Print_data.csv");
		if (!File.Exists(filePath))
		{
			return 0;
		}
		return File.ReadAllText(filePath).Split(new char[]
		{
			'\n'
		}).Length - 1;
	}

	// Token: 0x06000031 RID: 49 RVA: 0x00003108 File Offset: 0x00001308
	private static void GenerateDataDir(string directoryPath)
	{
		string dataDir = Path.Combine(directoryPath, "Data");
		if (!Directory.Exists(dataDir))
		{
			Directory.CreateDirectory(dataDir);
		}
	}

	// Token: 0x06000032 RID: 50 RVA: 0x00003134 File Offset: 0x00001334
	private static Task<PrintClass.PrintResponse> PostRequestPrint(PrintClass.PrintRequest printInfo)
	{
		PrintClass.<PostRequestPrint>d__20 <PostRequestPrint>d__;
		<PostRequestPrint>d__.<>t__builder = AsyncTaskMethodBuilder<PrintClass.PrintResponse>.Create();
		<PostRequestPrint>d__.printInfo = printInfo;
		<PostRequestPrint>d__.<>1__state = -1;
		<PostRequestPrint>d__.<>t__builder.Start<PrintClass.<PostRequestPrint>d__20>(ref <PostRequestPrint>d__);
		return <PostRequestPrint>d__.<>t__builder.Task;
	}

	// Token: 0x06000033 RID: 51 RVA: 0x00003178 File Offset: 0x00001378
	public static void WritePrintToExcel()
	{
		string eventPath = EventManagementPage.GetCurrentEvent().DirectoryPath;
		PrintClass.GenerateDataDir(eventPath);
		string dataDir = Path.Combine(eventPath, "Data");
		string[] PrintrowDataTemp = new string[2];
		PrintClass.localDate = DateTime.Now;
		string PrintfilePath = Path.Combine(dataDir, "Print_data.csv");
		StreamWriter PrintoutStream;
		if (!File.Exists(PrintfilePath))
		{
			PrintrowDataTemp[0] = "Tarih Saat";
			PrintrowDataTemp[1] = "Fotoğraf İsmi";
			PrintoutStream = File.CreateText(PrintfilePath);
			PrintoutStream.WriteLine(string.Format("{0},{1}", PrintrowDataTemp[0], PrintrowDataTemp[1]));
			PrintoutStream.Close();
		}
		PrintrowDataTemp[0] = string.Format("{5:00}-{4:00}-{3:00}-{2:00}-{1:00}-{0:00}", new object[]
		{
			PrintClass.localDate.Second,
			PrintClass.localDate.Minute,
			PrintClass.localDate.Hour,
			PrintClass.localDate.Day,
			PrintClass.localDate.Month,
			PrintClass.localDate.Year
		});
		PrintrowDataTemp[1] = Path.GetFileName(PrintClass.printFilename);
		PrintoutStream = File.AppendText(PrintfilePath);
		PrintoutStream.WriteLine(string.Format("{0},{1}", PrintrowDataTemp[0], PrintrowDataTemp[1]));
		PrintoutStream.Close();
	}

	// Token: 0x06000034 RID: 52 RVA: 0x000032AD File Offset: 0x000014AD
	public static void GetPrintInformation()
	{
		PrintClass.printerName = Settings.GetValueString("printer_name");
	}

	// Token: 0x06000035 RID: 53 RVA: 0x000032C0 File Offset: 0x000014C0
	public static void ControlSyncThread()
	{
		if (PrintClass.printClousSyncThread == null || !PrintClass.printClousSyncThread.IsAlive)
		{
			PrintClass.printClousSyncThread = new Thread(delegate()
			{
				PrintClass.<>c.<<ControlSyncThread>b__23_0>d <<ControlSyncThread>b__23_0>d;
				<<ControlSyncThread>b__23_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<ControlSyncThread>b__23_0>d.<>1__state = -1;
				<<ControlSyncThread>b__23_0>d.<>t__builder.Start<PrintClass.<>c.<<ControlSyncThread>b__23_0>d>(ref <<ControlSyncThread>b__23_0>d);
			});
			PrintClass.printClousSyncThread.Start();
		}
	}

	// Token: 0x06000036 RID: 54 RVA: 0x00003313 File Offset: 0x00001513
	private static void PrintDataControl()
	{
		if (PrintClass.printRequestList == null)
		{
			if (File.Exists(EventManagementPage.GetCurrentEvent().PrintRequestJsonPath))
			{
				PrintClass.printRequestList = ExtensionMethod.ReadJson<List<PrintClass.PrintRequest>>(EventManagementPage.GetCurrentEvent().PrintRequestJsonPath);
				return;
			}
			PrintClass.printRequestList = new List<PrintClass.PrintRequest>();
		}
	}

	// Token: 0x06000037 RID: 55 RVA: 0x0000334C File Offset: 0x0000154C
	public static void StartPrint(string printFileNameString, int copy = 1)
	{
		PrintClass.GetPrintInformation();
		PrintClass.ControlSyncThread();
		PrintClass.printFilename = printFileNameString;
		PrintClass.internetEnable = ExtensionMethod.IsInternetAvailable();
		PrintClass._t2 = new Thread(new ThreadStart(PrintClass.PrintThread));
		PrintClass._t2.ApartmentState = ApartmentState.STA;
		PrintClass._t2.Start();
		if (copy > 1)
		{
			new Thread(delegate()
			{
				for (int i = 1; i < copy; i++)
				{
					Thread.Sleep(2000);
					new Thread(new ThreadStart(PrintClass.PrintThread)).Start();
				}
			}).Start();
		}
	}

	// Token: 0x06000038 RID: 56 RVA: 0x000033CC File Offset: 0x000015CC
	public static void CreatePrintData(MediaClassBase media)
	{
		PrintClass.<>c__DisplayClass26_0 CS$<>8__locals1 = new PrintClass.<>c__DisplayClass26_0();
		CS$<>8__locals1.media = media;
		List<PrintData> printData = new List<PrintData>();
		if (File.Exists(EventManagementPage.GetCurrentEvent().PrintJsonPath))
		{
			printData = ExtensionMethod.ReadJson<List<PrintData>>(EventManagementPage.GetCurrentEvent().PrintJsonPath);
		}
		CS$<>8__locals1.newPrintData = new PrintData
		{
			Id = ObjectId.GenerateNewId().ToString(),
			IsCloudSync = false,
			EventID = EventManagementPage.GetCurrentEvent().IndexID,
			PrintDatetime = DateTime.Now,
			PrintFileInfo = CS$<>8__locals1.media.fileDetails,
			PrinterInfo = new PrinterInfo
			{
				PrinterName = PrintClass.printerName,
				PrinterSize = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageInfo
			}
		};
		printData.Add(CS$<>8__locals1.newPrintData);
		ExtensionMethod.CreateWriteJson<List<PrintData>>(printData, EventManagementPage.GetCurrentEvent().PrintJsonPath);
		PrintClass.PrintDataControl();
		Task.Run(delegate()
		{
			PrintClass.<>c__DisplayClass26_0.<<CreatePrintData>b__0>d <<CreatePrintData>b__0>d;
			<<CreatePrintData>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<CreatePrintData>b__0>d.<>4__this = CS$<>8__locals1;
			<<CreatePrintData>b__0>d.<>1__state = -1;
			<<CreatePrintData>b__0>d.<>t__builder.Start<PrintClass.<>c__DisplayClass26_0.<<CreatePrintData>b__0>d>(ref <<CreatePrintData>b__0>d);
			return <<CreatePrintData>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000039 RID: 57 RVA: 0x000034C7 File Offset: 0x000016C7
	private static void PrintThread()
	{
		if (ExtensionMethod.IsInternetAvailable())
		{
			Thread.Sleep(3000);
			if (PrintClass.printMode == PrintClass.PrintMode.WindowsSettings)
			{
				PrintClass.PrintProcessOld();
			}
			else
			{
				PrintClass.PrintImageWithValidation(PrintClass.printFilename, PrintClass.printerName);
			}
			PrintClass.WritePrintToExcel();
		}
	}

	// Token: 0x0600003A RID: 58 RVA: 0x000034FC File Offset: 0x000016FC
	private static bool IsVirtualPrinter(string nameprinter)
	{
		return PrintClass.virtualPrinterKeywords.Any((string keyword) => nameprinter.Contains(keyword));
	}

	// Token: 0x0600003B RID: 59 RVA: 0x0000352C File Offset: 0x0000172C
	private static bool IsPrinterSupportingSize_Inch(PrintQueue printerQueue, double widthInch, double heightInch)
	{
		PrintTicket defaultTicket = printerQueue.DefaultPrintTicket;
		PrintCapabilities capabilities = printerQueue.GetPrintCapabilities();
		if (PrintClass.IsVirtualPrinter(printerQueue.Name.ToLower()))
		{
			return true;
		}
		double tolerance = 0.2;
		if (capabilities.PageMediaSizeCapability != null && capabilities.PageMediaSizeCapability.Count != 0)
		{
			foreach (PageMediaSize size in capabilities.PageMediaSizeCapability)
			{
				if (size.Width != null && size.Height != null)
				{
					double pageWidthInch = size.Width.Value / 96.0;
					double pageHeightInch = size.Height.Value / 96.0;
					if (Math.Abs(pageWidthInch - widthInch) < tolerance && Math.Abs(pageHeightInch - heightInch) < tolerance)
					{
						return true;
					}
				}
			}
			return false;
		}
		if (capabilities.OrientedPageMediaHeight != null && capabilities.OrientedPageMediaWidth != null)
		{
			double pageWidthInch2 = capabilities.OrientedPageMediaWidth.Value / 96.0;
			double pageHeightInch2 = capabilities.OrientedPageMediaHeight.Value / 96.0;
			return true;
		}
		return false;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00003690 File Offset: 0x00001890
	public static FixedDocument CreateDocumentForPrinterArea(string imagePath, PrintQueue printer)
	{
		PrintCapabilities caps = printer.GetPrintCapabilities();
		PageImageableArea area = caps.PageImageableArea;
		if (area == null)
		{
			throw new Exception("Yazıcının yazdırılabilir alanı alınamadı.");
		}
		double pageWidth = caps.OrientedPageMediaWidth ?? area.ExtentWidth;
		double pageHeight = caps.OrientedPageMediaHeight ?? area.ExtentHeight;
		double imageableWidth = area.ExtentWidth;
		double imageableHeight = area.ExtentHeight;
		BitmapImage img = new BitmapImage();
		img.BeginInit();
		img.CacheOption = BitmapCacheOption.OnLoad;
		img.UriSource = new Uri(imagePath, UriKind.Absolute);
		img.EndInit();
		img.Freeze();
		double imageWidth = (double)img.PixelWidth;
		double imageHeight = (double)img.PixelHeight;
		bool isImagePortrait = imageHeight >= imageWidth;
		bool isPagePortrait = imageableHeight >= imageableWidth;
		if (isImagePortrait != isPagePortrait)
		{
			double temp = pageWidth;
			pageWidth = pageHeight;
			pageHeight = temp;
			temp = imageableWidth;
			imageableWidth = imageableHeight;
			imageableHeight = temp;
		}
		FixedDocument doc = new FixedDocument();
		doc.DocumentPaginator.PageSize = new System.Windows.Size(pageWidth, pageHeight);
		FixedPage page = new FixedPage
		{
			Width = pageWidth,
			Height = pageHeight
		};
		System.Windows.Controls.Image image = new System.Windows.Controls.Image
		{
			Source = img,
			Stretch = Stretch.Uniform,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};
		if (img.PixelHeight >= img.PixelWidth != imageableHeight >= imageableWidth)
		{
			image.LayoutTransform = new RotateTransform(-90.0);
		}
		Grid container = new Grid
		{
			Width = imageableWidth,
			Height = imageableHeight,
			HorizontalAlignment = HorizontalAlignment.Center,
			VerticalAlignment = VerticalAlignment.Center
		};
		container.Children.Add(image);
		FixedPage.SetLeft(container, area.OriginWidth);
		FixedPage.SetTop(container, area.OriginHeight);
		page.Children.Add(container);
		PageContent content = new PageContent();
		((IAddChild)content).AddChild(page);
		doc.Pages.Add(content);
		return doc;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x00003888 File Offset: 0x00001A88
	public static void PrintImageWithValidation(string imagePath, string printerName)
	{
		Application.Current.Dispatcher.Invoke(delegate()
		{
			LocalPrintServer printServer = new LocalPrintServer();
			PrintQueue queue = null;
			int widthInc = 0;
			int heightInc = 0;
			using (MagickImage mIamge = new MagickImage(imagePath))
			{
				int widthPx = (int)mIamge.Width;
				int heightPx = (int)mIamge.Height;
				widthInc = widthPx / 300;
				heightInc = heightPx / 300;
			}
			try
			{
				queue = printServer.GetPrintQueue(printerName);
			}
			catch
			{
				MessageBox.Show("Yazıcı bulunamadı: " + printerName, "Hata", MessageBoxButton.OK, MessageBoxImage.Hand);
				return;
			}
			PrintTicket ticket = queue.DefaultPrintTicket;
			if (!PrintClass.isUseDefaultPrintSettings)
			{
				ticket.PageMediaSize = new PageMediaSize((double)(widthInc * 96), (double)(heightInc * 96));
			}
			PageOrientation orientation = (widthInc >= heightInc) ? PageOrientation.Portrait : PageOrientation.Landscape;
			if (PrintClass.isForceLandscape)
			{
				orientation = PageOrientation.Landscape;
			}
			else if (PrintClass.isForcePortrait)
			{
				orientation = PageOrientation.Portrait;
			}
			ticket.PageOrientation = new PageOrientation?(orientation);
			FixedDocument doc = PrintClass.CreateDocumentForPrinterArea(imagePath, queue);
			XpsDocumentWriter writer = PrintQueue.CreateXpsDocumentWriter(queue);
			writer.Write(doc.DocumentPaginator, ticket);
		});
	}

	// Token: 0x0600003E RID: 62 RVA: 0x000038C4 File Offset: 0x00001AC4
	public static void PrintProcessOld()
	{
		ProcessStartInfo proc = new ProcessStartInfo();
		proc.FileName = "C:\\windows\\system32\\cmd.exe";
		proc.Arguments = string.Concat(new string[]
		{
			"/c rundll32 C:\\WINDOWS\\system32\\shimgvw.dll  ImageView_PrintTo  /pt   \"",
			PrintClass.printFilename.Replace("/", "\\"),
			"\"    \"",
			PrintClass.printerName,
			"\""
		});
		proc.CreateNoWindow = true;
		proc.UseShellExecute = false;
		Process process = new Process();
		process.StartInfo = proc;
		process.Start();
		process.WaitForExit();
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00003954 File Offset: 0x00001B54
	public static System.Drawing.Printing.PaperSize GetPaperSizeFromPixels(int width, int height, int dpi = 300)
	{
		int paperWidth = (int)((float)width / (float)dpi * 100f);
		int paperHeight = (int)((float)height / (float)dpi * 100f);
		return new System.Drawing.Printing.PaperSize(string.Format("{0}x{1}", width, height), paperWidth, paperHeight);
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00003998 File Offset: 0x00001B98
	public static void PrintImageFile(string filePath, string printerName)
	{
		using (PrintDocument pd = new PrintDocument())
		{
			pd.PrinterSettings.PrinterName = printerName;
			System.Drawing.Image img = System.Drawing.Image.FromFile(filePath);
			System.Drawing.Printing.PaperSize ps = PrintClass.GetPaperSizeFromPixels(img.Width, img.Height, 300);
			pd.DefaultPageSettings.PaperSize = ps;
			pd.PrinterSettings.DefaultPageSettings.PaperSize = ps;
			pd.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
			pd.PrintPage += delegate(object sender, PrintPageEventArgs e)
			{
				RectangleF bounds = e.PageBounds;
				float imgWidth = (float)img.Width;
				float imgHeight = (float)img.Height;
				float ratio = Math.Min(bounds.Width / imgWidth, bounds.Height / imgHeight);
				float scaledWidth = imgWidth * ratio;
				float scaledHeight = imgHeight * ratio;
				float left = (bounds.Width - scaledWidth) / 2f;
				float top = (bounds.Height - scaledHeight) / 2f;
				e.Graphics.DrawImage(img, left, top, scaledWidth, scaledHeight);
			};
			pd.Print();
		}
	}

	// Token: 0x04000021 RID: 33
	public static PrintClass.PrintMode printMode;

	// Token: 0x04000022 RID: 34
	public static bool isUseDefaultPrintSettings = false;

	// Token: 0x04000023 RID: 35
	public static bool isForcePortrait = false;

	// Token: 0x04000024 RID: 36
	public static bool isForceLandscape = false;

	// Token: 0x04000025 RID: 37
	private static Thread printClousSyncThread;

	// Token: 0x04000026 RID: 38
	private static string printFilename;

	// Token: 0x04000027 RID: 39
	private static Thread _t2;

	// Token: 0x04000028 RID: 40
	private static string printerName;

	// Token: 0x04000029 RID: 41
	private static DateTime localDate;

	// Token: 0x0400002A RID: 42
	public const string configFilename = "config.xml";

	// Token: 0x0400002B RID: 43
	private static bool internetEnable = false;

	// Token: 0x0400002C RID: 44
	private static int printNumber = 1;

	// Token: 0x0400002D RID: 45
	public static bool isForceQuit = false;

	// Token: 0x0400002E RID: 46
	private static List<PrintClass.PrintRequest> printRequestList;

	// Token: 0x0400002F RID: 47
	private static string[] virtualPrinterKeywords = new string[]
	{
		"pdf",
		"xps",
		"onenote",
		"fax",
		"anydesk",
		"snagit",
		"foxit",
		"doPDF",
		"virtual",
		"print to file"
	};

	// Token: 0x020000E6 RID: 230
	public enum PrintMode
	{
		// Token: 0x04000AB6 RID: 2742
		WindowsSettings,
		// Token: 0x04000AB7 RID: 2743
		NewPrintSettings
	}

	// Token: 0x020000E7 RID: 231
	[Serializable]
	public class PrintRequest
	{
		// Token: 0x04000AB8 RID: 2744
		public PrinterInfo PrinterInfo;

		// Token: 0x04000AB9 RID: 2745
		public FileInformation FileInfo;

		// Token: 0x04000ABA RID: 2746
		public string MediaHash;
	}

	// Token: 0x020000E8 RID: 232
	[Serializable]
	public class PrintResponse : GenericResponse
	{
	}
}
