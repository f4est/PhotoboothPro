using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A9 RID: 169
	public static class TemplateClass
	{
		// Token: 0x0600099E RID: 2462 RVA: 0x0003682C File Offset: 0x00034A2C
		public static string GetDescription(this Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if (attributes.Length == 0)
			{
				return value.ToString();
			}
			return attributes[0].Description;
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x0600099F RID: 2463 RVA: 0x00036878 File Offset: 0x00034A78
		public static TemplateClass.WriteFontInfo GetFontInfo
		{
			get
			{
				Point point = (from x in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects
				where x.isText
				select x).FirstOrDefault<TemplateObject>().objectPoint;
				return new TemplateClass.WriteFontInfo
				{
					point = point,
					selectFont = ExtensionMethod.GetSelectedFont()
				};
			}
		}

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x060009A0 RID: 2464 RVA: 0x000368E0 File Offset: 0x00034AE0
		public static bool IsHaveText
		{
			get
			{
				return (from x in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects
				where x.isText
				select x).Count<TemplateObject>() > 0;
			}
		}

		// Token: 0x060009A1 RID: 2465 RVA: 0x0003692D File Offset: 0x00034B2D
		public static void RefreshTemplateData()
		{
			TemplateClass.jsonPath = null;
			TemplateClass.templateJsonObjects.Clear();
			TemplateClass.TemplateObjectsVideo.Clear();
			TemplateClass.TemplateObjectsPhoto.Clear();
			TemplateClass.selectedCanvasID = (TemplateClass.selectedCanvasVideoID = 0);
			TemplateClass.ReadJsonTemplateObjects(true);
			TemplateClass.ReadJsonTemplateObjects(false);
		}

		// Token: 0x060009A2 RID: 2466 RVA: 0x0003696C File Offset: 0x00034B6C
		public static void CreateDefaultTemplateJson()
		{
			TemplateClass.jsonPath = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "template.json");
			List<TemplateJsonObject> templates = new List<TemplateJsonObject>();
			for (int i = 0; i < 6; i++)
			{
				templates.Add(new TemplateJsonObject
				{
					templatePhotoObjects = new List<TemplateObject>(),
					templateVideoObjects = new List<TemplateObject>(),
					PageHeightPhoto = 1800,
					PageWidthPhoto = 1200,
					PageHeightVideo = 1920,
					PageWidthVideo = 1080,
					PageInfo = "4'' x 6'' paper",
					DPI = 300
				});
			}
			string jsonFile = JsonConvert.SerializeObject(templates);
			File.WriteAllText(TemplateClass.jsonPath, jsonFile);
		}

		// Token: 0x060009A3 RID: 2467 RVA: 0x00036A1C File Offset: 0x00034C1C
		public static TemplateObject GetObject(int RID, bool isPhoto)
		{
			if (!isPhoto)
			{
				return (from x in TemplateClass.TemplateObjectsVideo
				where x.ID == RID
				select x).FirstOrDefault<TemplateObject>();
			}
			return (from x in TemplateClass.TemplateObjectsPhoto
			where x.ID == RID
			select x).FirstOrDefault<TemplateObject>();
		}

		// Token: 0x060009A4 RID: 2468 RVA: 0x00036A70 File Offset: 0x00034C70
		public static Image ResizeImage(Image imgToResize, Size destinationSize)
		{
			int originalWidth = imgToResize.Width;
			int originalHeight = imgToResize.Height;
			float hRatio = (float)originalHeight / (float)destinationSize.Height;
			float wRatio = (float)originalWidth / (float)destinationSize.Width;
			float ratio = Math.Min(hRatio, wRatio);
			int hScale = Convert.ToInt32((float)destinationSize.Height * ratio);
			int wScale = Convert.ToInt32((float)destinationSize.Width * ratio);
			int startX = (originalWidth - wScale) / 2;
			int startY = (originalHeight - hScale) / 2;
			Rectangle sourceRectangle = new Rectangle(startX, startY, wScale, hScale);
			Bitmap bitmap = new Bitmap(destinationSize.Width, destinationSize.Height);
			Rectangle destinationRectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
			using (Graphics g = Graphics.FromImage(bitmap))
			{
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;
				g.DrawImage(imgToResize, destinationRectangle, sourceRectangle, GraphicsUnit.Pixel);
			}
			return bitmap;
		}

		// Token: 0x060009A5 RID: 2469 RVA: 0x00036B5C File Offset: 0x00034D5C
		private static Image CropImage(Image img, Rectangle cropArea)
		{
			Image result;
			using (Bitmap bmpImage = new Bitmap(img))
			{
				result = bmpImage.Clone(cropArea, bmpImage.PixelFormat);
			}
			return result;
		}

		// Token: 0x060009A6 RID: 2470 RVA: 0x00036B9C File Offset: 0x00034D9C
		public static Image GetRawPhotoCrop(Image CapturedImage)
		{
			TemplateObject photoInfo = (from x in TemplateClass.TemplateObjectsPhoto
			where x.isRawPhoto
			select x).FirstOrDefault<TemplateObject>();
			double scale = Math.Max((double)photoInfo.objectSize.Width / (double)CapturedImage.Width, (double)photoInfo.objectSize.Height / (double)CapturedImage.Height);
			Size scaledSize = new Size((int)((double)CapturedImage.Width * scale), (int)((double)CapturedImage.Height * scale));
			int AddX = (scaledSize.Width - photoInfo.objectSize.Width) / 2;
			int AddY = (scaledSize.Height - photoInfo.objectSize.Height) / 2;
			CapturedImage = TemplateClass.ResizeImage(CapturedImage, photoInfo.objectSize);
			return CapturedImage;
		}

		// Token: 0x060009A7 RID: 2471 RVA: 0x00036C60 File Offset: 0x00034E60
		public static Image GetPhotoCrop(Image CapturedImage, int photoID, bool isPhoto, int indexOfIDPhotos)
		{
			TemplateObject photoInfo = isPhoto ? (from x in TemplateClass.TemplateObjectsPhoto
			where x.isCapture && x.photoID == photoID
			select x).ToArray<TemplateObject>()[indexOfIDPhotos] : (from x in TemplateClass.TemplateObjectsVideo
			where x.isCapture && x.photoID == photoID
			select x).ToArray<TemplateObject>()[indexOfIDPhotos];
			double scale = Math.Max((double)photoInfo.objectSize.Width / (double)CapturedImage.Width, (double)photoInfo.objectSize.Height / (double)CapturedImage.Height);
			Size scaledSize = new Size((int)((double)CapturedImage.Width * scale), (int)((double)CapturedImage.Height * scale));
			int AddX = (scaledSize.Width - photoInfo.objectSize.Width) / 2;
			int AddY = (scaledSize.Height - photoInfo.objectSize.Height) / 2;
			CapturedImage = TemplateClass.ResizeImage(CapturedImage, photoInfo.objectSize);
			return CapturedImage;
		}

		// Token: 0x060009A8 RID: 2472 RVA: 0x00036D40 File Offset: 0x00034F40
		public static Rectangle GetPhotoRectangle(int PhotoID, bool isPhoto, int indexOfIDPhotos)
		{
			TemplateObject photoInfo = isPhoto ? (from x in TemplateClass.TemplateObjectsPhoto
			where x.isCapture && PhotoID == x.photoID
			select x).ToArray<TemplateObject>()[indexOfIDPhotos] : (from x in TemplateClass.TemplateObjectsVideo
			where x.isCapture && PhotoID == x.photoID
			select x).ToArray<TemplateObject>()[indexOfIDPhotos];
			return new Rectangle(photoInfo.objectPoint, photoInfo.objectSize);
		}

		// Token: 0x060009A9 RID: 2473 RVA: 0x00036DAC File Offset: 0x00034FAC
		public static Rectangle GetRawPhotoRentangle()
		{
			TemplateObject photoInfo = (from x in TemplateClass.TemplateObjectsPhoto
			where x.isRawPhoto
			select x).FirstOrDefault<TemplateObject>();
			return new Rectangle(photoInfo.objectPoint, photoInfo.objectSize);
		}

		// Token: 0x060009AA RID: 2474 RVA: 0x00036DFC File Offset: 0x00034FFC
		public static bool IsHaveImageTemplate(bool isPhoto)
		{
			if (!isPhoto)
			{
				return (from x in TemplateClass.TemplateObjectsVideo
				where x.isCapture
				select x).FirstOrDefault<TemplateObject>() != null;
			}
			return (from x in TemplateClass.TemplateObjectsPhoto
			where x.isCapture
			select x).FirstOrDefault<TemplateObject>() != null;
		}

		// Token: 0x060009AB RID: 2475 RVA: 0x00036E70 File Offset: 0x00035070
		public static string GetPhotoPNGName()
		{
			string rotation = Settings.GetValueString("rotation") ?? "0";
			string photoIconName;
			if (rotation.Contains("90") || rotation.Contains("270"))
			{
				photoIconName = "PhotoBGProfilePortrait.png";
			}
			else
			{
				photoIconName = "PhotoBGProfile.png";
			}
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconName);
		}

		// Token: 0x060009AC RID: 2476 RVA: 0x00036ED4 File Offset: 0x000350D4
		public static string GetPhotoIconName()
		{
			bool? greenBoxValue = Settings.GetValueBoolean("greenbox");
			string rotation = Settings.GetValueString("rotation") ?? "0";
			string photoIconName;
			if (greenBoxValue != null && greenBoxValue.Value)
			{
				if (rotation.Contains("90") || rotation.Contains("270"))
				{
					photoIconName = "PhotoBGProfilePortrait.png";
				}
				else
				{
					photoIconName = "PhotoBGProfile.png";
				}
			}
			else if (rotation.Contains("90") || rotation.Contains("270"))
			{
				photoIconName = "PhotoBGProfilePortrait2.png";
			}
			else
			{
				photoIconName = "PhotoBGProfile2.png";
			}
			return photoIconName;
		}

		// Token: 0x060009AD RID: 2477 RVA: 0x00036F6C File Offset: 0x0003516C
		public static string GetRawPhotoIconName()
		{
			bool? greenBoxValue = Settings.GetValueBoolean("greenbox");
			string rotation = Settings.GetValueString("rotation") ?? "0";
			string photoIconName;
			if (greenBoxValue != null && greenBoxValue.Value)
			{
				if (rotation.Contains("90") || rotation.Contains("270"))
				{
					photoIconName = "PhotoRawBGProfilePortrait.png";
				}
				else
				{
					photoIconName = "PhotoRawBGProfile.png";
				}
			}
			else if (rotation.Contains("90") || rotation.Contains("270"))
			{
				photoIconName = "PhotoRawBGProfilePortrait2.png";
			}
			else
			{
				photoIconName = "PhotoRawBGProfile2.png";
			}
			return photoIconName;
		}

		// Token: 0x060009AE RID: 2478 RVA: 0x00037004 File Offset: 0x00035204
		public static bool ControlRawImage()
		{
			if (Mode.MonofunctionalPhotoBooth == MainSettingsPage.SelectedMode)
			{
				return TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects.Exists((TemplateObject x) => x.isRawPhoto);
			}
			return false;
		}

		// Token: 0x060009AF RID: 2479 RVA: 0x00037054 File Offset: 0x00035254
		public static bool IsRawImageBack()
		{
			TemplateObject renderPhoto = (from x in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects
			where x.isCapture
			orderby x.layer descending
			select x).FirstOrDefault<TemplateObject>();
			TemplateObject rawPhoto = (from x in TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects
			where x.isRawPhoto
			select x).FirstOrDefault<TemplateObject>();
			return renderPhoto.layer >= rawPhoto.layer;
		}

		// Token: 0x060009B0 RID: 2480 RVA: 0x00037114 File Offset: 0x00035314
		public static string GetTextIconName()
		{
			return "textProfile.png";
		}

		// Token: 0x060009B1 RID: 2481 RVA: 0x00037128 File Offset: 0x00035328
		public static void CheckaTrialWatermark(ColorConverter colorConverter, bool isPhoto)
		{
			int PaperWidth = isPhoto ? TemplateClass.PaperWidthPhoto : TemplateClass.PaperWidthVideo;
			int PaperHeight = isPhoto ? TemplateClass.PaperHeightPhoto : TemplateClass.PaperHeightVideo;
			TemplateClass.TemplateSize = new Size(PaperWidth, PaperHeight);
			string watermarkValue = isPhoto ? TemplateClass.watermarkPhotoKey : TemplateClass.watermarkVideoKey;
			string watermarkPath = Settings.GetValueString(watermarkValue);
			using (Image resultWatermarkImage = new Bitmap(TemplateClass.TemplateSize.Width, TemplateClass.TemplateSize.Height))
			{
				using (Graphics graph = Graphics.FromImage(resultWatermarkImage))
				{
					graph.InterpolationMode = InterpolationMode.High;
					graph.CompositingQuality = CompositingQuality.HighQuality;
					graph.SmoothingMode = SmoothingMode.AntiAlias;
					if (File.Exists(watermarkPath))
					{
						using (Bitmap useBitmap = (Bitmap)Image.FromFile(watermarkPath))
						{
							graph.DrawImage(useBitmap, new Rectangle(0, 0, useBitmap.Width, useBitmap.Height));
						}
					}
					Font drawFont = new Font("Arial", 55f);
					string drawText = "ActivationKing Demo. Purchase to remove the watermark!";
					SizeF textSize = graph.MeasureString(drawText, drawFont);
					float x = (float)(TemplateClass.TemplateSize.Width / 2) - textSize.Width / 2f;
					float y = (float)(TemplateClass.TemplateSize.Height / 2) - textSize.Height / 2f;
					double angleRadians = Math.Atan2((double)TemplateClass.TemplateSize.Height, (double)TemplateClass.TemplateSize.Width);
					double angleDegrees = angleRadians * 57.29577951308232;
					Matrix rotationMatrix = new Matrix();
					rotationMatrix.RotateAt((float)(-(float)angleDegrees), new PointF((float)(TemplateClass.TemplateSize.Width / 2), (float)(TemplateClass.TemplateSize.Height / 2)));
					graph.Transform = rotationMatrix;
					Color brushColor = (Color)colorConverter.ConvertFromString("#aaffffff");
					graph.DrawString(drawText, drawFont, new SolidBrush(brushColor), x, y);
					graph.ResetTransform();
				}
				watermarkPath = Path.Combine(Path.GetDirectoryName(watermarkPath), Path.GetFileNameWithoutExtension(watermarkPath) + "_n" + Path.GetExtension(watermarkPath));
				Settings.SetValue(watermarkValue, watermarkPath, true);
				if (File.Exists(watermarkPath))
				{
					File.Delete(watermarkPath);
				}
				resultWatermarkImage.Save(watermarkPath);
				TemplateClass.NoWatermark[(!isPhoto) ? 1 : 0] = false;
			}
		}

		// Token: 0x060009B2 RID: 2482 RVA: 0x0003739C File Offset: 0x0003559C
		public static void SaveBackgroundAndWatermark(bool isPhoto)
		{
			TemplateClass.watermarkPhotoKey = "watermarkPhoto" + TemplateClass.selectedCanvasID.ToString();
			TemplateClass.backgroundPhotoKey = "backgroundPhoto" + TemplateClass.selectedCanvasID.ToString();
			TemplateClass.watermarkVideoKey = "watermarkVideo" + TemplateClass.selectedCanvasVideoID.ToString();
			TemplateClass.backgroundVideoKey = "backgroundVideo" + TemplateClass.selectedCanvasVideoID.ToString();
			int NoIndex = (!isPhoto) ? 1 : 0;
			int PaperWidth = isPhoto ? TemplateClass.PaperWidthPhoto : TemplateClass.PaperWidthVideo;
			int PaperHeight = isPhoto ? TemplateClass.PaperHeightPhoto : TemplateClass.PaperHeightVideo;
			List<TemplateObject> refTemplate = isPhoto ? TemplateClass.TemplateObjectsPhoto : TemplateClass.TemplateObjectsVideo;
			TemplateClass.TemplateSize = new Size(PaperWidth, PaperHeight);
			string watermarkValue = isPhoto ? TemplateClass.watermarkPhotoKey : TemplateClass.watermarkVideoKey;
			string backgroundValue = isPhoto ? TemplateClass.backgroundPhotoKey : TemplateClass.backgroundVideoKey;
			string watermarkPath = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, watermarkValue + TemplateClass.selectedCanvasID.ToString() + ".png");
			string backgroundPath = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, backgroundValue + TemplateClass.selectedCanvasID.ToString() + ".png");
			while (File.Exists(watermarkPath))
			{
				try
				{
					File.Delete(watermarkPath);
				}
				catch (Exception)
				{
					watermarkPath = Path.Combine(Path.GetDirectoryName(watermarkPath), Path.GetFileNameWithoutExtension(watermarkPath) + "_n" + Path.GetExtension(watermarkPath));
				}
			}
			List<string> files = (from x in Directory.GetFiles(EventManagementPage.GetCurrentEvent().DirectoryPath)
			where x.Contains(watermarkValue)
			select x).ToList<string>();
			using (List<string>.Enumerator enumerator = files.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					string file = enumerator.Current;
					try
					{
						File.Delete(file);
					}
					catch (Exception)
					{
					}
				}
				goto IL_1F5;
			}
			IL_1C2:
			try
			{
				File.Delete(backgroundPath);
			}
			catch (Exception)
			{
				backgroundPath = Path.Combine(Path.GetDirectoryName(backgroundPath), Path.GetFileNameWithoutExtension(backgroundPath) + "_n" + Path.GetExtension(backgroundPath));
			}
			IL_1F5:
			if (File.Exists(backgroundPath))
			{
				goto IL_1C2;
			}
			Settings.SetValue(watermarkValue, watermarkPath, true);
			Settings.SetValue(backgroundValue, backgroundPath, true);
			if (refTemplate == null)
			{
				refTemplate = new List<TemplateObject>
				{
					new TemplateObject
					{
						fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", TemplateClass.GetPhotoIconName())),
						ID = 0,
						isCapture = true,
						photoID = 0,
						layer = 0,
						objectPoint = new Point(0, 0),
						objectSize = (isPhoto ? new Size(1200, 1800) : new Size(1080, 1920))
					}
				};
				TemplateClass.NoWatermark[NoIndex] = (TemplateClass.NoBackground[NoIndex] = true);
				TemplateClass.CheckCreateOnlyTrialWatermark(watermarkPath, NoIndex);
				return;
			}
			refTemplate = (from x in refTemplate
			where File.Exists(x.fileInfo.FullName)
			select x).ToList<TemplateObject>();
			if (MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth)
			{
				refTemplate = (from x in refTemplate
				where !x.isRawPhoto
				select x).ToList<TemplateObject>();
			}
			if (!refTemplate.Exists((TemplateObject x) => x.isCapture))
			{
				List<TemplateObject> allTemplate = isPhoto ? TemplateClass.TemplateObjectsPhoto : TemplateClass.TemplateObjectsVideo;
				(from x in allTemplate
				where x.isCapture
				select x).FirstOrDefault<TemplateObject>().fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", TemplateClass.GetPhotoIconName()));
				refTemplate = (from x in allTemplate
				where File.Exists(x.fileInfo.FullName)
				select x).ToList<TemplateObject>();
			}
			int photoIndexMax = (from x in refTemplate
			where x.isCapture
			select x).Max((TemplateObject x) => x.layer);
			int photoIndexMin = (from x in refTemplate
			where x.isCapture
			select x).Min((TemplateObject x) => x.layer);
			TemplateClass.capturePhotoCount = (from x in refTemplate
			where x.isCapture
			select x).Max((TemplateObject x) => x.photoID) + 1;
			if (refTemplate.Count == 1)
			{
				TemplateClass.NoWatermark[NoIndex] = (TemplateClass.NoBackground[NoIndex] = true);
				TemplateClass.CheckCreateOnlyTrialWatermark(watermarkPath, NoIndex);
				return;
			}
			if (photoIndexMax < refTemplate.Count - 1)
			{
				using (Image resultWatermarkImage = new Bitmap(TemplateClass.TemplateSize.Width, TemplateClass.TemplateSize.Height))
				{
					using (Graphics graph = Graphics.FromImage(resultWatermarkImage))
					{
						for (int i = photoIndexMax + 1; i < refTemplate.Count; i++)
						{
							if (!refTemplate[i].isText && !refTemplate[i].isRawPhoto)
							{
								using (Bitmap useBitmap = (Bitmap)Image.FromFile(refTemplate[i].fileInfo.FullName))
								{
									graph.DrawImage(useBitmap, new Rectangle(refTemplate[i].objectPoint.X, refTemplate[i].objectPoint.Y, refTemplate[i].objectSize.Width, refTemplate[i].objectSize.Height));
								}
							}
						}
						if (SessionData.accountInfo.isTrial)
						{
							Font drawFont = new Font("Arial", 55f);
							string drawText = "ActivationKing Demo. Purchase to remove the watermark!";
							SizeF textSize = graph.MeasureString(drawText, drawFont);
							float x2 = (float)(TemplateClass.TemplateSize.Width / 2) - textSize.Width / 2f;
							float y = (float)(TemplateClass.TemplateSize.Height / 2) - textSize.Height / 2f;
							double angleRadians = Math.Atan2((double)TemplateClass.TemplateSize.Height, (double)TemplateClass.TemplateSize.Width);
							double angleDegrees = angleRadians * 57.29577951308232;
							Matrix rotationMatrix = new Matrix();
							rotationMatrix.RotateAt((float)(-(float)angleDegrees), new PointF((float)(TemplateClass.TemplateSize.Width / 2), (float)(TemplateClass.TemplateSize.Height / 2)));
							graph.Transform = rotationMatrix;
							Color brushColor = (Color)new ColorConverter().ConvertFromString("#aaffffff");
							graph.DrawString(drawText, drawFont, new SolidBrush(brushColor), x2, y);
							graph.ResetTransform();
						}
					}
					resultWatermarkImage.Save(watermarkPath);
				}
				TemplateClass.NoWatermark[NoIndex] = false;
			}
			else
			{
				TemplateClass.NoWatermark[NoIndex] = true;
				TemplateClass.CheckCreateOnlyTrialWatermark(watermarkPath, NoIndex);
			}
			if (photoIndexMin > 0)
			{
				using (Image resultBackgroundImage = new Bitmap(TemplateClass.TemplateSize.Width, TemplateClass.TemplateSize.Height))
				{
					using (Graphics graph2 = Graphics.FromImage(resultBackgroundImage))
					{
						for (int j = 0; j < photoIndexMin; j++)
						{
							if (!refTemplate[j].isText && !refTemplate[j].isRawPhoto)
							{
								using (Bitmap useBitmap2 = (Bitmap)Image.FromFile(refTemplate[j].fileInfo.FullName))
								{
									graph2.DrawImage(useBitmap2, new Rectangle(refTemplate[j].objectPoint.X, refTemplate[j].objectPoint.Y, refTemplate[j].objectSize.Width, refTemplate[j].objectSize.Height));
								}
							}
						}
					}
					resultBackgroundImage.Save(backgroundPath);
				}
				TemplateClass.NoBackground[NoIndex] = false;
				return;
			}
			TemplateClass.NoBackground[NoIndex] = true;
		}

		// Token: 0x060009B3 RID: 2483 RVA: 0x00037CF4 File Offset: 0x00035EF4
		private static void CheckCreateOnlyTrialWatermark(string path, int NoIndex)
		{
			if (SessionData.accountInfo.isTrial)
			{
				using (Image resultWatermarkImage = new Bitmap(TemplateClass.TemplateSize.Width, TemplateClass.TemplateSize.Height))
				{
					using (Graphics graph = Graphics.FromImage(resultWatermarkImage))
					{
						Font drawFont = new Font("Arial", 55f);
						string drawText = "ActivationKing Demo. Purchase to remove the watermark!";
						SizeF textSize = graph.MeasureString(drawText, drawFont);
						float x = (float)(TemplateClass.TemplateSize.Width / 2) - textSize.Width / 2f;
						float y = (float)(TemplateClass.TemplateSize.Height / 2) - textSize.Height / 2f;
						double angleRadians = Math.Atan2((double)TemplateClass.TemplateSize.Height, (double)TemplateClass.TemplateSize.Width);
						double angleDegrees = angleRadians * 57.29577951308232;
						Matrix rotationMatrix = new Matrix();
						rotationMatrix.RotateAt((float)(-(float)angleDegrees), new PointF((float)(TemplateClass.TemplateSize.Width / 2), (float)(TemplateClass.TemplateSize.Height / 2)));
						graph.Transform = rotationMatrix;
						Color brushColor = (Color)new ColorConverter().ConvertFromString("#aaffffff");
						graph.DrawString(drawText, drawFont, new SolidBrush(brushColor), x, y);
						graph.ResetTransform();
					}
					resultWatermarkImage.Save(path);
				}
				TemplateClass.NoWatermark[NoIndex] = false;
			}
		}

		// Token: 0x060009B4 RID: 2484 RVA: 0x00037E7C File Offset: 0x0003607C
		public static void UpdateTemplate()
		{
			TemplateClass.watermarkPhotoKey = "watermarkPhoto" + TemplateClass.selectedCanvasID.ToString();
			TemplateClass.backgroundPhotoKey = "backgroundPhoto" + TemplateClass.selectedCanvasID.ToString();
			TemplateClass.watermarkVideoKey = "watermarkVideo" + TemplateClass.selectedCanvasVideoID.ToString();
			TemplateClass.backgroundVideoKey = "backgroundVideo" + TemplateClass.selectedCanvasVideoID.ToString();
			TemplateClass.PaperWidthPhoto = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageWidthPhoto;
			TemplateClass.PaperHeightPhoto = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageHeightPhoto;
			TemplateClass.PaperWidthVideo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageWidthVideo;
			TemplateClass.PaperHeightVideo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageHeightVideo;
			TemplateClass.TemplateObjectsPhoto = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects;
			TemplateClass.TemplateObjectsVideo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects;
			if (DSLR.isPhoto)
			{
				TemplateClass.PageInfo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].PageInfo;
				TemplateClass.capturePhotoCount = (from x in TemplateClass.TemplateObjectsPhoto
				where x.isCapture
				select x).Max((TemplateObject x) => x.photoID) + 1;
				return;
			}
			TemplateClass.PageInfo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].PageInfo;
			TemplateClass.capturePhotoCount = (from x in TemplateClass.TemplateObjectsVideo
			where x.isCapture
			select x).Max((TemplateObject x) => x.photoID) + 1;
		}

		// Token: 0x060009B5 RID: 2485 RVA: 0x00038068 File Offset: 0x00036268
		public static string GetPaperSize(int width, int height)
		{
			return PaperSizeData.GetPaperSize(width, height).Value.Item2.Name;
		}

		// Token: 0x060009B6 RID: 2486 RVA: 0x00038090 File Offset: 0x00036290
		public static void ReadJsonTemplateObjects(bool isPhoto)
		{
			int ID = isPhoto ? TemplateClass.selectedCanvasID : TemplateClass.selectedCanvasVideoID;
			if (TemplateClass.jsonPath == null)
			{
				TemplateClass.jsonPath = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "template.json");
			}
			try
			{
				if (File.Exists(TemplateClass.jsonPath))
				{
					string fileContent = File.ReadAllText(TemplateClass.jsonPath);
					TemplateJsonObject templateJsonObject = JsonConvert.DeserializeObject<TemplateJsonObject>(fileContent);
					TemplateClass.templateJsonObjects = new List<TemplateJsonObject>();
					TemplateClass.templateJsonObjects.Add(templateJsonObject);
					for (int i = 1; i < 6; i++)
					{
						TemplateClass.templateJsonObjects.Add(new TemplateJsonObject
						{
							templatePhotoObjects = new List<TemplateObject>(),
							templateVideoObjects = new List<TemplateObject>(),
							PageHeightPhoto = 1800,
							PageWidthPhoto = 1200,
							PageHeightVideo = 1920,
							PageWidthVideo = 1080,
							PageInfo = "4'' x 6'' paper",
							DPI = 300
						});
					}
					TemplateClass.WriteJsonTemplateObjects(0);
				}
			}
			catch (Exception)
			{
			}
			if (File.Exists(TemplateClass.jsonPath))
			{
				string fileContent2 = File.ReadAllText(TemplateClass.jsonPath);
				List<TemplateJsonObject> templateJsonObject2 = JsonConvert.DeserializeObject<List<TemplateJsonObject>>(fileContent2);
				for (int j = 0; j < templateJsonObject2.Count; j++)
				{
					if (templateJsonObject2[j].templatePhotoObjects.Count == 0)
					{
						templateJsonObject2[j].templatePhotoObjects = TemplateClass.ControlTemplateObject(templateJsonObject2[j].templatePhotoObjects, true);
					}
					if (templateJsonObject2[j].templateVideoObjects.Count == 0)
					{
						templateJsonObject2[j].templateVideoObjects = TemplateClass.ControlTemplateObject(templateJsonObject2[j].templateVideoObjects, false);
					}
				}
				if (templateJsonObject2 == null)
				{
					Debug.Log("TemplateClass", string.Format("File is corrupted\n\nFile name : {0}\nFile content : {1}", TemplateClass.jsonPath, fileContent2), "ReadJsonTemplateObjects", 776);
					templateJsonObject2 = new List<TemplateJsonObject>();
					templateJsonObject2[ID].templatePhotoObjects = new List<TemplateObject>();
					templateJsonObject2[ID].templateVideoObjects = new List<TemplateObject>();
				}
				TemplateClass.PaperWidthPhoto = templateJsonObject2[ID].PageWidthPhoto;
				TemplateClass.PaperHeightPhoto = templateJsonObject2[ID].PageHeightPhoto;
				TemplateClass.PaperWidthVideo = templateJsonObject2[ID].PageWidthVideo;
				TemplateClass.PaperHeightVideo = templateJsonObject2[ID].PageHeightVideo;
				TemplateClass.TemplateObjectsPhoto = templateJsonObject2[ID].templatePhotoObjects;
				TemplateClass.TemplateObjectsVideo = templateJsonObject2[ID].templateVideoObjects;
				TemplateClass.PageInfo = templateJsonObject2[ID].PageInfo;
				TemplateClass.templateJsonObjects = templateJsonObject2;
			}
			else
			{
				string photoIconPath = TemplateClass.GetPhotoIconName();
				for (int k = 0; k < 6; k++)
				{
					TemplateClass.templateJsonObjects.Add(new TemplateJsonObject
					{
						PageInfo = "4'' x 6'' paper",
						templatePhotoObjects = new List<TemplateObject>
						{
							new TemplateObject
							{
								fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconPath)),
								ID = 0,
								isCapture = true,
								photoID = 0,
								layer = 0,
								objectPoint = new Point(0, 0),
								objectSize = new Size(1200, 1800)
							}
						},
						templateVideoObjects = new List<TemplateObject>
						{
							new TemplateObject
							{
								fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconPath)),
								ID = 0,
								isCapture = true,
								photoID = 0,
								layer = 0,
								objectPoint = new Point(0, 0),
								objectSize = new Size(1080, 1920)
							}
						}
					});
				}
				TemplateClass.TemplateObjectsPhoto = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasID].templatePhotoObjects;
				TemplateClass.TemplateObjectsVideo = TemplateClass.templateJsonObjects[TemplateClass.selectedCanvasVideoID].templateVideoObjects;
			}
			TemplateClass.WriteJsonTemplateObjects(-1);
		}

		// Token: 0x060009B7 RID: 2487 RVA: 0x00038474 File Offset: 0x00036674
		public static string GetTemplateGalleryPhotoPath(int ID)
		{
			return string.Format("{0}/Template{1}.jpg", EventManagementPage.GetCurrentEvent().DirectoryPath, ID);
		}

		// Token: 0x060009B8 RID: 2488 RVA: 0x00038490 File Offset: 0x00036690
		public static string GetTemplateVideoGalleryPhotoPath(int ID)
		{
			return string.Format("{0}/TemplateV{1}.jpg", EventManagementPage.GetCurrentEvent().DirectoryPath, ID);
		}

		// Token: 0x060009B9 RID: 2489 RVA: 0x000384AC File Offset: 0x000366AC
		public static List<TemplateObject> ControlTemplateObject(List<TemplateObject> objects, bool isPhoto)
		{
			string photoIconPath = TemplateClass.GetPhotoIconName();
			if (isPhoto)
			{
				objects = new List<TemplateObject>
				{
					new TemplateObject
					{
						fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconPath)),
						ID = 0,
						isCapture = true,
						photoID = 0,
						layer = 0,
						objectPoint = new Point(0, 0),
						objectSize = new Size(TemplateClass.PaperWidthPhoto, TemplateClass.PaperHeightPhoto)
					}
				};
			}
			else
			{
				objects = new List<TemplateObject>
				{
					new TemplateObject
					{
						fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconPath)),
						ID = 0,
						isCapture = true,
						photoID = 0,
						layer = 0,
						objectPoint = new Point(0, 0),
						objectSize = new Size(TemplateClass.PaperWidthVideo, TemplateClass.PaperHeightVideo)
					}
				};
			}
			return objects;
		}

		// Token: 0x060009BA RID: 2490 RVA: 0x000385A8 File Offset: 0x000367A8
		public static void WriteJsonTemplateObjects(int ID = -1)
		{
			if (TemplateClass.jsonPath == null)
			{
				return;
			}
			if (TemplateClass.TemplateObjectsPhoto.Count == 0)
			{
				string photoIconPath = TemplateClass.GetPhotoIconName();
				TemplateClass.TemplateObjectsPhoto = new List<TemplateObject>
				{
					new TemplateObject
					{
						fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconPath)),
						ID = 0,
						isCapture = true,
						photoID = 0,
						layer = 0,
						objectPoint = new Point(0, 0),
						objectSize = new Size(TemplateClass.PaperWidthPhoto, TemplateClass.PaperHeightPhoto)
					}
				};
			}
			if (TemplateClass.TemplateObjectsVideo.Count == 0)
			{
				string photoIconPath2 = TemplateClass.GetPhotoIconName();
				TemplateClass.TemplateObjectsVideo = new List<TemplateObject>
				{
					new TemplateObject
					{
						fileInfo = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", photoIconPath2)),
						ID = 0,
						isCapture = true,
						photoID = 0,
						layer = 0,
						objectPoint = new Point(0, 0),
						objectSize = new Size(TemplateClass.PaperWidthVideo, TemplateClass.PaperHeightVideo)
					}
				};
			}
			if (ID != -1)
			{
				TemplateClass.templateJsonObjects[ID] = new TemplateJsonObject
				{
					templatePhotoObjects = TemplateClass.TemplateObjectsPhoto,
					templateVideoObjects = TemplateClass.TemplateObjectsVideo,
					PageHeightPhoto = TemplateClass.PaperHeightPhoto,
					PageWidthPhoto = TemplateClass.PaperWidthPhoto,
					PageHeightVideo = TemplateClass.PaperHeightVideo,
					PageWidthVideo = TemplateClass.PaperWidthVideo,
					PageInfo = TemplateClass.PageInfo,
					DPI = 300
				};
			}
			string jsonFile = JsonConvert.SerializeObject(TemplateClass.templateJsonObjects);
			File.WriteAllText(TemplateClass.jsonPath, jsonFile);
			try
			{
				TemplateClass.SaveBackgroundAndWatermark(false);
				TemplateClass.SaveBackgroundAndWatermark(true);
			}
			catch (Exception ex)
			{
				MessageBoxWindow.CreateWindow("Error", ex.ToString(), new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
			}
		}

		// Token: 0x0400096E RID: 2414
		private const int totalTemplateNumber = 6;

		// Token: 0x0400096F RID: 2415
		public static string watermarkPhotoKey = "watermarkPhoto";

		// Token: 0x04000970 RID: 2416
		public static string backgroundPhotoKey = "backgroundPhoto";

		// Token: 0x04000971 RID: 2417
		public static string watermarkVideoKey = "watermarkVideo";

		// Token: 0x04000972 RID: 2418
		public static string backgroundVideoKey = "backgroundVideo";

		// Token: 0x04000973 RID: 2419
		public static string PageInfo = "4'' x 6'' paper";

		// Token: 0x04000974 RID: 2420
		public static string jsonPath;

		// Token: 0x04000975 RID: 2421
		public static string watermarkPhotoPath;

		// Token: 0x04000976 RID: 2422
		public static string backgroundPhotoPath;

		// Token: 0x04000977 RID: 2423
		public static string watermarkVideoPath;

		// Token: 0x04000978 RID: 2424
		public static string backgroundVideoPath;

		// Token: 0x04000979 RID: 2425
		public static Size TemplateSize;

		// Token: 0x0400097A RID: 2426
		public static int PaperWidthPhoto = 1200;

		// Token: 0x0400097B RID: 2427
		public static int PaperWidthVideo = 1080;

		// Token: 0x0400097C RID: 2428
		public static int PaperHeightPhoto = 1700;

		// Token: 0x0400097D RID: 2429
		public static int PaperHeightVideo = 1920;

		// Token: 0x0400097E RID: 2430
		public static bool[] NoWatermark = new bool[]
		{
			true,
			true
		};

		// Token: 0x0400097F RID: 2431
		public static bool[] NoBackground = new bool[]
		{
			true,
			true
		};

		// Token: 0x04000980 RID: 2432
		public static bool isTemplateOpen = false;

		// Token: 0x04000981 RID: 2433
		public static int selectedCanvasID = 0;

		// Token: 0x04000982 RID: 2434
		public static int selectedCanvasVideoID = 0;

		// Token: 0x04000983 RID: 2435
		public static List<TemplateJsonObject> templateJsonObjects = new List<TemplateJsonObject>();

		// Token: 0x04000984 RID: 2436
		public static List<TemplateObject> TemplateObjectsPhoto = new List<TemplateObject>();

		// Token: 0x04000985 RID: 2437
		public static List<TemplateObject> TemplateObjectsVideo = new List<TemplateObject>();

		// Token: 0x04000986 RID: 2438
		public static int capturePhotoCount = 1;

		// Token: 0x02000273 RID: 627
		public class WriteFontInfo
		{
			// Token: 0x1700025C RID: 604
			// (get) Token: 0x06001084 RID: 4228 RVA: 0x00060A6E File Offset: 0x0005EC6E
			// (set) Token: 0x06001085 RID: 4229 RVA: 0x00060A76 File Offset: 0x0005EC76
			public EmbeddedFont selectFont { get; set; }

			// Token: 0x1700025D RID: 605
			// (get) Token: 0x06001086 RID: 4230 RVA: 0x00060A7F File Offset: 0x0005EC7F
			// (set) Token: 0x06001087 RID: 4231 RVA: 0x00060A87 File Offset: 0x0005EC87
			public Point point { get; set; }
		}

		// Token: 0x02000274 RID: 628
		public enum PaperSizeEnum
		{
			// Token: 0x04001078 RID: 4216
			[Description("6\" x 4\"")]
			P_6x4,
			// Token: 0x04001079 RID: 4217
			[Description("4\" x 6\"")]
			P_4x6,
			// Token: 0x0400107A RID: 4218
			[Description("8\" x 6\"")]
			P_8x6,
			// Token: 0x0400107B RID: 4219
			[Description("6\" x 8\"")]
			P_6x8,
			// Token: 0x0400107C RID: 4220
			[Description("5\" x 7\"")]
			P_5x7,
			// Token: 0x0400107D RID: 4221
			[Description("7\" x 5\"")]
			P_7x5,
			// Token: 0x0400107E RID: 4222
			[Description("1:1")]
			P_1x1,
			// Token: 0x0400107F RID: 4223
			[Description("9:16 Full HD")]
			P_9x16_Full_HD,
			// Token: 0x04001080 RID: 4224
			[Description("16:9 Full HD")]
			P_16x9_Full_HD,
			// Token: 0x04001081 RID: 4225
			[Description("9:16 HD")]
			P_9x16_HD,
			// Token: 0x04001082 RID: 4226
			[Description("16:9 HD")]
			P_16x9_HD,
			// Token: 0x04001083 RID: 4227
			[Description("6\" x 4\" Ultra Res")]
			P_6x4_Ultra_Res,
			// Token: 0x04001084 RID: 4228
			[Description("4\" x 6\" Ultra Res")]
			P_4x6_Ultra_Res,
			// Token: 0x04001085 RID: 4229
			[Description("8\" x 6\" Ultra Res")]
			P_8x6_Ultra_Res,
			// Token: 0x04001086 RID: 4230
			[Description("6\" x 8\" Ultra Res")]
			P_6x8_Ultra_Res,
			// Token: 0x04001087 RID: 4231
			[Description("5\" x 7\" Ultra Res")]
			P_5x7_Ultra_Res,
			// Token: 0x04001088 RID: 4232
			[Description("7\" x 5\" Ultra Res")]
			P_7x5_Ultra_Res,
			// Token: 0x04001089 RID: 4233
			[Description("1:1 Ultra Res")]
			P_1x1_Ultra_Res,
			// Token: 0x0400108A RID: 4234
			[Description("9:16 Ultra HD")]
			P_9x16_Ultra_HD,
			// Token: 0x0400108B RID: 4235
			[Description("16:9 Ultra HD")]
			P_16x9_Ultra_HD
		}

		// Token: 0x02000275 RID: 629
		public enum SaveTemplate
		{
			// Token: 0x0400108D RID: 4237
			Photo,
			// Token: 0x0400108E RID: 4238
			Video,
			// Token: 0x0400108F RID: 4239
			All
		}
	}
}
