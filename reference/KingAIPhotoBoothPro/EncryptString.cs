using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

// Token: 0x0200000C RID: 12
public static class EncryptString
{
	// Token: 0x06000042 RID: 66 RVA: 0x00003AE0 File Offset: 0x00001CE0
	public static string Encrypt(string plainText, string passPhrase)
	{
		byte[] saltStringBytes = EncryptString.Generate256BitsOfRandomEntropy();
		byte[] ivStringBytes = EncryptString.Generate256BitsOfRandomEntropy();
		byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
		string result;
		using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, 1000))
		{
			byte[] keyBytes = password.GetBytes(32);
			using (RijndaelManaged symmetricKey = new RijndaelManaged())
			{
				symmetricKey.BlockSize = 256;
				symmetricKey.Mode = CipherMode.CBC;
				symmetricKey.Padding = PaddingMode.PKCS7;
				using (ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
						{
							cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
							cryptoStream.FlushFinalBlock();
							byte[] cipherTextBytes = saltStringBytes;
							cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray<byte>();
							cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray<byte>();
							memoryStream.Close();
							cryptoStream.Close();
							result = Convert.ToBase64String(cipherTextBytes);
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00003C30 File Offset: 0x00001E30
	public static string Decrypt(string cipherText, string passPhrase)
	{
		byte[] cipherTextBytesWithSaltAndIv;
		try
		{
			cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
		}
		catch (FormatException ex)
		{
			Debug.Log("EncryptString", ex.Message, "Decrypt", 65);
			return null;
		}
		byte[] saltStringBytes = cipherTextBytesWithSaltAndIv.Take(32).ToArray<byte>();
		byte[] ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(32).Take(32).ToArray<byte>();
		byte[] cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(64).Take(cipherTextBytesWithSaltAndIv.Length - 64).ToArray<byte>();
		string result;
		using (Rfc2898DeriveBytes password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, 1000))
		{
			byte[] keyBytes = password.GetBytes(32);
			using (RijndaelManaged symmetricKey = new RijndaelManaged())
			{
				symmetricKey.BlockSize = 256;
				symmetricKey.Mode = CipherMode.CBC;
				symmetricKey.Padding = PaddingMode.PKCS7;
				using (ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
				{
					using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
					{
						using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
						{
							using (StreamReader streamReader = new StreamReader(cryptoStream, Encoding.UTF8))
							{
								result = streamReader.ReadToEnd();
							}
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x06000044 RID: 68 RVA: 0x00003DBC File Offset: 0x00001FBC
	private static byte[] Generate256BitsOfRandomEntropy()
	{
		byte[] randomBytes = new byte[32];
		using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
		{
			rngCsp.GetBytes(randomBytes);
		}
		return randomBytes;
	}

	// Token: 0x04000030 RID: 48
	private const int Keysize = 256;

	// Token: 0x04000031 RID: 49
	private const int DerivationIterations = 1000;
}
