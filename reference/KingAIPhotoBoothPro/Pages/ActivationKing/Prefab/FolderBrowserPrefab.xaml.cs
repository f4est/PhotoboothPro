using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000062 RID: 98
	public partial class FolderBrowserPrefab : Page
	{
		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x06000795 RID: 1941 RVA: 0x00032383 File Offset: 0x00030583
		// (set) Token: 0x06000796 RID: 1942 RVA: 0x0003238B File Offset: 0x0003058B
		public string Key { get; private set; }

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x06000797 RID: 1943 RVA: 0x00032394 File Offset: 0x00030594
		// (set) Token: 0x06000798 RID: 1944 RVA: 0x0003239C File Offset: 0x0003059C
		public string DefaultStatus { get; private set; }

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x06000799 RID: 1945 RVA: 0x000323A5 File Offset: 0x000305A5
		// (set) Token: 0x0600079A RID: 1946 RVA: 0x000323AD File Offset: 0x000305AD
		public string txtTitle { get; private set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x0600079B RID: 1947 RVA: 0x000323B6 File Offset: 0x000305B6
		// (set) Token: 0x0600079C RID: 1948 RVA: 0x000323BE File Offset: 0x000305BE
		public string txtDescription { get; private set; }

		// Token: 0x0600079D RID: 1949 RVA: 0x000323C7 File Offset: 0x000305C7
		public FolderBrowserPrefab(string key, string title, string defaultStatus = "", string description = "")
		{
			this.Key = key;
			this.DefaultStatus = defaultStatus;
			this.txtTitle = title;
			this.txtDescription = description;
			this.InitializeComponent();
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x000323F2 File Offset: 0x000305F2
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x000323FC File Offset: 0x000305FC
		private void GetFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				DialogResult result = dialog.ShowDialog();
				this.txtCapturedFolder.Content = dialog.SelectedPath;
			}
			Settings.SetValue(this.Key, this.txtCapturedFolder.Content.ToString(), true);
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00032460 File Offset: 0x00030660
		public void Load()
		{
			this.BrowseButtonBrush.ImageSource = BmsEngine.ChangeColorImage(AppInfo.ThemeColor, (BitmapSource)this.BrowseButtonBrush.ImageSource);
			this.txtFile.Content = this.txtTitle;
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
