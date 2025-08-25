using System;
using System.Collections.Generic;
using System.Text;

namespace KingAIPhotoBoothPro.Class
{
	// Token: 0x020000B9 RID: 185
	public class ParseGif
	{
		// Token: 0x06000A44 RID: 2628 RVA: 0x0003B690 File Offset: 0x00039890
		public List<int> ParseGifDataStream(byte[] gifData, int offset)
		{
			this.Delays.Clear();
			offset = this.ParseHeader(ref gifData, offset);
			for (offset = this.ParseLogicalScreen(ref gifData, offset); offset != -1; offset = this.ParseBlock(ref gifData, offset))
			{
			}
			return this.Delays;
		}

		// Token: 0x06000A45 RID: 2629 RVA: 0x0003B6CC File Offset: 0x000398CC
		private int ParseHeader(ref byte[] gifData, int offset)
		{
			string str = Encoding.ASCII.GetString(gifData, offset, 3);
			if (str != "GIF")
			{
				throw new FormatException("Not a proper GIF file: missing GIF header");
			}
			return 6;
		}

		// Token: 0x06000A46 RID: 2630 RVA: 0x0003B704 File Offset: 0x00039904
		private int ParseLogicalScreen(ref byte[] gifData, int offset)
		{
			int _logicalWidth = (int)BitConverter.ToUInt16(gifData, offset);
			int _logicalHeight = (int)BitConverter.ToUInt16(gifData, offset + 2);
			byte packedField = gifData[offset + 4];
			bool hasGlobalColorTable = (packedField & 128) > 0;
			int currentIndex = offset + 7;
			if (hasGlobalColorTable)
			{
				int colorTableLength = (int)(packedField & 7);
				colorTableLength = (int)Math.Pow(2.0, (double)(colorTableLength + 1)) * 3;
				currentIndex += colorTableLength;
			}
			return currentIndex;
		}

		// Token: 0x06000A47 RID: 2631 RVA: 0x0003B768 File Offset: 0x00039968
		private int ParseBlock(ref byte[] gifData, int offset)
		{
			byte b = gifData[offset];
			if (b != 33)
			{
				if (b == 44)
				{
					offset = this.ParseGraphicBlock(ref gifData, offset);
					return offset;
				}
				if (b != 59)
				{
					throw new FormatException("GIF format incorrect: missing graphic block or special-purpose block. ");
				}
				return -1;
			}
			else
			{
				if (gifData[offset + 1] == 249)
				{
					return this.ParseGraphicControlExtension(ref gifData, offset);
				}
				return this.ParseExtensionBlock(ref gifData, offset);
			}
		}

		// Token: 0x06000A48 RID: 2632 RVA: 0x0003B7C4 File Offset: 0x000399C4
		private int ParseGraphicControlExtension(ref byte[] gifData, int offset)
		{
			int length = (int)gifData[offset + 2];
			int returnOffset = offset + length + 2 + 1;
			byte packedField = gifData[offset + 3];
			int delay = (int)BitConverter.ToUInt16(gifData, offset + 4);
			int delayTime = (delay < 10) ? 10 : delay;
			this.Delays.Add(delayTime);
			while (gifData[returnOffset] != 0)
			{
				returnOffset = returnOffset + (int)gifData[returnOffset] + 1;
			}
			return returnOffset + 1;
		}

		// Token: 0x06000A49 RID: 2633 RVA: 0x0003B824 File Offset: 0x00039A24
		private int ParseExtensionBlock(ref byte[] gifData, int offset)
		{
			int length = (int)gifData[offset + 2];
			int returnOffset = offset + length + 2 + 1;
			if (gifData[offset + 1] == 255 && length > 10)
			{
				string netscape = Encoding.ASCII.GetString(gifData, offset + 3, 8);
				if (netscape == "NETSCAPE")
				{
					int _numberOfLoops = (int)BitConverter.ToUInt16(gifData, offset + 16);
					if (_numberOfLoops > 0)
					{
						_numberOfLoops++;
					}
				}
			}
			while (gifData[returnOffset] != 0)
			{
				returnOffset = returnOffset + (int)gifData[returnOffset] + 1;
			}
			return returnOffset + 1;
		}

		// Token: 0x06000A4A RID: 2634 RVA: 0x0003B89C File Offset: 0x00039A9C
		private int ParseGraphicBlock(ref byte[] gifData, int offset)
		{
			byte packedField = gifData[offset + 9];
			bool hasLocalColorTable = (packedField & 128) > 0;
			int currentIndex = offset + 9;
			if (hasLocalColorTable)
			{
				int colorTableLength = (int)(packedField & 7);
				colorTableLength = (int)Math.Pow(2.0, (double)(colorTableLength + 1)) * 3;
				currentIndex += colorTableLength;
			}
			currentIndex++;
			currentIndex++;
			while (gifData[currentIndex] != 0)
			{
				int length = (int)gifData[currentIndex];
				currentIndex += (int)gifData[currentIndex];
				currentIndex++;
			}
			return currentIndex + 1;
		}

		// Token: 0x04000A13 RID: 2579
		private List<int> Delays = new List<int>();
	}
}
