using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000023 RID: 35
	public partial class AIBackgroundPrefab : Page
	{
		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000193 RID: 403 RVA: 0x0000896C File Offset: 0x00006B6C
		// (remove) Token: 0x06000194 RID: 404 RVA: 0x000089A4 File Offset: 0x00006BA4
		public event Action<FaceSwapTarget> ClickEvent;

		// Token: 0x06000195 RID: 405 RVA: 0x000089D9 File Offset: 0x00006BD9
		public AIBackgroundPrefab()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000196 RID: 406 RVA: 0x000089E8 File Offset: 0x00006BE8
		public void LoadImage(string Path, FaceSwapTarget aIBackground)
		{
			this.thisAIBackground = aIBackground;
			this.bitmap = new BitmapImage();
			this.bitmap.CacheOption = BitmapCacheOption.Default;
			this.bitmap.BeginInit();
			this.bitmap.UriSource = new Uri(Path, UriKind.Absolute);
			this.bitmap.DecodePixelHeight = 800;
			this.bitmap.EndInit();
			this.AIBackgroundImage.Source = this.bitmap;
		}

		// Token: 0x06000197 RID: 407 RVA: 0x00008A5C File Offset: 0x00006C5C
		private void AIBackgroundButton_Click(object sender, RoutedEventArgs e)
		{
			this.ClickEvent(this.thisAIBackground);
		}

		// Token: 0x06000198 RID: 408 RVA: 0x00008A6F File Offset: 0x00006C6F
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			new Thread(delegate()
			{
				this.bitmap = null;
				GC.WaitForPendingFinalizers();
				GC.Collect();
			});
		}

		// Token: 0x04000119 RID: 281
		public FaceSwapTarget thisAIBackground;

		// Token: 0x0400011B RID: 283
		private BitmapImage bitmap;
	}
}
