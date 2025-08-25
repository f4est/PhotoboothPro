using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace KingAIPhotoBoothPro.Class.UI
{
	// Token: 0x020000BE RID: 190
	public class ScrollViewerDragBehavior : Behavior<ScrollViewer>
	{
		// Token: 0x06000A70 RID: 2672 RVA: 0x0003C658 File Offset: 0x0003A858
		protected override void OnAttached()
		{
			base.OnAttached();
			base.AssociatedObject.PreviewMouseDown += this.OnMouseDown;
			base.AssociatedObject.PreviewMouseMove += this.OnMouseMove;
			base.AssociatedObject.PreviewMouseUp += this.OnMouseUp;
		}

		// Token: 0x06000A71 RID: 2673 RVA: 0x0003C6B0 File Offset: 0x0003A8B0
		protected override void OnDetaching()
		{
			base.OnDetaching();
			base.AssociatedObject.PreviewMouseDown -= this.OnMouseDown;
			base.AssociatedObject.PreviewMouseMove -= this.OnMouseMove;
			base.AssociatedObject.PreviewMouseUp -= this.OnMouseUp;
		}

		// Token: 0x06000A72 RID: 2674 RVA: 0x0003C708 File Offset: 0x0003A908
		private void OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.IsClickOnScrollbar(e))
			{
				this.isDragging = false;
				return;
			}
			this.scrollMousePoint = e.GetPosition(base.AssociatedObject);
			this.vOffset = base.AssociatedObject.VerticalOffset;
			this.isDragging = false;
		}

		// Token: 0x06000A73 RID: 2675 RVA: 0x0003C748 File Offset: 0x0003A948
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed && !this.IsClickOnScrollbar(e))
			{
				Point currentMousePoint = e.GetPosition(base.AssociatedObject);
				double delta = this.scrollMousePoint.Y - currentMousePoint.Y;
				if (Math.Abs(delta) > 10.0)
				{
					this.isDragging = true;
					base.AssociatedObject.ScrollToVerticalOffset(this.vOffset + delta);
					base.AssociatedObject.CaptureMouse();
				}
			}
		}

		// Token: 0x06000A74 RID: 2676 RVA: 0x0003C7BF File Offset: 0x0003A9BF
		private void OnMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (base.AssociatedObject.IsMouseCaptured)
			{
				base.AssociatedObject.ReleaseMouseCapture();
			}
			if (!this.isDragging)
			{
				e.Handled = false;
			}
		}

		// Token: 0x06000A75 RID: 2677 RVA: 0x0003C7E8 File Offset: 0x0003A9E8
		private bool IsClickOnScrollbar(MouseEventArgs e)
		{
			DependencyObject depObj = e.OriginalSource as DependencyObject;
			while (depObj != null && depObj != base.AssociatedObject)
			{
				if (depObj is ScrollBar)
				{
					return true;
				}
				depObj = VisualTreeHelper.GetParent(depObj);
			}
			return false;
		}

		// Token: 0x04000A27 RID: 2599
		private Point scrollMousePoint;

		// Token: 0x04000A28 RID: 2600
		private double vOffset;

		// Token: 0x04000A29 RID: 2601
		private bool isDragging;

		// Token: 0x04000A2A RID: 2602
		private const double DragThreshold = 10.0;
	}
}
