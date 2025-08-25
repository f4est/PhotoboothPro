using System;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C4 RID: 196
	public static class BehaviorExtensions
	{
		// Token: 0x06000A96 RID: 2710 RVA: 0x0003CDD0 File Offset: 0x0003AFD0
		public static T GetBehavior<T>(this DependencyObject dependencyObject) where T : Behavior
		{
			BehaviorCollection behaviors = Interaction.GetBehaviors(dependencyObject);
			return behaviors.FirstOrDefault((Behavior b) => b is T) as T;
		}
	}
}
