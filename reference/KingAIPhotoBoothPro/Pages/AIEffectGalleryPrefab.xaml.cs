using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000025 RID: 37
	public partial class AIEffectGalleryPrefab : Page
	{
		// Token: 0x14000009 RID: 9
		// (add) Token: 0x060001A7 RID: 423 RVA: 0x00008E54 File Offset: 0x00007054
		// (remove) Token: 0x060001A8 RID: 424 RVA: 0x00008E8C File Offset: 0x0000708C
		public event Action<string> ClickEvent;

		// Token: 0x060001A9 RID: 425 RVA: 0x00008EC1 File Offset: 0x000070C1
		public AIEffectGalleryPrefab()
		{
			this.InitializeComponent();
		}

		// Token: 0x060001AA RID: 426 RVA: 0x00008ED0 File Offset: 0x000070D0
		public void LoadImage(AiEffectLocal aIEffectLocal)
		{
			this.thisAIEffect = aIEffectLocal;
			BitmapImage bitmap = new BitmapImage();
			bitmap.BeginInit();
			bitmap.UriSource = new Uri(this.thisAIEffect.CoverImageLocalPath, UriKind.Absolute);
			bitmap.DecodePixelHeight = 800;
			bitmap.EndInit();
			this.AIEffectImage.Source = bitmap;
		}

		// Token: 0x060001AB RID: 427 RVA: 0x00008F24 File Offset: 0x00007124
		private void AIEffectButton_Click(object sender, RoutedEventArgs e)
		{
			this.ClickEvent(this.thisAIEffect.Id);
		}

		// Token: 0x04000132 RID: 306
		public AiEffectLocal thisAIEffect;
	}
}
