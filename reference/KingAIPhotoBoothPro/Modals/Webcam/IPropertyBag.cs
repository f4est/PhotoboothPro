using System;
using System.Runtime.InteropServices;

namespace KingAIPhotoBoothPro.Modals.Webcam
{
	// Token: 0x02000083 RID: 131
	[Guid("55272A00-42CB-11CE-8135-00AA004BB851")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ComImport]
	internal interface IPropertyBag
	{
		// Token: 0x060008D3 RID: 2259
		[PreserveSig]
		int Read([MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, [MarshalAs(UnmanagedType.Struct)] [In] [Out] ref object value, [In] IntPtr errorLog);

		// Token: 0x060008D4 RID: 2260
		[PreserveSig]
		int Write([MarshalAs(UnmanagedType.LPWStr)] [In] string propertyName, [MarshalAs(UnmanagedType.Struct)] [In] ref object value);
	}
}
