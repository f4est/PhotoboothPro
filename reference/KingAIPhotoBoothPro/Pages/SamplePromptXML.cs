using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200002A RID: 42
	[XmlRoot(ElementName = "SamplePrompt")]
	public class SamplePromptXML
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x0600026A RID: 618 RVA: 0x0000CF40 File Offset: 0x0000B140
		// (set) Token: 0x0600026B RID: 619 RVA: 0x0000CF48 File Offset: 0x0000B148
		[XmlElement("Index")]
		public int Index { get; set; }

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x0600026C RID: 620 RVA: 0x0000CF51 File Offset: 0x0000B151
		// (set) Token: 0x0600026D RID: 621 RVA: 0x0000CF59 File Offset: 0x0000B159
		[XmlElement("FileName")]
		public string FileName { get; set; }

		// Token: 0x17000062 RID: 98
		// (get) Token: 0x0600026E RID: 622 RVA: 0x0000CF62 File Offset: 0x0000B162
		// (set) Token: 0x0600026F RID: 623 RVA: 0x0000CF6A File Offset: 0x0000B16A
		[XmlElement("Title")]
		public string Title { get; set; }

		// Token: 0x17000063 RID: 99
		// (get) Token: 0x06000270 RID: 624 RVA: 0x0000CF73 File Offset: 0x0000B173
		// (set) Token: 0x06000271 RID: 625 RVA: 0x0000CF7B File Offset: 0x0000B17B
		[XmlArray("Prompts")]
		[XmlArrayItem("Prompt")]
		public List<string> Prompts { get; set; }

		// Token: 0x17000064 RID: 100
		// (get) Token: 0x06000272 RID: 626 RVA: 0x0000CF84 File Offset: 0x0000B184
		// (set) Token: 0x06000273 RID: 627 RVA: 0x0000CF8C File Offset: 0x0000B18C
		[XmlElement("FileDetails")]
		public FileDetails FileDetails { get; set; }
	}
}
