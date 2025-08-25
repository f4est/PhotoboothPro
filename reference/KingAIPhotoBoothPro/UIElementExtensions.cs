using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

// Token: 0x02000005 RID: 5
public static class UIElementExtensions
{
	// Token: 0x06000013 RID: 19 RVA: 0x00002394 File Offset: 0x00000594
	public static void BackFadeIn(this Button element, TimeSpan duration, TimeSpan delay = default(TimeSpan))
	{
		UIElementExtensions.<>c__DisplayClass0_0 CS$<>8__locals1 = new UIElementExtensions.<>c__DisplayClass0_0();
		CS$<>8__locals1.delay = delay;
		CS$<>8__locals1.element = element;
		CS$<>8__locals1.duration = duration;
		Task.Run(delegate()
		{
			UIElementExtensions.<>c__DisplayClass0_0.<<BackFadeIn>b__0>d <<BackFadeIn>b__0>d;
			<<BackFadeIn>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<BackFadeIn>b__0>d.<>4__this = CS$<>8__locals1;
			<<BackFadeIn>b__0>d.<>1__state = -1;
			<<BackFadeIn>b__0>d.<>t__builder.Start<UIElementExtensions.<>c__DisplayClass0_0.<<BackFadeIn>b__0>d>(ref <<BackFadeIn>b__0>d);
			return <<BackFadeIn>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000014 RID: 20 RVA: 0x000023D0 File Offset: 0x000005D0
	public static void BackFadeOut(this Button element, TimeSpan duration, TimeSpan delay = default(TimeSpan))
	{
		UIElementExtensions.<>c__DisplayClass1_0 CS$<>8__locals1 = new UIElementExtensions.<>c__DisplayClass1_0();
		CS$<>8__locals1.delay = delay;
		CS$<>8__locals1.element = element;
		CS$<>8__locals1.duration = duration;
		Task.Run(delegate()
		{
			UIElementExtensions.<>c__DisplayClass1_0.<<BackFadeOut>b__0>d <<BackFadeOut>b__0>d;
			<<BackFadeOut>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<BackFadeOut>b__0>d.<>4__this = CS$<>8__locals1;
			<<BackFadeOut>b__0>d.<>1__state = -1;
			<<BackFadeOut>b__0>d.<>t__builder.Start<UIElementExtensions.<>c__DisplayClass1_0.<<BackFadeOut>b__0>d>(ref <<BackFadeOut>b__0>d);
			return <<BackFadeOut>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000240C File Offset: 0x0000060C
	public static void HalfFadeIn(this UIElement element, TimeSpan duration, TimeSpan delay = default(TimeSpan))
	{
		UIElementExtensions.<>c__DisplayClass2_0 CS$<>8__locals1 = new UIElementExtensions.<>c__DisplayClass2_0();
		CS$<>8__locals1.delay = delay;
		CS$<>8__locals1.element = element;
		CS$<>8__locals1.duration = duration;
		Task.Run(delegate()
		{
			UIElementExtensions.<>c__DisplayClass2_0.<<HalfFadeIn>b__0>d <<HalfFadeIn>b__0>d;
			<<HalfFadeIn>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<HalfFadeIn>b__0>d.<>4__this = CS$<>8__locals1;
			<<HalfFadeIn>b__0>d.<>1__state = -1;
			<<HalfFadeIn>b__0>d.<>t__builder.Start<UIElementExtensions.<>c__DisplayClass2_0.<<HalfFadeIn>b__0>d>(ref <<HalfFadeIn>b__0>d);
			return <<HalfFadeIn>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002448 File Offset: 0x00000648
	public static void HalfFadeOut(this UIElement element, TimeSpan duration, TimeSpan delay = default(TimeSpan))
	{
		UIElementExtensions.<>c__DisplayClass3_0 CS$<>8__locals1 = new UIElementExtensions.<>c__DisplayClass3_0();
		CS$<>8__locals1.delay = delay;
		CS$<>8__locals1.element = element;
		CS$<>8__locals1.duration = duration;
		Task.Run(delegate()
		{
			UIElementExtensions.<>c__DisplayClass3_0.<<HalfFadeOut>b__0>d <<HalfFadeOut>b__0>d;
			<<HalfFadeOut>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<HalfFadeOut>b__0>d.<>4__this = CS$<>8__locals1;
			<<HalfFadeOut>b__0>d.<>1__state = -1;
			<<HalfFadeOut>b__0>d.<>t__builder.Start<UIElementExtensions.<>c__DisplayClass3_0.<<HalfFadeOut>b__0>d>(ref <<HalfFadeOut>b__0>d);
			return <<HalfFadeOut>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002484 File Offset: 0x00000684
	public static void FadeIn(this UIElement element, TimeSpan duration, TimeSpan delay = default(TimeSpan))
	{
		UIElementExtensions.<>c__DisplayClass4_0 CS$<>8__locals1 = new UIElementExtensions.<>c__DisplayClass4_0();
		CS$<>8__locals1.delay = delay;
		CS$<>8__locals1.element = element;
		CS$<>8__locals1.duration = duration;
		Task.Run(delegate()
		{
			UIElementExtensions.<>c__DisplayClass4_0.<<FadeIn>b__0>d <<FadeIn>b__0>d;
			<<FadeIn>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<FadeIn>b__0>d.<>4__this = CS$<>8__locals1;
			<<FadeIn>b__0>d.<>1__state = -1;
			<<FadeIn>b__0>d.<>t__builder.Start<UIElementExtensions.<>c__DisplayClass4_0.<<FadeIn>b__0>d>(ref <<FadeIn>b__0>d);
			return <<FadeIn>b__0>d.<>t__builder.Task;
		});
	}

	// Token: 0x06000018 RID: 24 RVA: 0x000024C0 File Offset: 0x000006C0
	public static void FadeOut(this UIElement element, TimeSpan duration, TimeSpan delay = default(TimeSpan))
	{
		UIElementExtensions.<>c__DisplayClass5_0 CS$<>8__locals1 = new UIElementExtensions.<>c__DisplayClass5_0();
		CS$<>8__locals1.delay = delay;
		CS$<>8__locals1.element = element;
		CS$<>8__locals1.duration = duration;
		Task.Run(delegate()
		{
			UIElementExtensions.<>c__DisplayClass5_0.<<FadeOut>b__0>d <<FadeOut>b__0>d;
			<<FadeOut>b__0>d.<>t__builder = AsyncTaskMethodBuilder.Create();
			<<FadeOut>b__0>d.<>4__this = CS$<>8__locals1;
			<<FadeOut>b__0>d.<>1__state = -1;
			<<FadeOut>b__0>d.<>t__builder.Start<UIElementExtensions.<>c__DisplayClass5_0.<<FadeOut>b__0>d>(ref <<FadeOut>b__0>d);
			return <<FadeOut>b__0>d.<>t__builder.Task;
		});
	}
}
