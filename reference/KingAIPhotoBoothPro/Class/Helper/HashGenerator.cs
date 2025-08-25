using System;
using System.Security.Cryptography;
using System.Text;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000D1 RID: 209
	public class HashGenerator
	{
		// Token: 0x06000B06 RID: 2822 RVA: 0x00040514 File Offset: 0x0003E714
		public static string GenerateHash(string input)
		{
			string result;
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);
				string base64Hash = Convert.ToBase64String(hashBytes);
				StringBuilder alphanumericHash = new StringBuilder();
				foreach (char c in base64Hash)
				{
					if (char.IsLetterOrDigit(c))
					{
						alphanumericHash.Append(c);
					}
					if (alphanumericHash.Length >= 6)
					{
						break;
					}
				}
				result = alphanumericHash.ToString().ToLower();
			}
			return result;
		}
	}
}
