using System;
using System.Collections.Generic;

namespace KingAIPhotoBoothPro.Class.Helper
{
	// Token: 0x020000CB RID: 203
	public static class CurrencyMinorUnit
	{
		// Token: 0x06000AF4 RID: 2804 RVA: 0x0003FFA7 File Offset: 0x0003E1A7
		public static long ToMinorUnits(decimal amount, string currency)
		{
			if (CurrencyMinorUnit.ZeroDecimal.Contains(currency))
			{
				return (long)decimal.Truncate(amount);
			}
			return (long)decimal.Truncate(amount * 100m);
		}

		// Token: 0x04000A54 RID: 2644
		private static readonly HashSet<string> ZeroDecimal = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"BIF",
			"CLP",
			"DJF",
			"GNF",
			"JPY",
			"KMF",
			"KRW",
			"MGA",
			"PYG",
			"RWF",
			"UGX",
			"VND",
			"VUV",
			"XAF",
			"XOF",
			"XPF"
		};
	}
}
