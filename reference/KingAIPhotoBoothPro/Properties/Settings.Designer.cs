using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace KingAIPhotoBoothPro.Properties
{
	// Token: 0x0200001B RID: 27
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.8.0.0")]
	internal sealed partial class Settings : ApplicationSettingsBase
	{
		// Token: 0x1700002A RID: 42
		// (get) Token: 0x06000103 RID: 259 RVA: 0x00007134 File Offset: 0x00005334
		public static Settings Default
		{
			get
			{
				return Settings.defaultInstance;
			}
		}

		// Token: 0x040000CF RID: 207
		private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());
	}
}
