using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

// Token: 0x02000009 RID: 9
public class BoolToVisibilityConverter : IValueConverter
{
	// Token: 0x0600001F RID: 31 RVA: 0x000026F2 File Offset: 0x000008F2
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002705 File Offset: 0x00000905
	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
