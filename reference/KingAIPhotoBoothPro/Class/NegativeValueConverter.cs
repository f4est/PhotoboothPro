using System;
using System.Globalization;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A5 RID: 165
	public class NegativeValueConverter : IValueConverter
	{
		// Token: 0x06000991 RID: 2449 RVA: 0x00036478 File Offset: 0x00034678
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is double)
			{
				double doubleValue = (double)value;
				return -doubleValue;
			}
			return value;
		}

		// Token: 0x06000992 RID: 2450 RVA: 0x0003649D File Offset: 0x0003469D
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
