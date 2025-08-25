using System;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000074 RID: 116
	public class MediaVariationData : Base
	{
		// Token: 0x1700010A RID: 266
		// (get) Token: 0x0600085C RID: 2140 RVA: 0x0003372A File Offset: 0x0003192A
		// (set) Token: 0x0600085D RID: 2141 RVA: 0x00033732 File Offset: 0x00031932
		public string MediaHash { get; set; }

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x0600085E RID: 2142 RVA: 0x0003373B File Offset: 0x0003193B
		// (set) Token: 0x0600085F RID: 2143 RVA: 0x00033743 File Offset: 0x00031943
		public string VariationMediaHash { get; set; }

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x06000860 RID: 2144 RVA: 0x0003374C File Offset: 0x0003194C
		// (set) Token: 0x06000861 RID: 2145 RVA: 0x00033754 File Offset: 0x00031954
		public string Prompt { get; set; }

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x06000862 RID: 2146 RVA: 0x0003375D File Offset: 0x0003195D
		// (set) Token: 0x06000863 RID: 2147 RVA: 0x00033765 File Offset: 0x00031965
		public FileInformation FileInfo { get; set; }

		// Token: 0x1700010E RID: 270
		// (get) Token: 0x06000864 RID: 2148 RVA: 0x0003376E File Offset: 0x0003196E
		// (set) Token: 0x06000865 RID: 2149 RVA: 0x00033776 File Offset: 0x00031976
		public FileInformation RawFileInfo { get; set; }

		// Token: 0x1700010F RID: 271
		// (get) Token: 0x06000866 RID: 2150 RVA: 0x0003377F File Offset: 0x0003197F
		// (set) Token: 0x06000867 RID: 2151 RVA: 0x00033787 File Offset: 0x00031987
		public string RawUrl { get; set; }

		// Token: 0x17000110 RID: 272
		// (get) Token: 0x06000868 RID: 2152 RVA: 0x00033790 File Offset: 0x00031990
		// (set) Token: 0x06000869 RID: 2153 RVA: 0x00033798 File Offset: 0x00031998
		public string ResultUrl { get; set; }
	}
}
