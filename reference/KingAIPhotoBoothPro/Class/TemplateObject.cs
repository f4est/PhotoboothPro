using System;
using System.Drawing;
using System.IO;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000AB RID: 171
	public class TemplateObject
	{
		// Token: 0x060009BD RID: 2493 RVA: 0x0003888E File Offset: 0x00036A8E
		public TemplateObject()
		{
			this.objectSize = new Size(0, 0);
			this.objectPoint = new Point(0, 0);
		}

		// Token: 0x0400098F RID: 2447
		public int ID;

		// Token: 0x04000990 RID: 2448
		public Size objectSize;

		// Token: 0x04000991 RID: 2449
		public Point objectPoint;

		// Token: 0x04000992 RID: 2450
		public FileInfo fileInfo;

		// Token: 0x04000993 RID: 2451
		public int layer;

		// Token: 0x04000994 RID: 2452
		public bool isCapture;

		// Token: 0x04000995 RID: 2453
		public int photoID;

		// Token: 0x04000996 RID: 2454
		public bool isText;

		// Token: 0x04000997 RID: 2455
		public bool isRawPhoto;
	}
}
