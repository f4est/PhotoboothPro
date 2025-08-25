using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Class.ActivationKing;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x0200005E RID: 94
	public partial class EventPrefab : Page
	{
		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x06000754 RID: 1876 RVA: 0x00031741 File Offset: 0x0002F941
		// (set) Token: 0x06000755 RID: 1877 RVA: 0x00031749 File Offset: 0x0002F949
		public OperationalEvent DeclaredOperationalEvent
		{
			get
			{
				return this.declaredOperationalEvent;
			}
			set
			{
				this.declaredOperationalEvent = value;
				if (this.declaredOperationalEvent != null && this.txtName != null)
				{
					this.txtName.Text = this.declaredOperationalEvent.EventName;
				}
			}
		}

		// Token: 0x06000756 RID: 1878 RVA: 0x00031778 File Offset: 0x0002F978
		public void SetOperationalEvent(OperationalEvent newEvent)
		{
			this.DeclaredOperationalEvent = newEvent;
		}

		// Token: 0x06000757 RID: 1879 RVA: 0x00031781 File Offset: 0x0002F981
		public EventPrefab()
		{
			this.InitializeComponent();
		}

		// Token: 0x06000758 RID: 1880 RVA: 0x0003178F File Offset: 0x0002F98F
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.eventManagement = SettingsPage.eventManagementPage;
			this.LoadData();
		}

		// Token: 0x06000759 RID: 1881 RVA: 0x000317A4 File Offset: 0x0002F9A4
		public void LoadData()
		{
			this.txtName.Text = this.declaredOperationalEvent.EventName;
			this.txtEventCreatedTime.Text = this.declaredOperationalEvent.CreateTime.ToString("dd MMMM yyyy");
			if (!string.IsNullOrEmpty(this.declaredOperationalEvent.EventMotto))
			{
				base.ToolTip = "Motto: " + this.declaredOperationalEvent.EventMotto;
			}
		}

		// Token: 0x0600075A RID: 1882 RVA: 0x00031814 File Offset: 0x0002FA14
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.eventManagement.ReUseArchivedEvent(this.DeclaredOperationalEvent);
		}

		// Token: 0x0600075B RID: 1883 RVA: 0x00031827 File Offset: 0x0002FA27
		private void ButtonMedias_Click(object sender, RoutedEventArgs e)
		{
			ExtensionMethod.OpenUrl("https://activationshare.com/Content/EventMedias/" + this.declaredOperationalEvent.IndexID);
		}

		// Token: 0x040007EE RID: 2030
		private OperationalEvent declaredOperationalEvent;

		// Token: 0x040007EF RID: 2031
		private EventManagementPage eventManagement;
	}
}
