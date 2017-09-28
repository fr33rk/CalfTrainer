using System;
using System.Threading;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Services
{
	public class TimerService : ITimerService
	{
		#region Definitions

		private Timer mWrappedTimer;
		private object mTimerElapsedMonitor;

		#endregion Definitions

		#region Event Elapsed

		private void SignalElapsed()
		{
			Elapsed?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler Elapsed;

		#endregion Event Elapsed

		#region Start and stopping

		public void Start(int timerIntervalMs)
		{
			if (mWrappedTimer == null)
			{
				CreateAndStartNewTimer(timerIntervalMs);
			}
			else
			{
				throw new InvalidOperationException("Timer needs to be stopped first");
			}
		}

		public void Pause()
		{
			Monitor.Enter(mTimerElapsedMonitor);
			IsRunning = false;
		}

		public void Resume()
		{
			Monitor.Exit(mTimerElapsedMonitor);
			IsRunning = true;
		}

		public void Stop()
		{
			if (mWrappedTimer != null)
				StopAndDisposeTimer();
			else 
				throw new InvalidOperationException("Timer is stopped before it was started");
		}

		public bool IsRunning { get; private set; }

		#endregion Start and stopping

		private void CreateAndStartNewTimer(int timerIntervalMs)
		{
			mTimerElapsedMonitor = new object();
			mWrappedTimer = new Timer(OnTimerElapsed, null, timerIntervalMs, timerIntervalMs);
			IsRunning = true;
		}

		private void StopAndDisposeTimer()
		{
			mWrappedTimer.Dispose();
			mWrappedTimer = null;
			IsRunning = false;
		}

		private void OnTimerElapsed(object state)
		{
			if (Monitor.TryEnter(mTimerElapsedMonitor))
			{
				try
				{
					SignalElapsed();
				}
				finally
				{
					Monitor.Exit(mTimerElapsedMonitor);
				}
			}
		}
	}
}