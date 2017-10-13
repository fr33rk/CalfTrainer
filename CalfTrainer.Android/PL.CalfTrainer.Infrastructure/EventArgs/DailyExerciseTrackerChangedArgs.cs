using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Infrastructure.EventArgs
{
	public class DailyExerciseTrackerChangedArgs : System.EventArgs
	{
		public DailyExerciseTrackerChangedArgs(DailyExerciseTracker dailyExerciseTracker)
		{
			DailyExerciseTracker = dailyExerciseTracker;
		}

		public DailyExerciseTracker DailyExerciseTracker { get; }
	}
}