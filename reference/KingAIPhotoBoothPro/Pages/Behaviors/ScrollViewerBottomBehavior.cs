using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace KingAIPhotoBoothPro.Pages.Behaviors
{
	// Token: 0x02000065 RID: 101
	public class ScrollViewerBottomBehavior : Behavior<ScrollViewer>
	{
		// Token: 0x170000CB RID: 203
		// (get) Token: 0x060007C5 RID: 1989 RVA: 0x00032F5B File Offset: 0x0003115B
		// (set) Token: 0x060007C6 RID: 1990 RVA: 0x00032F6D File Offset: 0x0003116D
		public double Threshold
		{
			get
			{
				return (double)base.GetValue(ScrollViewerBottomBehavior.ThresholdProperty);
			}
			set
			{
				base.SetValue(ScrollViewerBottomBehavior.ThresholdProperty, value);
			}
		}

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x060007C7 RID: 1991 RVA: 0x00032F80 File Offset: 0x00031180
		// (remove) Token: 0x060007C8 RID: 1992 RVA: 0x00032FB8 File Offset: 0x000311B8
		public event RoutedEventHandler ReachedBottom;

		// Token: 0x060007C9 RID: 1993 RVA: 0x00032FED File Offset: 0x000311ED
		protected override void OnAttached()
		{
			base.OnAttached();
			base.AssociatedObject.ScrollChanged += this.ScrollViewer_ScrollChanged;
		}

		// Token: 0x060007CA RID: 1994 RVA: 0x0003300C File Offset: 0x0003120C
		protected override void OnDetaching()
		{
			base.AssociatedObject.ScrollChanged -= this.ScrollViewer_ScrollChanged;
			base.OnDetaching();
		}

		// Token: 0x060007CB RID: 1995 RVA: 0x0003302C File Offset: 0x0003122C
		private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			ScrollViewer scrollViewer = sender as ScrollViewer;
			if (scrollViewer == null)
			{
				return;
			}
			double verticalOffset = scrollViewer.VerticalOffset;
			double maxVerticalOffset = scrollViewer.ScrollableHeight;
			if (maxVerticalOffset - verticalOffset <= this.Threshold)
			{
				RoutedEventHandler reachedBottom = this.ReachedBottom;
				if (reachedBottom == null)
				{
					return;
				}
				reachedBottom(this, new RoutedEventArgs());
			}
		}

		// Token: 0x0400083F RID: 2111
		public static readonly DependencyProperty ThresholdProperty = DependencyProperty.Register("Threshold", typeof(double), typeof(ScrollViewerBottomBehavior), new PropertyMetadata(0.0));
	}
}
