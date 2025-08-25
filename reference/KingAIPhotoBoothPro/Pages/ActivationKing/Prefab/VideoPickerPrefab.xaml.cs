using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000058 RID: 88
	public partial class VideoPickerPrefab : Page
	{
		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060006EF RID: 1775 RVA: 0x0002FE71 File Offset: 0x0002E071
		// (set) Token: 0x060006F0 RID: 1776 RVA: 0x0002FE79 File Offset: 0x0002E079
		public string Key { get; private set; }

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x060006F1 RID: 1777 RVA: 0x0002FE82 File Offset: 0x0002E082
		// (set) Token: 0x060006F2 RID: 1778 RVA: 0x0002FE8A File Offset: 0x0002E08A
		public string DefaultStatus { get; private set; }

		// Token: 0x170000A2 RID: 162
		// (get) Token: 0x060006F3 RID: 1779 RVA: 0x0002FE93 File Offset: 0x0002E093
		// (set) Token: 0x060006F4 RID: 1780 RVA: 0x0002FE9B File Offset: 0x0002E09B
		public string txtTitle { get; private set; }

		// Token: 0x060006F5 RID: 1781 RVA: 0x0002FEA4 File Offset: 0x0002E0A4
		public VideoPickerPrefab(string key, string title, string defaultStatus = "")
		{
			this.Key = key;
			this.DefaultStatus = defaultStatus;
			this.txtTitle = title;
			this.InitializeComponent();
		}

		// Token: 0x060006F6 RID: 1782 RVA: 0x0002FEC8 File Offset: 0x0002E0C8
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "Video Files (*.wmv, *.avi, *.mp4)|*.wmv;*.avi;*.mp4";
				DialogResult result = dialog.ShowDialog();
				if (result != DialogResult.OK)
				{
					return;
				}
				this.path = dialog.FileName;
			}
			Settings.SetValue(this.Key, this.path, true);
			this.LoadImage();
		}

		// Token: 0x060006F7 RID: 1783 RVA: 0x0002FF34 File Offset: 0x0002E134
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060006F8 RID: 1784 RVA: 0x0002FF3C File Offset: 0x0002E13C
		private void LoadImage()
		{
			try
			{
				this.prefabImage.Source = ExtensionMethod.GetVideoThumbnail(this.path);
				this.txtFileName.Foreground = Brushes.Black;
				this.txtFileName.Text = System.IO.Path.GetFileName(this.path);
				this.UploadButtonGrid.Visibility = Visibility.Hidden;
			}
			catch (Exception)
			{
				this.txtFileName.Foreground = Brushes.Red;
				this.txtFileName.Text = "File Not Found!";
				this.UploadButtonGrid.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060006F9 RID: 1785 RVA: 0x0002FFD4 File Offset: 0x0002E1D4
		public void Load()
		{
			if (Settings.GetValueString(this.Key) == null)
			{
				this.path = this.DefaultStatus;
				Settings.SetValue(this.Key, this.DefaultStatus, true);
			}
			else
			{
				this.path = Settings.GetValueString(this.Key);
				this.LoadImage();
			}
			this.ReturnDefaultButtonGrid.Visibility = ((this.UploadButtonGrid.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible);
		}

		// Token: 0x060006FA RID: 1786 RVA: 0x00030041 File Offset: 0x0002E241
		private void ReturnDefaultButton_Click(object sender, RoutedEventArgs e)
		{
			this.path = Settings.GetSampleVideoDefaultValue(this.Key);
			Settings.SetValue(this.Key, this.path, true);
			this.Load();
		}

		// Token: 0x04000795 RID: 1941
		private string path;
	}
}
