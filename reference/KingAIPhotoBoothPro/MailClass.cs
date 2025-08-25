using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using KingAIPhotoBoothPro;
using KingAIPhotoBoothPro.Class;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Class.Helper;
using KingAIPhotoBoothPro.Modals;
using KingAIPhotoBoothPro.Pages;
using MongoDB.Bson;
using Newtonsoft.Json;

// Token: 0x0200000A RID: 10
public static class MailClass
{
	// Token: 0x06000022 RID: 34 RVA: 0x00002714 File Offset: 0x00000914
	public static Task<bool> TestMailAsync(string addressToSend)
	{
		MailClass.<TestMailAsync>d__22 <TestMailAsync>d__;
		<TestMailAsync>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
		<TestMailAsync>d__.addressToSend = addressToSend;
		<TestMailAsync>d__.<>1__state = -1;
		<TestMailAsync>d__.<>t__builder.Start<MailClass.<TestMailAsync>d__22>(ref <TestMailAsync>d__);
		return <TestMailAsync>d__.<>t__builder.Task;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002758 File Offset: 0x00000958
	private static void SendMailThreadFunc()
	{
		while (!MailClass.isForceQuit)
		{
			List<MailData> NoSendingMails = (from x in MailClass.mailDataList
			where !x.isMailSend
			select x).ToList<MailData>();
			for (int i = 0; i < NoSendingMails.Count; i++)
			{
				if (NoSendingMails[i].GetMedia() != null && NoSendingMails[i].GetMedia().isCloudSynced)
				{
					try
					{
						bool mailSent = false;
						MailClass.MailRequest mailRequest = new MailClass.MailRequest
						{
							Body = NoSendingMails[i].mailInfo.Body,
							FilePath = NoSendingMails[i].FileInf.Filename,
							HostMailAddress = NoSendingMails[i].mailInfo.HostMailAddress,
							MailAddressToSend = NoSendingMails[i].mailInfo.MailAddress,
							SmtpHost = NoSendingMails[i].mailInfo.SmtpHost,
							SmtpPassword = NoSendingMails[i].mailInfo.SmtpPassword,
							SmtpPort = NoSendingMails[i].mailInfo.SmtpPort,
							Subject = NoSendingMails[i].mailInfo.Subject,
							Message = MailClass.GetUrlForMedia(NoSendingMails[i].GetMedia().MediaHash, ""),
							isHTML = false,
							MediaHash = NoSendingMails[i].GetMedia().MediaHash
						};
						Task<HTTPHelper.PostResult> postResultTask = HTTPHelper.PostRequestAsync<MailClass.MailRequest>("api/app/sendemailparameters", mailRequest, null, "");
						postResultTask.Wait();
						if (postResultTask.Result.ServerCommunicationSuccess)
						{
							MailClass.MailResponse response = JsonConvert.DeserializeObject<MailClass.MailResponse>(postResultTask.Result.Message);
							mailSent = response.Success;
						}
						else
						{
							MailClass.MailResultMessage = postResultTask.Result.Message;
						}
						if (!mailSent)
						{
							MessageBoxWindow.CreateWindow("Mail Error", MailClass.MailResultMessage, new List<MessageBoxWindow.ButtonType>
							{
								MessageBoxWindow.ButtonType.Continue
							}, MessageBoxWindow.MessageIcon.Warning, null, MessageBoxWindow.MessageBoxSize.Large, false);
						}
						NoSendingMails[i].isMailSend = true;
					}
					catch (Exception)
					{
					}
					Thread.Sleep(1);
				}
			}
			Thread.Sleep(500);
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00002998 File Offset: 0x00000B98
	private static void GenerateDataDir(string eventPath)
	{
		string dataDir = Path.Combine(eventPath, "Data");
		if (!Directory.Exists(dataDir))
		{
			Directory.CreateDirectory(dataDir);
		}
	}

	// Token: 0x06000025 RID: 37 RVA: 0x000029C0 File Offset: 0x00000BC0
	public static void WriteMailToExcel(string MailResultMessage)
	{
		string eventPath = EventManagementPage.GetCurrentEvent().DirectoryPath;
		MailClass.GenerateDataDir(eventPath);
		string dataDir = Path.Combine(eventPath, "Data");
		string[] rowDataTemp = new string[6];
		MailClass.localDate = DateTime.Now;
		string filePath = Path.Combine(dataDir, "Mail_data.csv");
		StreamWriter outStream;
		if (!File.Exists(filePath))
		{
			rowDataTemp[0] = "Tarih Saat";
			rowDataTemp[1] = "Email";
			rowDataTemp[2] = "Fotoğraf İsmi";
			rowDataTemp[5] = "Gönderilme Durumu";
			rowDataTemp[3] = "Konu";
			rowDataTemp[4] = "Metin";
			outStream = File.CreateText(filePath);
			outStream.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}", new object[]
			{
				rowDataTemp[0],
				rowDataTemp[1],
				rowDataTemp[2],
				rowDataTemp[3],
				rowDataTemp[4],
				rowDataTemp[5]
			}));
			outStream.Close();
		}
		rowDataTemp[0] = string.Format("{5:00}-{4:00}-{3:00}-{2:00}-{1:00}-{0:00}", new object[]
		{
			MailClass.localDate.Second,
			MailClass.localDate.Minute,
			MailClass.localDate.Hour,
			MailClass.localDate.Day,
			MailClass.localDate.Month,
			MailClass.localDate.Year
		});
		rowDataTemp[1] = MailClass.mailAddressToSend;
		if (!string.IsNullOrEmpty(MailClass.mailAttachmentFilePath))
		{
			rowDataTemp[2] = Path.GetFileName(MailClass.mailAttachmentFilePath);
		}
		rowDataTemp[5] = MailResultMessage;
		rowDataTemp[3] = MailClass.mailSubject;
		rowDataTemp[4] = MailClass.mailBody;
		outStream = File.AppendText(filePath);
		outStream.WriteLine(string.Format("{0},{1},{2},{3},{4},{5}", new object[]
		{
			rowDataTemp[0],
			rowDataTemp[1],
			rowDataTemp[2],
			rowDataTemp[3],
			rowDataTemp[4],
			rowDataTemp[5]
		}));
		outStream.Close();
	}

	// Token: 0x06000026 RID: 38 RVA: 0x00002B88 File Offset: 0x00000D88
	public static int GetMailNumber()
	{
		if (EventManagementPage.GetCurrentEvent() == null)
		{
			return 0;
		}
		string eventPath = EventManagementPage.GetCurrentEvent().DirectoryPath;
		MailClass.GenerateDataDir(eventPath);
		string dataDir = Path.Combine(eventPath, "Data");
		MailClass.localDate = DateTime.Now;
		string filePath = Path.Combine(dataDir, "Mail_data.csv");
		if (!File.Exists(filePath))
		{
			return 0;
		}
		return File.ReadAllText(filePath).Split(new char[]
		{
			'\n'
		}).Length - 1;
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00002BF8 File Offset: 0x00000DF8
	public static int GetMailNumber(OperationalEvent operationalEvent)
	{
		if (operationalEvent == null)
		{
			return 0;
		}
		string eventPath = operationalEvent.DirectoryPath;
		MailClass.GenerateDataDir(eventPath);
		string dataDir = Path.Combine(eventPath, "Data");
		MailClass.localDate = DateTime.Now;
		string filePath = Path.Combine(dataDir, "Mail_data.csv");
		if (!File.Exists(filePath))
		{
			return 0;
		}
		return File.ReadAllText(filePath).Split(new char[]
		{
			'\n'
		}).Length - 1;
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00002C60 File Offset: 0x00000E60
	public static void StartEmailThread()
	{
		if (MailClass.mailDataList == null)
		{
			if (File.Exists(EventManagementPage.GetCurrentEvent().MailJsonPath))
			{
				List<MailData> mailDatas = ExtensionMethod.ReadJson<List<MailData>>(EventManagementPage.GetCurrentEvent().MailJsonPath);
				if (mailDatas == null)
				{
					MailClass.mailDataList = new List<MailData>();
					Debug.Log("MailClass", string.Format("File is corrupted\n\nFile name : {0}", EventManagementPage.GetCurrentEvent().MailJsonPath), "StartEmailThread", 278);
				}
				else
				{
					MailClass.mailDataList = (from x in mailDatas
					where x.FileInf != null
					select x).ToList<MailData>();
				}
			}
			else
			{
				MailClass.mailDataList = new List<MailData>();
			}
		}
		if (MailClass.sendThread == null)
		{
			MailClass.sendThread = new Thread(new ThreadStart(MailClass.SendMailThreadFunc));
			MailClass.sendThread.Start();
		}
		ExtensionMethod.CreateWriteJson<List<MailData>>(MailClass.mailDataList, EventManagementPage.GetCurrentEvent().MailJsonPath);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00002D45 File Offset: 0x00000F45
	private static string GetUrlForMedia(string mediaHash, string addFolder = "")
	{
		return string.Format("https://activationshare.com/m/{0}{1}", mediaHash, addFolder);
	}

	// Token: 0x0600002A RID: 42 RVA: 0x00002D54 File Offset: 0x00000F54
	public static void SendEmailNoUpload(string addressToSend, MediaClassBase attachmentFile, string mediaUrl)
	{
		if (MailClass.sendThread == null)
		{
			MailClass.sendThread = new Thread(new ThreadStart(MailClass.SendMailThreadFunc));
			MailClass.sendThread.Start();
		}
		string eventPath = EventManagementPage.GetCurrentEvent().DirectoryPath;
		MailClass.mailAddressToSend = addressToSend;
		MailClass.mailAttachmentFilePath = attachmentFile.resultFile.FullName;
		MailClass.eventPathForMail = eventPath;
		MailClass.GetMailInformation();
		MailClass.mailInfo = new MailClass.MailRequest
		{
			HostMailAddress = MailClass.senderMailaddress,
			SmtpPassword = MailClass.senderPassword,
			SmtpHost = MailClass.senderSmtpHost,
			SmtpPort = MailClass.senderSmtpPort,
			Subject = MailClass.mailSubject,
			Body = MailClass.mailBody,
			FilePath = MailClass.mailAttachmentFilePath,
			MailAddressToSend = MailClass.mailAddressToSend,
			Message = mediaUrl
		};
		MailClass.CreateMailData(MailClass.mailInfo, attachmentFile);
	}

	// Token: 0x0600002B RID: 43 RVA: 0x00002E28 File Offset: 0x00001028
	public static void GetMailInformation()
	{
		MailClass.senderMailaddress = Settings.GetValueString("mail_address");
		MailClass.senderPassword = Settings.GetValueString("mail_password");
		MailClass.senderSmtpHost = Settings.GetValueString("mail_host");
		MailClass.senderSmtpPort = Settings.GetValueInt("mail_port").GetValueOrDefault();
		MailClass.mailSubject = Settings.GetValueString("mail_subject");
		MailClass.mailBody = Settings.GetValueString("mail_body");
	}

	// Token: 0x0600002C RID: 44 RVA: 0x00002E98 File Offset: 0x00001098
	public static Task SendErrorWarningEmailAsync(string subtitle, string description, string notesHtml = "")
	{
		MailClass.<SendErrorWarningEmailAsync>d__32 <SendErrorWarningEmailAsync>d__;
		<SendErrorWarningEmailAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
		<SendErrorWarningEmailAsync>d__.<>1__state = -1;
		<SendErrorWarningEmailAsync>d__.<>t__builder.Start<MailClass.<SendErrorWarningEmailAsync>d__32>(ref <SendErrorWarningEmailAsync>d__);
		return <SendErrorWarningEmailAsync>d__.<>t__builder.Task;
	}

	// Token: 0x0600002D RID: 45 RVA: 0x00002ED4 File Offset: 0x000010D4
	public static void CreateMailData(MailClass.MailRequest mailInfo, MediaClassBase media)
	{
		if (MailClass.mailDataList == null)
		{
			if (File.Exists(EventManagementPage.GetCurrentEvent().MailJsonPath))
			{
				MailClass.mailDataList = ExtensionMethod.ReadJson<List<MailData>>(EventManagementPage.GetCurrentEvent().MailJsonPath);
			}
			else
			{
				MailClass.mailDataList = new List<MailData>();
			}
		}
		MailClass.mailDataList.Add(new MailData
		{
			Id = ObjectId.GenerateNewId().ToString(),
			IsCloudSync = false,
			EventID = EventManagementPage.GetCurrentEvent().IndexID,
			MailDatetime = DateTime.Now,
			FileInf = media.fileDetails,
			Message = mailInfo.Message,
			mailInfo = new MailInfo
			{
				Body = mailInfo.Body,
				HostMailAddress = mailInfo.HostMailAddress,
				MailAddress = mailInfo.MailAddressToSend,
				SmtpHost = mailInfo.SmtpHost,
				SmtpPassword = mailInfo.SmtpPassword,
				SmtpPort = mailInfo.SmtpPort,
				Subject = mailInfo.Subject
			}
		});
		ExtensionMethod.CreateWriteJson<List<MailData>>(MailClass.mailDataList, EventManagementPage.GetCurrentEvent().MailJsonPath);
	}

	// Token: 0x0400000D RID: 13
	public const string emailAddress = "hello@activationking.com";

	// Token: 0x0400000E RID: 14
	public static string streamingPath = Path.Combine(new string[]
	{
		AppDomain.CurrentDomain.BaseDirectory
	});

	// Token: 0x0400000F RID: 15
	public const string configFilename = "config.xml";

	// Token: 0x04000010 RID: 16
	private static string mailAddressToSend;

	// Token: 0x04000011 RID: 17
	private static string mailAttachmentFilePath;

	// Token: 0x04000012 RID: 18
	public static Thread sendThread;

	// Token: 0x04000013 RID: 19
	public static bool isForceQuit = false;

	// Token: 0x04000014 RID: 20
	public static bool isTestMail = false;

	// Token: 0x04000015 RID: 21
	private static string senderMailaddress;

	// Token: 0x04000016 RID: 22
	private static string senderPassword;

	// Token: 0x04000017 RID: 23
	private static string senderSmtpHost = "smtp.gmail.com";

	// Token: 0x04000018 RID: 24
	private static int senderSmtpPort = 587;

	// Token: 0x04000019 RID: 25
	private static string mailSubject;

	// Token: 0x0400001A RID: 26
	private static string mailBody;

	// Token: 0x0400001B RID: 27
	private static string MailResultMessage;

	// Token: 0x0400001C RID: 28
	private static MailClass.MailRequest mailInfo;

	// Token: 0x0400001D RID: 29
	public static string Frame;

	// Token: 0x0400001E RID: 30
	public static string eventPathForMail;

	// Token: 0x0400001F RID: 31
	private static DateTime localDate;

	// Token: 0x04000020 RID: 32
	public static List<MailData> mailDataList;

	// Token: 0x020000E1 RID: 225
	[Serializable]
	public class MailRequest
	{
		// Token: 0x04000AA0 RID: 2720
		public string HostMailAddress;

		// Token: 0x04000AA1 RID: 2721
		public string MailAddressToSend;

		// Token: 0x04000AA2 RID: 2722
		public string Body;

		// Token: 0x04000AA3 RID: 2723
		public string Subject;

		// Token: 0x04000AA4 RID: 2724
		public string Message;

		// Token: 0x04000AA5 RID: 2725
		public string FilePath;

		// Token: 0x04000AA6 RID: 2726
		public string SmtpPassword;

		// Token: 0x04000AA7 RID: 2727
		public string SmtpHost;

		// Token: 0x04000AA8 RID: 2728
		public int SmtpPort;

		// Token: 0x04000AA9 RID: 2729
		public bool isHTML;

		// Token: 0x04000AAA RID: 2730
		public string MediaHash;

		// Token: 0x04000AAB RID: 2731
		public string MailCCAddressToSend;
	}

	// Token: 0x020000E2 RID: 226
	public class MailResponse : GenericResponse
	{
	}
}
