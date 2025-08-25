using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using CameraControl.Devices;
using KingAIPhotoBoothPro.Class;

namespace KingAIPhotoBoothPro
{
	// Token: 0x0200000E RID: 14
	public class CameraSettingsWindows : Window, IComponentConnector
	{
		// Token: 0x0600004B RID: 75 RVA: 0x00003E72 File Offset: 0x00002072
		public CameraSettingsWindows()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003E80 File Offset: 0x00002080
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
			this.CompressionSettingsComboBox.SelectionChanged += this.CompressionSettingsComboBox_SelectionChanged;
			LiveViewClass.LiveViewImageLoaded += this.LiveViewClass_LiveViewImageLoaded;
			LiveViewClass.ReceiveLiveImage += this.LiveViewClass_ReceiveLiveImage;
			CameraControlClass.ReconnectedCameraEvent += this.CameraControlClass_ReconnectedCameraEvent;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003EDD File Offset: 0x000020DD
		private void CameraControlClass_ReconnectedCameraEvent()
		{
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003EE0 File Offset: 0x000020E0
		private void LiveViewClass_ReceiveLiveImage(bool isFirstReceive)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (isFirstReceive && !this.firstFrame)
				{
					CameraControlClass.isConnectedCamera = true;
					CameraControlClass.isCameraDisconnect = false;
					this.firstFrame = true;
				}
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00003F28 File Offset: 0x00002128
		private void LiveViewClass_LiveViewImageLoaded()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				if (this.firstFrame)
				{
					this.gridPreviewBackgroundImage.Source = LiveViewClass.staticImage;
				}
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x06000050 RID: 80 RVA: 0x00003F54 File Offset: 0x00002154
		private void ControlLargePicture()
		{
			if (this.thisCamera != null && this.thisCamera.CompressionSetting.IsEnabled && this.CompressionSettingsComboBox.SelectedItem.ToString().Contains("Large"))
			{
				int index = this.thisCamera.CompressionSetting.Values.ToList<string>().FindIndex((string x) => x.Contains("Small Normal"));
				if (index == -1)
				{
					index = this.thisCamera.CompressionSetting.Values.ToList<string>().FindIndex((string x) => x.Contains("Small"));
				}
				try
				{
					this.CompressionSettingsComboBox.SelectedIndex = index;
				}
				catch (Exception ex)
				{
				}
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x0000403C File Offset: 0x0000223C
		private void CompressionSettingsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.CompressionSettingsComboBox.SelectedItem.ToString().Contains("Large"))
			{
				MessageBoxWindow.CreateWindow("Image Size Warning", "If you take a large photo, all AI operations and QR operations will slow down. Please choose a lower format for faster processing.", null, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00004070 File Offset: 0x00002270
		public ICameraDevice CameraInfoControl(ICameraDevice camera)
		{
			if (camera.FocusMode.IsEnabled || camera.FocusMode.Values.Count > 0)
			{
				camera.FocusMode.IsEnabled = (camera.FocusMode.Values.Count > 0);
			}
			if (camera.IsoNumber.IsEnabled || camera.IsoNumber.Values.Count > 0)
			{
				camera.IsoNumber.IsEnabled = (camera.IsoNumber.Values.Count > 0);
			}
			if (camera.ShutterSpeed.IsEnabled || camera.ShutterSpeed.Values.Count > 0)
			{
				camera.ShutterSpeed.IsEnabled = (camera.ShutterSpeed.Values.Count > 0);
			}
			if (camera.WhiteBalance.IsEnabled || camera.WhiteBalance.Values.Count > 0)
			{
				camera.WhiteBalance.IsEnabled = (camera.WhiteBalance.Values.Count > 0);
			}
			if (camera.CompressionSetting.IsEnabled || camera.CompressionSetting.Values.Count > 0)
			{
				camera.CompressionSetting.IsEnabled = (camera.CompressionSetting.Values.Count > 0);
			}
			if (camera.FNumber.IsEnabled || camera.FNumber.Values.Count > 0)
			{
				camera.FNumber.IsEnabled = (camera.FNumber.Values.Count > 0);
			}
			if (camera.DisplayName.Length < 4)
			{
				camera.DisplayName = "(" + camera.DeviceName + ")";
			}
			return camera;
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000421C File Offset: 0x0000241C
		public void Load()
		{
			this.loadActive = true;
			ICameraDevice camera = CameraControlClass.DeviceManager.SelectedCameraDevice;
			camera = (this.thisCamera = this.CameraInfoControl(camera));
			this.cameraSettingsGrid.DataContext = camera;
			Task.Run(delegate()
			{
				CameraSettingsWindows.<<Load>b__13_0>d <<Load>b__13_0>d;
				<<Load>b__13_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<Load>b__13_0>d.<>4__this = this;
				<<Load>b__13_0>d.<>1__state = -1;
				<<Load>b__13_0>d.<>t__builder.Start<CameraSettingsWindows.<<Load>b__13_0>d>(ref <<Load>b__13_0>d);
				return <<Load>b__13_0>d.<>t__builder.Task;
			});
			if (!LiveViewClass.liveActive || !LiveViewClass.IsLiveViewTimerEnabled)
			{
				CameraControlClass.StartLiveCamera();
				LiveViewClass.IsLiveViewTimerEnabled = true;
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00004284 File Offset: 0x00002484
		public double GetBrightessValue()
		{
			double isoValue = (double)this.IsoNumberBox.SelectedIndex / ((double)this.thisCamera.IsoNumber.Values.Count - 1.0) * 100.0;
			double shutterValue = (1.0 - (double)this.ShutterBox.SelectedIndex / ((double)this.thisCamera.ShutterSpeed.Values.Count - 1.0)) * 100.0;
			double fstopValue = (1.0 - (double)this.FNumberBox.SelectedIndex / ((double)this.thisCamera.FNumber.Values.Count - 1.0)) * 100.0;
			return (isoValue + shutterValue + fstopValue) / 3.0;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x0000435E File Offset: 0x0000255E
		private void cameraSettingsGrid_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				base.DragMove();
			}
		}

		// Token: 0x06000056 RID: 86 RVA: 0x00004370 File Offset: 0x00002570
		private void brightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (this.loadActive || this.thisCamera == null)
			{
				return;
			}
			double brightness = e.NewValue;
			this.BrightnessValueLabel.Content = brightness.ToString("F0");
		}

		// Token: 0x06000057 RID: 87 RVA: 0x000043AC File Offset: 0x000025AC
		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			this.CompressionSettingsComboBox.SelectionChanged -= this.CompressionSettingsComboBox_SelectionChanged;
			LiveViewClass.LiveViewImageLoaded -= this.LiveViewClass_LiveViewImageLoaded;
			LiveViewClass.ReceiveLiveImage -= this.LiveViewClass_ReceiveLiveImage;
			CameraControlClass.ReconnectedCameraEvent -= this.CameraControlClass_ReconnectedCameraEvent;
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00004404 File Offset: 0x00002604
		private void Window_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (this.loadActive || this.thisCamera == null || this._pendingValue == this.brightnessSlider.Value)
			{
				return;
			}
			this._pendingValue = this.brightnessSlider.Value;
			double brightness = this._pendingValue;
			int isoIndex = -1;
			int shutterIndex = -1;
			int fstopIndex = -1;
			if (this.thisCamera.IsoNumber.IsEnabled)
			{
				isoIndex = (int)(brightness / 100.0 * (double)(this.thisCamera.IsoNumber.Values.Count - 1));
			}
			if (this.thisCamera.ShutterSpeed.IsEnabled)
			{
				shutterIndex = this.thisCamera.ShutterSpeed.Values.Count - 1 - (int)(brightness / 100.0 * (double)(this.thisCamera.ShutterSpeed.Values.Count - 1));
			}
			if (this.thisCamera.FNumber.IsEnabled)
			{
				fstopIndex = this.thisCamera.FNumber.Values.Count - 1 - (int)(brightness / 100.0 * (double)(this.thisCamera.FNumber.Values.Count - 1));
			}
			if (isoIndex != -1)
			{
				this.IsoNumberBox.SelectedIndex = isoIndex;
			}
			if (shutterIndex != -1)
			{
				this.ShutterBox.SelectedIndex = shutterIndex;
			}
			if (fstopIndex != -1)
			{
				this.FNumberBox.SelectedIndex = fstopIndex;
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x0000455B File Offset: 0x0000275B
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.CloseCameraSettingsWindow();
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00004564 File Offset: 0x00002764
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocater = new Uri("/KingAIPhotoBoothPro;component/pages/camerasettingswindows.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00004594 File Offset: 0x00002794
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				((CameraSettingsWindows)target).PreviewMouseUp += this.Window_PreviewMouseUp;
				((CameraSettingsWindows)target).Loaded += this.Window_Loaded;
				((CameraSettingsWindows)target).Unloaded += this.Window_Unloaded;
				return;
			case 2:
				this.GridMain = (Grid)target;
				return;
			case 3:
				((Grid)target).MouseDown += this.cameraSettingsGrid_MouseDown;
				return;
			case 4:
				this.cameraSettingsGrid = (GroupBox)target;
				return;
			case 5:
				this.FNumberBox = (ComboBox)target;
				return;
			case 6:
				this.IsoNumberBox = (ComboBox)target;
				return;
			case 7:
				this.ShutterBox = (ComboBox)target;
				return;
			case 8:
				this.CompressionSettingsComboBox = (ComboBox)target;
				return;
			case 9:
				this.BrightnessLabel = (Label)target;
				return;
			case 10:
				this.brightnessSlider = (Slider)target;
				this.brightnessSlider.ValueChanged += this.brightnessSlider_ValueChanged;
				return;
			case 11:
				this.BrightnessValueLabel = (Label)target;
				return;
			case 12:
				this.gridPreviewBackgroundImage = (Image)target;
				return;
			case 13:
				this.DeleteButton = (Button)target;
				this.DeleteButton.Click += this.DeleteButton_Click;
				return;
			case 14:
				this.DeleteButtonInsideGrid = (Grid)target;
				return;
			case 15:
				this.Circle = (Path)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000032 RID: 50
		private ICameraDevice thisCamera;

		// Token: 0x04000033 RID: 51
		private bool loadActive;

		// Token: 0x04000034 RID: 52
		private bool firstFrame;

		// Token: 0x04000035 RID: 53
		private bool _sliderDragging;

		// Token: 0x04000036 RID: 54
		private double _pendingValue;

		// Token: 0x04000037 RID: 55
		internal Grid GridMain;

		// Token: 0x04000038 RID: 56
		internal GroupBox cameraSettingsGrid;

		// Token: 0x04000039 RID: 57
		internal ComboBox FNumberBox;

		// Token: 0x0400003A RID: 58
		internal ComboBox IsoNumberBox;

		// Token: 0x0400003B RID: 59
		internal ComboBox ShutterBox;

		// Token: 0x0400003C RID: 60
		internal ComboBox CompressionSettingsComboBox;

		// Token: 0x0400003D RID: 61
		internal Label BrightnessLabel;

		// Token: 0x0400003E RID: 62
		internal Slider brightnessSlider;

		// Token: 0x0400003F RID: 63
		internal Label BrightnessValueLabel;

		// Token: 0x04000040 RID: 64
		internal Image gridPreviewBackgroundImage;

		// Token: 0x04000041 RID: 65
		internal Button DeleteButton;

		// Token: 0x04000042 RID: 66
		internal Grid DeleteButtonInsideGrid;

		// Token: 0x04000043 RID: 67
		internal Path Circle;

		// Token: 0x04000044 RID: 68
		private bool _contentLoaded;
	}
}
