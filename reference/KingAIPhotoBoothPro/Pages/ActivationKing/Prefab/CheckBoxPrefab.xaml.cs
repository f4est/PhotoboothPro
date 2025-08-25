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
	// Token: 0x0200005F RID: 95
	public partial class CheckBoxPrefab : Page
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x00031945 File Offset: 0x0002FB45
		// (set) Token: 0x0600075F RID: 1887 RVA: 0x0003194D File Offset: 0x0002FB4D
		public string Key { get; private set; }

		// Token: 0x170000B6 RID: 182
		// (get) Token: 0x06000760 RID: 1888 RVA: 0x00031956 File Offset: 0x0002FB56
		// (set) Token: 0x06000761 RID: 1889 RVA: 0x0003195E File Offset: 0x0002FB5E
		public bool DefaultStatus { get; private set; }

		// Token: 0x170000B7 RID: 183
		// (get) Token: 0x06000762 RID: 1890 RVA: 0x00031967 File Offset: 0x0002FB67
		// (set) Token: 0x06000763 RID: 1891 RVA: 0x0003196F File Offset: 0x0002FB6F
		public string txtTitle { get; private set; }

		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000764 RID: 1892 RVA: 0x00031978 File Offset: 0x0002FB78
		// (set) Token: 0x06000765 RID: 1893 RVA: 0x00031980 File Offset: 0x0002FB80
		public Action<bool> ValueChanged { get; set; }

		// Token: 0x06000766 RID: 1894 RVA: 0x00031989 File Offset: 0x0002FB89
		public CheckBoxPrefab(string key, string title, bool defaultStatus = false)
		{
			this.Key = key;
			this.txtTitle = title;
			this.DefaultStatus = defaultStatus;
			this.InitializeComponent();
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x000319AC File Offset: 0x0002FBAC
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x000319B4 File Offset: 0x0002FBB4
		public void SetValue(bool value)
		{
			this.CheckElement.IsChecked = new bool?(value);
			Settings.SetValue(this.Key, value);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x000319D4 File Offset: 0x0002FBD4
		public void Load()
		{
			this.Key == "upload";
			this.CheckElement.Content = this.txtTitle;
			if (Settings.GetValueBoolean(this.Key) == null)
			{
				this.CheckElement.IsChecked = new bool?(this.DefaultStatus);
				Settings.SetValue(this.Key, this.DefaultStatus);
				return;
			}
			this.CheckElement.IsChecked = new bool?(Settings.GetValueBoolean(this.Key).Value);
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x00031A63 File Offset: 0x0002FC63
		public void CheckElement_Checked(object sender, RoutedEventArgs e)
		{
			Settings.SetValue(this.Key, true);
			Action<bool> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(true);
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x00031A82 File Offset: 0x0002FC82
		public void CheckElement_Unchecked(object sender, RoutedEventArgs e)
		{
			this.UnCheck();
			Action<bool> valueChanged = this.ValueChanged;
			if (valueChanged == null)
			{
				return;
			}
			valueChanged(false);
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x00031A9B File Offset: 0x0002FC9B
		private void UnCheck()
		{
			this.CheckElement.Foreground = Brushes.Gray;
			Settings.SetValue(this.Key, false);
		}

		// Token: 0x0600076D RID: 1901 RVA: 0x00031ABC File Offset: 0x0002FCBC
		private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			this.Key == "upload";
			Page page = sender as Page;
			if (page != null)
			{
				Visibility newVisibility = page.Visibility;
				bool oldIsVisible = (bool)e.OldValue;
				bool newIsVisible = (bool)e.NewValue;
				if (newVisibility == Visibility.Visible)
				{
					this.Load();
				}
			}
		}
	}
}
