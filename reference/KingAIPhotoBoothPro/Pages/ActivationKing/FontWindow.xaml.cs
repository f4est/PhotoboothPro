using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000042 RID: 66
	public partial class FontWindow : Window
	{
		// Token: 0x060004BE RID: 1214 RVA: 0x0001A9A4 File Offset: 0x00018BA4
		public FontWindow()
		{
			this.InitializeComponent();
			this.fontNameCombobox = new ComboboxPrefab("selectedfontname", "Select Font", "", "", false);
			this.fontSizePrefab = new InputBoxPrefab("selectedfontsize", "Select Font Size", "100", "", false);
			this.FontExampleTextPrefab = new InputBoxPrefab("selectedexampletext", "Select Example Text", "Text", "", false);
			this.FontColorPrefab = new ColorPickerPrefab("selectedfontcolor", "Select Font Color", "#FF000000");
			this.FontPlaceholderTextPrefab = new InputBoxPrefab("selectedfontplaceholdertext", "Text Placeholder in Main Page", "Name And Surname", "", false);
			this.fontNameCombobox.ComboNameBox.SelectionChanged += this.ComboNameBox_SelectionChanged;
			this.fontSizePrefab.txtInput.TextChanged += this.TxtInput_TextChanged;
			this.FontExampleTextPrefab.txtInput.TextChanged += this.ExampleTxtInput_TextChanged;
			this.FontColorPrefab.colorChanged += this.FontColorPrefab_colorChanged;
		}

		// Token: 0x060004BF RID: 1215 RVA: 0x0001AACD File Offset: 0x00018CCD
		private void FontColorPrefab_colorChanged()
		{
			if (TemplatePage.AddedText != null)
			{
				Application.Current.Dispatcher.Invoke(delegate()
				{
					TemplatePage.AddedText.Foreground = new SolidColorBrush(Settings.GetValueColor("selectedfontcolor"));
				});
			}
		}

		// Token: 0x060004C0 RID: 1216 RVA: 0x0001AB04 File Offset: 0x00018D04
		private void ComboNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (TemplatePage.AddedText != null)
			{
				TemplatePage.AddedText.FontFamily = ExtensionMethod.FindFont(this.fontNameCombobox.ComboNameBox.SelectedIndex);
			}
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x0001AB2C File Offset: 0x00018D2C
		private void ExampleTxtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (TemplatePage.AddedText != null)
			{
				TemplatePage.AddedText.Text = this.FontExampleTextPrefab.txtInput.Text;
			}
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x0001AB50 File Offset: 0x00018D50
		private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (string.IsNullOrEmpty(textBox.Text.Trim()))
			{
				this._previousValidText = "";
				textBox.Text = "100";
				return;
			}
			double num;
			if (double.TryParse(textBox.Text, out num))
			{
				this._previousValidText = textBox.Text;
				if (TemplatePage.AddedText != null)
				{
					TemplatePage.AddedText.FontSize = (double)Convert.ToInt32(this.fontSizePrefab.txtInput.Text) * TemplatePage.canvasScale * 1.3300000429153442;
					return;
				}
			}
			else
			{
				int caretIndex = textBox.CaretIndex - 1;
				textBox.Text = this._previousValidText;
				textBox.CaretIndex = Math.Max(caretIndex, 0);
			}
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x0001AC04 File Offset: 0x00018E04
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.fontNameCombobox.ComboNameBox.ItemsSource = ExtensionMethod.GetFontNames();
			this.FontComboboxFrame.Content = this.fontNameCombobox;
			this.FontSizeFrame.Content = this.fontSizePrefab;
			this.FontExampleTextFrame.Content = this.FontExampleTextPrefab;
			this.FontColorFrame.Content = this.FontColorPrefab;
			this.FontPlaceholderTextFrame.Content = this.FontPlaceholderTextPrefab;
			if (Settings.GetValueString("selectedfontsize") == this.fontSizePrefab.DefaultStatus)
			{
				this.fontSizePrefab.txtInput.Text = "110";
			}
			if (Settings.GetValueString("selectedexampletext") == this.FontExampleTextPrefab.DefaultStatus)
			{
				this.FontExampleTextPrefab.txtInput.Text = "TextExample";
			}
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x0001ACDD File Offset: 0x00018EDD
		private void FontSettingsGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!this.FontColorFrame.IsFocused && e.ChangedButton == MouseButton.Left)
			{
				base.DragMove();
			}
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x0001ACFA File Offset: 0x00018EFA
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			base.Visibility = Visibility.Hidden;
		}

		// Token: 0x040004BA RID: 1210
		public ComboboxPrefab fontNameCombobox;

		// Token: 0x040004BB RID: 1211
		public InputBoxPrefab fontSizePrefab;

		// Token: 0x040004BC RID: 1212
		public InputBoxPrefab FontExampleTextPrefab;

		// Token: 0x040004BD RID: 1213
		public ColorPickerPrefab FontColorPrefab;

		// Token: 0x040004BE RID: 1214
		public InputBoxPrefab FontPlaceholderTextPrefab;

		// Token: 0x040004BF RID: 1215
		private string _previousValidText = "";
	}
}
