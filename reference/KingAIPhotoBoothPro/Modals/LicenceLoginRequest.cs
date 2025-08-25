using System;
using System.Reflection;
using System.Security.Principal;
using KingAIPhotoBoothPro.Class;

namespace KingAIPhotoBoothPro.Modals
{
	// Token: 0x0200007F RID: 127
	[Serializable]
	public class LicenceLoginRequest
	{
		// Token: 0x060008C3 RID: 2243 RVA: 0x00033C40 File Offset: 0x00031E40
		public LicenceLoginRequest()
		{
			this.DeviceUniqueIdentifier = DeviceInfo.GetDeviceUniqueIdentifier();
			this.DeviceName = WindowsIdentity.GetCurrent().Name;
			this.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}

		// Token: 0x17000137 RID: 311
		// (get) Token: 0x060008C4 RID: 2244 RVA: 0x00033C7D File Offset: 0x00031E7D
		// (set) Token: 0x060008C5 RID: 2245 RVA: 0x00033C85 File Offset: 0x00031E85
		public string DeviceUniqueIdentifier { get; private set; }

		// Token: 0x17000138 RID: 312
		// (get) Token: 0x060008C6 RID: 2246 RVA: 0x00033C8E File Offset: 0x00031E8E
		// (set) Token: 0x060008C7 RID: 2247 RVA: 0x00033C96 File Offset: 0x00031E96
		public string DeviceName { get; private set; }

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x060008C8 RID: 2248 RVA: 0x00033C9F File Offset: 0x00031E9F
		// (set) Token: 0x060008C9 RID: 2249 RVA: 0x00033CA7 File Offset: 0x00031EA7
		public string AppVersion { get; set; }

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x060008CA RID: 2250 RVA: 0x00033CB0 File Offset: 0x00031EB0
		// (set) Token: 0x060008CB RID: 2251 RVA: 0x00033CB8 File Offset: 0x00031EB8
		public string AccessToken { get; set; }

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x060008CC RID: 2252 RVA: 0x00033CC1 File Offset: 0x00031EC1
		// (set) Token: 0x060008CD RID: 2253 RVA: 0x00033CC9 File Offset: 0x00031EC9
		public string Email { get; set; }

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x060008CE RID: 2254 RVA: 0x00033CD2 File Offset: 0x00031ED2
		// (set) Token: 0x060008CF RID: 2255 RVA: 0x00033CDA File Offset: 0x00031EDA
		public string Code { get; set; }

		// Token: 0x040008CB RID: 2251
		public string AppIdentifier;
	}
}
