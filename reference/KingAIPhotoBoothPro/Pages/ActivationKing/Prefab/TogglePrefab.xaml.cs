using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x0200005D RID: 93
	public partial class TogglePrefab : Page
	{
		// Token: 0x170000B0 RID: 176
		// (get) Token: 0x06000744 RID: 1860 RVA: 0x0003153E File Offset: 0x0002F73E
		// (set) Token: 0x06000745 RID: 1861 RVA: 0x00031546 File Offset: 0x0002F746
		public string Key { get; private set; }

		// Token: 0x170000B1 RID: 177
		// (get) Token: 0x06000746 RID: 1862 RVA: 0x0003154F File Offset: 0x0002F74F
		// (set) Token: 0x06000747 RID: 1863 RVA: 0x00031557 File Offset: 0x0002F757
		public bool DefaultStatus { get; private set; }

		// Token: 0x170000B2 RID: 178
		// (get) Token: 0x06000748 RID: 1864 RVA: 0x00031560 File Offset: 0x0002F760
		// (set) Token: 0x06000749 RID: 1865 RVA: 0x00031568 File Offset: 0x0002F768
		public string txtTitle { get; private set; }

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x0600074A RID: 1866 RVA: 0x00031571 File Offset: 0x0002F771
		// (set) Token: 0x0600074B RID: 1867 RVA: 0x00031579 File Offset: 0x0002F779
		public bool IsHorizontalCenter { get; private set; }

		// Token: 0x0600074C RID: 1868 RVA: 0x00031582 File Offset: 0x0002F782
		public TogglePrefab(string key, string title, bool defaultStatus = false, bool isHorizontalCenter = false)
		{
			this.Key = key;
			this.txtTitle = title;
			this.DefaultStatus = defaultStatus;
			this.IsHorizontalCenter = isHorizontalCenter;
			this.InitializeComponent();
		}

		// Token: 0x0600074D RID: 1869 RVA: 0x000315AD File Offset: 0x0002F7AD
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x0600074E RID: 1870 RVA: 0x000315B5 File Offset: 0x0002F7B5
		public void SetValue(bool value)
		{
			this.toggleElement.IsChecked = new bool?(value);
			Settings.SetValue(this.Key, value);
		}

		// Token: 0x0600074F RID: 1871 RVA: 0x000315D4 File Offset: 0x0002F7D4
		public void Load()
		{
			this.toggleElement.Content = this.txtTitle;
			if (Settings.GetValueBoolean(this.Key) == null)
			{
				this.toggleElement.IsChecked = new bool?(this.DefaultStatus);
				Settings.SetValue(this.Key, this.DefaultStatus);
			}
			else
			{
				this.toggleElement.IsChecked = new bool?(Settings.GetValueBoolean(this.Key).Value);
			}
			if (this.IsHorizontalCenter)
			{
				this.toggleElement.HorizontalAlignment = HorizontalAlignment.Center;
			}
		}

		// Token: 0x06000750 RID: 1872 RVA: 0x00031667 File Offset: 0x0002F867
		private void ToggleElement_Checked(object sender, RoutedEventArgs e)
		{
			Settings.SetValue(this.Key, "true", true);
		}

		// Token: 0x06000751 RID: 1873 RVA: 0x0003167A File Offset: 0x0002F87A
		private void ToggleElement_Unchecked(object sender, RoutedEventArgs e)
		{
			this.toggleElement.Foreground = Brushes.Gray;
			Settings.SetValue(this.Key, "false", true);
		}
	}
}
