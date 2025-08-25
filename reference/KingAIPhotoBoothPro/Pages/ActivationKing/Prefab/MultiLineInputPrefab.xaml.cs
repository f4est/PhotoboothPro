using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000055 RID: 85
	public partial class MultiLineInputPrefab : Page
	{
		// Token: 0x17000097 RID: 151
		// (get) Token: 0x060006C4 RID: 1732 RVA: 0x0002F5C2 File Offset: 0x0002D7C2
		// (set) Token: 0x060006C5 RID: 1733 RVA: 0x0002F5CA File Offset: 0x0002D7CA
		public string Key { get; private set; }

		// Token: 0x17000098 RID: 152
		// (get) Token: 0x060006C6 RID: 1734 RVA: 0x0002F5D3 File Offset: 0x0002D7D3
		// (set) Token: 0x060006C7 RID: 1735 RVA: 0x0002F5DB File Offset: 0x0002D7DB
		public string DefaultStatus { get; private set; }

		// Token: 0x17000099 RID: 153
		// (get) Token: 0x060006C8 RID: 1736 RVA: 0x0002F5E4 File Offset: 0x0002D7E4
		// (set) Token: 0x060006C9 RID: 1737 RVA: 0x0002F5EC File Offset: 0x0002D7EC
		public string txtTitle { get; private set; }

		// Token: 0x1700009A RID: 154
		// (get) Token: 0x060006CA RID: 1738 RVA: 0x0002F5F5 File Offset: 0x0002D7F5
		// (set) Token: 0x060006CB RID: 1739 RVA: 0x0002F5FD File Offset: 0x0002D7FD
		public string txtDescription { get; private set; }

		// Token: 0x060006CC RID: 1740 RVA: 0x0002F608 File Offset: 0x0002D808
		public MultiLineInputPrefab(string key, string title, string defaultStatus = "", string description = "", bool isPassword = false)
		{
			this.Key = key;
			this.DefaultStatus = defaultStatus;
			this.txtTitle = title;
			this.txtDescription = description;
			this.InitializeComponent();
			if (isPassword)
			{
				this.txtInput.Visibility = Visibility.Collapsed;
				this.passwordInput.Visibility = Visibility.Visible;
				return;
			}
			this.txtInput.Visibility = Visibility.Visible;
			this.passwordInput.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0002F674 File Offset: 0x0002D874
		private void txtInput_TextChanged(object sender, RoutedEventArgs e)
		{
			if (this.passwordInput.Password.Length == 0)
			{
				this.txtInputPlaceholder.Visibility = Visibility.Visible;
			}
			else
			{
				this.txtInputPlaceholder.Visibility = Visibility.Hidden;
			}
			Settings.SetValue(this.Key, this.passwordInput.Password, true);
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x0002F6C4 File Offset: 0x0002D8C4
		private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.txtInput.Text.Length == 0)
			{
				this.txtInputPlaceholder.Visibility = Visibility.Visible;
			}
			else
			{
				this.txtInputPlaceholder.Visibility = Visibility.Hidden;
			}
			Settings.SetValue(this.Key, this.txtInput.Text, true);
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x0002F714 File Offset: 0x0002D914
		public void Load()
		{
			MultiLineInputPrefab.<Load>d__19 <Load>d__;
			<Load>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Load>d__.<>4__this = this;
			<Load>d__.<>1__state = -1;
			<Load>d__.<>t__builder.Start<MultiLineInputPrefab.<Load>d__19>(ref <Load>d__);
		}

		// Token: 0x060006D0 RID: 1744 RVA: 0x0002F74B File Offset: 0x0002D94B
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060006D1 RID: 1745 RVA: 0x0002F754 File Offset: 0x0002D954
		private void txtInput_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.txtInput.Text.Length == 0)
			{
				this.txtInputPlaceholder.Visibility = Visibility.Visible;
			}
			else if (this.txtInputPlaceholder.Visibility == Visibility.Visible)
			{
				this.txtInputPlaceholder.Visibility = Visibility.Collapsed;
			}
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				this.TextBoxBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#BABFC5"));
				this.TextBoxBorder.BorderThickness = new Thickness(1.0);
			}
		}

		// Token: 0x060006D2 RID: 1746 RVA: 0x0002F7DC File Offset: 0x0002D9DC
		private void txtInputPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.txtInputPlaceholder.Visibility = Visibility.Hidden;
			this.txtInput.Focus();
		}

		// Token: 0x060006D3 RID: 1747 RVA: 0x0002F7F8 File Offset: 0x0002D9F8
		private void ManagePortraitScreen()
		{
			bool isTight = SettingsPage.SettingsPageInstance.ActualWidth < SettingsPage.SettingsPageInstance.ActualHeight * 8.0;
			if (isTight)
			{
				if (this.gridBottom.ColumnDefinitions.Count > 1)
				{
					this.gridBottom.ColumnDefinitions[1].Width = new GridLength(6.0, GridUnitType.Star);
					this.gridBottom.ColumnDefinitions[2].Width = new GridLength(60.0, GridUnitType.Star);
					return;
				}
			}
			else if (this.gridBottom.ColumnDefinitions.Count > 1)
			{
				this.gridBottom.ColumnDefinitions[1].Width = new GridLength(10.0, GridUnitType.Star);
				this.gridBottom.ColumnDefinitions[2].Width = new GridLength(80.0, GridUnitType.Star);
			}
		}

		// Token: 0x060006D4 RID: 1748 RVA: 0x0002F8E7 File Offset: 0x0002DAE7
		private void size_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x060006D5 RID: 1749 RVA: 0x0002F8F0 File Offset: 0x0002DAF0
		private void txtInput_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox != null)
			{
				this.TextBoxBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B82F6"));
				this.TextBoxBorder.BorderThickness = new Thickness(1.0);
			}
		}
	}
}
