using System;
using System.Timers;

namespace CalfTrainer.Android.BuisinessLogic
{
	public enum SubExercise
	{
		Undefined,
		LongLeft,
		ShortLeft,
		LongRight,
		ShortRight
	}

	public class ActiveSubExcersizeChangedEventArgs : EventArgs
	{
		public ActiveSubExcersizeChangedEventArgs(SubExercise oldSubExercise, SubExercise newSubExercise)
		{
			OldSubExercise = oldSubExercise;
			NewSubExercise = newSubExercise;
		}

		public SubExercise OldSubExercise { get; }
		public SubExercise NewSubExercise { get; }
	}

	public class ExerciseService
	{
		private Exercise mExercise;
		private Timer mTimer;

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

		private void ExerciseOnIsDone(object sender, EventArgs eventArgs)
		{
			mTimer.Stop();
			ExerciseIsDone?.Invoke(sender, eventArgs);
		}

		public void Start()
		{
			mTimer.Start();
		}

		private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
		{
			mExercise.Tick();
			SignalExerciseChanged();
		}

		public event EventHandler<ActiveSubExcersizeChangedEventArgs> ActiveSubExerciseChanged;

		#region Event ExersizeChanged

		private void SignalExerciseChanged()
		{
			ExerciseChanged?.Invoke(this, new ExerciseChangedEventArgs(mExercise));
		}

		public event EventHandler<ExerciseChangedEventArgs> ExerciseChanged;

		#endregion Event ExersizeChanged


		
		public event EventHandler ExerciseIsDone;
	}
}