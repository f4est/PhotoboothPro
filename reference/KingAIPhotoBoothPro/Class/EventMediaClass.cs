using System;
using System.Collections.Generic;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Pages;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B8 RID: 184
	[Serializable]
	public class EventMediaClass
	{
		// Token: 0x06000A42 RID: 2626 RVA: 0x0003B664 File Offset: 0x00039864
		public void SaveToFile()
		{
			if (EventManagementPage.GetCurrentEvent() != null)
			{
				ExtensionMethod.CreateWriteJson<EventMediaClass>(this, EventManagementPage.GetCurrentEvent().EventMediaJsonPath);
			}
		}

		// Token: 0x04000A12 RID: 2578
		public List<MediaClassBase> mediaList = new List<MediaClassBase>();
	}
}
