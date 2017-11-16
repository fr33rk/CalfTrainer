using PL.CalfTrainer.Infrastructure.Enums;

namespace PL.CalfTrainer.Infrastructure.EventArgs
{
	public class ExerciseServiceStateChangedArgs : System.EventArgs
	{
		public ExerciseServiceStateChangedArgs(ExerciseServiceState newState)
		{
			NewState = newState;
		}

		public ExerciseServiceState NewState { get; }
	}
}