using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using KingAIPhotoBoothPro.Modals.Webcam;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A2 RID: 162
	public class SystemDeviceEnumerator : IDisposable
	{
		// Token: 0x06000987 RID: 2439 RVA: 0x00036264 File Offset: 0x00034464
		public SystemDeviceEnumerator()
		{
			Type comType = Type.GetTypeFromCLSID(new Guid("62BE5D10-60EB-11D0-BD3B-00A0C911CE86"));
			this._systemDeviceEnumerator = (ICreateDevEnum)Activator.CreateInstance(comType);
		}

		// Token: 0x06000988 RID: 2440 RVA: 0x00036298 File Offset: 0x00034498
		public IReadOnlyDictionary<int, string> ListVideoInputDevice()
		{
			Guid videoInputDeviceClass = new Guid("{860BB310-5D01-11D0-BD3B-00A0C911CE86}");
			IEnumMoniker enumMoniker;
			int hresult = this._systemDeviceEnumerator.CreateClassEnumerator(ref videoInputDeviceClass, out enumMoniker, 0);
			if (hresult != 0)
			{
				throw new ApplicationException("No devices of the category");
			}
			IMoniker[] moniker = new IMoniker[1];
			Dictionary<int, string> list = new Dictionary<int, string>();
			while (enumMoniker.Next(1, moniker, IntPtr.Zero) == 0 && moniker[0] != null)
			{
				VideoInputDevice device = new VideoInputDevice(moniker[0]);
				list.Add(list.Count, device.Name);
				Marshal.ReleaseComObject(moniker[0]);
				moniker[0] = null;
			}
			return list;
		}

		// Token: 0x06000989 RID: 2441 RVA: 0x00036324 File Offset: 0x00034524
		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing && this._systemDeviceEnumerator != null)
				{
					Marshal.ReleaseComObject(this._systemDeviceEnumerator);
					this._systemDeviceEnumerator = null;
				}
				this.disposed = true;
			}
		}

		// Token: 0x0600098A RID: 2442 RVA: 0x00036353 File Offset: 0x00034553
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		// Token: 0x0400096A RID: 2410
		private bool disposed;

		// Token: 0x0400096B RID: 2411
		private ICreateDevEnum _systemDeviceEnumerator;
	}
}
