using System;
using System.IO;
using KingAIPhotoBoothPro.Class;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200006F RID: 111
	public class FileInformation
	{
		// Token: 0x170000EE RID: 238
		// (get) Token: 0x0600081D RID: 2077 RVA: 0x00033475 File Offset: 0x00031675
		public string FullPath
		{
			get
			{
				if (this.Directory == null)
				{
					return null;
				}
				return Path.Combine(this.Directory, this.Filename);
			}
		}

		// Token: 0x0600081E RID: 2078 RVA: 0x00033494 File Offset: 0x00031694
		public MediaType GetMediaType()
		{
			string extension = Path.GetExtension(this.Filename).ToLower();
			if (extension == ".mp4" || extension == ".mov")
			{
				return MediaType.video;
			}
			if (extension == ".png" || extension == ".jpg" || extension == ".jpeg")
			{
				return MediaType.photo;
			}
			return MediaType.unknown;
		}

		// Token: 0x0400086C RID: 2156
		public string Filename;

		// Token: 0x0400086D RID: 2157
		public string Directory;

		// Token: 0x0400086E RID: 2158
		public int Width;

		// Token: 0x0400086F RID: 2159
		public int Height;

		// Token: 0x04000870 RID: 2160
		public long Filesize;
	}
}
