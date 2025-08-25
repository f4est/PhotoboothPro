using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000030 RID: 48
	public partial class SecondWindow : Window
	{
		// Token: 0x060002BB RID: 699 RVA: 0x0000E310 File Offset: 0x0000C510
		public SecondWindow()
		{
			this.InitializeComponent();
			Screen[] screens = Screen.AllScreens;
			Screen nonPrimaryScreen = screens.FirstOrDefault((Screen screen) => !screen.Primary);
			if (nonPrimaryScreen != null)
			{
				base.Left = (double)nonPrimaryScreen.WorkingArea.Left;
				base.Top = (double)nonPrimaryScreen.WorkingArea.Top;
				base.Width = (double)nonPrimaryScreen.WorkingArea.Width;
				base.Height = (double)nonPrimaryScreen.WorkingArea.Height;
				Console.WriteLine("Seçilen Ekran: " + nonPrimaryScreen.DeviceName);
				Console.WriteLine(string.Format("Konum: X={0}, Y={1}", base.Left, base.Top));
				Console.WriteLine(string.Format("Boyut: Width={0}, Height={1}", base.Width, base.Height));
				base.WindowState = WindowState.Normal;
				base.WindowStyle = WindowStyle.None;
				base.PreviewKeyDown += delegate(object s, System.Windows.Input.KeyEventArgs e)
				{
					if (e.Key == Key.Escape)
					{
						base.Close();
					}
				};
				Task.Run(delegate()
				{
					SecondWindow.<<-ctor>b__1_2>d <<-ctor>b__1_2>d;
					<<-ctor>b__1_2>d.<>t__builder = AsyncTaskMethodBuilder.Create();
					<<-ctor>b__1_2>d.<>4__this = this;
					<<-ctor>b__1_2>d.<>1__state = -1;
					<<-ctor>b__1_2>d.<>t__builder.Start<SecondWindow.<<-ctor>b__1_2>d>(ref <<-ctor>b__1_2>d);
					return <<-ctor>b__1_2>d.<>t__builder.Task;
				});
				SecondWindow.Instance = this;
				return;
			}
			throw new Exception("There is no second screen");
		}

		// Token: 0x060002BC RID: 700 RVA: 0x0000E454 File Offset: 0x0000C654
		public void UpdateImage(byte[] imageBytes)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				using (MemoryStream ms = new MemoryStream(imageBytes))
				{
					BitmapImage image = new BitmapImage();
					image.BeginInit();
					image.CacheOption = BitmapCacheOption.OnLoad;
					image.StreamSource = ms;
					image.EndInit();
					image.Freeze();
					this.FullScreenImage.Source = image;
				}
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060002BD RID: 701 RVA: 0x0000E494 File Offset: 0x0000C694
		public void UpdateImage(BitmapImage image)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.FullScreenImage.Source = image;
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x060002BE RID: 702 RVA: 0x0000E4D4 File Offset: 0x0000C6D4
		public void ChangeQrStatus(bool visibility, BitmapImage referanceImage = null)
		{
			if (visibility)
			{
				this.gridSecondQR.Visibility = Visibility.Visible;
				this.QRCodeImage.Source = referanceImage;
				return;
			}
			this.gridSecondQR.Visibility = Visibility.Collapsed;
		}

		// Token: 0x060002BF RID: 703 RVA: 0x0000E4FE File Offset: 0x0000C6FE
		private void FullScreenImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (base.WindowState == WindowState.Normal)
			{
				base.WindowState = WindowState.Maximized;
				return;
			}
			base.WindowState = WindowState.Normal;
		}

		// Token: 0x04000261 RID: 609
		public static SecondWindow Instance;
	}
}
