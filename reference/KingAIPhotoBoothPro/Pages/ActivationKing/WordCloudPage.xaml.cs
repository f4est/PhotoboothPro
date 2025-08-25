using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000047 RID: 71
	public partial class WordCloudPage : SettingsSubPage
	{
		// Token: 0x0600059E RID: 1438 RVA: 0x000268E4 File Offset: 0x00024AE4
		public WordCloudPage()
		{
			this.InitializeComponent();
			this.worldcloudInputBoxPrefab = new InputBoxPrefab("wordcloud", "Word Portrait Text", WordCloudPage.defaultString, "Write the words that will be in your photo", false);
			this.worldcloudFontComboBoxPrefab = new ComboboxPrefab("wordcloudfont", "Word Portrait Font Name", "", "Select your Word Portrait Font", false);
			this.colorPickerPrefabList = new List<ColorPickerPrefab>
			{
				new ColorPickerPrefab("wordcloudbackcolor", "Word Portrait Background Color", "#01FFFFF2"),
				new ColorPickerPrefab("wordcloudforegroundcolor", "Word Portrait Foreground Color", "#FD034751")
			};
			this.worldcloudFontComboBoxPrefab.ComboNameBox.ItemsSource = this.fontNames;
			this.worldcloudInputBoxFrame.Content = this.worldcloudInputBoxPrefab;
			this.worldcloudFontFrame.Content = this.worldcloudFontComboBoxPrefab;
			this.ColorPicker1Frame.Content = this.colorPickerPrefabList[0];
			this.ColorPicker2Frame.Content = this.colorPickerPrefabList[1];
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0002704C File Offset: 0x0002524C
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.spaceGrid.Height = (this.worldcloudFontFrame.Height = (this.worldcloudInputBoxFrame.Height = this.gridMain.ActualWidth * 0.08 + 1.0));
			this.ColorPicker2Frame.Height = (this.ColorPicker1Frame.Height = this.gridMain.ActualWidth * 0.4 + 1.0);
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x000270D7 File Offset: 0x000252D7
		public override string GetTitle()
		{
			return "WordPortre Settings";
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x000270DE File Offset: 0x000252DE
		public override string GetSubTitle()
		{
			return "You can change the texts inside your wordportre.";
		}

		// Token: 0x040005A7 RID: 1447
		private List<string> fontNames = new List<string>
		{
			"Anton-Regular",
			"BebasNeue-Regular",
			"Cabin-Bold",
			"Cabin-BoldItalic",
			"Cabin_Condensed-Bold",
			"Cabin_Condensed-BoldItalic",
			"Cabin_Condensed-Italic",
			"Cabin_Condensed-Medium",
			"Cabin_Condensed-MediumItalic",
			"Cabin_Condensed-Regular",
			"Cabin_Condensed-SemiBold",
			"Cabin_Condensed-SemiBoldItalic",
			"Cabin-Italic",
			"Cabin-Italic-VariableFont_wdth,wght",
			"Cabin-Medium",
			"Cabin-MediumItalic",
			"Cabin-Regular",
			"Cabin-SemiBold",
			"Cabin-SemiBoldItalic",
			"Cabin_SemiCondensed-Bold",
			"Cabin_SemiCondensed-BoldItalic",
			"Cabin_SemiCondensed-Italic",
			"Cabin_SemiCondensed-Medium",
			"Cabin_SemiCondensed-MediumItalic",
			"Cabin_SemiCondensed-Regular",
			"Cabin_SemiCondensed-SemiBold",
			"Cabin_SemiCondensed-SemiBoldItalic",
			"Cabin-VariableFont_wdth,wght",
			"InriaSans-Bold",
			"InriaSans-BoldItalic",
			"InriaSans-Italic",
			"InriaSans-Light",
			"InriaSans-LightItalic",
			"InriaSans-Regular",
			"Kanit-Black",
			"Kanit-Blackltalic",
			"Kanit-Bold",
			"Kanit-BoldItalic",
			"Kanit-ExtraBold",
			"Kanit-ExtraBoldItalic",
			"Kanit-ExtraLight",
			"Kanit-ExtraLightItalic",
			"Kanit-Italic",
			"Kanit-Light",
			"Kanit-LightItalic",
			"Kanit-Medium",
			"Kanit-Mediumltalic",
			"Kanit-Regular",
			"Kanit-SemiBold",
			"Kanit-SemiBoldItalic",
			"Kanit-Thin",
			"Kanit-ThinItalic",
			"NanumGothic-Bold",
			"NanumGothic-ExtraBold",
			"NanumGothic-Regular",
			"OpenSans-Bold",
			"OpenSans-BoldItalic",
			"OpenSans_Condensed-Bold",
			"OpenSans_Condensed-BoldItalic",
			"OpenSans_Condensed-ExtraBold",
			"OpenSans_Condensed-ExtraBoldItalic",
			"OpenSans_Condensed-Italic",
			"OpenSans_Condensed-Light",
			"OpenSans_Condensed-LightItalic",
			"OpenSans_Condensed-Medium",
			"OpenSans_Condensed-Mediumltalic",
			"OpenSans_Condensed-Regular",
			"OpenSans_Condensed-SemiBold",
			"OpenSans_Condensed-SemiBoldItalic",
			"OpenSans-ExtraBold",
			"OpenSans-ExtraBoldItalic",
			"OpenSans-Italic",
			"OpenSans-Italic-VariableFont_wdth,wght",
			"OpenSans-Light",
			"OpenSans-LightItalic",
			"OpenSans-Medium",
			"OpenSans-Mediumltalic",
			"OpenSans-Regular",
			"OpenSans-SemiBold",
			"OpenSans-SemiBoldItalic",
			"OpenSans-VariableFont_wdth,wght",
			"OpenSans_SemiCondensed-Bold",
			"OpenSans_SemiCondensed-BoldItalic",
			"OpenSans_SemiCondensed-ExtraBold",
			"OpenSans_SemiCondensed-ExtraBoldItalic",
			"OpenSans_SemiCondensed-Italic",
			"OpenSans_SemiCondensed-Light",
			"OpenSans_SemiCondensed-LightItalic",
			"OpenSans_SemiCondensed-Medium",
			"OpenSans_SemiCondensed-Mediumltalic",
			"OpenSans_SemiCondensed-Regular",
			"OpenSans_SemiCondensed-SemiBold",
			"OpenSans_SemiCondensed-SemiBoldItalic",
			"Oswald-Bold",
			"Oswald-ExtraLight",
			"Oswald-Light",
			"Oswald-Medium",
			"Oswald-Regular",
			"Oswald-SemiBold",
			"Oswald-VariableFont_wght",
			"PlayfairDisplay-Black",
			"PlayfairDisplay-Blackltalic",
			"PlayfairDisplay-Bold",
			"PlayfairDisplay-BoldItalic",
			"PlayfairDisplay-ExtraBold",
			"PlayfairDisplay-ExtraBoldItalic",
			"PlayfairDisplay-Italic",
			"PlayfairDisplay-Italic-VariableFont_wght",
			"PlayfairDisplay-Medium",
			"PlayfairDisplay-MediumItalic",
			"PlayfairDisplay-Regular",
			"PlayfairDisplay-SemiBold",
			"PlayfairDisplay-SemiBoldItalic",
			"PlayfairDisplay-VariableFont_wght",
			"PlaypenSans-Bold",
			"PlaypenSans-ExtraBold",
			"PlaypenSans-ExtraLight",
			"PlaypenSans-Light",
			"PlaypenSans-Medium",
			"PlaypenSans-Regular",
			"PlaypenSans-SemiBold",
			"PlaypenSans-Thin",
			"PlaypenSans-VariableFont_wght",
			"SofadiOne-Regular",
			"SourceCodePro-Black",
			"SourceCodePro-Blackltalic",
			"SourceCodePro-Bold",
			"SourceCodePro-BoldItalic",
			"SourceCodePro-ExtraBold",
			"SourceCodePro-ExtraBoldItalic",
			"SourceCodePro-ExtraLight",
			"SourceCodePro-ExtraLightitalic",
			"SourceCodePro-Italic",
			"SourceCodePro-Italic-VariableFont_wght",
			"SourceCodePro-Light",
			"SourceCodePro-LightItalic",
			"SourceCodePro-Medium",
			"SourceCodePro-MediumItalic",
			"SourceCodePro-Regular",
			"SourceCodePro-SemiBold",
			"SourceCodePro-SemiBoldItalic",
			"SourceCodePro-VariableFont_wght",
			"Teko-Bold",
			"Teko-Light",
			"Teko-Medium",
			"Teko-Regular",
			"Teko-SemiBold",
			"Teko-VariableFont_wght"
		};

		// Token: 0x040005A8 RID: 1448
		private InputBoxPrefab worldcloudInputBoxPrefab;

		// Token: 0x040005A9 RID: 1449
		private ComboboxPrefab worldcloudFontComboBoxPrefab;

		// Token: 0x040005AA RID: 1450
		private List<ColorPickerPrefab> colorPickerPrefabList;

		// Token: 0x040005AB RID: 1451
		public static string defaultString = "Inspiration, Creativity, Passion, Dream, Vision, Harmony, Strength, Wisdom, Beauty, Courage, Grace, Love, Hope, Faith, Joy, Peace, Unity, Soul, Spirit, Serenity, Freedom, Light, Integrity, Kindness, Compassion";
	}
}
