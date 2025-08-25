using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x02000051 RID: 81
	public partial class AIEffectPrefab : Page
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000677 RID: 1655 RVA: 0x0002E590 File Offset: 0x0002C790
		// (remove) Token: 0x06000678 RID: 1656 RVA: 0x0002E5C4 File Offset: 0x0002C7C4
		public static event Action<string> EnabledCheck;

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06000679 RID: 1657 RVA: 0x0002E5F8 File Offset: 0x0002C7F8
		// (remove) Token: 0x0600067A RID: 1658 RVA: 0x0002E62C File Offset: 0x0002C82C
		public static event Action<string> EnabledUnCheck;

		// Token: 0x0600067B RID: 1659 RVA: 0x0002E65F File Offset: 0x0002C85F
		public AIEffectPrefab(AiEffectLocal ThisAIEffect)
		{
			this.InitializeComponent();
			this.thisAIEffect = ThisAIEffect;
			this.NewPrefabGrid.Visibility = Visibility.Visible;
		}

		// Token: 0x0600067C RID: 1660 RVA: 0x0002E680 File Offset: 0x0002C880
		public Task ChangeBitmap(string path)
		{
			AIEffectPrefab.<ChangeBitmap>d__10 <ChangeBitmap>d__;
			<ChangeBitmap>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ChangeBitmap>d__.<>4__this = this;
			<ChangeBitmap>d__.path = path;
			<ChangeBitmap>d__.<>1__state = -1;
			<ChangeBitmap>d__.<>t__builder.Start<AIEffectPrefab.<ChangeBitmap>d__10>(ref <ChangeBitmap>d__);
			return <ChangeBitmap>d__.<>t__builder.Task;
		}

		// Token: 0x0600067D RID: 1661 RVA: 0x0002E6CC File Offset: 0x0002C8CC
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AIEffectPrefab.<Page_Loaded>d__11 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<AIEffectPrefab.<Page_Loaded>d__11>(ref <Page_Loaded>d__);
		}

		// Token: 0x0600067E RID: 1662 RVA: 0x0002E703 File Offset: 0x0002C903
		private void Page_Unloaded(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x0600067F RID: 1663 RVA: 0x0002E705 File Offset: 0x0002C905
		private void CheckElement_Checked(object sender, RoutedEventArgs e)
		{
			if (this.isLoad)
			{
				return;
			}
			if (AIEffectPrefab.EnabledCheck != null)
			{
				AIEffectPrefab.EnabledCheck(this.thisAIEffect.Id);
			}
		}

		// Token: 0x06000680 RID: 1664 RVA: 0x0002E72C File Offset: 0x0002C92C
		private void CheckElement_Unchecked(object sender, RoutedEventArgs e)
		{
			if (this.isLoad)
			{
				return;
			}
			if (AIEffectPrefab.EnabledUnCheck != null)
			{
				AIEffectPrefab.EnabledUnCheck(this.thisAIEffect.Id);
			}
		}

		// Token: 0x04000738 RID: 1848
		public int index;

		// Token: 0x04000739 RID: 1849
		private AiEffectLocal thisAIEffect;

		// Token: 0x0400073C RID: 1852
		private bool isLoad;
	}
}
