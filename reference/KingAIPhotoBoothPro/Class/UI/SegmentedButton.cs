using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace KingAIPhotoBoothPro.Class.UI
{
	// Token: 0x020000C0 RID: 192
	public class SegmentedButton : ToggleButton
	{
		// Token: 0x06000A79 RID: 2681 RVA: 0x0003C858 File Offset: 0x0003AA58
		static SegmentedButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(SegmentedButton), new FrameworkPropertyMetadata(typeof(SegmentedButton)));
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06000A7A RID: 2682 RVA: 0x0003C8B5 File Offset: 0x0003AAB5
		// (set) Token: 0x06000A7B RID: 2683 RVA: 0x0003C8C7 File Offset: 0x0003AAC7
		public string LeftText
		{
			get
			{
				return (string)base.GetValue(SegmentedButton.LeftTextProperty);
			}
			set
			{
				base.SetValue(SegmentedButton.LeftTextProperty, value);
			}
		}

		// Token: 0x04000A2B RID: 2603
		public static readonly DependencyProperty LeftTextProperty = DependencyProperty.Register("LeftText", typeof(string), typeof(SegmentedButton), new PropertyMetadata(string.Empty));
	}
}
