using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A4 RID: 164
	public class MarginTopBottomConverter : IValueConverter
	{
		// Token: 0x0600098E RID: 2446 RVA: 0x0003641C File Offset: 0x0003461C
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double height = System.Convert.ToDouble(value);
			double multiplier = 1.0;
			if (parameter != null)
			{
				multiplier = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
			}
			return new Thickness(0.0, height * multiplier, 0.0, height * multiplier);
		}

		// Token: 0x0600098F RID: 2447 RVA: 0x0003646B File Offset: 0x0003466B
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
