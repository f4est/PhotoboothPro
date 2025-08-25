using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200003C RID: 60
	public class SettingsPage : Page, IComponentConnector
	{
		// Token: 0x060003F4 RID: 1012 RVA: 0x00015FF4 File Offset: 0x000141F4
		public void SetSubPage(SettingsSubPageEnum settingsSubPage)
		{
			switch (settingsSubPage)
			{
			case SettingsSubPageEnum.DashboardPage:
				this.RightPanelPages.Content = SettingsPage.dashboardPage;
				this.ActivateEventButtonBlink();
				break;
			case SettingsSubPageEnum.EventManagementPage:
				this.RightPanelPages.Content = SettingsPage.eventManagementPage;
				break;
			case SettingsSubPageEnum.MainSettingsPage:
				this.RightPanelPages.Content = SettingsPage.mainSettingsPage;
				break;
			case SettingsSubPageEnum.LanguagePage:
				this.RightPanelPages.Content = SettingsPage.languagePage;
				break;
			case SettingsSubPageEnum.SharingPage:
				this.RightPanelPages.Content = SettingsPage.sharingPage;
				break;
			case SettingsSubPageEnum.TemplatePage:
				this.RightPanelPages.Content = SettingsPage.templatePage;
				break;
			case SettingsSubPageEnum.ThemePage:
				this.RightPanelPages.Content = SettingsPage.themePage;
				break;
			case SettingsSubPageEnum.TemplateVideoPage:
				this.RightPanelPages.Content = SettingsPage.templatePageVideo;
				break;
			case SettingsSubPageEnum.FaceSwapPage:
				this.RightPanelPages.Content = SettingsPage.faceswapPage;
				break;
			case SettingsSubPageEnum.PromptsPage:
				this.RightPanelPages.Content = SettingsPage.samplePromptsPage;
				break;
			case SettingsSubPageEnum.WordCloud:
				this.RightPanelPages.Content = SettingsPage.wordCloudPage;
				break;
			case SettingsSubPageEnum.AIEffectPage:
				this.RightPanelPages.Content = SettingsPage.aiEffectPage;
				break;
			case SettingsSubPageEnum.PrinterPage:
				this.RightPanelPages.Content = SettingsPage.printerPage;
				break;
			}
			this.txtTitle.Text = this.GetSubPage(settingsSubPage).GetTitle();
			this.txtSubTitle.Text = this.GetSubPage(settingsSubPage).GetSubTitle();
			this.GetMenuButton(settingsSubPage).Visibility = Visibility.Visible;
			for (int i = 0; i < this.LeftMenuButtonsList.Length; i++)
			{
				this.LeftMenuButtonsList[i].Background.Opacity = (double)((settingsSubPage != (SettingsSubPageEnum)i) ? 0f : 1f);
				this.LeftMenuButtonsList[i].Foreground = ((settingsSubPage != (SettingsSubPageEnum)i) ? Brushes.White : this.SeletedMenuButtonTextBrush);
			}
			SettingsPage.CurrentSubPage = settingsSubPage;
		}

		// Token: 0x060003F5 RID: 1013 RVA: 0x000161D4 File Offset: 0x000143D4
		public SettingsSubPage GetSubPage(SettingsSubPageEnum settingsSubPage)
		{
			switch (settingsSubPage)
			{
			case SettingsSubPageEnum.DashboardPage:
				return SettingsPage.dashboardPage;
			case SettingsSubPageEnum.EventManagementPage:
				return SettingsPage.eventManagementPage;
			case SettingsSubPageEnum.MainSettingsPage:
				return SettingsPage.mainSettingsPage;
			case SettingsSubPageEnum.LanguagePage:
				return SettingsPage.languagePage;
			case SettingsSubPageEnum.SharingPage:
				return SettingsPage.sharingPage;
			case SettingsSubPageEnum.TemplatePage:
				return SettingsPage.templatePage;
			case SettingsSubPageEnum.ThemePage:
				return SettingsPage.themePage;
			case SettingsSubPageEnum.TemplateVideoPage:
				return SettingsPage.templatePageVideo;
			case SettingsSubPageEnum.FaceSwapPage:
				return SettingsPage.faceswapPage;
			case SettingsSubPageEnum.PromptsPage:
				return SettingsPage.samplePromptsPage;
			case SettingsSubPageEnum.AIEffectPage:
				return SettingsPage.aiEffectPage;
			case SettingsSubPageEnum.PrinterPage:
				return SettingsPage.printerPage;
			}
			return SettingsPage.dashboardPage;
		}

		// Token: 0x060003F6 RID: 1014 RVA: 0x0001626A File Offset: 0x0001446A
		private Button GetMenuButton(SettingsSubPageEnum settingsSubPage)
		{
			return this.LeftMenuButtonsList[(int)settingsSubPage];
		}

		// Token: 0x17000070 RID: 112
		// (get) Token: 0x060003F7 RID: 1015 RVA: 0x00016274 File Offset: 0x00014474
		// (set) Token: 0x060003F8 RID: 1016 RVA: 0x000162AB File Offset: 0x000144AB
		public SolidColorBrush SeletedMenuButtonTextBrush
		{
			get
			{
				if (this.seletedMenuButtonTextBrush == null)
				{
					string hexColor = "#707270";
					this.seletedMenuButtonTextBrush = (SolidColorBrush)new BrushConverter().ConvertFromString(hexColor);
				}
				return this.seletedMenuButtonTextBrush;
			}
			set
			{
				this.seletedMenuButtonTextBrush = value;
			}
		}

		// Token: 0x17000071 RID: 113
		// (get) Token: 0x060003F9 RID: 1017 RVA: 0x000162B4 File Offset: 0x000144B4
		public Thickness LabelMargin
		{
			get
			{
				double screenWidth = SystemParameters.PrimaryScreenWidth;
				double margin = screenWidth * 0.01;
				return new Thickness(margin, 0.0, 0.0, 0.0);
			}
		}

		// Token: 0x17000072 RID: 114
		// (get) Token: 0x060003FA RID: 1018 RVA: 0x000162F4 File Offset: 0x000144F4
		public Thickness ButtonMargin
		{
			get
			{
				double screenWidth = SystemParameters.PrimaryScreenWidth;
				double margin = -screenWidth * 0.01;
				return new Thickness(margin, 0.0, 0.0, 0.0);
			}
		}

		// Token: 0x17000073 RID: 115
		// (get) Token: 0x060003FB RID: 1019 RVA: 0x00016335 File Offset: 0x00014535
		// (set) Token: 0x060003FC RID: 1020 RVA: 0x00016350 File Offset: 0x00014550
		public string gifPath
		{
			get
			{
				return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "loadingH.gif");
			}
			set
			{
			}
		}

		// Token: 0x060003FD RID: 1021 RVA: 0x00016352 File Offset: 0x00014552
		public SettingsPage()
		{
			base.DataContext = this;
			this.InitializeComponent();
			SettingsPage.StartAppButtonStatic = this.StartAppButton;
			this.InitializeActions();
			SettingsPage.InitializeSubPages();
			SettingsPage.SettingsPageInstance = this;
		}

		// Token: 0x060003FE RID: 1022 RVA: 0x0001638C File Offset: 0x0001458C
		public bool ErrorControl()
		{
			bool isMailActive = Settings.GetValueBoolean("mailenable").GetValueOrDefault();
			bool isPrintActive = Settings.GetValueBoolean("printenable").GetValueOrDefault();
			bool isSMSActive = Settings.GetValueBoolean("smsenable").GetValueOrDefault();
			bool isGreenBox = Settings.GetValueBoolean("greenbox").GetValueOrDefault();
			bool isPhotoActive = Settings.GetValueBoolean("photo").GetValueOrDefault();
			bool isVideoActive = Settings.GetValueBoolean("video").GetValueOrDefault();
			bool isPaymentActiveButPrintDeactive = !Settings.GetValueBoolean("printenable").Value && StripeController.PaymentPrintActive;
			bool isHaveError = false;
			if (EventManagementPage.GetCurrentEvent() == null)
			{
				MessageBoxWindow.CreateWindow("Create an Event", "Please start an event first!", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return true;
			}
			if (string.IsNullOrEmpty(EventManagementPage.GetCurrentEvent().EventHash))
			{
				MessageBoxWindow.CreateWindow("Event Upload Cloud ", "Please wait for Event Information to be Uploaded to the Cloud", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				isHaveError = true;
			}
			if ((Settings.GetValueBoolean("aipromptoptionactive").Value || Settings.GetValueBoolean("faceswapactive").Value || Settings.GetValueBoolean("wordportraitoptionactive").Value) && TemplateClass.TemplateObjectsPhoto.Count != 0)
			{
				if (TemplateClass.TemplateObjectsPhoto.Max((TemplateObject x) => x.photoID) > 0)
				{
					MessageBoxWindow.CreateWindow("Template Warning", "Only one photo is allowed in AI modes. Please edit the template page, Do you want to navigate to Template Settings page", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Yes,
						MessageBoxWindow.ButtonType.No
					}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToTemplatePage), MessageBoxWindow.MessageBoxSize.Large, false);
					isHaveError = true;
				}
			}
			if (SessionData.RunOnStartup)
			{
				MainSettingsPage.selectedCameraType = Settings.GetValueEnum<MainSettingsPage.CameraType>("cameratype");
			}
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR && CameraControlClass.DeviceManager != null && CameraControlClass.DeviceManager.SelectedCameraDevice.IsBusy)
			{
				MessageBoxWindow.CreateWindow("Camera Busy ", "Camera is being used by another app, Please select an available camera\nApplication restart may be required", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				isHaveError = true;
			}
			if (isMailActive)
			{
				string mailaddress = Settings.GetValueString("mail_address");
				string mailpass = Settings.GetValueString("mail_password");
				if (string.IsNullOrEmpty(mailaddress) || string.IsNullOrEmpty(mailpass) || mailaddress.Length < 3 || mailpass.Length < 3)
				{
					MessageBoxWindow.CreateWindow("Email Warning", "Email is active but email information is missing. Would you like to check email information? Do you want to navigate to Sharing Settings page", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Yes,
						MessageBoxWindow.ButtonType.No
					}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToSharingPage), MessageBoxWindow.MessageBoxSize.Large, false);
					isHaveError = true;
				}
			}
			if (isPaymentActiveButPrintDeactive)
			{
				MessageBoxWindow.CreateWindow("Payment Active But Printer isn't Active", "Print isn't active. Would you like to check print information? Do you want to navigate to Sharing Settings page", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Yes,
					MessageBoxWindow.ButtonType.No
				}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToPrinterPage), MessageBoxWindow.MessageBoxSize.Large, false);
				isHaveError = true;
			}
			if (isPrintActive)
			{
				string printerName = Settings.GetValueString("printer_name");
				if (string.IsNullOrEmpty(printerName))
				{
					MessageBoxWindow.CreateWindow("Print Warning", "Print is active but printer is missing. Would you like to check print information? Do you want to navigate to Sharing Settings page", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Yes,
						MessageBoxWindow.ButtonType.No
					}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToPrinterPage), MessageBoxWindow.MessageBoxSize.Large, false);
					isHaveError = true;
				}
			}
			if (isSMSActive)
			{
				string smsauth = Settings.GetValueString("smsauthtoken");
				string smsid = Settings.GetValueString("smssid");
				string smshost = Settings.GetValueString("smshost");
				if (string.IsNullOrEmpty(smsauth) || string.IsNullOrEmpty(smsid) || string.IsNullOrEmpty(smshost) || smsauth.Length < 3 || smsid.Length < 3 || smshost.Length < 3)
				{
					MessageBoxWindow.CreateWindow("SMS Warning", "SMS is active but sms information is missing. Would you like to check sms information? Do you want to navigate to Sharing Settings page", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Yes,
						MessageBoxWindow.ButtonType.No
					}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToSharingPage), MessageBoxWindow.MessageBoxSize.Large, false);
					isHaveError = true;
				}
			}
			if (isGreenBox && ((TemplateClass.NoBackground[0] && isPhotoActive) || (TemplateClass.NoBackground[1] && isVideoActive)))
			{
				MessageBoxWindow.CreateWindow("ChromaKey Warning", "ChromaKey is enabled but the Template has no background. If you don't add background chromakey it won't work.  Do you want to navigate to Template Settings page", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Yes,
					MessageBoxWindow.ButtonType.No
				}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToTemplatePage), MessageBoxWindow.MessageBoxSize.Large, false);
				isHaveError = true;
			}
			if (Settings.GetValueBoolean("faceswapactive").Value && !FaceSwap.IsLoadAllFaceSwap())
			{
				MessageBoxWindow.CreateWindow("AIFaceSwap Warning", "Some Photos are not uploaded to the system, Navigate to FaceSwap Gallery wait all images to be uploaded and then start the app.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToFaceSwapPage), MessageBoxWindow.MessageBoxSize.Large, false);
				isHaveError = true;
			}
			return isHaveError;
		}

		// Token: 0x060003FF RID: 1023 RVA: 0x000167BC File Offset: 0x000149BC
		public void StartAppFunction()
		{
			SettingsPage.<StartAppFunction>d__37 <StartAppFunction>d__;
			<StartAppFunction>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StartAppFunction>d__.<>1__state = -1;
			<StartAppFunction>d__.<>t__builder.Start<SettingsPage.<StartAppFunction>d__37>(ref <StartAppFunction>d__);
		}

		// Token: 0x06000400 RID: 1024 RVA: 0x000167EC File Offset: 0x000149EC
		public void StartAppButton_Click(object sender, RoutedEventArgs e)
		{
			SettingsPage.<StartAppButton_Click>d__38 <StartAppButton_Click>d__;
			<StartAppButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<StartAppButton_Click>d__.<>4__this = this;
			<StartAppButton_Click>d__.<>1__state = -1;
			<StartAppButton_Click>d__.<>t__builder.Start<SettingsPage.<StartAppButton_Click>d__38>(ref <StartAppButton_Click>d__);
		}

		// Token: 0x06000401 RID: 1025 RVA: 0x00016823 File Offset: 0x00014A23
		private void GoToSharingPage(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			if (messageBoxReturn == MessageBoxWindow.MessageBoxReturn.Yes)
			{
				SettingsPage.SettingsPageInstance.SetSubPage(SettingsSubPageEnum.SharingPage);
			}
		}

		// Token: 0x06000402 RID: 1026 RVA: 0x00016834 File Offset: 0x00014A34
		private void GoToPrinterPage(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			if (messageBoxReturn == MessageBoxWindow.MessageBoxReturn.Yes)
			{
				SettingsPage.SettingsPageInstance.SetSubPage(SettingsSubPageEnum.PrinterPage);
			}
		}

		// Token: 0x06000403 RID: 1027 RVA: 0x00016846 File Offset: 0x00014A46
		private void GoToTemplatePage(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			if (messageBoxReturn == MessageBoxWindow.MessageBoxReturn.Yes)
			{
				SettingsPage.SettingsPageInstance.SetSubPage(SettingsSubPageEnum.TemplatePage);
			}
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x00016857 File Offset: 0x00014A57
		private void GoToFaceSwapPage(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			if (messageBoxReturn == MessageBoxWindow.MessageBoxReturn.Continue)
			{
				SettingsPage.SettingsPageInstance.SetSubPage(SettingsSubPageEnum.FaceSwapPage);
			}
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x00016868 File Offset: 0x00014A68
		private void StartSalesWebsite(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			try
			{
				Process.Start(AppInfo.AppClass.InfoLink);
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x0001689C File Offset: 0x00014A9C
		private void QuitButton_Click(object sender, RoutedEventArgs e)
		{
			if (EventManagementPage.GetCurrentEvent() != null)
			{
				TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasID);
				TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasVideoID);
			}
			App.ExitApp();
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x000168C0 File Offset: 0x00014AC0
		private void EventManagementPageArchiveEventClicked()
		{
			for (int i = 2; i < this.LeftMenuButtonsList.Length; i++)
			{
				this.LeftMenuButtonsList[i].Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x000168EE File Offset: 0x00014AEE
		public void HideOrOpenButtons()
		{
			Application.Current.Dispatcher.Invoke(delegate()
			{
				this.MainSettingsButton.Visibility = ((EventManagementPage.GetCurrentEvent() != null) ? Visibility.Visible : Visibility.Collapsed);
				this.LanguageButton.Visibility = ((EventManagementPage.GetCurrentEvent() != null) ? Visibility.Visible : Visibility.Collapsed);
				this.SharingButton.Visibility = ((EventManagementPage.GetCurrentEvent() != null) ? Visibility.Visible : Visibility.Collapsed);
				this.ThemeButton.Visibility = ((EventManagementPage.GetCurrentEvent() != null) ? Visibility.Visible : Visibility.Collapsed);
				this.PrinterButton.Visibility = ((EventManagementPage.GetCurrentEvent() != null) ? Visibility.Visible : Visibility.Collapsed);
				this.TemplateButton.Visibility = (Settings.ElementsVisibilities.ImageTemplateMenuButtonVisibility ? Visibility.Visible : Visibility.Collapsed);
				this.TemplateVideoButton.Visibility = (Settings.ElementsVisibilities.VideoTemplateMenuButtonVisibility ? Visibility.Visible : Visibility.Collapsed);
				this.FaceSwapButton.Visibility = (Settings.ElementsVisibilities.FaceSwapTargetGalleryMenuButtonVisibility ? Visibility.Visible : Visibility.Collapsed);
				this.SamplePromptsPageButton.Visibility = (Settings.ElementsVisibilities.AiPromptGalleryMenuButtonVisibility ? Visibility.Visible : Visibility.Collapsed);
				this.WordPortrePageButton.Visibility = (Settings.ElementsVisibilities.WordPortraitMenuButtonVisibility ? Visibility.Visible : Visibility.Collapsed);
				this.AIEffectPageButton.Visibility = (Settings.ElementsVisibilities.AiEffectActiveFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
				ValueTuple<bool, string> startTheAppButtonActive = Settings.ElementsVisibilities.StartTheAppButtonActive;
				bool isStartAppButtonActive = startTheAppButtonActive.Item1;
				string message = startTheAppButtonActive.Item2;
				this.StartAppButton.IsEnabled = isStartAppButtonActive;
				this.StartAppButton.ToolTip = message;
			});
		}

		// Token: 0x17000074 RID: 116
		// (get) Token: 0x06000409 RID: 1033 RVA: 0x0001690C File Offset: 0x00014B0C
		private bool IsVideoPageActive
		{
			get
			{
				return Settings.GetValueBoolean("video").Value && !Settings.GetValueBoolean("wordportraitoptionactive").Value;
			}
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x00016944 File Offset: 0x00014B44
		private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
		{
			SettingsPage.StopUnnecessaryThreads();
			MessageBoxWindow.StartCloseMessageBoxWithTimeThread(AppInfo.AppClass.AppDefaultSettings.WaitMessageTime);
			TemplateClass.isTemplateOpen = false;
			this.LeftMenuButtonsList = new Button[]
			{
				this.DashboardButton,
				this.EventManagementButton,
				this.MainSettingsButton,
				this.LanguageButton,
				this.SharingButton,
				this.TemplateButton,
				this.ThemeButton,
				this.TemplateVideoButton,
				this.FaceSwapButton,
				this.SamplePromptsPageButton,
				this.WordPortrePageButton,
				this.AIEffectPageButton,
				this.PrinterButton
			};
			if (EventManagementPage.GetCurrentEvent() == null)
			{
				for (int i = 0; i < this.LeftMenuButtonsList.Length; i++)
				{
					if (i < 2)
					{
						this.LeftMenuButtonsList[i].Visibility = Visibility.Visible;
					}
					else
					{
						this.LeftMenuButtonsList[i].Visibility = Visibility.Collapsed;
					}
				}
			}
			else
			{
				this.HideOrOpenButtons();
			}
			this.AppName.Text = AppInfo.AppClass.DisplayName;
			this.AppProperties.Text = AppInfo.AppClass.Description;
			this.AppVersion.Text = string.Format(string.Format("{0} version {1}\n{2} Activation King - All rights reserved", AppInfo.AppClass.DisplayName, Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now.Year), Array.Empty<object>());
			Application.Current.Resources["AppUIColor"] = new SolidColorBrush(AppInfo.ThemeColor);
			Application.Current.Resources["AppUIColorLight"] = new SolidColorBrush(new Color
			{
				R = AppInfo.ThemeColor.R + (byte.MaxValue - AppInfo.ThemeColor.R) / 4,
				G = AppInfo.ThemeColor.G + (byte.MaxValue - AppInfo.ThemeColor.G) / 4,
				B = AppInfo.ThemeColor.B + (byte.MaxValue - AppInfo.ThemeColor.B) / 4,
				A = byte.MaxValue
			});
			if (SessionData.accountInfo.isTesterUser)
			{
				this.UpdateTesterGrid.Visibility = (AppVersionControl.CheckUpdate(AppInfo.AppClass.LatestTesterVersion) ? Visibility.Visible : Visibility.Collapsed);
			}
			else
			{
				this.UpdateTesterGrid.Visibility = Visibility.Collapsed;
			}
			this.UpdateGrid.Visibility = (AppVersionControl.CheckUpdate(AppInfo.AppClass.LatestVersion) ? Visibility.Visible : Visibility.Collapsed);
			if (EventManagementPage.GetCurrentEvent() == null)
			{
				this.SetSubPage(SettingsSubPageEnum.DashboardPage);
			}
			else
			{
				this.SetSubPage(SettingsSubPageEnum.MainSettingsPage);
			}
			this.StartTimer();
			Action settingsPageLoaded = SettingsPage.SettingsPageLoaded;
			if (settingsPageLoaded == null)
			{
				return;
			}
			settingsPageLoaded();
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00016C01 File Offset: 0x00014E01
		private void ActivateEventButtonBlink()
		{
			Task.Run(delegate()
			{
				SettingsPage.<<ActivateEventButtonBlink>b__50_0>d <<ActivateEventButtonBlink>b__50_0>d;
				<<ActivateEventButtonBlink>b__50_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ActivateEventButtonBlink>b__50_0>d.<>4__this = this;
				<<ActivateEventButtonBlink>b__50_0>d.<>1__state = -1;
				<<ActivateEventButtonBlink>b__50_0>d.<>t__builder.Start<SettingsPage.<<ActivateEventButtonBlink>b__50_0>d>(ref <<ActivateEventButtonBlink>b__50_0>d);
				return <<ActivateEventButtonBlink>b__50_0>d.<>t__builder.Task;
			});
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x00016C18 File Offset: 0x00014E18
		private void StartTimer()
		{
			this.licenceCheckTimer = new System.Timers.Timer(5000.0);
			this.licenceCheckTimer.Elapsed += this.OnTimedEvent;
			this.licenceCheckTimer.AutoReset = true;
			this.licenceCheckTimer.Enabled = true;
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00016C68 File Offset: 0x00014E68
		private void OnTimedEvent(object source, ElapsedEventArgs e)
		{
			new Thread(delegate()
			{
				SettingsPage.<<OnTimedEvent>b__53_0>d <<OnTimedEvent>b__53_0>d;
				<<OnTimedEvent>b__53_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<OnTimedEvent>b__53_0>d.<>4__this = this;
				<<OnTimedEvent>b__53_0>d.<>1__state = -1;
				<<OnTimedEvent>b__53_0>d.<>t__builder.Start<SettingsPage.<<OnTimedEvent>b__53_0>d>(ref <<OnTimedEvent>b__53_0>d);
			}).Start();
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00016C80 File Offset: 0x00014E80
		private void Logout()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				ActivationKingWindow.SetPage(ApplicationPage.LoginPage, true);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x0600040F RID: 1039 RVA: 0x00016CC0 File Offset: 0x00014EC0
		private static void InitializeSubPages()
		{
			if (SettingsPage.dashboardPage == null)
			{
				SettingsPage.dashboardPage = new DashboardPage();
				SettingsPage.eventManagementPage = new EventManagementPage();
				SettingsPage.mainSettingsPage = new MainSettingsPage();
				SettingsPage.themePage = new ThemePage();
				SettingsPage.sharingPage = new SharingPage();
				SettingsPage.languagePage = new LanguagePage();
				SettingsPage.templatePage = new TemplatePage();
				SettingsPage.templatePageVideo = new TemplatePageVideo();
				SettingsPage.faceswapPage = new FaceSwap();
				SettingsPage.samplePromptsPage = new SamplePromptsPage();
				SettingsPage.wordCloudPage = new WordCloudPage();
				SettingsPage.aiEffectPage = new AIEffectPage();
				SettingsPage.printerPage = new PrinterPage();
			}
		}

		// Token: 0x06000410 RID: 1040 RVA: 0x00016D5C File Offset: 0x00014F5C
		private void InitializeActions()
		{
			if (this.firstWork)
			{
				GoProCameraControlClass.returnWifiConnected += this.GoProCameraControlClass_returnWifiConnected;
				GoProCameraControlClass.scanCompleted += this.GoProCameraControlClass_scanCompleted;
				CameraControlClass.CameraConnectLoadCompleted += this.CameraControlClass_CameraConnectLoadCompleted;
				EventManagementPage.onEventChanged += this.EventManagementPage_onEventChanged;
				EventManagementPage.archiveEventAction += this.EventManagementPageArchiveEventClicked;
				MainSettingsPage.VideoGifCheckChanged += this.MainSettingsPage_VideoGifCheckChanged;
				this.firstWork = false;
			}
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x00016DE0 File Offset: 0x00014FE0
		private static void StopUnnecessaryThreads()
		{
			BunnyCDNHelper.isForceQuit = true;
			MailClass.isForceQuit = true;
			TwilioHelper.isForceQuit = true;
			VariationMediaHelper.isForceQuit = true;
			if (BunnyCDNHelper.UploadThread != null)
			{
				BunnyCDNHelper.UploadThread.Abort();
				BunnyCDNHelper.UploadThread = null;
			}
			if (MailClass.sendThread != null)
			{
				MailClass.sendThread.Abort();
				MailClass.sendThread = null;
			}
			if (TwilioHelper.sendThread != null)
			{
				TwilioHelper.sendThread.Abort();
				TwilioHelper.sendThread = null;
			}
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x00016E4A File Offset: 0x0001504A
		private void MainSettingsPage_VideoGifCheckChanged(bool isVisible)
		{
			this.TemplateVideoButton.Visibility = (isVisible ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x00016E60 File Offset: 0x00015060
		public void GoProCameraControlClass_returnWifiConnected(bool obj, bool obj2)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.HideOrOpenButtons();
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x00016E8C File Offset: 0x0001508C
		public void GoProCameraControlClass_scanCompleted(bool obj)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.HideOrOpenButtons();
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x00016EB8 File Offset: 0x000150B8
		private void CameraControlClass_CameraConnectLoadCompleted()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.HideOrOpenButtons();
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x00016EE4 File Offset: 0x000150E4
		private void EventManagementPage_onEventChanged(OperationalEvent newEvent)
		{
			Settings.OnEventChanged(newEvent);
			TemplateClass.RefreshTemplateData();
			if (File.Exists(EventManagementPage.GetCurrentEvent().EventMediaJsonPath))
			{
				DSLR.eventMediaClass = ExtensionMethod.ReadJson<EventMediaClass>(EventManagementPage.GetCurrentEvent().EventMediaJsonPath);
			}
			else if (DSLR.eventMediaClass != null)
			{
				DSLR.eventMediaClass.mediaList = new List<MediaClassBase>();
			}
			if (SettingsPage.isFirstEvent)
			{
				new Thread(delegate()
				{
					SettingsPage.isFirstEvent = false;
					Thread.Sleep(50);
					Application.Current.Dispatcher.Invoke(delegate()
					{
						this.SetSubPage(SettingsSubPageEnum.MainSettingsPage);
					});
				}).Start();
			}
			this.HideOrOpenButtons();
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00016F5C File Offset: 0x0001515C
		private void PageChange_Click(object sender, RoutedEventArgs e)
		{
			Button refButton = sender as Button;
			int UID = Convert.ToInt32(refButton.Uid);
			SettingsSubPageEnum selectedSubPage = (SettingsSubPageEnum)UID;
			if (SettingsPage.CurrentSubPage != selectedSubPage)
			{
				this.SetSubPage(selectedSubPage);
			}
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00016F90 File Offset: 0x00015190
		private void PageChangeButton_MouseEnter(object sender, MouseEventArgs e)
		{
			Button refButton = sender as Button;
			int UID = Convert.ToInt32(refButton.Uid);
			for (int i = 0; i < this.LeftMenuButtonsList.Length; i++)
			{
				if (SettingsPage.CurrentSubPage != (SettingsSubPageEnum)i)
				{
					this.LeftMenuButtonsList[i].Background.Opacity = (double)((UID != i) ? 0f : 0.2f);
				}
			}
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00016FF0 File Offset: 0x000151F0
		private void PageChangeButton_MouseLeave(object sender, MouseEventArgs e)
		{
			Button refButton = sender as Button;
			int UID = Convert.ToInt32(refButton.Uid);
			for (int i = 0; i < this.LeftMenuButtonsList.Length; i++)
			{
				if (SettingsPage.CurrentSubPage != (SettingsSubPageEnum)i)
				{
					this.LeftMenuButtonsList[i].Background.Opacity = 0.0;
				}
			}
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x00017048 File Offset: 0x00015248
		private void ProductSupportButtonRight_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(AppInfo.AppClass.SupportLink);
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x0001707C File Offset: 0x0001527C
		private void ActivationKingButtonRight_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start("https://activationking.com");
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x000170AC File Offset: 0x000152AC
		private void UpdateButton_Click(object sender, RoutedEventArgs e)
		{
			UpdateWindow yeni = new UpdateWindow(false);
			yeni.Show();
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x000170C8 File Offset: 0x000152C8
		private void btnUpdateTester_Click(object sender, RoutedEventArgs e)
		{
			UpdateWindow yeni = new UpdateWindow(true);
			yeni.Show();
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x000170E2 File Offset: 0x000152E2
		private void pageSettings_SizeChanged(object sender, SizeChangedEventArgs e)
		{
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x000170E4 File Offset: 0x000152E4
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRation = base.ActualWidth / 3840.0;
		}

		// Token: 0x06000420 RID: 1056 RVA: 0x00017113 File Offset: 0x00015313
		private void pageSettings_Unloaded(object sender, RoutedEventArgs e)
		{
			this.licenceCheckTimer.Stop();
			this.licenceCheckTimer.Dispose();
			this.RightPanelPages.Content = null;
		}

		// Token: 0x06000421 RID: 1057 RVA: 0x00017138 File Offset: 0x00015338
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocater = new Uri("/KingAIPhotoBoothPro;component/pages/activationking/settingspage.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}

		// Token: 0x06000422 RID: 1058 RVA: 0x00017168 File Offset: 0x00015368
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.pageSettings = (SettingsPage)target;
				this.pageSettings.Loaded += this.SettingsPage_Loaded;
				this.pageSettings.SizeChanged += this.pageSettings_SizeChanged;
				this.pageSettings.Unloaded += this.pageSettings_Unloaded;
				return;
			case 2:
				this.gridMain = (Grid)target;
				return;
			case 3:
				this.RightPanelGrid = (Grid)target;
				return;
			case 4:
				this.RightPanelPages = (Frame)target;
				return;
			case 5:
				this.Info = (Grid)target;
				return;
			case 6:
				this.txtTitle = (TextBlock)target;
				return;
			case 7:
				this.txtSubTitle = (TextBlock)target;
				return;
			case 8:
				this.ProductSupportButtonRight = (Button)target;
				this.ProductSupportButtonRight.Click += this.ProductSupportButtonRight_Click;
				return;
			case 9:
				this.ActivationKingButtonRight = (Button)target;
				this.ActivationKingButtonRight.Click += this.ActivationKingButtonRight_Click;
				return;
			case 10:
				this.LeftPanelGrid = (Grid)target;
				return;
			case 11:
				this.LeftPanelBackground = (Border)target;
				return;
			case 12:
				this.LeftPanelContentGrid = (Grid)target;
				return;
			case 13:
				this.AppName = (TextBlock)target;
				return;
			case 14:
				this.AppProperties = (TextBlock)target;
				return;
			case 15:
				this.DashboardButton = (Button)target;
				this.DashboardButton.Click += this.PageChange_Click;
				this.DashboardButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.DashboardButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 16:
				this.EventManagementButton = (Button)target;
				this.EventManagementButton.Click += this.PageChange_Click;
				this.EventManagementButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.EventManagementButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 17:
				this.MainSettingsButton = (Button)target;
				this.MainSettingsButton.Click += this.PageChange_Click;
				this.MainSettingsButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.MainSettingsButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 18:
				this.TemplateButton = (Button)target;
				this.TemplateButton.Click += this.PageChange_Click;
				this.TemplateButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.TemplateButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 19:
				this.TemplateVideoButton = (Button)target;
				this.TemplateVideoButton.Click += this.PageChange_Click;
				this.TemplateVideoButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.TemplateVideoButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 20:
				this.LanguageButton = (Button)target;
				this.LanguageButton.Click += this.PageChange_Click;
				this.LanguageButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.LanguageButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 21:
				this.ThemeButton = (Button)target;
				this.ThemeButton.Click += this.PageChange_Click;
				this.ThemeButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.ThemeButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 22:
				this.SharingButton = (Button)target;
				this.SharingButton.Click += this.PageChange_Click;
				this.SharingButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.SharingButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 23:
				this.PrinterButton = (Button)target;
				this.PrinterButton.Click += this.PageChange_Click;
				this.PrinterButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.PrinterButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 24:
				this.SamplePromptsPageButton = (Button)target;
				this.SamplePromptsPageButton.Click += this.PageChange_Click;
				this.SamplePromptsPageButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.SamplePromptsPageButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 25:
				this.FaceSwapButton = (Button)target;
				this.FaceSwapButton.Click += this.PageChange_Click;
				this.FaceSwapButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.FaceSwapButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 26:
				this.WordPortrePageButton = (Button)target;
				this.WordPortrePageButton.Click += this.PageChange_Click;
				this.WordPortrePageButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.WordPortrePageButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 27:
				this.AIEffectPageButton = (Button)target;
				this.AIEffectPageButton.Click += this.PageChange_Click;
				this.AIEffectPageButton.MouseEnter += this.PageChangeButton_MouseEnter;
				this.AIEffectPageButton.MouseLeave += this.PageChangeButton_MouseLeave;
				return;
			case 28:
				this.LeftBottomPanelGrid = (Grid)target;
				return;
			case 29:
				this.StartAppButton = (Button)target;
				this.StartAppButton.Click += this.StartAppButton_Click;
				return;
			case 30:
				this.QuitButton = (Button)target;
				this.QuitButton.Click += this.QuitButton_Click;
				return;
			case 31:
				this.UpdateTesterGrid = (Grid)target;
				return;
			case 32:
				this.btnUpdateTester = (Button)target;
				this.btnUpdateTester.Click += this.btnUpdateTester_Click;
				return;
			case 33:
				this.UpdateGrid = (Grid)target;
				return;
			case 34:
				this.btnUpdate = (Button)target;
				this.btnUpdate.Click += this.UpdateButton_Click;
				return;
			case 35:
				this.AppVersion = (TextBlock)target;
				return;
			case 36:
				this.ProductSupportButton = (Button)target;
				this.ProductSupportButton.Click += this.ProductSupportButtonRight_Click;
				return;
			case 37:
				this.ActivationKingButton = (Button)target;
				this.ActivationKingButton.Click += this.ActivationKingButtonRight_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x040003ED RID: 1005
		public static Action SettingsPageLoaded;

		// Token: 0x040003EE RID: 1006
		private bool firstWork = true;

		// Token: 0x040003EF RID: 1007
		public static bool isFirstEvent = true;

		// Token: 0x040003F0 RID: 1008
		private Button[] LeftMenuButtonsList;

		// Token: 0x040003F1 RID: 1009
		public static SettingsPage SettingsPageInstance = null;

		// Token: 0x040003F2 RID: 1010
		public static DashboardPage dashboardPage = null;

		// Token: 0x040003F3 RID: 1011
		public static EventManagementPage eventManagementPage = null;

		// Token: 0x040003F4 RID: 1012
		public static MainSettingsPage mainSettingsPage = null;

		// Token: 0x040003F5 RID: 1013
		public static ThemePage themePage = null;

		// Token: 0x040003F6 RID: 1014
		public static SharingPage sharingPage = null;

		// Token: 0x040003F7 RID: 1015
		public static LanguagePage languagePage = null;

		// Token: 0x040003F8 RID: 1016
		public static TemplatePage templatePage = null;

		// Token: 0x040003F9 RID: 1017
		public static FaceSwap faceswapPage = null;

		// Token: 0x040003FA RID: 1018
		public static SamplePromptsPage samplePromptsPage = null;

		// Token: 0x040003FB RID: 1019
		public static TemplatePageVideo templatePageVideo = null;

		// Token: 0x040003FC RID: 1020
		public static WordCloudPage wordCloudPage = null;

		// Token: 0x040003FD RID: 1021
		public static AIEffectPage aiEffectPage = null;

		// Token: 0x040003FE RID: 1022
		public static PrinterPage printerPage = null;

		// Token: 0x040003FF RID: 1023
		public static bool isTurnedBack = false;

		// Token: 0x04000400 RID: 1024
		public static SettingsSubPageEnum CurrentSubPage;

		// Token: 0x04000401 RID: 1025
		private static Button StartAppButtonStatic;

		// Token: 0x04000402 RID: 1026
		private SolidColorBrush seletedMenuButtonTextBrush;

		// Token: 0x04000403 RID: 1027
		private System.Timers.Timer licenceCheckTimer;

		// Token: 0x04000404 RID: 1028
		internal SettingsPage pageSettings;

		// Token: 0x04000405 RID: 1029
		internal Grid gridMain;

		// Token: 0x04000406 RID: 1030
		internal Grid RightPanelGrid;

		// Token: 0x04000407 RID: 1031
		internal Frame RightPanelPages;

		// Token: 0x04000408 RID: 1032
		internal Grid Info;

		// Token: 0x04000409 RID: 1033
		internal TextBlock txtTitle;

		// Token: 0x0400040A RID: 1034
		internal TextBlock txtSubTitle;

		// Token: 0x0400040B RID: 1035
		internal Button ProductSupportButtonRight;

		// Token: 0x0400040C RID: 1036
		internal Button ActivationKingButtonRight;

		// Token: 0x0400040D RID: 1037
		internal Grid LeftPanelGrid;

		// Token: 0x0400040E RID: 1038
		internal Border LeftPanelBackground;

		// Token: 0x0400040F RID: 1039
		internal Grid LeftPanelContentGrid;

		// Token: 0x04000410 RID: 1040
		internal TextBlock AppName;

		// Token: 0x04000411 RID: 1041
		internal TextBlock AppProperties;

		// Token: 0x04000412 RID: 1042
		internal Button DashboardButton;

		// Token: 0x04000413 RID: 1043
		internal Button EventManagementButton;

		// Token: 0x04000414 RID: 1044
		internal Button MainSettingsButton;

		// Token: 0x04000415 RID: 1045
		internal Button TemplateButton;

		// Token: 0x04000416 RID: 1046
		internal Button TemplateVideoButton;

		// Token: 0x04000417 RID: 1047
		internal Button LanguageButton;

		// Token: 0x04000418 RID: 1048
		internal Button ThemeButton;

		// Token: 0x04000419 RID: 1049
		internal Button SharingButton;

		// Token: 0x0400041A RID: 1050
		internal Button PrinterButton;

		// Token: 0x0400041B RID: 1051
		internal Button SamplePromptsPageButton;

		// Token: 0x0400041C RID: 1052
		internal Button FaceSwapButton;

		// Token: 0x0400041D RID: 1053
		internal Button WordPortrePageButton;

		// Token: 0x0400041E RID: 1054
		internal Button AIEffectPageButton;

		// Token: 0x0400041F RID: 1055
		internal Grid LeftBottomPanelGrid;

		// Token: 0x04000420 RID: 1056
		internal Button StartAppButton;

		// Token: 0x04000421 RID: 1057
		internal Button QuitButton;

		// Token: 0x04000422 RID: 1058
		internal Grid UpdateTesterGrid;

		// Token: 0x04000423 RID: 1059
		internal Button btnUpdateTester;

		// Token: 0x04000424 RID: 1060
		internal Grid UpdateGrid;

		// Token: 0x04000425 RID: 1061
		internal Button btnUpdate;

		// Token: 0x04000426 RID: 1062
		internal TextBlock AppVersion;

		// Token: 0x04000427 RID: 1063
		internal Button ProductSupportButton;

		// Token: 0x04000428 RID: 1064
		internal Button ActivationKingButton;

		// Token: 0x04000429 RID: 1065
		private bool _contentLoaded;
	}
}
