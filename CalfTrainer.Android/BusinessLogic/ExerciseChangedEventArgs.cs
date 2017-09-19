using System;

namespace CalfTrainer.Android.BusinessLogic
{
	public class ExerciseChangedEventArgs : EventArgs
	{
		public ExerciseChangedEventArgs(Exercise exercise)
		{
			Exercise = exercise;
		}

		public Exercise Exercise { get; }
	}
}