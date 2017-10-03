using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Infrastructure.EventArgs
{
	public class ExerciseChangedEventArgs : System.EventArgs
	{
		public ExerciseChangedEventArgs(Exercise exercise)
		{
			Exercise = exercise;
		}

		public Exercise Exercise { get; }
	}
}