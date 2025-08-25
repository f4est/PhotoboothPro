using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000061 RID: 97
	public partial class ComboboxPrefab : Page
	{
		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000782 RID: 1922 RVA: 0x00031EE1 File Offset: 0x000300E1
		// (set) Token: 0x06000783 RID: 1923 RVA: 0x00031EE9 File Offset: 0x000300E9
		public string Key { get; private set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000784 RID: 1924 RVA: 0x00031EF2 File Offset: 0x000300F2
		// (set) Token: 0x06000785 RID: 1925 RVA: 0x00031EFA File Offset: 0x000300FA
		public string EmptyString { get; private set; }

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x06000786 RID: 1926 RVA: 0x00031F03 File Offset: 0x00030103
		// (set) Token: 0x06000787 RID: 1927 RVA: 0x00031F0B File Offset: 0x0003010B
		public string txtTitle { get; private set; }

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000788 RID: 1928 RVA: 0x00031F14 File Offset: 0x00030114
		// (set) Token: 0x06000789 RID: 1929 RVA: 0x00031F1C File Offset: 0x0003011C
		public string txtDescription { get; set; }

		// Token: 0x0600078A RID: 1930 RVA: 0x00031F28 File Offset: 0x00030128
		public ComboboxPrefab(string key, string title, string emptyString = "", string description = "", bool isEmptyEnable = false)
		{
			this.Key = key;
			this.EmptyString = emptyString;
			this.txtTitle = title;
			this.txtDescription = description;
			this.isEmptyEnable = isEmptyEnable;
			this.InitializeComponent();
			this.ComboNameBox.SelectionChanged += this.ComboNameBox_SelectionChanged;
		}

		// Token: 0x0600078B RID: 1931 RVA: 0x00031F80 File Offset: 0x00030180
		public void Load()
		{
			this.Key == "mode";
			this.txtFile.Text = this.txtTitle;
			this.txtDesc.Text = this.txtDescription;
			if (string.IsNullOrEmpty(this.txtDescription))
			{
				this.gridColumn.ColumnDefinitions[0].Width = new GridLength(10.0, GridUnitType.Star);
				this.gridColumn.ColumnDefinitions[1].Width = new GridLength(0.0, GridUnitType.Star);
				this.gridColumn.ColumnDefinitions[2].Width = new GridLength(2.0, GridUnitType.Star);
			}
			string returnKey = Settings.GetValueString(this.Key);
			if (returnKey == null)
			{
				Settings.DefaultValues.TryGetValue(this.Key, out returnKey);
			}
			if (!string.IsNullOrEmpty(returnKey) && this.ComboNameBox.Items.Contains(returnKey))
			{
				this.ComboNameBox.SelectedIndex = this.ComboNameBox.Items.IndexOf(returnKey);
				this.LateLoad();
				return;
			}
			if (!this.isEmptyEnable && this.ComboNameBox.Items.Count > 0)
			{
				this.ComboNameBox.SelectedIndex = 0;
				Settings.SetValue(this.Key, this.ComboNameBox.Items[0].ToString(), true);
				this.LateLoad();
				return;
			}
			this.ComboNameBox.SelectedIndex = -1;
			this.ComboNameBox.Text = this.EmptyString;
			Settings.SetValue(this.Key, returnKey, true);
			this.LateLoad();
		}

		// Token: 0x0600078C RID: 1932 RVA: 0x0003211C File Offset: 0x0003031C
		private void LateLoad()
		{
			this.SetBorderColor();
		}

		// Token: 0x0600078D RID: 1933 RVA: 0x00032124 File Offset: 0x00030324
		private void ComboNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.lastEventChanged = e;
			this.Key == "cameratype";
			if (e.AddedItems.Count > 0)
			{
				if (e.AddedItems[0] != null)
				{
					Settings.SetValue(this.Key, e.AddedItems[0].ToString(), true);
				}
			}
			else if (e.RemovedItems.Count > 0)
			{
				Settings.SetValue(this.Key, "", true);
			}
			this.SetBorderColor();
		}

		// Token: 0x0600078E RID: 1934 RVA: 0x000321A9 File Offset: 0x000303A9
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
			this.SetBorderColor();
		}

		// Token: 0x0600078F RID: 1935 RVA: 0x000321B8 File Offset: 0x000303B8
		private void SetBorderColor()
		{
			if (this.ComboNameBox.SelectedItem == null)
			{
				this.ComboNameBox.BorderBrush = (SolidColorBrush)base.FindResource("UIErrorColor");
				this.ComboNameBox.BorderThickness = new Thickness(4.0);
				return;
			}
			this.ComboNameBox.BorderBrush = (SolidColorBrush)base.FindResource("UIGrayLight");
			this.ComboNameBox.BorderThickness = new Thickness(2.0);
		}

		// Token: 0x06000790 RID: 1936 RVA: 0x0003223B File Offset: 0x0003043B
		private void ToggleButton_Checked()
		{
		}

		// Token: 0x06000791 RID: 1937 RVA: 0x0003223D File Offset: 0x0003043D
		public void SetLoading(bool loading)
		{
			if (loading)
			{
				this.loadingGifGrid.Visibility = Visibility.Visible;
				return;
			}
			this.loadingGifGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000792 RID: 1938 RVA: 0x0003225B File Offset: 0x0003045B
		private void ComboNameBox_DropDownClosed(object sender, EventArgs e)
		{
		}

		// Token: 0x0400080E RID: 2062
		private bool isEmptyEnable;

		// Token: 0x0400080F RID: 2063
		private DispatcherTimer selectionTimer;

		// Token: 0x04000810 RID: 2064
		private SelectionChangedEventArgs lastEventChanged;
	}
}
