using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using KingAIPhotoBoothPro.Pages;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C2 RID: 194
	public class EventArchiveItemScript : Grid
	{
		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06000A81 RID: 2689 RVA: 0x0003C98B File Offset: 0x0003AB8B
		// (set) Token: 0x06000A82 RID: 2690 RVA: 0x0003C993 File Offset: 0x0003AB93
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
					this.txtName.Content = this.declaredOperationalEvent.EventName;
				}
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06000A83 RID: 2691 RVA: 0x0003C9C2 File Offset: 0x0003ABC2
		// (set) Token: 0x06000A84 RID: 2692 RVA: 0x0003C9D4 File Offset: 0x0003ABD4
		public string btnReUseXID
		{
			get
			{
				return (string)base.GetValue(EventArchiveItemScript.btnReUseProperty);
			}
			set
			{
				base.SetValue(EventArchiveItemScript.btnReUseProperty, this.btnReUseXID);
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06000A85 RID: 2693 RVA: 0x0003C9E7 File Offset: 0x0003ABE7
		// (set) Token: 0x06000A86 RID: 2694 RVA: 0x0003C9F9 File Offset: 0x0003ABF9
		public string btnDeleteXID
		{
			get
			{
				return (string)base.GetValue(EventArchiveItemScript.btnDeleteProperty);
			}
			set
			{
				base.SetValue(EventArchiveItemScript.btnDeleteProperty, this.btnDeleteXID);
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06000A87 RID: 2695 RVA: 0x0003CA0C File Offset: 0x0003AC0C
		// (set) Token: 0x06000A88 RID: 2696 RVA: 0x0003CA1E File Offset: 0x0003AC1E
		public string imgBackgroundXID
		{
			get
			{
				return (string)base.GetValue(EventArchiveItemScript.imgBackgroundProperty);
			}
			set
			{
				base.SetValue(EventArchiveItemScript.imgBackgroundProperty, this.imgBackgroundXID);
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06000A89 RID: 2697 RVA: 0x0003CA31 File Offset: 0x0003AC31
		// (set) Token: 0x06000A8A RID: 2698 RVA: 0x0003CA43 File Offset: 0x0003AC43
		public string txtNameXID
		{
			get
			{
				return (string)base.GetValue(EventArchiveItemScript.txtNameProperty);
			}
			set
			{
				base.SetValue(EventArchiveItemScript.txtNameProperty, this.txtNameXID);
			}
		}

		// Token: 0x06000A8B RID: 2699 RVA: 0x0003CA58 File Offset: 0x0003AC58
		private T FindUidButton<T>(DependencyObject parent, string uid) where T : Button
		{
			int count = VisualTreeHelper.GetChildrenCount(parent);
			if (count == 0)
			{
				return default(T);
			}
			for (int i = 0; i < count; i++)
			{
				int Childcount = VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(parent, i));
				T el = VisualTreeHelper.GetChild(parent, i) as T;
				if (el == null)
				{
					if (Childcount > 0)
					{
						el = this.FindUidButton<T>(VisualTreeHelper.GetChild(parent, i), uid);
						if (el != null)
						{
							return el;
						}
					}
				}
				else if (el.Uid == uid)
				{
					return el;
				}
			}
			return default(T);
		}

		// Token: 0x06000A8C RID: 2700 RVA: 0x0003CAF0 File Offset: 0x0003ACF0
		private T FindUidImage<T>(DependencyObject parent, string uid) where T : Image
		{
			int count = VisualTreeHelper.GetChildrenCount(parent);
			if (count == 0)
			{
				return default(T);
			}
			for (int i = 0; i < count; i++)
			{
				int Childcount = VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(parent, i));
				T el = VisualTreeHelper.GetChild(parent, i) as T;
				if (el == null)
				{
					if (Childcount > 0)
					{
						el = this.FindUidImage<T>(VisualTreeHelper.GetChild(parent, i), uid);
						if (el != null)
						{
							return el;
						}
					}
				}
				else if (el.Uid == uid)
				{
					return el;
				}
			}
			return default(T);
		}

		// Token: 0x06000A8D RID: 2701 RVA: 0x0003CB88 File Offset: 0x0003AD88
		private T FindUidLabel<T>(DependencyObject parent, string uid) where T : Label
		{
			int count = VisualTreeHelper.GetChildrenCount(parent);
			if (count == 0)
			{
				return default(T);
			}
			for (int i = 0; i < count; i++)
			{
				int Childcount = VisualTreeHelper.GetChildrenCount(VisualTreeHelper.GetChild(parent, i));
				T el = VisualTreeHelper.GetChild(parent, i) as T;
				if (el == null)
				{
					if (Childcount > 0)
					{
						el = this.FindUidLabel<T>(VisualTreeHelper.GetChild(parent, i), uid);
						if (el != null)
						{
							return el;
						}
					}
				}
				else if (el.Uid == uid)
				{
					return el;
				}
			}
			return default(T);
		}

		// Token: 0x06000A8E RID: 2702 RVA: 0x0003CC1E File Offset: 0x0003AE1E
		public void SetOperationalEvent(OperationalEvent newEvent)
		{
			this.DeclaredOperationalEvent = newEvent;
		}

		// Token: 0x06000A8F RID: 2703 RVA: 0x0003CC28 File Offset: 0x0003AE28
		public void Start()
		{
			this.btnReUse = this.FindUidButton<Button>(this, this.btnReUseXID);
			this.btnDelete = this.FindUidButton<Button>(this, this.btnDeleteXID);
			this.imgBackground = this.FindUidImage<Image>(this, this.imgBackgroundXID);
			this.txtName = this.FindUidLabel<Label>(this, this.txtNameXID);
			this.btnReUse.Click += this.BtnReUseClicked;
			this.btnDelete.Click += this.BtnDeleteClicked;
			this.eventManagement = SettingsPage.eventManagementPage;
		}

		// Token: 0x06000A90 RID: 2704 RVA: 0x0003CCBA File Offset: 0x0003AEBA
		public void LoadData()
		{
			this.txtName.Content = this.declaredOperationalEvent.EventName;
		}

		// Token: 0x06000A91 RID: 2705 RVA: 0x0003CCD2 File Offset: 0x0003AED2
		private void BtnDeleteClicked(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000A92 RID: 2706 RVA: 0x0003CCD4 File Offset: 0x0003AED4
		private void BtnReUseClicked(object sender, RoutedEventArgs e)
		{
			this.eventManagement.ReUseArchivedEvent(this.DeclaredOperationalEvent);
		}

		// Token: 0x04000A32 RID: 2610
		private OperationalEvent declaredOperationalEvent;

		// Token: 0x04000A33 RID: 2611
		public Button btnReUse;

		// Token: 0x04000A34 RID: 2612
		public Button btnDelete;

		// Token: 0x04000A35 RID: 2613
		private Image imgBackground;

		// Token: 0x04000A36 RID: 2614
		private Label txtName;

		// Token: 0x04000A37 RID: 2615
		private EventManagementPage eventManagement;

		// Token: 0x04000A38 RID: 2616
		public static readonly DependencyProperty btnReUseProperty = DependencyProperty.Register("btnReUseXID", typeof(string), typeof(EventArchiveItemScript));

		// Token: 0x04000A39 RID: 2617
		public static readonly DependencyProperty btnDeleteProperty = DependencyProperty.Register("btnDeleteXID", typeof(string), typeof(EventArchiveItemScript));

		// Token: 0x04000A3A RID: 2618
		public static readonly DependencyProperty imgBackgroundProperty = DependencyProperty.Register("imgBackgroundXID", typeof(string), typeof(EventArchiveItemScript));

		// Token: 0x04000A3B RID: 2619
		public static readonly DependencyProperty txtNameProperty = DependencyProperty.Register("txtNameXID", typeof(string), typeof(EventArchiveItemScript));
	}
}
