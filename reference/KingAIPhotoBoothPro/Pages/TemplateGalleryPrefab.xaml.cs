using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000032 RID: 50
	public partial class TemplateGalleryPrefab : Page
	{
		// Token: 0x1400000C RID: 12
		// (add) Token: 0x060002D0 RID: 720 RVA: 0x0000E928 File Offset: 0x0000CB28
		// (remove) Token: 0x060002D1 RID: 721 RVA: 0x0000E960 File Offset: 0x0000CB60
		public event Action<int> ClickEvent;

		// Token: 0x060002D2 RID: 722 RVA: 0x0000E995 File Offset: 0x0000CB95
		public TemplateGalleryPrefab()
		{
			this.InitializeComponent();
		}

		// Token: 0x060002D3 RID: 723 RVA: 0x0000E9A4 File Offset: 0x0000CBA4
		public void LoadImage(string Path, int ID)
		{
			this.TemplateGalleryID = ID;
			using (FileStream fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				BitmapImage bitmap = new BitmapImage();
				bitmap.BeginInit();
				bitmap.CacheOption = BitmapCacheOption.OnLoad;
				bitmap.StreamSource = fileStream;
				bitmap.DecodePixelHeight = 800;
				bitmap.EndInit();
				this.AIBackgroundImage.Source = bitmap;
			}
		}

		// Token: 0x060002D4 RID: 724 RVA: 0x0000EA18 File Offset: 0x0000CC18
		private void AIBackgroundButton_Click(object sender, RoutedEventArgs e)
		{
			this.ClickEvent(this.TemplateGalleryID);
		}

		// Token: 0x060002D5 RID: 725 RVA: 0x0000EA2B File Offset: 0x0000CC2B
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.bitmap = null;
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}

		// Token: 0x04000275 RID: 629
		public int TemplateGalleryID;

		// Token: 0x04000277 RID: 631
		private BitmapImage bitmap;
	}
}
