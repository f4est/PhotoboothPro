using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000050 RID: 80
	public partial class ThemePage : SettingsSubPage
	{
		// Token: 0x06000670 RID: 1648 RVA: 0x0002E1F0 File Offset: 0x0002C3F0
		public ThemePage()
		{
			this.InitializeComponent();
			this.colorPickerMainPrefab = new ColorPickerPrefab("colorbackgroundmaintheme", "Theme Color", "#00000000");
			this.colorPickerPrefab = new ColorPickerPrefab("colorbackgroundtheme", "Home Page Background Color", "#00000000");
			this.backgroundImagePickerPrefab = new ImagePickerPrefab("mainbackgroundimage", "", "");
			this.backgroundVideoPickerPrefab = new VideoPickerPrefab("mainbackgroundvideo", "", "");
			this.waitVideoPickerPrefab = new VideoPickerPrefab("waitvideo", "", "");
			this.processWaitVideoPickerPrefab = new VideoPickerPrefab("processwaitvideo", "", "");
			this.resultVideoPickerPrefab = new VideoPickerPrefab("resultvideo", "", "");
			this.backgroundImageToggle = new TogglePrefab("mainbackgroundimagecheck", "Home Background Image", false, false);
			this.backgroundVideoToggle = new TogglePrefab("mainbackgroundvideocheck", "Home Page Video", false, false);
			this.waitVideoToggle = new TogglePrefab("waitvideocheck", "Countdown Page Video", false, false);
			this.processWaitVideoToggle = new TogglePrefab("processwaitvideocheck", "Processing Waiting Video", false, false);
			this.resultVideoToggle = new TogglePrefab("resultvideocheck", "Sharing Page Video", false, false);
			this.colorPrefab2.Content = this.colorPickerMainPrefab;
			this.colorPrefab.Content = this.colorPickerPrefab;
			this.mainBackgroundFrame.Content = this.backgroundImagePickerPrefab;
			this.mainVideoFrame.Content = this.backgroundVideoPickerPrefab;
			this.mainVideoCheckFrame.Content = this.backgroundVideoToggle;
			this.mainBackgroundImageCheckFrame.Content = this.backgroundImageToggle;
			this.WaitTimeVideoFrame.Content = this.waitVideoPickerPrefab;
			this.WaitTimeVideoCheckFrame.Content = this.waitVideoToggle;
			this.WaitProcessVideoFrame.Content = this.processWaitVideoPickerPrefab;
			this.WaitProcessVideoCheckFrame.Content = this.processWaitVideoToggle;
			this.FinishVideoFrame.Content = this.resultVideoPickerPrefab;
			this.FinishVideoCheckFrame.Content = this.resultVideoToggle;
		}

		// Token: 0x06000671 RID: 1649 RVA: 0x0002E3FE File Offset: 0x0002C5FE
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000672 RID: 1650 RVA: 0x0002E400 File Offset: 0x0002C600
		public override string GetTitle()
		{
			return "Screen Theme Settings";
		}

		// Token: 0x06000673 RID: 1651 RVA: 0x0002E407 File Offset: 0x0002C607
		public override string GetSubTitle()
		{
			return "You can customize the theme colors, background images and videos";
		}

		// Token: 0x0400071A RID: 1818
		private TogglePrefab backgroundImageToggle;

		// Token: 0x0400071B RID: 1819
		private TogglePrefab backgroundVideoToggle;

		// Token: 0x0400071C RID: 1820
		private TogglePrefab waitVideoToggle;

		// Token: 0x0400071D RID: 1821
		private TogglePrefab processWaitVideoToggle;

		// Token: 0x0400071E RID: 1822
		private TogglePrefab resultVideoToggle;

		// Token: 0x0400071F RID: 1823
		private ColorPickerPrefab colorPickerPrefab;

		// Token: 0x04000720 RID: 1824
		private ColorPickerPrefab colorPickerMainPrefab;

		// Token: 0x04000721 RID: 1825
		private ImagePickerPrefab backgroundImagePickerPrefab;

		// Token: 0x04000722 RID: 1826
		private VideoPickerPrefab backgroundVideoPickerPrefab;

		// Token: 0x04000723 RID: 1827
		private VideoPickerPrefab waitVideoPickerPrefab;

		// Token: 0x04000724 RID: 1828
		private VideoPickerPrefab processWaitVideoPickerPrefab;

		// Token: 0x04000725 RID: 1829
		private VideoPickerPrefab resultVideoPickerPrefab;
	}
}
