using System;
using System.Windows;
using System.Windows.Controls;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A7 RID: 167
	public class SectionTemplate : ContentControl
	{
		// Token: 0x17000166 RID: 358
		// (get) Token: 0x06000997 RID: 2455 RVA: 0x000364DB File Offset: 0x000346DB
		// (set) Token: 0x06000998 RID: 2456 RVA: 0x000364ED File Offset: 0x000346ED
		public bool IsTopBorderVisible
		{
			get
			{
				return (bool)base.GetValue(SectionTemplate.IsTopBorderVisibleProperty);
			}
			set
			{
				base.SetValue(SectionTemplate.IsTopBorderVisibleProperty, value);
			}
		}

		// Token: 0x0400096D RID: 2413
		public static readonly DependencyProperty IsTopBorderVisibleProperty = DependencyProperty.Register("IsTopBorderVisible", typeof(bool), typeof(SectionTemplate), new PropertyMetadata(true));
	}
}
