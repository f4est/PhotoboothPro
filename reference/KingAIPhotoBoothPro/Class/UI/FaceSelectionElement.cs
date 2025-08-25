using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace KingAIPhotoBoothPro.Class.UI
{
	// Token: 0x020000BD RID: 189
	internal class FaceSelectionElement : Grid
	{
		// Token: 0x1700017A RID: 378
		// (get) Token: 0x06000A63 RID: 2659 RVA: 0x0003C376 File Offset: 0x0003A576
		// (set) Token: 0x06000A64 RID: 2660 RVA: 0x0003C37E File Offset: 0x0003A57E
		public bool IsSelected { get; private set; }

		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06000A65 RID: 2661 RVA: 0x0003C387 File Offset: 0x0003A587
		// (set) Token: 0x06000A66 RID: 2662 RVA: 0x0003C38F File Offset: 0x0003A58F
		public int FaceIndex { get; private set; }

		// Token: 0x06000A67 RID: 2663 RVA: 0x0003C398 File Offset: 0x0003A598
		public FaceSelectionElement(int faceIndex, bool isSelected = false)
		{
			base.Cursor = Cursors.Hand;
			base.Background = Brushes.Transparent;
			this._contentGrid = new Grid
			{
				Background = Brushes.Transparent
			};
			this._mainBorder = new Border
			{
				CornerRadius = new CornerRadius(10.0),
				BorderThickness = new Thickness(6.0),
				BorderBrush = this.normalColor,
				Background = Brushes.Transparent
			};
			this.SetSelected(isSelected);
			base.Children.Add(this._mainBorder);
			base.Children.Add(this._contentGrid);
			base.MouseEnter += this.OnMouseEnter;
			base.MouseLeave += this.OnMouseLeave;
			base.MouseLeftButtonDown += this.OnMouseClick;
			base.TouchDown += this.OnTouchDown;
			this.FaceIndex = faceIndex;
		}

		// Token: 0x06000A68 RID: 2664 RVA: 0x0003C52E File Offset: 0x0003A72E
		private void OnTouchDown(object sender, TouchEventArgs e)
		{
			this.Select();
		}

		// Token: 0x06000A69 RID: 2665 RVA: 0x0003C536 File Offset: 0x0003A736
		private void OnMouseEnter(object sender, MouseEventArgs e)
		{
			this._mainBorder.BorderBrush = this.hoverColor;
			base.Opacity = 0.8;
		}

		// Token: 0x06000A6A RID: 2666 RVA: 0x0003C558 File Offset: 0x0003A758
		private void OnMouseLeave(object sender, MouseEventArgs e)
		{
			this._mainBorder.BorderBrush = (this.IsSelected ? this.selectedColor : this.normalColor);
			base.Opacity = 1.0;
		}

		// Token: 0x06000A6B RID: 2667 RVA: 0x0003C58A File Offset: 0x0003A78A
		private void OnMouseClick(object sender, MouseButtonEventArgs e)
		{
			this.Select();
		}

		// Token: 0x06000A6C RID: 2668 RVA: 0x0003C592 File Offset: 0x0003A792
		private void Select()
		{
			Action<FaceSelectionElement, bool> selectionChanged = this.SelectionChanged;
			if (selectionChanged == null)
			{
				return;
			}
			selectionChanged(this, !this.IsSelected);
		}

		// Token: 0x06000A6D RID: 2669 RVA: 0x0003C5AE File Offset: 0x0003A7AE
		public void SetContent(UIElement content)
		{
			this._contentGrid.Children.Clear();
			this._contentGrid.Children.Add(content);
		}

		// Token: 0x06000A6E RID: 2670 RVA: 0x0003C5D2 File Offset: 0x0003A7D2
		public void SetBorderColors(Brush normalColor, Brush selectedColor, Brush hoverColor)
		{
			this.normalColor = normalColor;
			this.selectedColor = selectedColor;
			this.hoverColor = hoverColor;
			this._mainBorder.BorderBrush = (this.IsSelected ? selectedColor : normalColor);
		}

		// Token: 0x06000A6F RID: 2671 RVA: 0x0003C600 File Offset: 0x0003A800
		internal void SetSelected(bool isSelected)
		{
			this.IsSelected = isSelected;
			this._mainBorder.BorderBrush = (this.IsSelected ? this.selectedColor : this.normalColor);
			this._mainBorder.Background = (this.IsSelected ? this.selectedBackgroundColor : Brushes.Transparent);
		}

		// Token: 0x04000A1E RID: 2590
		private Border _mainBorder;

		// Token: 0x04000A1F RID: 2591
		private Grid _contentGrid;

		// Token: 0x04000A20 RID: 2592
		private Brush normalColor = Brushes.Green;

		// Token: 0x04000A21 RID: 2593
		private Brush selectedColor = Brushes.Red;

		// Token: 0x04000A22 RID: 2594
		private Brush selectedBackgroundColor = new SolidColorBrush(new Color
		{
			A = 150,
			R = 100,
			G = 0,
			B = 0
		});

		// Token: 0x04000A23 RID: 2595
		private Brush hoverColor = new SolidColorBrush(new Color
		{
			A = 150,
			R = byte.MaxValue,
			G = byte.MaxValue,
			B = byte.MaxValue
		});

		// Token: 0x04000A26 RID: 2598
		public Action<FaceSelectionElement, bool> SelectionChanged;
	}
}
