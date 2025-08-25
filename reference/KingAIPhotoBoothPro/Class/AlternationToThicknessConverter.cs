using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x02000099 RID: 153
	public class AlternationToThicknessConverter : IValueConverter
	{
		// Token: 0x0600092C RID: 2348 RVA: 0x00034084 File Offset: 0x00032284
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if ((int)value == 0)
			{
				return new Thickness(1.0, 1.0, 1.0, 1.0);
			}
			return new Thickness(1.0, 0.0, 1.0, 1.0);
		}

		// Token: 0x0600092D RID: 2349 RVA: 0x000340F8 File Offset: 0x000322F8
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
