using System;
using System.Globalization;
using Newtonsoft.Json;

namespace KingAIPhotoBoothPro.Helper
{
	// Token: 0x02000021 RID: 33
	public class SafeDateTimeConverter : JsonConverter
	{
		// Token: 0x0600018B RID: 395 RVA: 0x000087CA File Offset: 0x000069CA
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
		}

		// Token: 0x0600018C RID: 396 RVA: 0x000087F0 File Offset: 0x000069F0
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			string dateString = reader.Value.ToString();
			string format = "dd.MM.yyyy HH:mm:ss";
			DateTime dt;
			if (DateTime.TryParseExact(dateString, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
			{
				if (dt.Year < 1900)
				{
					return new DateTime(1900, 4, 30);
				}
				if (dt.Year > 2077)
				{
					return new DateTime(2077, 11, 16);
				}
				return dt;
			}
			else
			{
				if (!DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
				{
					return DateTime.Now.AddYears(1);
				}
				if (dt.Year < 1900)
				{
					return new DateTime(1900, 4, 30);
				}
				if (dt.Year > 2077)
				{
					return new DateTime(2077, 11, 16);
				}
				return dt;
			}
		}

		// Token: 0x0600018D RID: 397 RVA: 0x000088E8 File Offset: 0x00006AE8
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(((DateTime)value).ToString("yyyy-MM-ddTHH:mm:ss"));
		}
	}
}
