using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.Helper;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x0200005C RID: 92
	public partial class SliderPrefab : Page
	{
		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x0600072B RID: 1835 RVA: 0x00031138 File Offset: 0x0002F338
		// (set) Token: 0x0600072C RID: 1836 RVA: 0x00031140 File Offset: 0x0002F340
		public string Key { get; private set; }

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x0600072D RID: 1837 RVA: 0x00031149 File Offset: 0x0002F349
		// (set) Token: 0x0600072E RID: 1838 RVA: 0x00031151 File Offset: 0x0002F351
		public string DefaultStatus { get; private set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x0600072F RID: 1839 RVA: 0x0003115A File Offset: 0x0002F35A
		// (set) Token: 0x06000730 RID: 1840 RVA: 0x00031162 File Offset: 0x0002F362
		public string txtTitle { get; private set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000731 RID: 1841 RVA: 0x0003116B File Offset: 0x0002F36B
		// (set) Token: 0x06000732 RID: 1842 RVA: 0x00031173 File Offset: 0x0002F373
		public string txtDescription { get; private set; }

		// Token: 0x170000AB RID: 171
		// (get) Token: 0x06000733 RID: 1843 RVA: 0x0003117C File Offset: 0x0002F37C
		// (set) Token: 0x06000734 RID: 1844 RVA: 0x00031184 File Offset: 0x0002F384
		public bool horizontalSlider { get; private set; }

		// Token: 0x170000AC RID: 172
		// (get) Token: 0x06000735 RID: 1845 RVA: 0x0003118D File Offset: 0x0002F38D
		// (set) Token: 0x06000736 RID: 1846 RVA: 0x00031195 File Offset: 0x0002F395
		public bool reverseSlider { get; private set; }

		// Token: 0x170000AD RID: 173
		// (get) Token: 0x06000737 RID: 1847 RVA: 0x0003119E File Offset: 0x0002F39E
		// (set) Token: 0x06000738 RID: 1848 RVA: 0x000311A6 File Offset: 0x0002F3A6
		public int maxValue { get; private set; }

		// Token: 0x170000AE RID: 174
		// (get) Token: 0x06000739 RID: 1849 RVA: 0x000311AF File Offset: 0x0002F3AF
		// (set) Token: 0x0600073A RID: 1850 RVA: 0x000311B7 File Offset: 0x0002F3B7
		public int minValue { get; private set; }

		// Token: 0x170000AF RID: 175
		// (get) Token: 0x0600073B RID: 1851 RVA: 0x000311C0 File Offset: 0x0002F3C0
		// (set) Token: 0x0600073C RID: 1852 RVA: 0x000311C8 File Offset: 0x0002F3C8
		public double valueForSlider { get; private set; }

		// Token: 0x0600073D RID: 1853 RVA: 0x000311D4 File Offset: 0x0002F3D4
		public SliderPrefab(string key, string title, int sliderMin, int sliderMax, int sliderDefaultValue = 0, bool isHorizontal = true, bool reverse = false, string defaultStatus = "", string description = "")
		{
			this.Key = key;
			this.DefaultStatus = defaultStatus;
			this.txtTitle = title;
			this.txtDescription = description;
			this.maxValue = sliderMax;
			this.minValue = sliderMin;
			this.valueForSlider = (double)sliderDefaultValue;
			this.horizontalSlider = isHorizontal;
			this.reverseSlider = reverse;
			this.InitializeComponent();
		}

		// Token: 0x0600073E RID: 1854 RVA: 0x00031233 File Offset: 0x0002F433
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x0600073F RID: 1855 RVA: 0x0003123C File Offset: 0x0002F43C
		private void txtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.start)
			{
				try
				{
					this.valueForSlider = this.thisSlider.Value;
					Settings.SetValue(this.Key, this.thisText.Text, true);
				}
				catch (Exception)
				{
					this.valueForSlider = 0.0;
					Settings.SetValue(this.Key, "0", true);
				}
			}
		}

		// Token: 0x06000740 RID: 1856 RVA: 0x000312B0 File Offset: 0x0002F4B0
		public void Load()
		{
			string loadKey = Settings.GetValueString(this.Key);
			if (loadKey == null)
			{
				this.thisText.Text = this.DefaultStatus;
				Settings.SetValue(this.Key, this.DefaultStatus, true);
				this.valueForSlider = this.thisSlider.Value;
			}
			else if (loadKey != this.DefaultStatus)
			{
				this.thisText.Text = Settings.GetValueString(this.Key);
				this.valueForSlider = this.thisSlider.Value;
			}
			this.thisSlider.Maximum = (double)this.maxValue;
			this.thisSlider.Minimum = (double)this.minValue;
			if (this.horizontalSlider)
			{
				this.thisText.Margin = new Thickness(0.0, this.thisText.ActualWidth * 1.5, 0.0, 0.0);
				this.thisSlider.Orientation = Orientation.Horizontal;
				this.thisSlider.HorizontalAlignment = HorizontalAlignment.Stretch;
				this.thisSlider.VerticalAlignment = VerticalAlignment.Center;
			}
			else
			{
				this.thisText.Margin = new Thickness(0.0, 0.0, this.thisText.ActualWidth * 1.5, 0.0);
				this.thisSlider.Orientation = Orientation.Vertical;
				this.thisSlider.HorizontalAlignment = HorizontalAlignment.Center;
				this.thisSlider.VerticalAlignment = VerticalAlignment.Stretch;
			}
			this.thisSlider.IsDirectionReversed = this.reverseSlider;
			double parced;
			double.TryParse(this.thisText.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out parced);
			this.thisSlider.Value = parced;
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				Thread.Sleep(500);
				this.start = true;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x040007E4 RID: 2020
		private bool start;
	}
}
