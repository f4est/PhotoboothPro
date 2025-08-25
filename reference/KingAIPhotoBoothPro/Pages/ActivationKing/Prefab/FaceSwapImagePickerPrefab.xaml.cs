using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Amazon.Rekognition.Model;
using KingAIPhotoBoothPro.Class.UI;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000052 RID: 82
	public partial class FaceSwapImagePickerPrefab : Page
	{
		// Token: 0x1700008D RID: 141
		// (get) Token: 0x06000683 RID: 1667 RVA: 0x0002E8C1 File Offset: 0x0002CAC1
		// (set) Token: 0x06000684 RID: 1668 RVA: 0x0002E8C9 File Offset: 0x0002CAC9
		public string txtTitle { get; set; }

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06000685 RID: 1669 RVA: 0x0002E8D4 File Offset: 0x0002CAD4
		// (remove) Token: 0x06000686 RID: 1670 RVA: 0x0002E908 File Offset: 0x0002CB08
		public static event Action<FaceSwapImagePickerPrefab> ImageLoaded;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06000687 RID: 1671 RVA: 0x0002E93C File Offset: 0x0002CB3C
		// (remove) Token: 0x06000688 RID: 1672 RVA: 0x0002E970 File Offset: 0x0002CB70
		public static event Action<string, int> titleChanged;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06000689 RID: 1673 RVA: 0x0002E9A4 File Offset: 0x0002CBA4
		// (remove) Token: 0x0600068A RID: 1674 RVA: 0x0002E9D8 File Offset: 0x0002CBD8
		public static event Action<FaceSwapImagePickerPrefab> DeleteClicked;

		// Token: 0x1700008E RID: 142
		// (get) Token: 0x0600068B RID: 1675 RVA: 0x0002EA0B File Offset: 0x0002CC0B
		public bool IsLoading
		{
			get
			{
				return this.LoadingContainer.Visibility == Visibility.Visible;
			}
		}

		// Token: 0x1700008F RID: 143
		// (get) Token: 0x0600068C RID: 1676 RVA: 0x0002EA1B File Offset: 0x0002CC1B
		public bool IsEmpty
		{
			get
			{
				return this.FaceSwapTargetImage == null;
			}
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x0002EA28 File Offset: 0x0002CC28
		public FaceSwapImagePickerPrefab(int index, ref FaceSwapTarget faceSwapTarget)
		{
			this.FaceSwapTargetImage = faceSwapTarget;
			this.faceBorderlist = new List<FaceSelectionElement>();
			FaceSwapTarget faceSwapTarget2 = faceSwapTarget;
			this.txtTitle = ((faceSwapTarget2 != null) ? faceSwapTarget2.Title : null);
			this.index = index;
			this.InitializeComponent();
			this.NewPrefabGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x0002EA7B File Offset: 0x0002CC7B
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.AdjustCanvasToImageBounds();
			this.prefabImage.Loaded += delegate(object s, RoutedEventArgs ev)
			{
				this.AdjustCanvasToImageBounds();
			};
			this.prefabImage.SizeChanged += delegate(object s, SizeChangedEventArgs ev)
			{
				this.AdjustCanvasToImageBounds();
			};
			this.Load();
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0002EAB7 File Offset: 0x0002CCB7
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.prefabImage.Loaded -= delegate(object s, RoutedEventArgs ev)
			{
				this.AdjustCanvasToImageBounds();
			};
			this.prefabImage.SizeChanged -= delegate(object s, SizeChangedEventArgs ev)
			{
				this.AdjustCanvasToImageBounds();
			};
			this.prefabImage.Source = null;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x0002EAF4 File Offset: 0x0002CCF4
		private void SelectFaceSwapFile()
		{
			FaceSwapImagePickerPrefab.<SelectFaceSwapFile>d__23 <SelectFaceSwapFile>d__;
			<SelectFaceSwapFile>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SelectFaceSwapFile>d__.<>4__this = this;
			<SelectFaceSwapFile>d__.<>1__state = -1;
			<SelectFaceSwapFile>d__.<>t__builder.Start<FaceSwapImagePickerPrefab.<SelectFaceSwapFile>d__23>(ref <SelectFaceSwapFile>d__);
		}

		// Token: 0x06000691 RID: 1681 RVA: 0x0002EB2C File Offset: 0x0002CD2C
		public Task AddFromMultiSelect(string filepath)
		{
			FaceSwapImagePickerPrefab.<AddFromMultiSelect>d__24 <AddFromMultiSelect>d__;
			<AddFromMultiSelect>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<AddFromMultiSelect>d__.<>4__this = this;
			<AddFromMultiSelect>d__.filepath = filepath;
			<AddFromMultiSelect>d__.<>1__state = -1;
			<AddFromMultiSelect>d__.<>t__builder.Start<FaceSwapImagePickerPrefab.<AddFromMultiSelect>d__24>(ref <AddFromMultiSelect>d__);
			return <AddFromMultiSelect>d__.<>t__builder.Task;
		}

		// Token: 0x06000692 RID: 1682 RVA: 0x0002EB78 File Offset: 0x0002CD78
		private Task<bool> CreateFaceSwapTarget(string selectedFilePath)
		{
			FaceSwapImagePickerPrefab.<CreateFaceSwapTarget>d__25 <CreateFaceSwapTarget>d__;
			<CreateFaceSwapTarget>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<CreateFaceSwapTarget>d__.<>4__this = this;
			<CreateFaceSwapTarget>d__.selectedFilePath = selectedFilePath;
			<CreateFaceSwapTarget>d__.<>1__state = -1;
			<CreateFaceSwapTarget>d__.<>t__builder.Start<FaceSwapImagePickerPrefab.<CreateFaceSwapTarget>d__25>(ref <CreateFaceSwapTarget>d__);
			return <CreateFaceSwapTarget>d__.<>t__builder.Task;
		}

		// Token: 0x06000693 RID: 1683 RVA: 0x0002EBC3 File Offset: 0x0002CDC3
		private void ShowAddNewButton()
		{
			this.LoadingContainer.Visibility = Visibility.Hidden;
			this.NewPrefabGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x06000694 RID: 1684 RVA: 0x0002EBDD File Offset: 0x0002CDDD
		private void ShowLoading()
		{
			this.LoadingContainer.Visibility = Visibility.Visible;
			this.NewPrefabGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x06000695 RID: 1685 RVA: 0x0002EBF7 File Offset: 0x0002CDF7
		private void ShowContent()
		{
			this.LoadingContainer.Visibility = Visibility.Hidden;
			this.NewPrefabGrid.Visibility = Visibility.Hidden;
		}

		// Token: 0x06000696 RID: 1686 RVA: 0x0002EC11 File Offset: 0x0002CE11
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			this.SelectFaceSwapFile();
		}

		// Token: 0x06000697 RID: 1687 RVA: 0x0002EC1C File Offset: 0x0002CE1C
		private Task LoadImage()
		{
			FaceSwapImagePickerPrefab.<LoadImage>d__30 <LoadImage>d__;
			<LoadImage>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadImage>d__.<>4__this = this;
			<LoadImage>d__.<>1__state = -1;
			<LoadImage>d__.<>t__builder.Start<FaceSwapImagePickerPrefab.<LoadImage>d__30>(ref <LoadImage>d__);
			return <LoadImage>d__.<>t__builder.Task;
		}

		// Token: 0x06000698 RID: 1688 RVA: 0x0002EC60 File Offset: 0x0002CE60
		private void FaceSelectionChanged(FaceSelectionElement element, bool IsSelected)
		{
			FaceSwapImagePickerPrefab.<FaceSelectionChanged>d__31 <FaceSelectionChanged>d__;
			<FaceSelectionChanged>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<FaceSelectionChanged>d__.<>4__this = this;
			<FaceSelectionChanged>d__.element = element;
			<FaceSelectionChanged>d__.IsSelected = IsSelected;
			<FaceSelectionChanged>d__.<>1__state = -1;
			<FaceSelectionChanged>d__.<>t__builder.Start<FaceSwapImagePickerPrefab.<FaceSelectionChanged>d__31>(ref <FaceSelectionChanged>d__);
		}

		// Token: 0x06000699 RID: 1689 RVA: 0x0002ECA7 File Offset: 0x0002CEA7
		public void Load()
		{
			this.txtFaceSwapTitle.Text = this.txtTitle;
			this.LoadImage();
			this.AdjustCanvasToImageBounds();
		}

		// Token: 0x0600069A RID: 1690 RVA: 0x0002ECC7 File Offset: 0x0002CEC7
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			FaceSwapImagePickerPrefab.DeleteClicked(this);
		}

		// Token: 0x0600069B RID: 1691 RVA: 0x0002ECD4 File Offset: 0x0002CED4
		private void txtFaceSwapTitle_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (this.txtFaceSwapTitle.Text == "Write Title")
			{
				return;
			}
			this.txtTitle = this.txtFaceSwapTitle.Text;
			FaceSwapImagePickerPrefab.titleChanged(this.txtFaceSwapTitle.Text, this.index);
		}

		// Token: 0x0600069C RID: 1692 RVA: 0x0002ED25 File Offset: 0x0002CF25
		private void AddNewButton_Click(object sender, RoutedEventArgs e)
		{
			this.SelectFaceSwapFile();
		}

		// Token: 0x0600069D RID: 1693 RVA: 0x0002ED30 File Offset: 0x0002CF30
		private FaceSelectionElement AddDynamicBorderToImage(FaceDetail faceDetail, int faceIndex, bool isDisabled)
		{
			FaceSelectionElement dynamicBorder = new FaceSelectionElement(faceIndex, isDisabled)
			{
				Background = Brushes.Transparent,
				Width = this.imageCanvas.ActualWidth * (double)faceDetail.BoundingBox.Width,
				Height = this.imageCanvas.ActualHeight * (double)faceDetail.BoundingBox.Height
			};
			Canvas.SetLeft(dynamicBorder, this.imageCanvas.ActualWidth * (double)faceDetail.BoundingBox.Left);
			Canvas.SetTop(dynamicBorder, this.imageCanvas.ActualHeight * (double)faceDetail.BoundingBox.Top);
			this.imageCanvas.Children.Add(dynamicBorder);
			return dynamicBorder;
		}

		// Token: 0x0600069E RID: 1694 RVA: 0x0002EDDC File Offset: 0x0002CFDC
		private void SetDynamicBorderToImage(FaceSelectionElement dynamicBorder, FaceDetail faceDetail, float offset = 0.05f)
		{
			dynamicBorder.Width = this.imageCanvas.ActualWidth * (double)Math.Min(1f, faceDetail.BoundingBox.Width + offset * 2f);
			dynamicBorder.Height = this.imageCanvas.ActualHeight * (double)Math.Min(1f, faceDetail.BoundingBox.Height + offset * 2f);
			Canvas.SetLeft(dynamicBorder, this.imageCanvas.ActualWidth * (double)Math.Max(0f, faceDetail.BoundingBox.Left - offset));
			Canvas.SetTop(dynamicBorder, this.imageCanvas.ActualHeight * (double)Math.Max(0f, faceDetail.BoundingBox.Top - offset));
		}

		// Token: 0x0600069F RID: 1695 RVA: 0x0002EEA0 File Offset: 0x0002D0A0
		private void AdjustCanvasToImageBounds()
		{
			if (this.prefabImage.Source == null)
			{
				return;
			}
			if (!(this.prefabImage.Source is BitmapSource))
			{
				return;
			}
			double imageActualWidth = this.prefabImage.ActualWidth;
			double imageActualHeight = this.prefabImage.ActualHeight;
			System.Windows.Point imagePosition = this.prefabImage.TransformToAncestor(this.imageGrid).Transform(new System.Windows.Point(0.0, 0.0));
			Canvas.SetLeft(this.imageCanvas, imagePosition.X);
			Canvas.SetTop(this.imageCanvas, imagePosition.Y);
			this.imageCanvas.Width = imageActualWidth;
			this.imageCanvas.Height = imageActualHeight;
			Task.Run(delegate()
			{
				FaceSwapImagePickerPrefab.<<AdjustCanvasToImageBounds>b__38_0>d <<AdjustCanvasToImageBounds>b__38_0>d;
				<<AdjustCanvasToImageBounds>b__38_0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<AdjustCanvasToImageBounds>b__38_0>d.<>4__this = this;
				<<AdjustCanvasToImageBounds>b__38_0>d.<>1__state = -1;
				<<AdjustCanvasToImageBounds>b__38_0>d.<>t__builder.Start<FaceSwapImagePickerPrefab.<<AdjustCanvasToImageBounds>b__38_0>d>(ref <<AdjustCanvasToImageBounds>b__38_0>d);
				return <<AdjustCanvasToImageBounds>b__38_0>d.<>t__builder.Task;
			});
		}

		// Token: 0x060006A0 RID: 1696 RVA: 0x0002EF64 File Offset: 0x0002D164
		private void OnImageSourceChanged()
		{
			this.AdjustCanvasToImageBounds();
		}

		// Token: 0x0400074A RID: 1866
		public int index;

		// Token: 0x0400074E RID: 1870
		public FaceSwapTarget FaceSwapTargetImage;

		// Token: 0x0400074F RID: 1871
		private List<FaceSelectionElement> faceBorderlist;
	}
}
