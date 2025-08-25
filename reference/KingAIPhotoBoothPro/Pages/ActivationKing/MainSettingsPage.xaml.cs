using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x0200004D RID: 77
	public partial class MainSettingsPage : SettingsSubPage
	{
		// Token: 0x17000085 RID: 133
		// (get) Token: 0x060005D4 RID: 1492 RVA: 0x000287CF File Offset: 0x000269CF
		// (set) Token: 0x060005D5 RID: 1493 RVA: 0x000287D6 File Offset: 0x000269D6
		private static TextBlock statusTextBlock { get; set; }

		// Token: 0x17000086 RID: 134
		// (get) Token: 0x060005D6 RID: 1494 RVA: 0x000287DE File Offset: 0x000269DE
		// (set) Token: 0x060005D7 RID: 1495 RVA: 0x000287E5 File Offset: 0x000269E5
		private static System.Windows.Shapes.Path succesIconInstance { get; set; }

		// Token: 0x17000087 RID: 135
		// (get) Token: 0x060005D8 RID: 1496 RVA: 0x000287ED File Offset: 0x000269ED
		// (set) Token: 0x060005D9 RID: 1497 RVA: 0x000287F4 File Offset: 0x000269F4
		private static System.Windows.Shapes.Path warningIconInstance { get; set; }

		// Token: 0x17000088 RID: 136
		// (get) Token: 0x060005DA RID: 1498 RVA: 0x000287FC File Offset: 0x000269FC
		// (set) Token: 0x060005DB RID: 1499 RVA: 0x00028803 File Offset: 0x00026A03
		private static System.Windows.Shapes.Path errorIconInstance { get; set; }

		// Token: 0x17000089 RID: 137
		// (get) Token: 0x060005DC RID: 1500 RVA: 0x0002880B File Offset: 0x00026A0B
		// (set) Token: 0x060005DD RID: 1501 RVA: 0x00028812 File Offset: 0x00026A12
		private static Border txtStatusBorderInstance { get; set; }

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x060005DE RID: 1502 RVA: 0x0002881C File Offset: 0x00026A1C
		// (remove) Token: 0x060005DF RID: 1503 RVA: 0x00028850 File Offset: 0x00026A50
		public static event Action<bool> VideoGifCheckChanged;

		// Token: 0x1700008A RID: 138
		// (get) Token: 0x060005E0 RID: 1504 RVA: 0x00028883 File Offset: 0x00026A83
		// (set) Token: 0x060005E1 RID: 1505 RVA: 0x000288B6 File Offset: 0x00026AB6
		private Visibility GetCameraSettingsVisibilty
		{
			get
			{
				if (MainSettingsPage.selectedCameraType != MainSettingsPage.CameraType.DSLR)
				{
					return Visibility.Hidden;
				}
				if (this.CameraListPrefab.ComboNameBox.SelectedItem == null || this.CameraListPrefab.ComboNameBox.SelectedIndex == -1)
				{
					return Visibility.Hidden;
				}
				return Visibility.Visible;
			}
			set
			{
				this.getCameraSettingsVisibilty = value;
			}
		}

		// Token: 0x1700008B RID: 139
		// (get) Token: 0x060005E2 RID: 1506 RVA: 0x000288C0 File Offset: 0x00026AC0
		public static Mode SelectedMode
		{
			get
			{
				Mode[] modes = (Mode[])Enum.GetValues(typeof(Mode));
				foreach (Mode mode in modes)
				{
					string modeString = mode.GetDisplayName();
					if (modeString == KingAIPhotoBoothPro.Class.Helper.Settings.GetValueString("mode"))
					{
						return mode;
					}
				}
				return Mode.MultifunctionalPhotoBooth;
			}
		}

		// Token: 0x1700008C RID: 140
		// (get) Token: 0x060005E3 RID: 1507 RVA: 0x0002891C File Offset: 0x00026B1C
		public static AIMode AISelectedMode
		{
			get
			{
				AIMode[] modes = (AIMode[])Enum.GetValues(typeof(AIMode));
				foreach (AIMode mode in modes)
				{
					string modeString = mode.GetDisplayName();
					if (modeString == KingAIPhotoBoothPro.Class.Helper.Settings.GetValueString("selectedaimode"))
					{
						return mode;
					}
				}
				return AIMode.AIPromptTheme;
			}
		}

		// Token: 0x060005E4 RID: 1508 RVA: 0x00028978 File Offset: 0x00026B78
		public MainSettingsPage()
		{
			this.InitializeComponent();
			MainSettingsPage.ColorDropperCursor = new System.Windows.Input.Cursor(System.Windows.Application.GetResourceStream(new Uri("/Images/colorDropper.cur", UriKind.Relative)).Stream);
			this.PhotoBox.Cursor = MainSettingsPage.ColorDropperCursor;
			this.InitializePrefabs();
			this.InitializeLoadingGif();
			this.InitializeUiElementEvents();
			this.SetPrefabFrameContents();
			this.SetElementsInitialValues();
			MainSettingsPage.Instance = this;
			EventManagementPage.onEventChanged += this.EventManagementPage_onEventChanged;
			this.AIMotionPrompPrefab.txtInput.TextChanged += this.AIPrompTxtInput_TextChanged;
			this.aiOptionCheckBox = new List<CheckBoxPrefab>
			{
				this.AiPromptOptionActivePrefab,
				this.FaceSwapOptionActivePrefab,
				this.AiEffectOptionActivePrefab,
				this.WordPortraitOptionActivePrefab
			};
		}

		// Token: 0x060005E5 RID: 1509 RVA: 0x00028AA0 File Offset: 0x00026CA0
		private void AIPrompTxtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.AIMotionPrompPrefab.txtInput.Text.Length < 1)
			{
				this.AIMotionPrompPrefab.txtInput.Text = this.AIMotionPrompPrefab.DefaultStatus;
			}
		}

		// Token: 0x060005E6 RID: 1510 RVA: 0x00028AD8 File Offset: 0x00026CD8
		public static ApplicationPage SelectAIPage()
		{
			ApplicationPage selectedPage = ApplicationPage.PhotoShootPage;
			switch (MainSettingsPage.AISelectedMode)
			{
			case AIMode.AIPromptTheme:
				selectedPage = ApplicationPage.AIPrompGalleryPage;
				break;
			case AIMode.AIFaceSwap:
				selectedPage = ApplicationPage.FaceSwapBackgroundGalleryPage;
				break;
			case AIMode.AIEffect:
				selectedPage = ApplicationPage.AiEffectGalleryPage;
				break;
			}
			return selectedPage;
		}

		// Token: 0x060005E7 RID: 1511 RVA: 0x00028B16 File Offset: 0x00026D16
		private void EventManagementPage_onEventChanged(OperationalEvent newEvent)
		{
			MainSettingsPage.SetUiElementValues();
			SettingsPage.templatePageVideo.RefreshTemplate();
			SettingsPage.templatePage.RefreshTemplate();
		}

		// Token: 0x060005E8 RID: 1512 RVA: 0x00028B34 File Offset: 0x00026D34
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			MainSettingsPage.<Page_Loaded>d__99 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<MainSettingsPage.<Page_Loaded>d__99>(ref <Page_Loaded>d__);
		}

		// Token: 0x060005E9 RID: 1513 RVA: 0x00028B6C File Offset: 0x00026D6C
		private void AutoSelectWebcam()
		{
			if (this.CameraTypeListPrefab.ComboNameBox.SelectedItem == null && KingAIPhotoBoothPro.Class.Helper.Settings.GetValueString("cameratype") == null)
			{
				this.CameraTypeListPrefab.ComboNameBox.SelectedIndex = 2;
				if (this.CameraListPrefab.ComboNameBox.Items.Count > 0)
				{
					this.CameraListPrefab.ComboNameBox.SelectedIndex = 0;
					System.Windows.Controls.ComboBox comboNameBox = this.CameraListPrefab.ComboNameBox;
					object selectedItem = this.CameraListPrefab.ComboNameBox.SelectedItem;
					comboNameBox.Text = ((selectedItem != null) ? selectedItem.ToString() : null);
					if (this.RotationListPrefab.ComboNameBox.SelectedItem == null && this.RotationListPrefab.ComboNameBox.Items.Count > 0)
					{
						this.RotationListPrefab.ComboNameBox.SelectedIndex = 0;
					}
				}
			}
			Task.Run(delegate()
			{
				MainSettingsPage.<<AutoSelectWebcam>b__100_0>d <<AutoSelectWebcam>b__100_0>d;
				<<AutoSelectWebcam>b__100_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<AutoSelectWebcam>b__100_0>d.<>4__this = this;
				<<AutoSelectWebcam>b__100_0>d.<>1__state = -1;
				<<AutoSelectWebcam>b__100_0>d.<>t__builder.Start<MainSettingsPage.<<AutoSelectWebcam>b__100_0>d>(ref <<AutoSelectWebcam>b__100_0>d);
				return <<AutoSelectWebcam>b__100_0>d.<>t__builder.Task;
			});
		}

		// Token: 0x060005EA RID: 1514 RVA: 0x00028C50 File Offset: 0x00026E50
		private static void SetUiElementValues()
		{
			int maximumVal = (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueInt("videotime") == null) ? 8 : KingAIPhotoBoothPro.Class.Helper.Settings.GetValueInt("videotime").Value;
			double slowMoStart = (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueFloat("slowmotionstart") == null) ? 2.0 : ((double)KingAIPhotoBoothPro.Class.Helper.Settings.GetValueFloat("slowmotionstart").Value);
			double slowMoEnd = (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueFloat("slowmotionend") == null) ? 4.0 : ((double)KingAIPhotoBoothPro.Class.Helper.Settings.GetValueFloat("slowmotionend").Value);
			MainSettingsPage.Instance.SliderForSlowMotion.Maximum = (double)maximumVal;
			MainSettingsPage.Instance.SliderForSlowMotion.LowerValue = slowMoStart;
			MainSettingsPage.Instance.SliderForSlowMotion.HigherValue = slowMoEnd;
			MainSettingsPage.Instance.RangeSlider_HigherValueChanged(null, null);
			MainSettingsPage.Instance.RangeSlider_LowerValueChanged(null, null);
			if (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("musiccheck").GetValueOrDefault())
			{
				MainSettingsPage.Instance.Music_Checked(null, null);
			}
			else
			{
				MainSettingsPage.Instance.Music_Unchecked(null, null);
			}
			if (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("greenbox").GetValueOrDefault())
			{
				MainSettingsPage.Instance.GreenBox_Checked(null, null);
			}
			else
			{
				MainSettingsPage.Instance.GreenBox_Unchecked(null, null);
			}
			if (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("aibackground").GetValueOrDefault())
			{
				MainSettingsPage.Instance.AIBackgroundToggleElement_Checked(null, null);
			}
			if (KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("slowmotionenabled").GetValueOrDefault())
			{
				MainSettingsPage.Instance.SlowMotionEnabled_Checked(null, null);
				return;
			}
			MainSettingsPage.Instance.SlowMotionEnabled_Unchecked(null, null);
		}

		// Token: 0x060005EB RID: 1515 RVA: 0x00028DEC File Offset: 0x00026FEC
		private void SetElementsInitialValues()
		{
			if (!SettingsPage.isTurnedBack)
			{
				this.TestPhotoButton.IsEnabled = false;
				this.GreenBoxTreshOldPrefab.txtInput.IsEnabled = false;
				this.GridForChromaKey.Visibility = Visibility.Collapsed;
				this.TestPhotoButton.Background = new SolidColorBrush(AppInfo.ThemeColor);
				if (this.firstWork)
				{
					this.RotationListPrefab.IsEnabled = false;
					this.firstWork = false;
				}
				MainSettingsPage.CameraType[] cameraTypes = (MainSettingsPage.CameraType[])Enum.GetValues(typeof(MainSettingsPage.CameraType));
				List<string> cameraTypeList = new List<string>();
				foreach (MainSettingsPage.CameraType cameraType in cameraTypes)
				{
					cameraTypeList.Add(cameraType.ToString());
				}
				this.CameraTypeListPrefab.ComboNameBox.ItemsSource = cameraTypeList;
				this.CameraTypeListPrefab.ComboNameBox.SelectionChanged += this.CameraType_SelectionChangedAsync;
				Mode[] modes = (Mode[])Enum.GetValues(typeof(Mode));
				List<string> modeList = new List<string>();
				foreach (Mode mode in modes)
				{
					string modeString = mode.GetDisplayName();
					modeList.Add(modeString);
				}
				this.ModeListPrefab.ComboNameBox.ItemsSource = modeList;
				this.ModeListPrefab.ComboNameBox.SelectionChanged += this.Mode_SelectionChangedAsync;
				AIMode[] AImodes = (AIMode[])Enum.GetValues(typeof(AIMode));
				List<string> AImodeList = new List<string>();
				foreach (AIMode AImode in AImodes)
				{
					string modeString2 = AImode.GetDisplayName();
					AImodeList.Add(modeString2);
				}
				this.AiSelectionPrefab.ComboNameBox.ItemsSource = AImodeList;
				this.AiSelectionPrefab.ComboNameBox.SelectionChanged += this.AISelectionComboNameBox_SelectionChanged;
				List<string> paymentOptions = new List<string>
				{
					"Print & download",
					"Print only",
					"Download only"
				};
				List<string> paymentScreens = new List<string>
				{
					"At Start Screen",
					"At Sharing Screen"
				};
				this.PaymentTimeComboboxPrefab.ComboNameBox.ItemsSource = paymentOptions;
				this.PaymentOptionComboboxPrefab.ComboNameBox.ItemsSource = paymentScreens;
			}
			MainSettingsPage.statusTextBlock = this.txtStatus;
			MainSettingsPage.succesIconInstance = this.successIcon;
			MainSettingsPage.warningIconInstance = this.warningIcon;
			MainSettingsPage.errorIconInstance = this.errorIcon;
			MainSettingsPage.txtStatusBorderInstance = this.txtStatusBorder;
			this.txtStatus.Visibility = Visibility.Visible;
			this.txtStatusGrid.Visibility = Visibility.Visible;
			MainSettingsPage.StatusOutput("Chose Camera Type", MainSettingsPage.StatusOutputColor.Warning);
			this.RotationGrid.Visibility = Visibility.Collapsed;
			this.GifCheckFrame.Visibility = Visibility.Collapsed;
			this.VideoCheckFrame.Visibility = Visibility.Collapsed;
			this.PhotoCheckFrame.Visibility = Visibility.Collapsed;
			this.UploadDesktopEditCheckFrame.Visibility = Visibility.Collapsed;
			this.retryBTButton.Visibility = Visibility.Hidden;
			this.retryWifiButton.Visibility = Visibility.Hidden;
			this.ForceWifiCheckFrame.Visibility = Visibility.Hidden;
			this.CameraListFrame.Visibility = Visibility.Hidden;
		}

		// Token: 0x060005EC RID: 1516 RVA: 0x00029100 File Offset: 0x00027300
		private void AISelectionComboNameBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth)
			{
				System.Windows.Controls.ComboBox comboBox = sender as System.Windows.Controls.ComboBox;
				if (comboBox == null)
				{
					comboBox = this.AiSelectionPrefab.ComboNameBox;
				}
				this.SelectAndCloseOtherAIOptions(comboBox.SelectedIndex);
			}
		}

		// Token: 0x060005ED RID: 1517 RVA: 0x00029138 File Offset: 0x00027338
		private void SelectAndCloseOtherAIOptions(int ID)
		{
			for (int i = 0; i < this.aiOptionCheckBox.Count; i++)
			{
				this.aiOptionCheckBox[i].CheckElement.IsChecked = new bool?(ID == i);
				KingAIPhotoBoothPro.Class.Helper.Settings.SetValue(this.aiOptionCheckBox[i].Key, ID == i);
			}
		}

		// Token: 0x060005EE RID: 1518 RVA: 0x00029194 File Offset: 0x00027394
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.isCameraSelected)
			{
				if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
				{
					if (CameraControlClass.DeviceManager != null)
					{
						CameraControlClass.DeviceManager.CameraConnected -= this.DeviceManager_CameraConnected;
						CameraControlClass.DeviceManager.CameraDisconnected -= this.DeviceManager_CameraDisconnected;
						CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapture));
						CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Remove(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailed));
						return;
					}
				}
				else if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
				{
					WebcamControlClass.PhotoCaptured -= this.WebcamControlClass_PhotoCaptured;
					WebcamControlClass.WebcamConnected -= this.WebcamControlClass_WebcamConnected;
					WebcamControlClass.WebcamDisconnected -= this.WebcamControlClass_WebcamDisconnected;
				}
			}
		}

		// Token: 0x060005EF RID: 1519 RVA: 0x00029268 File Offset: 0x00027468
		private void SetPrefabFrameContents()
		{
			this.AIBackgroundEnabled.Content = this.aIBackgroundPrefab;
			this.GreenBoxEnabled.Content = this.GreenBoxPrefab;
			this.MirrorCheckFrame.Content = this.MirrorCheckPrefab;
			this.MirrorVideoCheckFrame.Content = this.MirrorVideoCheckPrefab;
			this.folderBrowser.Content = this.FolderPrefab;
			this.CameraListFrame.Content = this.CameraListPrefab;
			this.CameraTypeListFrame.Content = this.CameraTypeListPrefab;
			this.RotationListFrame.Content = this.RotationListPrefab;
			this.GreenBoxTreshOld.Content = this.GreenBoxTreshOldPrefab;
			this.colorGreenScreen.Content = this.colorGreenScreenPrefab;
			this.SlowMotionCheckFrame.Content = this.SlowMotionCheckPrefab;
			this.VideoLengthFrame.Content = this.VideoTimePrefab;
			this.GifFrameRateFrame.Content = this.GifFrameRatePrefab;
			this.GifRepeatCountFrame.Content = this.GifRepeatCountPrefab;
			this.SlowVideoEndFrame.Content = this.SlowMotionEndTimePrefab;
			this.SlowVideoStartFrame.Content = this.SlowMotionStartTimePrefab;
			this.MusicPathFrame.Content = this.musicFilePrefab;
			this.MusicSelectCheckFrame.Content = this.MusicCheckBoxPrefab;
			this.ForceWifiCheckFrame.Content = this.ForceWifiCheckPrefab;
			this.VideoWaitLenghtFrame.Content = this.VideoWaitLenghtPrefab;
			this.ModeListFrame.Content = this.ModeListPrefab;
			this.AiSelectionFrame.Content = this.AiSelectionPrefab;
			this.AiPromptActiveFrame.Content = this.AiPromptOptionActivePrefab;
			this.OrginalActiveFrame.Content = this.OrginalOptionActivePrefab;
			this.FaceSwapActiveFrame.Content = this.FaceSwapOptionActivePrefab;
			this.WordPortraitActiveFrame.Content = this.WordPortraitOptionActivePrefab;
			this.AiMotionActiveFrame.Content = this.AiMotionOptionActivePrefab;
			this.AiEffectActiveFrame.Content = this.AiEffectOptionActivePrefab;
			this.AiBeautifierActiveFrame.Content = this.AiBeautifierOptionActivePrefab;
			this.PhotoCheckFrame.Content = this.PhotoCheckPrefab;
			this.CameraPreviewVideoBrightnessSliderFrame.Content = this.CameraPreviewVideoBrightnessSliderPrefab;
			this.SlowMotionRateFrame.Content = this.SlowMotionRatePrefab;
			this.GalleryCheckFrame.Content = this.GalleryCheckPrefab;
			this.AiPromptResolutionSegmentedFrame.Content = this.AiPromptResolutionSegmentedPrefab;
			this.AiPromptTextBoxCheckFrame.Content = this.AiPromptTextBoxTogglePrefab;
			this.VideoCheckFrame.Content = this.VideoCheckPrefab;
			this.BackgroundDeliveryFrame.Content = this.BackgroundDeliveryPrefab;
			this.UploadDesktopEditCheckFrame.Content = this.DesktopEditingCheckPrefab;
			this.FullScreenPreviewFrame.Content = this.FullScreenPreviewCheckPrefab;
			this.BackgroundPreviewFrame.Content = this.BackgroundPreviewCheckPrefab;
			this.GifCheckFrame.Content = this.GifCheckPrefab;
			this.AIMotionPrompFrame.Content = this.AIMotionPrompPrefab;
			this.securityPasswordFrame.Content = this.ReturnPasswordPrefab;
			this.RunOnStartupFrame.Content = this.RunOnStartupPrefab;
			this.PaymentTimeComboboxFrame.Content = this.PaymentTimeComboboxPrefab;
			this.PaymentOptionComboboxFrame.Content = this.PaymentOptionComboboxPrefab;
			this.PaymentActiveFrame.Content = this.PaymentsActivePrefab;
			this.PaymentPrintPriceFrame.Content = this.PaymentPrintPricePrefab;
			this.PaymentCapturePriceFrame.Content = this.PaymentCapturePricePrefab;
			this.StripeAccountIDFrame.Content = this.StripeAccountIDPrefab;
			this.StripeSecretKeyFrame.Content = this.StripeSecretKeyPrefab;
		}

		// Token: 0x060005F0 RID: 1520 RVA: 0x000295D8 File Offset: 0x000277D8
		private void InitializePrefabs()
		{
			this.MusicCheckBoxPrefab = new TogglePrefab("musiccheck", "", false, false);
			this.ForceWifiCheckPrefab = new CheckBoxPrefab("forcewifi", "Force Use this GoPro Wifi", false);
			this.FolderPrefab = new FolderBrowserPrefab("capturedfolder", "Folder to Save Captured", "", "Event Name will be shown in the main display area.");
			this.musicFilePrefab = new FileBrowserPrefab("musicpath", "Music File Path", "", "", "Audio Files (*.mp3, *.wav)|*.mp3;*.wav");
			this.SlowMotionStartTimePrefab = new InputBoxPrefab("slowmotionstart", "Slow Motion Start", "2", "(Seconds)", false);
			this.SlowMotionEndTimePrefab = new InputBoxPrefab("slowmotionend", "Slow Motion End", "4", "(Seconds)", false);
			this.colorGreenScreenPrefab = new ColorPickerPrefab("colorgreenscreen", "Select ChromaKey Color", "#00000000");
			this.SlowMotionCheckPrefab = new TogglePrefab("slowmotionenabled", "", false, false);
			this.SlowMotionRatePrefab = new InputBoxPrefab("slowmotionrate", "SlowMotion Rate", "4", "Determines the rate at which your video will be slowed down", false);
			this.SlowMotionRatePrefab.ToolTip = "Video length is directly proportional to the slowdown ratio. For example: 4x slow motion = 1 second interval will be 4 seconds. Warning! High slowmotion rate require high fps.";
			this.ModeListPrefab = new ComboboxPrefab("mode", "Select Mode", "", "Select the photobooth mode", false);
			this.AiSelectionPrefab = new ComboboxPrefab("selectedaimode", "Select AI Mode", "", "Select the photobooth AI mode", false);
			this.AiPromptResolutionSegmentedPrefab = new SegmentedPrefab("aipromptresolution4k", "HD (Fast)", "4K Res", true, false, "Fast : 6 Credits | 4K : 10 Credits");
			this.AiPromptTextBoxTogglePrefab = new TogglePrefab("aipromptextbox", "AI Prompt Text Box", false, false);
			this.AiPromptOptionActivePrefab = new CheckBoxPrefab("aipromptoptionactive", "Ai Prompt", false);
			this.OrginalOptionActivePrefab = new CheckBoxPrefab("orginaloption", "Orginal Active", false);
			this.FaceSwapOptionActivePrefab = new CheckBoxPrefab("faceswapactive", "Face Swap", true);
			this.WordPortraitOptionActivePrefab = new CheckBoxPrefab("wordportraitoptionactive", "Word Portrait", false);
			this.AiMotionOptionActivePrefab = new CheckBoxPrefab("aimotionoptionactive", "Ai Motion (BETA)", false);
			this.AiEffectOptionActivePrefab = new CheckBoxPrefab("aieffectoptionactive", "Ai Effect", true);
			this.AiBeautifierOptionActivePrefab = new CheckBoxPrefab("aieffectoptionactive", "Ai Beautifier", true);
			this.PhotoCheckPrefab = new CheckBoxPrefab("photo", "Photo", true);
			this.VideoCheckPrefab = new CheckBoxPrefab("video", "Video / 360", true);
			this.GifCheckPrefab = new CheckBoxPrefab("gif", "GIF (Multi Shoot Video)", true);
			this.GalleryCheckPrefab = new CheckBoxPrefab("gallery", "Gallery", true);
			this.GifCheckPrefab.ToolTip = "Gif video will be generated if you add more than one photo shoot on image template.";
			this.DesktopEditingCheckPrefab = new CheckBoxPrefab("upload", "Desktop Editing", true);
			this.DesktopEditingCheckPrefab.ToolTip = "When desktop editing mode is on, you can perform AI Face Swap or AI Prompt Theme operations with photos on your computer without taking a photo.\\r\\n\\r\\nWith this mode, you can use our software not only as a photo booth but also as a desktop software.\\r\\n\\r\\nWe recommend you to use this mode to produce creative images for your AI Face\u00a0Swap\u00a0Gallery";
			this.BackgroundDeliveryPrefab = new CheckBoxPrefab("realtime", "Send results as email without waiting", false);
			this.FullScreenPreviewCheckPrefab = new CheckBoxPrefab("fullscreenpreview", "Fullscreen Preview", false);
			this.FullScreenPreviewCheckPrefab.ToolTip = "After taking a photo, you can see the output photo in a larger size in a separate window.";
			this.BackgroundPreviewCheckPrefab = new CheckBoxPrefab("backgroundpreviewvideo", "Background Preview Video", false);
			this.BackgroundPreviewCheckPrefab.ToolTip = "Allows you to see camera sections in the background at main page";
			this.OrginalOptionActivePrefab.ToolTip = "Activate to see original photos next to AI photos";
			this.CameraTypeListPrefab = new ComboboxPrefab("cameratype", "Select Camera Type", "Select Camera type", "Select your camera type", true);
			this.CameraListPrefab = new ComboboxPrefab("cameralist", "Select Camera", "", "All connected cameras are listed", true);
			this.RotationListPrefab = new ComboboxPrefab("rotation", "Camera Rotation", "", "", false);
			this.GreenBoxPrefab = new TogglePrefab("greenbox", " ", false, false);
			this.GreenBoxTreshOldPrefab = new InputBoxPrefab("greenboxthrash", "Threshold", "100", "0-255", false);
			this.aIBackgroundPrefab = new TogglePrefab("aibackground", "", false, false);
			this.VideoTimePrefab = new InputBoxPrefab("videotime", "Video Time", "6", "", false);
			this.GifFrameRatePrefab = new InputBoxPrefab("gifframerate", "Gif Frame Rate", "2", "", false);
			this.GifRepeatCountPrefab = new InputBoxPrefab("gifrepeatcount", "Gif Repeat Count", "3", "", false);
			this.VideoWaitLenghtPrefab = new InputBoxPrefab("countdownseconds", "Countdown Seconds", "5", "", false);
			this.AIMotionPrompPrefab = new InputBoxPrefab("aimotionpromp", "Promp", "fashion model posing dynamically with a slight smile, camera dolly zoom effect", "The text you write here will determine the movement of your video.", false);
			this.MirrorCheckPrefab = new CheckBoxPrefab("mirrorview", "Mirror Camera Horizontally On Preview Screen", false);
			this.MirrorVideoCheckPrefab = new CheckBoxPrefab("mirrorvideo", "Mirror Captured Video", false);
			this.CameraPreviewVideoBrightnessSliderPrefab = new SliderPrefab("previewbrightness", "Brightness", 0, 100, 50, true, false, "", "");
			this.CameraPreviewVideoBrightnessSliderPrefab.ToolTip = "It is the brightness setting of the screen that appears as the background in the main menu. If \"Background Preview Video\" is not active, it will not have any effect.";
			this.ReturnPasswordPrefab = new InputBoxPrefab("returnpassword", "Security Pin", "", "Pin to unlock admin settings.", false);
			this.PaymentTimeComboboxPrefab = new ComboboxPrefab("paymenttime", "Choose Payment Option", "Print & Download", "What do you want to pay for?", false);
			this.PaymentOptionComboboxPrefab = new ComboboxPrefab("paymentscreen", "When To Request Payment", "At Start Screen", "Pay first or take photo first?", false);
			this.RunOnStartupPrefab = new CheckBoxPrefab("runonstartup", "Automatically launch AI Photo Booth Pro with the most recent event when application starts.", false);
			this.PaymentsActivePrefab = new CheckBoxPrefab("paymentactive", "Payments Active", false);
			this.PaymentPrintPricePrefab = new InputBoxPrefab("printprice", "Print Price", "0", "$", false);
			this.PaymentCapturePricePrefab = new InputBoxPrefab("captureprice", "Download Price", "0", "$", false);
			this.StripeAccountIDPrefab = new InputBoxPrefab("stripeaccountid", "Stripe Account ID", "", "https://dashboard.stripe.com/test/settings/account example : acct_XXXX", false);
			this.StripeSecretKeyPrefab = new InputBoxPrefab("stripesecret", "Stripe Secret Key", "", "https://dashboard.stripe.com/test/apikeys example : sk_XXXX", false);
			this.StripeAccountIDPrefab.ToolTip = "You can find Account ID in \\\"https://dashboard.stripe.com/test/settings/account\\\" example : acct_XXXX\"";
			this.StripeSecretKeyPrefab.ToolTip = "You can find Secret Key in \"https://dashboard.stripe.com/test/apikeys\" example : sk_XXXX";
		}

		// Token: 0x060005F1 RID: 1521 RVA: 0x00029BE4 File Offset: 0x00027DE4
		private void ReorganizeUi()
		{
			this.ReOrganizeModeSectionElements();
			this.ReorganizeShootingSection();
			this.ReOrganizeSections();
			SettingsPage settingsPage = ActivationKingWindow.SettingsPage;
			if (settingsPage == null)
			{
				return;
			}
			settingsPage.HideOrOpenButtons();
		}

		// Token: 0x060005F2 RID: 1522 RVA: 0x00029C08 File Offset: 0x00027E08
		private void ReOrganizeModeSectionElements()
		{
			this.panelAiOptions.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiOptionsVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.panelAiPromptQuality.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiPromptResolutionVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.panelAiPromptTextBoxOption.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiPromptResolutionVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.panelShootingOptions.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.ModeShootingOptionsVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.AiMotionActiveFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiMotionActiveFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.aiMotionSettings.Visibility = (this.AiMotionOptionActivePrefab.CheckElement.IsChecked.Value ? Visibility.Visible : Visibility.Collapsed);
			this.CameraPreviewVideoBrightnessSettings.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.CameraPreviewBrightnessFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.OrginalActiveFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiSelectionFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			if (!KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiMotionActiveFrameVisibility)
			{
				this.AiMotionOptionActivePrefab.SetValue(false);
			}
			this.AiSelectionFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.AiSelectionFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.PhotoCheckFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.PhotoShootingOptionVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.VideoCheckFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.VideoShootingOptionVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.GifCheckFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.GifShootingOptionVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.UploadDesktopEditCheckFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.DesktopEditingShootingOptionVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.panelDeliveryOptions.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.BackgroundDeliveryOptionVisibility ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x060005F3 RID: 1523 RVA: 0x00029D70 File Offset: 0x00027F70
		private void ReorganizeShootingSection()
		{
			this.GifFrameRateFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.GifFrameRateFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.GifRepeatCountFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.GifRepeatCountFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.VideoLengthFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.VideoLengthFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.MirrorVideoCheckFrame.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.VideoLengthFrameVisibility ? Visibility.Visible : Visibility.Collapsed);
		}

		// Token: 0x060005F4 RID: 1524 RVA: 0x00029DD8 File Offset: 0x00027FD8
		private void ReOrganizeSections()
		{
			double modeSettingsHeight = this.gridMain.ActualWidth * 0.10999999940395355;
			this.panelModeInner.UpdateLayout();
			modeSettingsHeight += this.panelModeInner.ActualHeight;
			this.sectionModeSettings.Height = modeSettingsHeight;
			this.gridAIBackgroundSettings.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.SectionAIBackgroundSettingsVisibility ? Visibility.Visible : Visibility.Collapsed);
			this.gridChromaKeySettings.Visibility = (KingAIPhotoBoothPro.Class.Helper.Settings.ElementsVisibilities.SectionChromeKeyBackgroundSettingsVisibility ? Visibility.Visible : Visibility.Collapsed);
			double shootingSettingsHeight = this.gridMain.ActualWidth * 0.07999999821186066;
			this.panelShootingInner.UpdateLayout();
			shootingSettingsHeight += this.panelShootingInner.ActualHeight;
			this.gridCameraSettings.Height = shootingSettingsHeight;
		}

		// Token: 0x060005F5 RID: 1525 RVA: 0x00029E88 File Offset: 0x00028088
		private void InitializeUiElementEvents()
		{
			this.GreenBoxPrefab.toggleElement.Checked += this.GreenBox_Checked;
			this.GreenBoxPrefab.toggleElement.Unchecked += this.GreenBox_Unchecked;
			this.BackgroundDeliveryPrefab.CheckElement.Checked += this.RealtimeDeliveryCheckElement_Checked;
			this.RunOnStartupPrefab.CheckElement.Checked += this.RunOnStartupCheckElement_Checked;
			this.RunOnStartupPrefab.CheckElement.Unchecked += this.RunOnStartupCheckElement_Unchecked;
			this.aIBackgroundPrefab.toggleElement.Checked += this.AIBackgroundToggleElement_Checked;
			this.GreenBoxTreshOldPrefab.txtInput.TextChanged += this.TxtInput_TextChanged;
			this.colorGreenScreenPrefab.colorChanged += this.ColorElement_colorChanged;
			this.VideoTimePrefab.txtInput.TextChanged += this.VideoTime_TextChanged;
			this.MusicCheckBoxPrefab.toggleElement.Checked += this.Music_Checked;
			this.MusicCheckBoxPrefab.toggleElement.Unchecked += this.Music_Unchecked;
			this.CameraListPrefab.ComboNameBox.SelectionChanged += this.CameraList_SelectionChangedAsync;
			this.SlowMotionCheckPrefab.toggleElement.Checked += this.SlowMotionEnabled_Checked;
			this.SlowMotionCheckPrefab.toggleElement.Unchecked += this.SlowMotionEnabled_Unchecked;
			GoProCameraControlClass.returnWifiConnected += this.GoProCameraControlClass_returnWifiConnected;
			this.SlowMotionStartTimePrefab.txtInput.TextChanged += this.SlowMotionStart_TextChanged;
			this.SlowMotionEndTimePrefab.txtInput.TextChanged += this.SlowMotionEnd_TextChanged;
			this.SlowMotionRatePrefab.txtInput.TextChanged += this.SlowMotionRateTxtInput_TextChanged1;
			CheckBoxPrefab aiPromptOptionActivePrefab = this.AiPromptOptionActivePrefab;
			aiPromptOptionActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(aiPromptOptionActivePrefab.ValueChanged, new Action<bool>(this.AiPromptOptionActivePrefabValueChanged));
			CheckBoxPrefab faceSwapOptionActivePrefab = this.FaceSwapOptionActivePrefab;
			faceSwapOptionActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(faceSwapOptionActivePrefab.ValueChanged, new Action<bool>(this.FaceSwapOptionActivePrefabValueChanged));
			CheckBoxPrefab wordPortraitOptionActivePrefab = this.WordPortraitOptionActivePrefab;
			wordPortraitOptionActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(wordPortraitOptionActivePrefab.ValueChanged, new Action<bool>(this.WordPortraitOptionActivePrefabValueChanged));
			CheckBoxPrefab aiMotionOptionActivePrefab = this.AiMotionOptionActivePrefab;
			aiMotionOptionActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(aiMotionOptionActivePrefab.ValueChanged, new Action<bool>(this.AiMotionOptionActivePrefabValueChanged));
			CheckBoxPrefab aiEffectOptionActivePrefab = this.AiEffectOptionActivePrefab;
			aiEffectOptionActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(aiEffectOptionActivePrefab.ValueChanged, new Action<bool>(this.AiEffectOptionActivePrefabValueChanged));
			CheckBoxPrefab aiBeautifierOptionActivePrefab = this.AiBeautifierOptionActivePrefab;
			aiBeautifierOptionActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(aiBeautifierOptionActivePrefab.ValueChanged, new Action<bool>(this.AiBeautifierOptionActivePrefabValueChanged));
			CheckBoxPrefab photoCheckPrefab = this.PhotoCheckPrefab;
			photoCheckPrefab.ValueChanged = (Action<bool>)Delegate.Combine(photoCheckPrefab.ValueChanged, new Action<bool>(this.PhotoCheckPrefabValueChanged));
			CheckBoxPrefab videoCheckPrefab = this.VideoCheckPrefab;
			videoCheckPrefab.ValueChanged = (Action<bool>)Delegate.Combine(videoCheckPrefab.ValueChanged, new Action<bool>(this.VideoCheckPrefabValueChanged));
			CheckBoxPrefab backgroundPreviewCheckPrefab = this.BackgroundPreviewCheckPrefab;
			backgroundPreviewCheckPrefab.ValueChanged = (Action<bool>)Delegate.Combine(backgroundPreviewCheckPrefab.ValueChanged, new Action<bool>(this.BackgroundPreviewCheckValueChanged));
			CheckBoxPrefab gifCheckPrefab = this.GifCheckPrefab;
			gifCheckPrefab.ValueChanged = (Action<bool>)Delegate.Combine(gifCheckPrefab.ValueChanged, new Action<bool>(this.GifCheckPrefabValueChanged));
			this.DesktopEditingCheckPrefab.CheckElement.Checked += this.DesktopEditingElement_CheckControl;
			CheckBoxPrefab desktopEditingCheckPrefab = this.DesktopEditingCheckPrefab;
			desktopEditingCheckPrefab.ValueChanged = (Action<bool>)Delegate.Combine(desktopEditingCheckPrefab.ValueChanged, new Action<bool>(this.DesktopEditingCheckPrefabValueChanged));
			CheckBoxPrefab paymentsActivePrefab = this.PaymentsActivePrefab;
			paymentsActivePrefab.ValueChanged = (Action<bool>)Delegate.Combine(paymentsActivePrefab.ValueChanged, new Action<bool>(this.PaymentActiveValueChanged));
		}

		// Token: 0x060005F6 RID: 1526 RVA: 0x0002A26D File Offset: 0x0002846D
		private void PaymentActiveValueChanged(bool paymentActive)
		{
			if (paymentActive)
			{
				this.GalleryCheckPrefab.SetValue(!paymentActive);
			}
			this.GalleryCheckFrame.Visibility = (paymentActive ? Visibility.Collapsed : Visibility.Visible);
		}

		// Token: 0x060005F7 RID: 1527 RVA: 0x0002A294 File Offset: 0x00028494
		private void SlowMotionRateTxtInput_TextChanged1(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(this.SlowMotionRatePrefab.txtInput.Text))
				{
					return;
				}
				float rate = float.Parse(this.SlowMotionRatePrefab.txtInput.Text);
				if (rate < 2f)
				{
					this.SlowMotionRatePrefab.txtInput.Text = "2";
				}
				else if (rate > 8f)
				{
					this.SlowMotionRatePrefab.txtInput.Text = "8";
				}
			}
			catch (Exception)
			{
				this.SlowMotionRatePrefab.txtInput.Text = "8";
			}
			this.ReorganizeUi();
		}

		// Token: 0x060005F8 RID: 1528 RVA: 0x0002A33C File Offset: 0x0002853C
		private void BackgroundPreviewCheckValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x060005F9 RID: 1529 RVA: 0x0002A344 File Offset: 0x00028544
		private void AiBeautifierOptionActivePrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x060005FA RID: 1530 RVA: 0x0002A34C File Offset: 0x0002854C
		private void SetAtLeastOneShootingOption(string preferedKey = null)
		{
			if (!KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("photo").Value && !KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("video").Value && !KingAIPhotoBoothPro.Class.Helper.Settings.GetValueBoolean("upload").Value)
			{
				if (preferedKey == "photo")
				{
					this.PhotoCheckPrefab.SetValue(true);
					return;
				}
				if (preferedKey == "video")
				{
					this.VideoCheckPrefab.SetValue(true);
					return;
				}
				if (preferedKey == "upload")
				{
					this.DesktopEditingCheckPrefab.SetValue(true);
					return;
				}
				this.PhotoCheckPrefab.SetValue(true);
			}
		}

		// Token: 0x060005FB RID: 1531 RVA: 0x0002A3F4 File Offset: 0x000285F4
		private void DesktopEditingCheckPrefabValueChanged(bool obj)
		{
			this.SetAtLeastOneShootingOption("upload");
			if (obj && DateTime.Now > base.LoadedTime.AddSeconds(5.0))
			{
				MessageBoxWindow.CreateWindow("About Desktop Editing", "When desktop editing mode is on, you can perform AI Face Swap or AI Prompt Theme operations with photos on your computer without taking a photo.\r\n\r\nWith this mode, you can use our software not only as a photo booth but also as a desktop software.\r\n\r\nWe recommend you to use this mode to produce creative images for your AI Face\u00a0Swap\u00a0Gallery", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToSharingPage), MessageBoxWindow.MessageBoxSize.Large, false);
			}
			this.ReorganizeUi();
		}

		// Token: 0x060005FC RID: 1532 RVA: 0x0002A462 File Offset: 0x00028662
		private void VideoCheckPrefabValueChanged(bool obj)
		{
			this.SetAtLeastOneShootingOption("video");
			this.ReorganizeUi();
		}

		// Token: 0x060005FD RID: 1533 RVA: 0x0002A475 File Offset: 0x00028675
		private void GifCheckPrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x060005FE RID: 1534 RVA: 0x0002A47D File Offset: 0x0002867D
		private void PhotoCheckPrefabValueChanged(bool obj)
		{
			this.SetAtLeastOneShootingOption("photo");
			if (!obj)
			{
				KingAIPhotoBoothPro.Class.Helper.Settings.SetValue("gif", false);
			}
			this.ReorganizeUi();
		}

		// Token: 0x060005FF RID: 1535 RVA: 0x0002A49E File Offset: 0x0002869E
		private void WordPortraitOptionActivePrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x06000600 RID: 1536 RVA: 0x0002A4A6 File Offset: 0x000286A6
		private void AiMotionOptionActivePrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x06000601 RID: 1537 RVA: 0x0002A4AE File Offset: 0x000286AE
		private void AiEffectOptionActivePrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x06000602 RID: 1538 RVA: 0x0002A4B6 File Offset: 0x000286B6
		private void FaceSwapOptionActivePrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x06000603 RID: 1539 RVA: 0x0002A4BE File Offset: 0x000286BE
		private void AiPromptOptionActivePrefabValueChanged(bool obj)
		{
			this.ReorganizeUi();
		}

		// Token: 0x06000604 RID: 1540 RVA: 0x0002A4C6 File Offset: 0x000286C6
		private void DesktopEditingElement_CheckControl(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x0002A4C8 File Offset: 0x000286C8
		private void RunOnStartupCheckElement_Unchecked(object sender, RoutedEventArgs e)
		{
			string runOnStartupFilePath = System.IO.Path.Combine(SessionData.ApplicationDataFolderPath, "data4.bin");
			if (File.Exists(runOnStartupFilePath))
			{
				File.Delete(runOnStartupFilePath);
			}
			File.WriteAllText(runOnStartupFilePath, EncryptString.Encrypt("false", "runonstartup"));
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x0002A508 File Offset: 0x00028708
		private void RunOnStartupCheckElement_Checked(object sender, RoutedEventArgs e)
		{
			string runOnStartupFilePath = System.IO.Path.Combine(SessionData.ApplicationDataFolderPath, "data4.bin");
			if (File.Exists(runOnStartupFilePath))
			{
				File.Delete(runOnStartupFilePath);
			}
			File.WriteAllText(runOnStartupFilePath, EncryptString.Encrypt("true", "runonstartup"));
		}

		// Token: 0x06000607 RID: 1543 RVA: 0x0002A548 File Offset: 0x00028748
		private void RealtimeDeliveryCheckElement_Checked(object sender, RoutedEventArgs e)
		{
			if (!SettingsPage.sharingPage.CheckRealtimeDeliveryAccess())
			{
				this.BackgroundDeliveryPrefab.SetValue(false);
				MessageBoxWindow.CreateWindow("First Configure Your Mail Settings", "When background delivery mode is on, AI operations are performed in the background and the result is sent as an e-mail. Thus, your visitors do not have to wait for the operation to finish. You can continue\u00a0your\u00a0shooting.\n\nDo you want to navigate to Sharing Settings page?", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Yes,
					MessageBoxWindow.ButtonType.No
				}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToSharingPage), MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			if (DateTime.Now > base.LoadedTime.AddSeconds(5.0))
			{
				MessageBoxWindow.CreateWindow("About Background Delivery", "When background delivery mode is on, AI operations are performed in the background and the result is sent as an e-mail. Thus, your visitors do not have to wait for the operation to finish. You can continue\u00a0your\u00a0shooting.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, new Action<MessageBoxWindow.MessageBoxReturn>(this.GoToSharingPage), MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x06000608 RID: 1544 RVA: 0x0002A5EC File Offset: 0x000287EC
		private void GoToSharingPage(MessageBoxWindow.MessageBoxReturn messageBoxReturn)
		{
			if (messageBoxReturn == MessageBoxWindow.MessageBoxReturn.Yes)
			{
				SettingsPage.SettingsPageInstance.SetSubPage(SettingsSubPageEnum.SharingPage);
			}
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x0002A5FD File Offset: 0x000287FD
		private void AIBackgroundToggleElement_Checked(object sender, RoutedEventArgs e)
		{
			this.GreenBoxPrefab.SetValue(false);
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x0002A60B File Offset: 0x0002880B
		private void InitializeLoadingGif()
		{
			this.CameraListPrefab.SetLoading(true);
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x0002A61C File Offset: 0x0002881C
		private void SlowMotionEnabled_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.SlowMotionSettingsGrid.ActualWidth > 0.0)
			{
				this.SlowMotionSettingsGrid.Height = this.gridMain.ActualWidth * 0.065;
			}
			this.SlowVideoEndFrame.Visibility = (this.SlowVideoStartFrame.Visibility = (this.TwoSliderGrid.Visibility = Visibility.Collapsed));
			int rowNum = Grid.GetRow(this.SlowMotionSettingsGrid);
			this.SlowMotionSettingsSection.IsTopBorderVisible = false;
			this.SlowMotionSettingsSection.Visibility = Visibility.Collapsed;
			this.SlowMotionSettingsSectionSmall.Visibility = Visibility.Visible;
			Grid.SetRowSpan(this.SlowMotionCheckFrame, 5);
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x0002A6C4 File Offset: 0x000288C4
		private void SlowMotionEnabled_Checked(object sender, RoutedEventArgs e)
		{
			if (this.SlowMotionSettingsGrid.ActualWidth > 0.0)
			{
				this.SlowMotionSettingsGrid.Height = this.SlowMotionSettingsGrid.ActualWidth * 0.47;
			}
			this.SlowVideoEndFrame.Visibility = (this.SlowVideoStartFrame.Visibility = (this.TwoSliderGrid.Visibility = Visibility.Visible));
			int rowNum = Grid.GetRow(this.SlowMotionSettingsGrid);
			this.SlowMotionSettingsSection.IsTopBorderVisible = true;
			this.SlowMotionSettingsSection.Visibility = Visibility.Visible;
			this.SlowMotionSettingsSectionSmall.Visibility = Visibility.Collapsed;
			Grid.SetRowSpan(this.SlowMotionCheckFrame, 1);
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x0002A76C File Offset: 0x0002896C
		private void SlowMotionEnd_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(this.SlowMotionEndTimePrefab.txtInput.Text) && (double)float.Parse(this.SlowMotionEndTimePrefab.txtInput.Text) != this.SliderForSlowMotion.HigherValue)
				{
					this.SliderForSlowMotion.HigherValue = (double)float.Parse(this.SlowMotionEndTimePrefab.txtInput.Text);
				}
			}
			catch (Exception ex)
			{
				new MessageBoxWindow("Error", ex.Message, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x0600060E RID: 1550 RVA: 0x0002A80C File Offset: 0x00028A0C
		private void SlowMotionStart_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(this.SlowMotionStartTimePrefab.txtInput.Text) && (double)float.Parse(this.SlowMotionStartTimePrefab.txtInput.Text) != this.SliderForSlowMotion.LowerValue)
				{
					this.SliderForSlowMotion.LowerValue = (double)float.Parse(this.SlowMotionStartTimePrefab.txtInput.Text);
				}
			}
			catch (Exception ex)
			{
				new MessageBoxWindow("Error", ex.Message, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x0600060F RID: 1551 RVA: 0x0002A8AC File Offset: 0x00028AAC
		private void Music_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.MusicSettingsGrid.ActualWidth > 0.0)
			{
				this.MusicSettingsGrid.Height = this.gridMain.ActualWidth * 0.065;
			}
			int rowNum = Grid.GetRow(this.MusicSettingsGrid);
			this.MusicSettingSection.IsTopBorderVisible = false;
			this.MusicBottomGrid.Visibility = Visibility.Collapsed;
			this.MusicSettingSection.Visibility = Visibility.Collapsed;
			this.MusicSettingSectionSmall.Visibility = Visibility.Visible;
			Grid.SetRowSpan(this.MusicSelectCheckFrame, 5);
		}

		// Token: 0x06000610 RID: 1552 RVA: 0x0002A938 File Offset: 0x00028B38
		private void Music_Checked(object sender, RoutedEventArgs e)
		{
			if (this.MusicSettingsGrid.ActualWidth > 0.0)
			{
				this.MusicSettingsGrid.Height = this.MusicSettingsGrid.ActualWidth * 0.26;
			}
			int rowNum = Grid.GetRow(this.MusicSettingsGrid);
			this.MusicSettingSection.IsTopBorderVisible = true;
			this.MusicBottomGrid.Visibility = Visibility.Visible;
			this.MusicSettingSection.Visibility = Visibility.Visible;
			this.MusicSettingSectionSmall.Visibility = Visibility.Collapsed;
			Grid.SetRowSpan(this.MusicSelectCheckFrame, 1);
		}

		// Token: 0x06000611 RID: 1553 RVA: 0x0002A9C4 File Offset: 0x00028BC4
		private void VideoTime_TextChanged(object sender, TextChangedEventArgs e)
		{
			try
			{
				if (!string.IsNullOrEmpty(this.VideoTimePrefab.txtInput.Text))
				{
					int time = Convert.ToInt32(this.VideoTimePrefab.txtInput.Text);
					if (time < 4)
					{
						time = 4;
						this.VideoTimePrefab.txtInput.Text = "4";
					}
					this.SliderForSlowMotion.Maximum = (double)time;
					this.UpdateEllipsePositions();
				}
			}
			catch (Exception ex)
			{
				new MessageBoxWindow("Error", ex.Message, new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x06000612 RID: 1554 RVA: 0x0002AA64 File Offset: 0x00028C64
		private void ConnectDSLR()
		{
			if (!MainSettingsPage.firstTimeLoaded)
			{
				CameraControlClass.CameraConnectLoadCompleted += this.CameraControlClass_CameraConnectLoadCompleted;
				MainSettingsPage.firstTimeLoaded = true;
			}
			if (!CameraControlClass.isConnectedCamera)
			{
				new Thread(delegate()
				{
					Thread.Sleep(2500);
					DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						CameraControlClass.ConnectCamera();
					}), DispatcherPriority.Normal, Array.Empty<object>());
				}).Start();
				return;
			}
			CameraControlClass.CameraCompleteManuelEvent();
		}

		// Token: 0x06000613 RID: 1555 RVA: 0x0002AAB4 File Offset: 0x00028CB4
		private void Mode_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
		{
			MainSettingsPage.<Mode_SelectionChangedAsync>d__142 <Mode_SelectionChangedAsync>d__;
			<Mode_SelectionChangedAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Mode_SelectionChangedAsync>d__.<>4__this = this;
			<Mode_SelectionChangedAsync>d__.e = e;
			<Mode_SelectionChangedAsync>d__.<>1__state = -1;
			<Mode_SelectionChangedAsync>d__.<>t__builder.Start<MainSettingsPage.<Mode_SelectionChangedAsync>d__142>(ref <Mode_SelectionChangedAsync>d__);
		}

		// Token: 0x06000614 RID: 1556 RVA: 0x0002AAF4 File Offset: 0x00028CF4
		private void ModeSettingsCheckBoxesControl(bool isCameraSelect)
		{
			if (!isCameraSelect)
			{
				MainSettingsPage.CameraType[] array;
				if (MainSettingsPage.SelectedMode != Mode.Glambot)
				{
					array = (MainSettingsPage.CameraType[])Enum.GetValues(typeof(MainSettingsPage.CameraType));
				}
				else
				{
					MainSettingsPage.CameraType[] array2 = new MainSettingsPage.CameraType[2];
					array2[0] = MainSettingsPage.CameraType.GoPro;
					array = array2;
					array2[1] = MainSettingsPage.CameraType.Webcam;
				}
				MainSettingsPage.CameraType[] cameraTypes = array;
				this.SetCameraTypleList(cameraTypes);
			}
		}

		// Token: 0x06000615 RID: 1557 RVA: 0x0002AB38 File Offset: 0x00028D38
		private void CameraType_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
		{
			MainSettingsPage.<CameraType_SelectionChangedAsync>d__145 <CameraType_SelectionChangedAsync>d__;
			<CameraType_SelectionChangedAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CameraType_SelectionChangedAsync>d__.<>4__this = this;
			<CameraType_SelectionChangedAsync>d__.e = e;
			<CameraType_SelectionChangedAsync>d__.<>1__state = -1;
			<CameraType_SelectionChangedAsync>d__.<>t__builder.Start<MainSettingsPage.<CameraType_SelectionChangedAsync>d__145>(ref <CameraType_SelectionChangedAsync>d__);
		}

		// Token: 0x06000616 RID: 1558 RVA: 0x0002AB78 File Offset: 0x00028D78
		private void CameraList_SelectionChangedAsync(object sender, SelectionChangedEventArgs e)
		{
			MainSettingsPage.<CameraList_SelectionChangedAsync>d__146 <CameraList_SelectionChangedAsync>d__;
			<CameraList_SelectionChangedAsync>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<CameraList_SelectionChangedAsync>d__.<>4__this = this;
			<CameraList_SelectionChangedAsync>d__.e = e;
			<CameraList_SelectionChangedAsync>d__.<>1__state = -1;
			<CameraList_SelectionChangedAsync>d__.<>t__builder.Start<MainSettingsPage.<CameraList_SelectionChangedAsync>d__146>(ref <CameraList_SelectionChangedAsync>d__);
		}

		// Token: 0x06000617 RID: 1559 RVA: 0x0002ABB8 File Offset: 0x00028DB8
		private void SetCameraTypleList(MainSettingsPage.CameraType[] cameraTypes)
		{
			object selectedItem = this.CameraTypeListPrefab.ComboNameBox.SelectedItem;
			string selectedCameraType = (selectedItem != null) ? selectedItem.ToString() : null;
			object selectedItem2 = this.CameraListPrefab.ComboNameBox.SelectedItem;
			string selectedCamera = (selectedItem2 != null) ? selectedItem2.ToString() : null;
			object selectedItem3 = this.RotationListPrefab.ComboNameBox.SelectedItem;
			string selectedRotation = (selectedItem3 != null) ? selectedItem3.ToString() : null;
			this.CameraTypeListPrefab.ComboNameBox.ItemsSource = null;
			List<string> cameraTypeList = new List<string>();
			foreach (MainSettingsPage.CameraType cameraType in cameraTypes)
			{
				cameraTypeList.Add(cameraType.ToString());
			}
			this.CameraTypeListPrefab.ComboNameBox.ItemsSource = cameraTypeList;
			if (cameraTypeList.Contains(selectedCameraType))
			{
				this.CameraTypeListPrefab.ComboNameBox.SelectedItem = selectedCameraType;
			}
			if (this.CameraListPrefab.ComboNameBox.Items.Contains(selectedCamera))
			{
				this.CameraListPrefab.ComboNameBox.SelectedItem = selectedCamera;
			}
			if (this.RotationListPrefab.ComboNameBox.Items.Contains(selectedRotation))
			{
				this.RotationListPrefab.ComboNameBox.SelectedItem = selectedRotation;
			}
			this.AutoSelectWebcam();
		}

		// Token: 0x06000618 RID: 1560 RVA: 0x0002ACE6 File Offset: 0x00028EE6
		private void VideoGifPhotoCheckFrameControl()
		{
			this.ModeSettingsCheckBoxesControl(true);
		}

		// Token: 0x06000619 RID: 1561 RVA: 0x0002ACF0 File Offset: 0x00028EF0
		private void GoProCameraControlClass_returnWifiConnected(bool isWifiConnected, bool isShowMessage)
		{
			MainSettingsPage.<GoProCameraControlClass_returnWifiConnected>d__149 <GoProCameraControlClass_returnWifiConnected>d__;
			<GoProCameraControlClass_returnWifiConnected>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<GoProCameraControlClass_returnWifiConnected>d__.isWifiConnected = isWifiConnected;
			<GoProCameraControlClass_returnWifiConnected>d__.isShowMessage = isShowMessage;
			<GoProCameraControlClass_returnWifiConnected>d__.<>1__state = -1;
			<GoProCameraControlClass_returnWifiConnected>d__.<>t__builder.Start<MainSettingsPage.<GoProCameraControlClass_returnWifiConnected>d__149>(ref <GoProCameraControlClass_returnWifiConnected>d__);
		}

		// Token: 0x0600061A RID: 1562 RVA: 0x0002AD2F File Offset: 0x00028F2F
		private bool isGoProBLEConnected()
		{
			return GoProCameraControlClass.mBLED != null && GoProCameraControlClass.mBLED.ConnectionStatus == 1;
		}

		// Token: 0x0600061B RID: 1563 RVA: 0x0002AD4C File Offset: 0x00028F4C
		private void GoProScanStart()
		{
			if (!GoProCameraControlClass.isScanning && !GoProCameraControlClass.isPairing && !GoProCameraControlClass.isConnectingBLE && !GoProCameraControlClass.isConnectingWifi && !GoProCameraControlClass.isBusy && !GoProCameraControlClass.isWifiError)
			{
				MainSettingsPage.StatusOutput("baglanti islemi basladi.", MainSettingsPage.StatusOutputColor.Success);
				this.CameraListPrefab.SetLoading(true);
				GoProCameraControlClass.GoProScanAsync();
				return;
			}
			if (GoProCameraControlClass.isWifiError)
			{
				return;
			}
			MainSettingsPage.StatusOutput("Wait for the process to finish before click the button", MainSettingsPage.StatusOutputColor.Warning);
		}

		// Token: 0x0600061C RID: 1564 RVA: 0x0002ADB4 File Offset: 0x00028FB4
		private void RetryGoProScanClick(object sender, RoutedEventArgs e)
		{
			this.isConnectingGoProJson = false;
			this.CameraListPrefab.ComboNameBox.Items.Clear();
			this.GoProScanStart();
		}

		// Token: 0x0600061D RID: 1565 RVA: 0x0002ADD8 File Offset: 0x00028FD8
		private void GreenBox_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.gridChromaKeySettings.ActualWidth > 0.0)
			{
				this.gridChromaKeySettings.Height = this.gridChromaKeySettings.ActualWidth * 0.06;
			}
			this.GreenBoxTreshOldPrefab.txtInput.IsEnabled = false;
			this.GridForChromaKey.Visibility = Visibility.Collapsed;
			this.GreenBoxTreshOld.Visibility = Visibility.Collapsed;
			this.ChromaKeySection.IsTopBorderVisible = false;
			this.ChromaKeySection.Visibility = Visibility.Collapsed;
			this.ChromaKeySectionSmall.Visibility = Visibility.Visible;
			this.ChromaKeyContentArea.Visibility = Visibility.Collapsed;
			Grid.SetRowSpan(this.GreenBoxEnabled, 5);
		}

		// Token: 0x0600061E RID: 1566 RVA: 0x0002AE80 File Offset: 0x00029080
		private void GreenBox_Checked(object sender, RoutedEventArgs e)
		{
			if (this.gridChromaKeySettings.ActualWidth > 0.0)
			{
				this.gridChromaKeySettings.Height = this.gridChromaKeySettings.ActualWidth * 0.45;
			}
			this.GreenBoxTreshOldPrefab.txtInput.IsEnabled = true;
			this.GridForChromaKey.Visibility = Visibility.Visible;
			this.GreenBoxTreshOld.Visibility = Visibility.Visible;
			this.ChromaKeySection.IsTopBorderVisible = true;
			this.ChromaKeySection.Visibility = Visibility.Visible;
			this.ChromaKeySectionSmall.Visibility = Visibility.Collapsed;
			this.ChromaKeyContentArea.Visibility = Visibility.Visible;
			this.aIBackgroundPrefab.SetValue(false);
			Grid.SetRowSpan(this.GreenBoxEnabled, 1);
		}

		// Token: 0x0600061F RID: 1567 RVA: 0x0002AF34 File Offset: 0x00029134
		private void TxtInput_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(this.GreenBoxTreshOldPrefab.txtInput.Text))
			{
				this.GreenBoxTreshOldPrefab.txtInput.Text = "0";
				this.GreenBoxTreshOldPrefab.txtInput.CaretIndex = 1;
				return;
			}
			if (ExtensionMethod.IsTextAllowed(this.GreenBoxTreshOldPrefab.txtInput.Text))
			{
				this.GreenBoxTreshOldPrefab.txtInput.Text = Math.Min(Convert.ToInt32(this.GreenBoxTreshOldPrefab.txtInput.Text), 255).ToString();
				if (this.GreenBoxTreshOldPrefab.txtInput.CaretIndex == 0)
				{
					this.GreenBoxTreshOldPrefab.txtInput.CaretIndex = this.GreenBoxTreshOldPrefab.txtInput.Text.Length;
				}
				if (!string.IsNullOrEmpty(this.lastPhotoPath))
				{
					new Thread(delegate()
					{
						Thread.Sleep(250);
						try
						{
							DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
							{
								string resultFilePath = System.IO.Path.Combine(CameraControlClass.FolderForTemp, System.IO.Path.GetFileNameWithoutExtension(this.lastPhotoPath) + DSLR.selectedFileExtension);
								System.Drawing.Image resultImage = ExtensionMethod.Render(this.lastPhotoPath);
								this.PhotoBox.Source = ExtensionMethod.ConvertBitmap2BitmapSource((Bitmap)resultImage);
								this.GreenBoxTreshOldPrefab.txtInput.ScrollToEnd();
							}), DispatcherPriority.Normal, Array.Empty<object>());
							op.Wait();
						}
						catch (Exception ex)
						{
							new MessageBoxWindow("Camera Start Error", ex.ToString(), new List<MessageBoxWindow.ButtonType>
							{
								MessageBoxWindow.ButtonType.Continue
							}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						}
					}).Start();
					return;
				}
			}
			else
			{
				this.GreenBoxTreshOldPrefab.txtInput.Text = ExtensionMethod.ReChanged(e, this.GreenBoxTreshOldPrefab.txtInput);
				this.GreenBoxTreshOldPrefab.txtInput.CaretIndex = this.GreenBoxTreshOldPrefab.txtInput.Text.Length;
			}
		}

		// Token: 0x06000620 RID: 1568 RVA: 0x0002B074 File Offset: 0x00029274
		private void ColorElement_colorChanged()
		{
			if (!string.IsNullOrEmpty(this.lastPhotoPath))
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					string resultFilePath = System.IO.Path.Combine(CameraControlClass.FolderForTemp, System.IO.Path.GetFileNameWithoutExtension(this.lastPhotoPath) + DSLR.selectedFileExtension);
					System.Drawing.Image resultImage = ExtensionMethod.Render(this.lastPhotoPath);
					this.PhotoBox.Source = ExtensionMethod.ConvertBitmap2BitmapSource((Bitmap)resultImage);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
			}
		}

		// Token: 0x06000621 RID: 1569 RVA: 0x0002B0B4 File Offset: 0x000292B4
		private void PhotoCapture(string filepath)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				string resultFilePath = System.IO.Path.Combine(CameraControlClass.FolderForTemp, System.IO.Path.GetFileNameWithoutExtension(filepath) + DSLR.selectedFileExtension);
				System.Drawing.Image resultImage = ExtensionMethod.Render(filepath);
				this.PhotoBox.Source = ExtensionMethod.ConvertBitmap2BitmapSource((Bitmap)resultImage);
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			this.lastPhotoPath = filepath;
		}

		// Token: 0x06000622 RID: 1570 RVA: 0x0002B107 File Offset: 0x00029307
		private void PhotoFailed(string filepath)
		{
			new MessageBoxWindow("Photo Error", "Photo not Captured!", new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Continue
			}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
		}

		// Token: 0x06000623 RID: 1571 RVA: 0x0002B12C File Offset: 0x0002932C
		private void CameraControlClass_CameraConnectLoadCompleted()
		{
			if (CameraControlClass.DeviceManager != null)
			{
				CameraControlClass.DeviceManager.CameraConnected += this.DeviceManager_CameraConnected;
				CameraControlClass.DeviceManager.CameraDisconnected += this.DeviceManager_CameraDisconnected;
				CameraControlClass.PhotoCapturedEvent = (Action<string>)Delegate.Combine(CameraControlClass.PhotoCapturedEvent, new Action<string>(this.PhotoCapture));
				CameraControlClass.PhotoCapturedFailedEvent = (Action<string>)Delegate.Combine(CameraControlClass.PhotoCapturedFailedEvent, new Action<string>(this.PhotoFailed));
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					CameraControlClass.SetCamera(false, CameraControlClass.selectedCameraDeviceName, false);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
			}
			this.TestPhotoButton.IsEnabled = true;
			this.UpdateCameraListBox();
		}

		// Token: 0x06000624 RID: 1572 RVA: 0x0002B1FC File Offset: 0x000293FC
		public MainSettingsPage.IsFoundWifiInJsonReturnType IsFoundWifiInJson()
		{
			MainSettingsPage.<>c__DisplayClass161_0 CS$<>8__locals1 = new MainSettingsPage.<>c__DisplayClass161_0();
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.ssid = GoProCameraControlClass.GetCurrentWifiSSID();
			if (CS$<>8__locals1.ssid.Contains("Error"))
			{
				return MainSettingsPage.IsFoundWifiInJsonReturnType.WifiError;
			}
			if (CS$<>8__locals1.ssid == null)
			{
				return MainSettingsPage.IsFoundWifiInJsonReturnType.False;
			}
			if (!File.Exists(GoProCameraControlClass.connectedGoProStringsFilePath))
			{
				return MainSettingsPage.IsFoundWifiInJsonReturnType.False;
			}
			List<ConnectedGoPro> connectedGoPros = JsonConvert.DeserializeObject<List<ConnectedGoPro>>(File.ReadAllText(GoProCameraControlClass.connectedGoProStringsFilePath));
			CS$<>8__locals1.selectedGoPro = null;
			int i;
			int j;
			for (i = 0; i < CS$<>8__locals1.ssid.Count; i = j + 1)
			{
				CS$<>8__locals1.selectedGoPro = (from p in connectedGoPros
				where p.wifissid == CS$<>8__locals1.ssid[i]
				select p).FirstOrDefault<ConnectedGoPro>();
				if (CS$<>8__locals1.selectedGoPro != null)
				{
					GoProCameraControlClass.currentGoPro = CS$<>8__locals1.selectedGoPro;
					this.isConnectingGoProJson = true;
					break;
				}
				j = i;
			}
			if (CS$<>8__locals1.selectedGoPro == null)
			{
				return MainSettingsPage.IsFoundWifiInJsonReturnType.False;
			}
			base.Dispatcher.Invoke(delegate()
			{
				CS$<>8__locals1.<>4__this.CameraListPrefab.ComboNameBox.Items.Add(CS$<>8__locals1.selectedGoPro.name);
				CS$<>8__locals1.<>4__this.CameraListPrefab.ComboNameBox.SelectedIndex = 0;
				CS$<>8__locals1.<>4__this.CameraListPrefab.SetLoading(false);
				Mouse.OverrideCursor = null;
			});
			ActivationKingWindow.SettingsPage.HideOrOpenButtons();
			MainSettingsPage.StatusOutput(CS$<>8__locals1.selectedGoPro.wifissid + " connected.", MainSettingsPage.StatusOutputColor.Success);
			this.GoProCameraControlClass_returnWifiConnected(true, true);
			return MainSettingsPage.IsFoundWifiInJsonReturnType.True;
		}

		// Token: 0x06000625 RID: 1573 RVA: 0x0002B33C File Offset: 0x0002953C
		public void LastUpdateCameraList()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				MainSettingsPage.<<LastUpdateCameraList>b__162_0>d <<LastUpdateCameraList>b__162_0>d;
				<<LastUpdateCameraList>b__162_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
				<<LastUpdateCameraList>b__162_0>d.<>4__this = this;
				<<LastUpdateCameraList>b__162_0>d.<>1__state = -1;
				<<LastUpdateCameraList>b__162_0>d.<>t__builder.Start<MainSettingsPage.<<LastUpdateCameraList>b__162_0>d>(ref <<LastUpdateCameraList>b__162_0>d);
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x06000626 RID: 1574 RVA: 0x0002B370 File Offset: 0x00029570
		public void UpdateCameraListBox()
		{
			if (this.CameraTypeListPrefab.ComboNameBox.Items.Count < 1 || this.isCameraListUpdating)
			{
				return;
			}
			WebcamControlClass.WebcamClose();
			this.isCameraListUpdating = true;
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
			{
				DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					MainSettingsPage.<<UpdateCameraListBox>b__163_0>d <<UpdateCameraListBox>b__163_0>d;
					<<UpdateCameraListBox>b__163_0>d.<>t__builder = AsyncVoidMethodBuilder.Create();
					<<UpdateCameraListBox>b__163_0>d.<>4__this = this;
					<<UpdateCameraListBox>b__163_0>d.<>1__state = -1;
					<<UpdateCameraListBox>b__163_0>d.<>t__builder.Start<MainSettingsPage.<<UpdateCameraListBox>b__163_0>d>(ref <<UpdateCameraListBox>b__163_0>d);
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op.Wait();
			}
			else
			{
				DispatcherOperation op2 = base.Dispatcher.BeginInvoke(new Action(delegate()
				{
					this.CameraListPrefab.ComboNameBox.Items.Clear();
					for (int i = 0; i < GoProCameraControlClass.DeviceList.Count; i++)
					{
						if (!this.CameraListPrefab.ComboNameBox.Items.Contains(GoProCameraControlClass.DeviceList[i]))
						{
							this.CameraListPrefab.ComboNameBox.Items.Add(GoProCameraControlClass.DeviceList[i]);
						}
					}
					this.isSelecting = false;
					this.CameraListPrefab.SetLoading(false);
					Mouse.OverrideCursor = null;
					this.UpdateRotationComboBox();
				}), DispatcherPriority.Normal, Array.Empty<object>());
				op2.Wait();
			}
			this.isCameraListUpdating = false;
		}

		// Token: 0x06000627 RID: 1575 RVA: 0x0002B408 File Offset: 0x00029608
		private void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
		{
			CameraProperty property = ServiceProvider.Settings.CameraProperties.Get(cameraDevice);
			cameraDevice.DisplayName = property.DeviceName;
			cameraDevice.AttachedPhotoSession = ServiceProvider.Settings.GetSession(property.PhotoSessionName);
			CameraControlClass.isConnectedCamera = true;
			global::Debug.Log("DeviceManager_CameraConnected", cameraDevice.DisplayName, "DeviceManager_CameraConnected", 1989);
			this.UpdateCameraListBox();
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x0002B470 File Offset: 0x00029670
		private void DeviceManager_CameraDisconnected(ICameraDevice cameraDevice)
		{
			string deviceName = cameraDevice.DeviceName + (cameraDevice.IsBusy ? "(unavailable)" : "");
			bool found = (from x in CameraControlClass.DeviceList
			where x == deviceName
			select x).Count<string>() > 0;
			if (found)
			{
				CameraControlClass.DeviceList.Remove(deviceName);
			}
			this.UpdateCameraListBox();
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x0002B4E4 File Offset: 0x000296E4
		private void UpdateRotationComboBox()
		{
			this.RotationListPrefab.txtDescription = "Possible camera rotations are listed";
			this.RotationListPrefab.ComboNameBox.Items.Clear();
			for (int i = 0; i < this.RotationList.Count; i++)
			{
				this.RotationListPrefab.ComboNameBox.Items.Add(this.RotationList[i]);
			}
			this.RotationListPrefab.Load();
			if (!this.RotationListPrefab.IsEnabled && this.RotationListPrefab.ComboNameBox.Items.Count > 0 && this.RotationListPrefab.ComboNameBox.SelectedIndex == -1)
			{
				this.RotationListPrefab.ComboNameBox.SelectedIndex = 0;
			}
			this.RotationListPrefab.IsEnabled = true;
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x0002B5B0 File Offset: 0x000297B0
		private void WebcamControlClass_WebcamDisconnected(List<string> DisconnectedDeviceList)
		{
			MainSettingsPage.<>c__DisplayClass168_0 CS$<>8__locals1 = new MainSettingsPage.<>c__DisplayClass168_0();
			CS$<>8__locals1.DisconnectedDeviceList = DisconnectedDeviceList;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.i = 0;
			while (CS$<>8__locals1.i < CS$<>8__locals1.DisconnectedDeviceList.Count)
			{
				MainSettingsPage.<>c__DisplayClass168_1 CS$<>8__locals2 = new MainSettingsPage.<>c__DisplayClass168_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				MainSettingsPage.<>c__DisplayClass168_1 CS$<>8__locals3 = CS$<>8__locals2;
				IEnumerable<string> source = this.CameraListPrefab.ComboNameBox.Items.Cast<string>();
				Func<string, bool> predicate;
				if ((predicate = CS$<>8__locals2.CS$<>8__locals1.<>9__0) == null)
				{
					predicate = (CS$<>8__locals2.CS$<>8__locals1.<>9__0 = ((string x) => x == CS$<>8__locals2.CS$<>8__locals1.DisconnectedDeviceList[CS$<>8__locals2.CS$<>8__locals1.i]));
				}
				CS$<>8__locals3.device = source.Where(predicate).FirstOrDefault<string>();
				if (CS$<>8__locals2.device != null)
				{
					base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.CameraListPrefab.ComboNameBox.Items.Remove(CS$<>8__locals2.device);
					}), DispatcherPriority.Normal, Array.Empty<object>()).Wait();
				}
				int i = CS$<>8__locals1.i;
				CS$<>8__locals1.i = i + 1;
			}
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x0002B690 File Offset: 0x00029890
		private void WebcamControlClass_WebcamConnected(List<string> ConnectedDeviceList)
		{
			MainSettingsPage.<>c__DisplayClass169_0 CS$<>8__locals1 = new MainSettingsPage.<>c__DisplayClass169_0();
			CS$<>8__locals1.ConnectedDeviceList = ConnectedDeviceList;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.i = 0;
			while (CS$<>8__locals1.i < CS$<>8__locals1.ConnectedDeviceList.Count)
			{
				MainSettingsPage.<>c__DisplayClass169_1 CS$<>8__locals2 = new MainSettingsPage.<>c__DisplayClass169_1();
				CS$<>8__locals2.CS$<>8__locals1 = CS$<>8__locals1;
				MainSettingsPage.<>c__DisplayClass169_1 CS$<>8__locals3 = CS$<>8__locals2;
				IEnumerable<string> source = this.CameraListPrefab.ComboNameBox.Items.Cast<string>();
				Func<string, bool> predicate;
				if ((predicate = CS$<>8__locals2.CS$<>8__locals1.<>9__0) == null)
				{
					predicate = (CS$<>8__locals2.CS$<>8__locals1.<>9__0 = ((string x) => x == CS$<>8__locals2.CS$<>8__locals1.ConnectedDeviceList[CS$<>8__locals2.CS$<>8__locals1.i]));
				}
				CS$<>8__locals3.device = source.Where(predicate).FirstOrDefault<string>();
				if (CS$<>8__locals2.device == null)
				{
					base.Dispatcher.BeginInvoke(new Action(delegate()
					{
						CS$<>8__locals2.CS$<>8__locals1.<>4__this.CameraListPrefab.ComboNameBox.Items.Add(CS$<>8__locals2.device);
					}), DispatcherPriority.Normal, Array.Empty<object>()).Wait();
				}
				int i = CS$<>8__locals1.i;
				CS$<>8__locals1.i = i + 1;
			}
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x0002B770 File Offset: 0x00029970
		private void WebcamControlClass_PhotoCaptured(string filePath)
		{
			System.Windows.Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				string resultFilePath = System.IO.Path.Combine(CameraControlClass.FolderForTemp, System.IO.Path.GetFileNameWithoutExtension(filePath) + ".png");
				System.Drawing.Image resultImage = ExtensionMethod.Render(filePath);
				this.PhotoBox.Source = ExtensionMethod.ConvertBitmap2BitmapSource((Bitmap)resultImage);
			}), Array.Empty<object>()).Wait();
			this.lastPhotoPath = filePath;
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x0002B7C3 File Offset: 0x000299C3
		private void TestPhotoButton_Click(object sender, RoutedEventArgs e)
		{
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.GoPro)
			{
				GoProCameraControlClass.GoProShutterOn();
				return;
			}
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.DSLR)
			{
				CameraControlClass.TakePicture();
				return;
			}
			if (MainSettingsPage.selectedCameraType == MainSettingsPage.CameraType.Webcam)
			{
				WebcamControlClass.TakePicture(false);
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x0002B7EE File Offset: 0x000299EE
		public override string GetTitle()
		{
			return "Main Settings";
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x0002B7F5 File Offset: 0x000299F5
		public override string GetSubTitle()
		{
			return "Please define your event details.";
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x0002B7FC File Offset: 0x000299FC
		private void PhotoBox_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (string.IsNullOrEmpty(this.lastPhotoPath))
			{
				return;
			}
			System.Windows.Point wPoint = e.GetPosition(this.PhotoBox);
			System.Drawing.Point dPoint = new System.Drawing.Point((int)wPoint.X, (int)wPoint.Y);
			System.Drawing.Color changeColor = CameraControlClass.GetPositionColor(dPoint, (Bitmap)System.Drawing.Image.FromFile(this.lastPhotoPath), new System.Drawing.Size((int)this.PhotoBox.ActualWidth, (int)this.PhotoBox.ActualHeight));
			System.Windows.Media.Color wmColor = default(System.Windows.Media.Color);
			wmColor.A = changeColor.A;
			wmColor.R = changeColor.R;
			wmColor.G = changeColor.G;
			wmColor.B = changeColor.B;
			this.colorGreenScreenPrefab.colorPickElement.SelectedColor = wmColor;
			this.ColorElement_colorChanged();
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x0002B8C8 File Offset: 0x00029AC8
		public static void StatusOutput(string status, MainSettingsPage.StatusOutputColor statusColor)
		{
			System.Windows.Application.Current.Dispatcher.Invoke(delegate()
			{
				MainSettingsPage.statusTextBlock.Text = status;
				if (statusColor == MainSettingsPage.StatusOutputColor.Success)
				{
					MainSettingsPage.SetStatusTextColors("#F2FAF6", "#47B881", 0);
					return;
				}
				if (statusColor == MainSettingsPage.StatusOutputColor.Warning)
				{
					MainSettingsPage.SetStatusTextColors("#FFF9EE", "#FFAD0D", 1);
					return;
				}
				if (statusColor == MainSettingsPage.StatusOutputColor.Error)
				{
					MainSettingsPage.SetStatusTextColors("#FEF2F2", "#F64C4C", 2);
				}
			});
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0002B904 File Offset: 0x00029B04
		private static void SetStatusTextColors(string backgroundColorHex, string borderColorHex, int statusType)
		{
			System.Windows.Media.Color backgroundColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(backgroundColorHex);
			System.Windows.Media.Color borderColor = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(borderColorHex);
			SolidColorBrush newBorderBrush = new SolidColorBrush(borderColor);
			SolidColorBrush newBorderBackgroundColor = new SolidColorBrush(backgroundColor);
			if (statusType == 0)
			{
				MainSettingsPage.succesIconInstance.Visibility = Visibility.Visible;
				MainSettingsPage.warningIconInstance.Visibility = Visibility.Collapsed;
				MainSettingsPage.errorIconInstance.Visibility = Visibility.Collapsed;
			}
			else if (statusType == 1)
			{
				MainSettingsPage.succesIconInstance.Visibility = Visibility.Collapsed;
				MainSettingsPage.warningIconInstance.Visibility = Visibility.Visible;
				MainSettingsPage.errorIconInstance.Visibility = Visibility.Collapsed;
			}
			else if (statusType == 2)
			{
				MainSettingsPage.succesIconInstance.Visibility = Visibility.Collapsed;
				MainSettingsPage.warningIconInstance.Visibility = Visibility.Collapsed;
				MainSettingsPage.errorIconInstance.Visibility = Visibility.Visible;
			}
			MainSettingsPage.txtStatusBorderInstance.Background = newBorderBackgroundColor;
			MainSettingsPage.txtStatusBorderInstance.BorderBrush = newBorderBrush;
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x0002B9BF File Offset: 0x00029BBF
		private void RetryGoProWifiClick(object sender, RoutedEventArgs e)
		{
			this.isConnectingGoProJson = false;
			GoProCameraControlClass.GoProRetryWifiConnectAsync();
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x0002B9D0 File Offset: 0x00029BD0
		private void RangeSlider_HigherValueChanged(object sender, RoutedEventArgs e)
		{
			if (this.SliderForSlowMotion.HigherValue >= this.SliderForSlowMotion.Maximum)
			{
				return;
			}
			double newValue = Math.Round(this.SliderForSlowMotion.HigherValue / 0.25) * 0.25;
			this.SliderForSlowMotion.HigherValue = newValue;
			this.UpdateEllipsePositions();
			if (this.SlowMotionEndTimePrefab != null)
			{
				this.SlowMotionEndTimePrefab.txtInput.Text = this.SliderForSlowMotion.HigherValue.ToString();
				this.HigherTextBlock.Text = this.SliderForSlowMotion.HigherValue.ToString();
			}
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x0002BA78 File Offset: 0x00029C78
		private void RangeSlider_LowerValueChanged(object sender, RoutedEventArgs e)
		{
			if (this.SliderForSlowMotion.LowerValue <= 0.0)
			{
				return;
			}
			double newValue = Math.Round(this.SliderForSlowMotion.LowerValue / 0.25) * 0.25;
			this.SliderForSlowMotion.LowerValue = newValue;
			this.UpdateEllipsePositions();
			if (this.SlowMotionEndTimePrefab != null)
			{
				this.SlowMotionStartTimePrefab.txtInput.Text = this.SliderForSlowMotion.LowerValue.ToString();
				this.LowerTextBlock.Text = this.SliderForSlowMotion.LowerValue.ToString();
			}
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x0002BB1C File Offset: 0x00029D1C
		private void UpdateEllipsePositions()
		{
			if (this.LowerEllipseTransform == null || this.HigherEllipseTransform == null)
			{
				return;
			}
			double trackWidth = this.TwoSliderGrid.ActualWidth;
			double lowerRatio = (this.SliderForSlowMotion.LowerValue - this.SliderForSlowMotion.Minimum) / (this.SliderForSlowMotion.Maximum - this.SliderForSlowMotion.Minimum);
			double higherRatio = (this.SliderForSlowMotion.HigherValue - this.SliderForSlowMotion.Minimum) / (this.SliderForSlowMotion.Maximum - this.SliderForSlowMotion.Minimum);
			double val = this.SliderForSlowMotion.LowerValue / this.SliderForSlowMotion.Maximum;
			double easedVal = Math.Pow(val * 10.0, 3.0);
			double val2 = 1.0 - this.SliderForSlowMotion.HigherValue / this.SliderForSlowMotion.Maximum;
			double easedVal2 = Math.Pow(val2 * 10.0, 3.0);
			this.LowerEllipseTransform.X = lowerRatio * trackWidth - trackWidth / 2.0 - easedVal / 37.0;
			this.HigherEllipseTransform.X = higherRatio * trackWidth - trackWidth / 2.0 + easedVal2 / 37.0;
			this.HigherTextBlockTransform.X = this.HigherEllipseTransform.X;
			this.HigherTextBlockTransform.Y = this.HigherEllipseTransform.Y - 30.0;
			this.LowerTextBlockTransform.X = this.LowerEllipseTransform.X;
			this.LowerTextBlockTransform.Y = this.LowerEllipseTransform.Y - 30.0;
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x0002BCD2 File Offset: 0x00029ED2
		private void LowerThumbEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.isLowerEllipseDragging = true;
			this.LowerThumbEllipse.CaptureMouse();
			this.initialMousePosition = e.GetPosition(this.TwoSliderGrid);
			this.initialLowerValue = this.SliderForSlowMotion.LowerValue;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x0002BD0A File Offset: 0x00029F0A
		private void HigherThumbEllipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.isHigherEllipseDragging = true;
			this.HigherThumbEllipse.CaptureMouse();
			this.initialMousePosition = e.GetPosition(this.TwoSliderGrid);
			this.initialHigherValue = this.SliderForSlowMotion.HigherValue;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x0002BD44 File Offset: 0x00029F44
		private void Ellipse_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (this.isLowerEllipseDragging || this.isHigherEllipseDragging)
			{
				System.Windows.Point currentPosition = e.GetPosition(this.TwoSliderGrid);
				double changeInX = currentPosition.X - this.initialMousePosition.X;
				double valueChange = changeInX / this.TwoSliderGrid.ActualWidth * (this.SliderForSlowMotion.Maximum - this.SliderForSlowMotion.Minimum);
				if (this.isLowerEllipseDragging)
				{
					double newValue = this.initialLowerValue + valueChange;
					if (newValue < this.SliderForSlowMotion.HigherValue && newValue >= this.SliderForSlowMotion.Minimum)
					{
						this.SliderForSlowMotion.LowerValue = newValue;
						if (currentPosition.X < 0.0)
						{
							this.LowerEllipseTransform.X = 0.0;
							return;
						}
						if (currentPosition.X > this.HigherEllipseTransform.X)
						{
							this.LowerEllipseTransform.X = this.HigherEllipseTransform.X;
							return;
						}
						this.LowerEllipseTransform.X = currentPosition.X;
						return;
					}
				}
				else if (this.isHigherEllipseDragging)
				{
					double newValue2 = this.initialHigherValue + valueChange;
					if (newValue2 > this.SliderForSlowMotion.LowerValue && newValue2 <= this.SliderForSlowMotion.Maximum)
					{
						this.SliderForSlowMotion.HigherValue = newValue2;
						if (currentPosition.X > this.TwoSliderGrid.ActualWidth)
						{
							this.HigherEllipseTransform.X = this.TwoSliderGrid.ActualWidth;
							return;
						}
						if (currentPosition.X < this.LowerEllipseTransform.X)
						{
							this.HigherEllipseTransform.X = this.LowerEllipseTransform.X;
							return;
						}
						this.HigherEllipseTransform.X = currentPosition.X;
					}
				}
			}
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x0002BEFD File Offset: 0x0002A0FD
		private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (this.isLowerEllipseDragging)
			{
				this.isLowerEllipseDragging = false;
				this.LowerThumbEllipse.ReleaseMouseCapture();
				return;
			}
			if (this.isHigherEllipseDragging)
			{
				this.isHigherEllipseDragging = false;
				this.HigherThumbEllipse.ReleaseMouseCapture();
			}
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x0002BF34 File Offset: 0x0002A134
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (this.isLowerEllipseDragging)
			{
				System.Windows.Point currentPosition = e.GetPosition(this.TwoSliderGrid);
				double newValue = this.SliderForSlowMotion.Minimum + currentPosition.X / this.TwoSliderGrid.ActualWidth * (this.SliderForSlowMotion.Maximum - this.SliderForSlowMotion.Minimum);
				if (newValue < this.SliderForSlowMotion.HigherValue && newValue >= this.SliderForSlowMotion.Minimum)
				{
					this.SliderForSlowMotion.LowerValue = newValue;
					return;
				}
			}
			else if (this.isHigherEllipseDragging)
			{
				System.Windows.Point currentPosition2 = e.GetPosition(this.TwoSliderGrid);
				double newValue2 = this.SliderForSlowMotion.Minimum + currentPosition2.X / this.TwoSliderGrid.ActualWidth * (this.SliderForSlowMotion.Maximum - this.SliderForSlowMotion.Minimum);
				if (newValue2 > this.SliderForSlowMotion.LowerValue && newValue2 <= this.SliderForSlowMotion.Maximum)
				{
					this.SliderForSlowMotion.HigherValue = newValue2;
				}
			}
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x0002C039 File Offset: 0x0002A239
		protected override void OnMouseUp(MouseButtonEventArgs e)
		{
			base.OnMouseUp(e);
			if (this.isLowerEllipseDragging)
			{
				this.isLowerEllipseDragging = false;
				this.LowerThumbEllipse.ReleaseMouseCapture();
				return;
			}
			if (this.isHigherEllipseDragging)
			{
				this.isHigherEllipseDragging = false;
				this.HigherThumbEllipse.ReleaseMouseCapture();
			}
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0002C078 File Offset: 0x0002A278
		private void SelectOutputSharingFolderButton_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
			{
				folderDialog.Description = "Select a folder to use";
				folderDialog.ShowNewFolderButton = true;
				if (folderDialog.ShowDialog() == DialogResult.OK)
				{
					if (Directory.Exists(folderDialog.SelectedPath))
					{
						this.SelectedFolderPathTextBox.Text = folderDialog.SelectedPath;
						KingAIPhotoBoothPro.Class.Helper.Settings.SetValue("outputfolder", folderDialog.SelectedPath, true);
					}
					else
					{
						MessageBoxWindow.CreateWindow("Directory Selection Failed", "Directory does not exist", new List<MessageBoxWindow.ButtonType>
						{
							MessageBoxWindow.ButtonType.Continue
						}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						this.LoadSavedFolderPath();
					}
				}
			}
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x0002C118 File Offset: 0x0002A318
		private void LoadSavedFolderPath()
		{
			string savedFolderPath = KingAIPhotoBoothPro.Class.Helper.Settings.GetValueString("outputfolder");
			if (!string.IsNullOrEmpty(savedFolderPath))
			{
				this.SelectedFolderPathTextBox.Text = savedFolderPath;
				return;
			}
			this.SelectedFolderPathTextBox.Text = "No folder selected";
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x0002C155 File Offset: 0x0002A355
		private void SettingsSubPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ReOrganizeSections();
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x0002C15D File Offset: 0x0002A35D
		private void SelectFolderClearButton_Click(object sender, RoutedEventArgs e)
		{
			KingAIPhotoBoothPro.Class.Helper.Settings.SetValue("outputfolder", "", true);
			this.LoadSavedFolderPath();
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x0002C175 File Offset: 0x0002A375
		private void btnCameraSettings_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.Instance.ShowCloseCameraSettings();
		}

		// Token: 0x04000602 RID: 1538
		private static System.Windows.Input.Cursor ColorDropperCursor;

		// Token: 0x04000603 RID: 1539
		private bool firstWork = true;

		// Token: 0x04000604 RID: 1540
		private bool isConnectingGoProJson;

		// Token: 0x04000605 RID: 1541
		private List<string> RotationList = new List<string>
		{
			"0 degree",
			"90 degree",
			"180 degree",
			"270 degree"
		};

		// Token: 0x04000606 RID: 1542
		private string lastPhotoPath;

		// Token: 0x04000607 RID: 1543
		private string tryConnectingGopro = "";

		// Token: 0x04000608 RID: 1544
		private bool changeMod;

		// Token: 0x04000609 RID: 1545
		public static MainSettingsPage Instance;

		// Token: 0x0400060B RID: 1547
		public static MainSettingsPage.CameraType selectedCameraType;

		// Token: 0x0400060C RID: 1548
		private CheckBoxPrefab MirrorCheckPrefab;

		// Token: 0x0400060D RID: 1549
		private CheckBoxPrefab MirrorVideoCheckPrefab;

		// Token: 0x0400060E RID: 1550
		private TogglePrefab GreenBoxPrefab;

		// Token: 0x0400060F RID: 1551
		private TogglePrefab aIBackgroundPrefab;

		// Token: 0x04000610 RID: 1552
		private TogglePrefab MusicCheckBoxPrefab;

		// Token: 0x04000611 RID: 1553
		private TogglePrefab SlowMotionCheckPrefab;

		// Token: 0x04000612 RID: 1554
		private CheckBoxPrefab ForceWifiCheckPrefab;

		// Token: 0x04000613 RID: 1555
		private FolderBrowserPrefab FolderPrefab;

		// Token: 0x04000614 RID: 1556
		private FileBrowserPrefab musicFilePrefab;

		// Token: 0x04000615 RID: 1557
		private InputBoxPrefab GreenBoxTreshOldPrefab;

		// Token: 0x04000616 RID: 1558
		private InputBoxPrefab VideoTimePrefab;

		// Token: 0x04000617 RID: 1559
		private InputBoxPrefab GifFrameRatePrefab;

		// Token: 0x04000618 RID: 1560
		private InputBoxPrefab GifRepeatCountPrefab;

		// Token: 0x04000619 RID: 1561
		private InputBoxPrefab SlowMotionStartTimePrefab;

		// Token: 0x0400061A RID: 1562
		private InputBoxPrefab SlowMotionEndTimePrefab;

		// Token: 0x0400061B RID: 1563
		private InputBoxPrefab SlowMotionRatePrefab;

		// Token: 0x0400061C RID: 1564
		private InputBoxPrefab VideoWaitLenghtPrefab;

		// Token: 0x0400061D RID: 1565
		private InputBoxPrefab AIMotionPrompPrefab;

		// Token: 0x0400061E RID: 1566
		private ColorPickerPrefab colorGreenScreenPrefab;

		// Token: 0x0400061F RID: 1567
		private SliderPrefab CameraPreviewVideoBrightnessSliderPrefab;

		// Token: 0x04000620 RID: 1568
		private ComboboxPrefab ModeListPrefab;

		// Token: 0x04000621 RID: 1569
		private CheckBoxPrefab AiPromptOptionActivePrefab;

		// Token: 0x04000622 RID: 1570
		private CheckBoxPrefab OrginalOptionActivePrefab;

		// Token: 0x04000623 RID: 1571
		private CheckBoxPrefab FaceSwapOptionActivePrefab;

		// Token: 0x04000624 RID: 1572
		private CheckBoxPrefab WordPortraitOptionActivePrefab;

		// Token: 0x04000625 RID: 1573
		private CheckBoxPrefab AiMotionOptionActivePrefab;

		// Token: 0x04000626 RID: 1574
		private CheckBoxPrefab AiEffectOptionActivePrefab;

		// Token: 0x04000627 RID: 1575
		private CheckBoxPrefab AiBeautifierOptionActivePrefab;

		// Token: 0x04000628 RID: 1576
		private SegmentedPrefab AiPromptResolutionSegmentedPrefab;

		// Token: 0x04000629 RID: 1577
		private TogglePrefab AiPromptTextBoxTogglePrefab;

		// Token: 0x0400062A RID: 1578
		private CheckBoxPrefab PhotoCheckPrefab;

		// Token: 0x0400062B RID: 1579
		private CheckBoxPrefab VideoCheckPrefab;

		// Token: 0x0400062C RID: 1580
		private CheckBoxPrefab GifCheckPrefab;

		// Token: 0x0400062D RID: 1581
		private CheckBoxPrefab DesktopEditingCheckPrefab;

		// Token: 0x0400062E RID: 1582
		private CheckBoxPrefab FullScreenPreviewCheckPrefab;

		// Token: 0x0400062F RID: 1583
		private CheckBoxPrefab BackgroundPreviewCheckPrefab;

		// Token: 0x04000630 RID: 1584
		private CheckBoxPrefab GalleryCheckPrefab;

		// Token: 0x04000631 RID: 1585
		private CheckBoxPrefab BackgroundDeliveryPrefab;

		// Token: 0x04000632 RID: 1586
		private ComboboxPrefab CameraTypeListPrefab;

		// Token: 0x04000633 RID: 1587
		private ComboboxPrefab CameraListPrefab;

		// Token: 0x04000634 RID: 1588
		private ComboboxPrefab RotationListPrefab;

		// Token: 0x04000635 RID: 1589
		private ComboboxPrefab AiSelectionPrefab;

		// Token: 0x04000636 RID: 1590
		private InputBoxPrefab ReturnPasswordPrefab;

		// Token: 0x04000637 RID: 1591
		private CheckBoxPrefab RunOnStartupPrefab;

		// Token: 0x04000638 RID: 1592
		private CheckBoxPrefab PaymentsActivePrefab;

		// Token: 0x04000639 RID: 1593
		private ComboboxPrefab PaymentTimeComboboxPrefab;

		// Token: 0x0400063A RID: 1594
		private ComboboxPrefab PaymentOptionComboboxPrefab;

		// Token: 0x0400063B RID: 1595
		private InputBoxPrefab PaymentPrintPricePrefab;

		// Token: 0x0400063C RID: 1596
		private InputBoxPrefab PaymentCapturePricePrefab;

		// Token: 0x0400063D RID: 1597
		private InputBoxPrefab StripeAccountIDPrefab;

		// Token: 0x0400063E RID: 1598
		private InputBoxPrefab StripeSecretKeyPrefab;

		// Token: 0x0400063F RID: 1599
		public List<CheckBoxPrefab> aiOptionCheckBox;

		// Token: 0x04000640 RID: 1600
		private static bool firstTimeLoaded;

		// Token: 0x04000641 RID: 1601
		private bool isCameraSelected;

		// Token: 0x04000642 RID: 1602
		private bool isCameraListUpdating;

		// Token: 0x04000643 RID: 1603
		private string lastSelectedType = "";

		// Token: 0x04000644 RID: 1604
		private bool isSelecting;

		// Token: 0x04000645 RID: 1605
		private System.Windows.Point initialMousePosition;

		// Token: 0x04000646 RID: 1606
		private double initialLowerValue;

		// Token: 0x04000647 RID: 1607
		private double initialHigherValue;

		// Token: 0x04000648 RID: 1608
		private bool isLowerEllipseDragging;

		// Token: 0x04000649 RID: 1609
		private bool isHigherEllipseDragging;

		// Token: 0x0400064A RID: 1610
		private Visibility getCameraSettingsVisibilty;

		// Token: 0x020001F2 RID: 498
		public enum CameraType
		{
			// Token: 0x04000E65 RID: 3685
			DSLR,
			// Token: 0x04000E66 RID: 3686
			GoPro,
			// Token: 0x04000E67 RID: 3687
			Webcam,
			// Token: 0x04000E68 RID: 3688
			NoCamera
		}

		// Token: 0x020001F3 RID: 499
		public enum IsFoundWifiInJsonReturnType
		{
			// Token: 0x04000E6A RID: 3690
			True,
			// Token: 0x04000E6B RID: 3691
			False,
			// Token: 0x04000E6C RID: 3692
			WifiError
		}

		// Token: 0x020001F4 RID: 500
		public enum StatusOutputColor
		{
			// Token: 0x04000E6E RID: 3694
			Success,
			// Token: 0x04000E6F RID: 3695
			Warning,
			// Token: 0x04000E70 RID: 3696
			Error
		}
	}
}
