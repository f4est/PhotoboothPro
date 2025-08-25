using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000013 RID: 19
	public partial class VideoWindow : Window
	{
		// Token: 0x06000087 RID: 135 RVA: 0x000053BF File Offset: 0x000035BF
		private void PlayUdpMpegtsStream()
		{
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000053C1 File Offset: 0x000035C1
		public VideoWindow()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000089 RID: 137 RVA: 0x000053CF File Offset: 0x000035CF
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.PlayUdpMpegtsStream();
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000053D7 File Offset: 0x000035D7
		private void PART_FirstThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000053D9 File Offset: 0x000035D9
		private void PART_SecondThumb_DragStarted(object sender, DragStartedEventArgs e)
		{
		}
	}
}
