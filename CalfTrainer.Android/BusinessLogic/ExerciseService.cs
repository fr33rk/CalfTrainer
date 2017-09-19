using System;
using System.Timers;

namespace CalfTrainer.Android.BusinessLogic
{
	public enum SubExercise
	{
		Undefined,
		LongLeft,
		ShortLeft,
		LongRight,
		ShortRight
	}

	public class ExerciseService
	{
		#region Definitions

		private Exercise mExercise;
		private Timer mTimer;

		#endregion Definitions

		public ExerciseService()
		{
			
		}

		public void Prepare()
		{
			var config = new ExerciseConfiguration();
			mExercise = new Exercise(config);
			mExercise.ActiveSubExerciseChanged += (s, e) => ActiveSubExerciseChanged?.Invoke(s, e);
			mExercise.IsDone += ExerciseOnIsDone;

			mTimer = new Timer(1000);
			mTimer.Elapsed += TimerOnElapsed;

			SignalExerciseChanged();
		}

		public void Start()
		{
			mTimer.Start();
		}

		public void Pause()
		{
			mTimer.Stop();
		}

		public void Stop()
		{
			mTimer.Stop();
			Prepare();
			ExerciseIsDone?.Invoke(this, EventArgs.Empty);
		}

		public bool IsRunning => mTimer.Enabled;

		#region Exercise event handlers

		private void ExerciseOnIsDone(object sender, EventArgs eventArgs)
		{
			Stop();
		}

		#endregion Exercise event handlers

		#region Timer event

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			mExercise.HandleNextClockTick();
			SignalExerciseChanged();
		}

		#endregion Timer event

		#region event ActiveSubExerciseChanged

		public event EventHandler<ActiveSubExerciseChangedEventArgs> ActiveSubExerciseChanged;

		#endregion event ActiveSubExerciseChanged

		#region Event ExersizeChanged

		private void SignalExerciseChanged()
		{
			ExerciseChanged?.Invoke(this, new ExerciseChangedEventArgs(mExercise));
		}

		public event EventHandler<ExerciseChangedEventArgs> ExerciseChanged;

		#endregion Event ExersizeChanged

		#region Event ExerciseIsDone

		public event EventHandler ExerciseIsDone;

		#endregion Event ExerciseIsDone
	}
}