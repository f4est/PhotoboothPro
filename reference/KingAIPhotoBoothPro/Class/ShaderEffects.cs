using System;
using Kaliko.ImageLibrary;
using Kaliko.ImageLibrary.Filters;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000BB RID: 187
	public static class ShaderEffects
	{
		// Token: 0x06000A56 RID: 2646 RVA: 0x0003C0E8 File Offset: 0x0003A2E8
		public static void GreenFilter(KalikoImage image, string savePath)
		{
			ChromaKeyFilter chromaKey = new ChromaKeyFilter();
			chromaKey.Run(image);
			image.SavePng(savePath);
		}

		// Token: 0x02000296 RID: 662
		public class GreenManuelFilter : IFilter
		{
			// Token: 0x060010FD RID: 4349 RVA: 0x00063594 File Offset: 0x00061794
			public void Run(KalikoImage image)
			{
				byte[] bytes = image.ByteArray;
				int i = 0;
				int j = bytes.Length;
				while (i < j)
				{
					int b = (int)bytes[i];
					int g = (int)bytes[i + 1];
					int r = (int)bytes[i + 2];
					if (g > 200 && b < 50 && r < 50)
					{
						bytes[i] = (bytes[i + 1] = (bytes[i + 2] = (bytes[i + 3] = 0)));
					}
					i += 4;
				}
				image.ByteArray = bytes;
			}
		}
	}
}
