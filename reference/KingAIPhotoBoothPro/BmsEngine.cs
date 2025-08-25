using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KingAIPhotoBoothPro
{
	// Token: 0x02000018 RID: 24
	internal static class BmsEngine
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x00006E8C File Offset: 0x0000508C
		public static void Init(BitmapSource imageBms)
		{
			BmsEngine.parentBms = imageBms;
			BmsEngine.stride = imageBms.PixelWidth * ((imageBms.Format.BitsPerPixel + 7) / 8);
			BmsEngine.dataLength = BmsEngine.stride * imageBms.PixelHeight;
			BmsEngine.pixelWidth = imageBms.PixelWidth;
			BmsEngine.pixelHeight = imageBms.PixelHeight;
			BmsEngine.dpiX = imageBms.DpiX;
			BmsEngine.dpiY = imageBms.DpiY;
			BmsEngine.format = imageBms.Format;
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00006F08 File Offset: 0x00005108
		public static BitmapSource CloneBms(byte[] newRgbData)
		{
			return BitmapSource.Create(BmsEngine.pixelWidth, BmsEngine.pixelHeight, BmsEngine.dpiX, BmsEngine.dpiY, BmsEngine.format, null, newRgbData, BmsEngine.stride);
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00006F3C File Offset: 0x0000513C
		public static byte[] GetRgbData()
		{
			byte[] rgb = new byte[BmsEngine.dataLength];
			BmsEngine.parentBms.CopyPixels(rgb, BmsEngine.stride, 0);
			return rgb;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00006F68 File Offset: 0x00005168
		public static BitmapSource ChangeColorImage(Color rgb, BitmapSource bitmapChange)
		{
			BmsEngine.Init(bitmapChange);
			byte[] newJpegBytes = BmsEngine.GetRgbData();
			for (int i = 0; i < BmsEngine.dataLength; i += 4)
			{
				newJpegBytes[i] = rgb.B;
				newJpegBytes[i + 1] = rgb.G;
				newJpegBytes[i + 2] = rgb.R;
				newJpegBytes[i + 3] = ((newJpegBytes[i + 3] == 0) ? 0 : rgb.A);
			}
			return BmsEngine.CloneBms(newJpegBytes);
		}

		// Token: 0x040000C2 RID: 194
		private static BitmapSource parentBms;

		// Token: 0x040000C3 RID: 195
		public static int dataLength;

		// Token: 0x040000C4 RID: 196
		private static int stride;

		// Token: 0x040000C5 RID: 197
		private static int pixelWidth;

		// Token: 0x040000C6 RID: 198
		private static int pixelHeight;

		// Token: 0x040000C7 RID: 199
		private static double dpiX;

		// Token: 0x040000C8 RID: 200
		private static double dpiY;

		// Token: 0x040000C9 RID: 201
		private static PixelFormat format;
	}
}
