using System;
using System.Globalization;
using Newtonsoft.Json;

namespace PL.CalfTrainer.Business.Entities
{
	public class Exercise
	{
		#region Definitions

		private readonly ExerciseConfiguration mConfiguration;
		private bool mIsDone;

		#endregion Definitions

		#region Constructor(s)

		public Exercise(ExerciseConfiguration configuration)
		{
			mConfiguration = configuration;

			LongLeftCount = mConfiguration.NoOfRepetitions;
			ShortLeftCount = mConfiguration.NoOfRepetitions;
			LongRightCount = mConfiguration.NoOfRepetitions;
			ShortRightCount = mConfiguration.NoOfRepetitions;
			RemainingPreparationTime = mConfiguration.PreparationDuration;
			RemainingSubExerciseTime = mConfiguration.DurationPerStance;
			CurrentSubExercise = SubExercise.Undefined;
		}

		#endregion Constructor(s)

		#region Current counters

		public uint LongLeftCount { get; internal set; }
		public uint ShortLeftCount { get; internal set; }
		public uint LongRightCount { get; internal set; }
		public uint ShortRightCount { get; internal set; }

		public uint RemainingPreparationTime { get; internal set; }
		public uint RemainingSubExerciseTime { get; internal set; }

		#endregion Current counters

		public SubExercise CurrentSubExercise { get; internal set; }

		#region Remaining total time

		public TimeSpan RemainingTotalTime => TimeSpan.FromSeconds(
			(mConfiguration.DurationPerStance + mConfiguration.PreparationDuration + 1) // + 1 for switching
			* (LongLeftCount + LongRightCount + ShortLeftCount + ShortRightCount - 1) // -1 because the time of the last one depends on the remaining time.
			+ RemainingPreparationTime + RemainingSubExerciseTime
			+ 1); // First tick is used to start.

		#endregion Remaining total time
	}
}