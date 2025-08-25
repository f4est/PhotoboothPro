using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using KingAIPhotoBoothPro.Class;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x0200005A RID: 90
	public partial class PhotoPrefab : Page
	{
		// Token: 0x06000702 RID: 1794 RVA: 0x000302A1 File Offset: 0x0002E4A1
		public PhotoPrefab(string photoIconName)
		{
			this.InitializeComponent();
			this.photoIconName = photoIconName;
		}

		// Token: 0x06000703 RID: 1795 RVA: 0x000302B6 File Offset: 0x0002E4B6
		public int AllowedMaxPhotoID()
		{
			return (from x in TemplateClass.TemplateObjectsPhoto
			where x.isCapture
			select x).Count<TemplateObject>();
		}

		// Token: 0x06000704 RID: 1796 RVA: 0x000302E8 File Offset: 0x0002E4E8
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			Uri imageUri = new Uri("pack://application:,,,/Images/" + this.photoIconName, UriKind.Absolute);
			BitmapImage image = new BitmapImage();
			image.BeginInit();
			image.CacheOption = BitmapCacheOption.OnLoad;
			image.UriSource = imageUri;
			image.EndInit();
			image.Freeze();
			this.photoImage.Source = image;
			this.photoText.Text = this.photoID.ToString();
		}

		// Token: 0x06000705 RID: 1797 RVA: 0x00030354 File Offset: 0x0002E554
		private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
		{
		}

		// Token: 0x06000706 RID: 1798 RVA: 0x00030358 File Offset: 0x0002E558
		private void IncreaseButton_Click(object sender, RoutedEventArgs e)
		{
			int id = int.Parse(this.photoText.Text);
			id = Math.Min(id + 1, this.AllowedMaxPhotoID());
			this.photoID = id;
			this.photoText.Text = id.ToString();
			this.photoIDChanged(id, base.Uid);
		}

		// Token: 0x06000707 RID: 1799 RVA: 0x000303B0 File Offset: 0x0002E5B0
		private void DecreaseButton_Click(object sender, RoutedEventArgs e)
		{
			int id = int.Parse(this.photoText.Text);
			if (id == 1)
			{
				return;
			}
			id--;
			this.photoID = id;
			this.photoText.Text = id.ToString();
			this.photoIDChanged(id, base.Uid);
		}

		// Token: 0x06000708 RID: 1800 RVA: 0x00030402 File Offset: 0x0002E602
		public void PhotoIDChangedTemplatePage(int photoId, string uId)
		{
			if (base.Uid == uId)
			{
				this.photoID = photoId + 1;
				this.photoText.Text = this.photoID.ToString();
			}
		}

		// Token: 0x040007A9 RID: 1961
		public Action<int, string> photoIDChanged;

		// Token: 0x040007AA RID: 1962
		public int photoID;

		// Token: 0x040007AB RID: 1963
		public string photoIconName;
	}
}
