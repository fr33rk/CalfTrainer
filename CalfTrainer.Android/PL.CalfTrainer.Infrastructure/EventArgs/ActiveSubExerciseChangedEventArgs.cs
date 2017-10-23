using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Infrastructure.EventArgs
{
	public class ActiveSubExerciseChangedEventArgs : System.EventArgs
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