using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using KingAIPhotoBoothPro.Class.ActivationKing;
using KingAIPhotoBoothPro.Pages;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D9 RID: 217
	public static class TwilioHelper
	{
		// Token: 0x06000B53 RID: 2899 RVA: 0x00042D27 File Offset: 0x00040F27
		private static string GetFilePath()
		{
			return Path.Combine(EventManagementPage.GetCurrentEvent().DirectoryPath, "smsData.json");
		}

		// Token: 0x06000B54 RID: 2900 RVA: 0x00042D40 File Offset: 0x00040F40
		public static Task<bool> SendTest(string body, string sendNumber)
		{
			TwilioHelper.<SendTest>d__7 <SendTest>d__;
			<SendTest>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<SendTest>d__.body = body;
			<SendTest>d__.sendNumber = sendNumber;
			<SendTest>d__.<>1__state = -1;
			<SendTest>d__.<>t__builder.Start<TwilioHelper.<SendTest>d__7>(ref <SendTest>d__);
			return <SendTest>d__.<>t__builder.Task;
		}

		// Token: 0x06000B55 RID: 2901 RVA: 0x00042D8B File Offset: 0x00040F8B
		public static void GetTwilioInfo()
		{
			TwilioHelper.accountSid = Settings.GetValueString("smssid");
			TwilioHelper.authToken = Settings.GetValueString("smsauthtoken");
			TwilioHelper.hostNumber = Settings.GetValueString("smshost");
		}

		// Token: 0x06000B56 RID: 2902 RVA: 0x00042DBC File Offset: 0x00040FBC
		public static void SendSMS(string sendNumber, string body)
		{
			if (TwilioHelper.sendThread == null)
			{
				TwilioHelper.StartSMSThread();
			}
			if (string.IsNullOrEmpty(TwilioHelper.accountSid) || string.IsNullOrEmpty(TwilioHelper.authToken) || string.IsNullOrEmpty(TwilioHelper.hostNumber))
			{
				TwilioHelper.GetTwilioInfo();
			}
			TwilioHelper.smsDataList.Add(new SMSInfo
			{
				body = body,
				HostNumber = TwilioHelper.hostNumber,
				SendNumber = sendNumber,
				isMailSend = false
			});
			ExtensionMethod.CreateWriteJson<List<SMSInfo>>(TwilioHelper.smsDataList, TwilioHelper.GetFilePath());
		}

		// Token: 0x06000B57 RID: 2903 RVA: 0x00042E3C File Offset: 0x0004103C
		public static Task SendSMSAsync(SMSInfo smsInfo)
		{
			TwilioHelper.<SendSMSAsync>d__10 <SendSMSAsync>d__;
			<SendSMSAsync>d__.<>t__builder = AsyncTaskMethodBuilder.Create();
			<SendSMSAsync>d__.smsInfo = smsInfo;
			<SendSMSAsync>d__.<>1__state = -1;
			<SendSMSAsync>d__.<>t__builder.Start<TwilioHelper.<SendSMSAsync>d__10>(ref <SendSMSAsync>d__);
			return <SendSMSAsync>d__.<>t__builder.Task;
		}

		// Token: 0x06000B58 RID: 2904 RVA: 0x00042E80 File Offset: 0x00041080
		public static void StartSMSThread()
		{
			string filePath = TwilioHelper.GetFilePath();
			if (TwilioHelper.smsDataList == null)
			{
				if (File.Exists(filePath))
				{
					List<SMSInfo> smsInfos = ExtensionMethod.ReadJson<List<SMSInfo>>(filePath);
					if (smsInfos == null)
					{
						Debug.Log("TwilioHelper", string.Format("File is corrupted\n\nFile name : {0}", filePath), "StartSMSThread", 109);
						TwilioHelper.smsDataList = new List<SMSInfo>();
					}
					else
					{
						TwilioHelper.smsDataList = (from x in smsInfos
						where !x.isMailSend
						select x).ToList<SMSInfo>();
					}
				}
				else
				{
					TwilioHelper.smsDataList = new List<SMSInfo>();
				}
			}
			if (TwilioHelper.sendThread == null)
			{
				TwilioHelper.sendThread = new Thread(new ThreadStart(TwilioHelper.SendSmsThreadFunc));
				TwilioHelper.sendThread.Start();
			}
			ExtensionMethod.CreateWriteJson<List<SMSInfo>>(TwilioHelper.smsDataList, filePath);
		}

		// Token: 0x06000B59 RID: 2905 RVA: 0x00042F44 File Offset: 0x00041144
		private static void SendSmsThreadFunc()
		{
			TwilioHelper.<SendSmsThreadFunc>d__12 <SendSmsThreadFunc>d__;
			<SendSmsThreadFunc>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
			<SendSmsThreadFunc>d__.<>1__state = -1;
			<SendSmsThreadFunc>d__.<>t__builder.Start<TwilioHelper.<SendSmsThreadFunc>d__12>(ref <SendSmsThreadFunc>d__);
		}

		// Token: 0x04000A76 RID: 2678
		public static List<SMSInfo> smsDataList;

		// Token: 0x04000A77 RID: 2679
		public static bool isForceQuit;

		// Token: 0x04000A78 RID: 2680
		public static Thread sendThread;

		// Token: 0x04000A79 RID: 2681
		public static string accountSid;

		// Token: 0x04000A7A RID: 2682
		public static string authToken;

		// Token: 0x04000A7B RID: 2683
		public static string hostNumber;
	}
}
