using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using LottieSharp.WPF;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200003A RID: 58
	public class Login : Page, IComponentConnector
	{
		// Token: 0x060003AF RID: 943 RVA: 0x000145B3 File Offset: 0x000127B3
		public Login()
		{
			this.InitializeComponent();
		}

		// Token: 0x060003B0 RID: 944 RVA: 0x000145C4 File Offset: 0x000127C4
		private void Login_Loaded(object sender, RoutedEventArgs e)
		{
			Login.<Login_Loaded>d__5 <Login_Loaded>d__;
			<Login_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Login_Loaded>d__.<>4__this = this;
			<Login_Loaded>d__.<>1__state = -1;
			<Login_Loaded>d__.<>t__builder.Start<Login.<Login_Loaded>d__5>(ref <Login_Loaded>d__);
		}

		// Token: 0x060003B1 RID: 945 RVA: 0x000145FC File Offset: 0x000127FC
		private void LoadRunOnStartup()
		{
			string runOnStartupFilePath = Path.Combine(SessionData.ApplicationDataFolderPath, "data4.bin");
			if (File.Exists(runOnStartupFilePath))
			{
				string encryptString = File.ReadAllText(runOnStartupFilePath);
				encryptString = EncryptString.Decrypt(encryptString, "runonstartup");
				try
				{
					SessionData.RunOnStartup = bool.Parse(encryptString);
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x060003B2 RID: 946 RVA: 0x00014654 File Offset: 0x00012854
		private Task SetAppInfo()
		{
			Login.<SetAppInfo>d__7 <SetAppInfo>d__;
			<SetAppInfo>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SetAppInfo>d__.<>4__this = this;
			<SetAppInfo>d__.<>1__state = -1;
			<SetAppInfo>d__.<>t__builder.Start<Login.<SetAppInfo>d__7>(ref <SetAppInfo>d__);
			return <SetAppInfo>d__.<>t__builder.Task;
		}

		// Token: 0x060003B3 RID: 947 RVA: 0x00014698 File Offset: 0x00012898
		private Task<bool> GetAppIfo()
		{
			Login.<GetAppIfo>d__8 <GetAppIfo>d__;
			<GetAppIfo>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<GetAppIfo>d__.<>4__this = this;
			<GetAppIfo>d__.<>1__state = -1;
			<GetAppIfo>d__.<>t__builder.Start<Login.<GetAppIfo>d__8>(ref <GetAppIfo>d__);
			return <GetAppIfo>d__.<>t__builder.Task;
		}

		// Token: 0x060003B4 RID: 948 RVA: 0x000146DC File Offset: 0x000128DC
		private void ManageScreenOrientation()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRatio = base.ActualWidth / 3840.0;
			if (isPortraitScreen)
			{
				this.gridLayout.RowDefinitions[0].Height = new GridLength(250.0, GridUnitType.Star);
				this.gridLayout.RowDefinitions[this.gridLayout.RowDefinitions.Count - 1].Height = new GridLength(250.0, GridUnitType.Star);
				return;
			}
			this.gridLayout.RowDefinitions[0].Height = new GridLength(43.0, GridUnitType.Star);
			this.gridLayout.RowDefinitions[this.gridLayout.RowDefinitions.Count - 1].Height = new GridLength(43.0, GridUnitType.Star);
		}

		// Token: 0x060003B5 RID: 949 RVA: 0x000147C4 File Offset: 0x000129C4
		private void SetAppColorToElements()
		{
			SolidColorBrush appDarkColorBrush = new SolidColorBrush(new Color
			{
				R = AppInfo.ThemeColor.R * 3 / 4,
				G = AppInfo.ThemeColor.G * 3 / 4,
				B = AppInfo.ThemeColor.B * 3 / 4,
				A = AppInfo.ThemeColor.A
			});
			SolidColorBrush appColorBrush = new SolidColorBrush(AppInfo.ThemeColor);
			this.SignText.Foreground = appColorBrush;
			this.StartTrialText.Foreground = appColorBrush;
			this.ForgotText.Foreground = appColorBrush;
			this.SignButton.Background = appColorBrush;
			this.StartButtonTrial.Background = appColorBrush;
			this.StartButtonForgot.Background = appColorBrush;
			this.CancelButtonTrial.Background = appColorBrush;
			this.CancelButtonTrial.Foreground = appColorBrush;
			this.CancelButtonForgot.Foreground = appColorBrush;
			this.CancelButtonForgot.Background = appColorBrush;
			this.SubscribeNowButton.Background = appColorBrush;
			this.SubscribeNowButton.Foreground = appColorBrush;
			this.SpanGetCode.Foreground = appColorBrush;
			this.BorderLoadingBG.Background = appColorBrush;
			this.leftPanelCover.Background = appColorBrush;
			this.PopUPImage.Background = appColorBrush;
			this.MoreInfoTxt.Foreground = appColorBrush;
			this.supportTxt.Foreground = appColorBrush;
			this.NoButton.Foreground = appColorBrush;
			this.YesButton.Foreground = appColorBrush;
			this.btnUpdate.Foreground = appColorBrush;
			this.btnUpdate.BorderBrush = appColorBrush;
		}

		// Token: 0x060003B6 RID: 950 RVA: 0x00014954 File Offset: 0x00012B54
		private void LicenceLoginPostCompleted(bool success, string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					MessageBoxWindow.CreateWindow("Server Error", "Please contact with the developers of Activation King", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
					this.LoadingObject.Visibility = Visibility.Collapsed;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			if (!success)
			{
				this.LoadingObject.Visibility = Visibility.Collapsed;
				global::Debug.Log("Login Success False", message, "LicenceLoginPostCompleted", 278);
				return;
			}
			JsonSerializerSettings settings = new JsonSerializerSettings
			{
				DateTimeZoneHandling = DateTimeZoneHandling.Utc,
				DateParseHandling = DateParseHandling.DateTime,
				Culture = CultureInfo.InvariantCulture
			};
			LicenceLoginResponse response = JsonConvert.DeserializeObject<LicenceLoginResponse>(message, settings);
			if (response.LicenceValid)
			{
				Login.expressionTime = response.GetExpirationTimeString();
				this.SaveTokenInfo(response, Login.email);
				this.InfoLabel.Text = "Starting application...";
				Task.Run(delegate()
				{
					Login.<<LicenceLoginPostCompleted>b__11_1>d <<LicenceLoginPostCompleted>b__11_1>d;
					<<LicenceLoginPostCompleted>b__11_1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<LicenceLoginPostCompleted>b__11_1>d.<>4__this = this;
					<<LicenceLoginPostCompleted>b__11_1>d.<>1__state = -1;
					<<LicenceLoginPostCompleted>b__11_1>d.<>t__builder.Start<Login.<<LicenceLoginPostCompleted>b__11_1>d>(ref <<LicenceLoginPostCompleted>b__11_1>d);
					return <<LicenceLoginPostCompleted>b__11_1>d.<>t__builder.Task;
				});
				return;
			}
			base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LoadingObject.Visibility = Visibility.Collapsed;
				this.debugText.Content = response.Message;
				TimedAction.ExecuteWithDelay(delegate
				{
					this.debugText.Content = "";
				}, new TimeSpan(0, 0, 5));
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060003B7 RID: 951 RVA: 0x00014A5C File Offset: 0x00012C5C
		private void AppInfoPostCompleted(bool success, string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					MessageBoxWindow.CreateWindow("Server Error", "Please contact with the developers of Activation King", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
					this.LoadingObject.Visibility = Visibility.Collapsed;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			if (success)
			{
				AppInfo.SetAppInfoWithJsonString(message);
				this.SaveAppInfo(message);
				this.TokenLogin();
				DispatcherOperation op2 = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.SetupAppInfo();
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			DispatcherOperation op3 = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LoadingObject.Visibility = Visibility.Visible;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060003B8 RID: 952 RVA: 0x00014AE8 File Offset: 0x00012CE8
		private bool IsTokenValid()
		{
			string AccountInfoPath = Path.Combine(SessionData.ApplicationDataFolderPath, "data.bin");
			string appInfoFilePath = Path.Combine(SessionData.ApplicationDataFolderPath, "data2.bin");
			string tokenFilePath = Path.Combine(SessionData.ApplicationDataFolderPath, "data3.bin");
			if (File.Exists(AccountInfoPath))
			{
				string encryptString = File.ReadAllText(AccountInfoPath);
				encryptString = EncryptString.Decrypt(encryptString, "accessToken");
				try
				{
					SessionData.accountInfo = JsonConvert.DeserializeObject<AccountInfo>(encryptString);
					Login.email = SessionData.accountInfo.email;
				}
				catch (Exception)
				{
				}
			}
			if (File.Exists(appInfoFilePath))
			{
				try
				{
					string appInfo = File.ReadAllText(appInfoFilePath);
					App app = JsonConvert.DeserializeObject<App>(EncryptString.Decrypt(appInfo, "appInfo"));
					AppInfo.SetAppInfoWithClass(app);
				}
				catch (Exception)
				{
				}
			}
			if (File.Exists(tokenFilePath))
			{
				try
				{
					string appTokenString = File.ReadAllText(tokenFilePath);
					AppToken token = JsonConvert.DeserializeObject<AppToken>(EncryptString.Decrypt(appTokenString, "NoInternetToken"));
					if ((DateTime.ParseExact(token.TokenExpiryDate, Config.DatetimeStringFormat, CultureInfo.InvariantCulture) - DateTime.Now).Days > 0)
					{
						this.NoInternetToken = token;
						return true;
					}
				}
				catch (Exception)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x00014C20 File Offset: 0x00012E20
		public void SetupAppInfo()
		{
			this.cubeImage.Source = BmsEngine.ChangeColorImage(AppInfo.ThemeColor, (BitmapSource)this.cubeImage.Source);
			this.speechImage.Source = BmsEngine.ChangeColorImage(AppInfo.ThemeColor, (BitmapSource)this.speechImage.Source);
			this.ExitButtonBrush.ImageSource = BmsEngine.ChangeColorImage(AppInfo.ThemeColor, (BitmapSource)this.ExitButtonBrush.ImageSource);
			this.PopUpTitle.Content = AppInfo.AppClass.DisplayName;
			this.TxtLoadingTitle.Content = AppInfo.AppClass.DisplayName;
			this.TitleApp.Text = AppInfo.AppClass.DisplayName;
			this.TitleAppDesc.Text = AppInfo.AppClass.Description;
			this.TitleAppVersion.Text = string.Format("v{0}", Assembly.GetExecutingAssembly().GetName().Version);
		}

		// Token: 0x060003BA RID: 954 RVA: 0x00014D14 File Offset: 0x00012F14
		public void SaveTokenInfo(LicenceLoginResponse licence, string mail)
		{
			SessionData.accountInfo = new AccountInfo
			{
				accessToken = licence.AccessToken,
				userHash = licence.UserHash,
				email = Login.email,
				isTrial = licence.IsTrial,
				username = licence.AccountUsername,
				isTesterUser = licence.IsTesterUser
			};
			string filePath = Path.Combine(SessionData.ApplicationDataFolderPath, "data.bin");
			string filePath2 = Path.Combine(SessionData.ApplicationDataFolderPath, "data3.bin");
			string saveData = EncryptString.Encrypt(JsonConvert.SerializeObject(SessionData.accountInfo), "accessToken");
			string NoInternetSaveData = EncryptString.Encrypt(JsonConvert.SerializeObject(new AppToken
			{
				ExpirationDate = licence.ExpirationTime.ToString("dd MMMM yyyy"),
				TokenExpiryDate = DateTime.Now.AddDays(7.0).ToString(Config.DatetimeStringFormat),
				Username = licence.AccountUsername
			}), "NoInternetToken");
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			if (File.Exists(filePath2))
			{
				File.Delete(filePath2);
			}
			File.WriteAllText(filePath, saveData);
			File.WriteAllText(filePath2, NoInternetSaveData);
		}

		// Token: 0x060003BB RID: 955 RVA: 0x00014E34 File Offset: 0x00013034
		public void SaveAppInfo(string json)
		{
			string filePath = Path.Combine(SessionData.ApplicationDataFolderPath, "data2.bin");
			string saveData = EncryptString.Encrypt(json, "appInfo");
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			File.WriteAllText(filePath, saveData);
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00014E74 File Offset: 0x00013074
		private void TokenLogin()
		{
			Login.<TokenLogin>d__17 <TokenLogin>d__;
			<TokenLogin>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<TokenLogin>d__.<>4__this = this;
			<TokenLogin>d__.<>1__state = -1;
			<TokenLogin>d__.<>t__builder.Start<Login.<TokenLogin>d__17>(ref <TokenLogin>d__);
		}

		// Token: 0x060003BD RID: 957 RVA: 0x00014EAC File Offset: 0x000130AC
		private void licenceTokenLoginPostCompleted(bool success, string content)
		{
			if (!success)
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.LoadingObject.Visibility = Visibility.Collapsed;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			LicenceLoginResponse response = JsonConvert.DeserializeObject<LicenceLoginResponse>(content);
			if (response.LicenceValid)
			{
				Login.expressionTime = response.GetExpirationTimeString();
				this.SaveTokenInfo(response, Login.email);
				DispatcherOperation op2 = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					ActivationKingWindow.SetPage(ApplicationPage.SettingsPage, false);
					this.InfoLabel.Text = "Starting application...";
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			DispatcherOperation op3 = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LoadingObject.Visibility = Visibility.Collapsed;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00014F44 File Offset: 0x00013144
		public void RequestTrialVersionAsync(object sender, RoutedEventArgs e)
		{
			Login.<RequestTrialVersionAsync>d__19 <RequestTrialVersionAsync>d__;
			<RequestTrialVersionAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RequestTrialVersionAsync>d__.<>4__this = this;
			<RequestTrialVersionAsync>d__.<>1__state = -1;
			<RequestTrialVersionAsync>d__.<>t__builder.Start<Login.<RequestTrialVersionAsync>d__19>(ref <RequestTrialVersionAsync>d__);
		}

		// Token: 0x060003BF RID: 959 RVA: 0x00014F7C File Offset: 0x0001317C
		private void LicenceTrialLoginPostCompleted(bool status, string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					MessageBoxWindow.CreateWindow("Server Error", "Please contact with the developers of Activation King", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
					this.LoadingObject.Visibility = Visibility.Collapsed;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			if (status)
			{
				LicenceLoginResponse responseTrialToken = JsonConvert.DeserializeObject<LicenceLoginResponse>(message);
				DispatcherOperation op2 = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					if (responseTrialToken.LicenceValid)
					{
						MessageBoxWindow.CreateWindow("Your Trial is Activated!", "Check your email for the licence code.", new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Success, null, MessageBoxWindow.MessageBoxSize.Large, false);
						this.LoadingObject.Visibility = Visibility.Collapsed;
						this.RightGroupStartTrial.Visibility = Visibility.Collapsed;
						this.RightGroup.Visibility = Visibility.Visible;
						this.MailBox.Text = this.MailBoxTrial.Text;
						return;
					}
					this.LoadingObject.Visibility = Visibility.Collapsed;
					string details = string.Empty;
					switch (responseTrialToken.ResultCode)
					{
					case 9:
						details = "You already have a license. Please use the login panel to start the application.";
						MessageBoxWindow.CreateWindow(responseTrialToken.Message, details, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
						return;
					case 10:
						details = "Trial is not allowed. Please purchase " + AppInfo.AppClass.DisplayName + " app\n" + AppInfo.AppClass.InfoLink;
						MessageBoxWindow.CreateWindow(responseTrialToken.Message, details, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
						return;
					case 11:
						details = "Please purchase " + AppInfo.AppClass.DisplayName + " app\n" + AppInfo.AppClass.InfoLink;
						MessageBoxWindow.CreateWindow(responseTrialToken.Message, details, new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
						return;
					default:
						this.debugTextTrial.Text = responseTrialToken.Message;
						return;
					}
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			DispatcherOperation op3 = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LoadingObject.Visibility = Visibility.Collapsed;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060003C0 RID: 960 RVA: 0x0001500C File Offset: 0x0001320C
		private void LoadingImage_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060003C1 RID: 961 RVA: 0x0001500E File Offset: 0x0001320E
		private void leftPanelCover_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060003C2 RID: 962 RVA: 0x00015010 File Offset: 0x00013210
		private void SignInButton_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060003C3 RID: 963 RVA: 0x00015014 File Offset: 0x00013214
		private void MoreInfoButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(AppInfo.AppClass.InfoLink);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060003C4 RID: 964 RVA: 0x00015048 File Offset: 0x00013248
		private void SupportButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(AppInfo.AppClass.SupportLink);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060003C5 RID: 965 RVA: 0x0001507C File Offset: 0x0001327C
		private void SignButton_Click(object sender, RoutedEventArgs e)
		{
			Login.<SignButton_Click>d__26 <SignButton_Click>d__;
			<SignButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SignButton_Click>d__.<>4__this = this;
			<SignButton_Click>d__.<>1__state = -1;
			<SignButton_Click>d__.<>t__builder.Start<Login.<SignButton_Click>d__26>(ref <SignButton_Click>d__);
		}

		// Token: 0x060003C6 RID: 966 RVA: 0x000150B4 File Offset: 0x000132B4
		private void ShowLoadingWindow(string message)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LoadingObject.Visibility = Visibility.Visible;
				this.InfoLabel.Text = message;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x000150F4 File Offset: 0x000132F4
		private void Focus_Click(object sender, RoutedEventArgs e)
		{
			Button refID = sender as Button;
			int UID = (int)Convert.ToInt16(refID.Uid);
			if (UID == 0)
			{
				refID.Visibility = Visibility.Collapsed;
				this.MailBox.Focus();
				return;
			}
			if (UID == 1)
			{
				refID.Visibility = Visibility.Collapsed;
				this.LicenceKeyBox.Focus();
			}
		}

		// Token: 0x060003C8 RID: 968 RVA: 0x00015144 File Offset: 0x00013344
		private void FocusTrial_Click(object sender, RoutedEventArgs e)
		{
			Button refID = sender as Button;
			int UID = (int)Convert.ToInt16(refID.Uid);
			if (UID == 0)
			{
				refID.Visibility = Visibility.Collapsed;
				this.MailBoxTrial.Focus();
				return;
			}
			if (UID == 1)
			{
				refID.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060003C9 RID: 969 RVA: 0x00015186 File Offset: 0x00013386
		private void MailBoxTrial_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.MailBoxTrial.Text.Length == 0)
			{
				this.MailBoxPlaceholderTrial.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060003CA RID: 970 RVA: 0x000151A6 File Offset: 0x000133A6
		private void MailBoxTrial_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.MailBoxTrial.Text.Length == 0)
			{
				this.MailBoxPlaceholderTrial.Visibility = Visibility.Visible;
				return;
			}
			if (this.MailBoxPlaceholderTrial.Visibility == Visibility.Visible)
			{
				this.MailBoxPlaceholderTrial.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060003CB RID: 971 RVA: 0x000151E0 File Offset: 0x000133E0
		private void MailBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.MailBox.Text.Length == 0)
			{
				this.MailBoxPlaceholder.Visibility = Visibility.Visible;
				return;
			}
			if (this.MailBoxPlaceholder.Visibility == Visibility.Visible)
			{
				this.MailBoxPlaceholder.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060003CC RID: 972 RVA: 0x0001521A File Offset: 0x0001341A
		private void LicenceKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			if (this.LicenceKeyBox.Text.ToString().Length == 0)
			{
				this.LicenceKeyPlaceholder.Visibility = Visibility.Visible;
				return;
			}
			if (this.LicenceKeyPlaceholder.Visibility == Visibility.Visible)
			{
				this.LicenceKeyPlaceholder.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060003CD RID: 973 RVA: 0x00015259 File Offset: 0x00013459
		private void LicenceKeyBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.LicenceKeyPlaceholder.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060003CE RID: 974 RVA: 0x00015267 File Offset: 0x00013467
		private void LicenceKeyBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.LicenceKeyBox.Text.ToString().Length == 0)
			{
				this.LicenceKeyPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060003CF RID: 975 RVA: 0x0001528C File Offset: 0x0001348C
		private void MailBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.MailBox.Text.Length == 0)
			{
				this.MailBoxPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060003D0 RID: 976 RVA: 0x000152AC File Offset: 0x000134AC
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060003D1 RID: 977 RVA: 0x000152B0 File Offset: 0x000134B0
		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			double screenRatio = base.ActualWidth / 3840.0;
			if (isPortraitScreen)
			{
				this.gridLayout.RowDefinitions[0].Height = new GridLength(250.0, GridUnitType.Star);
				this.gridLayout.RowDefinitions[this.gridLayout.RowDefinitions.Count - 1].Height = new GridLength(250.0, GridUnitType.Star);
				return;
			}
			this.gridLayout.RowDefinitions[0].Height = new GridLength(43.0, GridUnitType.Star);
			this.gridLayout.RowDefinitions[this.gridLayout.RowDefinitions.Count - 1].Height = new GridLength(43.0, GridUnitType.Star);
		}

		// Token: 0x060003D2 RID: 978 RVA: 0x00015397 File Offset: 0x00013597
		private void NoButton_Click(object sender, RoutedEventArgs e)
		{
			this.PopUpObject.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060003D3 RID: 979 RVA: 0x000153A5 File Offset: 0x000135A5
		private void YesButton_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
			Environment.Exit(0);
		}

		// Token: 0x060003D4 RID: 980 RVA: 0x000153B7 File Offset: 0x000135B7
		private void ExitButton_Click(object sender, RoutedEventArgs e)
		{
			this.PopUpObject.Visibility = Visibility.Visible;
		}

		// Token: 0x060003D5 RID: 981 RVA: 0x000153C5 File Offset: 0x000135C5
		private void TrialButton_Click(object sender, RoutedEventArgs e)
		{
			this.StartButtonTrial.IsEnabled = true;
			this.RightGroup.Visibility = Visibility.Hidden;
			this.RightGroupStartTrial.Visibility = Visibility.Visible;
		}

		// Token: 0x060003D6 RID: 982 RVA: 0x000153EB File Offset: 0x000135EB
		private void CancelButtonTrial_Click(object sender, RoutedEventArgs e)
		{
			this.debugTextTrial.Text = "";
			this.debugText.Content = "";
			this.RightGroup.Visibility = Visibility.Visible;
			this.RightGroupStartTrial.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060003D7 RID: 983 RVA: 0x00015428 File Offset: 0x00013628
		private void SubscribeNowButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(AppInfo.AppClass.InfoLink);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060003D8 RID: 984 RVA: 0x0001545C File Offset: 0x0001365C
		private void ForgotButton_Click(object sender, RoutedEventArgs e)
		{
			this.StartButtonForgot.IsEnabled = true;
			this.RightGroupForgot.Visibility = Visibility.Visible;
			this.RightGroup.Visibility = Visibility.Hidden;
		}

		// Token: 0x060003D9 RID: 985 RVA: 0x00015484 File Offset: 0x00013684
		private void ForgotLicencePostCompleted(bool success, string message)
		{
			if (string.IsNullOrEmpty(message))
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					MessageBoxWindow.CreateWindow("Server Error", "Please contact with the developers of Activation King", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Error, null, MessageBoxWindow.MessageBoxSize.Large, false);
					this.LoadingObject.Visibility = Visibility.Collapsed;
				}), DispatcherPriority.Normal, Array.Empty<object>());
				return;
			}
			if (success)
			{
				LicenceLoginResponse response = JsonConvert.DeserializeObject<LicenceLoginResponse>(message);
				base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.debugTextForgot.Content = response.Message;
					this.MailBox.Text = this.MailBoxForgot.Text;
				}), DispatcherPriority.Normal, Array.Empty<object>());
			}
			base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LoadingObject.Visibility = Visibility.Collapsed;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060003DA RID: 986 RVA: 0x00015513 File Offset: 0x00013713
		private void CancelButtonForgot_Click(object sender, RoutedEventArgs e)
		{
			this.debugTextTrial.Text = "";
			this.debugText.Content = "";
			this.RightGroupForgot.Visibility = Visibility.Collapsed;
			this.RightGroup.Visibility = Visibility.Visible;
		}

		// Token: 0x060003DB RID: 987 RVA: 0x00015550 File Offset: 0x00013750
		private void RequestForgotVersionAsync(object sender, RoutedEventArgs e)
		{
			Login.<RequestForgotVersionAsync>d__48 <RequestForgotVersionAsync>d__;
			<RequestForgotVersionAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<RequestForgotVersionAsync>d__.<>4__this = this;
			<RequestForgotVersionAsync>d__.<>1__state = -1;
			<RequestForgotVersionAsync>d__.<>t__builder.Start<Login.<RequestForgotVersionAsync>d__48>(ref <RequestForgotVersionAsync>d__);
		}

		// Token: 0x060003DC RID: 988 RVA: 0x00015588 File Offset: 0x00013788
		private void FocusForgot_Click(object sender, RoutedEventArgs e)
		{
			Button refID = sender as Button;
			int UID = (int)Convert.ToInt16(refID.Uid);
			if (UID == 0)
			{
				refID.Visibility = Visibility.Collapsed;
				this.MailBoxForgot.Focus();
				return;
			}
			if (UID == 1)
			{
				refID.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060003DD RID: 989 RVA: 0x000155CA File Offset: 0x000137CA
		private void MailBoxForgot_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.MailBoxForgot.Text.Length == 0)
			{
				this.MailBoxPlaceholderForgot.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060003DE RID: 990 RVA: 0x000155EA File Offset: 0x000137EA
		private void MailBoxForgot_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.MailBoxForgot.Text.Length == 0)
			{
				this.MailBoxPlaceholderForgot.Visibility = Visibility.Visible;
				return;
			}
			if (this.MailBoxPlaceholderForgot.Visibility == Visibility.Visible)
			{
				this.MailBoxPlaceholderForgot.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060003DF RID: 991 RVA: 0x00015624 File Offset: 0x00013824
		private void Page_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				if (this.RightGroup.Visibility == Visibility.Visible)
				{
					this.SignButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
					return;
				}
				if (this.RightGroupStartTrial.Visibility == Visibility.Visible)
				{
					this.StartButtonTrial.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
					return;
				}
				if (this.RightGroupForgot.Visibility == Visibility.Visible)
				{
					this.StartButtonForgot.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
				}
			}
		}

		// Token: 0x060003E0 RID: 992 RVA: 0x000156A4 File Offset: 0x000138A4
		private void btnUpdate_Click(object sender, RoutedEventArgs e)
		{
			UpdateWindow yeni = new UpdateWindow(false);
			yeni.Show();
		}

		// Token: 0x060003E1 RID: 993 RVA: 0x000156C0 File Offset: 0x000138C0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocater = new Uri("/KingAIPhotoBoothPro;component/pages/activationking/login.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}

		// Token: 0x060003E2 RID: 994 RVA: 0x000156F0 File Offset: 0x000138F0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((Login)target).Loaded += this.Login_Loaded;
				((Login)target).Unloaded += this.Page_Unloaded;
				((Login)target).SizeChanged += this.Page_SizeChanged;
				((Login)target).KeyDown += this.Page_KeyDown;
				return;
			case 2:
				this.gridMain = (Grid)target;
				return;
			case 3:
				this.logo = (Image)target;
				return;
			case 4:
				this.gridLayout = (Grid)target;
				return;
			case 5:
				this.LoginPanel = (Grid)target;
				return;
			case 6:
				this.leftPanelCover = (Border)target;
				this.leftPanelCover.Loaded += this.leftPanelCover_Loaded;
				return;
			case 7:
				this.LeftGroup = (Grid)target;
				return;
			case 8:
				this.TitleApp = (TextBlock)target;
				return;
			case 9:
				this.TitleAppVersion = (TextBlock)target;
				return;
			case 10:
				this.TitleAppDesc = (TextBlock)target;
				return;
			case 11:
				this.MoreInfoButton = (Button)target;
				this.MoreInfoButton.Click += this.MoreInfoButton_Click;
				return;
			case 12:
				this.cubeImage = (Image)target;
				return;
			case 13:
				this.MoreInfoTxt = (TextBlock)target;
				return;
			case 14:
				this.SupportButton = (Button)target;
				this.SupportButton.Click += this.SupportButton_Click;
				return;
			case 15:
				this.speechImage = (Image)target;
				return;
			case 16:
				this.supportTxt = (TextBlock)target;
				return;
			case 17:
				this.UpdateGrid = (Grid)target;
				return;
			case 18:
				this.btnUpdate = (Button)target;
				this.btnUpdate.Click += this.btnUpdate_Click;
				return;
			case 19:
				this.RightGroup = (Grid)target;
				return;
			case 20:
				this.SignText = (Label)target;
				return;
			case 21:
				this.gridInputField = (Grid)target;
				return;
			case 22:
				this.MailBox = (TextBox)target;
				this.MailBox.TextChanged += this.MailBox_TextChanged;
				this.MailBox.LostFocus += this.MailBox_LostFocus;
				return;
			case 23:
				this.MailBoxPlaceholder = (Button)target;
				this.MailBoxPlaceholder.Click += this.Focus_Click;
				return;
			case 24:
				this.gridCodeInput = (Grid)target;
				return;
			case 25:
				this.LicenceKeyBox = (TextBox)target;
				this.LicenceKeyBox.LostFocus += this.LicenceKeyBox_LostFocus;
				this.LicenceKeyBox.GotFocus += this.LicenceKeyBox_GotFocus;
				this.LicenceKeyBox.TextChanged += new TextChangedEventHandler(this.LicenceKeyBox_PasswordChanged);
				return;
			case 26:
				this.LicenceKeyPlaceholder = (Button)target;
				this.LicenceKeyPlaceholder.Click += this.Focus_Click;
				return;
			case 27:
				this.debugText = (Label)target;
				return;
			case 28:
				this.SubscribeNowButton = (Button)target;
				this.SubscribeNowButton.Click += this.SubscribeNowButton_Click;
				return;
			case 29:
				this.SignButton = (Button)target;
				this.SignButton.Click += this.SignButton_Click;
				return;
			case 30:
				this.ForgotButton = (Button)target;
				this.ForgotButton.Click += this.ForgotButton_Click;
				return;
			case 31:
				this.SpanGetCode = (Span)target;
				return;
			case 32:
				this.TrialButton = (Button)target;
				this.TrialButton.Click += this.TrialButton_Click;
				return;
			case 33:
				this.RightGroupStartTrial = (Grid)target;
				return;
			case 34:
				this.StartTrialText = (Label)target;
				return;
			case 35:
				this.gridInputFieldTrial = (Grid)target;
				return;
			case 36:
				this.MailBoxTrial = (TextBox)target;
				this.MailBoxTrial.TextChanged += this.MailBoxTrial_TextChanged;
				this.MailBoxTrial.LostFocus += this.MailBoxTrial_LostFocus;
				return;
			case 37:
				this.MailBoxPlaceholderTrial = (Button)target;
				this.MailBoxPlaceholderTrial.Click += this.FocusTrial_Click;
				return;
			case 38:
				this.debugTextTrial = (TextBlock)target;
				return;
			case 39:
				this.CancelButtonTrial = (Button)target;
				this.CancelButtonTrial.Click += this.CancelButtonTrial_Click;
				return;
			case 40:
				this.StartButtonTrial = (Button)target;
				this.StartButtonTrial.Click += this.RequestTrialVersionAsync;
				return;
			case 41:
				this.RightGroupForgot = (Grid)target;
				return;
			case 42:
				this.ForgotText = (Label)target;
				return;
			case 43:
				this.gridInputFieldForgot = (Grid)target;
				return;
			case 44:
				this.MailBoxForgot = (TextBox)target;
				this.MailBoxForgot.TextChanged += this.MailBoxForgot_TextChanged;
				this.MailBoxForgot.LostFocus += this.MailBoxForgot_LostFocus;
				return;
			case 45:
				this.MailBoxPlaceholderForgot = (Button)target;
				this.MailBoxPlaceholderForgot.Click += this.FocusForgot_Click;
				return;
			case 46:
				this.debugTextForgot = (Label)target;
				return;
			case 47:
				this.CancelButtonForgot = (Button)target;
				this.CancelButtonForgot.Click += this.CancelButtonForgot_Click;
				return;
			case 48:
				this.StartButtonForgot = (Button)target;
				this.StartButtonForgot.Click += this.RequestForgotVersionAsync;
				return;
			case 49:
				this.ExitButton = (Button)target;
				this.ExitButton.Click += this.ExitButton_Click;
				return;
			case 50:
				this.ExitButtonBrush = (ImageBrush)target;
				return;
			case 51:
				this.LoadingObject = (Grid)target;
				return;
			case 52:
				this.BorderLoadingBG = (Border)target;
				return;
			case 53:
				this.TxtLoadingTitle = (Label)target;
				return;
			case 54:
				this.InfoLabel = (TextBlock)target;
				return;
			case 55:
				this.LoadingContainer = (Grid)target;
				return;
			case 56:
				this.lottie = (LottieAnimationView)target;
				return;
			case 57:
				this.PopUpObject = (Grid)target;
				return;
			case 58:
				this.PopUPImage = (Border)target;
				return;
			case 59:
				this.PopUpTitle = (Label)target;
				return;
			case 60:
				this.PopUpLabel = (Label)target;
				return;
			case 61:
				this.YesButton = (Button)target;
				this.YesButton.Click += this.YesButton_Click;
				return;
			case 62:
				this.NoButton = (Button)target;
				this.NoButton.Click += this.NoButton_Click;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400039D RID: 925
		public static string expressionTime;

		// Token: 0x0400039E RID: 926
		public static string email = "";

		// Token: 0x0400039F RID: 927
		private AppToken NoInternetToken;

		// Token: 0x040003A0 RID: 928
		public static Action LoginPageLoaded;

		// Token: 0x040003A1 RID: 929
		internal Grid gridMain;

		// Token: 0x040003A2 RID: 930
		internal Image logo;

		// Token: 0x040003A3 RID: 931
		internal Grid gridLayout;

		// Token: 0x040003A4 RID: 932
		internal Grid LoginPanel;

		// Token: 0x040003A5 RID: 933
		internal Border leftPanelCover;

		// Token: 0x040003A6 RID: 934
		internal Grid LeftGroup;

		// Token: 0x040003A7 RID: 935
		internal TextBlock TitleApp;

		// Token: 0x040003A8 RID: 936
		internal TextBlock TitleAppVersion;

		// Token: 0x040003A9 RID: 937
		internal TextBlock TitleAppDesc;

		// Token: 0x040003AA RID: 938
		internal Button MoreInfoButton;

		// Token: 0x040003AB RID: 939
		internal Image cubeImage;

		// Token: 0x040003AC RID: 940
		internal TextBlock MoreInfoTxt;

		// Token: 0x040003AD RID: 941
		internal Button SupportButton;

		// Token: 0x040003AE RID: 942
		internal Image speechImage;

		// Token: 0x040003AF RID: 943
		internal TextBlock supportTxt;

		// Token: 0x040003B0 RID: 944
		internal Grid UpdateGrid;

		// Token: 0x040003B1 RID: 945
		internal Button btnUpdate;

		// Token: 0x040003B2 RID: 946
		internal Grid RightGroup;

		// Token: 0x040003B3 RID: 947
		internal Label SignText;

		// Token: 0x040003B4 RID: 948
		internal Grid gridInputField;

		// Token: 0x040003B5 RID: 949
		internal TextBox MailBox;

		// Token: 0x040003B6 RID: 950
		internal Button MailBoxPlaceholder;

		// Token: 0x040003B7 RID: 951
		internal Grid gridCodeInput;

		// Token: 0x040003B8 RID: 952
		internal TextBox LicenceKeyBox;

		// Token: 0x040003B9 RID: 953
		internal Button LicenceKeyPlaceholder;

		// Token: 0x040003BA RID: 954
		internal Label debugText;

		// Token: 0x040003BB RID: 955
		internal Button SubscribeNowButton;

		// Token: 0x040003BC RID: 956
		internal Button SignButton;

		// Token: 0x040003BD RID: 957
		internal Button ForgotButton;

		// Token: 0x040003BE RID: 958
		internal Span SpanGetCode;

		// Token: 0x040003BF RID: 959
		internal Button TrialButton;

		// Token: 0x040003C0 RID: 960
		internal Grid RightGroupStartTrial;

		// Token: 0x040003C1 RID: 961
		internal Label StartTrialText;

		// Token: 0x040003C2 RID: 962
		internal Grid gridInputFieldTrial;

		// Token: 0x040003C3 RID: 963
		internal TextBox MailBoxTrial;

		// Token: 0x040003C4 RID: 964
		internal Button MailBoxPlaceholderTrial;

		// Token: 0x040003C5 RID: 965
		internal TextBlock debugTextTrial;

		// Token: 0x040003C6 RID: 966
		internal Button CancelButtonTrial;

		// Token: 0x040003C7 RID: 967
		internal Button StartButtonTrial;

		// Token: 0x040003C8 RID: 968
		internal Grid RightGroupForgot;

		// Token: 0x040003C9 RID: 969
		internal Label ForgotText;

		// Token: 0x040003CA RID: 970
		internal Grid gridInputFieldForgot;

		// Token: 0x040003CB RID: 971
		internal TextBox MailBoxForgot;

		// Token: 0x040003CC RID: 972
		internal Button MailBoxPlaceholderForgot;

		// Token: 0x040003CD RID: 973
		internal Label debugTextForgot;

		// Token: 0x040003CE RID: 974
		internal Button CancelButtonForgot;

		// Token: 0x040003CF RID: 975
		internal Button StartButtonForgot;

		// Token: 0x040003D0 RID: 976
		internal Button ExitButton;

		// Token: 0x040003D1 RID: 977
		internal ImageBrush ExitButtonBrush;

		// Token: 0x040003D2 RID: 978
		internal Grid LoadingObject;

		// Token: 0x040003D3 RID: 979
		internal Border BorderLoadingBG;

		// Token: 0x040003D4 RID: 980
		internal Label TxtLoadingTitle;

		// Token: 0x040003D5 RID: 981
		internal TextBlock InfoLabel;

		// Token: 0x040003D6 RID: 982
		internal Grid LoadingContainer;

		// Token: 0x040003D7 RID: 983
		internal LottieAnimationView lottie;

		// Token: 0x040003D8 RID: 984
		internal Grid PopUpObject;

		// Token: 0x040003D9 RID: 985
		internal Border PopUPImage;

		// Token: 0x040003DA RID: 986
		internal Label PopUpTitle;

		// Token: 0x040003DB RID: 987
		internal Label PopUpLabel;

		// Token: 0x040003DC RID: 988
		internal Button YesButton;

		// Token: 0x040003DD RID: 989
		internal Button NoButton;

		// Token: 0x040003DE RID: 990
		private bool _contentLoaded;
	}
}
