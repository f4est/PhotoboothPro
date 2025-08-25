using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace KingAIPhotoBoothPro.Properties
{
	// Token: 0x0200001A RID: 26
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x060000FF RID: 255 RVA: 0x000070E3 File Offset: 0x000052E3
		internal Resources()
		{
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x06000100 RID: 256 RVA: 0x000070EC File Offset: 0x000052EC
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					ResourceManager temp = new ResourceManager("KingAIPhotoBoothPro.Properties.Resources", typeof(Resources).Assembly);
					Resources.resourceMan = temp;
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x06000101 RID: 257 RVA: 0x00007125 File Offset: 0x00005325
		// (set) Token: 0x06000102 RID: 258 RVA: 0x0000712C File Offset: 0x0000532C
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x040000CD RID: 205
		private static ResourceManager resourceMan;

		// Token: 0x040000CE RID: 206
		private static CultureInfo resourceCulture;
	}
}
