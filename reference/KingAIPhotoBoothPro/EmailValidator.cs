using System;
using System.Net.Mail;

// Token: 0x02000007 RID: 7
public static class EmailValidator
{
	// Token: 0x0600001B RID: 27 RVA: 0x000025B8 File Offset: 0x000007B8
	public static bool IsValidEmail(string email)
	{
		if (string.IsNullOrEmpty(email))
		{
			return false;
		}
		bool result;
		try
		{
			MailAddress mail = new MailAddress(email);
			result = true;
		}
		catch (FormatException)
		{
			result = false;
		}
		return result;
	}
}
