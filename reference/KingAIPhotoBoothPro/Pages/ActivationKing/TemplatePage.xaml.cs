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
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;

namespace KingAIPhotoBoothPro.Pages.ActivationKing
{
	// Token: 0x02000045 RID: 69
	public partial class TemplatePage : SettingsSubPage, IStyleConnector
	{
		// Token: 0x060004E6 RID: 1254 RVA: 0x0001B78B File Offset: 0x0001998B
		public static TemplatePage GetTemplatePage()
		{
			return TemplatePage.thisTemplatePage;
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x0001B794 File Offset: 0x00019994
		public TemplatePage()
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
			TemplatePage.thisTemplatePage = this;
		}

		// Token: 0x060004E8 RID: 1256 RVA: 0x0001B8C6 File Offset: 0x00019AC6
		private void App_ApplicationExit(object sender, EventArgs e)
		{
			TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasID);
		}

		// Token: 0x060004E9 RID: 1257 RVA: 0x0001B8D2 File Offset: 0x00019AD2
		public override string GetTitle()
		{
			return "Image Template Settings";
		}

		// Token: 0x060004EA RID: 1258 RVA: 0x0001B8D9 File Offset: 0x00019AD9
		public override string GetSubTitle()
		{
			return "Please customise your photo template for your event.";
		}

		// Token: 0x060004EB RID: 1259 RVA: 0x0001B8E0 File Offset: 0x00019AE0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			TemplatePage.<Page_Loaded>d__40 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<TemplatePage.<Page_Loaded>d__40>(ref <Page_Loaded>d__);
		}

		// Token: 0x060004EC RID: 1260 RVA: 0x0001B917 File Offset: 0x00019B17
		private void InitialSelection()
		{
			if (this.isPortraitScreen)
			{
				this.imageListBoxPortrait.SelectedIndex = 0;
				return;
			}
			this.imageListBox.SelectedIndex = 0;
		}

		// Token: 0x060004ED RID: 1261 RVA: 0x0001B93C File Offset: 0x00019B3C
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
			TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasID);
			TemplateClass.ReadJsonTemplateObjects(true);
			this.imageNameList.Clear();
			this.ImageIDList.Clear();
			this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
			ActivationKingWindow.CloseFontWindow();
		}

		// Token: 0x060004EE RID: 1262 RVA: 0x0001B990 File Offset: 0x00019B90
		private void SetCanvasSize(double width, double height)
		{
			TemplatePage.canvasScale = Math.Min(this.canvasInitialWidth / width, this.canvasInitialHeight / height);
			this.canvasList[TemplateClass.selectedCanvasID].Width = TemplatePage.canvasScale * width;
			this.canvasList[TemplateClass.selectedCanvasID].Height = TemplatePage.canvasScale * height;
		}

		// Token: 0x060004EF RID: 1263 RVA: 0x0001B9F3 File Offset: 0x00019BF3
		private void ClearTemplate(object sender, RoutedEventArgs e)
		{
			TemplateClass.TemplateObjectsPhoto.Clear();
			this.imageNameList.Clear();
			this.ImageIDList.Clear();
			this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
		}

		// Token: 0x060004F0 RID: 1264 RVA: 0x0001BA30 File Offset: 0x00019C30
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
			TemplateObject delElement = TemplateClass.TemplateObjectsPhoto.Find((TemplateObject x) => x.ID == Convert.ToInt32(element.Uid));
			if (!delElement.isCapture)
			{
				if (delElement.isText)
				{
					this.AddTextButton.IsEnabled = true;
					TemplatePage.AddedText = null;
					ActivationKingWindow.CloseFontWindow();
				}
				this.DeleteElementProcess(element, delElement);
				foreach (TemplateObject templateObj in TemplateClass.TemplateObjectsPhoto)
				{
					if (templateObj.layer > delElement.layer)
					{
						templateObj.layer--;
					}
				}
				this.UpdateUndoButton();
				this.UpdateRedoButton();
				return;
			}
			List<TemplateObject> capturedObjects = (from item in TemplateClass.TemplateObjectsPhoto
			where item.isCapture
			select item).ToList<TemplateObject>();
			if (capturedObjects.Count <= 1)
			{
				MessageBoxWindow.CreateWindow("MAIN PHOTO ICON", "You Cannot Remove This Photo! Because you need at least one photo icon.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			this.DeleteElementProcess(element, delElement);
			foreach (TemplateObject templateObj2 in TemplateClass.TemplateObjectsPhoto)
			{
				if (templateObj2.layer > delElement.layer)
				{
					templateObj2.layer--;
				}
			}
			this.UpdateUndoButton();
			this.UpdateRedoButton();
		}

		// Token: 0x060004F1 RID: 1265 RVA: 0x0001BC54 File Offset: 0x00019E54
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
				this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(element);
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

		// Token: 0x060004F2 RID: 1266 RVA: 0x0001BD22 File Offset: 0x00019F22
		public void RefreshTemplate()
		{
			this.LastSize = new System.Windows.Size(0.0, 0.0);
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x0001BD44 File Offset: 0x00019F44
		private void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Pressed)
			{
				this.UpdatePreviousMoves();
				this.previousMousePosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]);
				System.Windows.Controls.Button button = (System.Windows.Controls.Button)sender;
				button.CaptureMouse();
				if (button.Name != "buttonMove")
				{
					this.isResizing = true;
				}
			}
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x0001BDA4 File Offset: 0x00019FA4
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
					double newWidth = Math.Max(50.0, oldWidth - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).X - this.previousMousePosition.X));
					double newHeight = Math.Max(50.0, oldHeight - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).Y - this.previousMousePosition.Y));
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
					double newWidth2 = Math.Max(50.0, oldWidth + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).X - this.previousMousePosition.X));
					double newHeight2 = Math.Max(50.0, oldHeight + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).Y - this.previousMousePosition.Y));
					if (this.keepAspectRatioCheckBox.IsChecked.Value)
					{
						newHeight2 = newWidth2 / oldWidth * oldHeight;
					}
					((FrameworkElement)this.lastSelectedElement).Width = newWidth2;
					((FrameworkElement)this.lastSelectedElement).Height = newHeight2;
				}
				else if (button.Name == "buttonTopRight")
				{
					double newWidth3 = Math.Max(50.0, oldWidth + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).X - this.previousMousePosition.X));
					double newHeight3 = Math.Max(50.0, oldHeight - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).Y - this.previousMousePosition.Y));
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
					double newWidth4 = Math.Max(50.0, oldWidth - (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).X - this.previousMousePosition.X));
					double newHeight4 = Math.Max(50.0, oldHeight + (e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]).Y - this.previousMousePosition.Y));
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
				this.previousMousePosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]);
				this.UpdateRectPositionAndSize(this.lastSelectedElement);
				this.UpdateButtonsPositions(this.lastSelectedElement);
				this.UpdatePositionText(this.lastSelectedElement);
				this.UpdateSizeText(this.lastSelectedElement);
				this.UpdateMoveButtonPosition(this.lastSelectedElement);
			}
		}

		// Token: 0x060004F5 RID: 1269 RVA: 0x0001C2E4 File Offset: 0x0001A4E4
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

		// Token: 0x060004F6 RID: 1270 RVA: 0x0001C347 File Offset: 0x0001A547
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

		// Token: 0x060004F7 RID: 1271 RVA: 0x0001C368 File Offset: 0x0001A568
		private void PageMouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.OriginalSource != this.positionXTextBox && e.OriginalSource != this.positionXTextBoxPortrait && e.OriginalSource != this.positionYTextBox && e.OriginalSource != this.positionYTextBoxPortrait && e.OriginalSource != this.heightTextBox && e.OriginalSource != this.heightTextBoxPortrait && e.OriginalSource != this.widthTextBox && e.OriginalSource != this.widthTextBoxPortrait)
			{
				this.dummyButton.Focus();
			}
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x0001C3F4 File Offset: 0x0001A5F4
		private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			DependencyObject source = e.OriginalSource as DependencyObject;
			while (source != null && !(source is Canvas))
			{
				System.Windows.Controls.Button button = source as System.Windows.Controls.Button;
				if (button != null && button.Name != "buttonMove")
				{
					return;
				}
				source = VisualTreeHelper.GetParent(source);
			}
			if (this.isMoving || this.isResizing)
			{
				return;
			}
			this.UpdatePreviousMoves();
			if (!(sender is System.Windows.Controls.Image) && !(sender is Grid) && !(sender is Frame))
			{
				if (sender is System.Windows.Controls.Button)
				{
					this.isMoving = true;
					this.elementClickedPosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]);
					this.elementClickedPosition.Y = this.elementClickedPosition.Y - Canvas.GetTop(this.buttonMove);
					this.elementClickedPosition.X = this.elementClickedPosition.X - Canvas.GetLeft(this.buttonMove);
					this.canvasList[TemplateClass.selectedCanvasID].CaptureMouse();
				}
				return;
			}
			this.isMoving = true;
			this.mouseDownTime = DateTime.Now;
			if (this.buttonMove != null)
			{
				this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(this.buttonMove);
				this.buttonMove = null;
			}
			this.selectedElement = (sender as UIElement);
			TemplateObject selectedObject = TemplateClass.TemplateObjectsPhoto.Find((TemplateObject x) => x.ID == Convert.ToInt32(this.selectedElement.Uid));
			if (selectedObject == null)
			{
				return;
			}
			this.imageListBox.SelectedIndex = selectedObject.layer;
			this.imageListBoxPortrait.SelectedIndex = selectedObject.layer;
			this.lastSelectedElement = this.selectedElement;
			this.elementClickedPosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]);
			this.elementClickedPosition.Y = this.elementClickedPosition.Y - Canvas.GetTop(this.lastSelectedElement);
			this.elementClickedPosition.X = this.elementClickedPosition.X - Canvas.GetLeft(this.lastSelectedElement);
			this.canvasList[TemplateClass.selectedCanvasID].CaptureMouse();
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x0001C5F0 File Offset: 0x0001A7F0
		private void Canvas_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			TimeSpan timedif = DateTime.Now - this.mouseDownTime;
			if (this.selectedElement == null || !this.canvasList[TemplateClass.selectedCanvasID].IsMouseCaptured)
			{
				if (this.buttonMove != null && this.canvasList[TemplateClass.selectedCanvasID].IsMouseCaptured)
				{
					System.Windows.Point currentPosition = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]);
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
			System.Windows.Point currentPosition2 = e.GetPosition(this.canvasList[TemplateClass.selectedCanvasID]);
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

		// Token: 0x060004FA RID: 1274 RVA: 0x0001C7A8 File Offset: 0x0001A9A8
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
			this.canvasList[TemplateClass.selectedCanvasID].ReleaseMouseCapture();
			this.isMoving = false;
			if (this.lastSelectedElement == null)
			{
				this.ClearButtonsAndRect();
				this.UpdateUndoneMoves();
				this.UpdateUndoButton();
			}
			if (this.lastSelectedElement != null)
			{
				if (this.IsOverlap(TemplateClass.GetObject(Convert.ToInt32(this.lastSelectedElement.Uid), true)))
				{
					this.CreateMoveButton();
				}
				else
				{
					this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(this.buttonMove);
					this.buttonMove = null;
				}
			}
			this.UpdateUndoneMoves();
			this.UpdateUndoButton();
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x0001C8CC File Offset: 0x0001AACC
		private void AddPhotoButton_Click(object sender, RoutedEventArgs e)
		{
			string photoIconName = TemplateClass.GetPhotoIconName();
			FileInfo fileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName));
			int IDNum = TemplateClass.TemplateObjectsPhoto.Count + this.deletedElements.Count;
			int layer = TemplateClass.TemplateObjectsPhoto.Count;
			TemplateObject newTemplateObject = this.CreateTemplateObject(fileInfo, IDNum, true, layer, false);
			TemplateClass.TemplateObjectsPhoto.Add(newTemplateObject);
			Frame addedFrame = this.AddPhotoPrefab(newTemplateObject);
			this.UpdateListBox(addedFrame, TemplateClass.TemplateObjectsPhoto, layer);
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x0001C950 File Offset: 0x0001AB50
		private void AddImagesFromTemplateJson(TemplateJsonObject templateJsonObj)
		{
			this.ImageIDList.Clear();
			this.imageNameList.Clear();
			this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
			List<TemplateObject> currentObject = templateJsonObj.templatePhotoObjects;
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
					UIElement addedElement = null;
					if (currentObject[i].isCapture)
					{
						addedElement = this.AddPhotoPrefab(currentObject[i]);
					}
					else if (currentObject[i].isText)
					{
						addedElement = this.AddTextPrefab(currentObject[i]);
					}
					else if (currentObject[i].isRawPhoto)
					{
						if (Mode.MonofunctionalPhotoBooth == MainSettingsPage.SelectedMode)
						{
							addedElement = this.AddRawPhotoPrefab(currentObject[i]);
						}
					}
					else
					{
						addedElement = this.AddImagePrefab(currentObject[i]);
					}
					if (addedElement != null)
					{
						this.UpdateListBox(addedElement, currentObject, i);
					}
				}
			}
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x0001CA94 File Offset: 0x0001AC94
		private Frame AddRawPhotoPrefab(TemplateObject templateObject)
		{
			string photoIconName = TemplateClass.GetRawPhotoIconName();
			FileInfo fileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName));
			if (fileInfo.Name != templateObject.fileInfo.Name)
			{
				templateObject.fileInfo = fileInfo;
			}
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
			BitmapImage image = new BitmapImage();
			using (FileStream stream = new FileStream(templateObject.fileInfo.FullName, FileMode.Open, FileAccess.Read))
			{
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.StreamSource = stream;
				image.EndInit();
			}
			imagePrefab.image.Source = image;
			imageFrame.Content = imagePrefab;
			this.SetFrameProperties(imageFrame, templateObject);
			imagePrefab.Uid = imageFrame.Uid;
			this.canvasList[TemplateClass.selectedCanvasID].Children.Add(imageFrame);
			return imageFrame;
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x0001CCFC File Offset: 0x0001AEFC
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
			List<TemplateObject> templateObjs = TemplateClass.TemplateObjectsPhoto;
			photoFrame.Content = photoPrefab;
			this.SetFrameProperties(photoFrame, templateObject);
			photoPrefab.Uid = photoFrame.Uid;
			this.canvasList[TemplateClass.selectedCanvasID].Children.Add(photoFrame);
			this.photoIDChangedTemplatePage = (Action<int, string>)Delegate.Combine(this.photoIDChangedTemplatePage, new Action<int, string>(photoPrefab.PhotoIDChangedTemplatePage));
			return photoFrame;
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x0001CF64 File Offset: 0x0001B164
		private void PhotoIDChanged(int value, string prefabUid)
		{
			foreach (TemplateObject item in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects)
			{
				if (item.ID == int.Parse(prefabUid))
				{
					item.photoID = value - 1;
				}
			}
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x0001CFD8 File Offset: 0x0001B1D8
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
			BitmapImage image = new BitmapImage();
			using (FileStream stream = new FileStream(templateObject.fileInfo.FullName, FileMode.Open, FileAccess.Read))
			{
				image.BeginInit();
				image.CacheOption = BitmapCacheOption.OnLoad;
				image.StreamSource = stream;
				image.EndInit();
			}
			imagePrefab.image.Source = image;
			imageFrame.Content = imagePrefab;
			this.SetFrameProperties(imageFrame, templateObject);
			imagePrefab.Uid = imageFrame.Uid;
			this.canvasList[TemplateClass.selectedCanvasID].Children.Add(imageFrame);
			return imageFrame;
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x0001D1F4 File Offset: 0x0001B3F4
		private Frame AddTextPrefab(TemplateObject templateObject)
		{
			Frame imageFrame = new Frame();
			TextPrefab imagePrefab = new TextPrefab();
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
			imageFrame.Content = imagePrefab;
			templateObject.objectSize = new System.Drawing.Size(TemplateClass.PaperWidthPhoto / 3, TemplateClass.PaperHeightPhoto / 3);
			templateObject.objectPoint = new System.Drawing.Point(TemplateClass.PaperWidthPhoto / 3, TemplateClass.PaperHeightPhoto / 3);
			this.SetFrameProperties(imageFrame, templateObject);
			imagePrefab.Uid = imageFrame.Uid;
			TemplatePage.AddedText = imagePrefab.textBox;
			TemplatePage.AddedText.FontFamily = ExtensionMethod.FindFont(Settings.GetValueString("selectedfontname"));
			TemplatePage.AddedText.Text = (string.IsNullOrEmpty(Settings.GetValueString("selectedexampletext")) ? "Text" : Settings.GetValueString("selectedexampletext"));
			TemplatePage.AddedText.FontSize = Math.Max((double)Settings.GetValueInt("selectedfontsize").Value * TemplatePage.canvasScale * 1.3300000429153442, TemplatePage.canvasScale);
			TemplatePage.AddedText.Foreground = new SolidColorBrush(Settings.GetValueColor("selectedfontcolor"));
			this.canvasList[TemplateClass.selectedCanvasID].Children.Add(imageFrame);
			return imageFrame;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0001D488 File Offset: 0x0001B688
		private void SetFrameProperties(Frame frame, TemplateObject templateObject)
		{
			double width = (double)templateObject.objectSize.Width * TemplatePage.canvasScale;
			double height = (double)templateObject.objectSize.Height * TemplatePage.canvasScale;
			height = ((height < 0.0) ? ((double)TemplateClass.PaperHeightPhoto) : Math.Min(10000.0, height));
			width = ((width < 0.0) ? ((double)TemplateClass.PaperWidthPhoto) : Math.Min(10000.0, width));
			frame.Width = width;
			frame.Height = height;
			frame.PreviewMouseDown += this.Canvas_PreviewMouseDown;
			frame.Uid = templateObject.ID.ToString();
			Canvas.SetLeft(frame, (double)templateObject.objectPoint.X * TemplatePage.canvasScale);
			Canvas.SetTop(frame, (double)templateObject.objectPoint.Y * TemplatePage.canvasScale);
			System.Windows.Controls.Panel.SetZIndex(frame, templateObject.layer);
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x0001D570 File Offset: 0x0001B770
		private void ChangePhotoID(UIElement lastSelected)
		{
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0001D574 File Offset: 0x0001B774
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
				this.canvasList[TemplateClass.selectedCanvasID].Children.Add(this.buttonMove);
				this.UpdateMoveButtonPosition(this.lastSelectedElement);
				System.Windows.Controls.Panel.SetZIndex(this.buttonMove, TemplateClass.TemplateObjectsPhoto.Count + 1);
				this.buttonMove.Name = "buttonMove";
				this.buttonMove.PreviewMouseDown += this.Canvas_PreviewMouseDown;
				this.buttonMove.PreviewMouseUp += this.Canvas_PreviewMouseUp;
				this.buttonMove.MouseMove += this.Button_MouseMove;
				this.buttonMove.MouseLeave += this.Button_MouseLeave;
			}
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x0001D6A4 File Offset: 0x0001B8A4
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
				int IDNum = TemplateClass.TemplateObjectsPhoto.Count + this.deletedElements.Count;
				int layer = TemplateClass.TemplateObjectsPhoto.Count;
				TemplateObject newTemplateObject = this.CreateTemplateObject(fileInfo, IDNum, false, layer, false);
				TemplateClass.TemplateObjectsPhoto.Add(newTemplateObject);
				Frame addedFrame = this.AddImagePrefab(newTemplateObject);
				this.UpdateListBox(addedFrame, TemplateClass.TemplateObjectsPhoto, layer);
			}
		}

		// Token: 0x06000506 RID: 1286 RVA: 0x0001D828 File Offset: 0x0001BA28
		private TemplateObject CreateTemplateObject(FileInfo fileInfo, int IDNum, bool isCapture, int layer, bool IsText = false)
		{
			BitmapImage bitmapImage = new BitmapImage(new Uri(fileInfo.FullName, UriKind.Absolute));
			return new TemplateObject
			{
				isCapture = isCapture,
				fileInfo = fileInfo,
				ID = IDNum,
				layer = layer,
				isText = IsText,
				objectPoint = new System.Drawing.Point(0, 0),
				objectSize = new System.Drawing.Size(bitmapImage.PixelWidth, bitmapImage.PixelHeight)
			};
		}

		// Token: 0x06000507 RID: 1287 RVA: 0x0001D898 File Offset: 0x0001BA98
		private string GetTemplateFilePath(string originalFileName)
		{
			string targetDirectory = System.IO.Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, TemplatePage.templateImagesFolderName);
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

		// Token: 0x06000508 RID: 1288 RVA: 0x0001D910 File Offset: 0x0001BB10
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
			TemplateObject selectedImage = TemplateClass.TemplateObjectsPhoto.Find((TemplateObject x) => x.ID == Convert.ToInt32(selectedImageID));
			if (TemplateClass.TemplateObjectsPhoto.Count > selectedImage.layer + i && selectedImage.layer + i >= 0)
			{
				TemplateObject nextImage = TemplateClass.TemplateObjectsPhoto[selectedImage.layer + i];
				selectedImage.layer += i;
				nextImage.layer -= i;
				TemplateClass.TemplateObjectsPhoto.Sort((TemplateObject t1, TemplateObject t2) => t1.layer.CompareTo(t2.layer));
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
				foreach (object obj in this.canvasList[TemplateClass.selectedCanvasID].Children)
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

		// Token: 0x06000509 RID: 1289 RVA: 0x0001DBDC File Offset: 0x0001BDDC
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

		// Token: 0x0600050A RID: 1290 RVA: 0x0001DDC0 File Offset: 0x0001BFC0
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
			x *= TemplatePage.canvasScale;
			y *= TemplatePage.canvasScale;
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

		// Token: 0x0600050B RID: 1291 RVA: 0x0001DEE4 File Offset: 0x0001C0E4
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

		// Token: 0x0600050C RID: 1292 RVA: 0x0001DF48 File Offset: 0x0001C148
		private void UpdateImagePosition(UIElement referenceElement)
		{
			if (this.buttonMove != null && this.lastSelectedElement != null)
			{
				Canvas.SetLeft(this.lastSelectedElement, Canvas.GetLeft(referenceElement) - ((FrameworkElement)this.lastSelectedElement).Width / 2.0 + this.buttonMove.Width / 2.0);
				Canvas.SetTop(this.lastSelectedElement, Canvas.GetTop(referenceElement) - ((FrameworkElement)this.lastSelectedElement).Height / 2.0 + this.buttonMove.Height / 2.0);
			}
		}

		// Token: 0x0600050D RID: 1293 RVA: 0x0001DFF0 File Offset: 0x0001C1F0
		private void UpdateMoveButtonPosition(UIElement referenceElement)
		{
			if (this.buttonMove != null)
			{
				Canvas.SetLeft(this.buttonMove, Canvas.GetLeft(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Width / 2.0 - this.buttonMove.Width / 2.0);
				Canvas.SetTop(this.buttonMove, Canvas.GetTop(this.lastSelectedElement) + ((FrameworkElement)this.lastSelectedElement).Height / 2.0 - this.buttonMove.Height / 2.0);
			}
		}

		// Token: 0x0600050E RID: 1294 RVA: 0x0001E098 File Offset: 0x0001C298
		private void UpdatePositionText(UIElement referenceElement)
		{
			double leftPos = Canvas.GetLeft(referenceElement) / TemplatePage.canvasScale;
			double topPos = Canvas.GetTop(referenceElement) / TemplatePage.canvasScale;
			if (this.isPortraitScreen)
			{
				this.positionXTextBoxPortrait.Text = ((int)Math.Round(leftPos)).ToString();
				this.positionYTextBoxPortrait.Text = ((int)Math.Round(topPos)).ToString();
				return;
			}
			this.positionXTextBox.Text = ((int)Math.Round(leftPos)).ToString();
			this.positionYTextBox.Text = ((int)Math.Round(topPos)).ToString();
		}

		// Token: 0x0600050F RID: 1295 RVA: 0x0001E130 File Offset: 0x0001C330
		private void UpdateSizeText(UIElement referenceElement)
		{
			double width = ((FrameworkElement)referenceElement).Width / TemplatePage.canvasScale;
			double height = ((FrameworkElement)referenceElement).Height / TemplatePage.canvasScale;
			if (this.isPortraitScreen)
			{
				this.widthTextBoxPortrait.Text = ((int)Math.Round(width)).ToString();
				this.heightTextBoxPortrait.Text = ((int)Math.Round(height)).ToString();
				return;
			}
			this.widthTextBox.Text = ((int)Math.Round(width)).ToString();
			this.heightTextBox.Text = ((int)Math.Round(height)).ToString();
		}

		// Token: 0x06000510 RID: 1296 RVA: 0x0001E1D4 File Offset: 0x0001C3D4
		private void UpdateImageSizeByText()
		{
			if (this.lastSelectedElement == null)
			{
				return;
			}
			double newWidth = 0.0;
			double newHeight = 0.0;
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
			((FrameworkElement)this.lastSelectedElement).Width = newWidth * TemplatePage.canvasScale;
			((FrameworkElement)this.lastSelectedElement).Height = newHeight * TemplatePage.canvasScale;
			this.UpdateTemplateClassImageSize(this.lastSelectedElement);
			this.UpdateRectPositionAndSize(this.lastSelectedElement);
			this.UpdatePositionText(this.lastSelectedElement);
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

		// Token: 0x06000511 RID: 1297 RVA: 0x0001E414 File Offset: 0x0001C614
		private void UpdateTemplateClassImageSize(UIElement referenceElement)
		{
			TemplateObject templateObject = TemplateClass.GetObject(Convert.ToInt32(referenceElement.Uid), true);
			if (templateObject == null)
			{
				return;
			}
			double width = ((FrameworkElement)referenceElement).Width / TemplatePage.canvasScale;
			double height = ((FrameworkElement)referenceElement).Height / TemplatePage.canvasScale;
			templateObject.objectSize = new System.Drawing.Size((int)Math.Round(width), (int)Math.Round(height));
		}

		// Token: 0x06000512 RID: 1298 RVA: 0x0001E474 File Offset: 0x0001C674
		private void UpdateTemplateClassImagePosition(UIElement referenceElement)
		{
			TemplateObject templateObject = TemplateClass.GetObject(Convert.ToInt32(referenceElement.Uid), true);
			if (templateObject == null)
			{
				return;
			}
			double leftPos = Canvas.GetLeft(referenceElement) / TemplatePage.canvasScale;
			double topPos = Canvas.GetTop(referenceElement) / TemplatePage.canvasScale;
			templateObject.objectPoint = new System.Drawing.Point((int)Math.Round(leftPos), (int)Math.Round(topPos));
		}

		// Token: 0x06000513 RID: 1299 RVA: 0x0001E4CA File Offset: 0x0001C6CA
		private void UpdateImageRotation()
		{
		}

		// Token: 0x06000514 RID: 1300 RVA: 0x0001E4CC File Offset: 0x0001C6CC
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

		// Token: 0x06000515 RID: 1301 RVA: 0x0001E50C File Offset: 0x0001C70C
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

		// Token: 0x06000516 RID: 1302 RVA: 0x0001E580 File Offset: 0x0001C780
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
			TemplateClass.PaperWidthPhoto = result.Value.Item2.Width;
			TemplateClass.PaperHeightPhoto = result.Value.Item2.Height;
			this.PaperSizeListBox.ToolTip = string.Format("{0}px : {1}px", result.Value.Item2.Width, result.Value.Item2.Height);
			TemplateClass.PageInfo = selectedValue;
			TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageInfo = selectedValue;
			this.SetCanvasSize((double)TemplateClass.PaperWidthPhoto, (double)TemplateClass.PaperHeightPhoto);
			this.ImageIDList.Clear();
			this.imageNameList.Clear();
			this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
			TemplateJsonObject templateJsonObject = this.GetTemplateJsonObject();
			System.Windows.Size NewSize = new System.Windows.Size((double)TemplateClass.PaperWidthPhoto, (double)TemplateClass.PaperHeightPhoto);
			if ((this.LastSize.Width != 0.0 || this.LastSize.Height != 0.0) && this.LastSize != NewSize)
			{
				templateJsonObject.templatePhotoObjects = this.ChangePaperSize(templateJsonObject, NewSize);
			}
			this.AddImagesFromTemplateJson(templateJsonObject);
			this.UpdatePreviousMoves();
			this.LastSize = NewSize;
		}

		// Token: 0x06000517 RID: 1303 RVA: 0x0001E744 File Offset: 0x0001C944
		private List<TemplateObject> ChangePaperSize(TemplateJsonObject templateJsonObject, System.Windows.Size NewSize)
		{
			List<TemplateObject> currentObject = templateJsonObject.templatePhotoObjects;
			for (int i = 0; i < currentObject.Count; i++)
			{
				currentObject[i].objectPoint = new System.Drawing.Point((int)((double)((float)currentObject[i].objectPoint.X) * (NewSize.Width / this.LastSize.Width)), (int)((double)((float)currentObject[i].objectPoint.Y) * (NewSize.Height / this.LastSize.Height)));
				currentObject[i].objectSize = new System.Drawing.Size((int)((double)((float)currentObject[i].objectSize.Width) * (NewSize.Width / this.LastSize.Width)), (int)((double)((float)currentObject[i].objectSize.Height) * (NewSize.Height / this.LastSize.Height)));
			}
			return currentObject;
		}

		// Token: 0x06000518 RID: 1304 RVA: 0x0001E834 File Offset: 0x0001CA34
		private void PaperSizeBox_Loaded(object sender, RoutedEventArgs e)
		{
			if (this.PaperSizeListBox.ItemsSource == null)
			{
				this.PaperSizeListBox.ItemsSource = PaperSizeData.GetNames();
				this.PaperSizeListBoxPortrait.ItemsSource = PaperSizeData.GetNames();
			}
			ValueTuple<PaperSizeEnum, PaperSize>? result = PaperSizeData.GetPaperSize(TemplateClass.PaperWidthPhoto, TemplateClass.PaperHeightPhoto);
			if (result != null)
			{
				this.PaperSizeListBox.SelectedItem = result.Value.Item2.Name;
				this.PaperSizeListBoxPortrait.SelectedItem = result.Value.Item2.Name;
				return;
			}
			this.PaperSizeListBox.SelectedIndex = 0;
			this.PaperSizeListBoxPortrait.SelectedIndex = 0;
		}

		// Token: 0x06000519 RID: 1305 RVA: 0x0001E8D8 File Offset: 0x0001CAD8
		private UIElement GetElementByID(string ID)
		{
			for (int i = 0; i < this.canvasList[TemplateClass.selectedCanvasID].Children.Count; i++)
			{
				if (this.canvasList[TemplateClass.selectedCanvasID].Children[i].Uid == ID)
				{
					return this.canvasList[TemplateClass.selectedCanvasID].Children[i];
				}
			}
			return null;
		}

		// Token: 0x0600051A RID: 1306 RVA: 0x0001E954 File Offset: 0x0001CB54
		private UIElement SelectItemByID(string selectedItemID)
		{
			this.isSelecting = true;
			if (this.buttonMove != null)
			{
				this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(this.buttonMove);
				this.buttonMove = null;
			}
			foreach (System.Windows.Controls.Button button in this.buttons)
			{
				button.Visibility = Visibility.Hidden;
			}
			this.buttons.Clear();
			foreach (TemplateObject item in TemplateClass.TemplateObjectsPhoto)
			{
			}
			for (int i = 0; i < this.canvasList[TemplateClass.selectedCanvasID].Children.Count; i++)
			{
				if (this.canvasList[TemplateClass.selectedCanvasID].Children[i].Uid == selectedItemID)
				{
					this.previousSelectedElement = this.lastSelectedElement;
					this.lastSelectedElement = this.canvasList[TemplateClass.selectedCanvasID].Children[i];
					if (this.previousSelectedElement != this.lastSelectedElement)
					{
						Frame previousFrame = this.previousSelectedElement as Frame;
						if (previousFrame != null)
						{
							List<System.Windows.Controls.Button> buttonsToRemove = new List<System.Windows.Controls.Button>();
							ImagePrefab imagePrefab = previousFrame.Content as ImagePrefab;
							if (imagePrefab != null)
							{
								imagePrefab.buttonBottomLeft.Visibility = Visibility.Hidden;
								imagePrefab.buttonBottomRight.Visibility = Visibility.Hidden;
								imagePrefab.buttonTopLeft.Visibility = Visibility.Hidden;
								imagePrefab.buttonTopRight.Visibility = Visibility.Hidden;
								imagePrefab.border.Visibility = Visibility.Hidden;
							}
							else
							{
								PhotoPrefab photoPref = previousFrame.Content as PhotoPrefab;
								if (photoPref != null)
								{
									photoPref.buttonBottomLeft.Visibility = Visibility.Hidden;
									photoPref.buttonBottomRight.Visibility = Visibility.Hidden;
									photoPref.buttonTopLeft.Visibility = Visibility.Hidden;
									photoPref.buttonTopRight.Visibility = Visibility.Hidden;
									photoPref.border.Visibility = Visibility.Hidden;
								}
								else
								{
									TextPrefab textPref = previousFrame.Content as TextPrefab;
									if (textPref != null)
									{
										textPref.buttonBottomLeft.Visibility = Visibility.Hidden;
										textPref.buttonBottomRight.Visibility = Visibility.Hidden;
										textPref.buttonTopLeft.Visibility = Visibility.Hidden;
										textPref.buttonTopRight.Visibility = Visibility.Hidden;
										textPref.border.Visibility = Visibility.Hidden;
									}
								}
							}
							this.isFirstWork = false;
						}
					}
					this.UpdatePositionText(this.lastSelectedElement);
					this.UpdateSizeText(this.lastSelectedElement);
					TemplateObject selectedTemplateObject = TemplateClass.TemplateObjectsPhoto.Find((TemplateObject x) => x.ID == Convert.ToInt32(this.lastSelectedElement.Uid));
					bool isCapture = selectedTemplateObject.isCapture;
					if (selectedTemplateObject.isText)
					{
						ActivationKingWindow.OpenFontWindow();
					}
					Frame photoFrame = this.lastSelectedElement as Frame;
					if (photoFrame != null)
					{
						PhotoPrefab photoPrefab = photoFrame.Content as PhotoPrefab;
						if (photoPrefab != null)
						{
							this.buttons.Add(photoPrefab.buttonTopLeft);
							this.buttons.Add(photoPrefab.buttonTopRight);
							this.buttons.Add(photoPrefab.buttonBottomLeft);
							this.buttons.Add(photoPrefab.buttonBottomRight);
							photoPrefab.border.Visibility = Visibility.Visible;
						}
					}
					Frame imageFrame = this.lastSelectedElement as Frame;
					if (imageFrame != null)
					{
						ImagePrefab imagePrefab2 = imageFrame.Content as ImagePrefab;
						if (imagePrefab2 != null)
						{
							this.buttons.Add(imagePrefab2.buttonTopLeft);
							this.buttons.Add(imagePrefab2.buttonTopRight);
							this.buttons.Add(imagePrefab2.buttonBottomLeft);
							this.buttons.Add(imagePrefab2.buttonBottomRight);
							imagePrefab2.border.Visibility = Visibility.Visible;
						}
					}
					Frame TextFrame = this.lastSelectedElement as Frame;
					if (TextFrame != null)
					{
						TextPrefab textPrefab = TextFrame.Content as TextPrefab;
						if (textPrefab != null)
						{
							this.buttons.Add(textPrefab.buttonTopLeft);
							this.buttons.Add(textPrefab.buttonTopRight);
							this.buttons.Add(textPrefab.buttonBottomLeft);
							this.buttons.Add(textPrefab.buttonBottomRight);
							textPrefab.border.Visibility = Visibility.Visible;
						}
					}
					foreach (System.Windows.Controls.Button item2 in this.buttons)
					{
						item2.Visibility = Visibility.Visible;
					}
					TemplateObject selectedObject = TemplateClass.TemplateObjectsPhoto.Find((TemplateObject x) => x.ID == Convert.ToInt32(this.lastSelectedElement.Uid));
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

		// Token: 0x0600051B RID: 1307 RVA: 0x0001EE10 File Offset: 0x0001D010
		private void LayerBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
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
			if (this.ImageIDList.Count > selectedItemIDIndex)
			{
				string selectedItemID = this.ImageIDList[selectedItemIDIndex];
				this.SelectItemByID(selectedItemID);
			}
			this.SyncImageDeleteListBoxPortrait();
			this.SyncImageDeleteListBox();
		}

		// Token: 0x0600051C RID: 1308 RVA: 0x0001EEBC File Offset: 0x0001D0BC
		private string ReChanged(TextChangedEventArgs e, System.Windows.Controls.TextBox textBoxRef)
		{
			string retunText = textBoxRef.Text;
			return retunText.Replace(retunText.Substring(e.Changes.First<TextChange>().Offset, e.Changes.First<TextChange>().AddedLength), "");
		}

		// Token: 0x0600051D RID: 1309 RVA: 0x0001EF04 File Offset: 0x0001D104
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

		// Token: 0x0600051E RID: 1310 RVA: 0x0001EFF4 File Offset: 0x0001D1F4
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

		// Token: 0x0600051F RID: 1311 RVA: 0x0001F17C File Offset: 0x0001D37C
		private void RotationTextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			this.UpdateImageRotation();
		}

		// Token: 0x06000520 RID: 1312 RVA: 0x0001F184 File Offset: 0x0001D384
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

		// Token: 0x06000521 RID: 1313 RVA: 0x0001F1C0 File Offset: 0x0001D3C0
		private bool IsOverlap(TemplateObject selectedObject)
		{
			int layerCount = TemplateClass.TemplateObjectsPhoto.Count - 1;
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
					TemplateObject aTemplateObject = TemplateClass.TemplateObjectsPhoto.Find((TemplateObject x) => x.layer == i);
					if (aTemplateObject == null)
					{
						return false;
					}
					for (int j = 0; j < this.canvasList[TemplateClass.selectedCanvasID].Children.Count; j++)
					{
						if (this.canvasList[TemplateClass.selectedCanvasID].Children[j].Uid == aTemplateObject.ID.ToString())
						{
							UIElement anElement = this.canvasList[TemplateClass.selectedCanvasID].Children[j];
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

		// Token: 0x06000522 RID: 1314 RVA: 0x0001F34C File Offset: 0x0001D54C
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
			TemplateClass.PaperHeightPhoto = previousTemplateJsonObj.PageHeightPhoto;
			TemplateClass.PaperWidthPhoto = previousTemplateJsonObj.PageWidthPhoto;
			this.SetCanvasSize((double)TemplateClass.PaperWidthPhoto, (double)TemplateClass.PaperHeightPhoto);
			this.LayerUpdate(previousTemplateJsonObj.templatePhotoObjects);
			this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
			this.AddImagesFromTemplateJson(previousTemplateJsonObj);
			TemplateClass.TemplateObjectsPhoto = previousTemplateJsonObj.templatePhotoObjects;
			this.PaperSizeBox_Loaded(null, null);
			this.ClearButtonsAndRect();
			this.lastVersionAfterUndone = this.GetTemplateJsonObjectAsCopy();
			this.UpdateRedoButton();
			this.UpdateUndoButton();
			this.isResizing = false;
			this.isMoving = false;
		}

		// Token: 0x06000523 RID: 1315 RVA: 0x0001F50C File Offset: 0x0001D70C
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
			TemplateClass.PaperHeightPhoto = undoneTemplateJsonObj.PageHeightPhoto;
			TemplateClass.PaperWidthPhoto = undoneTemplateJsonObj.PageWidthPhoto;
			this.SetCanvasSize((double)TemplateClass.PaperWidthPhoto, (double)TemplateClass.PaperHeightPhoto);
			this.LayerUpdate(undoneTemplateJsonObj.templatePhotoObjects);
			this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
			this.AddImagesFromTemplateJson(undoneTemplateJsonObj);
			TemplateClass.TemplateObjectsPhoto = undoneTemplateJsonObj.templatePhotoObjects;
			this.PaperSizeBox_Loaded(null, null);
			this.ClearButtonsAndRect();
			this.UpdateRedoButton();
			this.UpdateUndoButton();
			this.isMoving = false;
			this.isResizing = false;
		}

		// Token: 0x06000524 RID: 1316 RVA: 0x0001F68C File Offset: 0x0001D88C
		private TemplateJsonObject GetTemplateJsonObjectAsCopy()
		{
			List<TemplateObject> templateObjects = new List<TemplateObject>();
			foreach (TemplateObject templateObj in TemplateClass.TemplateObjectsPhoto)
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
				PageHeightPhoto = TemplateClass.PaperHeightPhoto,
				PageWidthPhoto = TemplateClass.PaperWidthPhoto,
				PageHeightVideo = TemplateClass.PaperHeightVideo,
				PageWidthVideo = TemplateClass.PaperWidthVideo,
				PageInfo = "4'' x 6'' paper",
				templatePhotoObjects = templateObjects
			};
		}

		// Token: 0x06000525 RID: 1317 RVA: 0x0001F798 File Offset: 0x0001D998
		private TemplateJsonObject GetTemplateJsonObjectAsCopy(TemplateJsonObject templatejsonobj)
		{
			List<TemplateObject> templateObjects = new List<TemplateObject>();
			foreach (TemplateObject templateObj in templatejsonobj.templatePhotoObjects)
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
				PageHeightPhoto = templatejsonobj.PageHeightPhoto,
				PageWidthPhoto = templatejsonobj.PageWidthPhoto,
				PageHeightVideo = templatejsonobj.PageHeightVideo,
				PageWidthVideo = templatejsonobj.PageWidthVideo,
				PageInfo = "4'' x 6'' paper",
				templatePhotoObjects = templateObjects
			};
		}

		// Token: 0x06000526 RID: 1318 RVA: 0x0001F8A8 File Offset: 0x0001DAA8
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

		// Token: 0x06000527 RID: 1319 RVA: 0x0001F938 File Offset: 0x0001DB38
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

		// Token: 0x06000528 RID: 1320 RVA: 0x0001F9AC File Offset: 0x0001DBAC
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

		// Token: 0x06000529 RID: 1321 RVA: 0x0001F9E9 File Offset: 0x0001DBE9
		private void UpdateRedoButton()
		{
			if (this.undoneMoves.Count == 0)
			{
				this.RedoButton.IsEnabled = false;
				return;
			}
			this.RedoButton.IsEnabled = true;
		}

		// Token: 0x0600052A RID: 1322 RVA: 0x0001FA11 File Offset: 0x0001DC11
		private void ResetPreviousAndUndoneMoves()
		{
			this.lastVersionAfterUndone = null;
			this.undoneMoves.Clear();
			this.previousMoves.Clear();
			this.UpdateUndoButton();
			this.UpdateRedoButton();
		}

		// Token: 0x0600052B RID: 1323 RVA: 0x0001FA3C File Offset: 0x0001DC3C
		private TemplateJsonObject GetTemplateJsonObject()
		{
			return new TemplateJsonObject
			{
				DPI = 300,
				PageHeightPhoto = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageHeightPhoto,
				PageWidthPhoto = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageWidthPhoto,
				PageHeightVideo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageHeightVideo,
				PageWidthVideo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageWidthVideo,
				PageInfo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageInfo,
				templatePhotoObjects = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects
			};
		}

		// Token: 0x0600052C RID: 1324 RVA: 0x0001FAF8 File Offset: 0x0001DCF8
		private bool CompareTemplateJsonObjects(TemplateJsonObject t1, TemplateJsonObject t2)
		{
			if (t1.PageInfo == t2.PageInfo && t1.PageWidthPhoto == t2.PageWidthPhoto && t1.PageHeightPhoto == t2.PageHeightPhoto && t1.DPI == t2.DPI && t1.templatePhotoObjects.Count == t2.templatePhotoObjects.Count)
			{
				for (int i = 0; i < t1.templatePhotoObjects.Count; i++)
				{
					if (t1.templatePhotoObjects[i].ID != t2.templatePhotoObjects[i].ID)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].layer != t2.templatePhotoObjects[i].layer)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].fileInfo != t2.templatePhotoObjects[i].fileInfo)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].isCapture != t2.templatePhotoObjects[i].isCapture)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].objectPoint.X != t2.templatePhotoObjects[i].objectPoint.X)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].objectPoint.Y != t2.templatePhotoObjects[i].objectPoint.Y)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].objectSize.Width != t2.templatePhotoObjects[i].objectSize.Width)
					{
						return false;
					}
					if (t1.templatePhotoObjects[i].objectSize.Height != t2.templatePhotoObjects[i].objectSize.Height)
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		// Token: 0x0600052D RID: 1325 RVA: 0x0001FCE0 File Offset: 0x0001DEE0
		private void LayerUpdate(List<TemplateObject> templateObjects)
		{
			for (int i = 0; i < templateObjects.Count; i++)
			{
				templateObjects[i].layer = i;
			}
		}

		// Token: 0x0600052E RID: 1326 RVA: 0x0001FD0C File Offset: 0x0001DF0C
		private void ClearButtonsAndRect()
		{
			if (this.buttonMove != null)
			{
				this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(this.buttonMove);
				this.buttonMove = null;
			}
			foreach (System.Windows.Controls.Button button in this.buttons)
			{
				this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(button);
			}
			this.buttons.Clear();
			if (this.previousMoves.Count <= 0)
			{
				this.UndoButton.IsEnabled = false;
			}
			if (this.rect != null)
			{
				this.canvasList[TemplateClass.selectedCanvasID].Children.Remove(this.rect);
				this.rect = null;
			}
		}

		// Token: 0x0600052F RID: 1327 RVA: 0x0001FDF8 File Offset: 0x0001DFF8
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

		// Token: 0x06000530 RID: 1328 RVA: 0x0001FEB4 File Offset: 0x0001E0B4
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

		// Token: 0x06000531 RID: 1329 RVA: 0x0001FFD5 File Offset: 0x0001E1D5
		private void SettingsSubPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.ManagePortraitScreen();
		}

		// Token: 0x06000532 RID: 1330 RVA: 0x0001FFDD File Offset: 0x0001E1DD
		private void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			if (this.lastSelectedElement != null)
			{
				this.UpdateSizeText(this.lastSelectedElement);
				this.UpdatePositionText(this.lastSelectedElement);
			}
		}

		// Token: 0x06000533 RID: 1331 RVA: 0x00020000 File Offset: 0x0001E200
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

		// Token: 0x06000534 RID: 1332 RVA: 0x000200AC File Offset: 0x0001E2AC
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

		// Token: 0x06000535 RID: 1333 RVA: 0x00020188 File Offset: 0x0001E388
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

		// Token: 0x06000536 RID: 1334 RVA: 0x000201DC File Offset: 0x0001E3DC
		private void SyncImageDeleteListBoxPortrait()
		{
			this.imageListBoxDeletePortrait.Items.Clear();
			foreach (object item in ((IEnumerable)this.imageListBoxPortrait.Items))
			{
				ListBoxItem newItem = new ListBoxItem();
				this.imageListBoxDeletePortrait.Items.Add(newItem);
			}
		}

		// Token: 0x06000537 RID: 1335 RVA: 0x00020258 File Offset: 0x0001E458
		private void SyncImageDeleteListBox()
		{
			this.imageListBoxDelete.Items.Clear();
			foreach (object item in ((IEnumerable)this.imageListBox.Items))
			{
				ListBoxItem newItem = new ListBoxItem();
				this.imageListBoxDelete.Items.Add(newItem);
			}
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x000202D4 File Offset: 0x0001E4D4
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

		// Token: 0x06000539 RID: 1337 RVA: 0x00020358 File Offset: 0x0001E558
		private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			System.Windows.Controls.TabControl selectedItem = (System.Windows.Controls.TabControl)sender;
			if (TemplateClass.selectedCanvasID != selectedItem.SelectedIndex)
			{
				TemplateClass.WriteJsonTemplateObjects(TemplateClass.selectedCanvasID);
				TemplateClass.selectedCanvasID = selectedItem.SelectedIndex;
				TemplateClass.ReadJsonTemplateObjects(true);
				this.UpdatePreviousMoves();
				TemplateClass.PageInfo = PaperSizeData.GetPaperSize(TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageWidthPhoto, TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageHeightPhoto).Value.Item2.Name;
				int ID = PaperSizeData.GetPaperSizeIndex(TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageWidthPhoto, TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageHeightPhoto);
				this.noChangePaper = (this.PaperSizeListBox.SelectedIndex != ID);
				this.SetCanvasSize((double)TemplateClass.PaperWidthPhoto, (double)TemplateClass.PaperHeightPhoto);
				this.ImageIDList.Clear();
				this.imageNameList.Clear();
				this.canvasList[TemplateClass.selectedCanvasID].Children.Clear();
				TemplateJsonObject templateJsonObject = this.GetTemplateJsonObject();
				this.AddImagesFromTemplateJson(templateJsonObject);
				this.LastSize = new System.Windows.Size((double)TemplateClass.PaperWidthPhoto, (double)TemplateClass.PaperHeightPhoto);
				this.PaperSizeListBox.SelectedIndex = ID;
				this.UpdatePreviousMoves();
			}
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x000204A4 File Offset: 0x0001E6A4
		private void AddTextButton_Click(object sender, RoutedEventArgs e)
		{
			this.UpdatePreviousMoves();
			if ((from x in TemplateClass.TemplateObjectsPhoto
			where x.isText
			select x).Count<TemplateObject>() == 0)
			{
				string photoIconName = TemplateClass.GetTextIconName();
				FileInfo fileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName));
				int IDNum = TemplateClass.TemplateObjectsPhoto.Count + this.deletedElements.Count;
				int layer = TemplateClass.TemplateObjectsPhoto.Count;
				TemplateObject newTemplateObject = this.CreateTemplateObject(fileInfo, IDNum, false, layer, true);
				TemplateClass.TemplateObjectsPhoto.Add(newTemplateObject);
				Frame addedFrame = this.AddTextPrefab(newTemplateObject);
				this.UpdateListBox(addedFrame, TemplateClass.TemplateObjectsPhoto, layer);
			}
			else
			{
				this.SelectItemByID((from x in TemplateClass.TemplateObjectsPhoto
				where x.isText
				select x).FirstOrDefault<TemplateObject>().ID.ToString());
			}
			ActivationKingWindow.OpenFontWindow();
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x000205A4 File Offset: 0x0001E7A4
		private void AddRawButton_Click(object sender, RoutedEventArgs e)
		{
			this.UpdatePreviousMoves();
			if ((from x in TemplateClass.TemplateObjectsPhoto
			where x.isRawPhoto
			select x).Count<TemplateObject>() == 0)
			{
				string photoIconName = TemplateClass.GetRawPhotoIconName();
				FileInfo fileInfo = new FileInfo(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName));
				int IDNum = TemplateClass.TemplateObjectsPhoto.Count + this.deletedElements.Count;
				int layer = TemplateClass.TemplateObjectsPhoto.Count;
				TemplateObject newTemplateObject = this.CreateTemplateObject(fileInfo, IDNum, false, layer, false);
				newTemplateObject.isRawPhoto = true;
				TemplateClass.TemplateObjectsPhoto.Add(newTemplateObject);
				Frame addedFrame = this.AddRawPhotoPrefab(newTemplateObject);
				this.UpdateListBox(addedFrame, TemplateClass.TemplateObjectsPhoto, layer);
				return;
			}
			this.SelectItemByID((from x in TemplateClass.TemplateObjectsPhoto
			where x.isRawPhoto
			select x).FirstOrDefault<TemplateObject>().ID.ToString());
		}

		// Token: 0x0600053F RID: 1343 RVA: 0x00021061 File Offset: 0x0001F261
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IStyleConnector.Connect(int connectionId, object target)
		{
			if (connectionId == 25)
			{
				((System.Windows.Controls.Button)target).Click += this.ImageListBoxDeleteButton_Click;
				return;
			}
			if (connectionId != 53)
			{
				return;
			}
			((System.Windows.Controls.Button)target).Click += this.ImageListBoxDeleteButton_Click;
		}

		// Token: 0x040004DC RID: 1244
		private UIElement selectedElement;

		// Token: 0x040004DD RID: 1245
		private UIElement lastSelectedElement;

		// Token: 0x040004DE RID: 1246
		private UIElement previousSelectedElement;

		// Token: 0x040004DF RID: 1247
		private System.Windows.Controls.Button buttonMove = new System.Windows.Controls.Button();

		// Token: 0x040004E0 RID: 1248
		private System.Windows.Point elementClickedPosition;

		// Token: 0x040004E1 RID: 1249
		private System.Windows.Point previousMousePosition;

		// Token: 0x040004E2 RID: 1250
		private System.Windows.Shapes.Rectangle rect;

		// Token: 0x040004E3 RID: 1251
		public static double canvasScale;

		// Token: 0x040004E4 RID: 1252
		private double canvasInitialWidth;

		// Token: 0x040004E5 RID: 1253
		private double canvasInitialHeight;

		// Token: 0x040004E6 RID: 1254
		private List<UIElement> deletedElements = new List<UIElement>();

		// Token: 0x040004E7 RID: 1255
		private ObservableCollection<string> imageNameList = new ObservableCollection<string>();

		// Token: 0x040004E8 RID: 1256
		private List<string> ImageIDList = new List<string>();

		// Token: 0x040004E9 RID: 1257
		private bool isResizing;

		// Token: 0x040004EA RID: 1258
		private bool isMoving;

		// Token: 0x040004EB RID: 1259
		private bool isSelecting;

		// Token: 0x040004EC RID: 1260
		private List<System.Windows.Controls.Button> buttons = new List<System.Windows.Controls.Button>();

		// Token: 0x040004ED RID: 1261
		private DateTime mouseDownTime;

		// Token: 0x040004EE RID: 1262
		private List<TemplateJsonObject> previousMoves = new List<TemplateJsonObject>();

		// Token: 0x040004EF RID: 1263
		private List<TemplateJsonObject> undoneMoves = new List<TemplateJsonObject>();

		// Token: 0x040004F0 RID: 1264
		private TemplateJsonObject lastVersionBeforeRedone = new TemplateJsonObject();

		// Token: 0x040004F1 RID: 1265
		private TemplateJsonObject lastVersionBeforeUndone = new TemplateJsonObject();

		// Token: 0x040004F2 RID: 1266
		private TemplateJsonObject lastVersionAfterUndone = new TemplateJsonObject();

		// Token: 0x040004F3 RID: 1267
		private TemplateJsonObject lastVersionBeforeMouseUp = new TemplateJsonObject();

		// Token: 0x040004F4 RID: 1268
		private List<Canvas> canvasList = new List<Canvas>();

		// Token: 0x040004F5 RID: 1269
		private static TemplatePage thisTemplatePage = null;

		// Token: 0x040004F6 RID: 1270
		private bool isPortraitScreen;

		// Token: 0x040004F7 RID: 1271
		private bool isFirstWork = true;

		// Token: 0x040004F8 RID: 1272
		private bool noChangePaper;

		// Token: 0x040004F9 RID: 1273
		public static string templateImagesFolderName = "TemplateImages";

		// Token: 0x040004FA RID: 1274
		public Action<int, string> photoIDChangedTemplatePage;

		// Token: 0x040004FB RID: 1275
		private System.Windows.Size LastSize = new System.Windows.Size(0.0, 0.0);

		// Token: 0x040004FC RID: 1276
		public static System.Windows.Controls.TextBox AddedText;

		// Token: 0x040004FD RID: 1277
		private int maxUndoCount = 5;

		// Token: 0x020001DF RID: 479
		public class ImageElement
		{
			// Token: 0x17000197 RID: 407
			// (get) Token: 0x06000DD3 RID: 3539 RVA: 0x00057916 File Offset: 0x00055B16
			// (set) Token: 0x06000DD4 RID: 3540 RVA: 0x0005791E File Offset: 0x00055B1E
			public string Name { get; set; }

			// Token: 0x17000198 RID: 408
			// (get) Token: 0x06000DD5 RID: 3541 RVA: 0x00057927 File Offset: 0x00055B27
			// (set) Token: 0x06000DD6 RID: 3542 RVA: 0x0005792F File Offset: 0x00055B2F
			public string ID { get; set; }
		}
	}
}
