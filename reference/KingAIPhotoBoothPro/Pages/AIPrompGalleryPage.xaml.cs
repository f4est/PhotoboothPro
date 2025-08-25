using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Pages.ActivationKing;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000026 RID: 38
	public partial class AIPrompGalleryPage : Page
	{
		// Token: 0x060001AE RID: 430 RVA: 0x00009007 File Offset: 0x00007207
		public AIPrompGalleryPage()
		{
			this.InitializeComponent();
		}

		// Token: 0x060001AF RID: 431 RVA: 0x0000903B File Offset: 0x0000723B
		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			ActivationKingWindow.SetPage(ApplicationPage.HomePage, false);
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x00009044 File Offset: 0x00007244
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			AIPrompGalleryPage.<Page_Loaded>d__9 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<AIPrompGalleryPage.<Page_Loaded>d__9>(ref <Page_Loaded>d__);
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0000907C File Offset: 0x0000727C
		private void LoadLanguageStrings()
		{
			string AIPrompBackgroundLabelString = Settings.GetValueString("lang_selectaipromp");
			if (!string.IsNullOrEmpty(AIPrompBackgroundLabelString))
			{
				this.AIPrompBackgroundGridLabel.Content = AIPrompBackgroundLabelString;
			}
		}

		// Token: 0x060001B2 RID: 434 RVA: 0x000090A8 File Offset: 0x000072A8
		private void CheckAndAddAIPrompPrefabThreadFunc()
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				this.AIPrompItems.Clear();
				this.AIPrompPrefabs.Clear();
			}), DispatcherPriority.Normal, Array.Empty<object>());
			op.Wait();
			for (int i = 0; i < this.AIPrompLocals.Count; i++)
			{
				this.CreateAIPrompItem(this.AIPrompLocals[i]);
			}
		}

		// Token: 0x060001B3 RID: 435 RVA: 0x00009104 File Offset: 0x00007304
		private void LoadAIPrompTargets()
		{
			string eventSamplePromptPath = SamplePromptsPage.samplePromptsJsonPath;
			AIPrompGalleryPage.samplePromptData = SamplePromptsPage.LoadSamplePromptData(eventSamplePromptPath);
			if (AIPrompGalleryPage.samplePromptData != null)
			{
				this.AIPrompLocals = AIPrompGalleryPage.samplePromptData.SamplePrompts;
			}
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00009139 File Offset: 0x00007339
		private void AIPrompGalleryItem_ClickEvent(SamplePrompt obj)
		{
			AIPrompGalleryPage.selectedTarget = true;
			AIPrompGalleryPage.selectedAIPrompGalleryItem = obj;
			ActivationKingWindow.SetPage(ApplicationPage.PhotoShootPage, false);
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x00009150 File Offset: 0x00007350
		private void CreateAIPrompItem(SamplePrompt AIPromp)
		{
			DispatcherOperation op = base.Dispatcher.BeginInvoke(new Action(delegate()
			{
				SamplePromptPrefab samplePromptPrefab = new SamplePromptPrefab();
				samplePromptPrefab.ClickEvent += this.AIPrompGalleryItem_ClickEvent;
				Frame newFrame = new Frame();
				newFrame.Margin = new Thickness(25.0);
				newFrame.Height = this.UniformGridContext.ActualWidth * 0.23 * (double)((float)AIPromp.FileDetails.Height) / (double)((float)AIPromp.FileDetails.Width);
				this.AIPrompItems.Add(newFrame);
				samplePromptPrefab.LoadImage(Path.Combine(new string[]
				{
					AIPromp.FileDetails.FullPath
				}), AIPromp);
				samplePromptPrefab.SamplePromptTitle.Text = AIPromp.Title;
				newFrame.Content = samplePromptPrefab;
				this.AIPrompPrefabs.Add(samplePromptPrefab);
			}), DispatcherPriority.Normal, Array.Empty<object>());
		}

		// Token: 0x0400013B RID: 315
		public ObservableCollection<Frame> AIPrompItems = new ObservableCollection<Frame>();

		// Token: 0x0400013C RID: 316
		public List<SamplePromptPrefab> AIPrompPrefabs = new List<SamplePromptPrefab>();

		// Token: 0x0400013D RID: 317
		private List<SamplePrompt> AIPrompLocals;

		// Token: 0x0400013E RID: 318
		public static bool selectedTarget;

		// Token: 0x0400013F RID: 319
		private static SamplePromptData samplePromptData;

		// Token: 0x04000140 RID: 320
		private string appPath = AppDomain.CurrentDomain.BaseDirectory;

		// Token: 0x04000141 RID: 321
		public static SamplePrompt selectedAIPrompGalleryItem;
	}
}
