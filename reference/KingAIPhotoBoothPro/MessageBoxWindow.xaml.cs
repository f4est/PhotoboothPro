using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Pages;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000010 RID: 16
	public partial class MessageBoxWindow : Window
	{
		// Token: 0x06000064 RID: 100 RVA: 0x00004817 File Offset: 0x00002A17
		public static void KillCloseMessageBoxWithTimeThread()
		{
			if (MessageBoxWindow.CloseControlThread != null && MessageBoxWindow.CloseControlThread.IsAlive)
			{
				MessageBoxWindow.CloseControlThread.Abort();
				MessageBoxWindow.CloseControlThread = null;
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000483C File Offset: 0x00002A3C
		public static void StartCloseMessageBoxWithTimeThread(int waitTime)
		{
			MessageBoxWindow.CloseControlThread = new Thread(delegate()
			{
				int time = waitTime;
				for (;;)
				{
					if (MessageBoxWindow.openWindows.Count <= 0)
					{
						goto IL_4F;
					}
					time--;
					if (time <= 0)
					{
						try
						{
							Application.Current.Dispatcher.Invoke(delegate()
							{
								MessageBoxWindow.openWindows[0].CloseWindow();
							});
							goto IL_65;
						}
						catch (Exception)
						{
							goto IL_65;
						}
						goto IL_4F;
					}
					IL_65:
					Thread.Sleep(1000);
					continue;
					IL_4F:
					if (time != waitTime)
					{
						time = waitTime + MessageBoxWindow.AddedTime;
						goto IL_65;
					}
					goto IL_65;
				}
			});
			MessageBoxWindow.CloseControlThread.Start();
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000066 RID: 102 RVA: 0x00004876 File Offset: 0x00002A76
		// (set) Token: 0x06000067 RID: 103 RVA: 0x0000487E File Offset: 0x00002A7E
		public MessageBoxWindow.MessageIcon BoxIcon { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000068 RID: 104 RVA: 0x00004887 File Offset: 0x00002A87
		// (set) Token: 0x06000069 RID: 105 RVA: 0x0000488F File Offset: 0x00002A8F
		public MessageBoxWindow.MessageBoxSize BoxSize { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x0600006A RID: 106 RVA: 0x00004898 File Offset: 0x00002A98
		// (set) Token: 0x0600006B RID: 107 RVA: 0x000048A0 File Offset: 0x00002AA0
		public MessageBoxWindow.MessageBoxReturn Return { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600006C RID: 108 RVA: 0x000048A9 File Offset: 0x00002AA9
		// (set) Token: 0x0600006D RID: 109 RVA: 0x000048B1 File Offset: 0x00002AB1
		public List<MessageBoxWindow.ButtonType> Buttons { get; set; }

		// Token: 0x0600006E RID: 110 RVA: 0x000048BA File Offset: 0x00002ABA
		public MessageBoxWindow.MessageBoxReturn ConvertButtonTypeToMessageBoxReturn(MessageBoxWindow.ButtonType buttonType)
		{
			return (MessageBoxWindow.MessageBoxReturn)buttonType;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000048C0 File Offset: 0x00002AC0
		public static void CreateWindow(string Title, string Desc, List<MessageBoxWindow.ButtonType> Butons = null, MessageBoxWindow.MessageIcon icon = MessageBoxWindow.MessageIcon.Warning, Action<MessageBoxWindow.MessageBoxReturn> callback = null, MessageBoxWindow.MessageBoxSize size = MessageBoxWindow.MessageBoxSize.Large, bool isDialog = false)
		{
			MessageBoxWindow.<>c__DisplayClass34_0 CS$<>8__locals1 = new MessageBoxWindow.<>c__DisplayClass34_0();
			CS$<>8__locals1.Title = Title;
			CS$<>8__locals1.Desc = Desc;
			CS$<>8__locals1.Butons = Butons;
			CS$<>8__locals1.icon = icon;
			CS$<>8__locals1.callback = callback;
			CS$<>8__locals1.size = size;
			CS$<>8__locals1.isDialog = isDialog;
			MessageBoxWindow.<>c__DisplayClass34_0 CS$<>8__locals2 = CS$<>8__locals1;
			List<MessageBoxWindow.ButtonType> butons;
			if ((butons = CS$<>8__locals1.Butons) == null)
			{
				(butons = new List<MessageBoxWindow.ButtonType>()).Add(MessageBoxWindow.ButtonType.Continue);
			}
			CS$<>8__locals2.Butons = butons;
			Application.Current.Dispatcher.Invoke(delegate()
			{
				global::Debug.Log("MessageBoxWindow", string.Format("{0}\n\n{1}", CS$<>8__locals1.Title, CS$<>8__locals1.Desc), "CreateWindow", 130);
			});
			if (EventManagementPage.GetCurrentEvent() != null)
			{
				string pathFolder = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "Log");
				if (!Directory.Exists(pathFolder))
				{
					Directory.CreateDirectory(pathFolder);
				}
				File.WriteAllText(Path.Combine(pathFolder, "Log_" + DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_ffff") + ".txt"), CS$<>8__locals1.Title + "\n" + CS$<>8__locals1.Desc);
			}
			Application.Current.Dispatcher.Invoke(delegate()
			{
				new MessageBoxWindow(CS$<>8__locals1.Title, CS$<>8__locals1.Desc, CS$<>8__locals1.Butons, CS$<>8__locals1.icon, CS$<>8__locals1.callback, CS$<>8__locals1.size, CS$<>8__locals1.isDialog);
			});
		}

		// Token: 0x06000070 RID: 112 RVA: 0x000049C8 File Offset: 0x00002BC8
		public MessageBoxWindow(string Title, string Desc, List<MessageBoxWindow.ButtonType> Buttons = null, MessageBoxWindow.MessageIcon icon = MessageBoxWindow.MessageIcon.Warning, Action<MessageBoxWindow.MessageBoxReturn> callback = null, MessageBoxWindow.MessageBoxSize size = MessageBoxWindow.MessageBoxSize.Large, bool isDialog = false)
		{
			MessageBoxWindow <>4__this = this;
			foreach (MessageBoxWindow window in MessageBoxWindow.openWindows.ToList<MessageBoxWindow>())
			{
				if (window != null)
				{
					window.Close();
				}
			}
			MessageBoxWindow.openWindows.Clear();
			this.MessageTitle = Title;
			this.MessageDesc = Desc;
			if (Buttons != null)
			{
				this.Buttons = Buttons;
			}
			this.BoxIcon = icon;
			this.BoxSize = size;
			this.Callback = callback;
			MessageBoxWindow.thisWindows = this;
			MessageBoxWindow.AddedTime = ((this.BoxSize == MessageBoxWindow.MessageBoxSize.Extra) ? 35 : 0);
			MessageBoxWindow.openWindows.Add(this);
			this.InitializeComponent();
			TimedAction.ExecuteWithDelay(delegate
			{
				try
				{
					if (isDialog)
					{
						<>4__this.ShowDialog();
						global::Debug.Log("Pencere Gösterildi: ", Title, ".ctor", 196);
					}
					else
					{
						<>4__this.Show();
						global::Debug.Log("Pencere Gösterildi: ", Title, ".ctor", 201);
					}
				}
				catch (Exception ex)
				{
					global::Debug.Log("MessageBoxWindow Error", Title + " basliklipencere gösterilemedi \nHata: " + ex.Message, ".ctor", 206);
				}
			}, TimeSpan.FromMilliseconds(5.0));
			base.KeyDown += this.MessageBoxWindow_KeyDown;
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00004B3C File Offset: 0x00002D3C
		private void ManageSize()
		{
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00004B40 File Offset: 0x00002D40
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.txtTitle.Text = this.MessageTitle;
			this.txtDesc.Text = this.MessageDesc;
			switch (this.BoxIcon)
			{
			case MessageBoxWindow.MessageIcon.Warning:
				this.iconImage.Source = new BitmapImage(new Uri("/Images/Warning.png", UriKind.Relative));
				break;
			case MessageBoxWindow.MessageIcon.Error:
				this.iconImage.Source = new BitmapImage(new Uri("/Images/Error.png", UriKind.Relative));
				break;
			case MessageBoxWindow.MessageIcon.Success:
				this.iconImage.Source = new BitmapImage(new Uri("/Images/Succesfull.png", UriKind.Relative));
				break;
			}
			if (base.ActualWidth >= base.ActualHeight)
			{
				this.mainGrid.Width = this.WindowGrid.ActualWidth * 0.35;
			}
			else
			{
				this.mainGrid.Width = this.WindowGrid.ActualWidth * 0.55;
			}
			if (this.BoxSize == MessageBoxWindow.MessageBoxSize.Extra)
			{
				this.txtDesc.TextAlignment = TextAlignment.Left;
				this.mainGrid.Width = this.mainGrid.ActualWidth * 1.7699999809265137;
				this.mainGrid.Height = this.mainGrid.ActualHeight * 1.559999942779541;
			}
			else
			{
				this.txtDesc.TextAlignment = TextAlignment.Center;
			}
			if (this.Buttons == null)
			{
				this.MultiGrid.Visibility = Visibility.Collapsed;
				this.SingleGrid.Visibility = Visibility.Collapsed;
				return;
			}
			if (this.Buttons.Count > 1)
			{
				this.MultiGrid.Visibility = Visibility.Visible;
				this.SingleGrid.Visibility = Visibility.Collapsed;
				this.btnMulti1.Content = this.Buttons[0].ToString();
				this.btnMulti2.Content = this.Buttons[1].ToString();
				return;
			}
			this.SingleGrid.Visibility = Visibility.Visible;
			this.MultiGrid.Visibility = Visibility.Collapsed;
			this.btnSingle.Content = this.Buttons[0].ToString();
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00004D68 File Offset: 0x00002F68
		private void Window_Unloaded(object sender, RoutedEventArgs e)
		{
			MessageBoxWindow.openWindows.Remove(this);
		}

		// Token: 0x06000074 RID: 116 RVA: 0x00004D78 File Offset: 0x00002F78
		private void btn_Click(object sender, RoutedEventArgs e)
		{
			Button refButton = sender as Button;
			int UID = (int)Convert.ToInt16(refButton.Uid);
			this.Return = this.ConvertButtonTypeToMessageBoxReturn(this.Buttons[UID]);
			if (this.Callback != null)
			{
				this.Callback(this.Return);
			}
			base.Close();
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00004DCF File Offset: 0x00002FCF
		public void CloseWindow()
		{
			MessageBoxWindow.openWindows.Remove(this);
			base.Close();
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00004DE3 File Offset: 0x00002FE3
		private void MessageBoxWindow_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				MessageBoxWindow.openWindows.Remove(this);
				base.Close();
			}
		}

		// Token: 0x0400004F RID: 79
		public Action<MessageBoxWindow.MessageBoxReturn> Callback;

		// Token: 0x04000050 RID: 80
		private static MessageBoxWindow thisWindows;

		// Token: 0x04000051 RID: 81
		private static List<MessageBoxWindow> openWindows = new List<MessageBoxWindow>();

		// Token: 0x04000052 RID: 82
		private static Thread CloseControlThread;

		// Token: 0x04000053 RID: 83
		private static int AddedTime = 0;

		// Token: 0x04000058 RID: 88
		public string MessageTitle;

		// Token: 0x04000059 RID: 89
		public string MessageDesc;

		// Token: 0x0400005A RID: 90
		private int[] LargeGridRowNumbers = new int[]
		{
			2,
			3,
			2
		};

		// Token: 0x0400005B RID: 91
		private int[] LargeGridColumnNumbers = new int[]
		{
			1,
			1,
			1
		};

		// Token: 0x0400005C RID: 92
		private int[] SmallGridRowNumbers = new int[]
		{
			4,
			3,
			4
		};

		// Token: 0x0400005D RID: 93
		private int[] SmallGridColumnNumbers = new int[]
		{
			3,
			2,
			3
		};

		// Token: 0x020000F4 RID: 244
		public enum MessageBoxReturn
		{
			// Token: 0x04000AD6 RID: 2774
			Cancel,
			// Token: 0x04000AD7 RID: 2775
			Continue,
			// Token: 0x04000AD8 RID: 2776
			TryAgain,
			// Token: 0x04000AD9 RID: 2777
			Yes,
			// Token: 0x04000ADA RID: 2778
			No
		}

		// Token: 0x020000F5 RID: 245
		public enum MessageIcon
		{
			// Token: 0x04000ADC RID: 2780
			Warning,
			// Token: 0x04000ADD RID: 2781
			Error,
			// Token: 0x04000ADE RID: 2782
			Success
		}

		// Token: 0x020000F6 RID: 246
		public enum MessageBoxSize
		{
			// Token: 0x04000AE0 RID: 2784
			Small,
			// Token: 0x04000AE1 RID: 2785
			Large,
			// Token: 0x04000AE2 RID: 2786
			Extra
		}

		// Token: 0x020000F7 RID: 247
		public enum ButtonType
		{
			// Token: 0x04000AE4 RID: 2788
			Cancel,
			// Token: 0x04000AE5 RID: 2789
			Continue,
			// Token: 0x04000AE6 RID: 2790
			TryAgain,
			// Token: 0x04000AE7 RID: 2791
			Yes,
			// Token: 0x04000AE8 RID: 2792
			No
		}
	}
}
