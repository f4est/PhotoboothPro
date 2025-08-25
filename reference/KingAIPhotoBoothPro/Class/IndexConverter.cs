using System;
using System.Globalization;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x0200009D RID: 157
	public class IndexConverter : IMultiValueConverter
	{
		// Token: 0x06000974 RID: 2420 RVA: 0x00035D20 File Offset: 0x00033F20
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			if (values == null || values.Length < 2)
			{
				return "Invalid data";
			}
			if (values[0] == null || values[1] == null)
			{
				return "Null value detected";
			}
			int index;
			if (!int.TryParse(values[0].ToString(), out index))
			{
				return "Invalid index";
			}
			string content = values[1].ToString();
			if (content == null)
			{
				return "Invalid content";
			}
			return string.Format("{0}. {1}", index + 1, content);
		}

		// Token: 0x06000975 RID: 2421 RVA: 0x00035D88 File Offset: 0x00033F88
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
