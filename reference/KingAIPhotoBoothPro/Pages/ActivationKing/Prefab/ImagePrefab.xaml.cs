using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000059 RID: 89
	public partial class ImagePrefab : Page
	{
		// Token: 0x170000A3 RID: 163
		// (get) Token: 0x060006FD RID: 1789 RVA: 0x000301BC File Offset: 0x0002E3BC
		// (set) Token: 0x060006FE RID: 1790 RVA: 0x000301C4 File Offset: 0x0002E3C4
		public ImageSource TemplateImage { get; set; }

		// Token: 0x060006FF RID: 1791 RVA: 0x000301CD File Offset: 0x0002E3CD
		public ImagePrefab()
		{
			this.InitializeComponent();
		}
	}
}
