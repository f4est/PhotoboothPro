using System;
using System.IO;
using System.Runtime.CompilerServices;
using KingAIPhotoBoothPro;

// Token: 0x02000006 RID: 6
public static class Debug
{
	// Token: 0x06000019 RID: 25 RVA: 0x000024FC File Offset: 0x000006FC
	public static void Log(string senderName, string message, [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
	{
		try
		{
			using (StreamWriter file = new StreamWriter(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fffff.log"), true))
			{
				file.WriteLine(Debug.GenerateDebugLine(senderName, message, memberName, lineNumber));
				file.Close();
			}
		}
		catch (Exception)
		{
		}
		ActivationKingWindow.LogDebug(senderName, message, memberName, lineNumber);
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002570 File Offset: 0x00000770
	public static string GenerateDebugLine(string senderName, string message, string memberName, int lineNumber)
	{
		return string.Format("{0}:{1} - Line:{2} ({3}): {4}", new object[]
		{
			senderName,
			memberName,
			lineNumber,
			DateTime.Now.ToString("HH:mm:ss dd/MM/yyyy"),
			message
		});
	}
}
