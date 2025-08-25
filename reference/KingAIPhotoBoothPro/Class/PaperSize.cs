using System;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A1 RID: 161
	public class PaperSize
	{
		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000980 RID: 2432 RVA: 0x00036212 File Offset: 0x00034412
		// (set) Token: 0x06000981 RID: 2433 RVA: 0x0003621A File Offset: 0x0003441A
		public string Name { get; set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000982 RID: 2434 RVA: 0x00036223 File Offset: 0x00034423
		// (set) Token: 0x06000983 RID: 2435 RVA: 0x0003622B File Offset: 0x0003442B
		public int Width { get; set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000984 RID: 2436 RVA: 0x00036234 File Offset: 0x00034434
		// (set) Token: 0x06000985 RID: 2437 RVA: 0x0003623C File Offset: 0x0003443C
		public int Height { get; set; }

		// Token: 0x06000986 RID: 2438 RVA: 0x00036245 File Offset: 0x00034445
		public PaperSize(string name, int width, int height)
		{
			this.Name = name;
			this.Width = width;
			this.Height = height;
		}
	}
}
