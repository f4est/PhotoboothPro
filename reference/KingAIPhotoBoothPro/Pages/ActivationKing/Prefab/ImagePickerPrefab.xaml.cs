using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000063 RID: 99
	public partial class ImagePickerPrefab : Page
	{
		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060007A3 RID: 1955 RVA: 0x000325D2 File Offset: 0x000307D2
		// (set) Token: 0x060007A4 RID: 1956 RVA: 0x000325DA File Offset: 0x000307DA
		public string Key { get; private set; }

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060007A5 RID: 1957 RVA: 0x000325E3 File Offset: 0x000307E3
		// (set) Token: 0x060007A6 RID: 1958 RVA: 0x000325EB File Offset: 0x000307EB
		public string DefaultStatus { get; private set; }

		// Token: 0x170000C6 RID: 198
		// (get) Token: 0x060007A7 RID: 1959 RVA: 0x000325F4 File Offset: 0x000307F4
		// (set) Token: 0x060007A8 RID: 1960 RVA: 0x000325FC File Offset: 0x000307FC
		public string txtTitle { get; private set; }

		// Token: 0x060007A9 RID: 1961 RVA: 0x00032605 File Offset: 0x00030805
		public ImagePickerPrefab(string key, string title, string defaultStatus = "")
		{
			this.Key = key;
			this.DefaultStatus = defaultStatus;
			this.txtTitle = title;
			this.InitializeComponent();
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00032628 File Offset: 0x00030828
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "PNG (*.png)|*.png|JPEG (*.jpeg)|*.jpeg|JPG (*.jpg)|*.jpg";
				DialogResult result = dialog.ShowDialog();
				if (result != DialogResult.OK)
				{
					return;
				}
				this.path = dialog.FileName;
			}
			byte[] data = File.ReadAllBytes(this.path);
			if (data.Length <= 10485760)
			{
				Settings.SetValue(this.Key, this.path, true);
				this.LoadImage();
				return;
			}
			new Thread(delegate()
			{
				Thread.Sleep(7000);
				base.Dispatcher.BeginInvoke(new Action(delegate()
				{
				}), DispatcherPriority.Normal, Array.Empty<object>());
			}).Start();
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x000326C0 File Offset: 0x000308C0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060007AC RID: 1964 RVA: 0x000326C8 File Offset: 0x000308C8
		private void LoadImage()
		{
			Uri fileLocalation = new Uri(System.IO.Path.Combine("file://", this.path));
			try
			{
				this.prefabImage.Source = new BitmapImage(fileLocalation);
				this.txtFileName.Foreground = Brushes.Black;
				this.txtFileName.Text = System.IO.Path.GetFileName(this.path);
				this.UploadButtonGrid.Visibility = Visibility.Hidden;
			}
			catch (Exception)
			{
				this.txtFileName.Foreground = Brushes.Red;
				this.txtFileName.Text = "File Not Found! (Max 10 MB)";
				this.UploadButtonGrid.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060007AD RID: 1965 RVA: 0x00032770 File Offset: 0x00030970
		public void Load()
		{
			if (Settings.GetValueString(this.Key) == null)
			{
				this.path = this.DefaultStatus;
				Settings.SetValue(this.Key, this.DefaultStatus, true);
				return;
			}
			this.path = Settings.GetValueString(this.Key);
			this.LoadImage();
		}

		// Token: 0x04000827 RID: 2087
		private string path;
	}
}
