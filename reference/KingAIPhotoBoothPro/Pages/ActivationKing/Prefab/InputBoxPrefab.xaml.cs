using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000064 RID: 100
	public partial class InputBoxPrefab : Page
	{
		// Token: 0x170000C7 RID: 199
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x00032904 File Offset: 0x00030B04
		// (set) Token: 0x060007B2 RID: 1970 RVA: 0x0003290C File Offset: 0x00030B0C
		public string Key { get; private set; }

		// Token: 0x170000C8 RID: 200
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x00032915 File Offset: 0x00030B15
		// (set) Token: 0x060007B4 RID: 1972 RVA: 0x0003291D File Offset: 0x00030B1D
		public string DefaultStatus { get; private set; }

		// Token: 0x170000C9 RID: 201
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x00032926 File Offset: 0x00030B26
		// (set) Token: 0x060007B6 RID: 1974 RVA: 0x0003292E File Offset: 0x00030B2E
		public string txtTitle { get; private set; }

		// Token: 0x170000CA RID: 202
		// (get) Token: 0x060007B7 RID: 1975 RVA: 0x00032937 File Offset: 0x00030B37
		// (set) Token: 0x060007B8 RID: 1976 RVA: 0x0003293F File Offset: 0x00030B3F
		public string txtDescription { get; private set; }

		// Token: 0x060007B9 RID: 1977 RVA: 0x00032948 File Offset: 0x00030B48
		public InputBoxPrefab(string key, string title, string defaultStatus = "", string description = "", bool isPassword = false)
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

		// Token: 0x060007BA RID: 1978 RVA: 0x000329B4 File Offset: 0x00030BB4
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

		// Token: 0x060007BB RID: 1979 RVA: 0x00032A04 File Offset: 0x00030C04
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

		// Token: 0x060007BC RID: 1980 RVA: 0x00032A54 File Offset: 0x00030C54
		public void Load()
		{
			this.Key == "returnpassword";
			this.txtFile.Text = this.txtTitle;
			this.txtDesc.Text = this.txtDescription;
			string loadKey = Settings.GetValueString(this.Key);
			if (loadKey == null)
			{
				this.txtInput.Text = this.DefaultStatus;
				this.passwordInput.Password = this.DefaultStatus;
				Settings.SetValue(this.Key, this.DefaultStatus, true);
			}
			else if (loadKey != this.DefaultStatus)
			{
				this.txtInput.Text = Settings.GetValueString(this.Key);
				this.passwordInput.Password = Settings.GetValueString(this.Key);
				this.txtInputPlaceholder.Visibility = Visibility.Hidden;
			}
			else
			{
				this.txtInput.Text = string.Empty;
				this.passwordInput.Password = string.Empty;
				this.txtInputPlaceholder.Visibility = Visibility.Visible;
			}
			this.txtInputPlaceholder.Content = this.DefaultStatus;
			if (string.IsNullOrEmpty(this.txtInput.Text) && string.IsNullOrEmpty(this.passwordInput.Password))
			{
				this.txtInputPlaceholder.Visibility = Visibility.Visible;
			}
			if (string.IsNullOrEmpty(this.txtDescription))
			{
				this.imgInfoIconGrid.Visibility = Visibility.Hidden;
				return;
			}
			this.imgInfoIconGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x00032BB4 File Offset: 0x00030DB4
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x00032BBC File Offset: 0x00030DBC
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

		// Token: 0x060007BF RID: 1983 RVA: 0x00032C44 File Offset: 0x00030E44
		private void txtInputPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.txtInputPlaceholder.Visibility = Visibility.Hidden;
			this.txtInput.Focus();
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x00032C60 File Offset: 0x00030E60
		private void ManagePortraitScreen()
		{
			bool isTight = SettingsPage.SettingsPageInstance.ActualWidth < SettingsPage.SettingsPageInstance.ActualHeight * 8.0;
			if (isTight)
			{
				this.gridBottom.ColumnDefinitions[1].Width = new GridLength(6.0, GridUnitType.Star);
				this.gridBottom.ColumnDefinitions[2].Width = new GridLength(60.0, GridUnitType.Star);
				return;
			}
			this.gridBottom.ColumnDefinitions[1].Width = new GridLength(10.0, GridUnitType.Star);
			this.gridBottom.ColumnDefinitions[2].Width = new GridLength(80.0, GridUnitType.Star);
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x00032D26 File Offset: 0x00030F26
		private void size_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x00032D30 File Offset: 0x00030F30
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
