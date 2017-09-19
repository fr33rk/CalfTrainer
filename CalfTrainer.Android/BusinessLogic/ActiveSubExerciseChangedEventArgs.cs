using System;

namespace CalfTrainer.Android.BusinessLogic
{
	public class ActiveSubExerciseChangedEventArgs : EventArgs
	{
		public ActiveSubExerciseChangedEventArgs(SubExercise oldSubExercise, SubExercise newSubExercise)
		{
			OldSubExercise = oldSubExercise;
			NewSubExercise = newSubExercise;
		}

		public SubExercise OldSubExercise { get; }
		public SubExercise NewSubExercise { get; }
	}
}