using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000A8 RID: 168
	public class SpecificMarginConverter : IValueConverter
	{
		// Token: 0x0600099B RID: 2459 RVA: 0x00036538 File Offset: 0x00034738
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double pageDimension = (double)value;
			if (parameter == null)
			{
				return new Thickness(0.0, 0.0, 0.0, 0.0);
			}
			string[] parameters = parameter.ToString().Split(new char[]
			{
				','
			});
			string marginSide = parameters[0];
			if (parameters.Length < 2)
			{
				return new Thickness(0.0, 0.0, 0.0, 0.0);
			}
			if (string.IsNullOrEmpty(parameters[1]))
			{
				return new Thickness(0.0, 0.0, 0.0, 0.0);
			}
			object result;
			try
			{
				double ratio = double.Parse(parameters[1], CultureInfo.InvariantCulture);
				double minMargin = (parameters.Length > 2) ? double.Parse(parameters[2], CultureInfo.InvariantCulture) : 0.0;
				double calculatedMargin = pageDimension * ratio;
				calculatedMargin = Math.Max(calculatedMargin, minMargin);
				if (!(marginSide == "left"))
				{
					if (!(marginSide == "right"))
					{
						if (!(marginSide == "up"))
						{
							if (!(marginSide == "top"))
							{
								if (!(marginSide == "down"))
								{
									if (!(marginSide == "bottom"))
									{
										result = new Thickness(0.0);
									}
									else
									{
										result = new Thickness(0.0, 0.0, 0.0, calculatedMargin);
									}
								}
								else
								{
									result = new Thickness(0.0, 0.0, 0.0, calculatedMargin);
								}
							}
							else
							{
								result = new Thickness(0.0, calculatedMargin, 0.0, 0.0);
							}
						}
						else
						{
							result = new Thickness(0.0, calculatedMargin, 0.0, 0.0);
						}
					}
					else
					{
						result = new Thickness(0.0, 0.0, calculatedMargin, 0.0);
					}
				}
				else
				{
					result = new Thickness(calculatedMargin, 0.0, 0.0, 0.0);
				}
			}
			catch (Exception)
			{
				result = new Thickness(0.0, 0.0, 0.0, 0.0);
			}
			return result;
		}

		// Token: 0x0600099C RID: 2460 RVA: 0x0003681C File Offset: 0x00034A1C
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
