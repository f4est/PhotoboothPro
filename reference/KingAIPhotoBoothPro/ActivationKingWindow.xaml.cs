using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using TermControls;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000016 RID: 22
	public partial class ActivationKingWindow : Window
	{
		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600009F RID: 159 RVA: 0x00005980 File Offset: 0x00003B80
		// (set) Token: 0x060000A0 RID: 160 RVA: 0x00005987 File Offset: 0x00003B87
		public static ActivationKingWindow Instance { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x060000A1 RID: 161 RVA: 0x0000598F File Offset: 0x00003B8F
		// (set) Token: 0x060000A2 RID: 162 RVA: 0x00005996 File Offset: 0x00003B96
		public static Frame FrameWindowFrame { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x060000A3 RID: 163 RVA: 0x0000599E File Offset: 0x00003B9E
		// (set) Token: 0x060000A4 RID: 164 RVA: 0x000059A5 File Offset: 0x00003BA5
		public static Login LoginPage { get; set; }

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x000059AD File Offset: 0x00003BAD
		// (set) Token: 0x060000A6 RID: 166 RVA: 0x000059B4 File Offset: 0x00003BB4
		public static SettingsPage SettingsPage { get; set; }

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x060000A7 RID: 167 RVA: 0x000059BC File Offset: 0x00003BBC
		// (set) Token: 0x060000A8 RID: 168 RVA: 0x000059C3 File Offset: 0x00003BC3
		public static DSLR DSLRMain { get; set; }

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x000059CB File Offset: 0x00003BCB
		// (set) Token: 0x060000AA RID: 170 RVA: 0x000059D2 File Offset: 0x00003BD2
		public static DSLRPhoto DSLRPhoto { get; set; }

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x060000AB RID: 171 RVA: 0x000059DA File Offset: 0x00003BDA
		// (set) Token: 0x060000AC RID: 172 RVA: 0x000059E1 File Offset: 0x00003BE1
		public static FaceSwapBackgroundGallery FaceSwapBackgroundGalleryPage { get; set; }

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x060000AD RID: 173 RVA: 0x000059E9 File Offset: 0x00003BE9
		// (set) Token: 0x060000AE RID: 174 RVA: 0x000059F0 File Offset: 0x00003BF0
		public static AISharing AISharingPage { get; set; }

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x060000AF RID: 175 RVA: 0x000059F8 File Offset: 0x00003BF8
		// (set) Token: 0x060000B0 RID: 176 RVA: 0x000059FF File Offset: 0x00003BFF
		public static DSLRGif DSLRGifPage { get; set; }

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x00005A07 File Offset: 0x00003C07
		// (set) Token: 0x060000B2 RID: 178 RVA: 0x00005A0E File Offset: 0x00003C0E
		public static DSLRVideo DSLRVideoPage { get; set; }

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00005A16 File Offset: 0x00003C16
		// (set) Token: 0x060000B4 RID: 180 RVA: 0x00005A1D File Offset: 0x00003C1D
		public static GoProVideo GoProVideoPage { get; set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000B5 RID: 181 RVA: 0x00005A25 File Offset: 0x00003C25
		// (set) Token: 0x060000B6 RID: 182 RVA: 0x00005A2C File Offset: 0x00003C2C
		public static Gallery GalleryPage { get; set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000B7 RID: 183 RVA: 0x00005A34 File Offset: 0x00003C34
		// (set) Token: 0x060000B8 RID: 184 RVA: 0x00005A3B File Offset: 0x00003C3B
		public static AIEffectGalleryPage AiEffectGalleryPage { get; set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000B9 RID: 185 RVA: 0x00005A43 File Offset: 0x00003C43
		// (set) Token: 0x060000BA RID: 186 RVA: 0x00005A4A File Offset: 0x00003C4A
		public static TemplateGallery TemplateGalleryPage { get; set; }

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00005A52 File Offset: 0x00003C52
		// (set) Token: 0x060000BC RID: 188 RVA: 0x00005A59 File Offset: 0x00003C59
		public static AIPrompGalleryPage AIPrompGalleryPage { get; set; }

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000BD RID: 189 RVA: 0x00005A61 File Offset: 0x00003C61
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00005A68 File Offset: 0x00003C68
		private static FontWindow FontSettingsWindow { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000BF RID: 191 RVA: 0x00005A70 File Offset: 0x00003C70
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x00005A77 File Offset: 0x00003C77
		private static CameraSettingsWindows CameraSettingsWindow { get; set; }

		// Token: 0x060000C1 RID: 193 RVA: 0x00005A80 File Offset: 0x00003C80
		public ActivationKingWindow()
		{
			ActivationKingWindow.ApplicationDataPathManagement();
			this.InitializeComponent();
			ActivationKingWindow.Instance = this;
			this.TrialWatermarkControl();
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00005B47 File Offset: 0x00003D47
		public static void OpenFontWindow()
		{
			if (ActivationKingWindow.FontSettingsWindow == null)
			{
				ActivationKingWindow.FontSettingsWindow = new FontWindow();
			}
			ActivationKingWindow.FontSettingsWindow.Visibility = Visibility.Visible;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00005B65 File Offset: 0x00003D65
		public static void CloseFontWindow()
		{
			if (ActivationKingWindow.FontSettingsWindow == null)
			{
				return;
			}
			ActivationKingWindow.FontSettingsWindow.Visibility = Visibility.Hidden;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00005B7A File Offset: 0x00003D7A
		public static void OpenCameraSettingsWindow()
		{
			if (ActivationKingWindow.CameraSettingsWindow == null)
			{
				ActivationKingWindow.CameraSettingsWindow = new CameraSettingsWindows();
			}
			ActivationKingWindow.CameraSettingsWindow.Visibility = Visibility.Visible;
			ActivationKingWindow.CameraSettingsWindow.Load();
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x00005BA2 File Offset: 0x00003DA2
		public static void CloseCameraSettingsWindow()
		{
			if (ActivationKingWindow.CameraSettingsWindow == null)
			{
				return;
			}
			ActivationKingWindow.CameraSettingsWindow.Visibility = Visibility.Hidden;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00005BB8 File Offset: 0x00003DB8
		private static void ApplicationDataPathManagement()
		{
			string[] downloadUrls = new string[]
			{
				"Installer.msi",
				"setup.exe"
			};
			for (int i = 0; i < 2; i++)
			{
				if (File.Exists(Path.Combine(SessionData.ApplicationDataFolderPath, downloadUrls[i])))
				{
					File.Delete(Path.Combine(SessionData.ApplicationDataFolderPath, downloadUrls[i]));
				}
			}
			if (!Directory.Exists(SessionData.ApplicationDataFolderPath))
			{
				Directory.CreateDirectory(SessionData.ApplicationDataFolderPath);
			}
		}

		// Token: 0x17000023 RID: 35
		// (get) Token: 0x060000C7 RID: 199 RVA: 0x00005C25 File Offset: 0x00003E25
		public static bool IsAdminPageActive
		{
			get
			{
				return ActivationKingWindow.CurrentPage < ApplicationPage.HomePage;
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x060000C8 RID: 200 RVA: 0x00005C2F File Offset: 0x00003E2F
		private static bool IsPaymentPageOpen
		{
			get
			{
				return ActivationKingWindow.CurrentPage == ApplicationPage.HomePage && ActivationKingWindow.DSLRMain.PaymentGrid.Visibility == Visibility.Visible;
			}
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00005C4D File Offset: 0x00003E4D
		private static bool IsIdleWaitingPage()
		{
			if (ActivationKingWindow.CurrentPage == ApplicationPage.AiSharingPage)
			{
				return !ActivationKingWindow.AISharingPage.IsThereWorkingVariation();
			}
			return ActivationKingWindow.CurrentPage > ApplicationPage.GifShootPage || ActivationKingWindow.IsPaymentPageOpen;
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00005C74 File Offset: 0x00003E74
		public static void SetPage(ApplicationPage page, bool forceNewInstance = false)
		{
			switch (page)
			{
			case ApplicationPage.LoginPage:
				if (ActivationKingWindow.LoginPage == null || forceNewInstance)
				{
					ActivationKingWindow.LoginPage = new Login();
					ActivationKingWindow.SettingsPage = new SettingsPage();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.LoginPage;
				break;
			case ApplicationPage.SettingsPage:
				if (ActivationKingWindow.SettingsPage == null || forceNewInstance)
				{
					ActivationKingWindow.SettingsPage = new SettingsPage();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.SettingsPage;
				break;
			case ApplicationPage.HomePage:
				if (ActivationKingWindow.DSLRMain == null || forceNewInstance)
				{
					ActivationKingWindow.DSLRMain = new DSLR();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.DSLRMain;
				ActivationKingWindow.Instance.SetBackgroundColor();
				break;
			case ApplicationPage.PhotoShootPage:
				if (ActivationKingWindow.DSLRPhoto == null || forceNewInstance)
				{
					ActivationKingWindow.DSLRPhoto = new DSLRPhoto();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.DSLRPhoto;
				break;
			case ApplicationPage.VideoShootPage:
				if (ActivationKingWindow.DSLRVideoPage == null || forceNewInstance)
				{
					ActivationKingWindow.DSLRVideoPage = new DSLRVideo();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.DSLRVideoPage;
				break;
			case ApplicationPage.GifShootPage:
				if (ActivationKingWindow.DSLRGifPage == null || forceNewInstance)
				{
					ActivationKingWindow.DSLRGifPage = new DSLRGif();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.DSLRGifPage;
				break;
			case ApplicationPage.AiSharingPage:
				if (ActivationKingWindow.AISharingPage == null || forceNewInstance)
				{
					ActivationKingWindow.AISharingPage = new AISharing();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.AISharingPage;
				break;
			case ApplicationPage.GalleryPage:
				if (ActivationKingWindow.GalleryPage == null || forceNewInstance)
				{
					ActivationKingWindow.GalleryPage = new Gallery();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.GalleryPage;
				break;
			case ApplicationPage.FaceSwapBackgroundGalleryPage:
				if (ActivationKingWindow.FaceSwapBackgroundGalleryPage == null || forceNewInstance)
				{
					ActivationKingWindow.FaceSwapBackgroundGalleryPage = new FaceSwapBackgroundGallery();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.FaceSwapBackgroundGalleryPage;
				break;
			case ApplicationPage.GoProShootPage:
				if (ActivationKingWindow.GoProVideoPage == null || forceNewInstance)
				{
					ActivationKingWindow.GoProVideoPage = new GoProVideo();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.GoProVideoPage;
				break;
			case ApplicationPage.TemplateGalleryPage:
				if (ActivationKingWindow.TemplateGalleryPage == null || forceNewInstance)
				{
					ActivationKingWindow.TemplateGalleryPage = new TemplateGallery();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.TemplateGalleryPage;
				break;
			case ApplicationPage.AiEffectGalleryPage:
				if (ActivationKingWindow.AiEffectGalleryPage == null || forceNewInstance)
				{
					ActivationKingWindow.AiEffectGalleryPage = new AIEffectGalleryPage();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.AiEffectGalleryPage;
				break;
			case ApplicationPage.AIPrompGalleryPage:
				if (ActivationKingWindow.AIPrompGalleryPage == null || forceNewInstance)
				{
					ActivationKingWindow.AIPrompGalleryPage = new AIPrompGalleryPage();
				}
				ActivationKingWindow.FrameWindowFrame.Content = ActivationKingWindow.AIPrompGalleryPage;
				break;
			default:
				return;
			}
			ActivationKingWindow.CurrentPage = page;
			ActivationKingWindow.returnMainPageTimer = 0;
			ActivationKingWindow.Instance.TrialWatermarkControl();
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00005EFA File Offset: 0x000040FA
		private void SetBackgroundColor()
		{
			this.gridMain.Background = new SolidColorBrush(Settings.GetValueColor("colorbackgroundmaintheme"));
			ActivationKingWindow.DSLRMain.gridMain.Background = new SolidColorBrush(Settings.GetValueColor("colorbackgroundtheme"));
		}

		// Token: 0x060000CC RID: 204 RVA: 0x00005F34 File Offset: 0x00004134
		public void CheckInternetGrid(bool internetCheck)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(delegate()
			{
				this.NoInternetGrid.Visibility = (internetCheck ? Visibility.Collapsed : Visibility.Visible);
			});
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00005F70 File Offset: 0x00004170
		public void TrialWatermarkControl()
		{
			if (!ActivationKingWindow.IsAdminPageActive)
			{
				if (SessionData.accountInfo != null)
				{
					base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						if (SessionData.accountInfo.isTrial)
						{
							this.trailGrid.Visibility = Visibility.Visible;
							double angleRadians = Math.Atan2((double)Screen.PrimaryScreen.Bounds.Height, (double)Screen.PrimaryScreen.Bounds.Width);
							double angleDegrees = angleRadians * 57.29577951308232;
							return;
						}
						this.trailGrid.Visibility = Visibility.Collapsed;
					}), Array.Empty<object>()).Wait();
					return;
				}
			}
			else
			{
				this.trailGrid.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x00005FB0 File Offset: 0x000041B0
		public static void LogDebug(string senderName, string message, string memberName, int lineNumber)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				ActivationKingWindow.debugText = global::Debug.GenerateDebugLine(senderName, message, memberName, lineNumber) + "\n" + ActivationKingWindow.debugText;
				ActivationKingWindow.Instance.txtDebugConsole.Text = ActivationKingWindow.debugText;
			}), Array.Empty<object>());
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00006000 File Offset: 0x00004200
		public void SetOverlay(bool value)
		{
			this.gridOverlay.Visibility = (value ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00006014 File Offset: 0x00004214
		private void AppDictionaryControl()
		{
			new Thread(delegate()
			{
				while (base.ActualWidth == 0.0)
				{
					Thread.Sleep(10);
				}
				base.Dispatcher.Invoke(delegate()
				{
					double referenceHeight = 2160.0;
					double refWidth = 3840.0;
					double heightRatio = base.ActualHeight / referenceHeight;
					double widthRatio = base.ActualWidth / refWidth;
					double ratio = Math.Min(widthRatio, heightRatio);
					System.Windows.Application.Current.Resources["AppUIColor"] = new SolidColorBrush(AppInfo.ThemeColor);
					System.Windows.Application.Current.Resources["AppUIColorLight"] = new SolidColorBrush(new System.Windows.Media.Color
					{
						R = AppInfo.ThemeColor.R + (byte.MaxValue - AppInfo.ThemeColor.R) / 4,
						G = AppInfo.ThemeColor.G + (byte.MaxValue - AppInfo.ThemeColor.G) / 4,
						B = AppInfo.ThemeColor.B + (byte.MaxValue - AppInfo.ThemeColor.B) / 4,
						A = byte.MaxValue
					});
					this.originalUICornerRadiusValue = (double)System.Windows.Application.Current.Resources["UICornerRadiusValue"];
					this.originalBannerCornerRadiusValue = (double)System.Windows.Application.Current.Resources["BannerCornerRadiusValue"];
					this.originalSectionCornerRadiusValue = (double)System.Windows.Application.Current.Resources["SectionCornerRadiusValue"];
					this.originalSizes = new double[this.resourses.Length];
					int i = 0;
					foreach (string resource in this.resourses)
					{
						this.originalSizes[i] = (double)System.Windows.Application.Current.Resources[resource];
						System.Windows.Application.Current.Resources[resource] = ratio * this.originalSizes[i];
						i++;
					}
				});
			}).Start();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x0000602C File Offset: 0x0000422C
		private void m_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (this.originalSizes == null)
			{
				return;
			}
			if (base.WindowState == WindowState.Normal && base.ActualWidth > base.ActualHeight * 2.0)
			{
				base.Width = base.ActualHeight * 2.0;
				e.Handled = true;
			}
			double referenceHeight = 2160.0;
			double refWidth = 3840.0;
			double heightRatio = base.ActualHeight / referenceHeight;
			double widthRatio = base.ActualWidth / refWidth;
			double ratio = Math.Min(widthRatio, heightRatio);
			if (ratio > 0.5)
			{
				ratio = 0.5;
			}
			int i = 0;
			foreach (string resource in this.resourses)
			{
				System.Windows.Application.Current.Resources[resource] = ratio * this.originalSizes[i];
				i++;
			}
			CornerRadius c = (CornerRadius)System.Windows.Application.Current.Resources["UICornerRadius"];
			c.TopLeft = ratio * this.originalUICornerRadiusValue;
			c.BottomLeft = ratio * this.originalUICornerRadiusValue;
			c.BottomRight = ratio * this.originalUICornerRadiusValue;
			c.TopRight = ratio * this.originalUICornerRadiusValue;
			System.Windows.Application.Current.Resources["UICornerRadius"] = c;
			CornerRadius cBanner = (CornerRadius)System.Windows.Application.Current.Resources["BannerCornerRadius"];
			cBanner.TopLeft = ratio * this.originalBannerCornerRadiusValue;
			cBanner.BottomLeft = ratio * this.originalBannerCornerRadiusValue;
			cBanner.BottomRight = ratio * this.originalBannerCornerRadiusValue;
			cBanner.TopRight = ratio * this.originalBannerCornerRadiusValue;
			System.Windows.Application.Current.Resources["BannerCornerRadius"] = cBanner;
			CornerRadius cSection = (CornerRadius)System.Windows.Application.Current.Resources["SectionCornerRadius"];
			cSection.TopLeft = ratio * this.originalSectionCornerRadiusValue;
			cSection.BottomLeft = ratio * this.originalSectionCornerRadiusValue;
			cSection.BottomRight = ratio * this.originalSectionCornerRadiusValue;
			cSection.TopRight = ratio * this.originalSectionCornerRadiusValue;
			System.Windows.Application.Current.Resources["SectionCornerRadius"] = cSection;
			if (base.ActualWidth > base.ActualHeight)
			{
				System.Windows.Application.Current.Resources["LoadingGridColumnCount"] = 4;
				return;
			}
			System.Windows.Application.Current.Resources["LoadingGridColumnCount"] = 6;
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x000062B8 File Offset: 0x000044B8
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.FrameWindowFrame = this.MainRoute;
			ActivationKingWindow.FrameWindowFrame.ClearValue(Window.LeftProperty);
			ExtensionMethod.UpdatePrinters();
			this.AppDictionaryControl();
			ActivationKingWindow.FaceSwapBackgroundGalleryPage = new FaceSwapBackgroundGallery();
			this.gridDebugConsole.Visibility = Visibility.Hidden;
			Task.Run(delegate()
			{
				ActivationKingWindow.<<Window_Loaded>b__98_0>d <<Window_Loaded>b__98_0>d;
				<<Window_Loaded>b__98_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Window_Loaded>b__98_0>d.<>4__this = this;
				<<Window_Loaded>b__98_0>d.<>1__state = -1;
				<<Window_Loaded>b__98_0>d.<>t__builder.Start<ActivationKingWindow.<<Window_Loaded>b__98_0>d>(ref <<Window_Loaded>b__98_0>d);
				return <<Window_Loaded>b__98_0>d.<>t__builder.Task;
			});
			try
			{
				SecondWindow fullScreenWindow = new SecondWindow();
				fullScreenWindow.Show();
			}
			catch (Exception)
			{
			}
			DSLR.PhotoBoothStarted = (Action)Delegate.Combine(DSLR.PhotoBoothStarted, new Action(this.PhotoBoothStarted));
			SettingsPage.SettingsPageLoaded = (Action)Delegate.Combine(SettingsPage.SettingsPageLoaded, new Action(this.SetWindowStateNormal));
			Login.LoginPageLoaded = (Action)Delegate.Combine(Login.LoginPageLoaded, new Action(this.SetWindowStateNormal));
			Task.Run(delegate()
			{
				ActivationKingWindow.<<Window_Loaded>b__98_1>d <<Window_Loaded>b__98_1>d;
				<<Window_Loaded>b__98_1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Window_Loaded>b__98_1>d.<>4__this = this;
				<<Window_Loaded>b__98_1>d.<>1__state = -1;
				<<Window_Loaded>b__98_1>d.<>t__builder.Start<ActivationKingWindow.<<Window_Loaded>b__98_1>d>(ref <<Window_Loaded>b__98_1>d);
				return <<Window_Loaded>b__98_1>d.<>t__builder.Task;
			});
		}

		// Token: 0x060000D3 RID: 211 RVA: 0x000063A8 File Offset: 0x000045A8
		private void SetWindowStateNormal()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (base.WindowState == WindowState.Normal)
				{
					return;
				}
				base.WindowState = WindowState.Normal;
				base.WindowStyle = WindowStyle.SingleBorderWindow;
				base.ResizeMode = ResizeMode.CanResize;
				Screen primaryScreen = Screen.AllScreens.FirstOrDefault((Screen screen) => screen.Primary);
				Rectangle workingArea = primaryScreen.WorkingArea;
				PresentationSource source = PresentationSource.FromVisual(this);
				if (source != null)
				{
					Matrix transform = source.CompositionTarget.TransformFromDevice;
					System.Windows.Point topLeft = transform.Transform(new System.Windows.Point((double)workingArea.Left, (double)workingArea.Top));
					System.Windows.Point bottomRight = transform.Transform(new System.Windows.Point((double)workingArea.Right, (double)workingArea.Bottom));
					double newWidth = bottomRight.X - topLeft.X;
					double newHeight = bottomRight.Y - topLeft.Y;
					if (newWidth > newHeight * 2.0)
					{
						newWidth = newHeight * 2.0;
					}
					base.Left = topLeft.X;
					base.Top = topLeft.Y;
					base.Width = newWidth;
					base.Height = newHeight;
					return;
				}
				throw new Exception("PresentationSource is null. Cannot transform coordinates.");
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060000D4 RID: 212 RVA: 0x000063D4 File Offset: 0x000045D4
		private void PhotoBoothStarted()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				base.WindowStyle = WindowStyle.None;
				base.WindowState = WindowState.Normal;
				base.WindowState = WindowState.Maximized;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060000D5 RID: 213 RVA: 0x00006400 File Offset: 0x00004600
		private static void MessageBoxWindow_ResultComplete(Action obj, MessageBoxWindow.MessageBoxReturn arg2)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(obj, Array.Empty<object>()).Wait();
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00006418 File Offset: 0x00004618
		private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.F11 && ActivationKingWindow.CurrentPage > ApplicationPage.SettingsPage)
			{
				LiveViewClass.IsLiveViewTimerEnabled = false;
				this.ReturnSettingsPage();
			}
			if (e.Key == Key.F1 && Keyboard.IsKeyDown(Key.LeftCtrl))
			{
				if (ActivationKingWindow.Instance.gridDebugConsole.Visibility != Visibility.Visible)
				{
					ActivationKingWindow.Instance.gridDebugConsole.Visibility = Visibility.Visible;
				}
				else
				{
					ActivationKingWindow.Instance.gridDebugConsole.Visibility = Visibility.Hidden;
				}
			}
			this.RefreshMainPageTimer();
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x0000648F File Offset: 0x0000468F
		public void ShowCloseCameraSettings()
		{
			if (ActivationKingWindow.CameraSettingsWindow == null)
			{
				ActivationKingWindow.OpenCameraSettingsWindow();
				return;
			}
			if (ActivationKingWindow.CameraSettingsWindow.Visibility == Visibility.Visible)
			{
				ActivationKingWindow.CloseCameraSettingsWindow();
				return;
			}
			ActivationKingWindow.OpenCameraSettingsWindow();
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000064B5 File Offset: 0x000046B5
		private void GridReturn_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (!ActivationKingWindow.IsAdminPageActive)
			{
				this.waitSecond = 0;
				this.returnNumber++;
				if (this.returnNumber > 4)
				{
					this.returnNumber = 0;
					this.ReturnSettingsPage();
				}
			}
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000064EC File Offset: 0x000046EC
		public void ReturnSettingsPage()
		{
			bool IsHaveReturnPass = !string.IsNullOrEmpty(Settings.GetValueString("returnpassword"));
			if (IsHaveReturnPass)
			{
				this.debugPassText.Text = "";
				this.ReturnEventMenuGrid.Visibility = Visibility.Visible;
				return;
			}
			ActivationKingWindow.SetPage(ApplicationPage.SettingsPage, false);
		}

		// Token: 0x060000DA RID: 218 RVA: 0x00006534 File Offset: 0x00004734
		private void txtDebugConsole_TextChanged(object sender, TextChangedEventArgs e)
		{
			System.Windows.Controls.TextBox textBox = sender as System.Windows.Controls.TextBox;
			if (textBox != null && textBox.Text != ActivationKingWindow.debugText)
			{
				textBox.Text = ActivationKingWindow.debugText;
				textBox.SelectionStart = textBox.Text.Length;
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00006579 File Offset: 0x00004779
		private void txtDebugConsole_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			this.gridMain.Focus();
		}

		// Token: 0x060000DC RID: 220 RVA: 0x00006587 File Offset: 0x00004787
		private void CloseKeyboardButton_Click(object sender, RoutedEventArgs e)
		{
			this.gridKeyboard.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00006595 File Offset: 0x00004795
		private void m_Closed(object sender, EventArgs e)
		{
			App.ExitApp();
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000659C File Offset: 0x0000479C
		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.ReturnEventMenuGrid.Visibility = Visibility.Hidden;
			this.passwordBox.Password = "";
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000065BC File Offset: 0x000047BC
		private void btnOk_Click(object sender, RoutedEventArgs e)
		{
			string pass = Settings.GetValueString("returnpassword");
			if (pass == this.passwordBox.Password)
			{
				this.ReturnEventMenuGrid.Visibility = Visibility.Hidden;
				this.passwordBox.Password = "";
				ActivationKingWindow.SetPage(ApplicationPage.SettingsPage, false);
				return;
			}
			this.debugPassText.Text = "Wrong Code";
			this.ClearDebugPassText();
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x00006621 File Offset: 0x00004821
		private void ClearDebugPassText()
		{
			new Thread(delegate()
			{
				Thread.Sleep(3000);
				base.Dispatcher.Invoke(delegate()
				{
					this.debugPassText.Text = "";
				});
			}).Start();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000663C File Offset: 0x0000483C
		private void ForgotPassButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.<ForgotPassButton_Click>d__114 <ForgotPassButton_Click>d__;
			<ForgotPassButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<ForgotPassButton_Click>d__.<>4__this = this;
			<ForgotPassButton_Click>d__.<>1__state = -1;
			<ForgotPassButton_Click>d__.<>t__builder.Start<ActivationKingWindow.<ForgotPassButton_Click>d__114>(ref <ForgotPassButton_Click>d__);
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x00006673 File Offset: 0x00004873
		private void RefreshMainPageTimer()
		{
			ActivationKingWindow.returnMainPageTimer = 0;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000667B File Offset: 0x0000487B
		private void m_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			this.RefreshMainPageTimer();
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x00006683 File Offset: 0x00004883
		private void m_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.RefreshMainPageTimer();
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000668B File Offset: 0x0000488B
		private void Window_PreviewTouchDown(object sender, TouchEventArgs e)
		{
			this.RefreshMainPageTimer();
		}

		// Token: 0x04000088 RID: 136
		private static bool appActive = true;

		// Token: 0x04000089 RID: 137
		private static int returnMainPageTimer = 0;

		// Token: 0x0400008A RID: 138
		private static int returnMainPageTimerStatic = 90;

		// Token: 0x0400008D RID: 141
		public static ApplicationPage CurrentPage = ApplicationPage.LoginPage;

		// Token: 0x0400009D RID: 157
		private int returnNumber;

		// Token: 0x0400009E RID: 158
		private int waitSecond;

		// Token: 0x0400009F RID: 159
		private double[] originalSizes;

		// Token: 0x040000A0 RID: 160
		private string[] resourses = new string[]
		{
			"FontSizeDisplay1",
			"FontSizeDisplay2",
			"FontSizeHeading1",
			"FontSizeHeading2",
			"FontSizeHeading3",
			"FontSizeTextTitle",
			"FontSizeTextRegular",
			"FontSizeTextSmall",
			"FontSizeButtonPrimary",
			"FontSizeButtonSmall",
			"UIInputHeight",
			"SliderTextBoxWidth",
			"SliderTextBoxHeight",
			"EventListElementHeight",
			"CheckBoxHeight",
			"UICornerRadiusValue",
			"FontSizeButtonExtraSmall"
		};

		// Token: 0x040000A1 RID: 161
		private double originalUICornerRadiusValue;

		// Token: 0x040000A2 RID: 162
		private double originalBannerCornerRadiusValue;

		// Token: 0x040000A3 RID: 163
		private double originalSectionCornerRadiusValue;

		// Token: 0x040000A4 RID: 164
		private static string debugText = "";
	}
}
