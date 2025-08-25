using System;
using OpenCvSharp;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D7 RID: 215
	public class NewFrameEventArgs : EventArgs
	{
		// Token: 0x17000191 RID: 401
		// (get) Token: 0x06000B48 RID: 2888 RVA: 0x00042CC4 File Offset: 0x00040EC4
		public Mat Frame { get; }

		// Token: 0x06000B49 RID: 2889 RVA: 0x00042CCC File Offset: 0x00040ECC
		public NewFrameEventArgs(Mat frame)
		{
			this.Frame = frame;
		}
	}
}
