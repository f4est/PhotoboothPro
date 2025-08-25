using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000019 RID: 25
	public class NavButton : ButtonBase
	{
		// Token: 0x060000F7 RID: 247 RVA: 0x00006FD0 File Offset: 0x000051D0
		static NavButton()
		{
			FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof(NavButton), new FrameworkPropertyMetadata(typeof(NavButton)));
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x060000F8 RID: 248 RVA: 0x0000707B File Offset: 0x0000527B
		// (set) Token: 0x060000F9 RID: 249 RVA: 0x0000708D File Offset: 0x0000528D
		public ImageSource ImageSource
		{
			get
			{
				return (ImageSource)base.GetValue(NavButton.ImageSourceProperty);
			}
			set
			{
				base.SetValue(NavButton.ImageSourceProperty, value);
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x060000FA RID: 250 RVA: 0x0000709B File Offset: 0x0000529B
		// (set) Token: 0x060000FB RID: 251 RVA: 0x000070AD File Offset: 0x000052AD
		public string Text
		{
			get
			{
				return (string)base.GetValue(NavButton.TextProperty);
			}
			set
			{
				base.SetValue(NavButton.TextProperty, value);
			}
		}

		// Token: 0x17000027 RID: 39
		// (get) Token: 0x060000FC RID: 252 RVA: 0x000070BB File Offset: 0x000052BB
		// (set) Token: 0x060000FD RID: 253 RVA: 0x000070CD File Offset: 0x000052CD
		public Uri NavUri
		{
			get
			{
				return (Uri)base.GetValue(NavButton.NavUriProperty);
			}
			set
			{
				base.SetValue(NavButton.NavUriProperty, value);
			}
		}

		// Token: 0x040000CA RID: 202
		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(NavButton), new PropertyMetadata(null));

		// Token: 0x040000CB RID: 203
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(NavButton), new PropertyMetadata(null));

		// Token: 0x040000CC RID: 204
		public static readonly DependencyProperty NavUriProperty = DependencyProperty.Register("NavUri", typeof(Uri), typeof(NavButton), new PropertyMetadata(null));
	}
}
