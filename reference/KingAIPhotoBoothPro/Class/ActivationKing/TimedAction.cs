using System;
using System.Windows.Threading;

namespace KingAIPhotoBoothPro.Class.ActivationKing
{
	// Token: 0x020000C6 RID: 198
	public static class TimedAction
	{
		// Token: 0x06000AE3 RID: 2787 RVA: 0x0003FD58 File Offset: 0x0003DF58
		public static void ExecuteWithDelay(Action action, TimeSpan delay)
		{
			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = delay;
			timer.Tag = action;
			timer.Tick += TimedAction.timer_Tick;
			timer.Start();
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x0003FD94 File Offset: 0x0003DF94
		private static void timer_Tick(object sender, EventArgs e)
		{
			DispatcherTimer timer = (DispatcherTimer)sender;
			Action action = (Action)timer.Tag;
			action();
			timer.Stop();
		}
	}
}
