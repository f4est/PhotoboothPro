using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x0200009F RID: 159
	public static class PaperSizeData
	{
		// Token: 0x0600097A RID: 2426 RVA: 0x00035E43 File Offset: 0x00034043
		public static List<string> GetNames()
		{
			return (from size in PaperSizeData.Sizes.Values
			select size.Name).ToList<string>();
		}

		// Token: 0x0600097B RID: 2427 RVA: 0x00035E78 File Offset: 0x00034078
		public static int GetPaperSizeIndex(int width, int height)
		{
			var index = PaperSizeData.Sizes.Select((KeyValuePair<PaperSizeEnum, PaperSize> pair, int i) => new
			{
				pair,
				i
			}).FirstOrDefault(x => x.pair.Value.Width == width && x.pair.Value.Height == height);
			if (index == null)
			{
				return -1;
			}
			return index.i;
		}

		// Token: 0x0600097C RID: 2428 RVA: 0x00035EDF File Offset: 0x000340DF
		public static PaperSize Get(PaperSizeEnum type)
		{
			return PaperSizeData.Sizes[type];
		}

		// Token: 0x0600097D RID: 2429 RVA: 0x00035EEC File Offset: 0x000340EC
		[return: TupleElementNames(new string[]
		{
			"Enum",
			"Size"
		})]
		public static ValueTuple<PaperSizeEnum, PaperSize>? GetPaperSize(int width, int height)
		{
			KeyValuePair<PaperSizeEnum, PaperSize> match = PaperSizeData.Sizes.FirstOrDefault((KeyValuePair<PaperSizeEnum, PaperSize> pair) => pair.Value.Width == width && pair.Value.Height == height);
			if (match.Equals(default(KeyValuePair<PaperSizeEnum, PaperSize>)))
			{
				return null;
			}
			return new ValueTuple<PaperSizeEnum, PaperSize>?(new ValueTuple<PaperSizeEnum, PaperSize>(match.Key, match.Value));
		}

		// Token: 0x0600097E RID: 2430 RVA: 0x00035F64 File Offset: 0x00034164
		[return: TupleElementNames(new string[]
		{
			"Enum",
			"Size"
		})]
		public static ValueTuple<PaperSizeEnum, PaperSize>? GetPaperSizeByName(string name)
		{
			KeyValuePair<PaperSizeEnum, PaperSize> match = PaperSizeData.Sizes.FirstOrDefault((KeyValuePair<PaperSizeEnum, PaperSize> pair) => pair.Value.Name == name);
			if (match.Equals(default(KeyValuePair<PaperSizeEnum, PaperSize>)))
			{
				return null;
			}
			return new ValueTuple<PaperSizeEnum, PaperSize>?(new ValueTuple<PaperSizeEnum, PaperSize>(match.Key, match.Value));
		}

		// Token: 0x04000951 RID: 2385
		public static readonly Dictionary<PaperSizeEnum, PaperSize> Sizes = new Dictionary<PaperSizeEnum, PaperSize>
		{
			{
				PaperSizeEnum.P_6x4,
				new PaperSize("6\" x 4\"", 1800, 1200)
			},
			{
				PaperSizeEnum.P_4x6,
				new PaperSize("4\" x 6\"", 1200, 1800)
			},
			{
				PaperSizeEnum.P_8x6,
				new PaperSize("8\" x 6\"", 2400, 1800)
			},
			{
				PaperSizeEnum.P_6x8,
				new PaperSize("6\" x 8\"", 1800, 2400)
			},
			{
				PaperSizeEnum.P_5x7,
				new PaperSize("5\" x 7\"", 1500, 2100)
			},
			{
				PaperSizeEnum.P_7x5,
				new PaperSize("7\" x 5\"", 2100, 1500)
			},
			{
				PaperSizeEnum.P_1_1,
				new PaperSize("1:1", 1280, 1280)
			},
			{
				PaperSizeEnum.P_9x16_Full_HD,
				new PaperSize("9:16 Full HD", 1080, 1920)
			},
			{
				PaperSizeEnum.P_16x9_Full_HD,
				new PaperSize("16:9 Full HD", 1920, 1080)
			},
			{
				PaperSizeEnum.P_9x16_HD,
				new PaperSize("9:16 HD", 720, 1280)
			},
			{
				PaperSizeEnum.P_16x9_HD,
				new PaperSize("16:9 HD", 1280, 720)
			},
			{
				PaperSizeEnum.P_6x4_Ultra_Res,
				new PaperSize("6\" x 4\" Ultra Res", 3600, 2400)
			},
			{
				PaperSizeEnum.P_4x6_Ultra_Res,
				new PaperSize("4\" x 6\" Ultra Res", 2400, 3600)
			},
			{
				PaperSizeEnum.P_8x6_Ultra_Res,
				new PaperSize("8\" x 6\" Ultra Res", 4800, 3600)
			},
			{
				PaperSizeEnum.P_6x8_Ultra_Res,
				new PaperSize("6\" x 8\" Ultra Res", 3600, 4800)
			},
			{
				PaperSizeEnum.P_5x7_Ultra_Res,
				new PaperSize("5\" x 7\" Ultra Res", 3000, 4200)
			},
			{
				PaperSizeEnum.P_7x5_Ultra_Res,
				new PaperSize("7\" x 5\" Ultra Res", 4200, 3000)
			},
			{
				PaperSizeEnum.P_1_1_Ultra_Res,
				new PaperSize("1:1 Ultra Res", 2560, 2560)
			},
			{
				PaperSizeEnum.P_9x16_Ultra_HD,
				new PaperSize("9:16 Ultra HD", 2160, 3840)
			},
			{
				PaperSizeEnum.P_16x9_Ultra_HD,
				new PaperSize("16:9 Ultra HD", 3840, 2160)
			}
		};
	}
}
