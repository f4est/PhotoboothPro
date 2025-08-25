using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A6 RID: 166
	public class ScreenHeightPercentageConverter : IValueConverter
	{
		// Token: 0x06000994 RID: 2452 RVA: 0x000364AC File Offset: 0x000346AC
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double percentage = System.Convert.ToDouble(parameter);
			return SystemParameters.PrimaryScreenHeight * percentage;
		}

		// Token: 0x06000995 RID: 2453 RVA: 0x000364CC File Offset: 0x000346CC
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
