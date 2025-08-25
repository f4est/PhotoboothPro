using System;
using System.IO;
using System.Net;
using System.Threading;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x0200009A RID: 154
	public static class FTPUpload
	{
		// Token: 0x0600092F RID: 2351 RVA: 0x00034107 File Offset: 0x00032307
		public static string GetFileUrl(string fileName)
		{
			return FTPUpload.HTTPHost + new FileInfo(fileName).Name;
		}

		// Token: 0x06000930 RID: 2352 RVA: 0x0003411E File Offset: 0x0003231E
		private static void UploadThreadFunc()
		{
			while (!FTPUpload.isForceQuit)
			{
				Thread.Sleep(500);
			}
		}

		// Token: 0x06000931 RID: 2353 RVA: 0x00034134 File Offset: 0x00032334
		private static void UploadFile(string filePath, MediaClassBase media)
		{
			try
			{
				if (!string.IsNullOrEmpty(filePath))
				{
					if (File.Exists(filePath))
					{
						WebClient client = new WebClient();
						string uploadFileName = new FileInfo(filePath).Name;
						Uri uri = new Uri(FTPUpload.FTPHost + uploadFileName);
						client.Credentials = new NetworkCredential(FTPUpload.FTPUserName, FTPUpload.FTPPassword);
						client.UploadFile(uri, "STOR", filePath);
						media.isUploaded = true;
					}
					else
					{
						Console.WriteLine("Attachment file not found");
					}
				}
			}
			catch (Exception)
			{
			}
		}

		// Token: 0x06000932 RID: 2354 RVA: 0x000341C0 File Offset: 0x000323C0
		public static void StartUpload()
		{
			if (FTPUpload.UploadThread == null)
			{
				FTPUpload.UploadThread = new Thread(new ThreadStart(FTPUpload.UploadThreadFunc));
				FTPUpload.UploadThread.Start();
			}
		}

		// Token: 0x04000912 RID: 2322
		private static string FTPHost = "ftp://licence.hisseliharikalar.com/Temp/";

		// Token: 0x04000913 RID: 2323
		private static string HTTPHost = "http://licence.hisseliharikalar.com/Temp/";

		// Token: 0x04000914 RID: 2324
		private static string FTPUserName = "hhf_licence_ft329497";

		// Token: 0x04000915 RID: 2325
		private static string FTPPassword = "^s6zd3D6";

		// Token: 0x04000916 RID: 2326
		public static Thread UploadThread = null;

		// Token: 0x04000917 RID: 2327
		public static bool isForceQuit = false;
	}
}
