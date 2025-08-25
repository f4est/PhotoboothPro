using System;
using MongoDB.Bson.Serialization.Attributes;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x02000067 RID: 103
	[BsonIgnoreExtraElements]
	public class AiEffectLocal : Base
	{
		// Token: 0x170000CC RID: 204
		// (get) Token: 0x060007CF RID: 1999 RVA: 0x000330EE File Offset: 0x000312EE
		// (set) Token: 0x060007D0 RID: 2000 RVA: 0x000330F6 File Offset: 0x000312F6
		public string CoverImageUrl { get; set; }

		// Token: 0x170000CD RID: 205
		// (get) Token: 0x060007D1 RID: 2001 RVA: 0x000330FF File Offset: 0x000312FF
		// (set) Token: 0x060007D2 RID: 2002 RVA: 0x00033107 File Offset: 0x00031307
		public string CoverImageLocalPath { get; set; }

		// Token: 0x170000CE RID: 206
		// (get) Token: 0x060007D3 RID: 2003 RVA: 0x00033110 File Offset: 0x00031310
		// (set) Token: 0x060007D4 RID: 2004 RVA: 0x00033118 File Offset: 0x00031318
		public string Title { get; set; }

		// Token: 0x170000CF RID: 207
		// (get) Token: 0x060007D5 RID: 2005 RVA: 0x00033121 File Offset: 0x00031321
		// (set) Token: 0x060007D6 RID: 2006 RVA: 0x00033129 File Offset: 0x00031329
		public string Prompt { get; set; }

		// Token: 0x170000D0 RID: 208
		// (get) Token: 0x060007D7 RID: 2007 RVA: 0x00033132 File Offset: 0x00031332
		// (set) Token: 0x060007D8 RID: 2008 RVA: 0x0003313A File Offset: 0x0003133A
		public bool IsNew { get; set; }

		// Token: 0x170000D1 RID: 209
		// (get) Token: 0x060007D9 RID: 2009 RVA: 0x00033143 File Offset: 0x00031343
		// (set) Token: 0x060007DA RID: 2010 RVA: 0x0003314B File Offset: 0x0003134B
		public bool IsFaceSwapActive { get; set; }

		// Token: 0x170000D2 RID: 210
		// (get) Token: 0x060007DB RID: 2011 RVA: 0x00033154 File Offset: 0x00031354
		// (set) Token: 0x060007DC RID: 2012 RVA: 0x0003315C File Offset: 0x0003135C
		public bool IsPublished { get; set; }

		// Token: 0x170000D3 RID: 211
		// (get) Token: 0x060007DD RID: 2013 RVA: 0x00033165 File Offset: 0x00031365
		// (set) Token: 0x060007DE RID: 2014 RVA: 0x0003316D File Offset: 0x0003136D
		public bool IsEnabled { get; set; } = true;

		// Token: 0x170000D4 RID: 212
		// (get) Token: 0x060007DF RID: 2015 RVA: 0x00033176 File Offset: 0x00031376
		// (set) Token: 0x060007E0 RID: 2016 RVA: 0x0003317E File Offset: 0x0003137E
		public bool isSaveLocal { get; set; }

		// Token: 0x170000D5 RID: 213
		// (get) Token: 0x060007E1 RID: 2017 RVA: 0x00033187 File Offset: 0x00031387
		// (set) Token: 0x060007E2 RID: 2018 RVA: 0x0003318F File Offset: 0x0003138F
		public FileInformation FileDetails { get; set; }
	}
}
