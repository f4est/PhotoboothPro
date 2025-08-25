using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000012 RID: 18
	public partial class UpdateWindow : Window
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600007D RID: 125 RVA: 0x0000501F File Offset: 0x0000321F
		private string SystemDownloadsFolder
		{
			get
			{
				return System.Windows.Forms.Application.StartupPath;
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600007E RID: 126 RVA: 0x00005026 File Offset: 0x00003226
		private string downloadPath
		{
			get
			{
				return Path.Combine(this.SystemDownloadsFolder, "king" + AppVersionControl.NewVersionName + ".exe");
			}
		}

		// Token: 0x0600007F RID: 127 RVA: 0x00005048 File Offset: 0x00003248
		public UpdateWindow(bool isTestVersion)
		{
			this.downloadTestVersion = isTestVersion;
			this.InitializeComponent();
			this.ManageSize();
			this.wc = new WebClient();
			if (!Directory.Exists(this.SystemDownloadsFolder))
			{
				Directory.CreateDirectory(this.SystemDownloadsFolder);
			}
			this.wc.DownloadProgressChanged += this.Wc_DownloadProgressChanged;
			this.wc.DownloadFileCompleted += this.Wc_DownloadFileCompleted;
		}

		// Token: 0x06000080 RID: 128 RVA: 0x000050C0 File Offset: 0x000032C0
		private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				MessageBoxWindow.CreateWindow("Download Error", "You can download new version from activationking.com website", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			else
			{
				this.progressForDownload.Value = 100.0;
				this.UILoadingPercent.Content = "%100";
				if (File.Exists(this.downloadPath))
				{
					Process.Start(this.downloadPath);
				}
			}
			System.Windows.Application.Current.Shutdown();
			Environment.Exit(0);
		}

		// Token: 0x06000081 RID: 129 RVA: 0x00005144 File Offset: 0x00003344
		private void ManageSize()
		{
			double Width = ActivationKingWindow.Instance.ActualWidth * 0.3125;
			double Height = ActivationKingWindow.Instance.ActualHeight * 0.37037035822868347;
			base.Width = Width;
			base.Height = Height;
			base.Left = (ActivationKingWindow.Instance.ActualWidth - Width) / 2.0;
			base.Top = (ActivationKingWindow.Instance.ActualHeight - Height) / 2.0;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000051C4 File Offset: 0x000033C4
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			string downloadUrl = AppInfo.AppClass.DownloadLinkLatestVersion;
			if (SessionData.accountInfo != null && SessionData.accountInfo.isTesterUser && this.downloadTestVersion)
			{
				downloadUrl = AppInfo.AppClass.DownloadLinkLatestTesterVersion;
			}
			this.wc.DownloadFileAsync(new Uri(downloadUrl), this.downloadPath);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000521C File Offset: 0x0000341C
		private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			this.progressForDownload.Value = (double)Math.Min(e.ProgressPercentage, 100);
			this.UILoadingPercent.Content = "%" + Math.Min(e.ProgressPercentage, 100).ToString();
			string text = (e.BytesReceived > 1048576L) ? (e.BytesReceived / 1048576L).ToString() : (e.BytesReceived / 1048576L).ToString();
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000052A7 File Offset: 0x000034A7
		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			this.wc.DownloadProgressChanged -= this.Wc_DownloadProgressChanged;
			this.wc.DownloadProgressChanged -= this.Wc_DownloadProgressChanged;
		}

		// Token: 0x0400006D RID: 109
		private WebClient wc;

		// Token: 0x0400006E RID: 110
		private bool downloadTestVersion;
	}
}
