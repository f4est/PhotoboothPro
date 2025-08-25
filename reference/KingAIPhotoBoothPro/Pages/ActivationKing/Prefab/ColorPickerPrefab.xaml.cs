using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using ColorPicker;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000060 RID: 96
	public partial class ColorPickerPrefab : Page
	{
		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000770 RID: 1904 RVA: 0x00031BE1 File Offset: 0x0002FDE1
		// (set) Token: 0x06000771 RID: 1905 RVA: 0x00031BE9 File Offset: 0x0002FDE9
		public string Key { get; private set; }

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x06000772 RID: 1906 RVA: 0x00031BF2 File Offset: 0x0002FDF2
		// (set) Token: 0x06000773 RID: 1907 RVA: 0x00031BFA File Offset: 0x0002FDFA
		public string DefaultColor { get; private set; }

		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000774 RID: 1908 RVA: 0x00031C03 File Offset: 0x0002FE03
		// (set) Token: 0x06000775 RID: 1909 RVA: 0x00031C0B File Offset: 0x0002FE0B
		public string txtTitle { get; private set; }

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06000776 RID: 1910 RVA: 0x00031C14 File Offset: 0x0002FE14
		// (remove) Token: 0x06000777 RID: 1911 RVA: 0x00031C4C File Offset: 0x0002FE4C
		public event Action colorChanged;

		// Token: 0x06000778 RID: 1912 RVA: 0x00031C81 File Offset: 0x0002FE81
		public ColorPickerPrefab(string key, string title, string defaultStatus = "#00000000")
		{
			this.Key = key;
			this.DefaultColor = defaultStatus;
			this.txtTitle = title;
			this.InitializeComponent();
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x00031CA4 File Offset: 0x0002FEA4
		private void colorPickElement_ColorChanged(object sender, RoutedEventArgs e)
		{
			this.color = this.colorPickElement.SelectedColor;
			this.colorHexCode = this.color.ToString();
			Settings.SetValue(this.Key, this.colorHexCode, true);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x00031CE0 File Offset: 0x0002FEE0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x00031CE8 File Offset: 0x0002FEE8
		public void Load()
		{
			this.txtName.Text = this.txtTitle;
			if (Settings.GetValueString(this.Key) == null)
			{
				this.colorHexCode = this.DefaultColor;
				Settings.SetValue(this.Key, this.DefaultColor, true);
				return;
			}
			this.colorHexCode = Settings.GetValueString(this.Key);
			this.color = (Color)ColorConverter.ConvertFromString(this.colorHexCode);
			this.colorPickElement.SelectedColor = this.color;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x00031D6A File Offset: 0x0002FF6A
		private void colorPickElement_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.colorChanged != null && this.isMouseDown)
			{
				this.colorChanged();
				this.isMouseDown = false;
			}
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x00031D8E File Offset: 0x0002FF8E
		private void colorPickElement_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			if (this.colorChanged != null)
			{
				new Thread(delegate()
				{
					Thread.Sleep(100);
					this.colorChanged();
				}).Start();
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x00031DAE File Offset: 0x0002FFAE
		private void colorPickElement_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.isMouseDown)
			{
				this.isMouseDown = true;
			}
		}

		// Token: 0x04000801 RID: 2049
		public string colorHexCode;

		// Token: 0x04000802 RID: 2050
		public Color color;

		// Token: 0x04000804 RID: 2052
		private bool isMouseDown;
	}
}
