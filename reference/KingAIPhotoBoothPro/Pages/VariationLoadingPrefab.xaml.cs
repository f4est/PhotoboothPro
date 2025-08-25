using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Variations;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000033 RID: 51
	public partial class VariationLoadingPrefab : Page
	{
		// Token: 0x1700006B RID: 107
		// (get) Token: 0x060002D8 RID: 728 RVA: 0x0000EB27 File Offset: 0x0000CD27
		public Frame Frame
		{
			get
			{
				return this.frame;
			}
		}

		// Token: 0x060002D9 RID: 729 RVA: 0x0000EB30 File Offset: 0x0000CD30
		public VariationLoadingPrefab(VariationMedia variation, Frame frame)
		{
			VariationLoadingPrefab <>4__this = this;
			this.InitializeComponent();
			this.variationMedia = variation;
			this.VariationLocalId = variation.LocalId;
			this.frame = frame;
			this.imgBackground.Source = this.imgBackground.Source;
			this.txtTitle.Dispatcher.Invoke<string>(() => <>4__this.txtTitle.Text = variation.VariationOperation.GetDisplayName());
			if (File.Exists(variation.PreviewImagePath))
			{
				VariationLoadingPrefab.bitmapPhoto = new BitmapImage();
				VariationLoadingPrefab.bitmapPhoto.BeginInit();
				VariationLoadingPrefab.bitmapPhoto.UriSource = new Uri(variation.PreviewImagePath, UriKind.Absolute);
				VariationLoadingPrefab.bitmapPhoto.EndInit();
				this.imgBackground.Source = VariationLoadingPrefab.bitmapPhoto;
			}
			this.cancellationTokenSource = new CancellationTokenSource();
			Task.Run(() => <>4__this.Update(<>4__this.cancellationTokenSource.Token), this.cancellationTokenSource.Token);
		}

		// Token: 0x1700006C RID: 108
		// (get) Token: 0x060002DA RID: 730 RVA: 0x0000EC37 File Offset: 0x0000CE37
		// (set) Token: 0x060002DB RID: 731 RVA: 0x0000EC3F File Offset: 0x0000CE3F
		public string VariationLocalId { get; set; }

		// Token: 0x060002DC RID: 732 RVA: 0x0000EC48 File Offset: 0x0000CE48
		private Task Update(CancellationToken cancellationToken)
		{
			VariationLoadingPrefab.<Update>d__11 <Update>d__;
			<Update>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Update>d__.<>4__this = this;
			<Update>d__.cancellationToken = cancellationToken;
			<Update>d__.<>1__state = -1;
			<Update>d__.<>t__builder.Start<VariationLoadingPrefab.<Update>d__11>(ref <Update>d__);
			return <Update>d__.<>t__builder.Task;
		}

		// Token: 0x060002DD RID: 733 RVA: 0x0000EC94 File Offset: 0x0000CE94
		public void Destroy()
		{
			BitmapImage bitmapImage = this.imgBackground.Source as BitmapImage;
			if (bitmapImage != null)
			{
				Stream streamSource = bitmapImage.StreamSource;
				if (streamSource != null)
				{
					streamSource.Dispose();
				}
				bitmapImage.StreamSource = null;
			}
			this.imgBackground.Source = null;
			GC.Collect();
			GC.WaitForPendingFinalizers();
			if (this.cancellationTokenSource != null)
			{
				this.cancellationTokenSource.Cancel();
				this.cancellationTokenSource.Dispose();
			}
		}

		// Token: 0x0400027F RID: 639
		private readonly VariationMedia variationMedia;

		// Token: 0x04000280 RID: 640
		private readonly Frame frame;

		// Token: 0x04000281 RID: 641
		private CancellationTokenSource cancellationTokenSource;

		// Token: 0x04000282 RID: 642
		public static BitmapImage bitmapPhoto;
	}
}
