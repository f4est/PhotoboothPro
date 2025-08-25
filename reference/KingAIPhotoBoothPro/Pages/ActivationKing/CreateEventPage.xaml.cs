using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000048 RID: 72
	public partial class CreateEventPage : Page
	{
		// Token: 0x17000083 RID: 131
		// (get) Token: 0x060005A6 RID: 1446 RVA: 0x000271D6 File Offset: 0x000253D6
		public SolidColorBrush ColorBrushTheme
		{
			get
			{
				return this.colorBrushTheme;
			}
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x000271E0 File Offset: 0x000253E0
		public void EditOpen(OperationalEvent operationalEvent)
		{
			this.operationalEventToEdit = operationalEvent;
			this.btnCreate.Visibility = Visibility.Hidden;
			this.btnEdit.Visibility = Visibility.Visible;
			this.btnCancel.Visibility = Visibility.Visible;
			this.EventNameText.Text = operationalEvent.EventName;
			this.EventMottoText.Text = operationalEvent.EventMotto;
			if (this.EventNameText.Text.Length > 0)
			{
				this.EventNamePlaceholder.Visibility = Visibility.Hidden;
			}
			if (this.EventMottoText.Text.Length > 0)
			{
				this.EventMottoPlaceholder.Visibility = Visibility.Hidden;
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x00027278 File Offset: 0x00025478
		public void CreateOpen()
		{
			Application.Current.Dispatcher.Invoke(delegate()
			{
				this.btnCreate.Visibility = Visibility.Visible;
				this.btnEdit.Visibility = Visibility.Hidden;
				this.btnCancel.Visibility = Visibility.Hidden;
				this.EventNameText.Text = "";
				this.EventMottoText.Text = "";
			});
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x00027295 File Offset: 0x00025495
		private void CancelClicked(object sender, RoutedEventArgs e)
		{
			this.EventNameText.Text = "";
			this.EventMottoText.Text = "";
			this.eventManagement.CancelEdit();
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x000272C2 File Offset: 0x000254C2
		private void EditClicked(object sender, RoutedEventArgs e)
		{
			this.operationalEventToEdit.EventName = this.EventNameText.Text;
			this.operationalEventToEdit.EventMotto = this.EventMottoText.Text;
			this.eventManagement.EditEvent(this.operationalEventToEdit);
		}

		// Token: 0x060005AB RID: 1451 RVA: 0x00027304 File Offset: 0x00025504
		private void CreateClicked(object sender, RoutedEventArgs e)
		{
			if (this.EventNameText.Text.Length > 0)
			{
				this.eventManagement.CreateEvent(this.EventNameText.Text, this.EventMottoText.Text);
				Settings.SetAllDefaultValues();
				Task.Run(delegate()
				{
					Settings.UpdateFaceSwapDefaultData();
					Settings.UpdateSamplePromptsDefaultData();
				});
				return;
			}
			Console.WriteLine("Input can not be empty!");
		}

		// Token: 0x060005AC RID: 1452 RVA: 0x0002737A File Offset: 0x0002557A
		public CreateEventPage()
		{
			this.InitializeComponent();
		}

		// Token: 0x060005AD RID: 1453 RVA: 0x00027388 File Offset: 0x00025588
		private void EventNameText_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.EventNameText.Text.Length == 0)
			{
				this.EventNamePlaceholder.Visibility = Visibility.Visible;
				return;
			}
			this.EventNamePlaceholder.Visibility = Visibility.Hidden;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000273B8 File Offset: 0x000255B8
		private void EventNameText_LostFocus(object sender, RoutedEventArgs e)
		{
			CreateEventPage.textLenght = this.EventNameText.Text.Length;
			if (this.EventNameText.Text.Length == 0)
			{
				this.EventNamePlaceholder.Visibility = Visibility.Visible;
				return;
			}
			this.EventNamePlaceholder.Visibility = Visibility.Hidden;
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x00027405 File Offset: 0x00025605
		private void EventMottoText_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.EventMottoText.Text.Length == 0)
			{
				this.EventMottoPlaceholder.Visibility = Visibility.Visible;
				return;
			}
			this.EventMottoPlaceholder.Visibility = Visibility.Hidden;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x00027432 File Offset: 0x00025632
		private void EventMottoText_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.EventMottoText.Text.Length == 0)
			{
				this.EventMottoPlaceholder.Visibility = Visibility.Visible;
				return;
			}
			this.EventMottoPlaceholder.Visibility = Visibility.Hidden;
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00027460 File Offset: 0x00025660
		private void Placeholder_Click(object sender, RoutedEventArgs e)
		{
			Button refID = sender as Button;
			int UID = (int)Convert.ToInt16(refID.Uid);
			if (UID == 0)
			{
				refID.Visibility = Visibility.Hidden;
				this.EventNameText.Focus();
				return;
			}
			if (UID == 1)
			{
				refID.Visibility = Visibility.Hidden;
				this.EventMottoText.Focus();
			}
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x000274B0 File Offset: 0x000256B0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.eventManagement = SettingsPage.eventManagementPage;
			this.btnCreate.Click += this.CreateClicked;
			this.btnEdit.Click += this.EditClicked;
			this.btnCancel.Click += this.CancelClicked;
			this.colorBrushTheme = new SolidColorBrush(AppInfo.ThemeColor);
			this.btnCreate.Background = new SolidColorBrush(AppInfo.ThemeColor);
			this.btnEdit.Background = new SolidColorBrush(AppInfo.ThemeColor);
			try
			{
				if (EventManagementPage.Events == null || EventManagementPage.Events.Count < 2)
				{
					this.ActivateDirectionBlink();
				}
			}
			catch (Exception ex)
			{
			}
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x00027578 File Offset: 0x00025778
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.btnCreate.Click -= this.CreateClicked;
			this.btnEdit.Click -= this.EditClicked;
			this.btnCancel.Click -= this.CancelClicked;
		}

		// Token: 0x060005B4 RID: 1460 RVA: 0x000275CA File Offset: 0x000257CA
		private void ActivateDirectionBlink()
		{
			Task.Run(delegate()
			{
				CreateEventPage.<<ActivateDirectionBlink>b__19_0>d <<ActivateDirectionBlink>b__19_0>d;
				<<ActivateDirectionBlink>b__19_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<ActivateDirectionBlink>b__19_0>d.<>4__this = this;
				<<ActivateDirectionBlink>b__19_0>d.<>1__state = -1;
				<<ActivateDirectionBlink>b__19_0>d.<>t__builder.Start<CreateEventPage.<<ActivateDirectionBlink>b__19_0>d>(ref <<ActivateDirectionBlink>b__19_0>d);
				return <<ActivateDirectionBlink>b__19_0>d.<>t__builder.Task;
			});
		}

		// Token: 0x040005B5 RID: 1461
		private EventManagementPage eventManagement;

		// Token: 0x040005B6 RID: 1462
		private OperationalEvent operationalEventToEdit;

		// Token: 0x040005B7 RID: 1463
		private SolidColorBrush colorBrushTheme;

		// Token: 0x040005B8 RID: 1464
		private static int textLenght;
	}
}
