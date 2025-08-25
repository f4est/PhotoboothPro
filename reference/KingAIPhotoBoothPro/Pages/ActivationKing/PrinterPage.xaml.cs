using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x0200004E RID: 78
	public partial class PrinterPage : SettingsSubPage
	{
		// Token: 0x06000651 RID: 1617 RVA: 0x0002CF44 File Offset: 0x0002B144
		public PrinterPage()
		{
			this.InitializeComponent();
			this.SliderTop = new SliderPrefab("printscaletop", "Print Scale Top", -50, 50, 0, false, false, "", "");
			this.SliderBottom = new SliderPrefab("printscalebottom", "Print Scale Bottom", -50, 50, 0, false, true, "", "");
			this.SliderLeft = new SliderPrefab("printscaleleft", "Print Scale Left", -50, 50, 0, true, true, "", "");
			this.SliderRight = new SliderPrefab("printscaleright", "Print Scale Right", -50, 50, 0, true, false, "", "");
			this.PrintButtonCheck = new TogglePrefab("printenable", "Print", false, false);
			this.printerListPrefab = new ComboboxPrefab("printer_name", "Select Printer", (ExtensionMethod.printers.Count > 0) ? ExtensionMethod.printers[0].Name : "", "", true);
			this.printerListPrefab.ComboNameBox.SelectionChanged += this.ComboNameBox_SelectionChanged;
			this.printerLimitPrefab = new InputBoxPrefab("printLimit", "Total Limit (-1 limitless)", "-1", "for the event", false);
			this.directPrintPrefab = new TogglePrefab("directprint", "One Print Per Photo", false, false);
			this.UseWindowsSettingsPrefab = new TogglePrefab("usewindowssettingsprint", "Use Windows Settings", false, false);
			this.UseDefaultPrintSettingsPrefab = new TogglePrefab("usedefaultprintsettings", "Use Default Print Settings", false, false);
			this.ForceLandscapePrefab = new TogglePrefab("forcelandscapeprint", "Force use Landscape", false, false);
			this.ForcePortraitPrefab = new TogglePrefab("forceportraitprint", "Force use Portrait", false, false);
			this.printerEachPhotoLimitPrefab = new InputBoxPrefab("printeachphotolimit", "Each Photo Limit (0 limitless)", "0", "for each photo", false);
			this.AutoPrintCheckPrefab = new TogglePrefab("autoprint", "Auto Print", false, false);
			this.printerEachPhotoLimitPrefab.ToolTip = "It shows the limit of how many prints you can get for each photo.\nIf you write 0, it will be unlimited.";
			this.AutoPrintCheckPrefab.ToolTip = "If you want auto print for each photo, enable this";
			this.printerComboBoxFrame.Content = this.printerListPrefab;
			this.PrintCheckFrame.Content = this.PrintButtonCheck;
			this.printerLimitFrame.Content = this.printerLimitPrefab;
			this.DirectPrintCheckFrame.Content = this.directPrintPrefab;
			this.ForceLandscapeFrame.Content = this.ForceLandscapePrefab;
			this.ForcePortraitFrame.Content = this.ForcePortraitPrefab;
			this.UseWindowsSettingsFrame.Content = this.UseWindowsSettingsPrefab;
			this.UseDefaultPrintSettingsFrame.Content = this.UseDefaultPrintSettingsPrefab;
			this.SliderTopFrame.Content = this.SliderTop;
			this.SliderBottomFrame.Content = this.SliderBottom;
			this.SliderLeftFrame.Content = this.SliderLeft;
			this.SliderRightFrame.Content = this.SliderRight;
			this.printerEachPhotoLimitFrame.Content = this.printerEachPhotoLimitPrefab;
			this.AutoPrintFrame.Content = this.AutoPrintCheckPrefab;
			this.SliderTop.thisSlider.ValueChanged += this.ThisSliderTop_ValueChanged;
			this.SliderBottom.thisSlider.ValueChanged += this.ThisSliderBottom_ValueChanged;
			this.SliderLeft.thisSlider.ValueChanged += this.ThisSliderLeft_ValueChanged;
			this.SliderRight.thisSlider.ValueChanged += this.ThisSliderRight_ValueChanged;
			this.printerLimitPrefab.txtInput.TextChanged += this.TxtInput_TextChanged;
			this.ForceLandscapePrefab.toggleElement.Checked += this.LandscapeToggleElement_Checked;
			this.ForcePortraitPrefab.toggleElement.Checked += this.PortraitToggleElement_Checked;
			this.printerEachPhotoLimitPrefab.txtInput.TextChanged += this.TxtIntInput_TextChanged;
			this.printerLimitPrefab.ToolTip = "It shows the limit of how many prints you can get for all event photo.\nIf you write -1 or 0, it will be unlimited.";
			this.directPrintPrefab.ToolTip = "Check this if you want only 1 print to be made when the print button is pressed";
			this.UseWindowsSettingsPrefab.ToolTip = "If printing systems created via Windows need to be used, activate it";
			this.UseDefaultPrintSettingsPrefab.ToolTip = "This setting allows you to print using your printer's default printer settings.\nEspecially for using different papers.";
			this.ForceLandscapeFrame.ToolTip = "If printing systems need force landscape, activate it";
			this.ForcePortraitFrame.ToolTip = "If printing systems need force landscape, activate it";
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x0002D37F File Offset: 0x0002B57F
		private void PortraitToggleElement_Checked(object sender, RoutedEventArgs e)
		{
			this.ForceLandscapePrefab.toggleElement.IsChecked = new bool?(false);
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x0002D397 File Offset: 0x0002B597
		private void LandscapeToggleElement_Checked(object sender, RoutedEventArgs e)
		{
			this.ForcePortraitPrefab.toggleElement.IsChecked = new bool?(false);
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x0002D3B0 File Offset: 0x0002B5B0
		private void CanvasForPrint_Loaded()
		{
			double width = this.CanvasForPrint.ActualWidth;
			double height = this.CanvasForPrint.ActualHeight;
			this.canvasPercentWidht = this.CanvasForPrint.ActualWidth / 100.0;
			this.canvasPercentHeight = this.CanvasForPrint.ActualHeight / 100.0;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x0002D40C File Offset: 0x0002B60C
		private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox referance = sender as TextBox;
			try
			{
				Convert.ToInt32(referance.Text);
			}
			catch (Exception)
			{
				referance.Text = "0";
			}
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x0002D44C File Offset: 0x0002B64C
		private void ThisSliderTop_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.canvasPercentWidht == 0.0)
			{
				this.CanvasForPrint_Loaded();
			}
			double percent = this.SliderTop.thisSlider.Value * this.canvasPercentHeight * -1.0;
			this.CanvasForPrint.Margin = new Thickness(this.CanvasForPrint.Margin.Left, percent, this.CanvasForPrint.Margin.Right, this.CanvasForPrint.Margin.Bottom);
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x0002D4DC File Offset: 0x0002B6DC
		private void ThisSliderBottom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.canvasPercentWidht == 0.0)
			{
				this.CanvasForPrint_Loaded();
			}
			double percent = this.SliderBottom.thisSlider.Value * this.canvasPercentHeight * -1.0;
			this.CanvasForPrint.Margin = new Thickness(this.CanvasForPrint.Margin.Left, this.CanvasForPrint.Margin.Top, this.CanvasForPrint.Margin.Right, percent);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x0002D56C File Offset: 0x0002B76C
		private void ThisSliderLeft_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.canvasPercentWidht == 0.0)
			{
				this.CanvasForPrint_Loaded();
			}
			double percent = this.SliderLeft.thisSlider.Value * this.canvasPercentWidht * -1.0;
			this.CanvasForPrint.Margin = new Thickness(percent, this.CanvasForPrint.Margin.Top, this.CanvasForPrint.Margin.Right, this.CanvasForPrint.Margin.Bottom);
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x0002D5FC File Offset: 0x0002B7FC
		private void ThisSliderRight_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.canvasPercentWidht == 0.0)
			{
				this.CanvasForPrint_Loaded();
			}
			double percent = this.SliderRight.thisSlider.Value * this.canvasPercentWidht * -1.0;
			this.CanvasForPrint.Margin = new Thickness(this.CanvasForPrint.Margin.Left, this.CanvasForPrint.Margin.Top, percent, this.CanvasForPrint.Margin.Bottom);
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x0002D68C File Offset: 0x0002B88C
		private void ComboNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				this.PrinterStatusImage.Source = BmsEngine.ChangeColorImage(ExtensionMethod.printers[this.printerListPrefab.ComboNameBox.SelectedIndex].Active ? Color.FromRgb(0, byte.MaxValue, 0) : Color.FromRgb(byte.MaxValue, 0, 0), (BitmapSource)this.PrinterStatusImage.Source);
			}
			catch (Exception)
			{
				this.PrinterStatusImage.Source = BmsEngine.ChangeColorImage(Color.FromRgb(byte.MaxValue, 0, 0), (BitmapSource)this.PrinterStatusImage.Source);
			}
		}

		// Token: 0x0600065B RID: 1627 RVA: 0x0002D738 File Offset: 0x0002B938
		private void TxtIntInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox referance = sender as TextBox;
			try
			{
				Convert.ToInt32(referance.Text);
			}
			catch (Exception)
			{
				referance.Text = "0";
			}
		}

		// Token: 0x0600065C RID: 1628 RVA: 0x0002D778 File Offset: 0x0002B978
		public override string GetSubTitle()
		{
			return "Here you can set your event options. According to your event need please set the options.";
		}

		// Token: 0x0600065D RID: 1629 RVA: 0x0002D77F File Offset: 0x0002B97F
		public override string GetTitle()
		{
			return "Print Settings";
		}

		// Token: 0x0600065E RID: 1630 RVA: 0x0002D788 File Offset: 0x0002B988
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			PrinterPage.<Page_Loaded>d__34 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<PrinterPage.<Page_Loaded>d__34>(ref <Page_Loaded>d__);
		}

		// Token: 0x0600065F RID: 1631 RVA: 0x0002D7C0 File Offset: 0x0002B9C0
		private Task ManagePrinterSelection()
		{
			PrinterPage.<ManagePrinterSelection>d__35 <ManagePrinterSelection>d__;
			<ManagePrinterSelection>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ManagePrinterSelection>d__.<>4__this = this;
			<ManagePrinterSelection>d__.<>1__state = -1;
			<ManagePrinterSelection>d__.<>t__builder.Start<PrinterPage.<ManagePrinterSelection>d__35>(ref <ManagePrinterSelection>d__);
			return <ManagePrinterSelection>d__.<>t__builder.Task;
		}

		// Token: 0x040006C0 RID: 1728
		private ComboboxPrefab printerListPrefab;

		// Token: 0x040006C1 RID: 1729
		private InputBoxPrefab printerLimitPrefab;

		// Token: 0x040006C2 RID: 1730
		private SliderPrefab SliderTop;

		// Token: 0x040006C3 RID: 1731
		private SliderPrefab SliderBottom;

		// Token: 0x040006C4 RID: 1732
		private SliderPrefab SliderLeft;

		// Token: 0x040006C5 RID: 1733
		private SliderPrefab SliderRight;

		// Token: 0x040006C6 RID: 1734
		private TogglePrefab PrintButtonCheck;

		// Token: 0x040006C7 RID: 1735
		private TogglePrefab directPrintPrefab;

		// Token: 0x040006C8 RID: 1736
		private TogglePrefab UseWindowsSettingsPrefab;

		// Token: 0x040006C9 RID: 1737
		private TogglePrefab UseDefaultPrintSettingsPrefab;

		// Token: 0x040006CA RID: 1738
		private TogglePrefab ForceLandscapePrefab;

		// Token: 0x040006CB RID: 1739
		private TogglePrefab ForcePortraitPrefab;

		// Token: 0x040006CC RID: 1740
		private TogglePrefab AutoPrintCheckPrefab;

		// Token: 0x040006CD RID: 1741
		public static int printedNumber;

		// Token: 0x040006CE RID: 1742
		public static double printTop;

		// Token: 0x040006CF RID: 1743
		public static double printBottom;

		// Token: 0x040006D0 RID: 1744
		public static double printLeft;

		// Token: 0x040006D1 RID: 1745
		public static double printRight;

		// Token: 0x040006D2 RID: 1746
		private double canvasPercentWidht;

		// Token: 0x040006D3 RID: 1747
		private double canvasPercentHeight;

		// Token: 0x040006D4 RID: 1748
		private InputBoxPrefab printerEachPhotoLimitPrefab;
	}
}
