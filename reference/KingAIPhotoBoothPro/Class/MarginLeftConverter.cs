using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000AF RID: 175
	public class MarginLeftConverter : IValueConverter
	{
		// Token: 0x060009E5 RID: 2533 RVA: 0x00039318 File Offset: 0x00037518
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double width = System.Convert.ToDouble(value);
			double multiplier = 1.0;
			if (parameter != null)
			{
				multiplier = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
			}
			return new Thickness(width * multiplier, 0.0, 0.0, 0.0);
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x0003936D File Offset: 0x0003756D
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
