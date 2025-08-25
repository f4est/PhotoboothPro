using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Modals.Api;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Variations;
using TermControls;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000027 RID: 39
	public partial class AISharing : Page, IStyleConnector
	{
		// Token: 0x17000053 RID: 83
		// (get) Token: 0x060001B9 RID: 441 RVA: 0x000092A4 File Offset: 0x000074A4
		public VariationMedia selectedVariationMedia
		{
			get
			{
				if (this.variationsOfOriginalMedia == null || this.variationsOfOriginalMedia.Count == 0 || AISharing.currentMidjourneyMediaIndex < 0 || AISharing.currentMidjourneyMediaIndex >= this.variationsOfOriginalMedia.Count)
				{
					return null;
				}
				return (from x in VariationMediaHelper.VariationMediasList
				where x.ResultLocalPath == this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultLocalPath
				select x).FirstOrDefault<VariationMedia>();
			}
		}

		// Token: 0x17000054 RID: 84
		// (get) Token: 0x060001BA RID: 442 RVA: 0x00009300 File Offset: 0x00007500
		public static string AIImagesFolderPath
		{
			get
			{
				string path = System.IO.Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "AIImages");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x17000055 RID: 85
		// (get) Token: 0x060001BB RID: 443 RVA: 0x00009334 File Offset: 0x00007534
		public static string WordPortraitFolderPath
		{
			get
			{
				string path = System.IO.Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "WordPortrait");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				return path;
			}
		}

		// Token: 0x17000056 RID: 86
		// (get) Token: 0x060001BC RID: 444 RVA: 0x00009366 File Offset: 0x00007566
		public BitmapImage GalleryIconImage
		{
			get
			{
				return this.galleryIconImage;
			}
		}

		// Token: 0x17000057 RID: 87
		// (get) Token: 0x060001BD RID: 445 RVA: 0x0000936E File Offset: 0x0000756E
		// (set) Token: 0x060001BE RID: 446 RVA: 0x00009376 File Offset: 0x00007576
		public System.Windows.Controls.TextBox LastFocusedTextBox { get; private set; }

		// Token: 0x060001BF RID: 447 RVA: 0x00009380 File Offset: 0x00007580
		public AISharing()
		{
			this.InitializeComponent();
		}

		// Token: 0x060001C0 RID: 448 RVA: 0x00009428 File Offset: 0x00007628
		private void CheckSharingButtons()
		{
			this.PrintButton.IsEnabled = true;
			this.PrintButton.Visibility = ((Settings.GetValueBoolean("printenable") != null) ? ((Settings.GetValueBoolean("printenable").Value && !this.IsTotalPrintLimitFull()) ? Visibility.Visible : Visibility.Collapsed) : Visibility.Collapsed);
			if (this.IsPaymentActive && !DSLR.selectedPrint && StripeController.paymentScreen != PaymentScreen.AtSharingScreen)
			{
				this.PrintButton.Visibility = Visibility.Hidden;
			}
			if (this.PrintButton.Visibility == Visibility.Visible)
			{
				PrintClass.printMode = (Settings.GetValueBoolean("usewindowssettingsprint").Value ? PrintClass.PrintMode.WindowsSettings : PrintClass.PrintMode.NewPrintSettings);
				PrintClass.isForceLandscape = Settings.GetValueBoolean("forcelandscapeprint").Value;
				PrintClass.isForcePortrait = Settings.GetValueBoolean("forceportraitprint").Value;
				PrintClass.isUseDefaultPrintSettings = Settings.GetValueBoolean("usedefaultprintsettings").Value;
			}
			this.EmailsButton.Visibility = ((Settings.GetValueBoolean("mailenable") != null) ? (Settings.GetValueBoolean("mailenable").Value ? Visibility.Visible : Visibility.Collapsed) : Visibility.Collapsed);
			this.SMSSButton.Visibility = ((Settings.GetValueBoolean("smsenable") != null) ? (Settings.GetValueBoolean("smsenable").Value ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden);
			this.gridQR.Visibility = ((Settings.GetValueBoolean("qrenable") != null) ? (Settings.GetValueBoolean("qrenable").Value ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden);
			this.SaveAsButton.Visibility = ((Settings.GetValueBoolean("saveas") != null) ? (Settings.GetValueBoolean("saveas").Value ? Visibility.Visible : Visibility.Hidden) : Visibility.Hidden);
		}

		// Token: 0x060001C1 RID: 449 RVA: 0x000095FD File Offset: 0x000077FD
		private bool IsTotalPrintLimitFull()
		{
			return DSLR.printlimit != -1 && DSLR.printlimit != 0 && PrinterPage.printedNumber >= DSLR.printlimit;
		}

		// Token: 0x060001C2 RID: 450 RVA: 0x0000961F File Offset: 0x0000781F
		private bool CheckPrintButtonEnabled()
		{
			if (this.IsTotalPrintLimitFull() || (this.limitForPrintPhoto != 0 && this.limitForPrintPhoto <= this.photoPrintedNumber))
			{
				MessageBoxWindow.CreateWindow("Print limit exceeded", "You can check out the sharing page in settings", null, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return false;
			}
			return true;
		}

		// Token: 0x060001C3 RID: 451 RVA: 0x00009656 File Offset: 0x00007856
		private void SkipVideoButton_Click(object sender, MouseButtonEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayerAds.Volume = 0.0;
		}

		// Token: 0x060001C4 RID: 452 RVA: 0x00009678 File Offset: 0x00007878
		private void VideoPlayerAds_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.adsVideoGrid.Visibility = Visibility.Collapsed;
			this.videoPlayerAds.Volume = 0.0;
		}

		// Token: 0x060001C5 RID: 453 RVA: 0x0000969C File Offset: 0x0000789C
		private string PreventSingleWordOnNewLine(string text)
		{
			string[] words = text.Split(new char[]
			{
				' '
			});
			if (words.Length > 1)
			{
				string[] array = words;
				int num = words.Length - 2;
				array[num] = array[num] + "\u00a0" + words[words.Length - 1];
				return string.Join("", words, 0, words.Length - 1);
			}
			return text;
		}

		// Token: 0x060001C6 RID: 454 RVA: 0x000096F4 File Offset: 0x000078F4
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AISharing.<Page_Loaded>d__52 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<AISharing.<Page_Loaded>d__52>(ref <Page_Loaded>d__);
		}

		// Token: 0x060001C7 RID: 455 RVA: 0x0000972C File Offset: 0x0000792C
		private void LoadPaymentsLanguageStrings()
		{
			List<string> keyList = new List<string>
			{
				"lang_paymentmainpaybutton",
				"lang_paymentmainbackbutton",
				"lang_paymentloadingtext",
				"lang_paymentloadingtitle",
				"lang_paymentloadingbackbutton",
				"lang_paymentloadingscanqrtext"
			};
			List<UIElement> textObjects = new List<UIElement>
			{
				this.btnApply,
				this.MainPaymentBackButton,
				this.PaymentLoadingPageText,
				this.PaymentLoadingPageTitle,
				this.DeleteButton,
				this.PaymentLoadingPageScanQRText
			};
			for (int i = 0; i < keyList.Count; i++)
			{
				this.LoadLanguageString2Object(keyList[i], textObjects[i]);
			}
		}

		// Token: 0x060001C8 RID: 456 RVA: 0x000097F4 File Offset: 0x000079F4
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
				System.Windows.Controls.Button btn = textBlock as System.Windows.Controls.Button;
				if (btn != null)
				{
					btn.Content = text;
				}
			}
		}

		// Token: 0x060001C9 RID: 457 RVA: 0x00009834 File Offset: 0x00007A34
		private void LoadLanguages()
		{
			this.GalleryButton.Content = Settings.GetValueString("lang_gallery", true);
			this.DoneButton.Content = Settings.GetValueString("lang_done", true);
			this.PrintButton.Content = Settings.GetValueString("lang_print", true);
			this.EmailsButton.Content = Settings.GetValueString("lang_email", true);
			this.SMSSButton.Content = Settings.GetValueString("lang_smsbutton", true);
			this.SaveAsButton.Content = Settings.GetValueString("lang_saveasbutton", true);
			this.AIImageButton.Content = Settings.GetValueString("lang_aiimgbutton", true);
			this.FaceSwapButton.Content = Settings.GetValueString("lang_faceswapbutton", true);
			this.WordCloudButton.Content = Settings.GetValueString("lang_wordcloudbutton", true);
			this.AiMotionButton.Content = Settings.GetValueString("lang_aimotionbutton", true);
			this.AiEffectButton.Content = Settings.GetValueString("lang_aieffectbutton", true);
			this.AiBeautifierButton.Content = Settings.GetValueString("lang_aibeautifierbutton", true);
			this.QRPreparingText.Text = Settings.GetValueString("lang_qrpreparing", true);
			this.ShotQRCodeText.Text = Settings.GetValueString("lang_qrshow", true);
		}

		// Token: 0x060001CA RID: 458 RVA: 0x00009978 File Offset: 0x00007B78
		private void SetModeVideo(string videoPath, string thumbnailPath, bool isChangeVideo)
		{
			this.imgResultPhoto.Visibility = Visibility.Collapsed;
			this.videoPlayerFullscreen.Visibility = Visibility.Visible;
			this.videoPlayer.Visibility = Visibility.Visible;
			if (isChangeVideo)
			{
				this.videoPlayer.Source = new Uri(videoPath, UriKind.Absolute);
				this.videoPlayerFullscreen.Source = new Uri(videoPath, UriKind.Absolute);
			}
			this.videoPlayerFullscreen.Position = (this.videoPlayer.Position = TimeSpan.Zero);
			this.videoPlayer.MediaEnded += this.VideoPlayer_MediaEnded;
			if (AISharing.SharingMediaclass != null)
			{
				TimedAction.ExecuteWithDelay(delegate
				{
					if (AISharing.SharingMediaclass != null && AISharing.SharingMediaclass.thumbnail != null && File.Exists(thumbnailPath))
					{
						AISharing.bitmapResultPhoto = new BitmapImage();
						AISharing.bitmapResultPhoto.BeginInit();
						AISharing.bitmapResultPhoto.UriSource = new Uri(thumbnailPath, UriKind.Absolute);
						AISharing.bitmapResultPhoto.EndInit();
						object dataItem = this.GalleryButton.DataContext;
						this.galleryIconImage = AISharing.bitmapResultPhoto;
					}
				}, TimeSpan.FromSeconds(2.0));
				return;
			}
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.galleryIconImage = new BitmapImage(new Uri("pack://application:,,,/AssemblyName;component/Resources/bProfile.png"));
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060001CB RID: 459 RVA: 0x00009A60 File Offset: 0x00007C60
		private void SetModePhoto(string filepath)
		{
			this.videoPlayer.Visibility = Visibility.Collapsed;
			this.imgResultPhoto.Visibility = Visibility.Visible;
			this.videoPlayerFullscreen.Visibility = Visibility.Hidden;
			AISharing.bitmapResultPhoto = new BitmapImage();
			AISharing.bitmapResultPhoto.BeginInit();
			AISharing.bitmapResultPhoto.UriSource = new Uri(filepath, UriKind.Absolute);
			AISharing.bitmapResultPhoto.EndInit();
			this.imgResultPhoto.Source = AISharing.bitmapResultPhoto;
			this.PhotoCapturedImageFullscreen.Source = AISharing.bitmapResultPhoto;
			this.galleryIconImage = AISharing.bitmapResultPhoto;
			AISharing.SetSecondScreenPhoto(this.imgResultPhoto);
		}

		// Token: 0x060001CC RID: 460 RVA: 0x00009AF8 File Offset: 0x00007CF8
		private void SetAiMotionEnable(VariationMedia variationMedia = null)
		{
			base.Dispatcher.Invoke(delegate()
			{
				if (this.selectedVariationMedia == null)
				{
					this.AiMotionButton.IsEnabled = false;
					return;
				}
				if (variationMedia == null)
				{
					this.AiMotionButton.IsEnabled = (this.selectedVariationMedia.VariationOperation == VariationType.AIFaceSwap || this.selectedVariationMedia.VariationOperation == VariationType.AIPrompt || this.selectedVariationMedia.VariationOperation == VariationType.AiEffect);
					return;
				}
				this.AiMotionButton.IsEnabled = (variationMedia.VariationOperation == VariationType.AIFaceSwap || variationMedia.VariationOperation == VariationType.AIPrompt || variationMedia.VariationOperation == VariationType.AiEffect);
			});
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00009B30 File Offset: 0x00007D30
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.videoPlayer.MediaEnded -= this.VideoPlayer_MediaEnded;
			this.QRCodeImage.Visibility = Visibility.Hidden;
			SecondWindow instance = SecondWindow.Instance;
			if (instance != null)
			{
				instance.ChangeQrStatus(false, null);
			}
			this.isFirstCountdownActive = false;
			if (this.changeLiveView)
			{
				LiveViewClass.IsLiveViewTimerEnabled = true;
			}
			this.imgResultPhoto.SourceUpdated -= this.ImgResultPhoto_SourceUpdated;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x00009B9E File Offset: 0x00007D9E
		private void ImgResultPhoto_SourceUpdated(object sender, DataTransferEventArgs e)
		{
			AISharing.SetSecondScreenPhoto(sender as System.Windows.Controls.Image);
		}

		// Token: 0x060001CF RID: 463 RVA: 0x00009BAC File Offset: 0x00007DAC
		private static void SetSecondScreenPhoto(System.Windows.Controls.Image image)
		{
			if ((AISharing.currentMidjourneyMediaIndex > 0 || ActivationKingWindow.AISharingPage.variationsOfOriginalMedia.Count > 0) && image != null)
			{
				BitmapImage source = image.Source as BitmapImage;
				SecondWindow instance = SecondWindow.Instance;
				if (instance == null)
				{
					return;
				}
				instance.UpdateImage(source);
			}
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00009BF4 File Offset: 0x00007DF4
		private Task ImageUpscale_StartedAsync()
		{
			AISharing.<ImageUpscale_StartedAsync>d__62 <ImageUpscale_StartedAsync>d__;
			<ImageUpscale_StartedAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ImageUpscale_StartedAsync>d__.<>4__this = this;
			<ImageUpscale_StartedAsync>d__.<>1__state = -1;
			<ImageUpscale_StartedAsync>d__.<>t__builder.Start<AISharing.<ImageUpscale_StartedAsync>d__62>(ref <ImageUpscale_StartedAsync>d__);
			return <ImageUpscale_StartedAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00009C38 File Offset: 0x00007E38
		private void SelectThisVariation(VariationMedia newVariationMedia)
		{
			for (int i = 0; i < this.variationsOfOriginalMedia.Count - 1; i++)
			{
				if (newVariationMedia.VariationMediaHash == this.variationsOfOriginalMedia[i].VariationMediaHash)
				{
					AISharing.currentMidjourneyMediaIndex = i;
					return;
				}
			}
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x00009C84 File Offset: 0x00007E84
		private void VariationMediaProcessCompleted(VariationMedia variationMedia)
		{
			AISharing.<>c__DisplayClass64_0 CS$<>8__locals1 = new AISharing.<>c__DisplayClass64_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.variationMedia = variationMedia;
			if (AISharing.SharingMediaclass != null)
			{
				if (CS$<>8__locals1.variationMedia.OriginalMediaHash == AISharing.SharingMediaclass.MediaHash)
				{
					if (!CS$<>8__locals1.variationMedia.MediaVariationSynced)
					{
						return;
					}
					this.LoadImageFiles(CS$<>8__locals1.variationMedia);
					this.ShowQRCode(CS$<>8__locals1.variationMedia.OriginalMediaHash);
					System.Windows.Application.Current.Dispatcher.Invoke(delegate()
					{
						AISharing.<>c__DisplayClass64_0.<<VariationMediaProcessCompleted>b__0>d <<VariationMediaProcessCompleted>b__0>d;
						<<VariationMediaProcessCompleted>b__0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
						<<VariationMediaProcessCompleted>b__0>d.<>4__this = CS$<>8__locals1;
						<<VariationMediaProcessCompleted>b__0>d.<>1__state = -1;
						<<VariationMediaProcessCompleted>b__0>d.<>t__builder.Start<AISharing.<>c__DisplayClass64_0.<<VariationMediaProcessCompleted>b__0>d>(ref <<VariationMediaProcessCompleted>b__0>d);
					});
				}
				ExtensionMethod.DeleteFilesWithSameDateInNameAsync(CS$<>8__locals1.variationMedia.ResultLocalPath, 10000, null, null, 3);
			}
			this.SelectThisVariation(CS$<>8__locals1.variationMedia);
			this.RefreshLoadingElements();
			this.SetAiMotionEnable(CS$<>8__locals1.variationMedia);
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x00009D45 File Offset: 0x00007F45
		private void MidJourneyHelper_StatusUpdate(string status, string mediaHash)
		{
			if (AISharing.SharingMediaclass != null)
			{
				AISharing.SharingMediaclass.MediaHash == mediaHash;
			}
		}

		// Token: 0x060001D4 RID: 468 RVA: 0x00009D5F File Offset: 0x00007F5F
		private void MidJourneyMedia_ImagineCanceled(string mediaHash)
		{
			if (AISharing.SharingMediaclass != null && AISharing.SharingMediaclass.MediaHash == mediaHash)
			{
				base.Dispatcher.Invoke(delegate()
				{
					this.isFirstCountdownActive = false;
					this.AIImageButton.LoadingAnimationVisibility = Visibility.Collapsed;
					this.AIImageButton.BlackCoverVisibility = Visibility.Collapsed;
					this.AIImageButton.IsEnabled = true;
				});
				this.RefreshLoadingElements();
			}
		}

		// Token: 0x060001D5 RID: 469 RVA: 0x00009D97 File Offset: 0x00007F97
		private void MidJourneyHelper_MJDataGenerateError(string error)
		{
			this.RefreshLoadingElements();
		}

		// Token: 0x060001D6 RID: 470 RVA: 0x00009D9F File Offset: 0x00007F9F
		private void ControlAIImages()
		{
			this.variationsOfOriginalMedia.Clear();
			this.LoadImageFiles(null);
			this.AIImagesButtonGrid.Visibility = ((this.variationsOfOriginalMedia.Count == 0) ? Visibility.Collapsed : Visibility.Visible);
		}

		// Token: 0x060001D7 RID: 471 RVA: 0x00009DCF File Offset: 0x00007FCF
		private string GetUrlForMedia(string mediaHash, string addFolder = "")
		{
			return string.Format("https://activationshare.com/m/{0}{1}", mediaHash, addFolder);
		}

		// Token: 0x060001D8 RID: 472 RVA: 0x00009DE0 File Offset: 0x00007FE0
		private void ShowQRCode(string mediaHash)
		{
			bool showVariation = this.isAIImagesSelected;
			base.Dispatcher.Invoke(delegate()
			{
				BitmapImage bitmapImage = this.QRCodeImage.Source as BitmapImage;
				if (bitmapImage != null)
				{
					Stream streamSource = bitmapImage.StreamSource;
					if (streamSource != null)
					{
						streamSource.Dispose();
					}
					bitmapImage.StreamSource = null;
				}
				this.QRCodeImage.Source = null;
				GC.Collect();
				GC.WaitForPendingFinalizers();
				this.QRCodeImage.Source = ExtensionMethod.GenerateQr(this.GetUrlForMedia(mediaHash, showVariation ? string.Format("?variationId={0}", AISharing.currentMidjourneyMediaIndex) : ""));
				this.QRCodeImage.Visibility = Visibility.Visible;
				SecondWindow instance = SecondWindow.Instance;
				if (instance != null)
				{
					instance.ChangeQrStatus(true, (BitmapImage)this.QRCodeImage.Source);
				}
				this.gifLoadingQR.Visibility = Visibility.Hidden;
			});
		}

		// Token: 0x060001D9 RID: 473 RVA: 0x00009E24 File Offset: 0x00008024
		private void BunnyCDNHelper_MediaLinkGenerated(MediaClassBase media)
		{
			if (AISharing.SharingMediaclass != null && media.index == AISharing.SharingMediaclass.index)
			{
				System.Windows.Application.Current.Dispatcher.Invoke(delegate()
				{
					this.GenerateAIImageButton.IsEnabled = true;
				});
				if (media.dataType == DataType.gif && !media.isUploadedGifFile)
				{
					return;
				}
				this.ShowQRCode(media.MediaHash);
			}
		}

		// Token: 0x060001DA RID: 474 RVA: 0x00009E83 File Offset: 0x00008083
		private void VideoPlayer_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.videoPlayer.Position = TimeSpan.Zero;
		}

		// Token: 0x060001DB RID: 475 RVA: 0x00009E98 File Offset: 0x00008098
		private void MailClass_MailCompletedEvent(bool arg1, string arg2)
		{
			MessageBoxWindow.CreateWindow("Mail Status : " + arg1.ToString(), " Mail Completed : " + arg2.ToString(), new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Continue
			}, MessageBoxWindow.MessageIcon.Success, null, MessageBoxWindow.MessageBoxSize.Large, false);
			base.Dispatcher.BeginInvoke(new Action(delegate()
			{
			}), Array.Empty<object>());
		}

		// Token: 0x060001DC RID: 476 RVA: 0x00009F0B File Offset: 0x0000810B
		private void GalleryButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.SetPage(ApplicationPage.GalleryPage, false);
		}

		// Token: 0x060001DD RID: 477 RVA: 0x00009F14 File Offset: 0x00008114
		private void DoneAction()
		{
			AISharing.SharingMediaclass = null;
			ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
		}

		// Token: 0x060001DE RID: 478 RVA: 0x00009F23 File Offset: 0x00008123
		private void DoneButton_Click(object sender, RoutedEventArgs e)
		{
			this.DoneAction();
		}

		// Token: 0x060001DF RID: 479 RVA: 0x00009F2C File Offset: 0x0000812C
		private void PrintButton_Click(object sender, RoutedEventArgs e)
		{
			this.MailGrid.Visibility = Visibility.Hidden;
			this.SMSGrid.Visibility = Visibility.Hidden;
			this.AIImageGrid.Visibility = Visibility.Hidden;
			if (this.isDirectPrint || this.IsPaymentActive)
			{
				this.printNumber = 1;
				this.PrintNow();
				return;
			}
			this.PrintGrid.Visibility = ((this.PrintGrid.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden);
			this.PrintNumberBox.Text = "";
		}

		// Token: 0x060001E0 RID: 480 RVA: 0x00009FA8 File Offset: 0x000081A8
		private void EmailButton_Click(object sender, RoutedEventArgs e)
		{
			this.PrintGrid.Visibility = Visibility.Hidden;
			this.SMSGrid.Visibility = Visibility.Hidden;
			this.AIImageGrid.Visibility = Visibility.Hidden;
			this.MailGrid.Visibility = ((this.MailGrid.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden);
			this.MailBox.Text = "";
			this.MailBox.Focus();
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x060001E1 RID: 481 RVA: 0x0000A020 File Offset: 0x00008220
		private void SMSButton_Click(object sender, RoutedEventArgs e)
		{
			this.PrintGrid.Visibility = Visibility.Hidden;
			this.MailGrid.Visibility = Visibility.Hidden;
			this.AIImageGrid.Visibility = Visibility.Hidden;
			this.SMSGrid.Visibility = ((this.SMSGrid.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden);
			this.SmsBox.Text = "";
		}

		// Token: 0x060001E2 RID: 482 RVA: 0x0000A07E File Offset: 0x0000827E
		private void MailBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.MailBox.Text.Length == 0)
			{
				this.MailBoxPlaceholder.Visibility = Visibility.Visible;
				return;
			}
			this.MailBoxPlaceholder.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060001E3 RID: 483 RVA: 0x0000A0AB File Offset: 0x000082AB
		private void MailBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.MailBox.Text.Length == 0)
			{
				this.MailBoxPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060001E4 RID: 484 RVA: 0x0000A0CB File Offset: 0x000082CB
		private void MailBoxPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.MailBoxPlaceholder.Visibility = Visibility.Collapsed;
			this.MailBox.Focus();
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x060001E5 RID: 485 RVA: 0x0000A0F1 File Offset: 0x000082F1
		private void KeyboardKeyDown(string key)
		{
			if (this.LastFocusedTextBox != null)
			{
				if (this.LastFocusedTextBox.Text == null)
				{
					this.LastFocusedTextBox.Text = "";
				}
				System.Windows.Controls.TextBox lastFocusedTextBox = this.LastFocusedTextBox;
				lastFocusedTextBox.Text += key;
			}
		}

		// Token: 0x060001E6 RID: 486 RVA: 0x0000A130 File Offset: 0x00008330
		private void KeyboardViewModel_DeletePressed()
		{
			if (this.LastFocusedTextBox != null && !string.IsNullOrEmpty(this.LastFocusedTextBox.Text))
			{
				this.LastFocusedTextBox.Text = this.LastFocusedTextBox.Text.Remove(this.LastFocusedTextBox.Text.Length - 1);
			}
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x0000A184 File Offset: 0x00008384
		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			this.LastFocusedTextBox = (sender as System.Windows.Controls.TextBox);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0000A194 File Offset: 0x00008394
		private void MailSendButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<MailSendButton_Click>d__86 <MailSendButton_Click>d__;
			<MailSendButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<MailSendButton_Click>d__.<>4__this = this;
			<MailSendButton_Click>d__.<>1__state = -1;
			<MailSendButton_Click>d__.<>t__builder.Start<AISharing.<MailSendButton_Click>d__86>(ref <MailSendButton_Click>d__);
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x0000A1CB File Offset: 0x000083CB
		private void pageSharing_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x17000058 RID: 88
		// (get) Token: 0x060001EA RID: 490 RVA: 0x0000A1D3 File Offset: 0x000083D3
		// (set) Token: 0x060001EB RID: 491 RVA: 0x0000A1DB File Offset: 0x000083DB
		public int GridColumnCount { get; set; } = 4;

		// Token: 0x060001EC RID: 492 RVA: 0x0000A1E4 File Offset: 0x000083E4
		private void ManagePortraitScreen()
		{
			bool isPortraitScreen = base.ActualWidth < base.ActualHeight;
			if (isPortraitScreen)
			{
				this.gridMain.RowDefinitions[0].Height = new GridLength(150.0, GridUnitType.Star);
				this.gridMain.RowDefinitions[this.gridMain.RowDefinitions.Count - 1].Height = new GridLength(150.0, GridUnitType.Star);
				Grid.SetRow(this.PhotoPanel, 0);
				Grid.SetRowSpan(this.PhotoPanel, 3);
				this.PhotoPanel.SetValue(Grid.RowProperty, 0);
				this.PhotoPanel.SetValue(Grid.RowSpanProperty, 3);
				this.UniformLoadingPanel.SetValue(Grid.ColumnProperty, 0);
				this.UniformLoadingPanel.SetValue(Grid.ColumnSpanProperty, 3);
				this.GridColumnCount = 6;
				return;
			}
			this.gridMain.RowDefinitions[0].Height = new GridLength(24.0, GridUnitType.Star);
			this.gridMain.RowDefinitions[this.gridMain.RowDefinitions.Count - 1].Height = new GridLength(14.0, GridUnitType.Star);
			Grid.SetRow(this.PhotoPanel, 1);
			Grid.SetRowSpan(this.PhotoPanel, 1);
			this.UniformLoadingPanel.SetValue(Grid.ColumnProperty, 1);
			this.UniformLoadingPanel.SetValue(Grid.ColumnSpanProperty, 1);
			this.GridColumnCount = 4;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x0000A37F File Offset: 0x0000857F
		private void PrintNowButton_Click(object sender, RoutedEventArgs e)
		{
			this.PrintNow();
		}

		// Token: 0x060001EE RID: 494 RVA: 0x0000A388 File Offset: 0x00008588
		public void PrintNow()
		{
			AISharing.<PrintNow>d__94 <PrintNow>d__;
			<PrintNow>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<PrintNow>d__.<>4__this = this;
			<PrintNow>d__.<>1__state = -1;
			<PrintNow>d__.<>t__builder.Start<AISharing.<PrintNow>d__94>(ref <PrintNow>d__);
		}

		// Token: 0x17000059 RID: 89
		// (get) Token: 0x060001EF RID: 495 RVA: 0x0000A3BF File Offset: 0x000085BF
		// (set) Token: 0x060001F0 RID: 496 RVA: 0x0000A3C7 File Offset: 0x000085C7
		public ObservableCollection<Frame> LoadingItems
		{
			get
			{
				return this.loadingItems;
			}
			set
			{
				this.loadingItems = value;
			}
		}

		// Token: 0x1700005A RID: 90
		// (get) Token: 0x060001F1 RID: 497 RVA: 0x0000A3D0 File Offset: 0x000085D0
		// (set) Token: 0x060001F2 RID: 498 RVA: 0x0000A3D8 File Offset: 0x000085D8
		public float TotalAmount { get; private set; }

		// Token: 0x060001F3 RID: 499 RVA: 0x0000A3E4 File Offset: 0x000085E4
		private void CreateLoadingItem(VariationMedia variation)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.MaxHeight = this.gridMain.ActualWidth * 0.2;
				VariationLoadingPrefab loadingPrefab = new VariationLoadingPrefab(variation, newFrame);
				this.LoadingItems.Add(newFrame);
				newFrame.Content = loadingPrefab;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x0000A424 File Offset: 0x00008624
		private void PrintNumberAdd_Click(object sender, RoutedEventArgs e)
		{
			this.printNumber = Math.Min(5, Math.Max(1, this.printNumber + 1));
			this.PrintNumberBox.Text = this.printNumber.ToString();
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x0000A456 File Offset: 0x00008656
		private void PrintNumberRemove_Click(object sender, RoutedEventArgs e)
		{
			this.printNumber = Math.Min(5, Math.Max(1, this.printNumber - 1));
			this.PrintNumberBox.Text = this.printNumber.ToString();
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x0000A488 File Offset: 0x00008688
		private void PrintNumberBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int newNumber = Convert.ToInt32(this.PrintNumberBox.Text);
				int limitNumber = Math.Min(5, Math.Max(1, newNumber));
				if (newNumber != limitNumber)
				{
					this.PrintNumberBox.Text = limitNumber.ToString();
				}
				this.printNumber = limitNumber;
			}
			catch (Exception)
			{
				this.PrintNumberBox.Text = this.printNumber.ToString();
			}
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x0000A4FC File Offset: 0x000086FC
		private void CloseKeyboardButton_Click(object sender, RoutedEventArgs e)
		{
			this.gridKeyboard.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x0000A50C File Offset: 0x0000870C
		private void PhotosIcon_Loaded(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Image imgIcon2 = sender as System.Windows.Controls.Image;
		}

		// Token: 0x060001F9 RID: 505 RVA: 0x0000A520 File Offset: 0x00008720
		private void ButtonIcon_Loaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060001FA RID: 506 RVA: 0x0000A522 File Offset: 0x00008722
		private void Button_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x060001FB RID: 507 RVA: 0x0000A524 File Offset: 0x00008724
		private void CloseFullscreenButton_Click(object sender, RoutedEventArgs e)
		{
			this.gridFullScreenPreview.Visibility = Visibility.Hidden;
		}

		// Token: 0x060001FC RID: 508 RVA: 0x0000A532 File Offset: 0x00008732
		private void btnFullscreen_Click(object sender, RoutedEventArgs e)
		{
			this.gridFullScreenPreview.Visibility = Visibility.Visible;
		}

		// Token: 0x060001FD RID: 509 RVA: 0x0000A540 File Offset: 0x00008740
		private void SmsBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.SmsBoxPlaceholder.Visibility = ((this.SmsBox.Text.Length == 0) ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x060001FE RID: 510 RVA: 0x0000A563 File Offset: 0x00008763
		private void SmsBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.SmsBox.Text.Length == 0)
			{
				this.SmsBoxPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x060001FF RID: 511 RVA: 0x0000A583 File Offset: 0x00008783
		private void SmsBoxPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.SmsBoxPlaceholder.Visibility = Visibility.Collapsed;
			this.SmsBox.Focus();
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x06000200 RID: 512 RVA: 0x0000A5AC File Offset: 0x000087AC
		private void SmsSendButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<SmsSendButton_Click>d__116 <SmsSendButton_Click>d__;
			<SmsSendButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SmsSendButton_Click>d__.<>4__this = this;
			<SmsSendButton_Click>d__.<>1__state = -1;
			<SmsSendButton_Click>d__.<>t__builder.Start<AISharing.<SmsSendButton_Click>d__116>(ref <SmsSendButton_Click>d__);
		}

		// Token: 0x06000201 RID: 513 RVA: 0x0000A5E3 File Offset: 0x000087E3
		private void ChoosePromptButton_Click(object sender, RoutedEventArgs e)
		{
			this.SamplePromptsGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x0000A5F4 File Offset: 0x000087F4
		private void LoadSamplePromptFiles()
		{
			string eventSamplePromptPath = SamplePromptsPage.samplePromptsJsonPath;
			if (SamplePromptsPage.SamplePromptData != null)
			{
				this.samplePrompts = SamplePromptsPage.SamplePromptData.SamplePrompts;
			}
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000A620 File Offset: 0x00008820
		private Task LoadAIEffectsInfo()
		{
			AISharing.<LoadAIEffectsInfo>d__119 <LoadAIEffectsInfo>d__;
			<LoadAIEffectsInfo>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadAIEffectsInfo>d__.<>4__this = this;
			<LoadAIEffectsInfo>d__.<>1__state = -1;
			<LoadAIEffectsInfo>d__.<>t__builder.Start<AISharing.<LoadAIEffectsInfo>d__119>(ref <LoadAIEffectsInfo>d__);
			return <LoadAIEffectsInfo>d__.<>t__builder.Task;
		}

		// Token: 0x06000204 RID: 516 RVA: 0x0000A664 File Offset: 0x00008864
		private void CheckAndAddSamplePromptPrefabThreadFunc()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.samplePromptItems.Clear();
				this.samplePromptPrefabs.Clear();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			for (int i = 0; i < this.samplePrompts.Count; i++)
			{
				this.CreateSamplePromptItem(this.samplePrompts[i]);
			}
		}

		// Token: 0x06000205 RID: 517 RVA: 0x0000A6C0 File Offset: 0x000088C0
		private void CheckAndAddAIEffectPrefabThreadFunc()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.AIEffectsItems.Clear();
				this.AIEffectGalleryPrefabs.Clear();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			for (int i = 0; i < this.aiEffectLocals.Count; i++)
			{
				if (this.aiEffectLocals[i].IsEnabled)
				{
					this.CreateAIEffectItem(this.aiEffectLocals[i]);
				}
			}
		}

		// Token: 0x06000206 RID: 518 RVA: 0x0000A730 File Offset: 0x00008930
		private void CreateSamplePromptItem(SamplePrompt samplePrompt)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				SamplePromptPrefab samplePromptPrefab = new SamplePromptPrefab();
				samplePromptPrefab.ClickEvent += this.SamplePromptPrefab_ClickEvent;
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.Height = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)samplePrompt.FileDetails.Height) / (double)((float)samplePrompt.FileDetails.Width);
				this.samplePromptItems.Add(newFrame);
				samplePromptPrefab.LoadImage(System.IO.Path.Combine(new string[]
				{
					samplePrompt.FileDetails.FullPath
				}), samplePrompt);
				samplePromptPrefab.SamplePromptTitle.Text = samplePrompt.Title;
				newFrame.Content = samplePromptPrefab;
				this.samplePromptPrefabs.Add(samplePromptPrefab);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000207 RID: 519 RVA: 0x0000A770 File Offset: 0x00008970
		private void CreateAIEffectItem(AiEffectLocal AIEffect)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				AIEffectGalleryPrefab AIEffectPrefab = new AIEffectGalleryPrefab();
				AIEffectPrefab.ClickEvent += this.AIEffectPrefab_ClickEvent;
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.Height = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)AIEffect.FileDetails.Height) / (double)((float)AIEffect.FileDetails.Width);
				this.AIEffectsItems.Add(newFrame);
				AIEffectPrefab.LoadImage(AIEffect);
				AIEffectPrefab.AIEffectTitle.Text = AIEffect.Title;
				newFrame.Content = AIEffectPrefab;
				this.AIEffectGalleryPrefabs.Add(AIEffectPrefab);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000208 RID: 520 RVA: 0x0000A7B0 File Offset: 0x000089B0
		private void AIEffectPrefab_ClickEvent(string id)
		{
			AISharing.<>c__DisplayClass124_0 CS$<>8__locals1 = new AISharing.<>c__DisplayClass124_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.id = id;
			string ID = CS$<>8__locals1.id;
			this.AIImagesButtonGrid.Visibility = Visibility.Collapsed;
			this.OriginalImageButtonGrid.Visibility = Visibility.Visible;
			Bitmap capturedPhotoSize = (Bitmap)System.Drawing.Image.FromFile(AISharing.SharingMediaclass.capturedFiles[0].FullName);
			CS$<>8__locals1.mailAddress = (Settings.GetValueBoolean("realtime").Value ? this.MailPromptBox.Text : "");
			string url = BunnyCDNHelper.GetBunnyLink(AISharing.SharingMediaclass.resultFile.FullName, SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
			string rawUrl = BunnyCDNHelper.GetBunnyLink(AISharing.SharingMediaclass.capturedFiles[0].FullName, SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
			new Thread(delegate()
			{
				if (AISharing.SharingMediaclass == null)
				{
					return;
				}
				while (string.IsNullOrEmpty(AISharing.SharingMediaclass.MediaHash))
				{
					Thread.Sleep(1000);
					if (AISharing.SharingMediaclass == null)
					{
						return;
					}
				}
				CS$<>8__locals1.mailAddress = CS$<>8__locals1.<>4__this.AIEffectmailAddress;
				VariationMediaHelper.CreateAiEffectJob(AISharing.SharingMediaclass, CS$<>8__locals1.id, CS$<>8__locals1.mailAddress);
				CS$<>8__locals1.<>4__this.RefreshLoadingElements();
				CS$<>8__locals1.<>4__this.AIEffectmailAddress = "";
			}).Start();
			this.AIEffectsGalleryGrid.Visibility = (this.SamplePromptsGrid.Visibility = Visibility.Hidden);
			this.AIEffectXMLErrorGrid.Visibility = (this.XMLErrorGrid.Visibility = Visibility.Collapsed);
			this.AIEffectmailAddress = (Settings.GetValueBoolean("realtime").Value ? this.AIEffectmailAddress : "");
			this.AiEffectButton.LoadingAnimationVisibility = Visibility.Visible;
			this.AiEffectButton.BlackCoverVisibility = Visibility.Visible;
			CS$<>8__locals1.remainingSeconds = Settings.GetValueString("lang_remainingseconds");
			Task.Run(delegate()
			{
				AISharing.<>c__DisplayClass124_0.<<AIEffectPrefab_ClickEvent>b__1>d <<AIEffectPrefab_ClickEvent>b__1>d;
				<<AIEffectPrefab_ClickEvent>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<AIEffectPrefab_ClickEvent>b__1>d.<>4__this = CS$<>8__locals1;
				<<AIEffectPrefab_ClickEvent>b__1>d.<>1__state = -1;
				<<AIEffectPrefab_ClickEvent>b__1>d.<>t__builder.Start<AISharing.<>c__DisplayClass124_0.<<AIEffectPrefab_ClickEvent>b__1>d>(ref <<AIEffectPrefab_ClickEvent>b__1>d);
				return <<AIEffectPrefab_ClickEvent>b__1>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000209 RID: 521 RVA: 0x0000A948 File Offset: 0x00008B48
		private void SamplePromptPrefab_ClickEvent(SamplePrompt obj)
		{
			this.SamplePromptsGrid.Visibility = Visibility.Hidden;
			this.AIImageGrid.Visibility = Visibility.Visible;
			this.PromptBoxPlaceholder.Visibility = Visibility.Collapsed;
			Random random = new Random();
			int index = random.Next(obj.Prompts.Count);
			string selectedPrompt = obj.Prompts[index];
			if (this.PromptInputField.Visibility == Visibility.Visible)
			{
				this.PromptBox.Text = selectedPrompt;
				return;
			}
			this.PromptBox.Text = selectedPrompt;
			this.SelectedPromptGrid.Visibility = Visibility.Visible;
			this.PromptTitle.Visibility = Visibility.Visible;
			this.SelectedPromptImg.Visibility = Visibility.Visible;
			this.PromptTitle.Text = obj.Title;
			BitmapImage bitmapSelectedPrompt = new BitmapImage();
			bitmapSelectedPrompt.BeginInit();
			bitmapSelectedPrompt.UriSource = new Uri(obj.FileDetails.FullPath, UriKind.Absolute);
			bitmapSelectedPrompt.EndInit();
			this.SelectedPromptImg.Source = bitmapSelectedPrompt;
		}

		// Token: 0x0600020A RID: 522 RVA: 0x0000AA2E File Offset: 0x00008C2E
		private void videoPlayerFullscreen_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.videoPlayerFullscreen.Position = TimeSpan.Zero;
		}

		// Token: 0x0600020B RID: 523 RVA: 0x0000AA40 File Offset: 0x00008C40
		private void PromptBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.PromptBox.Text.Length == 0)
			{
				this.PromptBoxPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x0600020C RID: 524 RVA: 0x0000AA60 File Offset: 0x00008C60
		private void PromptBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.PromptBox.Text.IndexOf("--ar") != -1 || this.PromptBox.Text.IndexOf("--v") != -1)
			{
				this.txtErrorBox.Text = "Don't enter the --ar and --v parameters; they will be automatically deleted.";
				return;
			}
			if (this.txtErrorBox.Text.Length > 1)
			{
				this.txtErrorBox.Text = "";
			}
		}

		// Token: 0x0600020D RID: 525 RVA: 0x0000AAD4 File Offset: 0x00008CD4
		private string FindAspectRatio(int width, int height)
		{
			string rotation = Settings.GetValueString("rotation") ?? "0";
			if (rotation.Contains("90") || rotation.Contains("270"))
			{
				return string.Format("{0}:{1}", height, width);
			}
			return string.Format("{0}:{1}", width, height);
		}

		// Token: 0x0600020E RID: 526 RVA: 0x0000AB3C File Offset: 0x00008D3C
		private Task CountdownFunctionAsync(string remainingSeconds, int elapsedSeconds = 40)
		{
			AISharing.<CountdownFunctionAsync>d__131 <CountdownFunctionAsync>d__;
			<CountdownFunctionAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<CountdownFunctionAsync>d__.<>4__this = this;
			<CountdownFunctionAsync>d__.remainingSeconds = remainingSeconds;
			<CountdownFunctionAsync>d__.elapsedSeconds = elapsedSeconds;
			<CountdownFunctionAsync>d__.<>1__state = -1;
			<CountdownFunctionAsync>d__.<>t__builder.Start<AISharing.<CountdownFunctionAsync>d__131>(ref <CountdownFunctionAsync>d__);
			return <CountdownFunctionAsync>d__.<>t__builder.Task;
		}

		// Token: 0x0600020F RID: 527 RVA: 0x0000AB8F File Offset: 0x00008D8F
		private void PromptBoxPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.PromptBoxPlaceholder.Visibility = Visibility.Collapsed;
			this.PromptBox.Focus();
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x06000210 RID: 528 RVA: 0x0000ABB8 File Offset: 0x00008DB8
		private void DownloadAIVariantImagesFromJson(string jsonDataPath, string destinationFolder, string mediaHash)
		{
			List<VariationMedia> mediaDatas = ExtensionMethod.ReadJson<List<VariationMedia>>(jsonDataPath);
			foreach (VariationMedia mediaData in mediaDatas)
			{
				if (mediaData.OriginalMediaHash == mediaHash)
				{
					string url = mediaData.MidJourneyResultSeperatedPhotosUrls[0];
					string fileName = System.IO.Path.GetFileName(new Uri(url).AbsolutePath);
					string fullPath = System.IO.Path.Combine(destinationFolder, fileName);
					using (WebClient client = new WebClient())
					{
						client.DownloadFile(url, fullPath);
					}
				}
			}
		}

		// Token: 0x06000211 RID: 529 RVA: 0x0000AC68 File Offset: 0x00008E68
		private void LoadImageFiles(VariationMedia variationMedia = null)
		{
			AISharing.<>c__DisplayClass134_0 CS$<>8__locals1 = new AISharing.<>c__DisplayClass134_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.variationMedia = variationMedia;
			if (VariationMediaHelper.VariationMediasList == null)
			{
				return;
			}
			List<VariationMedia> midJourneyMediasTemp = (from x in VariationMediaHelper.VariationMediasList
			where x.OriginalMediaHash == AISharing.SharingMediaclass.MediaHash && !string.IsNullOrEmpty(AISharing.SharingMediaclass.MediaHash) && x.MediaVariationSynced
			orderby x.GeneratedTime descending
			select x).ToList<VariationMedia>();
			if (midJourneyMediasTemp == null)
			{
				MessageBoxWindow.CreateWindow("Media Henüz gelmedi", "Bir süre daha lütfen bekleyin", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			this.isAIImagesSelected = (midJourneyMediasTemp.Count > 0);
			List<VariationMedia> itemsToRemove = new List<VariationMedia>();
			foreach (VariationMedia mjMedia in midJourneyMediasTemp)
			{
				if (!File.Exists(mjMedia.ResultLocalPath) && mjMedia.midJourneyProcess != MediaVariationModals.MidJourneyProcess.WaitingForVariationSelection)
				{
					itemsToRemove.Add(mjMedia);
				}
			}
			foreach (VariationMedia item in itemsToRemove)
			{
				midJourneyMediasTemp.Remove(item);
			}
			this.variationsOfOriginalMedia = midJourneyMediasTemp;
			base.Dispatcher.Invoke<Task>(delegate()
			{
				AISharing.<>c__DisplayClass134_0.<<LoadImageFiles>b__2>d <<LoadImageFiles>b__2>d;
				<<LoadImageFiles>b__2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<LoadImageFiles>b__2>d.<>4__this = CS$<>8__locals1;
				<<LoadImageFiles>b__2>d.<>1__state = -1;
				<<LoadImageFiles>b__2>d.<>t__builder.Start<AISharing.<>c__DisplayClass134_0.<<LoadImageFiles>b__2>d>(ref <<LoadImageFiles>b__2>d);
				return <<LoadImageFiles>b__2>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000212 RID: 530 RVA: 0x0000ADD8 File Offset: 0x00008FD8
		private void RefreshLoadingElements()
		{
			if (AISharing.refreshLoadingWorking)
			{
				return;
			}
			AISharing.refreshLoadingWorking = true;
			try
			{
				Task.Run(delegate()
				{
					AISharing.<<RefreshLoadingElements>b__139_0>d <<RefreshLoadingElements>b__139_0>d;
					<<RefreshLoadingElements>b__139_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<RefreshLoadingElements>b__139_0>d.<>4__this = this;
					<<RefreshLoadingElements>b__139_0>d.<>1__state = -1;
					<<RefreshLoadingElements>b__139_0>d.<>t__builder.Start<AISharing.<<RefreshLoadingElements>b__139_0>d>(ref <<RefreshLoadingElements>b__139_0>d);
					return <<RefreshLoadingElements>b__139_0>d.<>t__builder.Task;
				});
			}
			catch (Exception ex)
			{
			}
			AISharing.refreshLoadingWorking = false;
		}

		// Token: 0x06000213 RID: 531 RVA: 0x0000AE20 File Offset: 0x00009020
		private void LeftButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.currentMidjourneyMediaIndex--;
			if (AISharing.currentMidjourneyMediaIndex < 0)
			{
				AISharing.currentMidjourneyMediaIndex = this.variationsOfOriginalMedia.Count - 1;
			}
			this.ChangeVariation(false);
		}

		// Token: 0x06000214 RID: 532 RVA: 0x0000AE4F File Offset: 0x0000904F
		private void RightButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.currentMidjourneyMediaIndex++;
			if (AISharing.currentMidjourneyMediaIndex >= this.variationsOfOriginalMedia.Count)
			{
				AISharing.currentMidjourneyMediaIndex = 0;
			}
			this.ChangeVariation(false);
		}

		// Token: 0x06000215 RID: 533 RVA: 0x0000AE7C File Offset: 0x0000907C
		private void ChangeVariation(bool isForced = false)
		{
			if (this.selectedVariationMedia.ResultFileInfo.GetMediaType() == MediaType.photo)
			{
				this.imgResultPhoto.Source = new BitmapImage(new Uri(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultLocalPath, UriKind.Absolute));
				this.PhotoCapturedImageFullscreen.Source = this.imgResultPhoto.Source;
				this.SetModePhoto(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultLocalPath);
			}
			else
			{
				this.SetModeVideo(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultLocalPath, this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ThumbnailLocalPath, isForced);
			}
			this.ShowQRCode(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].OriginalMediaHash);
			this.SetAiMotionEnable(null);
		}

		// Token: 0x06000216 RID: 534 RVA: 0x0000AF4D File Offset: 0x0000914D
		private void OriginalImageButton_Click(object sender, RoutedEventArgs e)
		{
			this.OriginalImageButtonFunc();
		}

		// Token: 0x06000217 RID: 535 RVA: 0x0000AF58 File Offset: 0x00009158
		private void OriginalImageButtonFunc()
		{
			try
			{
				if (AISharing.SharingMediaclass.mediaType == MediaType.photo)
				{
					this.SetModePhoto(AISharing.SharingMediaclass.resultFile.FullName);
				}
				else
				{
					this.SetModeVideo(AISharing.SharingMediaclass.resultFile.FullName, AISharing.SharingMediaclass.thumbnail.FullName, true);
				}
				this.LeftButton.Visibility = Visibility.Hidden;
				this.RightButton.Visibility = Visibility.Hidden;
				if (this.AISelectIdGrid.Visibility == Visibility.Visible)
				{
					this.AISelectIdGrid.Visibility = Visibility.Collapsed;
				}
				this.isAIImagesSelected = false;
				this.OriginalImageButton.Opacity = 1.0;
				this.AIImagesButton.Opacity = 0.75;
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000218 RID: 536 RVA: 0x0000B024 File Offset: 0x00009224
		private void AIImagesButton_Click(object sender, RoutedEventArgs e)
		{
			this.LoadImageFiles(null);
			this.OriginalImageButton.Opacity = 0.75;
			this.AIImagesButton.Opacity = 1.0;
		}

		// Token: 0x06000219 RID: 537 RVA: 0x0000B055 File Offset: 0x00009255
		private void ChooseSingleCheck_Checked(object sender, RoutedEventArgs e)
		{
			if (this.ChooseMultiCheck != null)
			{
				this.ChooseMultiCheck.IsChecked = new bool?(false);
			}
		}

		// Token: 0x0600021A RID: 538 RVA: 0x0000B070 File Offset: 0x00009270
		private void ChooseSingleCheck_UnChecked(object sender, RoutedEventArgs e)
		{
			this.ChooseMultiCheck.IsChecked = new bool?(true);
		}

		// Token: 0x0600021B RID: 539 RVA: 0x0000B083 File Offset: 0x00009283
		private void ChooseMultiCheck_Checked(object sender, RoutedEventArgs e)
		{
			this.ChooseSingleCheck.IsChecked = new bool?(false);
		}

		// Token: 0x0600021C RID: 540 RVA: 0x0000B096 File Offset: 0x00009296
		private void ChooseMultiCheck_UnChecked(object sender, RoutedEventArgs e)
		{
			this.ChooseSingleCheck.IsChecked = new bool?(true);
		}

		// Token: 0x0600021D RID: 541 RVA: 0x0000B0AC File Offset: 0x000092AC
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.AIEffectsGalleryGrid.Visibility = (this.SamplePromptsGrid.Visibility = Visibility.Hidden);
			this.AIEffectXMLErrorGrid.Visibility = (this.XMLErrorGrid.Visibility = Visibility.Collapsed);
		}

		// Token: 0x0600021E RID: 542 RVA: 0x0000B0ED File Offset: 0x000092ED
		private void CloseFullscreenButton_Click(object sender, TouchEventArgs e)
		{
			this.CloseFullscreen();
		}

		// Token: 0x0600021F RID: 543 RVA: 0x0000B0F5 File Offset: 0x000092F5
		private void CloseFullscreen()
		{
			this.gridFullScreenPreview.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000220 RID: 544 RVA: 0x0000B103 File Offset: 0x00009303
		private void CloseFullscreenButton_Click(object sender, MouseButtonEventArgs e)
		{
			this.CloseFullscreen();
		}

		// Token: 0x06000221 RID: 545 RVA: 0x0000B10B File Offset: 0x0000930B
		private void Select1Button_Click(object sender, RoutedEventArgs e)
		{
			this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].selectedResultIndex = 0;
			this.AISelectIdGrid.Visibility = Visibility.Hidden;
			this.ImageUpscale_StartedAsync();
		}

		// Token: 0x06000222 RID: 546 RVA: 0x0000B136 File Offset: 0x00009336
		private void Select2Button_Click(object sender, RoutedEventArgs e)
		{
			this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].selectedResultIndex = 1;
			this.AISelectIdGrid.Visibility = Visibility.Hidden;
			this.ImageUpscale_StartedAsync();
		}

		// Token: 0x06000223 RID: 547 RVA: 0x0000B161 File Offset: 0x00009361
		private void Select3Button_Click(object sender, RoutedEventArgs e)
		{
			this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].selectedResultIndex = 2;
			this.AISelectIdGrid.Visibility = Visibility.Hidden;
			this.ImageUpscale_StartedAsync();
		}

		// Token: 0x06000224 RID: 548 RVA: 0x0000B18C File Offset: 0x0000938C
		private void Select4Button_Click(object sender, RoutedEventArgs e)
		{
			this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].selectedResultIndex = 3;
			this.AISelectIdGrid.Visibility = Visibility.Hidden;
			this.ImageUpscale_StartedAsync();
		}

		// Token: 0x06000225 RID: 549 RVA: 0x0000B1B7 File Offset: 0x000093B7
		private void ChangeVideoPhotoButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.SharingMediaclass.mediaType = ((AISharing.SharingMediaclass.mediaType == MediaType.photo) ? MediaType.video : MediaType.photo);
			this.Page_Loaded(null, null);
		}

		// Token: 0x06000226 RID: 550 RVA: 0x0000B1DC File Offset: 0x000093DC
		private void MailPromptBoxPlaceholder_Click(object sender, RoutedEventArgs e)
		{
			this.MailPromptBoxPlaceholder.Visibility = Visibility.Collapsed;
			this.MailPromptBox.Focus();
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x06000227 RID: 551 RVA: 0x0000B202 File Offset: 0x00009402
		private void MailPromptBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.MailPromptBox.Text.Length == 0)
			{
				this.MailPromptBoxPlaceholder.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x06000228 RID: 552 RVA: 0x0000B224 File Offset: 0x00009424
		private void SaveAsButton_Click(object sender, RoutedEventArgs e)
		{
			if (this.isAIImagesSelected)
			{
				string saveFilePath = this.SelectFile(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultMediaType, System.IO.Path.GetFileName(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultLocalPath));
				if (!string.IsNullOrEmpty(saveFilePath))
				{
					File.Copy(this.variationsOfOriginalMedia[AISharing.currentMidjourneyMediaIndex].ResultLocalPath, saveFilePath, true);
					return;
				}
			}
			else
			{
				string saveFilePath = this.SelectFile(AISharing.SharingMediaclass.mediaType, System.IO.Path.GetFileName(AISharing.SharingMediaclass.resultFile.FullName));
				if (!string.IsNullOrEmpty(saveFilePath))
				{
					File.Copy(AISharing.SharingMediaclass.resultFile.FullName, saveFilePath, true);
				}
			}
		}

		// Token: 0x06000229 RID: 553 RVA: 0x0000B2D8 File Offset: 0x000094D8
		private string SelectFile(MediaType SelectID, string defaultFileName = null)
		{
			SaveFileDialog openFileDialog = new SaveFileDialog();
			if (SelectID != MediaType.video)
			{
				if (SelectID == MediaType.photo)
				{
					openFileDialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";
				}
			}
			else
			{
				openFileDialog.Filter = "Video Files (*.mp4)|*.mp4";
			}
			if (!string.IsNullOrEmpty(defaultFileName))
			{
				openFileDialog.FileName = defaultFileName;
			}
			DialogResult dialog = openFileDialog.ShowDialog();
			if (dialog == DialogResult.OK)
			{
				return openFileDialog.FileName;
			}
			return string.Empty;
		}

		// Token: 0x0600022A RID: 554 RVA: 0x0000B330 File Offset: 0x00009530
		private void AISharingXAML_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			AISharing.<AISharingXAML_KeyDown>d__163 <AISharingXAML_KeyDown>d__;
			<AISharingXAML_KeyDown>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AISharingXAML_KeyDown>d__.<>4__this = this;
			<AISharingXAML_KeyDown>d__.e = e;
			<AISharingXAML_KeyDown>d__.<>1__state = -1;
			<AISharingXAML_KeyDown>d__.<>t__builder.Start<AISharing.<AISharingXAML_KeyDown>d__163>(ref <AISharingXAML_KeyDown>d__);
		}

		// Token: 0x0600022B RID: 555 RVA: 0x0000B370 File Offset: 0x00009570
		private void AiMotionButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<AiMotionButton_Click>d__164 <AiMotionButton_Click>d__;
			<AiMotionButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AiMotionButton_Click>d__.<>4__this = this;
			<AiMotionButton_Click>d__.<>1__state = -1;
			<AiMotionButton_Click>d__.<>t__builder.Start<AISharing.<AiMotionButton_Click>d__164>(ref <AiMotionButton_Click>d__);
		}

		// Token: 0x0600022C RID: 556 RVA: 0x0000B3A8 File Offset: 0x000095A8
		private void GenerateAIImageButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<GenerateAIImageButton_Click>d__165 <GenerateAIImageButton_Click>d__;
			<GenerateAIImageButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GenerateAIImageButton_Click>d__.<>4__this = this;
			<GenerateAIImageButton_Click>d__.<>1__state = -1;
			<GenerateAIImageButton_Click>d__.<>t__builder.Start<AISharing.<GenerateAIImageButton_Click>d__165>(ref <GenerateAIImageButton_Click>d__);
		}

		// Token: 0x0600022D RID: 557 RVA: 0x0000B3E0 File Offset: 0x000095E0
		private Task GenerateAIImage()
		{
			AISharing.<GenerateAIImage>d__166 <GenerateAIImage>d__;
			<GenerateAIImage>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<GenerateAIImage>d__.<>4__this = this;
			<GenerateAIImage>d__.<>1__state = -1;
			<GenerateAIImage>d__.<>t__builder.Start<AISharing.<GenerateAIImage>d__166>(ref <GenerateAIImage>d__);
			return <GenerateAIImage>d__.<>t__builder.Task;
		}

		// Token: 0x0600022E RID: 558 RVA: 0x0000B424 File Offset: 0x00009624
		private void AIImageButton_Click(object sender, RoutedEventArgs e)
		{
			this.PrintGrid.Visibility = Visibility.Hidden;
			this.MailGrid.Visibility = Visibility.Hidden;
			this.SMSGrid.Visibility = Visibility.Hidden;
			this.PromptBoxPlaceholder.Visibility = Visibility.Visible;
			this.AIImageGrid.Visibility = ((this.AIImageGrid.Visibility == Visibility.Hidden) ? Visibility.Visible : Visibility.Hidden);
			if (this.AIImageGrid.Visibility == Visibility.Visible)
			{
				this.ChooseSingleCheck.IsChecked = new bool?(true);
			}
			if (this.gifLoadingQR.Visibility == Visibility.Hidden)
			{
				this.GenerateAIImageButton.IsEnabled = true;
			}
			else
			{
				this.GenerateAIImageButton.IsEnabled = false;
			}
			this.ChooseSingleCheck.IsChecked = new bool?(true);
			this.PromptBox.Text = "";
		}

		// Token: 0x0600022F RID: 559 RVA: 0x0000B4E8 File Offset: 0x000096E8
		private void FaceSwapButton_Click(object sender, RoutedEventArgs e)
		{
			this.faceSwapmailAddress = (Settings.GetValueBoolean("realtime").Value ? this.MailPromptBox.Text : "");
			if (Settings.GetValueBoolean("realtime").Value && !ExtensionMethod.IsValidEmail(this.faceSwapmailAddress))
			{
				MessageBoxWindow.CreateWindow("Invalid Email", "Please enter a valid email address", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			ActivationKingWindow.SetPage(ApplicationPage.FaceSwapBackgroundGalleryPage, false);
		}

		// Token: 0x06000230 RID: 560 RVA: 0x0000B56C File Offset: 0x0000976C
		private void WordPortraitButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<>c__DisplayClass169_0 CS$<>8__locals1 = new AISharing.<>c__DisplayClass169_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.mailAddress = (Settings.GetValueBoolean("realtime").Value ? this.MailPromptBox.Text : "");
			if (Settings.GetValueBoolean("realtime").Value && !ExtensionMethod.IsValidEmail(CS$<>8__locals1.mailAddress))
			{
				MessageBoxWindow.CreateWindow("Invalid Email", "Please enter a valid email address", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			this.WordCloudButton.IsEnabled = false;
			CS$<>8__locals1.url = BunnyCDNHelper.GetBunnyLink(AISharing.SharingMediaclass.resultFile.FullName, SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
			CS$<>8__locals1.rawUrl = BunnyCDNHelper.GetBunnyLink(AISharing.SharingMediaclass.capturedFiles[0].FullName, SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
			new Thread(delegate()
			{
				if (AISharing.SharingMediaclass == null)
				{
					return;
				}
				while (string.IsNullOrEmpty(AISharing.SharingMediaclass.MediaHash))
				{
					Thread.Sleep(1000);
					if (AISharing.SharingMediaclass == null)
					{
						return;
					}
				}
				VariationMediaHelper.CreateWordPortrait(AISharing.SharingMediaclass, CS$<>8__locals1.rawUrl, CS$<>8__locals1.url, ExtensionMethod.ReadWordCloud(), Settings.GetValueString("wordcloudfont"), ExtensionMethod.RemoveAlphaChannelFromString(Settings.GetValueString("wordcloudforegroundcolor")), ExtensionMethod.RemoveAlphaChannelFromString(Settings.GetValueString("wordcloudbackcolor")), AISharing.WordPortraitFolderPath, CS$<>8__locals1.mailAddress);
				CS$<>8__locals1.<>4__this.RefreshLoadingElements();
				Thread.Sleep(5000);
				Dispatcher dispatcher = System.Windows.Application.Current.Dispatcher;
				Action callback;
				if ((callback = CS$<>8__locals1.<>9__2) == null)
				{
					callback = (CS$<>8__locals1.<>9__2 = delegate()
					{
						CS$<>8__locals1.<>4__this.WordCloudButton.IsEnabled = true;
					});
				}
				dispatcher.Invoke(callback);
			}).Start();
			FaceSwapBackgroundGallery.selectedTarget = false;
			this.FaceSwapButton.LoadingAnimationVisibility = Visibility.Visible;
			this.FaceSwapButton.BlackCoverVisibility = Visibility.Visible;
			CS$<>8__locals1.remainingSeconds = Settings.GetValueString("lang_remainingseconds");
			Task.Run(delegate()
			{
				AISharing.<>c__DisplayClass169_0.<<WordPortraitButton_Click>b__1>d <<WordPortraitButton_Click>b__1>d;
				<<WordPortraitButton_Click>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<WordPortraitButton_Click>b__1>d.<>4__this = CS$<>8__locals1;
				<<WordPortraitButton_Click>b__1>d.<>1__state = -1;
				<<WordPortraitButton_Click>b__1>d.<>t__builder.Start<AISharing.<>c__DisplayClass169_0.<<WordPortraitButton_Click>b__1>d>(ref <<WordPortraitButton_Click>b__1>d);
				return <<WordPortraitButton_Click>b__1>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000231 RID: 561 RVA: 0x0000B6BD File Offset: 0x000098BD
		private void PromptBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x06000232 RID: 562 RVA: 0x0000B6CB File Offset: 0x000098CB
		private void PromptBox_TouchDown(object sender, TouchEventArgs e)
		{
			this.gridKeyboard.Visibility = Visibility.Visible;
		}

		// Token: 0x06000233 RID: 563 RVA: 0x0000B6DC File Offset: 0x000098DC
		private void AISharingXAML_LostFocus(object sender, RoutedEventArgs e)
		{
			IInputElement newFocusedElement = Keyboard.FocusedElement;
			System.Windows.Controls.TextBox newTextBox = newFocusedElement as System.Windows.Controls.TextBox;
			if (newTextBox != null && newFocusedElement != this.PromptBox && newFocusedElement != this.MailPromptBox)
			{
				this.LastFocusedTextBox = null;
			}
		}

		// Token: 0x06000234 RID: 564 RVA: 0x0000B712 File Offset: 0x00009912
		internal bool IsThereWorkingVariation()
		{
			return this.LoadingItems.Count > 0;
		}

		// Token: 0x06000235 RID: 565 RVA: 0x0000B724 File Offset: 0x00009924
		private void AiEffectButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<AiEffectButton_Click>d__174 <AiEffectButton_Click>d__;
			<AiEffectButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<AiEffectButton_Click>d__.<>4__this = this;
			<AiEffectButton_Click>d__.<>1__state = -1;
			<AiEffectButton_Click>d__.<>t__builder.Start<AISharing.<AiEffectButton_Click>d__174>(ref <AiEffectButton_Click>d__);
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0000B75B File Offset: 0x0000995B
		private void AIEffectChoosePromptButton_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000237 RID: 567 RVA: 0x0000B760 File Offset: 0x00009960
		private void AiBeautifierButton_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<>c__DisplayClass176_0 CS$<>8__locals1 = new AISharing.<>c__DisplayClass176_0();
			CS$<>8__locals1.<>4__this = this;
			this.AIImagesButtonGrid.Visibility = Visibility.Collapsed;
			this.OriginalImageButtonGrid.Visibility = Visibility.Visible;
			CS$<>8__locals1.mailAddress = (Settings.GetValueBoolean("realtime").Value ? this.MailPromptBox.Text : "");
			string url = BunnyCDNHelper.GetBunnyLink(AISharing.SharingMediaclass.resultFile.FullName, SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
			string rawUrl = BunnyCDNHelper.GetBunnyLink(AISharing.SharingMediaclass.capturedFiles[0].FullName, SessionData.accountInfo.userHash, EventManagementPage.GetCurrentEvent().EventHash, "");
			new Thread(delegate()
			{
				if (AISharing.SharingMediaclass == null)
				{
					return;
				}
				while (string.IsNullOrEmpty(AISharing.SharingMediaclass.MediaHash))
				{
					Thread.Sleep(1000);
				}
				CS$<>8__locals1.mailAddress = CS$<>8__locals1.<>4__this.faceSwapmailAddress;
				VariationMediaHelper.CreateAIBeautifierJob(AISharing.SharingMediaclass, AISharing.AIImagesFolderPath, CS$<>8__locals1.mailAddress);
				CS$<>8__locals1.<>4__this.RefreshLoadingElements();
				CS$<>8__locals1.<>4__this.faceSwapmailAddress = "";
			}).Start();
			this.faceSwapmailAddress = (Settings.GetValueBoolean("realtime").Value ? this.faceSwapmailAddress : "");
			this.AiBeautifierButton.LoadingAnimationVisibility = Visibility.Visible;
			this.AiBeautifierButton.BlackCoverVisibility = Visibility.Visible;
			CS$<>8__locals1.remainingSeconds = Settings.GetValueString("lang_remainingseconds");
			Task.Run(delegate()
			{
				AISharing.<>c__DisplayClass176_0.<<AiBeautifierButton_Click>b__1>d <<AiBeautifierButton_Click>b__1>d;
				<<AiBeautifierButton_Click>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<AiBeautifierButton_Click>b__1>d.<>4__this = CS$<>8__locals1;
				<<AiBeautifierButton_Click>b__1>d.<>1__state = -1;
				<<AiBeautifierButton_Click>b__1>d.<>t__builder.Start<AISharing.<>c__DisplayClass176_0.<<AiBeautifierButton_Click>b__1>d>(ref <<AiBeautifierButton_Click>b__1>d);
				return <<AiBeautifierButton_Click>b__1>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000238 RID: 568 RVA: 0x0000B894 File Offset: 0x00009A94
		private void gridQR_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			AISharing.<gridQR_MouseLeftButtonUp>d__177 <gridQR_MouseLeftButtonUp>d__;
			<gridQR_MouseLeftButtonUp>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<gridQR_MouseLeftButtonUp>d__.<>4__this = this;
			<gridQR_MouseLeftButtonUp>d__.<>1__state = -1;
			<gridQR_MouseLeftButtonUp>d__.<>t__builder.Start<AISharing.<gridQR_MouseLeftButtonUp>d__177>(ref <gridQR_MouseLeftButtonUp>d__);
		}

		// Token: 0x06000239 RID: 569 RVA: 0x0000B8CB File Offset: 0x00009ACB
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			this.PaymentCancellationToken.Cancel();
		}

		// Token: 0x0600023A RID: 570 RVA: 0x0000B8D8 File Offset: 0x00009AD8
		private void PaymentPrintNumberRemove_Click(object sender, RoutedEventArgs e)
		{
			this.buyingPrintNumber = Math.Min(DSLR.maxBuyingPrintLimit, Math.Max(1, this.buyingPrintNumber - 1));
			this.PrintNumberBox.Text = this.buyingPrintNumber.ToString();
		}

		// Token: 0x0600023B RID: 571 RVA: 0x0000B90E File Offset: 0x00009B0E
		private void PaymentPrintNumberAdd_Click(object sender, RoutedEventArgs e)
		{
			this.buyingPrintNumber = Math.Min(DSLR.maxBuyingPrintLimit, Math.Max(1, this.buyingPrintNumber + 1));
			this.PrintNumberBox.Text = this.buyingPrintNumber.ToString();
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0000B944 File Offset: 0x00009B44
		private void PaymentPrintNumberBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int newNumber = Convert.ToInt32(this.PrintNumberBox.Text);
				int limitNumber = Math.Min(DSLR.maxBuyingPrintLimit, Math.Max(1, newNumber));
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

		// Token: 0x0600023D RID: 573 RVA: 0x0000B9C4 File Offset: 0x00009BC4
		private void DownloadNumberAdd_Click(object sender, RoutedEventArgs e)
		{
			this.buyingDigitalDownloadNumber = Math.Min(DSLR.maxBuyingDigitalDownloadLimit, Math.Max(1, this.buyingDigitalDownloadNumber + 1));
			this.DownloadNumberBox.Text = this.buyingDigitalDownloadNumber.ToString();
		}

		// Token: 0x0600023E RID: 574 RVA: 0x0000B9FA File Offset: 0x00009BFA
		private void DownloadNumberRemove_Click(object sender, RoutedEventArgs e)
		{
			this.buyingDigitalDownloadNumber = Math.Min(DSLR.maxBuyingDigitalDownloadLimit, Math.Max(1, this.buyingDigitalDownloadNumber - 1));
			this.DownloadNumberBox.Text = this.buyingDigitalDownloadNumber.ToString();
		}

		// Token: 0x0600023F RID: 575 RVA: 0x0000BA30 File Offset: 0x00009C30
		private void DownloadNumberBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				int newNumber = Convert.ToInt32(this.DownloadNumberBox.Text);
				int limitNumber = Math.Min(DSLR.maxBuyingDigitalDownloadLimit, Math.Max(1, newNumber));
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

		// Token: 0x06000240 RID: 576 RVA: 0x0000BAB0 File Offset: 0x00009CB0
		private Task<bool> OpenPaymentPage(bool isDigital)
		{
			AISharing.<OpenPaymentPage>d__185 <OpenPaymentPage>d__;
			<OpenPaymentPage>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<OpenPaymentPage>d__.<>4__this = this;
			<OpenPaymentPage>d__.isDigital = isDigital;
			<OpenPaymentPage>d__.<>1__state = -1;
			<OpenPaymentPage>d__.<>t__builder.Start<AISharing.<OpenPaymentPage>d__185>(ref <OpenPaymentPage>d__);
			return <OpenPaymentPage>d__.<>t__builder.Task;
		}

		// Token: 0x06000241 RID: 577 RVA: 0x0000BAFC File Offset: 0x00009CFC
		private void UpdatePaymentButtonText()
		{
			float payTotal = (float)this.buyingDigitalDownloadNumber * DSLR.downloadPrice + (float)this.buyingPrintNumber * DSLR.PrintPrice;
			this.TotalAmount = payTotal;
			if (this.btnApply != null)
			{
				this.btnApply.Content = string.Format("Let's Go - Pay ${0:0.00}", payTotal);
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x0000BB50 File Offset: 0x00009D50
		private void btnApply_Click(object sender, RoutedEventArgs e)
		{
			AISharing.<btnApply_Click>d__187 <btnApply_Click>d__;
			<btnApply_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<btnApply_Click>d__.<>1__state = -1;
			<btnApply_Click>d__.<>t__builder.Start<AISharing.<btnApply_Click>d__187>(ref <btnApply_Click>d__);
		}

		// Token: 0x06000243 RID: 579 RVA: 0x0000BB80 File Offset: 0x00009D80
		public Task<bool> StartPayment(bool isDigital)
		{
			AISharing.<StartPayment>d__188 <StartPayment>d__;
			<StartPayment>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<StartPayment>d__.<>4__this = this;
			<StartPayment>d__.isDigital = isDigital;
			<StartPayment>d__.<>1__state = -1;
			<StartPayment>d__.<>t__builder.Start<AISharing.<StartPayment>d__188>(ref <StartPayment>d__);
			return <StartPayment>d__.<>t__builder.Task;
		}

		// Token: 0x06000244 RID: 580 RVA: 0x0000BBCC File Offset: 0x00009DCC
		private string CalcPaymentSessionInfoText()
		{
			if (DSLR.currentPaymentSession.printCredit > 0 && DSLR.currentPaymentSession.downloadCredit > 0)
			{
				return string.Format("You've unlocked: {0} Print, {1} Digital Download", DSLR.currentPaymentSession.printCredit, DSLR.currentPaymentSession.downloadCredit);
			}
			if (DSLR.currentPaymentSession.printCredit > 0)
			{
				return string.Format("You've unlocked: {0} Print", DSLR.currentPaymentSession.printCredit);
			}
			return string.Format("You've unlocked: {0} Digital Download", DSLR.currentPaymentSession.downloadCredit);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x0000BC5C File Offset: 0x00009E5C
		private void PayingPhotoButton_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000246 RID: 582 RVA: 0x0000BC60 File Offset: 0x00009E60
		private void LoadingPaymentGridLoad()
		{
			AISharing.<>c__DisplayClass191_0 CS$<>8__locals1 = new AISharing.<>c__DisplayClass191_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.Time = StripeController.paymentLimitTime * 60;
			this.PaymentTimeText.Text = CS$<>8__locals1.Time.ToString();
			Task.Run(delegate()
			{
				AISharing.<>c__DisplayClass191_0.<<LoadingPaymentGridLoad>b__0>d <<LoadingPaymentGridLoad>b__0>d;
				<<LoadingPaymentGridLoad>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<LoadingPaymentGridLoad>b__0>d.<>4__this = CS$<>8__locals1;
				<<LoadingPaymentGridLoad>b__0>d.<>1__state = -1;
				<<LoadingPaymentGridLoad>b__0>d.<>t__builder.Start<AISharing.<>c__DisplayClass191_0.<<LoadingPaymentGridLoad>b__0>d>(ref <<LoadingPaymentGridLoad>b__0>d);
				return <<LoadingPaymentGridLoad>b__0>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000247 RID: 583 RVA: 0x0000BCB0 File Offset: 0x00009EB0
		private void MainPaymentBackButton_Click(object sender, RoutedEventArgs e)
		{
			this.PaymentGrid.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000248 RID: 584 RVA: 0x0000BCBE File Offset: 0x00009EBE
		private void PayingDigitalPhotoButton_Click(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x0600024C RID: 588 RVA: 0x0000CB8F File Offset: 0x0000AD8F
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 6)
			{
				((System.Windows.Controls.Image)target).Loaded += this.PhotosIcon_Loaded;
			}
		}

		// Token: 0x0400014A RID: 330
		private bool firstWork = true;

		// Token: 0x0400014B RID: 331
		public static BitmapImage bitmapResultPhoto;

		// Token: 0x0400014C RID: 332
		public static BitmapImage bitmapThumbnailResultPhoto;

		// Token: 0x0400014D RID: 333
		public static BitmapImage bitmapOriginalGridPhoto;

		// Token: 0x0400014E RID: 334
		public static BitmapImage bitmapAiImagesGridPhoto;

		// Token: 0x0400014F RID: 335
		private string faceSwapmailAddress = string.Empty;

		// Token: 0x04000150 RID: 336
		private string AIEffectmailAddress = string.Empty;

		// Token: 0x04000151 RID: 337
		private int photoPrintedNumber;

		// Token: 0x04000152 RID: 338
		private int limitForPrintPhoto;

		// Token: 0x04000153 RID: 339
		private bool isDirectPrint;

		// Token: 0x04000154 RID: 340
		private bool changeLiveView;

		// Token: 0x04000155 RID: 341
		private bool isExpandedQR;

		// Token: 0x04000156 RID: 342
		public bool IsPaymentActive;

		// Token: 0x04000157 RID: 343
		private bool IsPayPrint;

		// Token: 0x04000158 RID: 344
		private bool IsPayDigital;

		// Token: 0x04000159 RID: 345
		private bool PaidPrint;

		// Token: 0x0400015A RID: 346
		private bool PaidDigital;

		// Token: 0x0400015B RID: 347
		private CancellationTokenSource PaymentCancellationToken = new CancellationTokenSource();

		// Token: 0x0400015C RID: 348
		public static MediaClassBase SharingMediaclass;

		// Token: 0x0400015D RID: 349
		private List<VariationMedia> variationsOfOriginalMedia = new List<VariationMedia>();

		// Token: 0x0400015E RID: 350
		private string appPath = AppDomain.CurrentDomain.BaseDirectory;

		// Token: 0x0400015F RID: 351
		private static int currentMidjourneyMediaIndex;

		// Token: 0x04000160 RID: 352
		private int printNumber;

		// Token: 0x04000161 RID: 353
		private ObservableCollection<Frame> samplePromptItems = new ObservableCollection<Frame>();

		// Token: 0x04000162 RID: 354
		private List<SamplePromptPrefab> samplePromptPrefabs = new List<SamplePromptPrefab>();

		// Token: 0x04000163 RID: 355
		private List<SamplePrompt> samplePrompts;

		// Token: 0x04000164 RID: 356
		private ObservableCollection<Frame> AIEffectsItems = new ObservableCollection<Frame>();

		// Token: 0x04000165 RID: 357
		private List<AIEffectGalleryPrefab> AIEffectGalleryPrefabs = new List<AIEffectGalleryPrefab>();

		// Token: 0x04000166 RID: 358
		private List<AiEffectLocal> aiEffectLocals;

		// Token: 0x04000167 RID: 359
		private bool isAIImagesSelected;

		// Token: 0x04000168 RID: 360
		private bool isFirstCountdownActive = true;

		// Token: 0x04000169 RID: 361
		private bool isSecondCountdownActive = true;

		// Token: 0x0400016B RID: 363
		private BitmapImage galleryIconImage;

		// Token: 0x0400016D RID: 365
		private ObservableCollection<Frame> loadingItems = new ObservableCollection<Frame>();

		// Token: 0x0400016F RID: 367
		private int processCounter;

		// Token: 0x04000170 RID: 368
		private static bool refreshLoadingWorking;

		// Token: 0x04000171 RID: 369
		private int buyingPrintNumber;

		// Token: 0x04000172 RID: 370
		private int buyingDigitalDownloadNumber;

		// Token: 0x04000173 RID: 371
		private bool isWaitingPayment;
	}
}
