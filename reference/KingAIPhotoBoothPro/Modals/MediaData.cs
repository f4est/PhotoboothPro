using System;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000073 RID: 115
	public class MediaData : Base
	{
		// Token: 0x17000100 RID: 256
		// (get) Token: 0x06000847 RID: 2119 RVA: 0x00033678 File Offset: 0x00031878
		// (set) Token: 0x06000848 RID: 2120 RVA: 0x00033680 File Offset: 0x00031880
		public string EventID { get; set; }

		// Token: 0x17000101 RID: 257
		// (get) Token: 0x06000849 RID: 2121 RVA: 0x00033689 File Offset: 0x00031889
		// (set) Token: 0x0600084A RID: 2122 RVA: 0x00033691 File Offset: 0x00031891
		public string UserHash { get; set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x0600084B RID: 2123 RVA: 0x0003369A File Offset: 0x0003189A
		// (set) Token: 0x0600084C RID: 2124 RVA: 0x000336A2 File Offset: 0x000318A2
		public string EventHash { get; set; }

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x0600084D RID: 2125 RVA: 0x000336AB File Offset: 0x000318AB
		// (set) Token: 0x0600084E RID: 2126 RVA: 0x000336B3 File Offset: 0x000318B3
		public string MediaHash { get; set; }

		// Token: 0x17000104 RID: 260
		// (get) Token: 0x0600084F RID: 2127 RVA: 0x000336BC File Offset: 0x000318BC
		// (set) Token: 0x06000850 RID: 2128 RVA: 0x000336C4 File Offset: 0x000318C4
		public string RawUrl { get; set; }

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x06000851 RID: 2129 RVA: 0x000336CD File Offset: 0x000318CD
		// (set) Token: 0x06000852 RID: 2130 RVA: 0x000336D5 File Offset: 0x000318D5
		public string ResultUrl { get; set; }

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x06000853 RID: 2131 RVA: 0x000336DE File Offset: 0x000318DE
		// (set) Token: 0x06000854 RID: 2132 RVA: 0x000336E6 File Offset: 0x000318E6
		public string ThumbnailUrl { get; set; }

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x06000855 RID: 2133 RVA: 0x000336EF File Offset: 0x000318EF
		// (set) Token: 0x06000856 RID: 2134 RVA: 0x000336F7 File Offset: 0x000318F7
		public FileInformation FileInfo { get; set; }

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x06000857 RID: 2135 RVA: 0x00033700 File Offset: 0x00031900
		// (set) Token: 0x06000858 RID: 2136 RVA: 0x00033708 File Offset: 0x00031908
		public FileInformation RawFileInfo { get; set; }

		// Token: 0x17000109 RID: 265
		// (get) Token: 0x06000859 RID: 2137 RVA: 0x00033711 File Offset: 0x00031911
		// (set) Token: 0x0600085A RID: 2138 RVA: 0x00033719 File Offset: 0x00031919
		public ParticipantInformation ParticipantInfo { get; set; }
	}
}
