using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000024 RID: 36
	public partial class AIEffectGalleryPage : Page
	{
		// Token: 0x0600019C RID: 412 RVA: 0x00008B92 File Offset: 0x00006D92
		public AIEffectGalleryPage()
		{
			this.InitializeComponent();
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00008BC6 File Offset: 0x00006DC6
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
		}

		// Token: 0x0600019E RID: 414 RVA: 0x00008BD0 File Offset: 0x00006DD0
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AIEffectGalleryPage.<Page_Loaded>d__8 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<AIEffectGalleryPage.<Page_Loaded>d__8>(ref <Page_Loaded>d__);
		}

		// Token: 0x0600019F RID: 415 RVA: 0x00008C08 File Offset: 0x00006E08
		private void LoadLanguageStrings()
		{
			string AIEffectBackgroundLabelString = Settings.GetValueString("lang_selectaieffect");
			if (!string.IsNullOrEmpty(AIEffectBackgroundLabelString))
			{
				this.AIEffectBackgroundGridLabel.Content = AIEffectBackgroundLabelString;
			}
		}

		// Token: 0x060001A0 RID: 416 RVA: 0x00008C34 File Offset: 0x00006E34
		private void CheckAndAddAIEffectPrefabThreadFunc()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.AIEffectItems.Clear();
				this.AIEffectPrefabs.Clear();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			for (int i = 0; i < this.aiEffectLocals.Count; i++)
			{
				this.CreateAIEffectItem(this.aiEffectLocals[i]);
			}
		}

		// Token: 0x060001A1 RID: 417 RVA: 0x00008C90 File Offset: 0x00006E90
		private Task LoadAIEffectTargets()
		{
			AIEffectGalleryPage.<LoadAIEffectTargets>d__11 <LoadAIEffectTargets>d__;
			<LoadAIEffectTargets>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<LoadAIEffectTargets>d__.<>4__this = this;
			<LoadAIEffectTargets>d__.<>1__state = -1;
			<LoadAIEffectTargets>d__.<>t__builder.Start<AIEffectGalleryPage.<LoadAIEffectTargets>d__11>(ref <LoadAIEffectTargets>d__);
			return <LoadAIEffectTargets>d__.<>t__builder.Task;
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x00008CD3 File Offset: 0x00006ED3
		private void AIEffectGalleryItem_ClickEvent(string obj)
		{
			AIEffectGalleryPage.selectedTarget = true;
			AIEffectGalleryPage.selectedAIEffectGalleryItem = obj;
			ActivationKingWindow.SetPage(ApplicationPage.PhotoShootPage, false);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x00008CE8 File Offset: 0x00006EE8
		private void CreateAIEffectItem(AiEffectLocal AIEffect)
		{
			if (!File.Exists(AIEffect.FileDetails.FullPath))
			{
				return;
			}
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				AIEffectGalleryPrefab AIEffectPrefab = new AIEffectGalleryPrefab();
				AIEffectPrefab.ClickEvent += this.AIEffectGalleryItem_ClickEvent;
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.Height = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)AIEffect.FileDetails.Height) / (double)((float)AIEffect.FileDetails.Width);
				this.AIEffectItems.Add(newFrame);
				AIEffectPrefab.LoadImage(AIEffect);
				AIEffectPrefab.AIEffectTitle.Text = AIEffect.Title;
				newFrame.Content = AIEffectPrefab;
				this.AIEffectPrefabs.Add(AIEffectPrefab);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x04000124 RID: 292
		public ObservableCollection<Frame> AIEffectItems = new ObservableCollection<Frame>();

		// Token: 0x04000125 RID: 293
		public List<AIEffectGalleryPrefab> AIEffectPrefabs = new List<AIEffectGalleryPrefab>();

		// Token: 0x04000126 RID: 294
		private List<AiEffectLocal> aiEffectLocals;

		// Token: 0x04000127 RID: 295
		public static bool selectedTarget;

		// Token: 0x04000128 RID: 296
		private string appPath = AppDomain.CurrentDomain.BaseDirectory;

		// Token: 0x04000129 RID: 297
		public static string selectedAIEffectGalleryItem;
	}
}
