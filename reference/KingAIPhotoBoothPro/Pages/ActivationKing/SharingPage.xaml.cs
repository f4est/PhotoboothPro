using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x0200004F RID: 79
	public partial class SharingPage : SettingsSubPage
	{
		// Token: 0x06000665 RID: 1637 RVA: 0x0002DA64 File Offset: 0x0002BC64
		public bool CheckRealtimeDeliveryAccess()
		{
			if ((!Settings.GetValueBoolean(this.MailButtonCheck.Key)).GetValueOrDefault())
			{
				return false;
			}
			for (int i = 0; i < this.mailInfoList.Count; i++)
			{
				if (string.IsNullOrEmpty(Settings.GetValueString(this.mailInfoList[i].Key)))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000666 RID: 1638 RVA: 0x0002DAE8 File Offset: 0x0002BCE8
		public SharingPage()
		{
			this.InitializeComponent();
			this.MailButtonCheck = new TogglePrefab("mailenable", "Mail", false, false);
			this.SMSButtonCheck = new TogglePrefab("smsenable", "SMS", false, false);
			this.QRButtonCheck = new TogglePrefab("qrenable", "QR", true, false);
			this.SaveAsCheckPrefab = new TogglePrefab("saveas", "Save As PC", false, false);
			this.senderMailAddress = new InputBoxPrefab("mail_address", "Sender Mail Address", "", "", false);
			this.senderMailPassword = new InputBoxPrefab("mail_password", "Sender Mail Password", "", "", true);
			this.senderHost = new InputBoxPrefab("mail_host", "Sender Host", "example smtp.gmail.com", "", false);
			this.senderPort = new InputBoxPrefab("mail_port", "Sender Port", "example 587", "", false);
			this.subject = new InputBoxPrefab("mail_subject", "Subject", "Your Photo Booth Photo!", "", false);
			this.mailBody = new InputBoxPrefab("mail_body", "Mail Body", "Your media is here", "", false);
			this.lppdInputPrefab = new MultiLineInputPrefab("lppdtext", "Write LPPD Agreement", "", "Law on the Protection of Personal Data", false);
			this.lppdTitlePrefab = new InputBoxPrefab("lppdtitle", "Write LPPD Agreement Title", "", "", false);
			this.LppdCheckPrefab = new TogglePrefab("lppdactive", "LPPD", false, false);
			this.smsAuthTokenPrefab = new InputBoxPrefab("smsauthtoken", "SMS Auth Token", "", "Twilio Auth Token For Send SMS", false);
			this.smsSIDPrefab = new InputBoxPrefab("smssid", "SMS SID", "", "Twilio SID For Send SMS", false);
			this.smsHostNumberPrefab = new InputBoxPrefab("smshost", "SMS Host Phone Number", "", "Twilio Account Avaiable Phone Number Add Country Number (Exp +1)", false);
			this.smsTestNumberPrefab = new InputBoxPrefab("smstest", "SMS Test Phone Number", "", "Please Write Number For Test", false);
			this.mailTestNumberPrefab = new InputBoxPrefab("mailtest", "Receiver Mail Address", "", "Please Write Email For Test", false);
			this.senderHostFrame.Content = this.senderHost;
			this.senderMailPasswordFrame.Content = this.senderMailPassword;
			this.senderMailAddressFrame.Content = this.senderMailAddress;
			this.senderPortFrame.Content = this.senderPort;
			this.subjectFrame.Content = this.subject;
			this.mailBodyFrame.Content = this.mailBody;
			this.LPPDActiveFrame.Content = this.LppdCheckPrefab;
			this.LPPDInputFrame.Content = this.lppdInputPrefab;
			this.LPPDTitleFrame.Content = this.lppdTitlePrefab;
			this.MailCheckFrame.Content = this.MailButtonCheck;
			this.SMSCheckFrame.Content = this.SMSButtonCheck;
			this.QRCheckFrame.Content = this.QRButtonCheck;
			this.SaveAsCheckFrame.Content = this.SaveAsCheckPrefab;
			this.SMSAuthTokenFrame.Content = this.smsAuthTokenPrefab;
			this.SMSHostNumberFrame.Content = this.smsHostNumberPrefab;
			this.SMSTestNumberFrame.Content = this.smsTestNumberPrefab;
			this.mailTestInputFrame.Content = this.mailTestNumberPrefab;
			this.SMSSidFrame.Content = this.smsSIDPrefab;
			this.mailInfoList = new List<InputBoxPrefab>
			{
				this.senderMailAddress,
				this.senderMailPassword,
				this.senderHost,
				this.senderPort,
				this.subject,
				this.mailBody
			};
		}

		// Token: 0x06000667 RID: 1639 RVA: 0x0002DE99 File Offset: 0x0002C099
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000668 RID: 1640 RVA: 0x0002DE9B File Offset: 0x0002C09B
		public override string GetTitle()
		{
			return "Sharing Settings";
		}

		// Token: 0x06000669 RID: 1641 RVA: 0x0002DEA2 File Offset: 0x0002C0A2
		public override string GetSubTitle()
		{
			return "You can adjust sharing settings here.";
		}

		// Token: 0x0600066A RID: 1642 RVA: 0x0002DEAC File Offset: 0x0002C0AC
		private void SMSTestButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.smsTestNumberPrefab.txtInput.Text.Length > 11 && this.smsTestNumberPrefab.txtInput.Text.Contains("+"))
			{
				this.SMSTestButton.IsEnabled = false;
				string testNumber = this.smsTestNumberPrefab.txtInput.Text;
				new Thread(delegate()
				{
					Task<bool> smsTask = TwilioHelper.SendTest("Test SMS", testNumber);
					smsTask.Wait();
					if (smsTask.Result)
					{
						MessageBoxWindow.CreateWindow("Succes", "SMS Send Succesfull", new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					}
					else
					{
						MessageBoxWindow.CreateWindow("SMS Didn't Send", "Please Control Twilio Account", new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					}
					Application.Current.Dispatcher.Invoke(delegate()
					{
						this.SMSTestButton.IsEnabled = true;
					});
				}).Start();
				return;
			}
			MessageBoxWindow.CreateWindow("SMS Didn't Send", "Please Control Test Phone Number", new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Continue
			}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
		}

		// Token: 0x0600066B RID: 1643 RVA: 0x0002DF54 File Offset: 0x0002C154
		private void MailTestButton_Click(object sender, RoutedEventArgs e)
		{
			SharingPage.<MailTestButton_Click>d__25 <MailTestButton_Click>d__;
			<MailTestButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<MailTestButton_Click>d__.<>4__this = this;
			<MailTestButton_Click>d__.<>1__state = -1;
			<MailTestButton_Click>d__.<>t__builder.Start<SharingPage.<MailTestButton_Click>d__25>(ref <MailTestButton_Click>d__);
		}

		// Token: 0x040006EB RID: 1771
		private TogglePrefab MailButtonCheck;

		// Token: 0x040006EC RID: 1772
		private TogglePrefab SMSButtonCheck;

		// Token: 0x040006ED RID: 1773
		private TogglePrefab QRButtonCheck;

		// Token: 0x040006EE RID: 1774
		private TogglePrefab SaveAsCheckPrefab;

		// Token: 0x040006EF RID: 1775
		private TogglePrefab LppdCheckPrefab;

		// Token: 0x040006F0 RID: 1776
		private InputBoxPrefab senderMailAddress;

		// Token: 0x040006F1 RID: 1777
		private InputBoxPrefab senderMailPassword;

		// Token: 0x040006F2 RID: 1778
		private InputBoxPrefab senderHost;

		// Token: 0x040006F3 RID: 1779
		private InputBoxPrefab senderPort;

		// Token: 0x040006F4 RID: 1780
		private InputBoxPrefab subject;

		// Token: 0x040006F5 RID: 1781
		private InputBoxPrefab mailBody;

		// Token: 0x040006F6 RID: 1782
		private InputBoxPrefab lppdTitlePrefab;

		// Token: 0x040006F7 RID: 1783
		private MultiLineInputPrefab lppdInputPrefab;

		// Token: 0x040006F8 RID: 1784
		private InputBoxPrefab smsAuthTokenPrefab;

		// Token: 0x040006F9 RID: 1785
		private InputBoxPrefab smsSIDPrefab;

		// Token: 0x040006FA RID: 1786
		private InputBoxPrefab smsHostNumberPrefab;

		// Token: 0x040006FB RID: 1787
		private InputBoxPrefab smsTestNumberPrefab;

		// Token: 0x040006FC RID: 1788
		private InputBoxPrefab mailTestNumberPrefab;

		// Token: 0x040006FD RID: 1789
		public List<InputBoxPrefab> mailInfoList;
	}
}
