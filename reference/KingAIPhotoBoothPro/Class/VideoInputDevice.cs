using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using KingAIPhotoBoothPro.Modals.Webcam;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A3 RID: 163
	public class VideoInputDevice
	{
		// Token: 0x17000165 RID: 357
		// (get) Token: 0x0600098B RID: 2443 RVA: 0x00036362 File Offset: 0x00034562
		public string Name { get; }

		// Token: 0x0600098C RID: 2444 RVA: 0x0003636A File Offset: 0x0003456A
		public VideoInputDevice(IMoniker moniker)
		{
			this.Name = this.GetFriendlyName(moniker);
		}

		// Token: 0x0600098D RID: 2445 RVA: 0x00036380 File Offset: 0x00034580
		private string GetFriendlyName(IMoniker moniker)
		{
			object bagObject = null;
			string result;
			try
			{
				Guid bagId = typeof(IPropertyBag).GUID;
				moniker.BindToStorage(null, null, ref bagId, out bagObject);
				IPropertyBag propertyBag = (IPropertyBag)bagObject;
				object value = null;
				int hresult = propertyBag.Read("FriendlyName", ref value, IntPtr.Zero);
				if (hresult != 0)
				{
					Marshal.ThrowExceptionForHR(hresult);
				}
				result = ((value as string) ?? string.Empty);
			}
			catch (Exception)
			{
				result = string.Empty;
			}
			finally
			{
				if (bagObject != null)
				{
					Marshal.ReleaseComObject(bagObject);
					bagObject = null;
				}
			}
			return result;
		}
	}
}
