using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000022 RID: 34
	public class HomeButton : Button
	{
		// Token: 0x17000052 RID: 82
		// (get) Token: 0x0600018F RID: 399 RVA: 0x00008916 File Offset: 0x00006B16
		// (set) Token: 0x06000190 RID: 400 RVA: 0x00008928 File Offset: 0x00006B28
		public ImageSource ButtonImageSource
		{
			get
			{
				return (ImageSource)base.GetValue(HomeButton.ButtonImageSourceProperty);
			}
			set
			{
				base.SetValue(HomeButton.ButtonImageSourceProperty, value);
			}
		}

		// Token: 0x04000118 RID: 280
		public static readonly DependencyProperty ButtonImageSourceProperty = DependencyProperty.Register("ButtonImageSource", typeof(ImageSource), typeof(HomeButton), new PropertyMetadata(null));
	}
}
