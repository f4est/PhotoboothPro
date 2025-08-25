using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200002F RID: 47
	public partial class SamplePromptPrefab : Page
	{
		// Token: 0x1400000B RID: 11
		// (add) Token: 0x060002B4 RID: 692 RVA: 0x0000E168 File Offset: 0x0000C368
		// (remove) Token: 0x060002B5 RID: 693 RVA: 0x0000E1A0 File Offset: 0x0000C3A0
		public event Action<SamplePrompt> ClickEvent;

		// Token: 0x060002B6 RID: 694 RVA: 0x0000E1D5 File Offset: 0x0000C3D5
		public SamplePromptPrefab()
		{
			this.InitializeComponent();
		}

		// Token: 0x060002B7 RID: 695 RVA: 0x0000E1E4 File Offset: 0x0000C3E4
		public void LoadImage(string Path, SamplePrompt samplePrompt)
		{
			this.thisSamplePrompt = samplePrompt;
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(Path, UriKind.Absolute);
			bitmap.DecodePixelHeight = 800;
			bitmap.EndInit();
			this.SamplePromptImage.Source = bitmap;
		}

		// Token: 0x060002B8 RID: 696 RVA: 0x0000E22E File Offset: 0x0000C42E
		private void SamplePromptButton_Click(object sender, RoutedEventArgs e)
		{
			this.ClickEvent(this.thisSamplePrompt);
		}

		// Token: 0x04000258 RID: 600
		public SamplePrompt thisSamplePrompt;
	}
}
