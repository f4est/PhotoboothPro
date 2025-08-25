using System;
using System.Xml.Serialization;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200002B RID: 43
	[XmlRoot(ElementName = "FileDetails")]
	public class FileDetails
	{
		// Token: 0x17000065 RID: 101
		// (get) Token: 0x06000275 RID: 629 RVA: 0x0000CF9D File Offset: 0x0000B19D
		// (set) Token: 0x06000276 RID: 630 RVA: 0x0000CFA5 File Offset: 0x0000B1A5
		[XmlElement("Filename")]
		public string Filename { get; set; }

		// Token: 0x17000066 RID: 102
		// (get) Token: 0x06000277 RID: 631 RVA: 0x0000CFAE File Offset: 0x0000B1AE
		// (set) Token: 0x06000278 RID: 632 RVA: 0x0000CFB6 File Offset: 0x0000B1B6
		[XmlElement("Width")]
		public int Width { get; set; }

		// Token: 0x17000067 RID: 103
		// (get) Token: 0x06000279 RID: 633 RVA: 0x0000CFBF File Offset: 0x0000B1BF
		// (set) Token: 0x0600027A RID: 634 RVA: 0x0000CFC7 File Offset: 0x0000B1C7
		[XmlElement("Height")]
		public int Height { get; set; }

		// Token: 0x17000068 RID: 104
		// (get) Token: 0x0600027B RID: 635 RVA: 0x0000CFD0 File Offset: 0x0000B1D0
		// (set) Token: 0x0600027C RID: 636 RVA: 0x0000CFD8 File Offset: 0x0000B1D8
		[XmlElement("Filesize")]
		public int Filesize { get; set; }
	}
}
