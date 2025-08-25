using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000054 RID: 84
	public partial class IsCropCheckBox : Page
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0002F46E File Offset: 0x0002D66E
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x0002F476 File Offset: 0x0002D676
		public bool isCrop { get; private set; }

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x0002F47F File Offset: 0x0002D67F
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x0002F487 File Offset: 0x0002D687
		public string txtTitle { get; private set; }

		// Token: 0x060006BD RID: 1725 RVA: 0x0002F490 File Offset: 0x0002D690
		public IsCropCheckBox(int index, string title, bool isCrop = false)
		{
			this.index = index;
			this.txtTitle = title;
			this.isCrop = isCrop;
			this.InitializeComponent();
		}

		// Token: 0x060006BE RID: 1726 RVA: 0x0002F4B3 File Offset: 0x0002D6B3
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x060006BF RID: 1727 RVA: 0x0002F4BB File Offset: 0x0002D6BB
		public void Load()
		{
			this.CheckElement.Content = this.txtTitle;
			this.CheckElement.IsChecked = new bool?(this.isCrop);
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x0002F4E4 File Offset: 0x0002D6E4
		public void CheckElement_Checked(object sender, RoutedEventArgs e)
		{
			this.isCrop = true;
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x0002F4ED File Offset: 0x0002D6ED
		public void CheckElement_Unchecked(object sender, RoutedEventArgs e)
		{
			this.CheckElement.Foreground = Brushes.Gray;
			this.isCrop = false;
		}

		// Token: 0x0400076D RID: 1901
		public int index;
	}
}
