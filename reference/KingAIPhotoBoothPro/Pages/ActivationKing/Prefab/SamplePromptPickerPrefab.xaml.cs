using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ImageMagick;
using KingAIPhotoBoothPro.Modals;

namespace KingAIPhotoBoothPro.Pages.ActivationKing.Prefab
{
	// Token: 0x0200005B RID: 91
	public partial class SamplePromptPickerPrefab : Page
	{
		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x0600070B RID: 1803 RVA: 0x000305A1 File Offset: 0x0002E7A1
		// (set) Token: 0x0600070C RID: 1804 RVA: 0x000305A9 File Offset: 0x0002E7A9
		public string DefaultStatus { get; private set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x0600070D RID: 1805 RVA: 0x000305B2 File Offset: 0x0002E7B2
		// (set) Token: 0x0600070E RID: 1806 RVA: 0x000305BA File Offset: 0x0002E7BA
		public string txtTitle { get; set; }

		// Token: 0x14000018 RID: 24
		// (add) Token: 0x0600070F RID: 1807 RVA: 0x000305C4 File Offset: 0x0002E7C4
		// (remove) Token: 0x06000710 RID: 1808 RVA: 0x000305F8 File Offset: 0x0002E7F8
		public static event Action<SamplePromptPickerPrefab> imageLoaded;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06000711 RID: 1809 RVA: 0x0003062C File Offset: 0x0002E82C
		// (remove) Token: 0x06000712 RID: 1810 RVA: 0x00030660 File Offset: 0x0002E860
		public static event Action<SamplePromptPickerPrefab> deleteClicked;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06000713 RID: 1811 RVA: 0x00030694 File Offset: 0x0002E894
		// (remove) Token: 0x06000714 RID: 1812 RVA: 0x000306C8 File Offset: 0x0002E8C8
		public static event Action newSamplePromptAdded;

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000715 RID: 1813 RVA: 0x000306FB File Offset: 0x0002E8FB
		// (set) Token: 0x06000716 RID: 1814 RVA: 0x00030703 File Offset: 0x0002E903
		public SamplePrompt SamplePrompt { get; set; }

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06000717 RID: 1815 RVA: 0x0003070C File Offset: 0x0002E90C
		// (remove) Token: 0x06000718 RID: 1816 RVA: 0x00030740 File Offset: 0x0002E940
		public static event Action<string, int> titleChanged;

		// Token: 0x06000719 RID: 1817 RVA: 0x00030774 File Offset: 0x0002E974
		public SamplePromptPickerPrefab(int index, SamplePrompt samplePrompt, bool initialization = false)
		{
			this.index = index;
			this.txtTitle = samplePrompt.Title;
			this.initialization = initialization;
			this.path = samplePrompt.FileDetails.FullPath;
			this.SamplePrompt = samplePrompt;
			this.InitializeComponent();
		}

		// Token: 0x0600071A RID: 1818 RVA: 0x000307D0 File Offset: 0x0002E9D0
		private void AddBackground_Click(object sender, RoutedEventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = "JPG (*.jpg)|*.jpg";
				DialogResult result = dialog.ShowDialog();
				if (result != DialogResult.OK)
				{
					return;
				}
				this.path = dialog.FileName;
			}
			byte[] data = File.ReadAllBytes(this.path);
			if (data.Length > 10485760)
			{
				MessageBoxWindow.CreateWindow("Image is too large", "Image must be smaller than 10 MB.", new List<MessageBoxWindow.ButtonType>
				{
					MessageBoxWindow.ButtonType.Continue
				}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
				return;
			}
			string uploadedFileNewName = string.Format("{0:yyyy_MM_dd_HH_mm_ss_fff}_{1}", DateTime.Now, System.IO.Path.GetFileName(this.path));
			string uploadedFileDesiredPath = System.IO.Path.Combine(SamplePromptsPage.imageFolderPath, uploadedFileNewName);
			File.Copy(this.path, uploadedFileDesiredPath, true);
			try
			{
				using (MagickImage mImage = new MagickImage(uploadedFileDesiredPath))
				{
					this.SamplePrompt.FileDetails = new FileInformation
					{
						Filename = uploadedFileNewName,
						Directory = SamplePromptsPage.imageFolderPath,
						Width = (int)mImage.Width,
						Height = (int)mImage.Height,
						Filesize = (long)mImage.ToByteArray().Length
					};
				}
			}
			catch
			{
				this.SamplePrompt.FileDetails = new FileInformation
				{
					Filename = uploadedFileNewName,
					Directory = SamplePromptsPage.imageFolderPath,
					Width = 0,
					Height = 0,
					Filesize = (long)data.Length
				};
			}
			this.path = this.SamplePrompt.FileDetails.FullPath;
			this.LoadImage();
			SamplePromptPickerPrefab.newSamplePromptAdded();
		}

		// Token: 0x0600071B RID: 1819 RVA: 0x00030978 File Offset: 0x0002EB78
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			this.Load();
		}

		// Token: 0x0600071C RID: 1820 RVA: 0x00030980 File Offset: 0x0002EB80
		private void LoadImage()
		{
			try
			{
				if (File.Exists(this.SamplePrompt.FileDetails.FullPath))
				{
					BitmapImage bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.UriSource = new Uri(this.SamplePrompt.FileDetails.FullPath, UriKind.Absolute);
					bitmap.DecodePixelWidth = 200;
					bitmap.EndInit();
					this.prefabImage.Source = bitmap;
					if (!this.initialization)
					{
						Action<SamplePromptPickerPrefab> action = SamplePromptPickerPrefab.imageLoaded;
						if (action != null)
						{
							action(this);
						}
					}
					this.BackgroundText.Text = "";
				}
				else
				{
					this.BackgroundText.Text = "Background Image (File Not Found)";
				}
			}
			catch
			{
				this.BackgroundText.Text = "Background Image";
			}
		}

		// Token: 0x0600071D RID: 1821 RVA: 0x00030A4C File Offset: 0x0002EC4C
		public void Load()
		{
			if (!string.IsNullOrEmpty(this.path))
			{
				this.MakeEnable();
				if (this.SamplePrompt.Prompts.Count > 0)
				{
					this.promptIndex = 1;
					this.txtSamplePromptPrompt.Text = this.SamplePrompt.Prompts[0];
				}
				else
				{
					this.SamplePrompt.Prompts.Add("Write Prompt");
					this.txtSamplePromptPrompt.Text = "Write Prompt";
				}
				this.txtSamplePromptTitle.Text = this.SamplePrompt.Title;
				this.PromptHeaderText.Text = string.Format("Prompt ({0})", this.promptIndex);
				this.LoadImage();
				return;
			}
			this.MakeDisable();
		}

		// Token: 0x0600071E RID: 1822 RVA: 0x00030B0F File Offset: 0x0002ED0F
		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			SamplePromptPickerPrefab.deleteClicked(this);
		}

		// Token: 0x0600071F RID: 1823 RVA: 0x00030B1C File Offset: 0x0002ED1C
		private void TxtSamplePromptTitle_TextChanged(object sender, TextChangedEventArgs e)
		{
			string defaultText = "write title";
			if (this.txtSamplePromptTitle.Text.ToLower() == defaultText)
			{
				return;
			}
			SamplePromptPickerPrefab.titleChanged(this.txtSamplePromptTitle.Text, this.index);
		}

		// Token: 0x06000720 RID: 1824 RVA: 0x00030B64 File Offset: 0x0002ED64
		private void TxtSamplePromptPrompt_TextChanged(object sender, TextChangedEventArgs e)
		{
			string defaultText = "write prompt";
			if (!(this.txtSamplePromptPrompt.Text.ToLower() == defaultText))
			{
				this.SetPromptValue();
			}
		}

		// Token: 0x06000721 RID: 1825 RVA: 0x00030B98 File Offset: 0x0002ED98
		private void SetPromptValue()
		{
			if (this.promptIndex - 1 < this.SamplePrompt.Prompts.Count)
			{
				this.SamplePrompt.Prompts[this.promptIndex - 1] = this.txtSamplePromptPrompt.Text;
				return;
			}
			this.SamplePrompt.Prompts.Add(this.txtSamplePromptPrompt.Text);
		}

		// Token: 0x06000722 RID: 1826 RVA: 0x00030BFE File Offset: 0x0002EDFE
		private void CheckBox_Checked(object sender, RoutedEventArgs e)
		{
		}

		// Token: 0x06000723 RID: 1827 RVA: 0x00030C00 File Offset: 0x0002EE00
		private void ButtonPreviousVariant_Click(object sender, RoutedEventArgs e)
		{
			if (this.promptIndex - 1 >= 1)
			{
				this.promptIndex--;
				this.promptIndexText.Text = this.promptIndex.ToString();
				this.txtSamplePromptPrompt.Text = this.SamplePrompt.Prompts[this.promptIndex - 1];
				this.PromptHeaderText.Text = string.Format("Prompt ({0})", this.promptIndex);
			}
		}

		// Token: 0x06000724 RID: 1828 RVA: 0x00030C80 File Offset: 0x0002EE80
		private void ButtonNextVariant_Click(object sender, RoutedEventArgs e)
		{
			if (this.promptIndex < this.SamplePrompt.Prompts.Count)
			{
				this.promptIndex++;
				this.promptIndexText.Text = this.promptIndex.ToString();
				this.txtSamplePromptPrompt.Text = this.SamplePrompt.Prompts[this.promptIndex - 1];
				this.PromptHeaderText.Text = string.Format("Prompt ({0})", this.promptIndex);
			}
		}

		// Token: 0x06000725 RID: 1829 RVA: 0x00030D0C File Offset: 0x0002EF0C
		private void AddVariant_Click(object sender, RoutedEventArgs e)
		{
			this.SamplePrompt.Prompts.Add("");
			this.promptIndex = this.SamplePrompt.Prompts.Count;
			this.promptIndexText.Text = this.promptIndex.ToString();
			this.txtSamplePromptPrompt.Text = "";
			this.txtSamplePromptPrompt.Focus();
			this.PromptHeaderText.Text = string.Format("Prompt ({0})", this.promptIndex);
		}

		// Token: 0x06000726 RID: 1830 RVA: 0x00030D96 File Offset: 0x0002EF96
		private void AddPrompt_Click(object sender, RoutedEventArgs e)
		{
			this.MakeEnable();
			this.AddBackground_Click(null, null);
		}

		// Token: 0x06000727 RID: 1831 RVA: 0x00030DA8 File Offset: 0x0002EFA8
		private void MakeDisable()
		{
			this.InsideGrid.Opacity = 0.4000000059604645;
			this.DeleteButton.Visibility = Visibility.Collapsed;
			this.AddPromptButton.Visibility = Visibility.Visible;
			this.AddButton.Visibility = Visibility.Collapsed;
			this.AddVariantButton.Visibility = Visibility.Collapsed;
			this.NextPromptButtonBorder.Visibility = Visibility.Collapsed;
			this.PreviousPromptButtonBorder.Visibility = Visibility.Collapsed;
			this.promptIndexText.Visibility = Visibility.Collapsed;
		}

		// Token: 0x06000728 RID: 1832 RVA: 0x00030E20 File Offset: 0x0002F020
		private void MakeEnable()
		{
			this.InsideGrid.Opacity = 1.0;
			this.DeleteButton.Visibility = Visibility.Visible;
			this.AddPromptButton.Visibility = Visibility.Collapsed;
			this.AddButton.Visibility = Visibility.Visible;
			this.AddVariantButton.Visibility = Visibility.Visible;
			this.NextPromptButtonBorder.Visibility = Visibility.Visible;
			this.PreviousPromptButtonBorder.Visibility = Visibility.Visible;
			this.promptIndexText.Visibility = Visibility.Visible;
		}

		// Token: 0x040007BA RID: 1978
		public string path;

		// Token: 0x040007BB RID: 1979
		public int index;

		// Token: 0x040007BF RID: 1983
		public int promptIndex = 1;

		// Token: 0x040007C0 RID: 1984
		public int promptCount = 1;

		// Token: 0x040007C1 RID: 1985
		private bool initialization;
	}
}
