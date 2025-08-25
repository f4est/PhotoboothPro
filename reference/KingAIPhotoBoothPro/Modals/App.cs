using System;
using MongoDB.Bson.Serialization.Attributes;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200007C RID: 124
	[BsonIgnoreExtraElements]
	[Serializable]
	public class App
	{
		// Token: 0x040008B8 RID: 2232
		public string Name;

		// Token: 0x040008B9 RID: 2233
		public string Category;

		// Token: 0x040008BA RID: 2234
		public string DisplayName;

		// Token: 0x040008BB RID: 2235
		public string Description;

		// Token: 0x040008BC RID: 2236
		public string DownloadLinkLatestVersion;

		// Token: 0x040008BD RID: 2237
		public string DownloadLinkLatestTesterVersion;

		// Token: 0x040008BE RID: 2238
		public string InfoLink;

		// Token: 0x040008BF RID: 2239
		public string SupportLink;

		// Token: 0x040008C0 RID: 2240
		public string CreditLink;

		// Token: 0x040008C1 RID: 2241
		public string ThemeColorHexString;

		// Token: 0x040008C2 RID: 2242
		public string LatestVersion;

		// Token: 0x040008C3 RID: 2243
		public string LatestTesterVersion;

		// Token: 0x040008C4 RID: 2244
		public string AppHash;

		// Token: 0x040008C5 RID: 2245
		public AppSettings AppDefaultSettings;
	}
}
