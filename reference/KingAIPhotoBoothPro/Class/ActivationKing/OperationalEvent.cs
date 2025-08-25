using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using KingAIPhotoBoothPro.Modals;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C7 RID: 199
	public class OperationalEvent
	{
		// Token: 0x06000AE5 RID: 2789 RVA: 0x0003FDC0 File Offset: 0x0003DFC0
		public int GetMediaNumber()
		{
			if (!File.Exists(this.EventMediaJsonPath))
			{
				return 0;
			}
			string json = File.ReadAllText(this.EventMediaJsonPath);
			EventMediaClass thisEventJson = JsonConvert.DeserializeObject<EventMediaClass>(json);
			if (thisEventJson == null)
			{
				Debug.Log(base.GetType().Name, string.Format("File is corrupted\n\nFile name : {0}\nFile content : {1}", this.EventMediaJsonPath, json), "GetMediaNumber", 33);
				File.Delete(this.EventMediaJsonPath);
				return -1;
			}
			return thisEventJson.mediaList.Count;
		}

		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06000AE6 RID: 2790 RVA: 0x0003FE32 File Offset: 0x0003E032
		public string DirectoryPath
		{
			get
			{
				if (this.directoryEvent == null)
				{
					this.directoryEvent = Path.Combine(SessionData.UserDataFolderPath, this.IndexID);
				}
				if (!Directory.Exists(this.directoryEvent))
				{
					Directory.CreateDirectory(this.directoryEvent);
				}
				return this.directoryEvent;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06000AE7 RID: 2791 RVA: 0x0003FE71 File Offset: 0x0003E071
		public string MailJsonPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "mailData.json");
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06000AE8 RID: 2792 RVA: 0x0003FE83 File Offset: 0x0003E083
		public string PrintJsonPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "printData.json");
			}
		}

		// Token: 0x17000187 RID: 391
		// (get) Token: 0x06000AE9 RID: 2793 RVA: 0x0003FE95 File Offset: 0x0003E095
		public string PrintRequestJsonPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "printRequest.json");
			}
		}

		// Token: 0x17000188 RID: 392
		// (get) Token: 0x06000AEA RID: 2794 RVA: 0x0003FEA7 File Offset: 0x0003E0A7
		public string EventXMLPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "event.xml");
			}
		}

		// Token: 0x17000189 RID: 393
		// (get) Token: 0x06000AEB RID: 2795 RVA: 0x0003FEB9 File Offset: 0x0003E0B9
		public string EventMediaJsonPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "eventMedias.json");
			}
		}

		// Token: 0x1700018A RID: 394
		// (get) Token: 0x06000AEC RID: 2796 RVA: 0x0003FECB File Offset: 0x0003E0CB
		public string SettingsXMLPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "settings.xml");
			}
		}

		// Token: 0x1700018B RID: 395
		// (get) Token: 0x06000AED RID: 2797 RVA: 0x0003FEDD File Offset: 0x0003E0DD
		public string TemplateJsonPath
		{
			get
			{
				return Path.Combine(this.DirectoryPath, "template.json");
			}
		}

		// Token: 0x1700018C RID: 396
		// (get) Token: 0x06000AEE RID: 2798 RVA: 0x0003FEF0 File Offset: 0x0003E0F0
		public string EventHash
		{
			get
			{
				if (string.IsNullOrEmpty(this.eventHash))
				{
					List<EventApplication> events = new List<EventApplication>();
					if (File.Exists(SessionData.UserEventsJsonPath))
					{
						events = ExtensionMethod.ReadJson<List<EventApplication>>(SessionData.UserEventsJsonPath);
						EventApplication editEvent = (from x in events
						where x.Id == this.IndexID
						select x).FirstOrDefault<EventApplication>();
						if (editEvent != null)
						{
							this.eventHash = editEvent.EventHash;
						}
					}
				}
				return this.eventHash;
			}
		}

		// Token: 0x06000AEF RID: 2799 RVA: 0x0003FF54 File Offset: 0x0003E154
		public void SaveLocalXmlFile()
		{
			new Thread(delegate()
			{
				ExtensionMethod.CreateEventXml<OperationalEvent>(this, this.EventXMLPath, "Event", "Event Properties XML");
			}).Start();
		}

		// Token: 0x04000A44 RID: 2628
		public string IndexID;

		// Token: 0x04000A45 RID: 2629
		public string EventName;

		// Token: 0x04000A46 RID: 2630
		public string EventMotto;

		// Token: 0x04000A47 RID: 2631
		public string eventHash;

		// Token: 0x04000A48 RID: 2632
		public DateTime CreateTime;

		// Token: 0x04000A49 RID: 2633
		public DateTime LastUsedTime;

		// Token: 0x04000A4A RID: 2634
		private string directoryEvent;
	}
}
