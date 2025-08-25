using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media;
using KingAIPhotoBoothPro.Class.Helper;
using OpenCvSharp;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000BA RID: 186
	internal class ReplaceGreenScreenFilter : IDisposable
	{
		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06000A4C RID: 2636 RVA: 0x0003B91B File Offset: 0x00039B1B
		// (set) Token: 0x06000A4D RID: 2637 RVA: 0x0003B923 File Offset: 0x00039B23
		public byte GreenScale
		{
			get
			{
				return this._greenScale;
			}
			set
			{
				if (value < 0 || value > 255)
				{
					throw new ArgumentOutOfRangeException("GreenScale should >=0 and <=255");
				}
				this._greenScale = value;
			}
		}

		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06000A4E RID: 2638 RVA: 0x0003B943 File Offset: 0x00039B43
		// (set) Token: 0x06000A4F RID: 2639 RVA: 0x0003B94B File Offset: 0x00039B4B
		public double MinBlockPercent
		{
			get
			{
				return this._minBlockPercent;
			}
			set
			{
				if (value <= 0.0 || value >= 1.0)
				{
					throw new ArgumentOutOfRangeException("GreenScale should >0 and <1");
				}
				this._minBlockPercent = value;
			}
		}

		// Token: 0x06000A50 RID: 2640 RVA: 0x0003B977 File Offset: 0x00039B77
		public void SetBackgroundImage(Mat backgroundImage)
		{
			if (backgroundImage != null)
			{
				this._backgroundImage = backgroundImage.Clone();
			}
		}

		// Token: 0x06000A51 RID: 2641 RVA: 0x0003B988 File Offset: 0x00039B88
		public ReplaceGreenScreenFilter(Mat backgroundImage)
		{
			this.SetBackgroundImage(backgroundImage);
		}

		// Token: 0x06000A52 RID: 2642 RVA: 0x0003B9BC File Offset: 0x00039BBC
		private unsafe void RenderGreenScreenMask(Mat src, Mat matMask, ReplaceGreenScreenFilter.FilterColor filterColor)
		{
			if (src.Channels() < 3)
			{
				throw new ArgumentException("src.Channels() should >=3");
			}
			if (matMask.Channels() != 1)
			{
				throw new ArgumentException("matMask.Channels() should == 1");
			}
			if (src.Size() != matMask.Size())
			{
				throw new ArgumentException("src.Size()!=matMask.Size()");
			}
			int rows = src.Rows;
			int cols = src.Cols;
			Color themeColor = (Color)ColorConverter.ConvertFromString(Settings.GetValueString("colorgreenscreen"));
			for (int x = 0; x < rows; x++)
			{
				Vec3b* srcRow = (Vec3b*)((void*)src.Ptr(x));
				byte* maskRow = (byte*)((void*)matMask.Ptr(x));
				for (int y = 0; y < cols; y++)
				{
					Vec3b* pData = srcRow + y;
					byte blue = pData->Item0;
					byte green = pData->Item1;
					byte red = pData->Item2;
					byte max = (blue > green) ? blue : green;
					byte max2 = (max > red) ? max : red;
					byte absMax = new byte[]
					{
						red,
						green,
						blue
					}.Max<byte>() - new byte[]
					{
						red,
						green,
						blue
					}.Min<byte>();
					if (filterColor == ReplaceGreenScreenFilter.FilterColor.Green)
					{
						if (green == max2 && green > this._greenScale)
						{
							maskRow[y] = 0;
						}
						else
						{
							maskRow[y] = byte.MaxValue;
						}
					}
					else if (filterColor == ReplaceGreenScreenFilter.FilterColor.Blue)
					{
						if (blue == max2 && blue > this._greenScale)
						{
							maskRow[y] = 0;
						}
						else
						{
							maskRow[y] = byte.MaxValue;
						}
					}
					else if (filterColor == ReplaceGreenScreenFilter.FilterColor.Black)
					{
						byte maxBlack = (byte)Math.Min((int)(this._greenScale * 3), 128);
						if (max2 < this._greenScale)
						{
							maskRow[y] = 0;
						}
						else if (max2 < maxBlack && absMax < 15)
						{
							maskRow[y] = 0;
						}
						else
						{
							maskRow[y] = byte.MaxValue;
						}
					}
					else if (filterColor == ReplaceGreenScreenFilter.FilterColor.RandomColor)
					{
						if (Math.Abs((int)(themeColor.R - red)) < (int)this._greenScale && Math.Abs((int)(themeColor.G - green)) < (int)this._greenScale && Math.Abs((int)(themeColor.B - blue)) < (int)this._greenScale)
						{
							maskRow[y] = 0;
						}
						else
						{
							maskRow[y] = byte.MaxValue;
						}
					}
				}
			}
		}

		// Token: 0x06000A53 RID: 2643 RVA: 0x0003BC00 File Offset: 0x00039E00
		private void GammaCorrect(Mat src, double gamma)
		{
			Mat lookUpTable;
			this.dictGammaCorrectionLUT.TryGetValue(gamma, out lookUpTable);
			if (lookUpTable == null)
			{
				lookUpTable = new Mat(new Size(1, 256), MatType.CV_8UC1, new Scalar(0.0));
				for (int i = 0; i <= 255; i++)
				{
					double value = Math.Pow((double)i / 255.0, gamma) * 255.0;
					lookUpTable.Set<byte>(0, i, np.clip((byte)value, 0, byte.MaxValue));
				}
				this.dictGammaCorrectionLUT[gamma] = lookUpTable;
			}
			Cv2.LUT(src, lookUpTable, src);
		}

		// Token: 0x06000A54 RID: 2644 RVA: 0x0003BCAC File Offset: 0x00039EAC
		public void Apply(Mat src)
		{
			using (ResourceTracker t = new ResourceTracker())
			{
				Stopwatch sw = new Stopwatch();
				sw.Start();
				Size srcSize = src.Size();
				Mat matMask = t.NewMat(srcSize, MatType.CV_8UC1, new Scalar(0.0));
				ReplaceGreenScreenFilter.FilterColor filterColor = ReplaceGreenScreenFilter.FilterColor.RandomColor;
				Color themeColor = (Color)ColorConverter.ConvertFromString(Settings.GetValueString("colorgreenscreen"));
				if (themeColor.R <= 3 && themeColor.B <= 3 && themeColor.G >= 252)
				{
					filterColor = ReplaceGreenScreenFilter.FilterColor.Green;
				}
				if (themeColor.R <= 3 && themeColor.G <= 3 && themeColor.B >= 252)
				{
					filterColor = ReplaceGreenScreenFilter.FilterColor.Blue;
				}
				if (themeColor.R <= 3 && themeColor.B <= 3 && themeColor.G <= 3)
				{
					filterColor = ReplaceGreenScreenFilter.FilterColor.Black;
				}
				this.RenderGreenScreenMask(src, matMask, filterColor);
				int minBlockArea = (int)((double)(srcSize.Width * srcSize.Height) * this.MinBlockPercent);
				IEnumerable<Point[]> contoursExternalForeground = from c in (from c in Cv2.FindContoursAsArray(matMask, RetrievalModes.External, ContourApproximationModes.ApproxNone, null)
				select new
				{
					contour = c,
					Area = (int)Cv2.ContourArea(c, false)
				} into c
				where c.Area >= minBlockArea
				orderby c.Area descending
				select c).Take(5)
				select c.contour;
				Mat matMaskForeground = t.NewMat(srcSize, MatType.CV_8UC1, new Scalar(0.0));
				matMaskForeground.DrawContours(contoursExternalForeground, -1, new Scalar(255.0), -1, LineTypes.Link8, null, int.MaxValue, null);
				Mat matInternalHollow = t.NewMat(srcSize, MatType.CV_8UC1, new Scalar(0.0));
				Cv2.BitwiseXor(matMaskForeground, matMask, matInternalHollow, null);
				int minHollowArea = (int)((double)minBlockArea * 0.5);
				IEnumerable<Point[]> contoursInternalHollow = from c in (from c in Cv2.FindContoursAsArray(matInternalHollow, RetrievalModes.External, ContourApproximationModes.ApproxNone, null)
				select new
				{
					contour = c,
					Area = Cv2.ContourArea(c, false)
				} into c
				where c.Area >= (double)minHollowArea
				orderby c.Area descending
				select c).Take(10)
				select c.contour;
				foreach (Point[] c2 in contoursInternalHollow)
				{
					matMaskForeground.FillConvexPoly(c2, new Scalar(0.0), LineTypes.Link8, 0);
				}
				Mat element = t.T<Mat>(Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3)));
				Cv2.MorphologyEx(matMaskForeground, matMaskForeground, MorphTypes.Close, element, null, 6, BorderTypes.Constant, null);
				Cv2.GaussianBlur(matMaskForeground, matMaskForeground, new Size(3, 3), 0.0, 0.0, BorderTypes.Reflect101);
				Mat foreground = t.NewMat(src.Size(), MatType.CV_8UC4, new Scalar(0.0));
				ZackCVHelper.AddAlphaChannel(src, foreground, matMaskForeground);
				if (this._backgroundImage != null)
				{
					Cv2.Resize(this._backgroundImage, src, src.Size(), 0.0, 0.0, InterpolationFlags.Linear);
				}
				ZackCVHelper.DrawOverlay(src, foreground);
			}
		}

		// Token: 0x06000A55 RID: 2645 RVA: 0x0003C0D0 File Offset: 0x0003A2D0
		public void Dispose()
		{
			if (this._backgroundImage != null)
			{
				this._backgroundImage.Dispose();
			}
		}

		// Token: 0x04000A14 RID: 2580
		private byte _greenScale = 30;

		// Token: 0x04000A15 RID: 2581
		private double _minBlockPercent = 0.01;

		// Token: 0x04000A16 RID: 2582
		private Mat _backgroundImage;

		// Token: 0x04000A17 RID: 2583
		private Dictionary<double, Mat> dictGammaCorrectionLUT = new Dictionary<double, Mat>();

		// Token: 0x02000293 RID: 659
		public enum FilterColor
		{
			// Token: 0x0400113A RID: 4410
			Green,
			// Token: 0x0400113B RID: 4411
			Blue,
			// Token: 0x0400113C RID: 4412
			Black,
			// Token: 0x0400113D RID: 4413
			RandomColor
		}
	}
}
