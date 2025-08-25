using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using Microsoft.Win32;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000049 RID: 73
	public partial class CurrentEventPage : Page
	{
		// Token: 0x17000084 RID: 132
		// (get) Token: 0x060005BC RID: 1468 RVA: 0x00027935 File Offset: 0x00025B35
		// (set) Token: 0x060005BD RID: 1469 RVA: 0x0002793D File Offset: 0x00025B3D
		public OperationalEvent DeclaredOperationalEvent
		{
			get
			{
				return this.declaredOperationalEvent;
			}
			set
			{
				this.declaredOperationalEvent = value;
			}
		}

		// Token: 0x060005BE RID: 1470 RVA: 0x00027946 File Offset: 0x00025B46
		private void Start()
		{
			CurrentEventPage.currentEventUIScript = this;
			this.eventManagement = SettingsPage.eventManagementPage;
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00027959 File Offset: 0x00025B59
		private void ArchiveEventClicked(object sender, RoutedEventArgs e)
		{
			this.eventManagement.ArchiveEventClicked();
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x00027966 File Offset: 0x00025B66
		private void CopyEventClicked(object sender, RoutedEventArgs e)
		{
			this.eventManagement.CopyEvent(this.declaredOperationalEvent);
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00027979 File Offset: 0x00025B79
		private void EditEventClicked(object sender, RoutedEventArgs e)
		{
			this.eventManagement.EditEventClicked();
		}

		// Token: 0x060005C2 RID: 1474 RVA: 0x00027986 File Offset: 0x00025B86
		public void Clear()
		{
			this.txtEventName.Text = "";
			this.txtEventMotto.Text = "";
			this.txtEventCreatedTime.Text = "";
			this.declaredOperationalEvent = null;
		}

		// Token: 0x060005C3 RID: 1475 RVA: 0x000279C0 File Offset: 0x00025BC0
		public void SetOperationalEvent(OperationalEvent newEvent)
		{
			this.DeclaredOperationalEvent = newEvent;
			this.txtEventName.Text = this.DeclaredOperationalEvent.EventName;
			this.txtEventMotto.Text = this.DeclaredOperationalEvent.EventMotto;
			try
			{
				this.txtEventCreatedTime.Text = this.DeclaredOperationalEvent.CreateTime.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture);
			}
			catch (Exception ex)
			{
				this.txtEventCreatedTime.Text = "";
			}
			this.PrintNumberText.Inlines.Clear();
			this.PrintNumberText.Inlines.Add(PrintClass.GetPrintNumber().ToString());
			this.MailNumberText.Inlines.Clear();
			this.MailNumberText.Inlines.Add(MailClass.GetMailNumber().ToString());
			this.ImageNumberText.Inlines.Clear();
			this.ImageNumberText.Inlines.Add(this.DeclaredOperationalEvent.GetMediaNumber().ToString());
		}

		// Token: 0x060005C4 RID: 1476 RVA: 0x00027AD8 File Offset: 0x00025CD8
		public CurrentEventPage()
		{
			this.InitializeComponent();
			this.worker.WorkerReportsProgress = true;
			this.worker.DoWork += this.Worker_DoWork;
			this.worker.ProgressChanged += this.Worker_ProgressChanged;
			this.worker.RunWorkerCompleted += this.Worker_RunWorkerCompleted;
			this.worker.WorkerSupportsCancellation = true;
		}

		// Token: 0x060005C5 RID: 1477 RVA: 0x00027B59 File Offset: 0x00025D59
		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.progressBarWorker.Visibility = Visibility.Hidden;
			if (!string.IsNullOrEmpty(this.LastZipPath) && File.Exists(this.LastZipPath))
			{
				Process.Start(this.LastZipPath);
			}
		}

		// Token: 0x060005C6 RID: 1478 RVA: 0x00027B8D File Offset: 0x00025D8D
		private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			this.progressBarWorker.Value = (double)e.ProgressPercentage;
		}

		// Token: 0x060005C7 RID: 1479 RVA: 0x00027BA4 File Offset: 0x00025DA4
		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			string folderRaw = System.IO.Path.Combine(this.DeclaredOperationalEvent.DirectoryPath, "Captured");
			string folderResult = System.IO.Path.Combine(this.DeclaredOperationalEvent.DirectoryPath, "Render");
			string folderAllImages = System.IO.Path.Combine(this.DeclaredOperationalEvent.DirectoryPath, "AIImages");
			string folderWordPortre = System.IO.Path.Combine(this.DeclaredOperationalEvent.DirectoryPath, "WordCloud");
			string zipPath = System.IO.Path.Combine(this.DeclaredOperationalEvent.DirectoryPath, "Temp", "tempArchive.zip");
			(sender as BackgroundWorker).ReportProgress(5);
			using (FileStream zipToOpen = new FileStream(zipPath, FileMode.Create))
			{
				using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
				{
					ExtensionMethod.AddFolderToZip(archive, folderRaw, "Raw");
					(sender as BackgroundWorker).ReportProgress(45);
					ExtensionMethod.AddFolderToZip(archive, folderResult, "Result");
					(sender as BackgroundWorker).ReportProgress(65);
					if (Directory.Exists(folderAllImages))
					{
						ExtensionMethod.AddFolderToZip(archive, folderAllImages, "AI");
					}
					if (Directory.Exists(folderWordPortre))
					{
						ExtensionMethod.AddFolderToZip(archive, folderWordPortre, "WordPortrait");
					}
					(sender as BackgroundWorker).ReportProgress(85);
				}
			}
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Zip files (*.zip)|*.zip";
			saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			if (saveFileDialog.ShowDialog().GetValueOrDefault())
			{
				(sender as BackgroundWorker).ReportProgress(50);
				if (File.Exists(saveFileDialog.FileName))
				{
					File.Delete(saveFileDialog.FileName);
				}
				File.Move(zipPath, saveFileDialog.FileName);
				(sender as BackgroundWorker).ReportProgress(100);
			}
		}

		// Token: 0x060005C8 RID: 1480 RVA: 0x00027D5C File Offset: 0x00025F5C
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Start();
			if (EventManagementPage.GetCurrentEvent() != null)
			{
				this.SetOperationalEvent(EventManagementPage.GetCurrentEvent());
			}
			else if (this.declaredOperationalEvent != null)
			{
				this.SetOperationalEvent(this.declaredOperationalEvent);
				this.declaredOperationalEvent = null;
			}
			bool qrEnabled = this.declaredOperationalEvent != null && !string.IsNullOrEmpty(this.DeclaredOperationalEvent.eventHash);
			if (qrEnabled)
			{
				this.eventQR.Source = ExtensionMethod.GenerateQr("https://activationshare.com/Content/EventMedias/" + this.declaredOperationalEvent.IndexID);
			}
			this.eventQR.Visibility = (qrEnabled ? Visibility.Visible : Visibility.Hidden);
			this.btnEditEvent.Background = new SolidColorBrush(AppInfo.ThemeColor);
		}

		// Token: 0x060005C9 RID: 1481 RVA: 0x00027E0C File Offset: 0x0002600C
		private void archiveMedia_Click(object sender, RoutedEventArgs e)
		{
			if (!this.worker.IsBusy)
			{
				this.worker.RunWorkerAsync();
				this.progressBarWorker.Visibility = Visibility.Visible;
				return;
			}
			this.worker.CancelAsync();
			this.progressBarWorker.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060005CA RID: 1482 RVA: 0x00027E4A File Offset: 0x0002604A
		private void btnToMedias_Click(object sender, RoutedEventArgs e)
		{
			ExtensionMethod.OpenUrl("https://activationshare.com/Content/EventMedias/" + this.declaredOperationalEvent.IndexID);
		}

		// Token: 0x040005CD RID: 1485
		private BackgroundWorker worker = new BackgroundWorker();

		// Token: 0x040005CE RID: 1486
		private OperationalEvent declaredOperationalEvent;

		// Token: 0x040005CF RID: 1487
		private EventManagementPage eventManagement;

		// Token: 0x040005D0 RID: 1488
		public string LastZipPath;

		// Token: 0x040005D1 RID: 1489
		public static CurrentEventPage currentEventUIScript;
	}
}
