using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace KingAIPhotoBoothPro.Modals.Webcam
{
	// Token: 0x02000082 RID: 130
	[Guid("29840822-5B84-11D0-BD3B-00A0C911CE86")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface ICreateDevEnum
	{
		// Token: 0x060008D2 RID: 2258
		[PreserveSig]
		int CreateClassEnumerator([In] ref Guid deviceClass, out IEnumMoniker enumMoniker, [In] int flags);
	}
}
