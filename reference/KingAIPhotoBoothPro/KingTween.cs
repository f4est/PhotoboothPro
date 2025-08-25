using System;
using System.Windows.Threading;

// Token: 0x02000008 RID: 8
public class KingTween
{
	// Token: 0x0600001C RID: 28 RVA: 0x000025F4 File Offset: 0x000007F4
	public KingTween(double startValue, double endValue, TimeSpan duration, Action<double> updateCallback)
	{
		this._startValue = startValue;
		this._endValue = endValue;
		this._duration = duration;
		this._updateCallback = updateCallback;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x0000261C File Offset: 0x0000081C
	public void Start()
	{
		this._startTime = DateTime.Now;
		this._timer = new DispatcherTimer
		{
			Interval = TimeSpan.FromMilliseconds(16.0)
		};
		this._timer.Tick += this.OnTimerTick;
		this._timer.Start();
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002678 File Offset: 0x00000878
	private void OnTimerTick(object sender, EventArgs e)
	{
		double progress = Math.Min((DateTime.Now - this._startTime).TotalMilliseconds / this._duration.TotalMilliseconds, 1.0);
		double currentValue = this._startValue + (this._endValue - this._startValue) * progress;
		this._updateCallback(currentValue);
		if (progress >= 1.0)
		{
			this._timer.Stop();
		}
	}

	// Token: 0x04000007 RID: 7
	private DispatcherTimer _timer;

	// Token: 0x04000008 RID: 8
	private DateTime _startTime;

	// Token: 0x04000009 RID: 9
	private double _startValue;

	// Token: 0x0400000A RID: 10
	private double _endValue;

	// Token: 0x0400000B RID: 11
	private TimeSpan _duration;

	// Token: 0x0400000C RID: 12
	private Action<double> _updateCallback;
}
