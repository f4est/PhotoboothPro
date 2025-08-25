using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000034 RID: 52
	public class DashboardPage : SettingsSubPage, IComponentConnector
	{
		// Token: 0x060002E4 RID: 740 RVA: 0x0000EE58 File Offset: 0x0000D058
		public DashboardPage()
		{
			this.InitializeComponent();
			ExtensionMethod.AddFormattedTextToTextBlock(this.ThankLicenceLabel, new List<string>
			{
				"Thank you for choosing Activation King products.\n" + AppInfo.AppClass.Name + " software licence is valid till ",
				Login.expressionTime
			}, new List<ExtensionMethod.FontType>
			{
				ExtensionMethod.FontType.Normal,
				ExtensionMethod.FontType.Bold
			});
			this.DiscoverButton.Background = new SolidColorBrush(AppInfo.ThemeColor);
			this.AICreditsCountText.Text = "0";
			this.FaceSwapCredit.Text = string.Format("{0} Left", 0);
			this.AIPrompCredit.Text = string.Format("{0} Left", 0);
		}

		// Token: 0x060002E5 RID: 741 RVA: 0x0000EF30 File Offset: 0x0000D130
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			DashboardPage.<Page_Loaded>d__4 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<DashboardPage.<Page_Loaded>d__4>(ref <Page_Loaded>d__);
		}

		// Token: 0x060002E6 RID: 742 RVA: 0x0000EF68 File Offset: 0x0000D168
		private Task UpdateAICreditStatusUIAsync()
		{
			DashboardPage.<UpdateAICreditStatusUIAsync>d__5 <UpdateAICreditStatusUIAsync>d__;
			<UpdateAICreditStatusUIAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<UpdateAICreditStatusUIAsync>d__.<>4__this = this;
			<UpdateAICreditStatusUIAsync>d__.<>1__state = -1;
			<UpdateAICreditStatusUIAsync>d__.<>t__builder.Start<DashboardPage.<UpdateAICreditStatusUIAsync>d__5>(ref <UpdateAICreditStatusUIAsync>d__);
			return <UpdateAICreditStatusUIAsync>d__.<>t__builder.Task;
		}

		// Token: 0x060002E7 RID: 743 RVA: 0x0000EFAC File Offset: 0x0000D1AC
		private void LogOutButton_Click(object sender, RoutedEventArgs e)
		{
			DashboardPage.<LogOutButton_Click>d__6 <LogOutButton_Click>d__;
			<LogOutButton_Click>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<LogOutButton_Click>d__.<>4__this = this;
			<LogOutButton_Click>d__.<>1__state = -1;
			<LogOutButton_Click>d__.<>t__builder.Start<DashboardPage.<LogOutButton_Click>d__6>(ref <LogOutButton_Click>d__);
		}

		// Token: 0x060002E8 RID: 744 RVA: 0x0000EFE4 File Offset: 0x0000D1E4
		public Task Logout()
		{
			DashboardPage.<Logout>d__7 <Logout>d__;
			<Logout>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Logout>d__.<>4__this = this;
			<Logout>d__.<>1__state = -1;
			<Logout>d__.<>t__builder.Start<DashboardPage.<Logout>d__7>(ref <Logout>d__);
			return <Logout>d__.<>t__builder.Task;
		}

		// Token: 0x060002E9 RID: 745 RVA: 0x0000F028 File Offset: 0x0000D228
		private void LicenceLogoutPostCompleted(bool success, string content)
		{
			Application.Current.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.LogOutButton.IsEnabled = true;
			}), Array.Empty<object>()).Wait();
			if (!success)
			{
				MessageBoxWindow.CreateWindow("Connection Error", "Please try again!", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			LicenceLoginResponse response = JsonConvert.DeserializeObject<LicenceLoginResponse>(content);
			if (response.LicenceValid)
			{
				DashboardPage.ClearLoginData();
				return;
			}
			MessageBoxWindow.CreateWindow("Server Error", response.Message, new List<MessageBoxWindow.ButtonType>
			{
				MessageBoxWindow.ButtonType.Continue
			}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			DashboardPage.ClearLoginData();
		}

		// Token: 0x060002EA RID: 746 RVA: 0x0000F0B8 File Offset: 0x0000D2B8
		private static void ClearLoginData()
		{
			string[] filePaths = new string[]
			{
				System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Config.AppIdentifier, "data.bin"),
				System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Config.AppIdentifier, "data3.bin")
			};
			for (int i = 0; i < filePaths.Length; i++)
			{
				if (File.Exists(filePaths[i]))
				{
					File.Delete(filePaths[i]);
				}
			}
			SessionData.accountInfo = null;
			ActivationKingWindow.SetPage(ApplicationPage.LoginPage, false);
		}

		// Token: 0x060002EB RID: 747 RVA: 0x0000F12C File Offset: 0x0000D32C
		private void DiscoverButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start("https://activationking.com");
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x060002EC RID: 748 RVA: 0x0000F15C File Offset: 0x0000D35C
		public override string GetTitle()
		{
			return "Dashboard";
		}

		// Token: 0x060002ED RID: 749 RVA: 0x0000F163 File Offset: 0x0000D363
		public override string GetSubTitle()
		{
			return "";
		}

		// Token: 0x060002EE RID: 750 RVA: 0x0000F16C File Offset: 0x0000D36C
		private void BuyNowButton_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Process.Start(AppInfo.AppClass.CreditLink);
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x060002EF RID: 751 RVA: 0x0000F1A0 File Offset: 0x0000D3A0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocater = new Uri("/KingAIPhotoBoothPro;component/pages/activationking/dashboardpage.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}

		// Token: 0x060002F0 RID: 752 RVA: 0x0000F1D0 File Offset: 0x0000D3D0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060002F1 RID: 753 RVA: 0x0000F1DC File Offset: 0x0000D3DC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.gridMain = (Grid)target;
				return;
			case 2:
				this.Properties = (Grid)target;
				return;
			case 3:
				this.borderLoginSmooth = (Border)target;
				return;
			case 4:
				this.UserNameLabel = (TextBlock)target;
				return;
			case 5:
				this.txtSoftwareName = (TextBlock)target;
				return;
			case 6:
				this.ThankLicenceLabel = (TextBlock)target;
				return;
			case 7:
				this.LogOutButton = (Button)target;
				this.LogOutButton.Click += this.LogOutButton_Click;
				return;
			case 8:
				this.DiscoverButton = (Button)target;
				this.DiscoverButton.Click += this.DiscoverButton_Click;
				return;
			case 9:
				this.AICreditsGrid = (Grid)target;
				return;
			case 10:
				this.AICreditsHeaderGrid = (Grid)target;
				return;
			case 11:
				this.CreditStatusMainGrid = (Grid)target;
				return;
			case 12:
				this.CreditStatusBackgroundBorder = (Border)target;
				return;
			case 13:
				this.CreditStatusText = (TextBlock)target;
				return;
			case 14:
				this.CreditsCountsGrid = (Grid)target;
				return;
			case 15:
				this.TotalCreditsGrid = (Grid)target;
				return;
			case 16:
				this.TotalCreditsGridBorder = (Border)target;
				return;
			case 17:
				this.TotalCreditsIconGrid = (Grid)target;
				return;
			case 18:
				this.TotalCreditsIconPath = (System.Windows.Shapes.Path)target;
				return;
			case 19:
				this.TotalCreditsTextGrid = (Grid)target;
				return;
			case 20:
				this.AICreditsCountText = (TextBlock)target;
				return;
			case 21:
				this.BuyNowGrid = (Grid)target;
				return;
			case 22:
				this.BuyNowButton = (Button)target;
				this.BuyNowButton.Click += this.BuyNowButton_Click;
				return;
			case 23:
				this.BackgroundBorder = (Border)target;
				return;
			case 24:
				this.BuyNowIconPathBorder = (Border)target;
				return;
			case 25:
				this.BuyNowIconPath = (System.Windows.Shapes.Path)target;
				return;
			case 26:
				this.FaceSwapCostGrid = (Grid)target;
				return;
			case 27:
				this.FaceSwapCostGridBackgroundBorder = (Border)target;
				return;
			case 28:
				this.FaceSwapIconGrid = (Grid)target;
				return;
			case 29:
				this.FaceSwapIconPath = (System.Windows.Shapes.Path)target;
				return;
			case 30:
				this.FaceSwapCredit = (TextBlock)target;
				return;
			case 31:
				this.FaceSwapCostIcon = (System.Windows.Shapes.Path)target;
				return;
			case 32:
				this.AIPromptCostGrid = (Grid)target;
				return;
			case 33:
				this.AIPromptCostGridBackgroundBorder = (Border)target;
				return;
			case 34:
				this.AIPromptIconGrid = (Grid)target;
				return;
			case 35:
				this.AIPromptIconPath = (System.Windows.Shapes.Path)target;
				return;
			case 36:
				this.AIPrompCredit = (TextBlock)target;
				return;
			case 37:
				this.AIPromptCostIcon = (System.Windows.Shapes.Path)target;
				return;
			case 38:
				this.loadingGifGrid = (Grid)target;
				return;
			case 39:
				this.LoadingDotsContainer = (Grid)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x0400028C RID: 652
		private float aICreditsLowLimit = 100f;

		// Token: 0x0400028D RID: 653
		private float aICreditsVeryLowLimit = 25f;

		// Token: 0x0400028E RID: 654
		private static bool firstOpen = true;

		// Token: 0x0400028F RID: 655
		internal Grid gridMain;

		// Token: 0x04000290 RID: 656
		internal Grid Properties;

		// Token: 0x04000291 RID: 657
		internal Border borderLoginSmooth;

		// Token: 0x04000292 RID: 658
		internal TextBlock UserNameLabel;

		// Token: 0x04000293 RID: 659
		internal TextBlock txtSoftwareName;

		// Token: 0x04000294 RID: 660
		internal TextBlock ThankLicenceLabel;

		// Token: 0x04000295 RID: 661
		internal Button LogOutButton;

		// Token: 0x04000296 RID: 662
		internal Button DiscoverButton;

		// Token: 0x04000297 RID: 663
		internal Grid AICreditsGrid;

		// Token: 0x04000298 RID: 664
		internal Grid AICreditsHeaderGrid;

		// Token: 0x04000299 RID: 665
		internal Grid CreditStatusMainGrid;

		// Token: 0x0400029A RID: 666
		internal Border CreditStatusBackgroundBorder;

		// Token: 0x0400029B RID: 667
		internal TextBlock CreditStatusText;

		// Token: 0x0400029C RID: 668
		internal Grid CreditsCountsGrid;

		// Token: 0x0400029D RID: 669
		internal Grid TotalCreditsGrid;

		// Token: 0x0400029E RID: 670
		internal Border TotalCreditsGridBorder;

		// Token: 0x0400029F RID: 671
		internal Grid TotalCreditsIconGrid;

		// Token: 0x040002A0 RID: 672
		internal System.Windows.Shapes.Path TotalCreditsIconPath;

		// Token: 0x040002A1 RID: 673
		internal Grid TotalCreditsTextGrid;

		// Token: 0x040002A2 RID: 674
		internal TextBlock AICreditsCountText;

		// Token: 0x040002A3 RID: 675
		internal Grid BuyNowGrid;

		// Token: 0x040002A4 RID: 676
		internal Button BuyNowButton;

		// Token: 0x040002A5 RID: 677
		internal Border BackgroundBorder;

		// Token: 0x040002A6 RID: 678
		internal Border BuyNowIconPathBorder;

		// Token: 0x040002A7 RID: 679
		internal System.Windows.Shapes.Path BuyNowIconPath;

		// Token: 0x040002A8 RID: 680
		internal Grid FaceSwapCostGrid;

		// Token: 0x040002A9 RID: 681
		internal Border FaceSwapCostGridBackgroundBorder;

		// Token: 0x040002AA RID: 682
		internal Grid FaceSwapIconGrid;

		// Token: 0x040002AB RID: 683
		internal System.Windows.Shapes.Path FaceSwapIconPath;

		// Token: 0x040002AC RID: 684
		internal TextBlock FaceSwapCredit;

		// Token: 0x040002AD RID: 685
		internal System.Windows.Shapes.Path FaceSwapCostIcon;

		// Token: 0x040002AE RID: 686
		internal Grid AIPromptCostGrid;

		// Token: 0x040002AF RID: 687
		internal Border AIPromptCostGridBackgroundBorder;

		// Token: 0x040002B0 RID: 688
		internal Grid AIPromptIconGrid;

		// Token: 0x040002B1 RID: 689
		internal System.Windows.Shapes.Path AIPromptIconPath;

		// Token: 0x040002B2 RID: 690
		internal TextBlock AIPrompCredit;

		// Token: 0x040002B3 RID: 691
		internal System.Windows.Shapes.Path AIPromptCostIcon;

		// Token: 0x040002B4 RID: 692
		internal Grid loadingGifGrid;

		// Token: 0x040002B5 RID: 693
		internal Grid LoadingDotsContainer;

		// Token: 0x040002B6 RID: 694
		private bool _contentLoaded;
	}
}
