using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Linq;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages.ActivationKing;
using KingAIPhotoBoothPro.Pages.ActivationKing.Prefab;
using KingAIPhotoBoothPro.Pages.Behaviors;
using MongoDB.Bson;

namespace KingAIPhotoBoothPro.Pages
{
	// Token: 0x02000039 RID: 57
	public class EventManagementPage : SettingsSubPage, IComponentConnector
	{
		// Token: 0x1400000D RID: 13
		// (add) Token: 0x0600038B RID: 907 RVA: 0x00013788 File Offset: 0x00011988
		// (remove) Token: 0x0600038C RID: 908 RVA: 0x000137BC File Offset: 0x000119BC
		public static event Action archiveEventAction;

		// Token: 0x1400000E RID: 14
		// (add) Token: 0x0600038D RID: 909 RVA: 0x000137F0 File Offset: 0x000119F0
		// (remove) Token: 0x0600038E RID: 910 RVA: 0x00013824 File Offset: 0x00011A24
		public static event EventManagementPage.EventChanged onEventChanged;

		// Token: 0x0600038F RID: 911 RVA: 0x00013857 File Offset: 0x00011A57
		public static void RefreshEvents()
		{
			EventManagementPage.Events = new List<OperationalEvent>();
			EventManagementPage.archiveEventAction = null;
			EventManagementPage.CurrentEvent = null;
			EventManagementPage.loadedEventCount = 0;
		}

		// Token: 0x06000390 RID: 912 RVA: 0x00013875 File Offset: 0x00011A75
		public EventManagementPage()
		{
			this.InitializeComponent();
			EventManagementPage.createEditPanel = new CreateEventPage();
			EventManagementPage.currentEventPanel = new CurrentEventPage();
		}

		// Token: 0x06000391 RID: 913 RVA: 0x000138A4 File Offset: 0x00011AA4
		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			EventManagementPage.<Page_Loaded>d__17 <Page_Loaded>d__;
			<Page_Loaded>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<Page_Loaded>d__.<>4__this = this;
			<Page_Loaded>d__.<>1__state = -1;
			<Page_Loaded>d__.<>t__builder.Start<EventManagementPage.<Page_Loaded>d__17>(ref <Page_Loaded>d__);
		}

		// Token: 0x06000392 RID: 914 RVA: 0x000138DC File Offset: 0x00011ADC
		private Task Start()
		{
			EventManagementPage.<Start>d__18 <Start>d__;
			<Start>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<Start>d__.<>4__this = this;
			<Start>d__.<>1__state = -1;
			<Start>d__.<>t__builder.Start<EventManagementPage.<Start>d__18>(ref <Start>d__);
			return <Start>d__.<>t__builder.Task;
		}

		// Token: 0x06000393 RID: 915 RVA: 0x0001391F File Offset: 0x00011B1F
		private void ScrollBehavior_ReachedBottom(object sender, RoutedEventArgs e)
		{
			this.AddOlderEvents();
		}

		// Token: 0x06000394 RID: 916 RVA: 0x00013927 File Offset: 0x00011B27
		private void LoadCurrentEventPanel()
		{
			this.EventFrame.Content = EventManagementPage.currentEventPanel;
		}

		// Token: 0x06000395 RID: 917 RVA: 0x0001393C File Offset: 0x00011B3C
		public T ReadXml<T>(T empty, string path)
		{
			XElement root;
			try
			{
				root = XElement.Load(path);
			}
			catch (Exception)
			{
				return default(T);
			}
			Type type = typeof(T);
			FieldInfo[] fields = type.GetFields();
			int i = 0;
			while (i < fields.Length)
			{
				FieldInfo prop = fields[i];
				if (prop.FieldType == typeof(Guid))
				{
					try
					{
						prop.SetValue(empty, Guid.Parse(root.Element(prop.Name).Value));
						goto IL_28D;
					}
					catch (Exception)
					{
						prop.SetValue(empty, Guid.NewGuid());
						goto IL_28D;
					}
					goto IL_A1;
				}
				goto IL_A1;
				IL_28D:
				i++;
				continue;
				IL_A1:
				if (prop.FieldType == typeof(bool))
				{
					try
					{
						prop.SetValue(empty, bool.Parse(root.Element(prop.Name).Value));
						goto IL_28D;
					}
					catch (Exception)
					{
						prop.SetValue(empty, false);
						goto IL_28D;
					}
				}
				if (prop.FieldType == typeof(int))
				{
					try
					{
						prop.SetValue(empty, int.Parse(root.Element(prop.Name).Value));
						goto IL_28D;
					}
					catch (Exception)
					{
						prop.SetValue(empty, 0);
						goto IL_28D;
					}
				}
				if (prop.FieldType == typeof(float))
				{
					try
					{
						prop.SetValue(empty, float.Parse(root.Element(prop.Name).Value));
						goto IL_28D;
					}
					catch (Exception)
					{
						prop.SetValue(empty, 0);
						goto IL_28D;
					}
				}
				if (prop.FieldType == typeof(DateTime))
				{
					try
					{
						prop.SetValue(empty, DateTime.ParseExact(root.Element(prop.Name).Value, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture));
						goto IL_28D;
					}
					catch (Exception ex)
					{
						prop.SetValue(empty, DateTime.Now);
						goto IL_28D;
					}
				}
				if (prop.FieldType == typeof(string))
				{
					try
					{
						prop.SetValue(empty, root.Element(prop.Name).Value);
					}
					catch (Exception)
					{
						prop.SetValue(empty, "");
					}
					goto IL_28D;
				}
				goto IL_28D;
			}
			return empty;
		}

		// Token: 0x06000396 RID: 918 RVA: 0x00013C44 File Offset: 0x00011E44
		public void AddEventsJson(OperationalEvent newOperationalEvent)
		{
			List<EventApplication> events = new List<EventApplication>();
			if (File.Exists(SessionData.UserEventsJsonPath))
			{
				events = ExtensionMethod.ReadJson<List<EventApplication>>(SessionData.UserEventsJsonPath);
				EventApplication editEvent = (from x in events
				where x.Id == newOperationalEvent.IndexID
				select x).FirstOrDefault<EventApplication>();
				if (editEvent != null)
				{
					editEvent.ChangeTime = new DateTime?(DateTime.Now);
					editEvent.EventMotto = newOperationalEvent.EventMotto;
					editEvent.EventName = newOperationalEvent.EventName;
					editEvent.EventHash = newOperationalEvent.EventHash;
					editEvent.IsCloudSync = false;
					ExtensionMethod.CreateWriteJson<List<EventApplication>>(events, SessionData.UserEventsJsonPath);
					return;
				}
			}
			events.Add(new EventApplication
			{
				ChangeTime = new DateTime?(DateTime.Now),
				EventMotto = newOperationalEvent.EventMotto,
				EventName = newOperationalEvent.EventName,
				GeneratedTime = DateTime.Now,
				Id = newOperationalEvent.IndexID,
				EventHash = newOperationalEvent.EventHash,
				IsCloudSync = false
			});
			ExtensionMethod.CreateWriteJson<List<EventApplication>>(events, SessionData.UserEventsJsonPath);
		}

		// Token: 0x06000397 RID: 919 RVA: 0x00013D68 File Offset: 0x00011F68
		public void ChangeEventsJson(OperationalEvent changedOperationalEvent)
		{
			List<EventApplication> events = new List<EventApplication>();
			if (File.Exists(SessionData.UserEventsJsonPath))
			{
				events = ExtensionMethod.ReadJson<List<EventApplication>>(SessionData.UserEventsJsonPath);
			}
			(from x in events
			where x.Id == changedOperationalEvent.IndexID
			select x).FirstOrDefault<EventApplication>().IsCloudSync = false;
		}

		// Token: 0x06000398 RID: 920 RVA: 0x00013DBC File Offset: 0x00011FBC
		public void CopyEvent(OperationalEvent refOperationalEvent)
		{
			EventManagementPage.<>c__DisplayClass24_0 CS$<>8__locals1 = new EventManagementPage.<>c__DisplayClass24_0();
			CS$<>8__locals1.<>4__this = this;
			this.EventLoadingGrid.Visibility = Visibility.Visible;
			CS$<>8__locals1.newEvent = null;
			string[] copyFiles = new string[]
			{
				"settings.xml",
				"template.json"
			};
			this.CreateEvent(refOperationalEvent.EventName + " - Copy", refOperationalEvent.EventMotto);
			for (int i = 0; i < copyFiles.Length; i++)
			{
				File.Copy(System.IO.Path.Combine(refOperationalEvent.DirectoryPath, copyFiles[i]), System.IO.Path.Combine(EventManagementPage.CurrentEvent.DirectoryPath, copyFiles[i]), true);
			}
			string templateImagesDirectory = System.IO.Path.Combine(EventManagementPage.CurrentEvent.DirectoryPath, TemplatePage.templateImagesFolderName);
			if (!Directory.Exists(templateImagesDirectory))
			{
				Directory.CreateDirectory(templateImagesDirectory);
			}
			TemplateJsonObject newTemplateJSONObject = new TemplateJsonObject();
			try
			{
				newTemplateJSONObject = ExtensionMethod.ReadJson<TemplateJsonObject>(EventManagementPage.GetCurrentEvent().TemplateJsonPath);
			}
			catch
			{
			}
			ExtensionMethod.CopyDirectory(FaceSwap.faceSwapFolderPathByEvent(refOperationalEvent), FaceSwap.faceSwapFolderPath);
			ExtensionMethod.CopyDirectory(SamplePromptsPage.samplePromptsFolderPathByEvent(refOperationalEvent), SamplePromptsPage.samplePromptsFolderPath);
			CS$<>8__locals1.newEvent = EventManagementPage.CurrentEvent;
			Application.Current.Dispatcher.Invoke(delegate()
			{
				CS$<>8__locals1.<>4__this.SetCurrentEvent(null);
				CS$<>8__locals1.<>4__this.LoadCreatePanel();
			});
			this.ReadAllEventDataAndlist();
			Task.Run(delegate()
			{
				EventManagementPage.<>c__DisplayClass24_0.<<CopyEvent>b__1>d <<CopyEvent>b__1>d;
				<<CopyEvent>b__1>d.<>t__builder = AsyncTaskMethodBuilder.Create();
				<<CopyEvent>b__1>d.<>4__this = CS$<>8__locals1;
				<<CopyEvent>b__1>d.<>1__state = -1;
				<<CopyEvent>b__1>d.<>t__builder.Start<EventManagementPage.<>c__DisplayClass24_0.<<CopyEvent>b__1>d>(ref <<CopyEvent>b__1>d);
				return <<CopyEvent>b__1>d.<>t__builder.Task;
			});
		}

		// Token: 0x06000399 RID: 921 RVA: 0x00013F04 File Offset: 0x00012104
		public void CreateEvent(string Name, string Motto)
		{
			if (string.IsNullOrEmpty(Name))
			{
				throw new Exception("Event name can not be empty!");
			}
			string id = ObjectId.GenerateNewId().ToString();
			OperationalEvent newOperationalEvent = new OperationalEvent
			{
				CreateTime = DateTime.Now,
				LastUsedTime = DateTime.Now,
				EventMotto = Motto,
				EventName = Name,
				IndexID = id
			};
			SettingsPage.isFirstEvent = true;
			newOperationalEvent.SaveLocalXmlFile();
			this.AddEventsJson(newOperationalEvent);
			this.ReadAllEventDataAndlist();
			this.SetCurrentEvent(newOperationalEvent);
			this.LoadCurrentEventPanel();
			ExtensionMethod.CreateWriteJson<EventMediaClass>(new EventMediaClass(), newOperationalEvent.EventMediaJsonPath);
		}

		// Token: 0x0600039A RID: 922 RVA: 0x00013FA0 File Offset: 0x000121A0
		internal void ArchiveEventClicked()
		{
			this.SetCurrentEvent(null);
			Action action = EventManagementPage.archiveEventAction;
			if (action != null)
			{
				action();
			}
			this.LoadCreatePanel();
			this.ReadAllEventDataAndlist();
		}

		// Token: 0x0600039B RID: 923 RVA: 0x00013FC6 File Offset: 0x000121C6
		private void LoadCreatePanel()
		{
			this.EventFrame.Content = EventManagementPage.createEditPanel;
			EventManagementPage.createEditPanel.CreateOpen();
		}

		// Token: 0x0600039C RID: 924 RVA: 0x00013FE2 File Offset: 0x000121E2
		private void LoadEditPanel()
		{
			this.EventFrame.Content = EventManagementPage.createEditPanel;
			EventManagementPage.createEditPanel.EditOpen(EventManagementPage.CurrentEvent);
		}

		// Token: 0x0600039D RID: 925 RVA: 0x00014004 File Offset: 0x00012204
		internal void EditEvent(OperationalEvent operationalEventToEdit)
		{
			OperationalEvent operationalEvent = EventManagementPage.Events.Find((OperationalEvent x) => x.IndexID == operationalEventToEdit.IndexID);
			operationalEvent.EventName = operationalEventToEdit.EventName;
			operationalEvent.EventMotto = operationalEventToEdit.EventMotto;
			operationalEvent.SaveLocalXmlFile();
			this.AddEventsJson(operationalEvent);
			this.LoadCurrentEventPanel();
		}

		// Token: 0x0600039E RID: 926 RVA: 0x0001406A File Offset: 0x0001226A
		public void EditEventClicked()
		{
			this.LoadEditPanel();
		}

		// Token: 0x0600039F RID: 927 RVA: 0x00014072 File Offset: 0x00012272
		internal void CancelEdit()
		{
			this.LoadCurrentEventPanel();
		}

		// Token: 0x060003A0 RID: 928 RVA: 0x0001407C File Offset: 0x0001227C
		internal void ReUseArchivedEvent(OperationalEvent operationalEventToUse)
		{
			SettingsPage.isFirstEvent = true;
			OperationalEvent operationalEvent = EventManagementPage.Events.Find((OperationalEvent x) => x.IndexID == operationalEventToUse.IndexID);
			this.SetCurrentEvent(operationalEvent);
			this.LoadCurrentEventPanel();
		}

		// Token: 0x060003A1 RID: 929 RVA: 0x000140C0 File Offset: 0x000122C0
		public void SetCurrentEvent(OperationalEvent eventToSet)
		{
			if (EventManagementPage.CurrentEvent != null)
			{
				EventManagementPage.CurrentEvent.LastUsedTime = DateTime.Now;
				EventManagementPage.CurrentEvent.SaveLocalXmlFile();
			}
			if (EventManagementPage.CurrentEvent == eventToSet)
			{
				return;
			}
			EventManagementPage.CurrentEvent = eventToSet;
			if (eventToSet != null)
			{
				EventManagementPage.onEventChanged(eventToSet);
			}
			else
			{
				CurrentEventPage.currentEventUIScript.Clear();
			}
			this.ReadAllEventDataAndlist();
			ExtensionMethod.WriteEventsDataToDatabase();
			this.gridFrame.Visibility = Visibility.Visible;
		}

		// Token: 0x060003A2 RID: 930 RVA: 0x00014130 File Offset: 0x00012330
		public Task ReadAllEventDataAndlist()
		{
			EventManagementPage.<ReadAllEventDataAndlist>d__35 <ReadAllEventDataAndlist>d__;
			<ReadAllEventDataAndlist>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<ReadAllEventDataAndlist>d__.<>4__this = this;
			<ReadAllEventDataAndlist>d__.<>1__state = -1;
			<ReadAllEventDataAndlist>d__.<>t__builder.Start<EventManagementPage.<ReadAllEventDataAndlist>d__35>(ref <ReadAllEventDataAndlist>d__);
			return <ReadAllEventDataAndlist>d__.<>t__builder.Task;
		}

		// Token: 0x060003A3 RID: 931 RVA: 0x00014174 File Offset: 0x00012374
		private void AddOlderEvents()
		{
			if (EventManagementPage.Events.Count > EventManagementPage.loadedEventCount)
			{
				if (EventManagementPage.loadedEventCount == 0 || this.EventReadIsWorking)
				{
					return;
				}
				List<OperationalEvent> EventsToLoad = (from x in EventManagementPage.Events
				orderby x.LastUsedTime descending
				select x).Skip(EventManagementPage.loadedEventCount).Take(10).ToList<OperationalEvent>();
				EventManagementPage.loadedEventCount += 10;
				int i;
				Action <>9__1;
				int j;
				for (i = 0; i < EventsToLoad.Count; i = j + 1)
				{
					if (EventManagementPage.CurrentEvent == null || !(EventsToLoad[i].IndexID == EventManagementPage.CurrentEvent.IndexID))
					{
						Dispatcher dispatcher = Application.Current.Dispatcher;
						Action callback;
						if ((callback = <>9__1) == null)
						{
							callback = (<>9__1 = delegate()
							{
								EventPrefab eventPrefab = new EventPrefab();
								eventPrefab.DeclaredOperationalEvent = EventsToLoad[i];
								Frame newFrame = new Frame();
								newFrame.Content = eventPrefab;
								this.EventList.Children.Insert(this.EventList.Children.Count, newFrame);
							});
						}
						dispatcher.Invoke(callback);
					}
					j = i;
				}
			}
		}

		// Token: 0x060003A4 RID: 932 RVA: 0x0001428C File Offset: 0x0001248C
		public static OperationalEvent GetCurrentEvent()
		{
			return EventManagementPage.CurrentEvent;
		}

		// Token: 0x060003A5 RID: 933 RVA: 0x00014293 File Offset: 0x00012493
		public static List<OperationalEvent> GetAllEvents()
		{
			return EventManagementPage.Events;
		}

		// Token: 0x060003A6 RID: 934 RVA: 0x0001429A File Offset: 0x0001249A
		public override string GetTitle()
		{
			return "Event Management";
		}

		// Token: 0x060003A7 RID: 935 RVA: 0x000142A1 File Offset: 0x000124A1
		public override string GetSubTitle()
		{
			return "Create a new event or edit your existing events or re-use your events on your archive. ";
		}

		// Token: 0x060003A8 RID: 936 RVA: 0x000142A8 File Offset: 0x000124A8
		private void SettingsSubPage_Unloaded(object sender, RoutedEventArgs e)
		{
			ExtensionMethod.WriteEventsDataToDatabase();
		}

		// Token: 0x060003A9 RID: 937 RVA: 0x000142B0 File Offset: 0x000124B0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocater = new Uri("/KingAIPhotoBoothPro;component/pages/activationking/eventmanagementpage.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocater);
		}

		// Token: 0x060003AA RID: 938 RVA: 0x000142E0 File Offset: 0x000124E0
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}

		// Token: 0x060003AB RID: 939 RVA: 0x000142EC File Offset: 0x000124EC
		[DebuggerNonUserCode]
		[GeneratedCode("PresentationBuildTasks", "4.0.0.0")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
			case 1:
				this.gridMain = (Grid)target;
				return;
			case 2:
				this.EventFrame = (Frame)target;
				return;
			case 3:
				this.EventLoadingGrid = (Grid)target;
				return;
			case 4:
				this.LoadingContainer = (Grid)target;
				return;
			case 5:
				this.gridFrame = (Grid)target;
				return;
			case 6:
				this.infoGrid = (StackPanel)target;
				return;
			case 7:
				this.infoImage = (System.Windows.Shapes.Path)target;
				return;
			case 8:
				this.ScrollEventArchive = (ScrollViewer)target;
				return;
			case 9:
				this.scrollBehavior = (ScrollViewerBottomBehavior)target;
				return;
			case 10:
				this.EventList = (StackPanel)target;
				return;
			default:
				this._contentLoaded = true;
				return;
			}
		}

		// Token: 0x04000387 RID: 903
		public static List<OperationalEvent> Events;

		// Token: 0x04000389 RID: 905
		private static CreateEventPage createEditPanel;

		// Token: 0x0400038A RID: 906
		private static CurrentEventPage currentEventPanel;

		// Token: 0x0400038B RID: 907
		private static OperationalEvent CurrentEvent;

		// Token: 0x0400038C RID: 908
		private List<Page> AllEventGameObjectList = new List<Page>();

		// Token: 0x0400038D RID: 909
		public Page EventPrefab;

		// Token: 0x0400038E RID: 910
		public Transform EventParentTransform;

		// Token: 0x04000390 RID: 912
		private static int loadedEventCount;

		// Token: 0x04000391 RID: 913
		public bool EventReadIsWorking;

		// Token: 0x04000392 RID: 914
		internal Grid gridMain;

		// Token: 0x04000393 RID: 915
		internal Frame EventFrame;

		// Token: 0x04000394 RID: 916
		internal Grid EventLoadingGrid;

		// Token: 0x04000395 RID: 917
		internal Grid LoadingContainer;

		// Token: 0x04000396 RID: 918
		internal Grid gridFrame;

		// Token: 0x04000397 RID: 919
		internal StackPanel infoGrid;

		// Token: 0x04000398 RID: 920
		internal System.Windows.Shapes.Path infoImage;

		// Token: 0x04000399 RID: 921
		internal ScrollViewer ScrollEventArchive;

		// Token: 0x0400039A RID: 922
		internal ScrollViewerBottomBehavior scrollBehavior;

		// Token: 0x0400039B RID: 923
		internal StackPanel EventList;

		// Token: 0x0400039C RID: 924
		private bool _contentLoaded;

		// Token: 0x0200017F RID: 383
		// (Invoke) Token: 0x06000CED RID: 3309
		public delegate void EventChanged(OperationalEvent newEvent);
	}
}
