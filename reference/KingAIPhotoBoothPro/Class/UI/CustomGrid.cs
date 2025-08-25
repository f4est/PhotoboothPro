using System;
using System.Windows.Controls;

namespace KingAIPhotoBoothPro.Class.UI
{
	// Token: 0x020000BF RID: 191
	public class CustomGrid : Grid
	{
		// Token: 0x06000A77 RID: 2679 RVA: 0x0003C829 File Offset: 0x0003AA29
		public CustomGrid()
		{
			this.InitializeDefaultComponents();
		}

		// Token: 0x06000A78 RID: 2680 RVA: 0x0003C838 File Offset: 0x0003AA38
		private void InitializeDefaultComponents()
		{
			Border border = new Border();
			base.Children.Add(border);
		}
	}
}
