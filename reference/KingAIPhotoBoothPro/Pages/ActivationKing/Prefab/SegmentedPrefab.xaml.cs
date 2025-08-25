using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Class.UI;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000056 RID: 86
	public partial class SegmentedPrefab : Page
	{
		// Token: 0x1700009B RID: 155
		// (get) Token: 0x060006D9 RID: 1753 RVA: 0x0002FB63 File Offset: 0x0002DD63
		// (set) Token: 0x060006DA RID: 1754 RVA: 0x0002FB6B File Offset: 0x0002DD6B
		public string Key { get; private set; }

		// Token: 0x1700009C RID: 156
		// (get) Token: 0x060006DB RID: 1755 RVA: 0x0002FB74 File Offset: 0x0002DD74
		// (set) Token: 0x060006DC RID: 1756 RVA: 0x0002FB7C File Offset: 0x0002DD7C
		public bool DefaultStatus { get; private set; }

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x060006DD RID: 1757 RVA: 0x0002FB85 File Offset: 0x0002DD85
		// (set) Token: 0x060006DE RID: 1758 RVA: 0x0002FB8D File Offset: 0x0002DD8D
		public string RightText { get; private set; }

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x060006DF RID: 1759 RVA: 0x0002FB96 File Offset: 0x0002DD96
		// (set) Token: 0x060006E0 RID: 1760 RVA: 0x0002FBA8 File Offset: 0x0002DDA8
		public string LeftText
		{
			get
			{
				return (string)base.GetValue(SegmentedPrefab.LeftTextProperty);
			}
			set
			{
				base.SetValue(SegmentedPrefab.LeftTextProperty, value);
			}
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x060006E1 RID: 1761 RVA: 0x0002FBB6 File Offset: 0x0002DDB6
		// (set) Token: 0x060006E2 RID: 1762 RVA: 0x0002FBBE File Offset: 0x0002DDBE
		public bool IsHorizontalCenter { get; private set; }

		// Token: 0x060006E3 RID: 1763 RVA: 0x0002FBC8 File Offset: 0x0002DDC8
		public SegmentedPrefab(string key, string leftText, string rightText, bool defaultStatus = false, bool isHorizontalCenter = false, string tooltip = "")
		{
			this.Key = key;
			this.LeftText = leftText;
			this.RightText = rightText;
			this.DefaultStatus = defaultStatus;
			this.IsHorizontalCenter = isHorizontalCenter;
			if (!string.IsNullOrEmpty(tooltip))
			{
				base.ToolTip = tooltip;
			}
			this.InitializeComponent();
		}

		// Token: 0x060006E4 RID: 1764 RVA: 0x0002FC17 File Offset: 0x0002DE17
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060006E5 RID: 1765 RVA: 0x0002FC20 File Offset: 0x0002DE20
		public void Load()
		{
			this.toggleElement.Content = this.RightText;
			this.toggleElement.LeftText = this.LeftText;
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
			this.toggleElement.ToolTip = base.ToolTip;
		}

		// Token: 0x060006E6 RID: 1766 RVA: 0x0002FCD5 File Offset: 0x0002DED5
		private void ToggleElement_Checked(object sender, RoutedEventArgs e)
		{
			Settings.SetValue(this.Key, "true", true);
		}

		// Token: 0x060006E7 RID: 1767 RVA: 0x0002FCE8 File Offset: 0x0002DEE8
		private void ToggleElement_Unchecked(object sender, RoutedEventArgs e)
		{
			Settings.SetValue(this.Key, "false", true);
		}

		// Token: 0x04000787 RID: 1927
		public static readonly DependencyProperty LeftTextProperty = DependencyProperty.Register("LeftText", typeof(string), typeof(SegmentedPrefab), new PropertyMetadata(string.Empty));
	}
}
