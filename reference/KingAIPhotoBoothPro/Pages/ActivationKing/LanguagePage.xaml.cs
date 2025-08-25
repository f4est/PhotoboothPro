using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x0200004A RID: 74
	public partial class LanguagePage : SettingsSubPage
	{
		// Token: 0x060005CD RID: 1485 RVA: 0x00028124 File Offset: 0x00026324
		public LanguagePage()
		{
			this.InitializeComponent();
			this.inputBoxPrefabs = new List<InputBoxPrefab>
			{
				new InputBoxPrefab("lang_photo", "Photo", "Photo", "", false),
				new InputBoxPrefab("lang_video", "Video", "Video", "", false),
				new InputBoxPrefab("lang_gif", "Gif", "Gif", "", false),
				new InputBoxPrefab("lang_cancel", "Cancel", "Cancel", "", false),
				new InputBoxPrefab("lang_done", "Done", "DONE", "", false),
				new InputBoxPrefab("lang_gallery", "Gallery", "Gallery", "", false),
				new InputBoxPrefab("lang_remainingseconds", "Remaining Seconds", "Approximately Remaining Seconds", "", false),
				new InputBoxPrefab("lang_selectfacebackground", "Select Background", "Select Background", "", false),
				new InputBoxPrefab("lang_print", "Print", "PRINT", "", false),
				new InputBoxPrefab("lang_email", "E-mail", "E-MAIL", "", false),
				new InputBoxPrefab("lang_close", "Close", "CLOSE", "", false),
				new InputBoxPrefab("lang_qrpreparing", "QR Preparing Text", "Preparing Your Media To Share...", "", false),
				new InputBoxPrefab("lang_qrshow", "QR Show Text", "Scan The QR Code To Download Your Media", "", false),
				new InputBoxPrefab("lang_printnow", "Print Now Button", "PRINT NOW", "", false),
				new InputBoxPrefab("lang_mailsend", "Mail Send Button", "SEND", "", false),
				new InputBoxPrefab("lang_smsbutton", "SMS Button Title", "SMS", "", false),
				new InputBoxPrefab("lang_saveasbutton", "Save As Button Title", "Save As", "", false),
				new InputBoxPrefab("lang_resultimgtitle", "AI Sharing Result Image Title", "AI IMAGES", "", false),
				new InputBoxPrefab("lang_originalimgtitle", "AI Sharing Original Image Title", "ORIGINAL", "", false),
				new InputBoxPrefab("lang_aiimgbutton", "AI Prompt Button Title", "AI Prompt", "", false),
				new InputBoxPrefab("lang_faceswapbutton", "Faceswap Button Title", "Face Swap", "", false),
				new InputBoxPrefab("lang_wordcloudbutton", "Word Portrait Button Title", "Word Portrait", "", false),
				new InputBoxPrefab("lang_aimotionbutton", "AI Motion Button Title", "AI Motion", "", false),
				new InputBoxPrefab("lang_import", "Import Button Title", "Import", "", false),
				new InputBoxPrefab("lang_delete", "Gallery Delete Button Title", "DELETE", "", false),
				new InputBoxPrefab("lang_aieffectbutton", "AI Effect Button Title", "AI Effect", "", false),
				new InputBoxPrefab("lang_aibeautifierbutton", "AI Beautifier Button Title", "AI Beautifier", "", false),
				new InputBoxPrefab("lang_paymentmaintitle", "Payment Main Page Title", "Pick Your Fun!", "", false),
				new InputBoxPrefab("lang_paymentmaintext", "Payment Main Page Text", "Choose how you want your photos - and how many!", "", false),
				new InputBoxPrefab("lang_paymentmainpaybutton", "Payment Main Page Pay Button", "Let's Go-Pay ", "", false),
				new InputBoxPrefab("lang_paymentmainbackbutton", "Payment Main Page Back Button", "GO BACK", "", false),
				new InputBoxPrefab("lang_paymentmainprinttitle", "Payment Main Page Print Title", "Print", "", false),
				new InputBoxPrefab("lang_paymentmainprinttext", "Payment Main Page Print Text", "Take it home: 1 photo print + digital download.", "", false),
				new InputBoxPrefab("lang_paymentmaindigitaltitle", "Payment Main Page Digital Title", "Digital Download", "", false),
				new InputBoxPrefab("lang_paymentmaindigitaltext", "Payment Main Page Digital Text", "Go digital: Instant download (no print).", "", false),
				new InputBoxPrefab("lang_paymentloadingtext", "Payment Loading Page Text", "Complete your payment to keep the fun going.", "", false),
				new InputBoxPrefab("lang_paymentloadingtitle", "Payment Loading Page Title", "Almost There!", "", false),
				new InputBoxPrefab("lang_paymentloadingbackbutton", "Payment Loading Page Back Button", "GO BACK", "", false),
				new InputBoxPrefab("lang_paymentloadingscanqrtext", "Payment Loading Page Scan QR Text", "Scan the QR code with your phone to pay.", "", false),
				new InputBoxPrefab("lang_paymentconfirmationtitle", "Payment Confirmation Page Title", "You're All Set!", "", false),
				new InputBoxPrefab("lang_paymentconfirmationtext", "Payment Confirmation Page Text", "You've unlocked: ", "", false),
				new InputBoxPrefab("lang_paymentconfirmationbackbutton", "Payment Confirmation Page Back Button", "REMOVE SESSION", "", false),
				new InputBoxPrefab("lang_paymentcorfirmationprintbutton", "Payment Confirmation Page Pay Button", "Start Print ", "", false),
				new InputBoxPrefab("lang_paymentconfirmationdigitalbutton", "Payment Confirmation Page Pay Button", "Start Digital Download", "", false)
			};
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x000286C8 File Offset: 0x000268C8
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.LanguageList.Children.Clear();
			int counter = 0;
			foreach (InputBoxPrefab item in this.inputBoxPrefabs)
			{
				Frame newFrame = new Frame();
				newFrame.Content = item;
				this.LanguageList.Children.Insert(this.LanguageList.Children.Count, newFrame);
				counter++;
			}
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00028758 File Offset: 0x00026958
		public override string GetTitle()
		{
			return "Screen Language Settings";
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x0002875F File Offset: 0x0002695F
		public override string GetSubTitle()
		{
			return "You can customize the texts on the screen";
		}

		// Token: 0x040005EF RID: 1519
		private List<InputBoxPrefab> inputBoxPrefabs;
	}
}
