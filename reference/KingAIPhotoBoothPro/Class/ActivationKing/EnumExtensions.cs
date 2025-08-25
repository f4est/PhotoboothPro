using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C3 RID: 195
	public static class EnumExtensions
	{
		// Token: 0x06000A95 RID: 2709 RVA: 0x0003CD8C File Offset: 0x0003AF8C
		public static string GetDisplayName(this Enum enumValue)
		{
			Type enumType = enumValue.GetType();
			MemberInfo[] member = enumType.GetMember(enumValue.ToString());
			if (member.Length == 0)
			{
				return "Make Selection...";
			}
			DisplayAttribute displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
			if (displayAttribute == null)
			{
				return enumValue.ToString();
			}
			return displayAttribute.Name;
		}
	}
}
