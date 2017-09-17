using System;

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

		public void Prepare()
		{
			var config = new ExerciseConfiguration();
			mExercise = new Exercise(config);

			SignalExerciseChanged();

			ActiveSubExerciseChanged?.Invoke(this, new ActiveSubExcersizeChangedEventArgs(SubExercise.Undefined, SubExercise.LongLeft));
		}

		public event EventHandler<ActiveSubExcersizeChangedEventArgs> ActiveSubExerciseChanged;

		#region Event ExersizeChanged

		private void SignalExerciseChanged()
		{
			ExerciseChanged?.Invoke(this, new ExerciseChangedEventArgs(mExercise));
		}

		public event EventHandler<ExerciseChangedEventArgs> ExerciseChanged;

		#endregion Event ExersizeChanged
	}
}