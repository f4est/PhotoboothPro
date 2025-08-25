using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Services;
using Microsoft.Win32;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000035 RID: 53
	public partial class DSLR : Page
	{
		// Token: 0x060002F6 RID: 758 RVA: 0x0000F594 File Offset: 0x0000D794
		public DSLR()
		{
			this.InitializeComponent();
			this.gridF11Warning.FadeIn(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0));
			this.gridF11Warning.FadeOut(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(5.0));
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				Motion360Platform.Instance.Activate();
			}
		}

		// Token: 0x060002F7 RID: 759 RVA: 0x0000F644 File Offset: 0x0000D844
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.isOpenLPPD = Settings.GetValueBoolean("lppdactive").Value;
			ActivationKingWindow.CloseCameraSettingsWindow();
			this.isCameraPreview = (Settings.GetValueBoolean("backgroundpreviewvideo").Value && MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR);
			this.LiveViewScaleTransform.ScaleX = (double)(Settings.GetValueBoolean("mirrorview").Value ? -1 : 1);
			if (this.isCameraPreview)
			{
				double brightness = (double)Settings.GetValueFloat("previewbrightness").GetValueOrDefault(-1f);
				if (brightness > 100.0)
				{
					brightness /= 100.0;
				}
				if (brightness > -1.0)
				{
					this.CameraProperties = CameraControlClass.GetCameraBrightness(null);
					CameraControlClass.SetCameraBrightness(brightness);
				}
			}
			DSLR.isFileImport = false;
			CameraControlClass.FolderCheck();
			this.PromptBox.Text = (DSLR.NameAndSurname = "");
			this.NameSurnameGrid.Visibility = (TemplateClass.IsHaveText ? Visibility.Visible : Visibility.Hidden);
			this.PromptBoxPlaceholder.Visibility = Visibility.Visible;
			this.PromptBoxPlaceholder.Content = Settings.GetValueString("selectedfontplaceholdertext");
			this.LoadLanguageStrings();
			DSLR.isPhoto = (DSLR.isImportFile = false);
			this.MirrorBoothVideoTimeCount = 150;
			this.ManageMirrorBoothVideo();
			DSLR.GetEventMedias();
			DSLR.ManageBackgroundJobs();
			double.TryParse(Settings.GetValueString("printscaletop"), NumberStyles.Any, CultureInfo.InvariantCulture, out PrinterPage.printTop);
			double.TryParse(Settings.GetValueString("printscalebottom"), NumberStyles.Any, CultureInfo.InvariantCulture, out PrinterPage.printBottom);
			double.TryParse(Settings.GetValueString("printscaleleft"), NumberStyles.Any, CultureInfo.InvariantCulture, out PrinterPage.printLeft);
			double.TryParse(Settings.GetValueString("printscaleright"), NumberStyles.Any, CultureInfo.InvariantCulture, out PrinterPage.printRight);
			ActivationKingWindow.Instance.TrialWatermarkControl();
			bool isbackgroundImage = Settings.GetValueBoolean("mainbackgroundimagecheck").GetValueOrDefault();
			if (isbackgroundImage && File.Exists(Settings.GetValueString("mainbackgroundimage") ?? "0.xyz"))
			{
				BitmapImage thisImage = new BitmapImage();
				thisImage.BeginInit();
				thisImage.UriSource = new Uri(Settings.GetValueString("mainbackgroundimage"), UriKind.Absolute);
				thisImage.EndInit();
				this.BackgroundImage.Source = thisImage;
			}
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR || MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
			{
				DSLR.isHaveGreenBox = Settings.GetValueBoolean("greenbox").GetValueOrDefault();
				DSLR.isHaveAIBackground = Settings.GetValueBoolean("aibackground").GetValueOrDefault();
				this.isShowPhoto = Settings.GetValueBoolean("photo").GetValueOrDefault();
				this.isShowVideo = Settings.GetValueBoolean("video").GetValueOrDefault();
			}
			else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				this.isShowVideo = true;
				GoProCameraControlClass.SendKeepAlive();
				GoProCameraControlClass.GoProSet120FPS();
				GoProCameraControlClass.GoProSetVideoMode();
			}
			else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.NoCamera)
			{
				this.isShowPhoto = false;
				this.isShowVideo = false;
			}
			this.isShowImport = Settings.GetValueBoolean("upload").GetValueOrDefault();
			this.isShowGallery = Settings.GetValueBoolean("gallery").GetValueOrDefault();
			DSLR.printlimit = ((Settings.GetValueInt("printLimit") == null) ? -1 : Settings.GetValueInt("printLimit").Value);
			this.CheckButtonsGrid();
			PrinterPage.printedNumber = PrintClass.GetPrintNumber();
			if (!TemplateClass.isTemplateOpen)
			{
				TemplateClass.ReadJsonTemplateObjects(true);
				TemplateClass.ReadJsonTemplateObjects(false);
				TemplateClass.SaveBackgroundAndWatermark(false);
				TemplateClass.SaveBackgroundAndWatermark(true);
				TemplateClass.isTemplateOpen = true;
			}
			string backgroundValue = this.isShowPhoto ? TemplateClass.backgroundPhotoKey : TemplateClass.backgroundVideoKey;
			if (File.Exists(Settings.GetValueString(backgroundValue)))
			{
				CameraControlClass.BackgroundPath = Settings.GetValueString(backgroundValue);
				CameraControlClass.LoadBackground(CameraControlClass.BackgroundPath);
				CameraControlClass.isHaveBackground = true;
			}
			else
			{
				CameraControlClass.isHaveBackground = false;
			}
			LiveViewClass.ReceiveLiveImage += this.LiveViewClass_ReceiveLiveImage;
			CameraControlClass.ReconnectedCameraEvent += this.CameraControlClass_ReconnectedCameraEvent;
			this.PhotoButton.IsEnabled = (this.VideoButton.IsEnabled = false);
			this.startLoad = (!LiveViewClass.liveActive || CameraControlClass.isCameraDisconnect);
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					if (this.startLoad)
					{
						this.startLoad = false;
						new Thread(delegate()
						{
							CameraControlClass.SetCamera(false, CameraControlClass.selectedCameraDeviceName, true);
						}).Start();
						new Thread(delegate()
						{
							while (!this.firstFrame && ActivationKingWindow.CurrentPage == ApplicationPage.HomePage)
							{
								Thread.Sleep(3000);
								if (DSLR.IsLoadCamera)
								{
									DispatcherOperation operation = base.Dispatcher.BeginInvoke(new Action(delegate()
									{
										if (!this.PhotoButton.IsEnabled)
										{
											this.PhotoButton.IsEnabled = (this.VideoButton.IsEnabled = (this.GalleryButton.IsEnabled = true));
										}
									}), DispatcherPriority.Normal, Array.Empty<object>());
									return;
								}
								if (!this.firstFrame)
								{
									CameraControlClass.SetCamera(false, CameraControlClass.selectedCameraDeviceName, true);
								}
							}
						}).Start();
						return;
					}
					CameraControlClass.StartLiveCamera();
				}), Array.Empty<object>());
			}
			else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				this.LiveViewClass_ReceiveLiveImage(true);
			}
			new Thread(delegate()
			{
				Thread.Sleep(3500);
				DispatcherOperation operation = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					if (DSLR.IsLoadCamera && !this.PhotoButton.IsEnabled)
					{
						this.PhotoButton.IsEnabled = (this.VideoButton.IsEnabled = (this.GalleryButton.IsEnabled = true));
					}
				}), DispatcherPriority.Normal, Array.Empty<object>());
			}).Start();
			GoProCameraControlClass.isSlowMotion = Settings.GetValueBoolean("slowmotionenabled").GetValueOrDefault();
			GoProCameraControlClass.videoLength = Settings.GetValueInt("videotime").GetValueOrDefault(6);
			float? valueFloat = Settings.GetValueFloat("slowmotionstart");
			GoProCameraControlClass.slowMotionVideoStart = ((valueFloat != null) ? ((double)valueFloat.GetValueOrDefault()) : 2.0);
			valueFloat = Settings.GetValueFloat("slowmotionend");
			GoProCameraControlClass.slowMotionVideoEnd = ((valueFloat != null) ? ((double)valueFloat.GetValueOrDefault()) : 4.0);
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
			{
				this.LoadingContainerPhoto.Visibility = (this.LoadingContainerVideo.Visibility = Visibility.Collapsed);
				this.PhotoButton.IsEnabled = (this.VideoButton.IsEnabled = (this.GalleryButton.IsEnabled = true));
				this.IconLoadingBackground1.Visibility = (this.IconLoadingBackground2.Visibility = Visibility.Collapsed);
			}
			else
			{
				WebcamControlClass.WebcamThreadControl();
			}
			if (Settings.GetValueBoolean("faceswapactive").Value)
			{
				this.loadingGifGrid.Visibility = Visibility.Visible;
				Task.Run(delegate()
				{
					DSLR.<<Page_Loaded>b__39_1>d <<Page_Loaded>b__39_1>d;
					<<Page_Loaded>b__39_1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<Page_Loaded>b__39_1>d.<>4__this = this;
					<<Page_Loaded>b__39_1>d.<>1__state = -1;
					<<Page_Loaded>b__39_1>d.<>t__builder.Start<DSLR.<<Page_Loaded>b__39_1>d>(ref <<Page_Loaded>b__39_1>d);
					return <<Page_Loaded>b__39_1>d.<>t__builder.Task;
				}).ContinueWith(delegate(Task t)
				{
					this.loadingGifGrid.Visibility = Visibility.Collapsed;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			if (Settings.GetValueBoolean("aipromptoptionactive").Value)
			{
				this.loadingGifGrid.Visibility = Visibility.Visible;
				Task.Run(delegate()
				{
					DSLR.<<Page_Loaded>b__39_3>d <<Page_Loaded>b__39_3>d;
					<<Page_Loaded>b__39_3>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<Page_Loaded>b__39_3>d.<>4__this = this;
					<<Page_Loaded>b__39_3>d.<>1__state = -1;
					<<Page_Loaded>b__39_3>d.<>t__builder.Start<DSLR.<<Page_Loaded>b__39_3>d>(ref <<Page_Loaded>b__39_3>d);
					return <<Page_Loaded>b__39_3>d.<>t__builder.Task;
				}).ContinueWith(delegate(Task t)
				{
					this.loadingGifGrid.Visibility = Visibility.Collapsed;
				}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			this.GlambotUiManagement();
			if (this.isPreviewCameraBackgroundActive)
			{
				WebcamControlClass.LiveViewImageLoaded += this.WebcamControlClass_LiveViewImageLoaded;
				LiveViewClass.LiveViewImageLoaded += this.LiveViewClass_LiveViewImageLoaded;
			}
			new Thread(delegate()
			{
				DSLR.<<Page_Loaded>b__39_5>d <<Page_Loaded>b__39_5>d;
				<<Page_Loaded>b__39_5>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<Page_Loaded>b__39_5>d.<>4__this = this;
				<<Page_Loaded>b__39_5>d.<>1__state = -1;
				<<Page_Loaded>b__39_5>d.<>t__builder.Start<DSLR.<<Page_Loaded>b__39_5>d>(ref <<Page_Loaded>b__39_5>d);
			}).Start();
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				Motion360Platform.Instance.SetTime(5);
			}
			this.isPayment = StripeController.IsPayment(PaymentScreen.AtStartScreen);
			this.TotalAmount = 0f;
			if (this.isPayment)
			{
				this.LoadPaymentsLanguageStrings();
				DSLR.downloadPrice = Settings.GetValueFloat("captureprice").Value;
				DSLR.PrintPrice = Settings.GetValueFloat("printprice").Value;
				if (DSLR.currentPaymentSession != null && DSLR.currentPaymentSession.IsHaveCredit)
				{
					this.PayingDigitalPhotoButtonGrid.Visibility = ((DSLR.currentPaymentSession.downloadCredit > 0) ? Visibility.Visible : Visibility.Collapsed);
					this.PayingPhotoButtonGrid.Visibility = ((DSLR.currentPaymentSession.printCredit > 0) ? Visibility.Visible : Visibility.Collapsed);
					this.emptyGrid.Visibility = ((DSLR.currentPaymentSession.printCredit < 1 || DSLR.currentPaymentSession.downloadCredit < 1) ? Visibility.Collapsed : Visibility.Visible);
					this.PaymentSessionInfoText.Text = this.CalcPaymentSessionInfoText();
					this.loadingPaymentGrid.Visibility = (this.MainPaymentPage.Visibility = Visibility.Collapsed);
					this.PaymentGrid.Visibility = (this.SuccessPaymentGrid.Visibility = Visibility.Visible);
				}
			}
			Keyboard.Focus(this);
		}

		// Token: 0x060002F8 RID: 760 RVA: 0x0000FDAC File Offset: 0x0000DFAC
		private string CalcPaymentSessionInfoText()
		{
			string PaymentConfirmationPageString = Settings.GetValueString("lang_paymentconfirmationtext", true);
			string printText = Settings.GetValueString("lang_paymentmainprinttitle", true);
			string downloadText = Settings.GetValueString("lang_paymentmaindigitaltitle", true);
			if (DSLR.currentPaymentSession.printCredit > 0 && DSLR.currentPaymentSession.downloadCredit > 0)
			{
				return string.Format("{0} {1} {2}, {3} {4}", new object[]
				{
					PaymentConfirmationPageString,
					DSLR.currentPaymentSession.printCredit,
					printText,
					DSLR.currentPaymentSession.downloadCredit,
					downloadText
				});
			}
			if (DSLR.currentPaymentSession.printCredit > 0)
			{
				return string.Format("{0} {1} {2}", PaymentConfirmationPageString, DSLR.currentPaymentSession.printCredit, printText);
			}
			return string.Format("{0} {1} {2}", PaymentConfirmationPageString, DSLR.currentPaymentSession.downloadCredit, downloadText);
		}

		// Token: 0x060002F9 RID: 761 RVA: 0x0000FE7C File Offset: 0x0000E07C
		private void LiveViewClass_LiveViewImageLoaded()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (this.firstFrame && this.isPreviewCameraBackgroundActive)
				{
					this.gridPreviewBackgroundImage.Source = LiveViewClass.staticImage;
				}
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060002FA RID: 762 RVA: 0x0000FEA8 File Offset: 0x0000E0A8
		private void WebcamControlClass_LiveViewImageLoaded(byte[] imageData)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				BitmapImage bitmapImage = new BitmapImage();
				using (MemoryStream ms = new MemoryStream(imageData))
				{
					bitmapImage.BeginInit();
					bitmapImage.StreamSource = ms;
					bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
					bitmapImage.EndInit();
				}
				bitmapImage.Freeze();
				string rotation = Settings.GetValueString("rotation") ?? "0";
				if (rotation.Contains("90"))
				{
					TransformedBitmap rotatedBitmap = new TransformedBitmap(bitmapImage, new RotateTransform(90.0));
					this.gridPreviewBackgroundImage.Source = rotatedBitmap;
					return;
				}
				if (rotation.Contains("180"))
				{
					TransformedBitmap rotatedBitmap2 = new TransformedBitmap(bitmapImage, new RotateTransform(180.0));
					this.gridPreviewBackgroundImage.Source = rotatedBitmap2;
					return;
				}
				if (rotation.Contains("270"))
				{
					TransformedBitmap rotatedBitmap3 = new TransformedBitmap(bitmapImage, new RotateTransform(270.0));
					this.gridPreviewBackgroundImage.Source = rotatedBitmap3;
					return;
				}
				this.gridPreviewBackgroundImage.Source = bitmapImage;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060002FB RID: 763 RVA: 0x0000FEE8 File Offset: 0x0000E0E8
		private void Logout(string message)
		{
			MessageBoxWindow.CreateWindow("Logged Out", "This licence is activated by another device. You can buy a new license to continue to use. " + message, new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Continue
			}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.StartSalesWebsite), MessageBoxWindow.MessageBoxSize.Large, false);
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				ActivationKingWindow.SetPage(ApplicationPage.LoginPage, true);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060002FC RID: 764 RVA: 0x0000FF58 File Offset: 0x0000E158
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

		// Token: 0x060002FD RID: 765 RVA: 0x0000FF8C File Offset: 0x0000E18C
		private static void GetEventMedias()
		{
			if (DSLR.eventMediaClass == null)
			{
				if (File.Exists(EventManagementPage.GetCurrentEvent().EventMediaJsonPath))
				{
					DSLR.eventMediaClass = ExtensionMethod.ReadJson<EventMediaClass>(EventManagementPage.GetCurrentEvent().EventMediaJsonPath);
					return;
				}
				DSLR.eventMediaClass = new EventMediaClass();
			}
		}

		// Token: 0x060002FE RID: 766 RVA: 0x0000FFC8 File Offset: 0x0000E1C8
		private static void ManageBackgroundJobs()
		{
			BunnyCDNHelper.isForceQuit = false;
			MailClass.isForceQuit = false;
			TwilioHelper.isForceQuit = false;
			VariationMediaHelper.isForceQuit = false;
			BunnyCDNHelper.StartUploadThread();
			if (Settings.GetValueBoolean("mailenable") != null && Settings.GetValueBoolean("mailenable").Value)
			{
				MailClass.StartEmailThread();
			}
			if (Settings.GetValueBoolean("smsenable") != null && Settings.GetValueBoolean("smsenable").Value)
			{
				TwilioHelper.StartSMSThread();
			}
		}

		// Token: 0x060002FF RID: 767 RVA: 0x00010054 File Offset: 0x0000E254
		private void GlambotUiManagement()
		{
			if (MainSettingsPage.SelectedMode == Mode.Glambot)
			{
				this.GlambotHomePageMainGrid.Visibility = Visibility.Visible;
			}
			else
			{
				this.GlambotHomePageMainGrid.Visibility = Visibility.Collapsed;
			}
			this.GlambotSelectionMainGrid.Visibility = Visibility.Collapsed;
			this.isPreviewCameraBackgroundActive = Settings.GetValueBoolean("backgroundpreviewvideo").GetValueOrDefault();
			if (this.isPreviewCameraBackgroundActive)
			{
				this.gridPreviewBackground.Visibility = Visibility.Visible;
				return;
			}
			this.gridPreviewBackground.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000300 RID: 768 RVA: 0x000100C8 File Offset: 0x0000E2C8
		private void ManageMirrorBoothVideo()
		{
			string videoPath = Settings.GetValueString("mainbackgroundvideo") ?? "0.xyz";
			bool isVideoEnabled = Settings.GetValueBoolean("mainbackgroundvideocheck").GetValueOrDefault();
			if (File.Exists(videoPath) && isVideoEnabled)
			{
				this.mirrorBoothThreadActive = true;
				this.MirrorBoothVideoThread = new Thread(new ThreadStart(this.MirrorBoothVideoControl));
				this.MirrorBoothVideoThread.SetApartmentState(ApartmentState.STA);
				this.MirrorBoothVideoThread.Start();
				return;
			}
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
		}

		// Token: 0x06000301 RID: 769 RVA: 0x0001015C File Offset: 0x0000E35C
		private void MirrorBoothVideoControl()
		{
			while (this.mirrorBoothThreadActive)
			{
				if (this.MirrorBoothVideoTimeCount > 120)
				{
					this.mirrorBoothThreadActive = false;
					this.MirrorBoothVideoTimeCount = 0;
					Application.Current.Dispatcher.Invoke(delegate()
					{
						string videoPath = Settings.GetValueString("mainbackgroundvideo") ?? "0.xyz";
						this.videoPlayer.MediaEnded += this.VideoPlayer_MediaEnded;
						this.videoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
						this.adsVideoGrid.Visibility = Visibility.Visible;
						this.videoPlayer.Volume = 100.0;
					});
				}
				else
				{
					this.MirrorBoothVideoTimeCount++;
				}
				Thread.Sleep(250);
			}
		}

		// Token: 0x06000302 RID: 770 RVA: 0x000101C0 File Offset: 0x0000E3C0
		public bool VideoVisibility()
		{
			return MainSettingsPage.SelectedMode == Mode.MultifunctionalPhotoBooth || MainSettingsPage.SelectedMode == Mode.Glambot;
		}

		// Token: 0x06000303 RID: 771 RVA: 0x000101D4 File Offset: 0x0000E3D4
		private void CheckButtonsGrid()
		{
			this.gridPhoto.Visibility = (this.isShowPhoto ? Visibility.Visible : Visibility.Collapsed);
			this.gridVideo.Visibility = ((this.isShowVideo && this.VideoVisibility()) ? Visibility.Visible : Visibility.Collapsed);
			this.gridImport.Visibility = (this.isShowImport ? Visibility.Visible : Visibility.Collapsed);
			this.gridGallery.Visibility = (this.isShowGallery ? Visibility.Visible : Visibility.Collapsed);
			this.gridButtons.Width = Math.Min(base.ActualWidth, base.ActualHeight) * 0.9;
			this.gridButtons.Height = this.gridPhoto.Width;
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00010281 File Offset: 0x0000E481
		private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.videoPlayer.Position = TimeSpan.Zero;
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00010294 File Offset: 0x0000E494
		private void LoadLanguageStrings()
		{
			string photoButtonLabel = Settings.GetValueString("lang_photo", true);
			if (!string.IsNullOrEmpty(photoButtonLabel))
			{
				this.PhotoButtonLabel.Text = photoButtonLabel;
			}
			string videoButtonLabel = Settings.GetValueString("lang_video", true);
			if (!string.IsNullOrEmpty(videoButtonLabel))
			{
				this.VideoButtonLabel.Text = videoButtonLabel;
			}
			string galleryButtonLabel = Settings.GetValueString("lang_gallery", true);
			if (!string.IsNullOrEmpty(galleryButtonLabel))
			{
				this.GalleryButtonLabel.Text = galleryButtonLabel;
			}
			string importButtonLabel = Settings.GetValueString("lang_import", true);
			if (!string.IsNullOrEmpty(importButtonLabel))
			{
				this.ImportButtonLabel.Text = importButtonLabel;
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00010324 File Offset: 0x0000E524
		public void LoadLanguageString2Object(string key, UIElement textBlock)
		{
			string text = Settings.GetValueString(key, true);
			if (!string.IsNullOrEmpty(text))
			{
				TextBlock tb = textBlock as TextBlock;
				if (tb != null)
				{
					tb.Text = text;
					return;
				}
				Button btn = textBlock as Button;
				if (btn != null)
				{
					btn.Content = text;
				}
			}
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00010364 File Offset: 0x0000E564
		private void LoadPaymentsLanguageStrings()
		{
			List<string> keyList = new List<string>
			{
				"lang_paymentmaintitle",
				"lang_paymentmaintext",
				"lang_paymentmainpaybutton",
				"lang_paymentmainbackbutton",
				"lang_paymentmainprinttitle",
				"lang_paymentmainprinttext",
				"lang_paymentmaindigitaltitle",
				"lang_paymentmaindigitaltext",
				"lang_paymentloadingtext",
				"lang_paymentloadingtitle",
				"lang_paymentloadingbackbutton",
				"lang_paymentloadingscanqrtext",
				"lang_paymentconfirmationtitle",
				"lang_paymentconfirmationtext",
				"lang_paymentconfirmationbackbutton",
				"lang_paymentcorfirmationprintbutton",
				"lang_paymentconfirmationdigitalbutton"
			};
			List<UIElement> textObjects = new List<UIElement>
			{
				this.MainPaymentTitle,
				this.MainPaymentText,
				this.btnApply,
				this.MainPaymentBackButton,
				this.PaymentMainPagePrintTitle,
				this.PaymentMainPagePrintText,
				this.MainPageDigitalTitle,
				this.PaymentMainPageDigitalText,
				this.PaymentLoadingPageText,
				this.PaymentLoadingPageTitle,
				this.DeleteButton,
				this.PaymentLoadingPageScanQRText,
				this.ConfirmationPageTitle,
				this.PaymentSessionInfoText,
				this.RemoveCreditButton,
				this.ConfirmationPagePrintText,
				this.digitalText
			};
			for (int i = 0; i < keyList.Count; i++)
			{
				this.LoadLanguageString2Object(keyList[i], textObjects[i]);
			}
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00010529 File Offset: 0x0000E729
		private void CameraControlClass_ReconnectedCameraEvent()
		{
			if (!ActivationKingWindow.IsAdminPageActive)
			{
				this.firstFrame = false;
				this.Page_Loaded(null, null);
			}
		}

		// Token: 0x06000309 RID: 777 RVA: 0x00010544 File Offset: 0x0000E744
		private void LiveViewClass_ReceiveLiveImage(bool isFirstReceive)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (isFirstReceive && !this.firstFrame)
				{
					this.LoadingContainerPhoto.Visibility = (this.LoadingContainerVideo.Visibility = Visibility.Collapsed);
					this.PhotoButton.IsEnabled = (this.VideoButton.IsEnabled = (this.GalleryButton.IsEnabled = true));
					this.IconLoadingBackground1.Visibility = (this.IconLoadingBackground2.Visibility = Visibility.Collapsed);
					CameraControlClass.isConnectedCamera = true;
					CameraControlClass.isCameraDisconnect = false;
					this.firstFrame = true;
					DSLR.IsLoadCamera = true;
				}
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0001058C File Offset: 0x0000E78C
		public void TakePhoto()
		{
			LiveViewClass.IsLiveViewTimerEnabled = true;
			DSLR.isPhoto = true;
			if ((from x in TemplateClass.templateJsonObjects
			where x.templatePhotoObjects.Count<TemplateObject>() > 1
			select x).Count<TemplateJsonObject>() > 1)
			{
				ActivationKingWindow.SetPage(ApplicationPage.TemplateGalleryPage, true);
				return;
			}
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				this.SetAIPage();
				return;
			}
			ActivationKingWindow.SetPage(ApplicationPage.PhotoShootPage, true);
		}

		// Token: 0x0600030B RID: 779 RVA: 0x000105F5 File Offset: 0x0000E7F5
		private void SetAIPage()
		{
			ActivationKingWindow.SetPage(MainSettingsPage.SelectAIPage(), true);
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00010604 File Offset: 0x0000E804
		private void PhotoButton_Click(object sender, RoutedEventArgs e)
		{
			DSLR.<PhotoButton_Click>d__60 <PhotoButton_Click>d__;
			<PhotoButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PhotoButton_Click>d__.<>4__this = this;
			<PhotoButton_Click>d__.<>1__state = -1;
			<PhotoButton_Click>d__.<>t__builder.Start<DSLR.<PhotoButton_Click>d__60>(ref <PhotoButton_Click>d__);
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0001063C File Offset: 0x0000E83C
		private List<string> SelectFile(int SelectID)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			switch (SelectID)
			{
			case 0:
				openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
				break;
			case 1:
				openFileDialog.Filter = "Video Files (*.mp4)|*.mp4";
				break;
			case 2:
				openFileDialog.Filter = "Image And Video Files (*.png;*.jpg;*.jpeg;*.mp4)|*.png;*.jpg;*.jpeg;*.mp4";
				break;
			}
			openFileDialog.Multiselect = (TemplateClass.capturePhotoCount > 1);
			bool? result = openFileDialog.ShowDialog();
			if (!result.GetValueOrDefault())
			{
				return new List<string>();
			}
			if (TemplateClass.capturePhotoCount != openFileDialog.FileNames.Length)
			{
				MessageBoxWindow.CreateWindow("Not Enough Photos", "Please select the amount of photos specified on the Template page.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
			else if (result.GetValueOrDefault())
			{
				return openFileDialog.FileNames.ToList<string>();
			}
			return new List<string>();
		}

		// Token: 0x0600030E RID: 782 RVA: 0x000106F6 File Offset: 0x0000E8F6
		private int FindSelectableFileFormatID()
		{
			if (MainSettingsPage.SelectedMode > Mode.MultifunctionalPhotoBooth || TemplateClass.capturePhotoCount > 1)
			{
				return 0;
			}
			return 2;
		}

		// Token: 0x0600030F RID: 783 RVA: 0x0001070B File Offset: 0x0000E90B
		private void ControlPhotoQuality(string path, string savePath)
		{
		}

		// Token: 0x06000310 RID: 784 RVA: 0x00010710 File Offset: 0x0000E910
		private void FileImportFromPC()
		{
			DSLR.<FileImportFromPC>d__64 <FileImportFromPC>d__;
			<FileImportFromPC>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<FileImportFromPC>d__.<>4__this = this;
			<FileImportFromPC>d__.<>1__state = -1;
			<FileImportFromPC>d__.<>t__builder.Start<DSLR.<FileImportFromPC>d__64>(ref <FileImportFromPC>d__);
		}

		// Token: 0x06000311 RID: 785 RVA: 0x00010748 File Offset: 0x0000E948
		private void GoDirectProcessPage(List<string> capturedFiles, MediaType mediatype)
		{
			bool multiTemplate = (from x in TemplateClass.templateJsonObjects
			where x.templatePhotoObjects.Count<TemplateObject>() > 1
			select x).Count<TemplateJsonObject>() > 1;
			ApplicationPage pageToChange;
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				DSLR.isPhoto = (mediatype == MediaType.photo);
				pageToChange = MainSettingsPage.SelectAIPage();
			}
			else if (mediatype == MediaType.video)
			{
				pageToChange = ApplicationPage.VideoShootPage;
				multiTemplate = ((from x in TemplateClass.templateJsonObjects
				where x.templateVideoObjects.Count<TemplateObject>() > 1
				select x).Count<TemplateJsonObject>() > 1);
			}
			else
			{
				if (mediatype != MediaType.photo)
				{
					return;
				}
				DSLR.isPhoto = true;
				pageToChange = ApplicationPage.PhotoShootPage;
			}
			if (multiTemplate)
			{
				pageToChange = ApplicationPage.TemplateGalleryPage;
			}
			DSLR.isImportFile = true;
			DSLR.importFilePaths = capturedFiles;
			ActivationKingWindow.SetPage(pageToChange, true);
		}

		// Token: 0x06000312 RID: 786 RVA: 0x00010801 File Offset: 0x0000EA01
		private void GifButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.SetPage(ApplicationPage.GifShootPage, true);
		}

		// Token: 0x06000313 RID: 787 RVA: 0x0001080C File Offset: 0x0000EA0C
		private void VideoButton_Click(object sender, RoutedEventArgs e)
		{
			DSLR.<VideoButton_Click>d__67 <VideoButton_Click>d__;
			<VideoButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<VideoButton_Click>d__.<>1__state = -1;
			<VideoButton_Click>d__.<>t__builder.Start<DSLR.<VideoButton_Click>d__67>(ref <VideoButton_Click>d__);
		}

		// Token: 0x06000314 RID: 788 RVA: 0x0001083C File Offset: 0x0000EA3C
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.isCameraPreview)
			{
				CameraControlClass.SetCameraBrightness(this.CameraProperties);
			}
			LiveViewClass.ReceiveLiveImage -= this.LiveViewClass_ReceiveLiveImage;
			CameraControlClass.ReconnectedCameraEvent -= this.CameraControlClass_ReconnectedCameraEvent;
			ActivationKingWindow.Instance.TrialWatermarkControl();
			this.mirrorBoothThreadActive = false;
			if (this.isPreviewCameraBackgroundActive)
			{
				WebcamControlClass.LiveViewImageLoaded -= this.WebcamControlClass_LiveViewImageLoaded;
				LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			}
			this.firstFrame = false;
			if (this.isWaitingPayment)
			{
				this.PaymentCancellationToken.Cancel();
			}
		}

		// Token: 0x06000315 RID: 789 RVA: 0x000108D3 File Offset: 0x0000EAD3
		private void GridReturn_MouseDown(object sender, MouseButtonEventArgs e)
		{
		}

		// Token: 0x06000316 RID: 790 RVA: 0x000108D5 File Offset: 0x0000EAD5
		private void GalleryButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.SetPage(ApplicationPage.GalleryPage, false);
		}

		// Token: 0x06000317 RID: 791 RVA: 0x000108E0 File Offset: 0x0000EAE0
		public void DeleteUselessFaceSwapImages()
		{
			FaceSwapBackgroundGallery.FaceSwapData faceSwapData = null;
			string jsonFilePath = FaceSwap.faceSwapJSONPath;
			if (File.Exists(jsonFilePath))
			{
				faceSwapData = ExtensionMethod.ReadJson<FaceSwapBackgroundGallery.FaceSwapData>(jsonFilePath);
			}
			if (faceSwapData == null)
			{
				return;
			}
			List<string> faceSwapTargetNames = faceSwapData.FaceSwapTargets.SelectMany((FaceSwapTarget item) => new List<string>
			{
				item.FileDetails.Filename,
				"00" + item.FileDetails.Filename
			}).ToList<string>();
			string directoryPath = Path.Combine(new string[]
			{
				FaceSwap.imageFolderPath
			});
			if (Directory.Exists(directoryPath))
			{
				DirectoryInfo di = new DirectoryInfo(directoryPath);
				foreach (FileInfo file in di.GetFiles())
				{
					try
					{
						if (!faceSwapTargetNames.Contains(file.Name))
						{
							file.Delete();
						}
					}
					catch (Exception e)
					{
					}
				}
			}
		}

		// Token: 0x06000318 RID: 792 RVA: 0x000109AC File Offset: 0x0000EBAC
		public void UpdatePrompts()
		{
			DSLR.<UpdatePrompts>d__72 <UpdatePrompts>d__;
			<UpdatePrompts>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<UpdatePrompts>d__.<>1__state = -1;
			<UpdatePrompts>d__.<>t__builder.Start<DSLR.<UpdatePrompts>d__72>(ref <UpdatePrompts>d__);
		}

		// Token: 0x06000319 RID: 793 RVA: 0x000109DC File Offset: 0x0000EBDC
		public Task UpdateFaceSwapPhotosAsync()
		{
			DSLR.<UpdateFaceSwapPhotosAsync>d__73 <UpdateFaceSwapPhotosAsync>d__;
			<UpdateFaceSwapPhotosAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateFaceSwapPhotosAsync>d__.<>4__this = this;
			<UpdateFaceSwapPhotosAsync>d__.<>1__state = -1;
			<UpdateFaceSwapPhotosAsync>d__.<>t__builder.Start<DSLR.<UpdateFaceSwapPhotosAsync>d__73>(ref <UpdateFaceSwapPhotosAsync>d__);
			return <UpdateFaceSwapPhotosAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600031A RID: 794 RVA: 0x00010A1F File Offset: 0x0000EC1F
		private void ImportButton_Click(object sender, RoutedEventArgs e)
		{
			this.FileImportFromPC();
		}

		// Token: 0x0600031B RID: 795 RVA: 0x00010A27 File Offset: 0x0000EC27
		private void SkipVideoButton_Click(object sender, MouseButtonEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayer.Volume = 0.0;
			this.ManageMirrorBoothVideo();
		}

		// Token: 0x0600031C RID: 796 RVA: 0x00010A50 File Offset: 0x0000EC50
		private void RecordButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.glamBotMove == DSLR.GlamBotMove.None)
			{
				return;
			}
			switch (this.glamBotMove)
			{
			case DSLR.GlamBotMove.Move1:
				KukaUdpMessenger.SendMessage("1");
				break;
			case DSLR.GlamBotMove.Move2:
				KukaUdpMessenger.SendMessage("2");
				break;
			case DSLR.GlamBotMove.Move3:
				KukaUdpMessenger.SendMessage("3");
				break;
			case DSLR.GlamBotMove.Move4:
				KukaUdpMessenger.SendMessage("4");
				break;
			}
			if (ActivationKingWindow.DSLRVideoPage != null)
			{
				ActivationKingWindow.DSLRVideoPage = null;
			}
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				ActivationKingWindow.SetPage(ApplicationPage.GoProShootPage, false);
				return;
			}
			ActivationKingWindow.SetPage(ApplicationPage.VideoShootPage, false);
		}

		// Token: 0x0600031D RID: 797 RVA: 0x00010ADC File Offset: 0x0000ECDC
		private void SelectButton1_Click(object sender, RoutedEventArgs e)
		{
			this.ResetAllBordersOpacity();
			Border border = ExtensionMethod.FindChildByName<Border>(this.SelectButton1, "Selected");
			if (border != null)
			{
				border.Opacity = 1.0;
			}
			this.glamBotMove = DSLR.GlamBotMove.Move1;
		}

		// Token: 0x0600031E RID: 798 RVA: 0x00010B1C File Offset: 0x0000ED1C
		private void SelectButton2_Click(object sender, RoutedEventArgs e)
		{
			this.ResetAllBordersOpacity();
			Border border = ExtensionMethod.FindChildByName<Border>(this.SelectButton2, "Selected");
			if (border != null)
			{
				border.Opacity = 1.0;
			}
			this.glamBotMove = DSLR.GlamBotMove.Move2;
		}

		// Token: 0x0600031F RID: 799 RVA: 0x00010B5C File Offset: 0x0000ED5C
		private void SelectButton3_Click(object sender, RoutedEventArgs e)
		{
			this.ResetAllBordersOpacity();
			Border border = ExtensionMethod.FindChildByName<Border>(this.SelectButton3, "Selected");
			if (border != null)
			{
				border.Opacity = 1.0;
			}
			this.glamBotMove = DSLR.GlamBotMove.Move3;
		}

		// Token: 0x06000320 RID: 800 RVA: 0x00010B9C File Offset: 0x0000ED9C
		private void SelectButton4_Click(object sender, RoutedEventArgs e)
		{
			this.ResetAllBordersOpacity();
			Border border = ExtensionMethod.FindChildByName<Border>(this.SelectButton4, "Selected");
			if (border != null)
			{
				border.Opacity = 1.0;
			}
			this.glamBotMove = DSLR.GlamBotMove.Move4;
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00010BDC File Offset: 0x0000EDDC
		private void ResetAllBordersOpacity()
		{
			Border border = ExtensionMethod.FindChildByName<Border>(this.SelectButton1, "Selected");
			if (border != null)
			{
				border.Opacity = 0.0;
			}
			Border border2 = ExtensionMethod.FindChildByName<Border>(this.SelectButton2, "Selected");
			if (border2 != null)
			{
				border2.Opacity = 0.0;
			}
			Border border3 = ExtensionMethod.FindChildByName<Border>(this.SelectButton3, "Selected");
			if (border3 != null)
			{
				border3.Opacity = 0.0;
			}
			Border border4 = ExtensionMethod.FindChildByName<Border>(this.SelectButton4, "Selected");
			if (border4 != null)
			{
				border4.Opacity = 0.0;
			}
			this.glamBotMove = DSLR.GlamBotMove.None;
		}

		// Token: 0x06000322 RID: 802 RVA: 0x00010C7C File Offset: 0x0000EE7C
		private void GlambotGalleryButton_Click(object sender, RoutedEventArgs e)
		{
			this.GalleryButton_Click(null, null);
		}

		// Token: 0x06000323 RID: 803 RVA: 0x00010C86 File Offset: 0x0000EE86
		private void GlambotMovementPageButton_Click(object sender, RoutedEventArgs e)
		{
			this.GlambotSelectionMainGrid.Visibility = Visibility.Visible;
			this.GlambotHomePageMainGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000324 RID: 804 RVA: 0x00010CA0 File Offset: 0x0000EEA0
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.GlambotSelectionMainGrid.Visibility = Visibility.Collapsed;
			this.GlambotHomePageMainGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x06000325 RID: 805 RVA: 0x00010CBA File Offset: 0x0000EEBA
		private void GlambotStartIdle_Click(object sender, RoutedEventArgs e)
		{
			KukaUdpMessenger.SendMessage("5");
		}

		// Token: 0x06000326 RID: 806 RVA: 0x00010CC6 File Offset: 0x0000EEC6
		private void GlambotMovementStopIdle_Click(object sender, RoutedEventArgs e)
		{
			KukaUdpMessenger.SendMessage("0");
		}

		// Token: 0x06000327 RID: 807 RVA: 0x00010CD2 File Offset: 0x0000EED2
		private void PromptBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			DSLR.NameAndSurname = this.PromptBox.Text;
		}

		// Token: 0x06000328 RID: 808 RVA: 0x00010CE4 File Offset: 0x0000EEE4
		private void PromptBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.PromptBox.Text.Length == 0)
			{
				this.PromptBoxPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000329 RID: 809 RVA: 0x00010D04 File Offset: 0x0000EF04
		private void PromptBoxPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.PromptBoxPlaceholder.Visibility = Visibility.Collapsed;
			this.PromptBox.Focus();
		}

		// Token: 0x0600032A RID: 810 RVA: 0x00010D1E File Offset: 0x0000EF1E
		private void Page_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return && this.gridPhoto.Visibility == Visibility.Visible && MainSettingsPage.SelectedMode != Mode.Glambot && !this.isPayment)
			{
				this.TakePhoto();
			}
		}

		// Token: 0x0600032B RID: 811 RVA: 0x00010D4C File Offset: 0x0000EF4C
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			this.PaymentCancellationToken.Cancel();
		}

		// Token: 0x0600032C RID: 812 RVA: 0x00010D59 File Offset: 0x0000EF59
		private void PrintNumberAdd_Click(object sender, RoutedEventArgs e)
		{
			this.buyingPrintNumber = Math.Min(DSLR.maxBuyingPrintLimit, Math.Max(1, this.buyingPrintNumber + 1));
			this.PrintNumberBox.Text = this.buyingPrintNumber.ToString();
		}

		// Token: 0x0600032D RID: 813 RVA: 0x00010D90 File Offset: 0x0000EF90
		private void PrintNumberRemove_Click(object sender, RoutedEventArgs e)
		{
			int numberMin = (this.buyingDigitalDownloadNumber <= 0) ? 1 : 0;
			this.buyingPrintNumber = Math.Min(DSLR.maxBuyingPrintLimit, Math.Max(numberMin, this.buyingPrintNumber - 1));
			this.PrintNumberBox.Text = this.buyingPrintNumber.ToString();
		}

		// Token: 0x0600032E RID: 814 RVA: 0x00010DE0 File Offset: 0x0000EFE0
		private void PrintNumberBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int numberMin = (this.buyingDigitalDownloadNumber <= 0) ? 1 : 0;
				int newNumber = Convert.ToInt32(this.PrintNumberBox.Text);
				int limitNumber = Math.Min(DSLR.maxBuyingPrintLimit, Math.Max(numberMin, newNumber));
				if (newNumber != limitNumber)
				{
					this.PrintNumberBox.Text = limitNumber.ToString();
				}
				this.buyingPrintNumber = limitNumber;
			}
			catch (Exception)
			{
				this.PrintNumberBox.Text = this.buyingPrintNumber.ToString();
			}
			this.UpdatePaymentButtonText();
		}

		// Token: 0x0600032F RID: 815 RVA: 0x00010E6C File Offset: 0x0000F06C
		private void DownloadNumberAdd_Click(object sender, RoutedEventArgs e)
		{
			this.buyingDigitalDownloadNumber = Math.Min(DSLR.maxBuyingDigitalDownloadLimit, Math.Max(1, this.buyingDigitalDownloadNumber + 1));
			this.DownloadNumberBox.Text = this.buyingDigitalDownloadNumber.ToString();
		}

		// Token: 0x06000330 RID: 816 RVA: 0x00010EA4 File Offset: 0x0000F0A4
		private void DownloadNumberRemove_Click(object sender, RoutedEventArgs e)
		{
			int numberMin = (this.buyingPrintNumber <= 0) ? 1 : 0;
			this.buyingDigitalDownloadNumber = Math.Min(DSLR.maxBuyingDigitalDownloadLimit, Math.Max(numberMin, this.buyingDigitalDownloadNumber - 1));
			this.DownloadNumberBox.Text = this.buyingDigitalDownloadNumber.ToString();
		}

		// Token: 0x06000331 RID: 817 RVA: 0x00010EF4 File Offset: 0x0000F0F4
		private void DownloadNumberBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int numberMin = (this.buyingPrintNumber <= 0) ? 1 : 0;
				int newNumber = Convert.ToInt32(this.DownloadNumberBox.Text);
				int limitNumber = Math.Min(DSLR.maxBuyingDigitalDownloadLimit, Math.Max(numberMin, newNumber));
				if (newNumber != limitNumber)
				{
					this.DownloadNumberBox.Text = limitNumber.ToString();
				}
				this.buyingDigitalDownloadNumber = limitNumber;
			}
			catch (Exception)
			{
				this.DownloadNumberBox.Text = this.buyingDigitalDownloadNumber.ToString();
			}
			this.UpdatePaymentButtonText();
		}

		// Token: 0x06000332 RID: 818 RVA: 0x00010F80 File Offset: 0x0000F180
		private void OpenPaymentPage()
		{
			this.DownloadNumberBox.Text = "1";
			this.PrintNumberBox.Text = "1";
			this.PrintPaymentAmountText.Text = string.Format("${0:#.00}", DSLR.PrintPrice);
			this.DownloadPaymentAmountText.Text = string.Format("${0:#.00}", DSLR.downloadPrice);
			this.btnApply.Content = Settings.GetValueString("lang_paymentmainpaybutton") + " $2,00";
			this.buyingPrintNumber = 0;
			this.buyingDigitalDownloadNumber = 0;
			switch (StripeController.paymentOption)
			{
			case PaymentOption.Printdownload:
				this.PaymentPrintGrid.Visibility = Visibility.Visible;
				this.PaymentDownloadGrid.Visibility = Visibility.Visible;
				this.buyingPrintNumber = 1;
				this.buyingDigitalDownloadNumber = 1;
				break;
			case PaymentOption.Printonly:
				this.PaymentPrintGrid.Visibility = Visibility.Visible;
				this.PaymentDownloadGrid.Visibility = Visibility.Collapsed;
				this.buyingPrintNumber = 1;
				break;
			case PaymentOption.Downloadonly:
				this.PaymentPrintGrid.Visibility = Visibility.Collapsed;
				this.PaymentDownloadGrid.Visibility = Visibility.Visible;
				this.buyingDigitalDownloadNumber = 1;
				break;
			}
			this.loadingPaymentGrid.Visibility = Visibility.Collapsed;
			this.SuccessPaymentGrid.Visibility = Visibility.Collapsed;
			this.MainPaymentPage.Visibility = Visibility.Visible;
			this.PaymentGrid.Visibility = Visibility.Visible;
			this.UpdatePaymentButtonText();
		}

		// Token: 0x06000333 RID: 819 RVA: 0x000110D0 File Offset: 0x0000F2D0
		private void UpdatePaymentButtonText()
		{
			float payTotal = (float)this.buyingDigitalDownloadNumber * DSLR.downloadPrice + (float)this.buyingPrintNumber * DSLR.PrintPrice;
			this.TotalAmount = payTotal;
			if (this.btnApply != null)
			{
				this.btnApply.Content = string.Format("{1} ${0:0.00}", payTotal, Settings.GetValueString("lang_paymentmainpaybutton", true));
			}
		}

		// Token: 0x06000334 RID: 820 RVA: 0x00011130 File Offset: 0x0000F330
		private void btnApply_Click(object sender, RoutedEventArgs e)
		{
			DSLR.<btnApply_Click>d__103 <btnApply_Click>d__;
			<btnApply_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<btnApply_Click>d__.<>4__this = this;
			<btnApply_Click>d__.<>1__state = -1;
			<btnApply_Click>d__.<>t__builder.Start<DSLR.<btnApply_Click>d__103>(ref <btnApply_Click>d__);
		}

		// Token: 0x06000335 RID: 821 RVA: 0x00011167 File Offset: 0x0000F367
		private void PayingPhotoButton_Click(object sender, RoutedEventArgs e)
		{
			this.PaymentGrid.Visibility = Visibility.Collapsed;
			DSLR.selectedPrint = true;
			DSLR.PaidPaymentSession = new PaymentSession(-1, 0);
			this.TakePhoto();
		}

		// Token: 0x06000336 RID: 822 RVA: 0x00011190 File Offset: 0x0000F390
		private void LoadingPaymentGridLoad()
		{
			DSLR.<>c__DisplayClass105_0 CS$<>8__locals1 = new DSLR.<>c__DisplayClass105_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.Time = StripeController.paymentLimitTime * 60;
			this.PaymentTimeText.Text = CS$<>8__locals1.Time.ToString();
			Task.Run(delegate()
			{
				DSLR.<>c__DisplayClass105_0.<<LoadingPaymentGridLoad>b__0>d <<LoadingPaymentGridLoad>b__0>d;
				<<LoadingPaymentGridLoad>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<LoadingPaymentGridLoad>b__0>d.<>4__this = CS$<>8__locals1;
				<<LoadingPaymentGridLoad>b__0>d.<>1__state = -1;
				<<LoadingPaymentGridLoad>b__0>d.<>t__builder.Start<DSLR.<>c__DisplayClass105_0.<<LoadingPaymentGridLoad>b__0>d>(ref <<LoadingPaymentGridLoad>b__0>d);
				return <<LoadingPaymentGridLoad>b__0>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000337 RID: 823 RVA: 0x000111E0 File Offset: 0x0000F3E0
		private void MainPaymentBackButton_Click(object sender, RoutedEventArgs e)
		{
			this.PaymentGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000338 RID: 824 RVA: 0x000111EE File Offset: 0x0000F3EE
		private void PayingDigitalPhotoButton_Click(object sender, RoutedEventArgs e)
		{
			this.PaymentGrid.Visibility = Visibility.Collapsed;
			DSLR.selectedPrint = false;
			DSLR.PaidPaymentSession = new PaymentSession(0, -1);
			this.TakePhoto();
		}

		// Token: 0x06000339 RID: 825 RVA: 0x00011214 File Offset: 0x0000F414
		private void RemoveCreditButton_Click(object sender, RoutedEventArgs e)
		{
			MessageBoxWindow.CreateWindow("Cancel Session", "This action will delete your credits. Do you want to continue?", new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Yes,
				MessageBoxWindow.ButtonType.No
			}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.RemoveCreditMessageBoxReturn), MessageBoxWindow.MessageBoxSize.Large, false);
		}

		// Token: 0x0600033A RID: 826 RVA: 0x00011248 File Offset: 0x0000F448
		public void RemoveCreditMessageBoxReturn(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			if (messageBoxReturn == MessageBoxWindow.MessageBoxReturn.Yes)
			{
				DSLR.currentPaymentSession.downloadCredit = (DSLR.currentPaymentSession.printCredit = 0);
				this.PaymentGrid.Visibility = Visibility.Collapsed;
			}
		}

		// Token: 0x0600033B RID: 827 RVA: 0x0001127D File Offset: 0x0000F47D
		private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x0600033C RID: 828 RVA: 0x00011288 File Offset: 0x0000F488
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			if (isPortraitScreen)
			{
				this.ConfirmationPageTitle.FontSize = 76.0;
				this.PaymentSessionInfoText.FontSize = 45.0;
				this.ConfirmationPagePrintText.FontSize = 45.0;
				this.digitalText.FontSize = 45.0;
				this.btnApply.FontSize = 42.0;
				this.MainPaymentTitle.FontSize = 76.0;
				this.MainPaymentText.FontSize = 35.0;
				this.PaymentMainPagePrintTitle.FontSize = 35.0;
				this.PaymentMainPagePrintText.FontSize = 18.0;
				this.MainPageDigitalTitle.FontSize = 35.0;
				this.PaymentMainPageDigitalText.FontSize = 18.0;
				this.PaymentLoadingPageText.FontSize = 47.0;
				this.PaymentLoadingPageTitle.FontSize = 86.0;
				this.PaymentLoadingPageScanQRText.FontSize = 42.0;
				this.PaymentTimeText.FontSize = 53.0;
				return;
			}
			this.ConfirmationPageTitle.FontSize = 61.0;
			this.PaymentSessionInfoText.FontSize = 30.0;
			this.ConfirmationPagePrintText.FontSize = 30.0;
			this.digitalText.FontSize = 30.0;
			this.btnApply.FontSize = 27.0;
			this.MainPaymentTitle.FontSize = 61.0;
			this.MainPaymentText.FontSize = 30.0;
			this.PaymentMainPagePrintTitle.FontSize = 30.0;
			this.PaymentMainPagePrintText.FontSize = 13.0;
			this.MainPageDigitalTitle.FontSize = 30.0;
			this.PaymentMainPageDigitalText.FontSize = 13.0;
			this.PaymentLoadingPageText.FontSize = 22.0;
			this.PaymentLoadingPageTitle.FontSize = 61.0;
			this.PaymentLoadingPageScanQRText.FontSize = 22.0;
			this.PaymentTimeText.FontSize = 38.0;
		}

		// Token: 0x040002B7 RID: 695
		public static EventMediaClass eventMediaClass;

		// Token: 0x040002B8 RID: 696
		public static string NameAndSurname = "";

		// Token: 0x040002B9 RID: 697
		public static int printlimit = -1;

		// Token: 0x040002BA RID: 698
		private bool startLoad = true;

		// Token: 0x040002BB RID: 699
		private bool firstFrame;

		// Token: 0x040002BC RID: 700
		public static bool isHaveGreenBox = false;

		// Token: 0x040002BD RID: 701
		public static bool isHaveAIBackground = false;

		// Token: 0x040002BE RID: 702
		private bool isShowPhoto;

		// Token: 0x040002BF RID: 703
		private bool isShowVideo;

		// Token: 0x040002C0 RID: 704
		private bool isShowImport;

		// Token: 0x040002C1 RID: 705
		private bool isShowGallery;

		// Token: 0x040002C2 RID: 706
		public static bool IsLoadCamera = false;

		// Token: 0x040002C3 RID: 707
		public static bool isImportFile = false;

		// Token: 0x040002C4 RID: 708
		private bool isOpenLPPD;

		// Token: 0x040002C5 RID: 709
		private bool isCameraPreview;

		// Token: 0x040002C6 RID: 710
		private List<string> CameraProperties = new List<string>();

		// Token: 0x040002C7 RID: 711
		public static List<string> importFilePaths = null;

		// Token: 0x040002C8 RID: 712
		private static string previousFaceSwapJSONContent = "";

		// Token: 0x040002C9 RID: 713
		public static bool isFileImport = false;

		// Token: 0x040002CA RID: 714
		public static string selectedFileExtension = ".jpg";

		// Token: 0x040002CB RID: 715
		private Thread MirrorBoothVideoThread;

		// Token: 0x040002CC RID: 716
		private bool mirrorBoothThreadActive;

		// Token: 0x040002CD RID: 717
		private int MirrorBoothVideoTimeCount = 150;

		// Token: 0x040002CE RID: 718
		private bool isPreviewCameraBackgroundActive;

		// Token: 0x040002CF RID: 719
		public static Action PhotoBoothStarted;

		// Token: 0x040002D0 RID: 720
		public bool isPayment;

		// Token: 0x040002D1 RID: 721
		private CancellationTokenSource PaymentCancellationToken = new CancellationTokenSource();

		// Token: 0x040002D2 RID: 722
		private int buyingPrintNumber = 1;

		// Token: 0x040002D3 RID: 723
		public static int maxBuyingPrintLimit = 99;

		// Token: 0x040002D4 RID: 724
		private int buyingDigitalDownloadNumber = 1;

		// Token: 0x040002D5 RID: 725
		public static int maxBuyingDigitalDownloadLimit = 99;

		// Token: 0x040002D6 RID: 726
		public static float downloadPrice = 0f;

		// Token: 0x040002D7 RID: 727
		public static float PrintPrice = 0f;

		// Token: 0x040002D8 RID: 728
		private float TotalAmount;

		// Token: 0x040002D9 RID: 729
		public static PaymentSession currentPaymentSession = null;

		// Token: 0x040002DA RID: 730
		public static PaymentSession PaidPaymentSession = null;

		// Token: 0x040002DB RID: 731
		private bool isWaitingPayment;

		// Token: 0x040002DC RID: 732
		public static bool selectedPrint = false;

		// Token: 0x040002DD RID: 733
		private DSLR.GlamBotMove glamBotMove;

		// Token: 0x040002DE RID: 734
		public static bool isPhoto;

		// Token: 0x02000162 RID: 354
		private enum GlamBotMove
		{
			// Token: 0x04000C81 RID: 3201
			None,
			// Token: 0x04000C82 RID: 3202
			Move1,
			// Token: 0x04000C83 RID: 3203
			Move2,
			// Token: 0x04000C84 RID: 3204
			Move3,
			// Token: 0x04000C85 RID: 3205
			Move4
		}
	}
}
