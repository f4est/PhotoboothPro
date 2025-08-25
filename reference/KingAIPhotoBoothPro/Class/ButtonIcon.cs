using System;
using System.Windows;
using System.Windows.Controls;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000AD RID: 173
	public class ButtonIcon : Button
	{
		// Token: 0x17000169 RID: 361
		// (get) Token: 0x060009BF RID: 2495 RVA: 0x000388B8 File Offset: 0x00036AB8
		// (set) Token: 0x060009C0 RID: 2496 RVA: 0x000388C5 File Offset: 0x00036AC5
		public object Icon
		{
			get
			{
				return base.GetValue(ButtonIcon.IconProperty);
			}
			set
			{
				base.SetValue(ButtonIcon.IconProperty, value);
			}
		}

		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060009C1 RID: 2497 RVA: 0x000388D3 File Offset: 0x00036AD3
		// (set) Token: 0x060009C2 RID: 2498 RVA: 0x000388E5 File Offset: 0x00036AE5
		public double IconMargin
		{
			get
			{
				return (double)base.GetValue(ButtonIcon.IconMarginProperty);
			}
			set
			{
				base.SetValue(ButtonIcon.IconMarginProperty, value);
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060009C3 RID: 2499 RVA: 0x000388F8 File Offset: 0x00036AF8
		// (set) Token: 0x060009C4 RID: 2500 RVA: 0x0003890A File Offset: 0x00036B0A
		public Visibility BlackCoverVisibility
		{
			get
			{
				return (Visibility)base.GetValue(ButtonIcon.BlackCoverVisibilityProperty);
			}
			set
			{
				base.SetValue(ButtonIcon.BlackCoverVisibilityProperty, value);
			}
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060009C5 RID: 2501 RVA: 0x0003891D File Offset: 0x00036B1D
		// (set) Token: 0x060009C6 RID: 2502 RVA: 0x0003892F File Offset: 0x00036B2F
		public Visibility LoadingAnimationVisibility
		{
			get
			{
				return (Visibility)base.GetValue(ButtonIcon.LoadingAnimationVisibilityProperty);
			}
			set
			{
				base.SetValue(ButtonIcon.LoadingAnimationVisibilityProperty, value);
			}
		}

		// Token: 0x0400099B RID: 2459
		public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(object), typeof(ButtonIcon), new PropertyMetadata(null));

		// Token: 0x0400099C RID: 2460
		public static readonly DependencyProperty IconMarginProperty = DependencyProperty.Register("IconMargin", typeof(double), typeof(ButtonIcon), new PropertyMetadata(0.0));

		// Token: 0x0400099D RID: 2461
		public static readonly DependencyProperty BlackCoverVisibilityProperty = DependencyProperty.Register("BlackCoverVisibility", typeof(Visibility), typeof(ButtonIcon), new PropertyMetadata(Visibility.Collapsed));

		// Token: 0x0400099E RID: 2462
		public static readonly DependencyProperty LoadingAnimationVisibilityProperty = DependencyProperty.Register("LoadingAnimationVisibility", typeof(Visibility), typeof(ButtonIcon), new PropertyMetadata(Visibility.Collapsed));
	}
}
