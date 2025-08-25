using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageMagick;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000046 RID: 70
	public partial class TemplatePageVideo : SettingsSubPage, IStyleConnector
	{
		// Token: 0x06000544 RID: 1348 RVA: 0x000210FD File Offset: 0x0001F2FD
		public static TemplatePageVideo GetTemplatePage()
		{
			return TemplatePageVideo.thisTemplatePage;
		}

		// Token: 0x06000545 RID: 1349 RVA: 0x00021104 File Offset: 0x0001F304
		public TemplatePageVideo()
		{
			this.InitializeComponent();
			this.canvasList = new List<Canvas>
			{
				this.canvas1,
				this.canvas2,
				this.canvas3,
				this.canvas4,
				this.canvas5,
				this.canvas6
			};
			App.ApplicationExit += this.App_ApplicationExit;
			TemplatePageVideo.thisTemplatePage = this;
		}

		// Token: 0x06000546 RID: 1350 RVA: 0x0002122F File Offset: 0x0001F42F
		private void App_ApplicationExit(object sender, EventArgs e)
		{
			TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasVideoID);
		}

		// Token: 0x06000547 RID: 1351 RVA: 0x0002123B File Offset: 0x0001F43B
		public override string GetTitle()
		{
			return "Video Template Settings";
		}

		// Token: 0x06000548 RID: 1352 RVA: 0x00021242 File Offset: 0x0001F442
		public override string GetSubTitle()
		{
			return "These settings will be used in the output of your Video and Gif 360 content.";
		}

		// Token: 0x06000549 RID: 1353 RVA: 0x00021249 File Offset: 0x0001F449
		public void RefreshTemplate()
		{
			this.LastSize = new System.Windows.Size(0.0, 0.0);
		}

		// Token: 0x0600054A RID: 1354 RVA: 0x00021268 File Offset: 0x0001F468
		private void SaveCanvasAsJpg(Canvas canvas, string filePath, int quality = 90)
		{
			int heightCanvas = 0;
			int widthCanvas = 0;
			if (this.isPortraitScreen)
			{
				heightCanvas = (int)this.GridCanvasTab.ActualHeight;
			}
			else
			{
				widthCanvas = (int)this.GridCanvasTab.ActualWidth;
			}
			int width = (int)canvas.ActualWidth;
			int height = (int)canvas.ActualHeight;
			if (width == 0 || height == 0)
			{
				return;
			}
			int ID = (from x in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects
			where x.isCapture
			select x).FirstOrDefault<TemplateObject>().layer;
			UIElement captureElement = null;
			if (canvas.Children.Count > ID)
			{
				captureElement = canvas.Children[ID];
				if (captureElement != null)
				{
					captureElement.Visibility = Visibility.Hidden;
				}
			}
			RenderTargetBitmap renderBitmap;
			if (this.isPortraitScreen)
			{
				renderBitmap = new RenderTargetBitmap(width, heightCanvas, 96.0, 96.0, PixelFormats.Pbgra32);
			}
			else
			{
				renderBitmap = new RenderTargetBitmap(widthCanvas, height, 96.0, 96.0, PixelFormats.Pbgra32);
			}
			renderBitmap.Render(canvas);
			CroppedBitmap croppedBitmap;
			if (this.isPortraitScreen)
			{
				int addy = Math.Min((heightCanvas - height) / 2, 5);
				croppedBitmap = new CroppedBitmap(renderBitmap, new Int32Rect(0, (heightCanvas - height) / 2 - addy, width, height));
			}
			else
			{
				int addx = Math.Min((widthCanvas - width) / 2, 5);
				croppedBitmap = new CroppedBitmap(renderBitmap, new Int32Rect((widthCanvas - width) / 2 - addx, 0, width, height));
			}
			JpegBitmapEncoder encoder = new JpegBitmapEncoder
			{
				QualityLevel = quality
			};
			encoder.Frames.Add(BitmapFrame.Create(croppedBitmap));
			try
			{
				if (File.Exists(filePath))
				{
					File.Delete(filePath);
				}
			}
			catch (Exception ex)
			{
				return;
			}
			using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
			{
				encoder.Save(fileStream);
			}
			if (captureElement != null)
			{
				captureElement.Visibility = Visibility.Visible;
			}
		}

		// Token: 0x0600054B RID: 1355 RVA: 0x0002144C File Offset: 0x0001F64C
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			TemplatePageVideo.<Page_Loaded>d__42 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<TemplatePageVideo.<Page_Loaded>d__42>(ref <Page_Loaded>d__);
		}

		// Token: 0x0600054C RID: 1356 RVA: 0x00021483 File Offset: 0x0001F683
		private void InitialSelection()
		{
			if (this.isPortraitScreen)
			{
				this.imageListBoxPortrait.SelectedIndex = 0;
				return;
			}
			this.imageListBox.SelectedIndex = 0;
		}

		// Token: 0x0600054D RID: 1357 RVA: 0x000214A8 File Offset: 0x0001F6A8
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			this.SaveCanvasAsJpg(this.canvasList[TemplateClass.selectedCanvasVideoID], TemplateClass.GetTemplateVideoGalleryPhotoPath(TemplateClass.selectedCanvasVideoID), 90);
			TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasVideoID);
			TemplateClass.ReadJsonTemplateObjects(false);
			this.imageNameList.Clear();
			this.ImageIDList.Clear();
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
		}

		// Token: 0x0600054E RID: 1358 RVA: 0x00021518 File Offset: 0x0001F718
		private void SetCanvasSize(double width, double height)
		{
			this.canvasScale = Math.Min(this.canvasInitialWidth / width, this.canvasInitialHeight / height);
			this.canvasList[TemplateClass.selectedCanvasVideoID].Width = this.canvasScale * width;
			this.canvasList[TemplateClass.selectedCanvasVideoID].Height = this.canvasScale * height;
		}

		// Token: 0x0600054F RID: 1359 RVA: 0x0002157E File Offset: 0x0001F77E
		private void ClearTemplate(object sender, RoutedEventArgs e)
		{
			TemplateClass.TemplateObjectsVideo.Clear();
			this.imageNameList.Clear();
			this.ImageIDList.Clear();
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
		}

		// Token: 0x06000550 RID: 1360 RVA: 0x000215BC File Offset: 0x0001F7BC
		private void DeleteElement(object sender, RoutedEventArgs e)
		{
			this.UpdatePreviousMoves();
			UIElement element = null;
			if (this.lastSelectedElement == null)
			{
				string selectedElementID;
				if (this.isPortraitScreen)
				{
					if (this.imageListBoxPortrait.SelectedIndex < 0)
					{
						return;
					}
					selectedElementID = this.imageListBoxPortrait.SelectedIndex.ToString();
				}
				else
				{
					if (this.imageListBox.SelectedIndex < 0)
					{
						return;
					}
					selectedElementID = this.imageListBox.SelectedIndex.ToString();
				}
				element = this.GetElementByID(selectedElementID);
			}
			else
			{
				element = this.lastSelectedElement;
			}
			if (element == null)
			{
				return;
			}
			TemplateObject delElement = TemplateClass.TemplateObjectsVideo.Find((TemplateObject x) => x.ID == Convert.ToInt32(element.Uid));
			if (delElement.isCapture)
			{
				new MessageBoxWindow("MAIN PHOTO ICON", "You Cannot Remove This Photo! Because it is the main photo icon.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			TemplateClass.TemplateObjectsVideo.Remove(delElement);
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(element);
			this.lastSelectedElement = null;
			this.imageNameList.RemoveAt(delElement.layer);
			this.ImageIDList.RemoveAt(delElement.layer);
			this.SyncImageDeleteListBox();
			this.SyncImageDeleteListBoxPortrait();
			this.imageListBox.SelectedIndex = -1;
			this.imageListBoxPortrait.SelectedIndex = -1;
			this.ClearButtonsAndRect();
			this.deletedElements.Add(element);
			foreach (TemplateObject templateObj in TemplateClass.TemplateObjectsVideo)
			{
				if (templateObj.layer > delElement.layer)
				{
					templateObj.layer--;
				}
			}
			this.UpdateUndoButton();
			this.UpdateRedoButton();
		}

		// Token: 0x06000551 RID: 1361 RVA: 0x00021794 File Offset: 0x0001F994
		private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.UpdatePreviousMoves();
				this.previousMousePosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]);
				System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
				button.CaptureMouse();
				if (button.Name != "buttonMove")
				{
					this.isResizing = true;
				}
			}
		}

		// Token: 0x06000552 RID: 1362 RVA: 0x000217F4 File Offset: 0x0001F9F4
		private void Button_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
			if (button.Name == "buttonTopLeft")
			{
				base.Cursor = System.Windows.Input.Cursors.SizeNWSE;
			}
			else if (button.Name == "buttonBottomRight")
			{
				base.Cursor = System.Windows.Input.Cursors.SizeNWSE;
			}
			else if (button.Name == "buttonTopRight")
			{
				base.Cursor = System.Windows.Input.Cursors.SizeNESW;
			}
			else if (button.Name == "buttonBottomLeft")
			{
				base.Cursor = System.Windows.Input.Cursors.SizeNESW;
			}
			else if (button.Name == "buttonMove")
			{
				base.Cursor = System.Windows.Input.Cursors.ScrollAll;
			}
			if (this.isResizing && this.lastSelectedElement != null)
			{
				double oldWidth = ((FrameworkElement)this.lastSelectedElement).Width;
				double oldHeight = ((FrameworkElement)this.lastSelectedElement).Height;
				if (button.Name == "buttonTopLeft")
				{
					double newWidth = Math.Max(50.0, oldWidth - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).X - this.previousMousePosition.X));
					double newHeight = Math.Max(50.0, oldHeight - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).Y - this.previousMousePosition.Y));
					if (this.keepAspectRatioCheckBox.IsChecked.Value)
					{
						newHeight = newWidth / oldWidth * oldHeight;
					}
					double widthDif = newWidth - oldWidth;
					double heightDif = newHeight - oldHeight;
					Canvas.SetLeft(this.lastSelectedElement, Canvas.GetLeft(this.lastSelectedElement) - widthDif);
					Canvas.SetTop(this.lastSelectedElement, Canvas.GetTop(this.lastSelectedElement) - heightDif);
					((FrameworkElement)this.lastSelectedElement).Width = newWidth;
					((FrameworkElement)this.lastSelectedElement).Height = newHeight;
				}
				else if (button.Name == "buttonBottomRight")
				{
					double newWidth2 = Math.Max(50.0, oldWidth + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).X - this.previousMousePosition.X));
					double newHeight2 = Math.Max(50.0, oldHeight + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).Y - this.previousMousePosition.Y));
					if (this.keepAspectRatioCheckBox.IsChecked.Value)
					{
						newHeight2 = newWidth2 / oldWidth * oldHeight;
					}
					((FrameworkElement)this.lastSelectedElement).Width = newWidth2;
					((FrameworkElement)this.lastSelectedElement).Height = newHeight2;
				}
				else if (button.Name == "buttonTopRight")
				{
					double newWidth3 = Math.Max(50.0, oldWidth + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).X - this.previousMousePosition.X));
					double newHeight3 = Math.Max(50.0, oldHeight - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).Y - this.previousMousePosition.Y));
					if (this.keepAspectRatioCheckBox.IsChecked.Value)
					{
						newHeight3 = newWidth3 / oldWidth * oldHeight;
					}
					double widthDif2 = newWidth3 - oldWidth;
					double heightDif2 = newHeight3 - oldHeight;
					Canvas.SetLeft(this.lastSelectedElement, Canvas.GetLeft(this.lastSelectedElement));
					Canvas.SetTop(this.lastSelectedElement, Canvas.GetTop(this.lastSelectedElement) - heightDif2);
					((FrameworkElement)this.lastSelectedElement).Width = newWidth3;
					((FrameworkElement)this.lastSelectedElement).Height = newHeight3;
				}
				else if (button.Name == "buttonBottomLeft")
				{
					double newWidth4 = Math.Max(50.0, oldWidth - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).X - this.previousMousePosition.X));
					double newHeight4 = Math.Max(50.0, oldHeight + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]).Y - this.previousMousePosition.Y));
					if (this.keepAspectRatioCheckBox.IsChecked.Value)
					{
						newHeight4 = newWidth4 / oldWidth * oldHeight;
					}
					double widthDif3 = newWidth4 - oldWidth;
					double heightDif3 = newHeight4 - oldHeight;
					Canvas.SetLeft(this.lastSelectedElement, Canvas.GetLeft(this.lastSelectedElement) - widthDif3);
					Canvas.SetTop(this.lastSelectedElement, Canvas.GetTop(this.lastSelectedElement));
					((FrameworkElement)this.lastSelectedElement).Width = newWidth4;
					((FrameworkElement)this.lastSelectedElement).Height = newHeight4;
				}
				this.previousMousePosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]);
				this.UpdateRectPositionAndSize(this.lastSelectedElement);
				this.UpdateButtonsPositions(this.lastSelectedElement);
				this.UpdatePositionText(this.lastSelectedElement);
				this.UpdateSizeText(this.lastSelectedElement);
				this.UpdateMoveButtonPosition(this.lastSelectedElement);
			}
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00021D34 File Offset: 0x0001FF34
		private void Button_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
			button.ReleaseMouseCapture();
			this.isResizing = false;
			base.Cursor = System.Windows.Input.Cursors.Arrow;
			if (this.lastSelectedElement != null && this.lastSelectedElement.Uid != "")
			{
				this.UpdateTemplateClassImagePosition(this.lastSelectedElement);
				this.UpdateTemplateClassImageSize(this.lastSelectedElement);
			}
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x00021D97 File Offset: 0x0001FF97
		private void Button_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (this.isResizing)
			{
				return;
			}
			if (this.isMoving)
			{
				return;
			}
			base.Cursor = System.Windows.Input.Cursors.Arrow;
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x00021DB8 File Offset: 0x0001FFB8
		private void PageMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource != this.positionXTextBox && e.OriginalSource != this.positionXTextBoxPortrait && e.OriginalSource != this.positionYTextBox && e.OriginalSource != this.positionYTextBoxPortrait && e.OriginalSource != this.heightTextBox && e.OriginalSource != this.heightTextBoxPortrait && e.OriginalSource != this.widthTextBox && e.OriginalSource != this.widthTextBoxPortrait)
			{
				this.dummyButton.Focus();
			}
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x00021E44 File Offset: 0x00020044
		private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (this.isMoving || this.isResizing)
			{
				return;
			}
			this.UpdatePreviousMoves();
			if (!(sender is System.Windows.Controls.Image) && !(sender is Grid))
			{
				if (sender is System.Windows.Controls.Button)
				{
					this.isMoving = true;
					this.elementClickedPosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]);
					this.elementClickedPosition.Y = this.elementClickedPosition.Y - Canvas.GetTop(this.buttonMove);
					this.elementClickedPosition.X = this.elementClickedPosition.X - Canvas.GetLeft(this.buttonMove);
					this.canvasList[TemplateClass.selectedCanvasVideoID].CaptureMouse();
				}
				return;
			}
			this.isMoving = true;
			this.mouseDownTime = DateTime.Now;
			if (this.buttonMove != null)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.buttonMove);
				this.buttonMove = null;
			}
			this.selectedElement = (sender as UIElement);
			TemplateObject selectedObject = TemplateClass.TemplateObjectsVideo.Find((TemplateObject x) => x.ID == Convert.ToInt32(this.selectedElement.Uid));
			if (selectedObject == null)
			{
				return;
			}
			this.imageListBox.SelectedIndex = selectedObject.layer;
			this.imageListBoxPortrait.SelectedIndex = selectedObject.layer;
			this.lastSelectedElement = this.selectedElement;
			this.elementClickedPosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]);
			this.elementClickedPosition.Y = this.elementClickedPosition.Y - Canvas.GetTop(this.lastSelectedElement);
			this.elementClickedPosition.X = this.elementClickedPosition.X - Canvas.GetLeft(this.lastSelectedElement);
			this.canvasList[TemplateClass.selectedCanvasVideoID].CaptureMouse();
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x00021FFC File Offset: 0x000201FC
		private void Canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			TimeSpan timedif = DateTime.Now - this.mouseDownTime;
			if (this.selectedElement == null || !this.canvasList[TemplateClass.selectedCanvasVideoID].IsMouseCaptured)
			{
				if (this.buttonMove != null && this.canvasList[TemplateClass.selectedCanvasVideoID].IsMouseCaptured)
				{
					System.Windows.Point currentPosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]);
					double deltaX = currentPosition.X - this.elementClickedPosition.X;
					double deltaY = currentPosition.Y - this.elementClickedPosition.Y;
					Canvas.SetLeft(this.buttonMove, deltaX);
					Canvas.SetTop(this.buttonMove, deltaY);
					this.UpdateButtonsPositions(this.lastSelectedElement);
					this.UpdateRectPositionAndSize(this.lastSelectedElement);
					this.UpdatePositionText(this.lastSelectedElement);
					this.UpdateSizeText(this.lastSelectedElement);
					this.UpdateImagePosition(this.buttonMove);
				}
				return;
			}
			if (this.previousSelectedElement != this.lastSelectedElement && timedif.TotalSeconds < 0.2)
			{
				return;
			}
			System.Windows.Point currentPosition2 = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasVideoID]);
			double deltaX2 = currentPosition2.X - this.elementClickedPosition.X;
			double deltaY2 = currentPosition2.Y - this.elementClickedPosition.Y;
			Canvas.SetLeft(this.selectedElement, deltaX2);
			Canvas.SetTop(this.selectedElement, deltaY2);
			this.UpdateButtonsPositions(this.selectedElement);
			this.UpdateRectPositionAndSize(this.selectedElement);
			this.UpdatePositionText(this.selectedElement);
			this.UpdateSizeText(this.selectedElement);
			this.UpdateMoveButtonPosition(this.selectedElement);
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x000221B4 File Offset: 0x000203B4
		private void Canvas_PreviewMouseUp(object sender, MouseButtonEventArgs e)
		{
			base.Cursor = System.Windows.Input.Cursors.Arrow;
			if (this.isMoving && this.lastSelectedElement != null)
			{
				this.UpdateTemplateClassImagePosition(this.lastSelectedElement);
			}
			if (this.selectedElement != null && !(sender is System.Windows.Controls.Button))
			{
				if (this.selectedElement.Uid != "")
				{
					this.UpdateTemplateClassImagePosition(this.selectedElement);
				}
				this.selectedElement = null;
			}
			if (this.isResizing && this.lastSelectedElement != null)
			{
				this.UpdateTemplateClassImageSize(this.lastSelectedElement);
			}
			this.canvasList[TemplateClass.selectedCanvasVideoID].ReleaseMouseCapture();
			this.isMoving = false;
			if (this.lastSelectedElement == null)
			{
				this.ClearButtonsAndRect();
				this.UpdateUndoneMoves();
				this.UpdateUndoButton();
			}
			if (this.lastSelectedElement != null)
			{
				if (this.IsOverlap(TemplateClass.GetObject(Convert.ToInt32(this.lastSelectedElement.Uid), false)))
				{
					this.CreateMoveButton();
				}
				else
				{
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.buttonMove);
					this.buttonMove = null;
				}
			}
			this.UpdateUndoneMoves();
			this.UpdateUndoButton();
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x000222D8 File Offset: 0x000204D8
		private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
		{
			string photoIconName = TemplateClass.GetPhotoIconName();
			FileInfo newFileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName));
			System.Windows.Controls.Image addImage = new System.Windows.Controls.Image();
			BitmapImage bitmapImage = new BitmapImage(new Uri(newFileInfo.FullName, UriKind.Absolute));
			addImage.Stretch = Stretch.UniformToFill;
			addImage.Source = bitmapImage;
			addImage.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			addImage.VerticalAlignment = VerticalAlignment.Center;
			Grid addImageGrid = new Grid();
			double width = (double)bitmapImage.PixelWidth * this.canvasScale;
			double height = (double)bitmapImage.PixelHeight * this.canvasScale;
			addImageGrid.Width = width;
			addImageGrid.Height = height;
			addImageGrid.Children.Add(addImage);
			int capturedPhotoCount = 0;
			foreach (TemplateObject templateObj in TemplateClass.TemplateObjectsVideo)
			{
				if (templateObj.isCapture)
				{
					capturedPhotoCount++;
				}
			}
			TextBlock textBlock = new TextBlock();
			textBlock.Text = (capturedPhotoCount + 1).ToString();
			textBlock.Foreground = new SolidColorBrush(Colors.White);
			textBlock.FontSize = 160.0;
			textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
			textBlock.VerticalAlignment = VerticalAlignment.Center;
			addImageGrid.Children.Add(textBlock);
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(addImageGrid);
			Canvas.SetLeft(addImageGrid, 0.0);
			Canvas.SetTop(addImageGrid, 0.0);
			addImageGrid.PreviewMouseDown += this.Canvas_PreviewMouseDown;
			int count = TemplateClass.TemplateObjectsVideo.Count;
			addImageGrid.Uid = count.ToString();
			TemplateClass.TemplateObjectsVideo.Add(new TemplateObject
			{
				isCapture = true,
				photoID = capturedPhotoCount,
				fileInfo = newFileInfo,
				ID = count,
				layer = count,
				objectPoint = new System.Drawing.Point((int)(addImageGrid.RenderTransformOrigin.X / this.canvasScale), (int)(addImageGrid.RenderTransformOrigin.Y / this.canvasScale)),
				objectSize = new System.Drawing.Size((int)(addImageGrid.Width / this.canvasScale), (int)(addImageGrid.Height / this.canvasScale))
			});
			System.Windows.Controls.Panel.SetZIndex(addImageGrid, TemplateClass.TemplateObjectsVideo[count].layer);
			this.UpdateListBox(addImageGrid, TemplateClass.TemplateObjectsVideo, count);
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00022558 File Offset: 0x00020758
		private void AddImagesFromTemplateJson(TemplateJsonObject templateJsonObj)
		{
			this.ImageIDList.Clear();
			this.imageNameList.Clear();
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
			List<TemplateObject> currentObject = templateJsonObj.templateVideoObjects;
			for (int i = 0; i < currentObject.Count; i++)
			{
				if (!File.Exists(currentObject[i].fileInfo.FullName))
				{
					string deletedFileName = System.IO.Path.GetFileName(currentObject[i].fileInfo.FullName);
					this.DeleteElementProcess(null, currentObject[i]);
					MessageBoxWindow.CreateWindow("File Doesn't Exist", "The file " + deletedFileName + " has been deleted or moved.", new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, true);
				}
				else
				{
					UIElement addedElement;
					if (currentObject[i].isCapture)
					{
						addedElement = this.AddPhotoPrefab(currentObject[i]);
					}
					else
					{
						addedElement = this.AddImagePrefab(currentObject[i]);
					}
					this.UpdateListBox(addedElement, currentObject, i);
				}
			}
		}

		// Token: 0x0600055B RID: 1371 RVA: 0x00022654 File Offset: 0x00020854
		private Frame AddPhotoPrefab(TemplateObject templateObject)
		{
			string photoIconName = TemplateClass.GetPhotoIconName();
			FileInfo fileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName));
			if (fileInfo.Name != templateObject.fileInfo.Name)
			{
				templateObject.fileInfo = fileInfo;
			}
			Frame photoFrame = new Frame();
			PhotoPrefab photoPrefab = new PhotoPrefab(templateObject.fileInfo.Name);
			photoPrefab.photoID = templateObject.photoID + 1;
			photoPrefab.buttonTopLeft.PreviewMouseDown += this.Button_PreviewMouseDown;
			photoPrefab.buttonTopLeft.PreviewMouseMove += this.Button_MouseMove;
			photoPrefab.buttonTopLeft.PreviewMouseUp += this.Button_PreviewMouseUp;
			photoPrefab.buttonTopLeft.MouseLeave += this.Button_MouseLeave;
			photoPrefab.buttonTopRight.PreviewMouseDown += this.Button_PreviewMouseDown;
			photoPrefab.buttonTopRight.PreviewMouseMove += this.Button_MouseMove;
			photoPrefab.buttonTopRight.PreviewMouseUp += this.Button_PreviewMouseUp;
			photoPrefab.buttonTopRight.MouseLeave += this.Button_MouseLeave;
			photoPrefab.buttonBottomLeft.PreviewMouseDown += this.Button_PreviewMouseDown;
			photoPrefab.buttonBottomLeft.PreviewMouseMove += this.Button_MouseMove;
			photoPrefab.buttonBottomLeft.PreviewMouseUp += this.Button_PreviewMouseUp;
			photoPrefab.buttonBottomLeft.MouseLeave += this.Button_MouseLeave;
			photoPrefab.buttonBottomRight.PreviewMouseDown += this.Button_PreviewMouseDown;
			photoPrefab.buttonBottomRight.PreviewMouseMove += this.Button_MouseMove;
			photoPrefab.buttonBottomRight.PreviewMouseUp += this.Button_PreviewMouseUp;
			photoPrefab.buttonBottomRight.MouseLeave += this.Button_MouseLeave;
			PhotoPrefab photoPrefab2 = photoPrefab;
			photoPrefab2.photoIDChanged = (Action<int, string>)Delegate.Combine(photoPrefab2.photoIDChanged, new Action<int, string>(this.PhotoIDChanged));
			List<TemplateObject> templateObjs = TemplateClass.TemplateObjectsVideo;
			photoFrame.Content = photoPrefab;
			this.SetFrameProperties(photoFrame, templateObject);
			photoPrefab.Uid = photoFrame.Uid;
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(photoFrame);
			this.photoIDChangedTemplatePage = (Action<int, string>)Delegate.Combine(this.photoIDChangedTemplatePage, new Action<int, string>(photoPrefab.PhotoIDChangedTemplatePage));
			return photoFrame;
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x000228BC File Offset: 0x00020ABC
		private void PhotoIDChanged(int value, string prefabUid)
		{
			foreach (TemplateObject item in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects)
			{
				if (item.ID == int.Parse(prefabUid))
				{
					item.photoID = value - 1;
				}
			}
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00022930 File Offset: 0x00020B30
		private void DeleteElementProcess(UIElement element, TemplateObject delElement)
		{
			Frame frame = element as Frame;
			if (frame != null)
			{
				ImagePrefab imgPrefab = frame.Content as ImagePrefab;
				if (imgPrefab != null)
				{
					imgPrefab.image.Source = null;
				}
			}
			TemplateClass.TemplateObjectsPhoto.Remove(delElement);
			if (element != null)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(element);
			}
			this.lastSelectedElement = null;
			if (this.imageNameList.Count > delElement.layer)
			{
				this.imageNameList.RemoveAt(delElement.layer);
				this.ImageIDList.RemoveAt(delElement.layer);
				this.SyncImageDeleteListBox();
				this.SyncImageDeleteListBoxPortrait();
			}
			this.imageListBox.SelectedIndex = -1;
			this.imageListBoxPortrait.SelectedIndex = -1;
			this.ClearButtonsAndRect();
			this.deletedElements.Add(element);
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x000229FE File Offset: 0x00020BFE
		private void ChangePhotoID(UIElement lastSelected)
		{
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x00022A00 File Offset: 0x00020C00
		private void CreateMoveButton()
		{
			if (this.buttonMove != null)
			{
				return;
			}
			if (this.lastSelectedElement != null)
			{
				SolidColorBrush myBrush = new SolidColorBrush();
				myBrush.Color = System.Windows.Media.Color.FromArgb(128, 0, 0, byte.MaxValue);
				System.Windows.Controls.Button button = new System.Windows.Controls.Button();
				this.buttonMove = button;
				this.buttonMove.Width = 20.0;
				this.buttonMove.Height = 20.0;
				this.buttonMove.Background = myBrush;
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(this.buttonMove);
				this.UpdateMoveButtonPosition(this.lastSelectedElement);
				System.Windows.Controls.Panel.SetZIndex(this.buttonMove, TemplateClass.TemplateObjectsVideo.Count + 1);
				this.buttonMove.Name = "buttonMove";
				this.buttonMove.PreviewMouseDown += this.Canvas_PreviewMouseDown;
				this.buttonMove.PreviewMouseUp += this.Canvas_PreviewMouseUp;
				this.buttonMove.MouseMove += this.Button_MouseMove;
				this.buttonMove.MouseLeave += this.Button_MouseLeave;
			}
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00022B30 File Offset: 0x00020D30
		private void AddImageButton_Click(object sender, RoutedEventArgs e)
		{
			this.UpdatePreviousMoves();
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string originalFileName = System.IO.Path.GetFileName(openFileDialog.FileName);
				string targetFilePath = this.GetTemplateFilePath(originalFileName);
				try
				{
					using (MagickImage mImage = new MagickImage(openFileDialog.FileName))
					{
						mImage.AutoOrient();
						uint originalWidth = mImage.Width;
						uint originalHeight = mImage.Height;
						double hRatio = (double)((float)TemplateClass.PaperHeightPhoto) * 2.0 / (double)originalHeight;
						double wRatio = (double)((float)TemplateClass.PaperWidthPhoto) * 2.0 / (double)originalWidth;
						double ratio = Math.Min(hRatio, wRatio);
						if (ratio < 1.0)
						{
							mImage.Resize((uint)(originalWidth * ratio), (uint)(originalHeight * ratio));
						}
						mImage.Write(targetFilePath);
					}
				}
				catch (Exception ex)
				{
					MessageBoxWindow.CreateWindow("Error Loading Image", ex.Message, new List<MessageBoxWindow.ButtonType>
					{
						MessageBoxWindow.ButtonType.Continue
					}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
					return;
				}
				FileInfo fileInfo = new FileInfo(targetFilePath);
				int IDNum = TemplateClass.TemplateObjectsVideo.Count + this.deletedElements.Count;
				int layer = TemplateClass.TemplateObjectsVideo.Count;
				TemplateObject newTemplateObject = this.CreateTemplateObject(fileInfo, IDNum, false, layer);
				TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects.Add(newTemplateObject);
				Frame addedFrame = this.AddImagePrefab(newTemplateObject);
				this.UpdateListBox(addedFrame, TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects, layer);
			}
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00022CD0 File Offset: 0x00020ED0
		private Frame AddImagePrefab(TemplateObject templateObject)
		{
			Frame imageFrame = new Frame();
			ImagePrefab imagePrefab = new ImagePrefab();
			imagePrefab.buttonTopLeft.PreviewMouseDown += this.Button_PreviewMouseDown;
			imagePrefab.buttonTopLeft.PreviewMouseMove += this.Button_MouseMove;
			imagePrefab.buttonTopLeft.PreviewMouseUp += this.Button_PreviewMouseUp;
			imagePrefab.buttonTopLeft.MouseLeave += this.Button_MouseLeave;
			imagePrefab.buttonTopRight.PreviewMouseDown += this.Button_PreviewMouseDown;
			imagePrefab.buttonTopRight.PreviewMouseMove += this.Button_MouseMove;
			imagePrefab.buttonTopRight.PreviewMouseUp += this.Button_PreviewMouseUp;
			imagePrefab.buttonTopRight.MouseLeave += this.Button_MouseLeave;
			imagePrefab.buttonBottomLeft.PreviewMouseDown += this.Button_PreviewMouseDown;
			imagePrefab.buttonBottomLeft.PreviewMouseMove += this.Button_MouseMove;
			imagePrefab.buttonBottomLeft.PreviewMouseUp += this.Button_PreviewMouseUp;
			imagePrefab.buttonBottomLeft.MouseLeave += this.Button_MouseLeave;
			imagePrefab.buttonBottomRight.PreviewMouseDown += this.Button_PreviewMouseDown;
			imagePrefab.buttonBottomRight.PreviewMouseMove += this.Button_MouseMove;
			imagePrefab.buttonBottomRight.PreviewMouseUp += this.Button_PreviewMouseUp;
			imagePrefab.buttonBottomRight.MouseLeave += this.Button_MouseLeave;
			using (FileStream stream = new FileStream(templateObject.fileInfo.FullName, FileMode.Open, FileAccess.Read))
			{
				BitmapImage image = new BitmapImage();
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.StreamSource = stream;
				image.EndInit();
				imagePrefab.image.Source = image;
			}
			imageFrame.Content = imagePrefab;
			this.SetFrameProperties(imageFrame, templateObject);
			imagePrefab.Uid = imageFrame.Uid;
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(imageFrame);
			return imageFrame;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00022EEC File Offset: 0x000210EC
		private void SetFrameProperties(Frame frame, TemplateObject templateObject)
		{
			double width = (double)templateObject.objectSize.Width * this.canvasScale;
			double height = (double)templateObject.objectSize.Height * this.canvasScale;
			height = ((height < 0.0) ? ((double)TemplateClass.PaperHeightVideo) : Math.Min(10000.0, height));
			width = ((width < 0.0) ? ((double)TemplateClass.PaperWidthVideo) : Math.Min(10000.0, width));
			frame.Width = width;
			frame.Height = height;
			frame.PreviewMouseDown += this.Canvas_PreviewMouseDown;
			frame.Uid = templateObject.ID.ToString();
			Canvas.SetLeft(frame, (double)templateObject.objectPoint.X * this.canvasScale);
			Canvas.SetTop(frame, (double)templateObject.objectPoint.Y * this.canvasScale);
			System.Windows.Controls.Panel.SetZIndex(frame, templateObject.layer);
		}

		// Token: 0x06000563 RID: 1379 RVA: 0x00022FD8 File Offset: 0x000211D8
		private TemplateObject CreateTemplateObject(FileInfo fileInfo, int IDNum, bool isCapture, int layer)
		{
			BitmapImage bitmapImage = new BitmapImage(new Uri(fileInfo.FullName, UriKind.Absolute));
			return new TemplateObject
			{
				isCapture = isCapture,
				fileInfo = fileInfo,
				ID = IDNum,
				layer = layer,
				objectPoint = new System.Drawing.Point(0, 0),
				objectSize = new System.Drawing.Size(bitmapImage.PixelWidth, bitmapImage.PixelHeight)
			};
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x00023040 File Offset: 0x00021240
		private string GetTemplateFilePath(string originalFileName)
		{
			string targetDirectory = System.IO.Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, TemplatePageVideo.templateImagesFolderName);
			if (!Directory.Exists(targetDirectory))
			{
				Directory.CreateDirectory(targetDirectory);
			}
			string targetFilePath = System.IO.Path.Combine(targetDirectory, originalFileName);
			string fileWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(originalFileName);
			string extension = System.IO.Path.GetExtension(originalFileName);
			int fileIndex = 1;
			while (File.Exists(targetFilePath))
			{
				string newFileName = string.Format("{0}({1}){2}", fileWithoutExtension, fileIndex++, extension);
				targetFilePath = System.IO.Path.Combine(targetDirectory, newFileName);
			}
			return targetFilePath;
		}

		// Token: 0x06000565 RID: 1381 RVA: 0x000230B8 File Offset: 0x000212B8
		private void ChangeLayer(object sender, RoutedEventArgs e)
		{
			this.UpdatePreviousMoves();
			int selectedIndex = this.imageListBox.SelectedIndex;
			if (this.isPortraitScreen)
			{
				if (this.imageListBoxPortrait.SelectedIndex == -1)
				{
					return;
				}
				selectedIndex = this.imageListBoxPortrait.SelectedIndex;
			}
			else
			{
				if (this.imageListBox.SelectedIndex == -1)
				{
					return;
				}
				selectedIndex = this.imageListBox.SelectedIndex;
			}
			System.Windows.Controls.Button senderButton = sender as System.Windows.Controls.Button;
			string s = senderButton.Name;
			int i = 0;
			if (s.Contains("Increase"))
			{
				i = 1;
			}
			else
			{
				i = -1;
			}
			string selectedImageName = this.imageNameList[selectedIndex];
			string selectedImageID = this.ImageIDList[selectedIndex];
			TemplateObject selectedImage = TemplateClass.TemplateObjectsVideo.Find((TemplateObject x) => x.ID == Convert.ToInt32(selectedImageID));
			if (TemplateClass.TemplateObjectsVideo.Count > selectedImage.layer + i && selectedImage.layer + i >= 0)
			{
				TemplateObject nextImage = TemplateClass.TemplateObjectsVideo[selectedImage.layer + i];
				selectedImage.layer += i;
				nextImage.layer -= i;
				TemplateClass.TemplateObjectsVideo.Sort((TemplateObject t1, TemplateObject t2) => t1.layer.CompareTo(t2.layer));
				int index;
				int index2;
				if (this.isPortraitScreen)
				{
					index = this.imageListBoxPortrait.SelectedIndex;
					index2 = index + i;
				}
				else
				{
					index = this.imageListBox.SelectedIndex;
					index2 = index + i;
				}
				string temp = this.ImageIDList[index];
				this.ImageIDList[index] = this.ImageIDList[index2];
				this.ImageIDList[index2] = temp;
				string temp2 = this.imageNameList[index];
				this.imageNameList[index] = this.imageNameList[index2];
				this.imageNameList[index2] = temp2;
				foreach (object obj in this.canvasList[TemplateClass.selectedCanvasVideoID].Children)
				{
					UIElement element = (UIElement)obj;
					if (element.Uid == selectedImage.ID.ToString())
					{
						System.Windows.Controls.Panel.SetZIndex(element, selectedImage.layer);
					}
					else if (element.Uid == nextImage.ID.ToString())
					{
						System.Windows.Controls.Panel.SetZIndex(element, nextImage.layer);
					}
				}
				if (this.isPortraitScreen)
				{
					this.imageListBoxPortrait.SelectedIndex = selectedIndex + i;
				}
				else
				{
					this.imageListBox.SelectedIndex = selectedIndex + i;
				}
			}
			this.UpdateUndoButton();
		}

		// Token: 0x06000566 RID: 1382 RVA: 0x00023384 File Offset: 0x00021584
		private void UpdateButtonsPositions(UIElement selectedElement)
		{
			if (this.buttons.Count < 1)
			{
				return;
			}
			Canvas.SetLeft(this.buttons[0], Canvas.GetLeft(selectedElement) - this.buttons[0].Width / 2.0);
			Canvas.SetTop(this.buttons[0], Canvas.GetTop(selectedElement) - this.buttons[0].Height / 2.0);
			Canvas.SetLeft(this.buttons[1], Canvas.GetLeft(selectedElement) + ((FrameworkElement)selectedElement).Width - this.buttons[1].Width / 2.0);
			Canvas.SetTop(this.buttons[1], Canvas.GetTop(selectedElement) - this.buttons[1].Height / 2.0);
			Canvas.SetLeft(this.buttons[2], Canvas.GetLeft(selectedElement) + ((FrameworkElement)selectedElement).Width - this.buttons[2].Width / 2.0);
			Canvas.SetTop(this.buttons[2], Canvas.GetTop(selectedElement) + ((FrameworkElement)selectedElement).Height - this.buttons[2].Height / 2.0);
			Canvas.SetLeft(this.buttons[3], Canvas.GetLeft(selectedElement) - this.buttons[3].Width / 2.0);
			Canvas.SetTop(this.buttons[3], Canvas.GetTop(selectedElement) + ((FrameworkElement)selectedElement).Height - this.buttons[3].Height / 2.0);
		}

		// Token: 0x06000567 RID: 1383 RVA: 0x00023568 File Offset: 0x00021768
		private void UpdateImagePositionWithTextBox()
		{
			if (this.lastSelectedElement == null)
			{
				return;
			}
			double x = 0.0;
			double y = 0.0;
			if (this.isPortraitScreen)
			{
				double.TryParse(this.positionXTextBoxPortrait.Text, out x);
				double.TryParse(this.positionYTextBoxPortrait.Text, out y);
			}
			else
			{
				double.TryParse(this.positionXTextBox.Text, out x);
				double.TryParse(this.positionYTextBox.Text, out y);
			}
			x *= this.canvasScale;
			y *= this.canvasScale;
			Canvas.SetLeft(this.lastSelectedElement, x);
			Canvas.SetTop(this.lastSelectedElement, y);
			this.UpdateTemplateClassImagePosition(this.lastSelectedElement);
			this.UpdateRectPositionAndSize(this.lastSelectedElement);
			this.UpdateButtonsPositions(this.lastSelectedElement);
			this.UpdateMoveButtonPosition(this.lastSelectedElement);
			if (this.IsOverlap(TemplateClass.GetObject(Convert.ToInt32(this.lastSelectedElement.Uid), true)))
			{
				this.CreateMoveButton();
				return;
			}
			this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(this.buttonMove);
			this.buttonMove = null;
		}

		// Token: 0x06000568 RID: 1384 RVA: 0x00023690 File Offset: 0x00021890
		private void UpdateRectPositionAndSize(UIElement referenceElement)
		{
			if (this.rect != null)
			{
				Canvas.SetLeft(this.rect, Canvas.GetLeft(referenceElement));
				Canvas.SetTop(this.rect, Canvas.GetTop(referenceElement));
				this.rect.Width = ((FrameworkElement)referenceElement).Width;
				this.rect.Height = ((FrameworkElement)referenceElement).Height;
			}
		}

		// Token: 0x06000569 RID: 1385 RVA: 0x000236F4 File Offset: 0x000218F4
		private void UpdateImagePosition(UIElement referenceElement)
		{
			if (this.buttonMove != null && this.lastSelectedElement != null)
			{
				Canvas.SetLeft(this.lastSelectedElement, Canvas.GetLeft(referenceElement) - ((FrameworkElement)this.lastSelectedElement).Width / 2.0 + this.buttonMove.Width / 2.0);
				Canvas.SetTop(this.lastSelectedElement, Canvas.GetTop(referenceElement) - ((FrameworkElement)this.lastSelectedElement).Height / 2.0 + this.buttonMove.Height / 2.0);
			}
		}

		// Token: 0x0600056A RID: 1386 RVA: 0x0002379C File Offset: 0x0002199C
		private void UpdateMoveButtonPosition(UIElement referenceElement)
		{
			if (this.buttonMove != null)
			{
				Canvas.SetLeft(this.buttonMove, Canvas.GetLeft(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Width / 2.0 - this.buttonMove.Width / 2.0);
				Canvas.SetTop(this.buttonMove, Canvas.GetTop(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Height / 2.0 - this.buttonMove.Height / 2.0);
			}
		}

		// Token: 0x0600056B RID: 1387 RVA: 0x00023844 File Offset: 0x00021A44
		private void UpdatePositionText(UIElement referenceElement)
		{
			if (this.isPortraitScreen)
			{
				this.positionXTextBoxPortrait.Text = ((int)(Canvas.GetLeft(referenceElement) / this.canvasScale)).ToString();
				this.positionYTextBoxPortrait.Text = ((int)(Canvas.GetTop(referenceElement) / this.canvasScale)).ToString();
				return;
			}
			this.positionXTextBox.Text = ((int)(Canvas.GetLeft(referenceElement) / this.canvasScale)).ToString();
			this.positionYTextBox.Text = ((int)(Canvas.GetTop(referenceElement) / this.canvasScale)).ToString();
		}

		// Token: 0x0600056C RID: 1388 RVA: 0x000238E0 File Offset: 0x00021AE0
		private void UpdateSizeText(UIElement referenceElement)
		{
			if (this.isPortraitScreen)
			{
				this.widthTextBoxPortrait.Text = ((int)(((FrameworkElement)referenceElement).Width / this.canvasScale)).ToString();
				this.heightTextBoxPortrait.Text = ((int)(((FrameworkElement)referenceElement).Height / this.canvasScale)).ToString();
				return;
			}
			this.widthTextBox.Text = ((int)(((FrameworkElement)referenceElement).Width / this.canvasScale)).ToString();
			this.heightTextBox.Text = ((int)(((FrameworkElement)referenceElement).Height / this.canvasScale)).ToString();
		}

		// Token: 0x0600056D RID: 1389 RVA: 0x00023990 File Offset: 0x00021B90
		private void UpdateImageSizeByText()
		{
			if (this.lastSelectedElement == null)
			{
				return;
			}
			double newWidth = 0.0;
			double newHeight = 0.0;
			int widthPortrait = string.IsNullOrEmpty(this.widthTextBoxPortrait.Text) ? 0 : Convert.ToInt32(this.widthTextBoxPortrait.Text);
			int heightPortrait = string.IsNullOrEmpty(this.heightTextBoxPortrait.Text) ? 0 : Convert.ToInt32(this.heightTextBoxPortrait.Text);
			int width = string.IsNullOrEmpty(this.widthTextBox.Text) ? 0 : Convert.ToInt32(this.widthTextBox.Text);
			int height = string.IsNullOrEmpty(this.heightTextBox.Text) ? 0 : Convert.ToInt32(this.heightTextBox.Text);
			int[] SizeNumberArray = new int[]
			{
				widthPortrait,
				heightPortrait,
				width,
				height
			};
			for (int i = 0; i < SizeNumberArray.Length; i++)
			{
				if (SizeNumberArray[i] < 0)
				{
					this.SizeTextBoxs[i].Text = "0";
				}
			}
			if (this.isPortraitScreen)
			{
				if (this.widthTextBoxPortrait.Text != "" && this.heightTextBoxPortrait.Text != "")
				{
					string[] parts = this.widthTextBoxPortrait.Text.Split(new char[]
					{
						'.'
					});
					parts = parts[0].Split(new char[]
					{
						','
					});
					newWidth = (double)Convert.ToInt32(parts[0]);
					parts = this.heightTextBoxPortrait.Text.Split(new char[]
					{
						'.'
					});
					parts = parts[0].Split(new char[]
					{
						','
					});
					newHeight = (double)Convert.ToInt32(parts[0]);
				}
			}
			else if (this.widthTextBox.Text != "" && this.heightTextBox.Text != "")
			{
				string[] parts2 = this.widthTextBox.Text.Split(new char[]
				{
					'.'
				});
				parts2 = parts2[0].Split(new char[]
				{
					','
				});
				newWidth = (double)Convert.ToInt32(parts2[0]);
				parts2 = this.heightTextBox.Text.Split(new char[]
				{
					'.'
				});
				parts2 = parts2[0].Split(new char[]
				{
					','
				});
				newHeight = (double)Convert.ToInt32(parts2[0]);
			}
			((FrameworkElement)this.lastSelectedElement).Width = newWidth * this.canvasScale;
			((FrameworkElement)this.lastSelectedElement).Height = newHeight * this.canvasScale;
			this.UpdateTemplateClassImageSize(this.lastSelectedElement);
			this.UpdateRectPositionAndSize(this.lastSelectedElement);
			this.UpdatePositionText(this.lastSelectedElement);
			this.UpdateButtonsPositions(this.lastSelectedElement);
			this.UpdateMoveButtonPosition(this.lastSelectedElement);
			if (this.IsOverlap(TemplateClass.GetObject(Convert.ToInt32(this.lastSelectedElement.Uid), false)))
			{
				this.CreateMoveButton();
				return;
			}
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.buttonMove);
			this.buttonMove = null;
		}

		// Token: 0x0600056E RID: 1390 RVA: 0x00023CC4 File Offset: 0x00021EC4
		private void UpdateTemplateClassImageSize(UIElement referenceElement)
		{
			TemplateObject templateObject = TemplateClass.GetObject(Convert.ToInt32(referenceElement.Uid), false);
			if (templateObject == null)
			{
				return;
			}
			templateObject.objectSize = new System.Drawing.Size((int)(((FrameworkElement)referenceElement).Width / this.canvasScale), (int)(((FrameworkElement)referenceElement).Height / this.canvasScale));
		}

		// Token: 0x0600056F RID: 1391 RVA: 0x00023D18 File Offset: 0x00021F18
		private void UpdateTemplateClassImagePosition(UIElement referenceElement)
		{
			TemplateObject templateObject = TemplateClass.GetObject(Convert.ToInt32(referenceElement.Uid), false);
			if (templateObject == null)
			{
				return;
			}
			templateObject.objectPoint = new System.Drawing.Point((int)(Canvas.GetLeft(referenceElement) / this.canvasScale), (int)(Canvas.GetTop(referenceElement) / this.canvasScale));
		}

		// Token: 0x06000570 RID: 1392 RVA: 0x00023D62 File Offset: 0x00021F62
		private void UpdateImageRotation()
		{
		}

		// Token: 0x06000571 RID: 1393 RVA: 0x00023D64 File Offset: 0x00021F64
		private string ImageNameSetter(string imageName, int count)
		{
			string[] parts = imageName.Split(new string[]
			{
				"-"
			}, StringSplitOptions.RemoveEmptyEntries);
			imageName = parts[0];
			imageName = imageName + "-" + count.ToString();
			return imageName;
		}

		// Token: 0x06000572 RID: 1394 RVA: 0x00023DA4 File Offset: 0x00021FA4
		private void UpdateListBox(UIElement image, List<TemplateObject> templateObjects, int count)
		{
			string imageName = templateObjects[count].fileInfo.Name;
			if (imageName.ToLower().Contains("photobgprofile"))
			{
				imageName = "Captured Photo";
			}
			int i = 1;
			while (this.imageNameList.Contains(imageName))
			{
				imageName = this.ImageNameSetter(imageName, i);
				i++;
			}
			this.imageNameList.Add(imageName);
			this.ImageIDList.Add(image.Uid);
		}

		// Token: 0x06000573 RID: 1395 RVA: 0x00023E18 File Offset: 0x00022018
		private void PaperSizeBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.UpdatePreviousMoves();
			if (this.canvasInitialWidth == 0.0 || this.canvasInitialHeight == 0.0 || double.IsNaN(this.canvasInitialWidth) || double.IsNaN(this.canvasInitialHeight))
			{
				return;
			}
			if (this.noChangePaper)
			{
				this.noChangePaper = false;
				return;
			}
			System.Windows.Controls.ComboBox comboBox = sender as System.Windows.Controls.ComboBox;
			string selectedItem = comboBox.SelectedItem as string;
			string selectedValue = selectedItem;
			ValueTuple<PaperSizeEnum, PaperSize>? result = PaperSizeData.GetPaperSizeByName(selectedValue);
			TemplateClass.PaperWidthVideo = result.Value.Item2.Width;
			TemplateClass.PaperHeightVideo = result.Value.Item2.Height;
			TemplateClass.PageInfo = selectedValue;
			TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageInfo = selectedValue;
			this.SetCanvasSize((double)TemplateClass.PaperWidthVideo, (double)TemplateClass.PaperHeightVideo);
			this.ImageIDList.Clear();
			this.imageNameList.Clear();
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
			TemplateJsonObject templateJsonObject = this.GetTemplateJsonObject();
			System.Windows.Size NewSize = new System.Windows.Size((double)TemplateClass.PaperWidthVideo, (double)TemplateClass.PaperHeightVideo);
			if ((this.LastSize.Width != 0.0 || this.LastSize.Height != 0.0) && this.LastSize != NewSize)
			{
				templateJsonObject.templateVideoObjects = this.ChangePaperSize(templateJsonObject, NewSize);
			}
			this.AddImagesFromTemplateJson(this.GetTemplateJsonObject());
			this.LastSize = NewSize;
			this.UpdatePreviousMoves();
		}

		// Token: 0x06000574 RID: 1396 RVA: 0x00023FA0 File Offset: 0x000221A0
		private List<TemplateObject> ChangePaperSize(TemplateJsonObject templateJsonObject, System.Windows.Size NewSize)
		{
			List<TemplateObject> currentObject = templateJsonObject.templateVideoObjects;
			for (int i = 0; i < currentObject.Count; i++)
			{
				currentObject[i].objectSize = new System.Drawing.Size((int)((double)((float)currentObject[i].objectSize.Width) * (NewSize.Width / this.LastSize.Width)), (int)((double)((float)currentObject[i].objectSize.Height) * (NewSize.Height / this.LastSize.Height)));
				currentObject[i].objectPoint = new System.Drawing.Point((int)((double)((float)currentObject[i].objectPoint.X) * (NewSize.Width / this.LastSize.Width)), (int)((double)((float)currentObject[i].objectPoint.Y) * (NewSize.Height / this.LastSize.Height)));
			}
			return currentObject;
		}

		// Token: 0x06000575 RID: 1397 RVA: 0x00024090 File Offset: 0x00022290
		private void PaperSizeBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.PaperSizeListBox.ItemsSource == null)
			{
				this.PaperSizeListBox.ItemsSource = PaperSizeData.GetNames();
				this.PaperSizeListBoxPortrait.ItemsSource = PaperSizeData.GetNames();
			}
			ValueTuple<PaperSizeEnum, PaperSize>? result = PaperSizeData.GetPaperSize(TemplateClass.PaperWidthVideo, TemplateClass.PaperHeightVideo);
			if (result != null)
			{
				this.PaperSizeListBox.SelectedItem = result.Value.Item2.Name;
				this.PaperSizeListBoxPortrait.SelectedItem = result.Value.Item2.Name;
				return;
			}
			this.PaperSizeListBox.SelectedIndex = 0;
			this.PaperSizeListBoxPortrait.SelectedIndex = 0;
		}

		// Token: 0x06000576 RID: 1398 RVA: 0x00024134 File Offset: 0x00022334
		private UIElement GetElementByID(string ID)
		{
			for (int i = 0; i < this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Count; i++)
			{
				if (this.canvasList[TemplateClass.selectedCanvasVideoID].Children[i].Uid == ID)
				{
					return this.canvasList[TemplateClass.selectedCanvasVideoID].Children[i];
				}
			}
			return null;
		}

		// Token: 0x06000577 RID: 1399 RVA: 0x000241B0 File Offset: 0x000223B0
		private UIElement SelectItemByID(string selectedItemID)
		{
			this.isSelecting = true;
			if (this.buttonMove != null)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.buttonMove);
				this.buttonMove = null;
			}
			foreach (System.Windows.Controls.Button button in this.buttons)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(button);
			}
			this.buttons.Clear();
			for (int i = 0; i < this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Count; i++)
			{
				if (this.canvasList[TemplateClass.selectedCanvasVideoID].Children[i].Uid == selectedItemID)
				{
					this.previousSelectedElement = this.lastSelectedElement;
					this.lastSelectedElement = this.canvasList[TemplateClass.selectedCanvasVideoID].Children[i];
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.rect);
					this.rect = null;
					this.rect = new System.Windows.Shapes.Rectangle();
					this.rect.Stroke = System.Windows.Media.Brushes.Orange;
					this.rect.StrokeDashArray = new DoubleCollection(new double[]
					{
						5.0,
						2.0
					});
					this.rect.StrokeThickness = 2.0;
					this.rect.Width = ((FrameworkElement)this.lastSelectedElement).Width;
					this.rect.Height = ((FrameworkElement)this.lastSelectedElement).Height;
					Canvas.SetLeft(this.rect, Canvas.GetLeft(this.lastSelectedElement));
					Canvas.SetTop(this.rect, Canvas.GetTop(this.lastSelectedElement));
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(this.rect);
					System.Windows.Controls.Panel.SetZIndex(this.rect, TemplateClass.TemplateObjectsVideo.Count + 1);
					this.UpdatePositionText(this.lastSelectedElement);
					this.UpdateSizeText(this.lastSelectedElement);
					SolidColorBrush myBrush = new SolidColorBrush();
					myBrush.Color = System.Windows.Media.Color.FromArgb(128, 0, 0, byte.MaxValue);
					System.Windows.Controls.Button buttonTopLeft = new System.Windows.Controls.Button();
					buttonTopLeft.Width = 20.0;
					buttonTopLeft.Height = 20.0;
					buttonTopLeft.Background = myBrush;
					double positionXTL = Canvas.GetLeft(this.lastSelectedElement) - buttonTopLeft.Width / 2.0;
					double positionYTL = Canvas.GetTop(this.lastSelectedElement) - buttonTopLeft.Height / 2.0;
					Canvas.SetLeft(buttonTopLeft, positionXTL);
					Canvas.SetTop(buttonTopLeft, positionYTL);
					System.Windows.Controls.Panel.SetZIndex(buttonTopLeft, TemplateClass.TemplateObjectsVideo.Count + 1);
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(buttonTopLeft);
					buttonTopLeft.PreviewMouseDown += this.Button_PreviewMouseDown;
					buttonTopLeft.PreviewMouseMove += this.Button_MouseMove;
					buttonTopLeft.PreviewMouseUp += this.Button_PreviewMouseUp;
					buttonTopLeft.MouseLeave += this.Button_MouseLeave;
					this.buttons.Add(buttonTopLeft);
					buttonTopLeft.Name = "buttonTopLeft";
					System.Windows.Controls.Button buttonTopRight = new System.Windows.Controls.Button();
					buttonTopRight.Width = 20.0;
					buttonTopRight.Height = 20.0;
					buttonTopRight.Background = myBrush;
					double positionXTR = Canvas.GetLeft(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Width - buttonTopRight.Width / 2.0;
					double positionYTR = Canvas.GetTop(this.lastSelectedElement) - buttonTopRight.Height / 2.0;
					Canvas.SetLeft(buttonTopRight, positionXTR);
					Canvas.SetTop(buttonTopRight, positionYTR);
					System.Windows.Controls.Panel.SetZIndex(buttonTopRight, TemplateClass.TemplateObjectsVideo.Count + 1);
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(buttonTopRight);
					buttonTopRight.PreviewMouseDown += this.Button_PreviewMouseDown;
					buttonTopRight.PreviewMouseMove += this.Button_MouseMove;
					buttonTopRight.PreviewMouseUp += this.Button_PreviewMouseUp;
					buttonTopRight.MouseLeave += this.Button_MouseLeave;
					this.buttons.Add(buttonTopRight);
					buttonTopRight.Name = "buttonTopRight";
					System.Windows.Controls.Button buttonBottomRight = new System.Windows.Controls.Button();
					buttonBottomRight.Width = 20.0;
					buttonBottomRight.Height = 20.0;
					buttonBottomRight.Background = myBrush;
					double positionXBR = Canvas.GetLeft(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Width - buttonBottomRight.Width / 2.0;
					double positionYBR = Canvas.GetTop(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Height - buttonBottomRight.Height / 2.0;
					Canvas.SetLeft(buttonBottomRight, positionXBR);
					Canvas.SetTop(buttonBottomRight, positionYBR);
					System.Windows.Controls.Panel.SetZIndex(buttonBottomRight, TemplateClass.TemplateObjectsVideo.Count + 1);
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(buttonBottomRight);
					buttonBottomRight.PreviewMouseDown += this.Button_PreviewMouseDown;
					buttonBottomRight.PreviewMouseMove += this.Button_MouseMove;
					buttonBottomRight.PreviewMouseUp += this.Button_PreviewMouseUp;
					buttonBottomRight.MouseLeave += this.Button_MouseLeave;
					this.buttons.Add(buttonBottomRight);
					buttonBottomRight.Name = "buttonBottomRight";
					System.Windows.Controls.Button buttonBottomLeft = new System.Windows.Controls.Button();
					buttonBottomLeft.Width = 20.0;
					buttonBottomLeft.Height = 20.0;
					buttonBottomLeft.Background = myBrush;
					double positionXBL = Canvas.GetLeft(this.lastSelectedElement) - buttonBottomLeft.Width / 2.0;
					double positionYBL = Canvas.GetTop(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Height - buttonBottomLeft.Height / 2.0;
					Canvas.SetLeft(buttonBottomLeft, positionXBL);
					Canvas.SetTop(buttonBottomLeft, positionYBL);
					System.Windows.Controls.Panel.SetZIndex(buttonBottomLeft, TemplateClass.TemplateObjectsVideo.Count + 1);
					this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Add(buttonBottomLeft);
					buttonBottomLeft.PreviewMouseDown += this.Button_PreviewMouseDown;
					buttonBottomLeft.PreviewMouseMove += this.Button_MouseMove;
					buttonBottomLeft.PreviewMouseUp += this.Button_PreviewMouseUp;
					buttonBottomLeft.MouseLeave += this.Button_MouseLeave;
					this.buttons.Add(buttonBottomLeft);
					buttonBottomLeft.Name = "buttonBottomLeft";
					TemplateObject selectedObject = TemplateClass.TemplateObjectsVideo.Find((TemplateObject x) => x.ID == Convert.ToInt32(this.lastSelectedElement.Uid));
					if (this.IsOverlap(selectedObject))
					{
						this.CreateMoveButton();
					}
					this.isSelecting = false;
					return this.lastSelectedElement;
				}
			}
			this.isSelecting = false;
			return null;
		}

		// Token: 0x06000578 RID: 1400 RVA: 0x00024900 File Offset: 0x00022B00
		private void LayerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.ImageIDList.Count == 0)
			{
				return;
			}
			System.Windows.Controls.ListBox layerBox = sender as System.Windows.Controls.ListBox;
			if (layerBox.Name == this.imageListBox.Name && !this.isPortraitScreen)
			{
				if (this.imageListBox.SelectedIndex == -1)
				{
					return;
				}
			}
			else
			{
				if (!this.isPortraitScreen || !(layerBox.Name == this.imageListBoxPortrait.Name))
				{
					return;
				}
				if (this.imageListBoxPortrait.SelectedIndex == -1)
				{
					return;
				}
			}
			int selectedItemIDIndex = layerBox.SelectedIndex;
			string selectedItemID = this.ImageIDList[selectedItemIDIndex];
			this.SelectItemByID(selectedItemID);
			this.SyncImageDeleteListBoxPortrait();
			this.SyncImageDeleteListBox();
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x000249AC File Offset: 0x00022BAC
		private string ReChanged(TextChangedEventArgs e, System.Windows.Controls.TextBox textBoxRef)
		{
			string retunText = textBoxRef.Text;
			return retunText.Replace(retunText.Substring(e.Changes.First<TextChange>().Offset, e.Changes.First<TextChange>().AddedLength), "");
		}

		// Token: 0x0600057A RID: 1402 RVA: 0x000249F4 File Offset: 0x00022BF4
		private void PositionTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			System.Windows.Controls.TextBox referanceTextBox = sender as System.Windows.Controls.TextBox;
			if (string.IsNullOrEmpty(referanceTextBox.Text) || referanceTextBox.Text == "0")
			{
				referanceTextBox.Text = "0";
				referanceTextBox.CaretIndex = 1;
				return;
			}
			if (!ExtensionMethod.IsTextAllowed(referanceTextBox.Text))
			{
				referanceTextBox.Text = this.ReChanged(e, referanceTextBox);
				referanceTextBox.CaretIndex = referanceTextBox.Text.Length;
				return;
			}
			if (referanceTextBox.Text.StartsWith("0") && referanceTextBox.Text.Length > 1)
			{
				referanceTextBox.Text = referanceTextBox.Text.TrimStart(new char[]
				{
					'0'
				});
				if (string.IsNullOrEmpty(referanceTextBox.Text))
				{
					referanceTextBox.Text = "0";
				}
				referanceTextBox.CaretIndex = 1;
			}
			if (this.isResizing || this.isMoving || this.isSelecting)
			{
				return;
			}
			this.UpdateImagePositionWithTextBox();
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x00024AE4 File Offset: 0x00022CE4
		private void SizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			System.Windows.Controls.TextBox referanceTextBox = sender as System.Windows.Controls.TextBox;
			foreach (TextChange change in e.Changes)
			{
				if (change.AddedLength > 0)
				{
					int offset = change.Offset;
					if (offset < referanceTextBox.Text.Length)
					{
						char lastChar = referanceTextBox.Text[offset];
						if (!char.IsDigit(lastChar))
						{
							int caret = referanceTextBox.CaretIndex;
							referanceTextBox.Text = referanceTextBox.Text.Remove(offset, 1);
							referanceTextBox.CaretIndex = caret - 1;
							return;
						}
					}
				}
			}
			if (string.IsNullOrEmpty(referanceTextBox.Text) || referanceTextBox.Text == "0")
			{
				referanceTextBox.Text = "0";
				referanceTextBox.CaretIndex = 1;
				return;
			}
			if (ExtensionMethod.IsTextAllowed(referanceTextBox.Text))
			{
				if (referanceTextBox.Text.StartsWith("0") && referanceTextBox.Text.Length > 1)
				{
					referanceTextBox.Text = referanceTextBox.Text.TrimStart(new char[]
					{
						'0'
					});
					if (string.IsNullOrEmpty(referanceTextBox.Text))
					{
						referanceTextBox.Text = "0";
					}
					referanceTextBox.CaretIndex = 1;
				}
				if (this.isResizing || this.isMoving || this.isSelecting)
				{
					return;
				}
				this.UpdateImageSizeByText();
				return;
			}
			else
			{
				referanceTextBox.Text = this.ReChanged(e, referanceTextBox);
				referanceTextBox.CaretIndex = referanceTextBox.Text.Length;
			}
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x00024C6C File Offset: 0x00022E6C
		private void RotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateImageRotation();
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x00024C74 File Offset: 0x00022E74
		public void ChangeParent(Grid grid, Canvas newParent)
		{
			System.Windows.Controls.Panel panel = grid.Parent as System.Windows.Controls.Panel;
			if (panel != null)
			{
				panel.Children.Remove(grid);
			}
			if (newParent != null)
			{
				newParent.Children.Add(grid);
			}
		}

		// Token: 0x0600057E RID: 1406 RVA: 0x00024CB0 File Offset: 0x00022EB0
		private bool IsOverlap(TemplateObject selectedObject)
		{
			int layerCount = TemplateClass.TemplateObjectsVideo.Count - 1;
			if (selectedObject == null)
			{
				return false;
			}
			if (selectedObject.layer != layerCount)
			{
				int i2;
				int i;
				for (i = selectedObject.layer + 1; i <= layerCount; i = i2 + 1)
				{
					TemplateObject aTemplateObject = TemplateClass.TemplateObjectsVideo.Find((TemplateObject x) => x.layer == i);
					if (aTemplateObject == null)
					{
						return false;
					}
					for (int j = 0; j < this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Count; j++)
					{
						if (this.canvasList[TemplateClass.selectedCanvasVideoID].Children[j].Uid == aTemplateObject.ID.ToString())
						{
							UIElement anElement = this.canvasList[TemplateClass.selectedCanvasVideoID].Children[j];
							if (Canvas.GetLeft(this.lastSelectedElement) < Canvas.GetLeft(anElement) + ((FrameworkElement)anElement).Width && Canvas.GetLeft(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Width > Canvas.GetLeft(anElement) && Canvas.GetTop(this.lastSelectedElement) < Canvas.GetTop(anElement) + ((FrameworkElement)anElement).Height && Canvas.GetTop(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Height > Canvas.GetTop(anElement))
							{
								return true;
							}
						}
					}
					i2 = i;
				}
			}
			return false;
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x00024E3C File Offset: 0x0002303C
		private void Undo(object sender, RoutedEventArgs e)
		{
			if (this.previousMoves.Count == 0)
			{
				return;
			}
			this.isResizing = true;
			this.isMoving = true;
			TemplateJsonObject current = this.GetTemplateJsonObject();
			if (this.previousMoves.Count == 1 && this.CompareTemplateJsonObjects(current, this.previousMoves[0]))
			{
				return;
			}
			this.lastVersionBeforeUndone = this.GetTemplateJsonObjectAsCopy();
			TemplateJsonObject previousTemplateJsonObj = this.previousMoves[this.previousMoves.Count - 1];
			if (this.CompareTemplateJsonObjects(this.lastVersionBeforeUndone, this.previousMoves[this.previousMoves.Count - 1]))
			{
				this.previousMoves.RemoveAt(this.previousMoves.Count - 1);
				if (this.previousMoves.Count == 0)
				{
					this.isResizing = false;
					this.isMoving = false;
					return;
				}
				previousTemplateJsonObj = this.previousMoves[this.previousMoves.Count - 1];
			}
			this.undoneMoves.Add(this.GetTemplateJsonObjectAsCopy());
			this.previousMoves.RemoveAt(this.previousMoves.Count - 1);
			this.ImageIDList.Clear();
			this.imageNameList.Clear();
			TemplateClass.PaperHeightVideo = previousTemplateJsonObj.PageHeightVideo;
			TemplateClass.PaperWidthVideo = previousTemplateJsonObj.PageWidthVideo;
			this.SetCanvasSize((double)TemplateClass.PaperWidthVideo, (double)TemplateClass.PaperHeightVideo);
			this.LayerUpdate(previousTemplateJsonObj.templateVideoObjects);
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
			this.AddImagesFromTemplateJson(previousTemplateJsonObj);
			TemplateClass.TemplateObjectsVideo = previousTemplateJsonObj.templateVideoObjects;
			this.PaperSizeBox_Loaded(null, null);
			this.ClearButtonsAndRect();
			this.lastVersionAfterUndone = this.GetTemplateJsonObjectAsCopy();
			this.UpdateRedoButton();
			this.UpdateUndoButton();
			this.isResizing = false;
			this.isMoving = false;
		}

		// Token: 0x06000580 RID: 1408 RVA: 0x00024FFC File Offset: 0x000231FC
		private void Redo(object sender, RoutedEventArgs e)
		{
			if (this.undoneMoves.Count == 0)
			{
				return;
			}
			this.isMoving = true;
			this.isResizing = true;
			this.UpdatePreviousMoves();
			TemplateJsonObject undoneTemplateJsonObj = this.undoneMoves[this.undoneMoves.Count - 1];
			this.lastVersionBeforeRedone = this.GetTemplateJsonObjectAsCopy();
			if (this.CompareTemplateJsonObjects(this.lastVersionBeforeRedone, this.undoneMoves[this.undoneMoves.Count - 1]))
			{
				this.undoneMoves.RemoveAt(this.undoneMoves.Count - 1);
				if (this.undoneMoves.Count == 0)
				{
					this.isResizing = false;
					this.isMoving = false;
					return;
				}
				undoneTemplateJsonObj = this.undoneMoves[this.undoneMoves.Count - 1];
			}
			this.undoneMoves.RemoveAt(this.undoneMoves.Count - 1);
			this.ImageIDList.Clear();
			this.imageNameList.Clear();
			TemplateClass.PaperHeightVideo = undoneTemplateJsonObj.PageHeightVideo;
			TemplateClass.PaperWidthVideo = undoneTemplateJsonObj.PageWidthVideo;
			this.SetCanvasSize((double)TemplateClass.PaperWidthVideo, (double)TemplateClass.PaperHeightVideo);
			this.LayerUpdate(undoneTemplateJsonObj.templateVideoObjects);
			this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
			this.AddImagesFromTemplateJson(undoneTemplateJsonObj);
			TemplateClass.TemplateObjectsVideo = undoneTemplateJsonObj.templateVideoObjects;
			this.PaperSizeBox_Loaded(null, null);
			this.ClearButtonsAndRect();
			this.UpdateRedoButton();
			this.UpdateUndoButton();
			this.isMoving = false;
			this.isResizing = false;
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x0002517C File Offset: 0x0002337C
		private TemplateJsonObject GetTemplateJsonObjectAsCopy()
		{
			List<TemplateObject> templateObjects = new List<TemplateObject>();
			foreach (TemplateObject templateObj in TemplateClass.TemplateObjectsVideo)
			{
				TemplateObject templateObject = new TemplateObject
				{
					fileInfo = templateObj.fileInfo,
					ID = Convert.ToInt32(templateObj.ID),
					isCapture = templateObj.isCapture,
					photoID = templateObj.photoID,
					layer = templateObj.layer,
					objectPoint = templateObj.objectPoint,
					objectSize = templateObj.objectSize
				};
				templateObjects.Add(templateObject);
			}
			return new TemplateJsonObject
			{
				DPI = 300,
				PageHeightVideo = TemplateClass.PaperHeightVideo,
				PageWidthVideo = TemplateClass.PaperWidthVideo,
				PageHeightPhoto = TemplateClass.PaperHeightPhoto,
				PageWidthPhoto = TemplateClass.PaperWidthPhoto,
				PageInfo = "4'' x 6'' paper",
				templateVideoObjects = templateObjects
			};
		}

		// Token: 0x06000582 RID: 1410 RVA: 0x00025288 File Offset: 0x00023488
		private TemplateJsonObject GetTemplateJsonObjectAsCopy(TemplateJsonObject templatejsonobj)
		{
			List<TemplateObject> templateObjects = new List<TemplateObject>();
			foreach (TemplateObject templateObj in templatejsonobj.templateVideoObjects)
			{
				TemplateObject templateObject = new TemplateObject
				{
					fileInfo = templateObj.fileInfo,
					ID = Convert.ToInt32(templateObj.ID),
					isCapture = templateObj.isCapture,
					photoID = templateObj.photoID,
					layer = templateObj.layer,
					objectPoint = templateObj.objectPoint,
					objectSize = templateObj.objectSize
				};
				templateObjects.Add(templateObject);
			}
			return new TemplateJsonObject
			{
				DPI = 300,
				PageHeightVideo = templatejsonobj.PageHeightVideo,
				PageWidthVideo = templatejsonobj.PageWidthVideo,
				PageHeightPhoto = templatejsonobj.PageHeightPhoto,
				PageWidthPhoto = templatejsonobj.PageWidthPhoto,
				PageInfo = "4'' x 6'' paper",
				templateVideoObjects = templateObjects
			};
		}

		// Token: 0x06000583 RID: 1411 RVA: 0x00025398 File Offset: 0x00023598
		private void UpdatePreviousMoves()
		{
			TemplateJsonObject current = this.GetTemplateJsonObject();
			if (this.previousMoves.Count > 0 && this.CompareTemplateJsonObjects(current, this.previousMoves[this.previousMoves.Count - 1]))
			{
				return;
			}
			if (this.previousMoves.Count <= this.maxUndoCount)
			{
				this.previousMoves.Add(this.GetTemplateJsonObjectAsCopy());
			}
			else
			{
				this.previousMoves.RemoveAt(0);
				this.previousMoves.Add(this.GetTemplateJsonObjectAsCopy());
			}
			this.UpdateUndoButton();
		}

		// Token: 0x06000584 RID: 1412 RVA: 0x00025428 File Offset: 0x00023628
		private void UpdateUndoButton()
		{
			TemplateJsonObject current = this.GetTemplateJsonObject();
			if (this.previousMoves.Count == 1 && this.CompareTemplateJsonObjects(current, this.previousMoves[0]))
			{
				this.UndoButton.IsEnabled = false;
			}
			else if (this.previousMoves.Count == 0)
			{
				this.UndoButton.IsEnabled = false;
			}
			else
			{
				this.UndoButton.IsEnabled = true;
			}
			this.UpdateRedoButton();
		}

		// Token: 0x06000585 RID: 1413 RVA: 0x0002549C File Offset: 0x0002369C
		private void UpdateUndoneMoves()
		{
			TemplateJsonObject current = this.GetTemplateJsonObject();
			if (this.lastVersionAfterUndone == null)
			{
				return;
			}
			if (!this.CompareTemplateJsonObjects(current, this.lastVersionAfterUndone))
			{
				this.undoneMoves.Clear();
			}
			this.UpdateRedoButton();
		}

		// Token: 0x06000586 RID: 1414 RVA: 0x000254D9 File Offset: 0x000236D9
		private void UpdateRedoButton()
		{
			if (this.undoneMoves.Count == 0)
			{
				this.RedoButton.IsEnabled = false;
				return;
			}
			this.RedoButton.IsEnabled = true;
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00025501 File Offset: 0x00023701
		private void ResetPreviousAndUndoneMoves()
		{
			this.lastVersionAfterUndone = null;
			this.undoneMoves.Clear();
			this.previousMoves.Clear();
			this.UpdateUndoButton();
			this.UpdateRedoButton();
		}

		// Token: 0x06000588 RID: 1416 RVA: 0x0002552C File Offset: 0x0002372C
		private TemplateJsonObject GetTemplateJsonObject()
		{
			return new TemplateJsonObject
			{
				DPI = 300,
				PageHeightVideo = TemplateClass.PaperHeightVideo,
				PageWidthVideo = TemplateClass.PaperWidthVideo,
				PageHeightPhoto = TemplateClass.PaperHeightPhoto,
				PageWidthPhoto = TemplateClass.PaperWidthPhoto,
				PageInfo = TemplateClass.PageInfo,
				templateVideoObjects = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects
			};
		}

		// Token: 0x06000589 RID: 1417 RVA: 0x0002559C File Offset: 0x0002379C
		private bool CompareTemplateJsonObjects(TemplateJsonObject t1, TemplateJsonObject t2)
		{
			if (t1.PageInfo == t2.PageInfo && t1.PageWidthVideo == t2.PageWidthVideo && t1.PageHeightVideo == t2.PageHeightVideo && t1.DPI == t2.DPI && t1.templateVideoObjects.Count == t2.templateVideoObjects.Count)
			{
				for (int i = 0; i < t1.templateVideoObjects.Count; i++)
				{
					if (t1.templateVideoObjects[i].ID != t2.templateVideoObjects[i].ID)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].layer != t2.templateVideoObjects[i].layer)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].fileInfo != t2.templateVideoObjects[i].fileInfo)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].isCapture != t2.templateVideoObjects[i].isCapture)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].objectPoint.X != t2.templateVideoObjects[i].objectPoint.X)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].objectPoint.Y != t2.templateVideoObjects[i].objectPoint.Y)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].objectSize.Width != t2.templateVideoObjects[i].objectSize.Width)
					{
						return false;
					}
					if (t1.templateVideoObjects[i].objectSize.Height != t2.templateVideoObjects[i].objectSize.Height)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600058A RID: 1418 RVA: 0x00025784 File Offset: 0x00023984
		private void LayerUpdate(List<TemplateObject> templateObjects)
		{
			for (int i = 0; i < templateObjects.Count; i++)
			{
				templateObjects[i].layer = i;
			}
		}

		// Token: 0x0600058B RID: 1419 RVA: 0x000257B0 File Offset: 0x000239B0
		private void ClearButtonsAndRect()
		{
			if (this.buttonMove != null)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.buttonMove);
				this.buttonMove = null;
			}
			foreach (System.Windows.Controls.Button button in this.buttons)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(button);
			}
			this.buttons.Clear();
			if (this.previousMoves.Count <= 0)
			{
				this.UndoButton.IsEnabled = false;
			}
			if (this.rect != null)
			{
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Remove(this.rect);
				this.rect = null;
			}
		}

		// Token: 0x0600058C RID: 1420 RVA: 0x0002589C File Offset: 0x00023A9C
		private void TemplatePage_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z && TemplatePage.GetTemplatePage() != null && this.UndoButton.IsEnabled)
			{
				this.Undo(null, null);
			}
			if (Keyboard.IsKeyDown(Key.LeftCtrl) && e.Key == Key.Z && TemplatePage.GetTemplatePage() != null && Keyboard.IsKeyDown(Key.LeftShift) && this.UndoButton.IsEnabled)
			{
				this.Redo(null, null);
			}
			if (e.Key == Key.Delete && this.lastSelectedElement != null)
			{
				this.DeleteElement(null, null);
			}
			if (e.Key == Key.Return)
			{
				this.UpdatePositionText(this.lastSelectedElement);
				this.UpdateSizeText(this.lastSelectedElement);
				this.dummyButton.Focus();
			}
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x00025958 File Offset: 0x00023B58
		private void ManagePortraitScreen()
		{
			this.isPortraitScreen = (base.ActualWidth < base.ActualHeight);
			double screenRation = base.ActualWidth / 3840.0;
			if (this.isPortraitScreen)
			{
				this.gridRotationManage.RowDefinitions[1].Height = new GridLength(this.gridMain.ActualWidth * 0.2);
				this.gridContent.ColumnDefinitions[0].Width = new GridLength(0.0);
				this.gridContent.ColumnDefinitions[1].Width = new GridLength(0.0);
				return;
			}
			this.gridRotationManage.RowDefinitions[1].Height = new GridLength(0.0);
			this.gridContent.ColumnDefinitions[0].Width = new GridLength(55.0, GridUnitType.Star);
			this.gridContent.ColumnDefinitions[1].Width = new GridLength(10.0, GridUnitType.Star);
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00025A79 File Offset: 0x00023C79
		private void SettingsSubPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x0600058F RID: 1423 RVA: 0x00025A81 File Offset: 0x00023C81
		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.lastSelectedElement != null)
			{
				this.UpdateSizeText(this.lastSelectedElement);
				this.UpdatePositionText(this.lastSelectedElement);
			}
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00025AA4 File Offset: 0x00023CA4
		private void imageListBoxPortrait_Loaded(object sender, RoutedEventArgs e)
		{
			ScrollViewer scrollViewer = this.FindVisualChild<ScrollViewer>(this.imageListBoxPortrait);
			if (scrollViewer != null)
			{
				this.externalScrollBarImageListBoxPortrait.Maximum = scrollViewer.ScrollableHeight;
				this.externalScrollBarImageListBoxPortrait.ViewportSize = scrollViewer.ViewportHeight;
				this.externalScrollBarImageListBoxPortrait.Value = scrollViewer.VerticalOffset;
				this.externalScrollBarImageListBoxPortrait.Scroll += delegate(object s, System.Windows.Controls.Primitives.ScrollEventArgs ev)
				{
					scrollViewer.ScrollToVerticalOffset(this.externalScrollBarImageListBoxPortrait.Value);
				};
				scrollViewer.ScrollChanged += delegate(object s, ScrollChangedEventArgs ev)
				{
					this.externalScrollBarImageListBoxPortrait.Maximum = ev.ExtentHeight;
					this.externalScrollBarImageListBoxPortrait.ViewportSize = ev.ViewportHeight;
					this.externalScrollBarImageListBoxPortrait.Value = ev.VerticalOffset;
				};
			}
			this.SyncImageDeleteListBoxPortrait();
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00025B50 File Offset: 0x00023D50
		private void imageListBox_Loaded(object sender, RoutedEventArgs e)
		{
			ScrollViewer scrollViewer1 = this.FindVisualChild<ScrollViewer>(this.imageListBox);
			ScrollViewer scrollViewer2 = this.FindVisualChild<ScrollViewer>(this.imageListBoxDelete);
			if (scrollViewer1 != null)
			{
				this.externalScrollBarImageListBox.Maximum = scrollViewer1.ScrollableHeight;
				this.externalScrollBarImageListBox.ViewportSize = scrollViewer1.ViewportHeight;
				this.externalScrollBarImageListBox.Value = scrollViewer1.VerticalOffset;
				this.externalScrollBarImageListBox.Scroll += delegate(object s, System.Windows.Controls.Primitives.ScrollEventArgs ev)
				{
					scrollViewer1.ScrollToVerticalOffset(this.externalScrollBarImageListBox.Value);
					ScrollViewer scrollViewer = scrollViewer2;
					if (scrollViewer == null)
					{
						return;
					}
					scrollViewer.ScrollToVerticalOffset(this.externalScrollBarImageListBox.Value);
				};
				scrollViewer1.ScrollChanged += delegate(object s, ScrollChangedEventArgs ev)
				{
					this.externalScrollBarImageListBox.Maximum = ev.ExtentHeight;
					this.externalScrollBarImageListBox.ViewportSize = ev.ViewportHeight;
					this.externalScrollBarImageListBox.Value = ev.VerticalOffset;
				};
			}
			if (scrollViewer2 != null)
			{
				scrollViewer2.ScrollChanged += delegate(object s, ScrollChangedEventArgs ev)
				{
					ScrollViewer scrollViewer = scrollViewer1;
					if (scrollViewer == null)
					{
						return;
					}
					scrollViewer.ScrollToVerticalOffset(ev.VerticalOffset);
				};
			}
			this.SyncImageDeleteListBox();
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x00025C2C File Offset: 0x00023E2C
		private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				DependencyObject child = VisualTreeHelper.GetChild(obj, i);
				if (child != null && child is T)
				{
					return (T)((object)child);
				}
				T childOfChild = this.FindVisualChild<T>(child);
				if (childOfChild != null)
				{
					return childOfChild;
				}
			}
			return default(T);
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x00025C80 File Offset: 0x00023E80
		private void SyncImageDeleteListBoxPortrait()
		{
			this.imageListBoxDeletePortrait.Items.Clear();
			foreach (object item in ((IEnumerable)this.imageListBoxPortrait.Items))
			{
				ListBoxItem newItem = new ListBoxItem();
				this.imageListBoxDeletePortrait.Items.Add(newItem);
			}
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x00025CFC File Offset: 0x00023EFC
		private void SyncImageDeleteListBox()
		{
			this.imageListBoxDelete.Items.Clear();
			foreach (object item in ((IEnumerable)this.imageListBox.Items))
			{
				ListBoxItem newItem = new ListBoxItem();
				this.imageListBoxDelete.Items.Add(newItem);
			}
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x00025D78 File Offset: 0x00023F78
		private void ImageListBoxDeleteButton_Click(object sender, RoutedEventArgs e)
		{
			System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
			DependencyObject parent = VisualTreeHelper.GetParent(button);
			while (!(parent is ListBoxItem))
			{
				parent = VisualTreeHelper.GetParent(parent);
			}
			ListBoxItem listBoxItem = (ListBoxItem)parent;
			System.Windows.Controls.ListBox listBox = ItemsControl.ItemsControlFromItemContainer(listBoxItem) as System.Windows.Controls.ListBox;
			int index = listBox.ItemContainerGenerator.IndexFromContainer(listBoxItem);
			if (index >= 0 && index < this.imageNameList.Count)
			{
				this.lastSelectedElement = this.GetElementByID(this.ImageIDList[index]);
				this.DeleteElement(null, null);
			}
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x00025DFC File Offset: 0x00023FFC
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Windows.Controls.TabControl selectedItem = (System.Windows.Controls.TabControl)sender;
			this.SaveCanvasAsJpg(this.canvasList[TemplateClass.selectedCanvasVideoID], TemplateClass.GetTemplateVideoGalleryPhotoPath(TemplateClass.selectedCanvasVideoID), 90);
			if (TemplateClass.selectedCanvasVideoID != selectedItem.SelectedIndex)
			{
				TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasVideoID);
				TemplateClass.selectedCanvasVideoID = selectedItem.SelectedIndex;
				TemplateClass.ReadJsonTemplateObjects(false);
				this.UpdatePreviousMoves();
				TemplateClass.PageInfo = PaperSizeData.GetPaperSize(TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageWidthVideo, TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageHeightVideo).Value.Item2.Name;
				int ID = PaperSizeData.GetPaperSizeIndex(TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageWidthVideo, TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageHeightVideo);
				this.noChangePaper = (this.PaperSizeListBox.SelectedIndex != ID);
				this.SetCanvasSize((double)TemplateClass.PaperWidthVideo, (double)TemplateClass.PaperHeightVideo);
				this.ImageIDList.Clear();
				this.imageNameList.Clear();
				this.canvasList[TemplateClass.selectedCanvasVideoID].Children.Clear();
				TemplateJsonObject templateJsonObject = this.GetTemplateJsonObject();
				this.AddImagesFromTemplateJson(templateJsonObject);
				this.LastSize = new System.Windows.Size((double)TemplateClass.PaperWidthVideo, (double)TemplateClass.PaperHeightVideo);
				this.PaperSizeListBox.SelectedIndex = ID;
				this.UpdatePreviousMoves();
			}
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x00026861 File Offset: 0x00024A61
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 22)
			{
				((System.Windows.Controls.Button)target).Click += this.ImageListBoxDeleteButton_Click;
				return;
			}
			if (connectionId != 48)
			{
				return;
			}
			((System.Windows.Controls.Button)target).Click += this.ImageListBoxDeleteButton_Click;
		}

		// Token: 0x04000544 RID: 1348
		private UIElement selectedElement;

		// Token: 0x04000545 RID: 1349
		private UIElement lastSelectedElement;

		// Token: 0x04000546 RID: 1350
		private UIElement previousSelectedElement;

		// Token: 0x04000547 RID: 1351
		private System.Windows.Controls.Button buttonMove = new System.Windows.Controls.Button();

		// Token: 0x04000548 RID: 1352
		private System.Windows.Point elementClickedPosition;

		// Token: 0x04000549 RID: 1353
		private System.Windows.Point previousMousePosition;

		// Token: 0x0400054A RID: 1354
		private System.Windows.Shapes.Rectangle rect;

		// Token: 0x0400054B RID: 1355
		private double canvasScale;

		// Token: 0x0400054C RID: 1356
		private double canvasInitialWidth;

		// Token: 0x0400054D RID: 1357
		private double canvasInitialHeight;

		// Token: 0x0400054E RID: 1358
		private List<UIElement> deletedElements = new List<UIElement>();

		// Token: 0x0400054F RID: 1359
		private ObservableCollection<string> imageNameList = new ObservableCollection<string>();

		// Token: 0x04000550 RID: 1360
		private List<string> ImageIDList = new List<string>();

		// Token: 0x04000551 RID: 1361
		private bool isResizing;

		// Token: 0x04000552 RID: 1362
		private bool isMoving;

		// Token: 0x04000553 RID: 1363
		private bool isSelecting;

		// Token: 0x04000554 RID: 1364
		private List<System.Windows.Controls.Button> buttons = new List<System.Windows.Controls.Button>();

		// Token: 0x04000555 RID: 1365
		private DateTime mouseDownTime;

		// Token: 0x04000556 RID: 1366
		private List<TemplateJsonObject> previousMoves = new List<TemplateJsonObject>();

		// Token: 0x04000557 RID: 1367
		private List<TemplateJsonObject> undoneMoves = new List<TemplateJsonObject>();

		// Token: 0x04000558 RID: 1368
		private TemplateJsonObject lastVersionBeforeRedone = new TemplateJsonObject();

		// Token: 0x04000559 RID: 1369
		private TemplateJsonObject lastVersionBeforeUndone = new TemplateJsonObject();

		// Token: 0x0400055A RID: 1370
		private TemplateJsonObject lastVersionAfterUndone = new TemplateJsonObject();

		// Token: 0x0400055B RID: 1371
		private TemplateJsonObject lastVersionBeforeMouseUp = new TemplateJsonObject();

		// Token: 0x0400055C RID: 1372
		public Action<int, string> photoIDChangedTemplatePage;

		// Token: 0x0400055D RID: 1373
		private List<Canvas> canvasList = new List<Canvas>();

		// Token: 0x0400055E RID: 1374
		private static TemplatePageVideo thisTemplatePage = null;

		// Token: 0x0400055F RID: 1375
		private bool isPortraitScreen;

		// Token: 0x04000560 RID: 1376
		public static string templateImagesFolderName = "TemplateImages";

		// Token: 0x04000561 RID: 1377
		private System.Windows.Size LastSize = new System.Windows.Size(0.0, 0.0);

		// Token: 0x04000562 RID: 1378
		private bool noChangePaper;

		// Token: 0x04000563 RID: 1379
		private System.Windows.Controls.TextBox[] SizeTextBoxs;

		// Token: 0x04000564 RID: 1380
		private System.Windows.Controls.TextBox[] PositionsTextBoxs;

		// Token: 0x04000565 RID: 1381
		private int maxUndoCount = 5;

		// Token: 0x020001E7 RID: 487
		public class ImageElement
		{
			// Token: 0x17000199 RID: 409
			// (get) Token: 0x06000DEF RID: 3567 RVA: 0x00057E92 File Offset: 0x00056092
			// (set) Token: 0x06000DF0 RID: 3568 RVA: 0x00057E9A File Offset: 0x0005609A
			public string Name { get; set; }

			// Token: 0x1700019A RID: 410
			// (get) Token: 0x06000DF1 RID: 3569 RVA: 0x00057EA3 File Offset: 0x000560A3
			// (set) Token: 0x06000DF2 RID: 3570 RVA: 0x00057EAB File Offset: 0x000560AB
			public string ID { get; set; }
		}
	}
}
