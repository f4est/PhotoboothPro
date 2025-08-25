using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x0200002C RID: 44
	[XmlRoot("SamplePromptData")]
	public class SamplePromptDataXML
	{
		// Token: 0x17000069 RID: 105
		// (get) Token: 0x0600027E RID: 638 RVA: 0x0000CFE9 File Offset: 0x0000B1E9
		// (set) Token: 0x0600027F RID: 639 RVA: 0x0000CFF1 File Offset: 0x0000B1F1
		[XmlElement("ImageFolder")]
		public string ImageFolder { get; set; }

		// Token: 0x1700006A RID: 106
		// (get) Token: 0x06000280 RID: 640 RVA: 0x0000CFFA File Offset: 0x0000B1FA
		// (set) Token: 0x06000281 RID: 641 RVA: 0x0000D002 File Offset: 0x0000B202
		[XmlArray("SamplePrompts")]
		[XmlArrayItem("SamplePrompt")]
		public List<SamplePromptXML> SamplePrompts { get; set; }
	}
}
