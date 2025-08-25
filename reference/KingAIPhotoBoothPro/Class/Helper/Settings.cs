using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Xml;
using ImageMagick;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D5 RID: 213
	public class Settings
	{
		// Token: 0x06000B25 RID: 2853 RVA: 0x0004165C File Offset: 0x0003F85C
		public static T GetValueEnum<T>(string key) where T : Enum
		{
			T[] options = (T[])Enum.GetValues(typeof(T));
			string value = Settings.GetValueString(key);
			if (string.IsNullOrEmpty(value))
			{
				return options.FirstOrDefault<T>();
			}
			foreach (T option in options)
			{
				string modeString = option.GetDisplayName();
				if (modeString == value)
				{
					return option;
				}
			}
			return options.FirstOrDefault<T>();
		}

		// Token: 0x06000B26 RID: 2854 RVA: 0x000416D0 File Offset: 0x0003F8D0
		public static string GetValueString(string key)
		{
			key = key.ToLower();
			string value = null;
			Settings.SettingsPairs.TryGetValue(key, out value);
			return value;
		}

		// Token: 0x06000B27 RID: 2855 RVA: 0x000416F8 File Offset: 0x0003F8F8
		public static string GetValueString(string key, bool enabledReturnDefault)
		{
			key = key.ToLower();
			string value = null;
			Settings.SettingsPairs.TryGetValue(key, out value);
			if (value == null && enabledReturnDefault)
			{
				Settings.DefaultValues.TryGetValue(key, out value);
			}
			return value;
		}

		// Token: 0x06000B28 RID: 2856 RVA: 0x00041734 File Offset: 0x0003F934
		public static float? GetValueFloat(string key)
		{
			float? result;
			if (string.IsNullOrEmpty(Settings.GetValueString(key)))
			{
				result = null;
				return result;
			}
			try
			{
				result = new float?((float)Convert.ToDouble(Settings.GetValueString(key)));
			}
			catch (Exception)
			{
				throw new Exception("Wrong Value");
			}
			return result;
		}

		// Token: 0x06000B29 RID: 2857 RVA: 0x0004178C File Offset: 0x0003F98C
		public static int? GetValueInt(string key)
		{
			int? result;
			if (string.IsNullOrEmpty(Settings.GetValueString(key)))
			{
				result = null;
				return result;
			}
			try
			{
				result = new int?(Convert.ToInt32(Settings.GetValueString(key)));
			}
			catch (Exception)
			{
				throw new Exception("Wrong Value");
			}
			return result;
		}

		// Token: 0x06000B2A RID: 2858 RVA: 0x000417E4 File Offset: 0x0003F9E4
		public static bool? GetValueBoolean(string key)
		{
			if (string.IsNullOrEmpty(Settings.GetValueString(key)))
			{
				return new bool?(false);
			}
			bool? result;
			try
			{
				result = new bool?(Convert.ToBoolean(Settings.GetValueString(key)));
			}
			catch (Exception)
			{
				throw new Exception("Wrong Value");
			}
			return result;
		}

		// Token: 0x06000B2B RID: 2859 RVA: 0x00041838 File Offset: 0x0003FA38
		public static Color GetValueColor(string key)
		{
			string colorString = Settings.GetValueString(key);
			if (!string.IsNullOrEmpty(colorString))
			{
				try
				{
					return (Color)ColorConverter.ConvertFromString(colorString);
				}
				catch (Exception)
				{
					return (Color)ColorConverter.ConvertFromString("#000000");
				}
			}
			return (Color)ColorConverter.ConvertFromString("#000000");
		}

		// Token: 0x06000B2C RID: 2860 RVA: 0x00041898 File Offset: 0x0003FA98
		public static Task<bool> RetrySetValueAsync(string key, string value, int retryCount = 3, int delayMilliseconds = 1000)
		{
			Settings.<RetrySetValueAsync>d__8 <RetrySetValueAsync>d__;
			<RetrySetValueAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<RetrySetValueAsync>d__.key = key;
			<RetrySetValueAsync>d__.value = value;
			<RetrySetValueAsync>d__.retryCount = retryCount;
			<RetrySetValueAsync>d__.delayMilliseconds = delayMilliseconds;
			<RetrySetValueAsync>d__.<>1__state = -1;
			<RetrySetValueAsync>d__.<>t__builder.Start<Settings.<RetrySetValueAsync>d__8>(ref <RetrySetValueAsync>d__);
			return <RetrySetValueAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000B2D RID: 2861 RVA: 0x000418F3 File Offset: 0x0003FAF3
		public static void SetValue(string key, string value, bool writeLocalFile = true)
		{
			key = key.ToLower();
			if (Settings.SettingsPairs.ContainsKey(key))
			{
				Settings.SettingsPairs[key] = value;
			}
			else
			{
				Settings.SettingsPairs.Add(key, value);
			}
			if (writeLocalFile)
			{
				Settings.WriteSettingsToFile();
			}
		}

		// Token: 0x06000B2E RID: 2862 RVA: 0x0004192C File Offset: 0x0003FB2C
		public static void SetValue(string key, bool value)
		{
			Settings.SetValue(key, value.ToString(), true);
		}

		// Token: 0x06000B2F RID: 2863 RVA: 0x0004193C File Offset: 0x0003FB3C
		public static void SetValue(string key, int value)
		{
			Settings.SetValue(key, value.ToString(), true);
		}

		// Token: 0x06000B30 RID: 2864 RVA: 0x0004194C File Offset: 0x0003FB4C
		public static void SetValue(string key, float value)
		{
			Settings.SetValue(key, value.ToString(), true);
		}

		// Token: 0x06000B31 RID: 2865 RVA: 0x0004195C File Offset: 0x0003FB5C
		public static void OnEventChanged(OperationalEvent operationalEvent)
		{
			Settings.ReadSettingsFromEventFile();
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x00041964 File Offset: 0x0003FB64
		private static void ReadSettingsFromEventFile()
		{
			if (Settings.SettingsReadIsWorking)
			{
				return;
			}
			while (Settings.SettingsWriteIsWorking)
			{
				Thread.Sleep(100);
			}
			if (Settings.CurrentEventFolder == null)
			{
				return;
			}
			if (!File.Exists(EventManagementPage.GetCurrentEvent().SettingsXMLPath))
			{
				Settings.WriteSettingsToFile();
				return;
			}
			Settings.SettingsReadIsWorking = true;
			XmlDocument _data = new XmlDocument();
			try
			{
				_data.Load(EventManagementPage.GetCurrentEvent().SettingsXMLPath);
				Settings.SettingsPairs = new Dictionary<string, string>();
				foreach (object obj in _data.DocumentElement.ChildNodes)
				{
					XmlNode item = (XmlNode)obj;
					Settings.SetValue(item.Name, item.InnerText, false);
				}
			}
			catch (Exception ex)
			{
				Settings.WriteSettingsToFile();
				Settings.SettingsReadIsWorking = false;
				return;
			}
			Settings.SettingsReadIsWorking = false;
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x00041A4C File Offset: 0x0003FC4C
		private static void WriteSettingsToFile()
		{
			if (Settings.SettingsWriteIsWorking)
			{
				return;
			}
			while (Settings.SettingsReadIsWorking)
			{
				Thread.Sleep(100);
			}
			if (Settings.CurrentEventFolder == null)
			{
				Console.WriteLine("Current Event Null!");
				return;
			}
			Settings.SettingsWriteIsWorking = true;
			XmlDocument _data = new XmlDocument();
			bool writeSuccess = false;
			do
			{
				try
				{
					Settings.CreateXMLFile(EventManagementPage.GetCurrentEvent().SettingsXMLPath, "Settings", AppInfo.AppClass.Name);
					writeSuccess = true;
				}
				catch (Exception)
				{
				}
			}
			while (!writeSuccess);
			_data.Load(EventManagementPage.GetCurrentEvent().SettingsXMLPath);
			foreach (string key in Settings.SettingsPairs.Keys)
			{
				XmlElement _element = _data.CreateElement(key);
				_element.InnerText = Settings.SettingsPairs[key];
				_data.DocumentElement.AppendChild(_element);
			}
			XmlTextWriter _write = new XmlTextWriter(EventManagementPage.GetCurrentEvent().SettingsXMLPath, null);
			_write.Formatting = System.Xml.Formatting.Indented;
			_data.WriteContentTo(_write);
			_write.Close();
			Settings.SettingsWriteIsWorking = false;
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x00041B70 File Offset: 0x0003FD70
		private static void CreateXMLFile(string fileName, string startElement, string comment = "Activation King")
		{
			XmlTextWriter createXML = new XmlTextWriter(fileName, Encoding.UTF8);
			createXML.WriteStartDocument();
			createXML.WriteComment(comment);
			createXML.WriteStartElement(startElement);
			createXML.WriteEndDocument();
			createXML.Close();
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x00041BAC File Offset: 0x0003FDAC
		public static void DeleteSamplePromptsSettingsElement(string key)
		{
			key = key.ToLower();
			if (Settings.SettingsPairs.ContainsKey(key))
			{
				Settings.SettingsPairs.Remove(key);
			}
			if (Settings.SettingsPairs.ContainsKey(key + "title"))
			{
				Settings.SettingsPairs.Remove(key + "title");
			}
			if (Settings.SettingsPairs.ContainsKey(key + "prompt"))
			{
				Settings.SettingsPairs.Remove(key + "prompt");
			}
			Settings.WriteSettingsToFile();
		}

		// Token: 0x17000190 RID: 400
		// (get) Token: 0x06000B36 RID: 2870 RVA: 0x00041C39 File Offset: 0x0003FE39
		public static string CurrentEventFolder
		{
			get
			{
				if (EventManagementPage.GetCurrentEvent() == null)
				{
					return null;
				}
				return EventManagementPage.GetCurrentEvent().DirectoryPath;
			}
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x00041C50 File Offset: 0x0003FE50
		public static void SetAllDefaultValues()
		{
			Settings.SettingsPairs.Clear();
			foreach (KeyValuePair<string, string> pair in Settings.DefaultValues)
			{
				Settings.SetValue(pair.Key, pair.Value, true);
			}
			Settings.SetSampleVideoDefaultValues();
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x00041CC0 File Offset: 0x0003FEC0
		public static string GetSampleVideoDefaultValue(string key)
		{
			string sampleVideoTargetFolder = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "SampleVideo");
			string[] fileEntries = (from s in (from s in Directory.EnumerateFiles(sampleVideoTargetFolder, "*.*", SearchOption.TopDirectoryOnly)
			where s.EndsWith(".mp4")
			select s).ToArray<string>()
			orderby s
			select s).ToArray<string>();
			string[] KeyValues = new string[]
			{
				"mainbackgroundvideo",
				"waitvideo",
				"processwaitvideo",
				"resultvideo"
			};
			int index = KeyValues.ToList<string>().FindIndex((string x) => x == key);
			return fileEntries[index];
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x00041D94 File Offset: 0x0003FF94
		public static void SetSampleVideoDefaultValues()
		{
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string sampleVideosDefaultFolderPath = Path.Combine(appDirectory, "SampleVideo");
			string[] fileEntries = (from s in (from s in Directory.EnumerateFiles(sampleVideosDefaultFolderPath, "*.*", SearchOption.TopDirectoryOnly)
			where s.EndsWith(".mp4")
			select s).ToArray<string>()
			orderby s
			select s).ToArray<string>();
			string[] KeyValues = new string[]
			{
				"mainbackgroundvideo",
				"waitvideo",
				"processwaitvideo",
				"resultvideo"
			};
			string sampleVideoTargetFolder = Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "SampleVideo");
			if (!Directory.Exists(sampleVideoTargetFolder))
			{
				Directory.CreateDirectory(sampleVideoTargetFolder);
			}
			for (int i = 0; i < fileEntries.Length; i++)
			{
				string filePath = fileEntries[i];
				string fileName = Path.GetFileName(filePath);
				string uploadedFileNewName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_fff") + "_defaultBackground_" + fileName;
				string uploadedFileDesiredPath = Path.Combine(sampleVideoTargetFolder, uploadedFileNewName);
				File.Copy(filePath, uploadedFileDesiredPath, true);
				Settings.SetValue(KeyValues[i], uploadedFileDesiredPath, true);
			}
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x00041EC8 File Offset: 0x000400C8
		public static void UpdateSamplePromptsDefaultData()
		{
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string samplePromptsDefaultFolderPath = Path.Combine(appDirectory, "SamplePromptsDefault");
			string jsonFilePath = Path.Combine(samplePromptsDefaultFolderPath, "SamplePromptData.json");
			if (!File.Exists(jsonFilePath))
			{
				Console.WriteLine("SamplePromptData.json dosyası bulunamadı.");
				return;
			}
			string jsonData = File.ReadAllText(jsonFilePath);
			SamplePromptData samplePromptData = JsonConvert.DeserializeObject<SamplePromptData>(jsonData);
			if (samplePromptData == null || samplePromptData.SamplePrompts == null || !samplePromptData.SamplePrompts.Any<SamplePrompt>())
			{
				Console.WriteLine("SamplePromptData.json içinde geçerli veri bulunamadı.");
				return;
			}
			int counter = 0;
			foreach (SamplePrompt prompt in samplePromptData.SamplePrompts)
			{
				string filePath = Path.Combine(samplePromptsDefaultFolderPath, prompt.FileDetails.Filename);
				if (File.Exists(filePath))
				{
					FileInfo fileInfo = new FileInfo(filePath);
					string uploadedFileNewName = string.Format("{0:yyyy_MM_dd_HH_mm_ss_ffffff}_{1:00}.jpg", DateTime.Now, counter++);
					string uploadedFileDesiredPath = Path.Combine(SamplePromptsPage.imageFolderPath, uploadedFileNewName);
					using (MagickImage mImage = new MagickImage(filePath))
					{
						mImage.Format = MagickFormat.Jpg;
						prompt.FileDetails = new FileInformation
						{
							Filename = uploadedFileNewName,
							Directory = SamplePromptsPage.imageFolderPath,
							Width = (int)mImage.Width,
							Height = (int)mImage.Height,
							Filesize = (long)mImage.ToByteArray().Length
						};
						mImage.Write(prompt.FileDetails.FullPath);
						continue;
					}
				}
				Console.WriteLine("Dosya " + prompt.FileDetails.Filename + " klasörde bulunamadı.");
			}
			string updatedJson = JsonConvert.SerializeObject(samplePromptData);
			File.WriteAllText(SamplePromptsPage.samplePromptsJsonPath, updatedJson);
			Console.WriteLine("SamplePromptData.json başarıyla güncellendi.");
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x000420C4 File Offset: 0x000402C4
		public static void UpdateFaceSwapDefaultData()
		{
			string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string faceSwapDefaultFolderPath = Path.Combine(appDirectory, "FaceSwapDefault");
			string jsonFilePath = Path.Combine(faceSwapDefaultFolderPath, "FaceSwapData.json");
			if (!File.Exists(jsonFilePath))
			{
				Console.WriteLine("FaceSwapData.json dosyası bulunamadı.");
				return;
			}
			string jsonData = File.ReadAllText(jsonFilePath);
			FaceSwapBackgroundGallery.FaceSwapData faceSwapData = JsonConvert.DeserializeObject<FaceSwapBackgroundGallery.FaceSwapData>(jsonData);
			if (faceSwapData == null || faceSwapData.FaceSwapTargets == null || !faceSwapData.FaceSwapTargets.Any<FaceSwapTarget>())
			{
				Console.WriteLine("FaceSwapData.json içinde geçerli veri bulunamadı.");
				return;
			}
			int counter = 0;
			foreach (FaceSwapTarget target in faceSwapData.FaceSwapTargets)
			{
				string filePath = Path.Combine(faceSwapDefaultFolderPath, target.FileDetails.Filename);
				if (File.Exists(filePath))
				{
					FileInfo fileInfo = new FileInfo(filePath);
					string uploadedFileNewName = string.Format("{0:yyyy_MM_dd_HH_mm_ss_ffffff}_{1:00}.jpg", DateTime.Now, counter++);
					string uploadedFileDesiredPath = Path.Combine(FaceSwap.imageFolderPath, uploadedFileNewName);
					using (MagickImage mImage = new MagickImage(filePath))
					{
						mImage.Format = MagickFormat.Jpg;
						target.FileDetails = new FileInformation
						{
							Filename = uploadedFileNewName,
							Directory = FaceSwap.imageFolderPath,
							Width = (int)mImage.Width,
							Height = (int)mImage.Height,
							Filesize = (long)mImage.ToByteArray().Length
						};
						mImage.Write(target.FileDetails.FullPath);
						continue;
					}
				}
				Console.WriteLine("Dosya " + target.FileDetails.Filename + " klasörde bulunamadı.");
			}
			string updatedJson = JsonConvert.SerializeObject(faceSwapData);
			File.WriteAllText(FaceSwap.faceSwapJSONPath, updatedJson);
			Console.WriteLine("FaceSwapData.json başarıyla güncellendi.");
		}

		// Token: 0x06000B3C RID: 2876 RVA: 0x000422C0 File Offset: 0x000404C0
		public static string GetCurrentEventFolderPath()
		{
			return Settings.CurrentEventFolder;
		}

		// Token: 0x04000A66 RID: 2662
		internal static Dictionary<string, string> SettingsPairs = new Dictionary<string, string>();

		// Token: 0x04000A67 RID: 2663
		private static bool SettingsReadIsWorking = false;

		// Token: 0x04000A68 RID: 2664
		private static bool SettingsWriteIsWorking = false;

		// Token: 0x04000A69 RID: 2665
		public static Dictionary<string, string> DefaultValues = new Dictionary<string, string>
		{
			{
				"musiccheck",
				"false"
			},
			{
				"forcewifi",
				"true"
			},
			{
				"greenbox",
				"false"
			},
			{
				"aibackground",
				"false"
			},
			{
				"slowmotionenabled",
				"false"
			},
			{
				"lang_photo",
				"Photo"
			},
			{
				"lang_video",
				"Video"
			},
			{
				"lang_gif",
				"Gif"
			},
			{
				"lang_cancel",
				"Cancel"
			},
			{
				"lang_done",
				"Done"
			},
			{
				"lang_gallery",
				"Gallery"
			},
			{
				"lang_import",
				"Import"
			},
			{
				"lang_remainingseconds",
				"Approximately Remaining Seconds"
			},
			{
				"lang_selectfacebackground",
				"Select Background"
			},
			{
				"lang_selectaieffect",
				"Select AI Effect"
			},
			{
				"lang_print",
				"Print"
			},
			{
				"lang_email",
				"E-mail"
			},
			{
				"lang_close",
				"Close"
			},
			{
				"lang_qrpreparing",
				"Preparing Your \n Media To Share..."
			},
			{
				"lang_qrshow",
				"Scan The QR Code to Download Your Media"
			},
			{
				"lang_printnow",
				"Print"
			},
			{
				"lang_mailsend",
				"Send"
			},
			{
				"lang_resultimgtitle",
				"AI IMAGES"
			},
			{
				"lang_originalimgtitle",
				"ORIGINAL"
			},
			{
				"lang_aiimgbutton",
				"AI Prompt"
			},
			{
				"lang_faceswapbutton",
				"Face Swap"
			},
			{
				"lang_wordcloudbutton",
				"Word Portrait"
			},
			{
				"lang_aimotionbutton",
				"AI Motion"
			},
			{
				"lang_aieffectbutton",
				"AI Effect"
			},
			{
				"lang_selectaipromp",
				"Select AI Prompt"
			},
			{
				"lang_aibeautifierbutton",
				"AI Beautifier"
			},
			{
				"lang_smsbutton",
				"SMS"
			},
			{
				"lang_saveasbutton",
				"Save As"
			},
			{
				"lang_delete",
				"Delete"
			},
			{
				"lang_paymentmaintitle",
				"Pick Your Fun!"
			},
			{
				"lang_paymentmaintext",
				"Choose how you want your photos - and how many!"
			},
			{
				"lang_paymentmainpaybutton",
				"Let's Go-Pay "
			},
			{
				"lang_paymentmainbackbutton",
				"GO BACK"
			},
			{
				"lang_paymentmainprinttitle",
				"Print"
			},
			{
				"lang_paymentmainprinttext",
				"Take it home: 1 photo print + digital download."
			},
			{
				"lang_paymentmaindigitaltitle",
				"Digital Download"
			},
			{
				"lang_paymentmaindigitaltext",
				"Go digital: Instant download (no print)."
			},
			{
				"lang_paymentloadingtext",
				"Complete your payment to keep the fun going."
			},
			{
				"lang_paymentloadingtitle",
				"Almost There!"
			},
			{
				"lang_paymentloadingbackbutton",
				"GO BACK"
			},
			{
				"lang_paymentloadingscanqrtext",
				"Scan the QR code with your phone to pay."
			},
			{
				"lang_paymentconfirmationtitle",
				"You're All Set!"
			},
			{
				"lang_paymentconfirmationtext",
				"You've unlocked: "
			},
			{
				"lang_paymentconfirmationbackbutton",
				"CANCEL SESSION"
			},
			{
				"lang_paymentcorfirmationprintbutton",
				"Start Print "
			},
			{
				"lang_paymentconfirmationdigitalbutton",
				"Start Digital Download"
			},
			{
				"printeachphotolimit",
				"0"
			},
			{
				"mainbackgroundvideocheck",
				"false"
			},
			{
				"waitvideocheck",
				"false"
			},
			{
				"usewindowssettingsprint",
				"false"
			},
			{
				"usedefaultprintsettings",
				"false"
			},
			{
				"printLimit",
				"-1"
			},
			{
				"directprint",
				"false"
			},
			{
				"processwaitvideocheck",
				"false"
			},
			{
				"resultvideocheck",
				"false"
			},
			{
				"countdownseconds",
				"5"
			},
			{
				"slowmotionstart",
				"3"
			},
			{
				"slowmotionend",
				"4"
			},
			{
				"slowmotionrate",
				"4"
			},
			{
				"videotime",
				"6"
			},
			{
				"greenboxthrash",
				"100"
			},
			{
				"colorgreenscreen",
				"#00FF00"
			},
			{
				"rotation",
				"0"
			},
			{
				"colorbackgroundtheme",
				"#000000"
			},
			{
				"colorbackgroundmaintheme",
				"#000000"
			},
			{
				"realtime",
				"false"
			},
			{
				"mode",
				Mode.MultifunctionalPhotoBooth.GetDisplayName()
			},
			{
				"cameratype",
				MainSettingsPage.CameraType.Webcam.ToString()
			},
			{
				"wordcloudbackcolor",
				"#01FFFFF2"
			},
			{
				"wordcloud",
				"Inspiration, Creativity, Passion, Dream, Vision, Harmony, Strength, Wisdom, Beauty, Courage, Grace, Love, Hope, Faith, Joy, Peace, Unity, Soul, Spirit, Serenity, Freedom, Light, Integrity, Kindness, Compassion"
			},
			{
				"wordcloudfont",
				"Anton-Regular"
			},
			{
				"wordcloudforegroundcolor",
				"#FD034751"
			},
			{
				"aipromptresolution4k",
				"false"
			},
			{
				"selectedfontcolor",
				"#FF000000"
			},
			{
				"selectedexampletext",
				"Text"
			},
			{
				"selectedfontsize",
				"100"
			},
			{
				"selectedfontplaceholdertext",
				"Name and Surname"
			},
			{
				"aipromptoptionactive",
				"true"
			},
			{
				"faceswapactive",
				"true"
			},
			{
				"wordportraitoptionactive",
				"false"
			},
			{
				"aimotionoptionactive",
				"true"
			},
			{
				"aieffectoptionactive",
				"true"
			},
			{
				"aimotionpromp",
				"fashion model posing dynamically with a slight smile, camera dolly zoom effect"
			},
			{
				"gifframerate",
				"2"
			},
			{
				"aipromptextbox",
				"false"
			},
			{
				"photo",
				"true"
			},
			{
				"video",
				"false"
			},
			{
				"upload",
				"false"
			},
			{
				"gif",
				"false"
			},
			{
				"gallery",
				"true"
			},
			{
				"saveas",
				"false"
			},
			{
				"printenable",
				"false"
			},
			{
				"mailenable",
				"false"
			},
			{
				"smsenable",
				"false"
			},
			{
				"qrenable",
				"true"
			},
			{
				"paymentactive",
				"false"
			},
			{
				"printprice",
				"2"
			},
			{
				"captureprice",
				"1"
			}
		};

		// Token: 0x020002C7 RID: 711
		public static class Keys
		{
			// Token: 0x04001245 RID: 4677
			public const string Photo = "photo";

			// Token: 0x04001246 RID: 4678
			public const string Video = "video";

			// Token: 0x04001247 RID: 4679
			public const string SaveAs = "saveas";

			// Token: 0x04001248 RID: 4680
			public const string DesktopEditing = "upload";

			// Token: 0x04001249 RID: 4681
			public const string FullscreenPreview = "fullscreenpreview";

			// Token: 0x0400124A RID: 4682
			public const string BackgroundPreviewVideo = "backgroundpreviewvideo";

			// Token: 0x0400124B RID: 4683
			public const string Gif = "gif";

			// Token: 0x0400124C RID: 4684
			public const string Gallery = "gallery";

			// Token: 0x0400124D RID: 4685
			public const string MusicCheck = "musiccheck";

			// Token: 0x0400124E RID: 4686
			public const string ForceWifi = "forcewifi";

			// Token: 0x0400124F RID: 4687
			public const string GreenBox = "greenbox";

			// Token: 0x04001250 RID: 4688
			public const string AIBackground = "aibackground";

			// Token: 0x04001251 RID: 4689
			public const string SlowMotionEnabled = "slowmotionenabled";

			// Token: 0x04001252 RID: 4690
			public const string LangPhoto = "lang_photo";

			// Token: 0x04001253 RID: 4691
			public const string LangVideo = "lang_video";

			// Token: 0x04001254 RID: 4692
			public const string LangGif = "lang_gif";

			// Token: 0x04001255 RID: 4693
			public const string LangCancel = "lang_cancel";

			// Token: 0x04001256 RID: 4694
			public const string LangDone = "lang_done";

			// Token: 0x04001257 RID: 4695
			public const string LangGallery = "lang_gallery";

			// Token: 0x04001258 RID: 4696
			public const string LangImport = "lang_import";

			// Token: 0x04001259 RID: 4697
			public const string LangRemainingSeconds = "lang_remainingseconds";

			// Token: 0x0400125A RID: 4698
			public const string LangSelectFaceBackground = "lang_selectfacebackground";

			// Token: 0x0400125B RID: 4699
			public const string LangPrint = "lang_print";

			// Token: 0x0400125C RID: 4700
			public const string LangEmail = "lang_email";

			// Token: 0x0400125D RID: 4701
			public const string LangClose = "lang_close";

			// Token: 0x0400125E RID: 4702
			public const string LangQrPreparing = "lang_qrpreparing";

			// Token: 0x0400125F RID: 4703
			public const string LangQrShow = "lang_qrshow";

			// Token: 0x04001260 RID: 4704
			public const string LangPrintNow = "lang_printnow";

			// Token: 0x04001261 RID: 4705
			public const string LangMailSend = "lang_mailsend";

			// Token: 0x04001262 RID: 4706
			public const string LangResultImgTitle = "lang_resultimgtitle";

			// Token: 0x04001263 RID: 4707
			public const string LangOriginalImgTitle = "lang_originalimgtitle";

			// Token: 0x04001264 RID: 4708
			public const string LangAIImgButtonTitle = "lang_aiimgbutton";

			// Token: 0x04001265 RID: 4709
			public const string LangFaceSwapButtonTitle = "lang_faceswapbutton";

			// Token: 0x04001266 RID: 4710
			public const string LangWordCloudButtonTitle = "lang_wordcloudbutton";

			// Token: 0x04001267 RID: 4711
			public const string LangAIMotionButtonTitle = "lang_aimotionbutton";

			// Token: 0x04001268 RID: 4712
			public const string LangAIEffectButtonTitle = "lang_aieffectbutton";

			// Token: 0x04001269 RID: 4713
			public const string LangSelectAIPromp = "lang_selectaipromp";

			// Token: 0x0400126A RID: 4714
			public const string LangAIBeautifierButtonTitle = "lang_aibeautifierbutton";

			// Token: 0x0400126B RID: 4715
			public const string LangSelectAIEffect = "lang_selectaieffect";

			// Token: 0x0400126C RID: 4716
			public const string LangSMSButtonTitle = "lang_smsbutton";

			// Token: 0x0400126D RID: 4717
			public const string LangSaveAsButtonTitle = "lang_saveasbutton";

			// Token: 0x0400126E RID: 4718
			public const string LangDeleteButtonTitle = "lang_delete";

			// Token: 0x0400126F RID: 4719
			public const string LangPaymentMainPageTitle = "lang_paymentmaintitle";

			// Token: 0x04001270 RID: 4720
			public const string LangPaymentMainPageText = "lang_paymentmaintext";

			// Token: 0x04001271 RID: 4721
			public const string LangPaymentMainPagePrintTitle = "lang_paymentmainprinttitle";

			// Token: 0x04001272 RID: 4722
			public const string LangPaymentMainPagePrintText = "lang_paymentmainprinttext";

			// Token: 0x04001273 RID: 4723
			public const string LangPaymentMainPageDigitalTitle = "lang_paymentmaindigitaltitle";

			// Token: 0x04001274 RID: 4724
			public const string LangPaymentMainPageDigitalText = "lang_paymentmaindigitaltext";

			// Token: 0x04001275 RID: 4725
			public const string LangPaymentMainPagePayButton = "lang_paymentmainpaybutton";

			// Token: 0x04001276 RID: 4726
			public const string LangPaymentMainPageBackButton = "lang_paymentmainbackbutton";

			// Token: 0x04001277 RID: 4727
			public const string LangPaymentLoadingPageTitle = "lang_paymentloadingtitle";

			// Token: 0x04001278 RID: 4728
			public const string LangPaymentLoadingPageText = "lang_paymentloadingtext";

			// Token: 0x04001279 RID: 4729
			public const string LangPaymentLoadingPageBackButton = "lang_paymentloadingbackbutton";

			// Token: 0x0400127A RID: 4730
			public const string LangPaymentLoadingPageScanQRText = "lang_paymentloadingscanqrtext";

			// Token: 0x0400127B RID: 4731
			public const string LangPaymentConfirmationPageTitle = "lang_paymentconfirmationtitle";

			// Token: 0x0400127C RID: 4732
			public const string LangPaymentConfirmationPageText = "lang_paymentconfirmationtext";

			// Token: 0x0400127D RID: 4733
			public const string LangPaymentConfirmationPageBackButton = "lang_paymentconfirmationbackbutton";

			// Token: 0x0400127E RID: 4734
			public const string LangPaymentConfirmationPagePrintButton = "lang_paymentcorfirmationprintbutton";

			// Token: 0x0400127F RID: 4735
			public const string LangPaymentConfirmationPageDigitalButton = "lang_paymentconfirmationdigitalbutton";

			// Token: 0x04001280 RID: 4736
			public const string PrintEnable = "printenable";

			// Token: 0x04001281 RID: 4737
			public const string MailEnable = "mailenable";

			// Token: 0x04001282 RID: 4738
			public const string SmsEnable = "smsenable";

			// Token: 0x04001283 RID: 4739
			public const string QrEnable = "qrenable";

			// Token: 0x04001284 RID: 4740
			public const string MainBackgroundVideoCheck = "mainbackgroundvideocheck";

			// Token: 0x04001285 RID: 4741
			public const string WaitVideoCheck = "waitvideocheck";

			// Token: 0x04001286 RID: 4742
			public const string ProcessWaitVideoCheck = "processwaitvideocheck";

			// Token: 0x04001287 RID: 4743
			public const string PrintLimit = "printLimit";

			// Token: 0x04001288 RID: 4744
			public const string DirectPrint = "directprint";

			// Token: 0x04001289 RID: 4745
			public const string UseWindowsSettingsPrint = "usewindowssettingsprint";

			// Token: 0x0400128A RID: 4746
			public const string UseDefaultPrintSettings = "usedefaultprintsettings";

			// Token: 0x0400128B RID: 4747
			public const string ForceLandscapePrint = "forcelandscapeprint";

			// Token: 0x0400128C RID: 4748
			public const string ForcePortraitPrint = "forceportraitprint";

			// Token: 0x0400128D RID: 4749
			public const string ResultVideoCheck = "resultvideocheck";

			// Token: 0x0400128E RID: 4750
			public const string CountdownSeconds = "countdownseconds";

			// Token: 0x0400128F RID: 4751
			public const string SlowMotionStart = "slowmotionstart";

			// Token: 0x04001290 RID: 4752
			public const string SlowMotionEnd = "slowmotionend";

			// Token: 0x04001291 RID: 4753
			public const string SlowMotionRate = "slowmotionrate";

			// Token: 0x04001292 RID: 4754
			public const string VideoTime = "videotime";

			// Token: 0x04001293 RID: 4755
			public const string AIMotionPromp = "aimotionpromp";

			// Token: 0x04001294 RID: 4756
			public const string Greenboxthrash = "greenboxthrash";

			// Token: 0x04001295 RID: 4757
			public const string ColorGreenScreen = "colorgreenscreen";

			// Token: 0x04001296 RID: 4758
			public const string Rotation = "rotation";

			// Token: 0x04001297 RID: 4759
			public const string MainBackgroundVideo = "mainbackgroundvideo";

			// Token: 0x04001298 RID: 4760
			public const string WaitVideo = "waitvideo";

			// Token: 0x04001299 RID: 4761
			public const string ProcessWaitVideo = "processwaitvideo";

			// Token: 0x0400129A RID: 4762
			public const string ColorHomePageBackground = "colorbackgroundtheme";

			// Token: 0x0400129B RID: 4763
			public const string ColorOtherBackground = "colorbackgroundmaintheme";

			// Token: 0x0400129C RID: 4764
			public const string BackgroundEmailDelivery = "realtime";

			// Token: 0x0400129D RID: 4765
			public const string MirrorVideo = "mirrorvideo";

			// Token: 0x0400129E RID: 4766
			public const string ResultAnimationVideo = "resultvideo";

			// Token: 0x0400129F RID: 4767
			public const string ModeMain = "mode";

			// Token: 0x040012A0 RID: 4768
			public const string SelectedAIMode = "selectedaimode";

			// Token: 0x040012A1 RID: 4769
			public const string OutputFolder = "outputfolder";

			// Token: 0x040012A2 RID: 4770
			public const string WordCloudStrings = "wordcloud";

			// Token: 0x040012A3 RID: 4771
			public const string WordCloudFontName = "wordcloudfont";

			// Token: 0x040012A4 RID: 4772
			public const string WordCloudBackColor = "wordcloudbackcolor";

			// Token: 0x040012A5 RID: 4773
			public const string WordCloudForeground = "wordcloudforegroundcolor";

			// Token: 0x040012A6 RID: 4774
			public const string MusicPath = "musicpath";

			// Token: 0x040012A7 RID: 4775
			public const string Gifframerate = "gifframerate";

			// Token: 0x040012A8 RID: 4776
			public const string OrginalOption = "orginaloption";

			// Token: 0x040012A9 RID: 4777
			public const string LPPDActive = "lppdactive";

			// Token: 0x040012AA RID: 4778
			public const string LPPDText = "lppdtext";

			// Token: 0x040012AB RID: 4779
			public const string LPPDTitle = "lppdtitle";

			// Token: 0x040012AC RID: 4780
			public const string SelectedFontName = "selectedfontname";

			// Token: 0x040012AD RID: 4781
			public const string SelectedFontColor = "selectedfontcolor";

			// Token: 0x040012AE RID: 4782
			public const string SelectedFontSize = "selectedfontsize";

			// Token: 0x040012AF RID: 4783
			public const string SelectedExampleText = "selectedexampletext";

			// Token: 0x040012B0 RID: 4784
			public const string SelectedFontPlaceholderText = "selectedfontplaceholdertext";

			// Token: 0x040012B1 RID: 4785
			public const string CameraPreviewBrightness = "previewbrightness";

			// Token: 0x040012B2 RID: 4786
			public const string FaceSwapOptionActive = "faceswapactive";

			// Token: 0x040012B3 RID: 4787
			public const string AiPromptOptionActive = "aipromptoptionactive";

			// Token: 0x040012B4 RID: 4788
			public const string WordPortraitOptionActive = "wordportraitoptionactive";

			// Token: 0x040012B5 RID: 4789
			public const string AiMotionOptionActive = "aimotionoptionactive";

			// Token: 0x040012B6 RID: 4790
			public const string AiEffectOptionActive = "aieffectoptionactive";

			// Token: 0x040012B7 RID: 4791
			public const string AiBeautifierOptionActive = "aieffectoptionactive";

			// Token: 0x040012B8 RID: 4792
			public const string AiPromptResolution4K = "aipromptresolution4k";

			// Token: 0x040012B9 RID: 4793
			public const string AiPromptTextBox = "aipromptextbox";

			// Token: 0x040012BA RID: 4794
			public const string CameraType = "cameratype";

			// Token: 0x040012BB RID: 4795
			public const string CameraList = "cameralist";

			// Token: 0x040012BC RID: 4796
			public const string RunOnStartup = "runonstartup";

			// Token: 0x040012BD RID: 4797
			public const string Returnpassword = "returnpassword";

			// Token: 0x040012BE RID: 4798
			public const string PaymentTime = "paymenttime";

			// Token: 0x040012BF RID: 4799
			public const string PaymentScreen = "paymentscreen";

			// Token: 0x040012C0 RID: 4800
			public const string PaymentsActive = "paymentactive";

			// Token: 0x040012C1 RID: 4801
			public const string PaymentsPrintPrice = "printprice";

			// Token: 0x040012C2 RID: 4802
			public const string PaymentsCapturePrice = "captureprice";

			// Token: 0x040012C3 RID: 4803
			public const string PaymentsStripeSecret = "stripesecret";

			// Token: 0x040012C4 RID: 4804
			public const string PaymentsStripeAccID = "stripeaccountid";

			// Token: 0x040012C5 RID: 4805
			public const string AutoPrint = "autoprint";

			// Token: 0x040012C6 RID: 4806
			public const string PrintEachPhotoLimit = "printeachphotolimit";
		}

		// Token: 0x020002C8 RID: 712
		public static class ElementsVisibilities
		{
			// Token: 0x17000280 RID: 640
			// (get) Token: 0x060011AC RID: 4524 RVA: 0x000679E8 File Offset: 0x00065BE8
			public static bool AIFaceSwapButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("faceswapactive").Value && MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000281 RID: 641
			// (get) Token: 0x060011AD RID: 4525 RVA: 0x00067A18 File Offset: 0x00065C18
			public static bool AIImageButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aipromptoptionactive").Value && MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000282 RID: 642
			// (get) Token: 0x060011AE RID: 4526 RVA: 0x00067A48 File Offset: 0x00065C48
			public static bool WordCloudButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("wordportraitoptionactive").Value && MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000283 RID: 643
			// (get) Token: 0x060011AF RID: 4527 RVA: 0x00067A78 File Offset: 0x00065C78
			public static bool AIMotionButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aimotionoptionactive").Value && MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000284 RID: 644
			// (get) Token: 0x060011B0 RID: 4528 RVA: 0x00067AA8 File Offset: 0x00065CA8
			public static bool AIEffectButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aieffectoptionactive").Value && MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000285 RID: 645
			// (get) Token: 0x060011B1 RID: 4529 RVA: 0x00067AD8 File Offset: 0x00065CD8
			public static bool AIBeatifuerButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aieffectoptionactive").Value && MainSettingsPage.SelectedMode != Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000286 RID: 646
			// (get) Token: 0x060011B2 RID: 4530 RVA: 0x00067B06 File Offset: 0x00065D06
			public static bool AiOptionsVisibility
			{
				get
				{
					return MainSettingsPage.SelectedMode == Mode.MultifunctionalPhotoBooth;
				}
			}

			// Token: 0x17000287 RID: 647
			// (get) Token: 0x060011B3 RID: 4531 RVA: 0x00067B10 File Offset: 0x00065D10
			public static bool ModeShootingOptionsVisibility
			{
				get
				{
					return MainSettingsPage.SelectedMode == Mode.MultifunctionalPhotoBooth || MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x17000288 RID: 648
			// (get) Token: 0x060011B4 RID: 4532 RVA: 0x00067B23 File Offset: 0x00065D23
			public static bool BackgroundDeliveryOptionVisibility
			{
				get
				{
					return true;
				}
			}

			// Token: 0x17000289 RID: 649
			// (get) Token: 0x060011B5 RID: 4533 RVA: 0x00067B28 File Offset: 0x00065D28
			public static bool AiPromptResolutionVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aipromptoptionactive").Value && MainSettingsPage.SelectedMode == Mode.MultifunctionalPhotoBooth;
				}
			}

			// Token: 0x1700028A RID: 650
			// (get) Token: 0x060011B6 RID: 4534 RVA: 0x00067B54 File Offset: 0x00065D54
			public static bool AiPromptTextBoxVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aipromptoptionactive").Value;
				}
			}

			// Token: 0x1700028B RID: 651
			// (get) Token: 0x060011B7 RID: 4535 RVA: 0x00067B73 File Offset: 0x00065D73
			public static bool PhotoShootingOptionVisibility
			{
				get
				{
					return Settings.GetValueEnum<MainSettingsPage.CameraType>("cameratype") != MainSettingsPage.CameraType.NoCamera;
				}
			}

			// Token: 0x1700028C RID: 652
			// (get) Token: 0x060011B8 RID: 4536 RVA: 0x00067B85 File Offset: 0x00065D85
			public static bool VideoShootingOptionVisibility
			{
				get
				{
					return Settings.GetValueEnum<MainSettingsPage.CameraType>("cameratype") != MainSettingsPage.CameraType.NoCamera;
				}
			}

			// Token: 0x1700028D RID: 653
			// (get) Token: 0x060011B9 RID: 4537 RVA: 0x00067B98 File Offset: 0x00065D98
			public static bool GifShootingOptionVisibility
			{
				get
				{
					return Settings.GetValueBoolean("photo").Value && Settings.GetValueEnum<MainSettingsPage.CameraType>("cameratype") != MainSettingsPage.CameraType.NoCamera;
				}
			}

			// Token: 0x1700028E RID: 654
			// (get) Token: 0x060011BA RID: 4538 RVA: 0x00067BCB File Offset: 0x00065DCB
			public static bool DesktopEditingShootingOptionVisibility
			{
				get
				{
					return true;
				}
			}

			// Token: 0x1700028F RID: 655
			// (get) Token: 0x060011BB RID: 4539 RVA: 0x00067BCE File Offset: 0x00065DCE
			public static bool SectionAIBackgroundSettingsVisibility
			{
				get
				{
					return MainSettingsPage.SelectedMode == Mode.MultifunctionalPhotoBooth;
				}
			}

			// Token: 0x17000290 RID: 656
			// (get) Token: 0x060011BC RID: 4540 RVA: 0x00067BD8 File Offset: 0x00065DD8
			public static bool SectionChromeKeyBackgroundSettingsVisibility
			{
				get
				{
					return MainSettingsPage.SelectedMode == Mode.MultifunctionalPhotoBooth;
				}
			}

			// Token: 0x17000291 RID: 657
			// (get) Token: 0x060011BD RID: 4541 RVA: 0x00067BE4 File Offset: 0x00065DE4
			public static bool GifFrameRateFrameVisibility
			{
				get
				{
					return Settings.GetValueBoolean("gif").Value;
				}
			}

			// Token: 0x17000292 RID: 658
			// (get) Token: 0x060011BE RID: 4542 RVA: 0x00067C04 File Offset: 0x00065E04
			public static bool GifRepeatCountFrameVisibility
			{
				get
				{
					return Settings.GetValueBoolean("gif").Value;
				}
			}

			// Token: 0x17000293 RID: 659
			// (get) Token: 0x060011BF RID: 4543 RVA: 0x00067C24 File Offset: 0x00065E24
			public static bool VideoLengthFrameVisibility
			{
				get
				{
					return Settings.GetValueBoolean("video").Value;
				}
			}

			// Token: 0x17000294 RID: 660
			// (get) Token: 0x060011C0 RID: 4544 RVA: 0x00067C44 File Offset: 0x00065E44
			public static bool ImageTemplateMenuButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("photo").Value || Settings.GetValueBoolean("upload").Value;
				}
			}

			// Token: 0x17000295 RID: 661
			// (get) Token: 0x060011C1 RID: 4545 RVA: 0x00067C7C File Offset: 0x00065E7C
			public static bool VideoTemplateMenuButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("video").Value || Settings.GetValueBoolean("gif").Value || Settings.GetValueBoolean("upload").Value || Settings.GetValueBoolean("aimotionoptionactive").Value;
				}
			}

			// Token: 0x17000296 RID: 662
			// (get) Token: 0x060011C2 RID: 4546 RVA: 0x00067CDC File Offset: 0x00065EDC
			public static bool AiPromptGalleryMenuButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aipromptoptionactive").Value;
				}
			}

			// Token: 0x17000297 RID: 663
			// (get) Token: 0x060011C3 RID: 4547 RVA: 0x00067CFC File Offset: 0x00065EFC
			public static bool FaceSwapTargetGalleryMenuButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("faceswapactive").Value;
				}
			}

			// Token: 0x17000298 RID: 664
			// (get) Token: 0x060011C4 RID: 4548 RVA: 0x00067D1C File Offset: 0x00065F1C
			public static bool WordPortraitMenuButtonVisibility
			{
				get
				{
					return Settings.GetValueBoolean("wordportraitoptionactive").Value;
				}
			}

			// Token: 0x17000299 RID: 665
			// (get) Token: 0x060011C5 RID: 4549 RVA: 0x00067D3C File Offset: 0x00065F3C
			public static bool AiMotionActiveFrameVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aipromptoptionactive").Value || Settings.GetValueBoolean("faceswapactive").Value || Settings.GetValueBoolean("aieffectoptionactive").Value || Settings.GetValueBoolean("aieffectoptionactive").Value;
				}
			}

			// Token: 0x1700029A RID: 666
			// (get) Token: 0x060011C6 RID: 4550 RVA: 0x00067D9C File Offset: 0x00065F9C
			public static bool CameraPreviewBrightnessFrameVisibility
			{
				get
				{
					return Settings.GetValueBoolean("backgroundpreviewvideo").Value;
				}
			}

			// Token: 0x1700029B RID: 667
			// (get) Token: 0x060011C7 RID: 4551 RVA: 0x00067DBC File Offset: 0x00065FBC
			public static bool AiEffectActiveFrameVisibility
			{
				get
				{
					return Settings.GetValueBoolean("aieffectoptionactive").Value;
				}
			}

			// Token: 0x1700029C RID: 668
			// (get) Token: 0x060011C8 RID: 4552 RVA: 0x00067DDB File Offset: 0x00065FDB
			public static bool AiSelectionFrameVisibility
			{
				get
				{
					return MainSettingsPage.SelectedMode == Mode.MonofunctionalPhotoBooth;
				}
			}

			// Token: 0x1700029D RID: 669
			// (get) Token: 0x060011C9 RID: 4553 RVA: 0x00067DE8 File Offset: 0x00065FE8
			[TupleElementNames(new string[]
			{
				"isActive",
				"message"
			})]
			public static ValueTuple<bool, string> StartTheAppButtonActive
			{
				[return: TupleElementNames(new string[]
				{
					"isActive",
					"message"
				})]
				get
				{
					if (!Settings.GetValueBoolean("photo").Value && !Settings.GetValueBoolean("video").Value && !Settings.GetValueBoolean("upload").Value)
					{
						return new ValueTuple<bool, string>(false, "At least one option (Photo, Video, or Desktop Editing) must be selected.");
					}
					MainSettingsPage.CameraType cameraType = MainSettingsPage.CameraType.DSLR;
					try
					{
						cameraType = Settings.GetValueEnum<MainSettingsPage.CameraType>("cameratype");
					}
					catch (Exception ex)
					{
						return new ValueTuple<bool, string>(false, "Camera type setting is invalid or missing.");
					}
					string cameraListValue = Settings.GetValueString("cameralist");
					switch (cameraType)
					{
					case MainSettingsPage.CameraType.DSLR:
						if (CameraControlClass.DeviceManager == null)
						{
							return new ValueTuple<bool, string>(false, "No DSLR camera selected.");
						}
						if (!string.IsNullOrEmpty(cameraListValue))
						{
							return new ValueTuple<bool, string>(true, "DSLR camera is ready.");
						}
						return new ValueTuple<bool, string>(false, "No DSLR camera selected.");
					case MainSettingsPage.CameraType.GoPro:
						if (string.IsNullOrEmpty(cameraListValue))
						{
							return new ValueTuple<bool, string>(false, "No GoPro camera selected.");
						}
						if (!GoProCameraControlClass.IsCameraReady)
						{
							return new ValueTuple<bool, string>(false, "GoPro is not ready. Check WiFi and Bluetooth connection.");
						}
						return new ValueTuple<bool, string>(true, "GoPro is ready to use.");
					case MainSettingsPage.CameraType.Webcam:
						if (WebcamControlClass.openCvCameraHelper == null)
						{
							return new ValueTuple<bool, string>(false, "No webcam selected.");
						}
						if (!string.IsNullOrEmpty(cameraListValue))
						{
							return new ValueTuple<bool, string>(true, "Webcam is ready.");
						}
						return new ValueTuple<bool, string>(false, "No webcam selected.");
					case MainSettingsPage.CameraType.NoCamera:
						return new ValueTuple<bool, string>(true, "No camera mode selected, application can start.");
					default:
						return new ValueTuple<bool, string>(false, "Unknown camera type.");
					}
					ValueTuple<bool, string> result;
					return result;
				}
			}
		}
	}
}
