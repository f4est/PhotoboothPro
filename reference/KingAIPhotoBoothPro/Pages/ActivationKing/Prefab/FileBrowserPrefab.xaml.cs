using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000053 RID: 83
	public partial class FileBrowserPrefab : Page
	{
		// Token: 0x17000090 RID: 144
		// (get) Token: 0x060006A9 RID: 1705 RVA: 0x0002F21F File Offset: 0x0002D41F
		// (set) Token: 0x060006AA RID: 1706 RVA: 0x0002F227 File Offset: 0x0002D427
		public string Key { get; private set; }

		// Token: 0x17000091 RID: 145
		// (get) Token: 0x060006AB RID: 1707 RVA: 0x0002F230 File Offset: 0x0002D430
		// (set) Token: 0x060006AC RID: 1708 RVA: 0x0002F238 File Offset: 0x0002D438
		public string DefaultStatus { get; private set; }

		// Token: 0x17000092 RID: 146
		// (get) Token: 0x060006AD RID: 1709 RVA: 0x0002F241 File Offset: 0x0002D441
		// (set) Token: 0x060006AE RID: 1710 RVA: 0x0002F249 File Offset: 0x0002D449
		public string txtTitle { get; private set; }

		// Token: 0x17000093 RID: 147
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x0002F252 File Offset: 0x0002D452
		// (set) Token: 0x060006B0 RID: 1712 RVA: 0x0002F25A File Offset: 0x0002D45A
		public string txtDescription { get; private set; }

		// Token: 0x17000094 RID: 148
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x0002F263 File Offset: 0x0002D463
		// (set) Token: 0x060006B2 RID: 1714 RVA: 0x0002F26B File Offset: 0x0002D46B
		public string extensions { get; private set; }

		// Token: 0x060006B3 RID: 1715 RVA: 0x0002F274 File Offset: 0x0002D474
		public FileBrowserPrefab(string key, string title, string defaultStatus = "", string description = "", string extensions = "All Files (*.*)|*.*")
		{
			this.Key = key;
			this.DefaultStatus = defaultStatus;
			this.txtTitle = title;
			this.txtDescription = description;
			this.extensions = extensions;
			this.InitializeComponent();
		}

		// Token: 0x060006B4 RID: 1716 RVA: 0x0002F2A7 File Offset: 0x0002D4A7
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060006B5 RID: 1717 RVA: 0x0002F2B0 File Offset: 0x0002D4B0
		private void GetFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = this.extensions;
				DialogResult result = dialog.ShowDialog();
				this.txtCapturedFolder.Content = dialog.FileName;
			}
			Settings.SetValue(this.Key, this.txtCapturedFolder.Content.ToString(), true);
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x0002F320 File Offset: 0x0002D520
		public void Load()
		{
			this.txtFile.Text = this.txtTitle;
			this.txtDesc.Content = this.txtDescription;
			if (Settings.GetValueString(this.Key) == null)
			{
				this.txtCapturedFolder.Content = this.DefaultStatus;
				Settings.SetValue(this.Key, this.DefaultStatus, true);
				return;
			}
			this.txtCapturedFolder.Content = Settings.GetValueString(this.Key);
		}
	}
}
