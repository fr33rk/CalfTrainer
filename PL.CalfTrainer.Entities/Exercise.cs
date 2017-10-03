using System;

namespace PL.CalfTrainer.Entities
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

			Reset();
		}

		#endregion Constructor(s)

		public void Reset()
		{
			LongLeftCount = mConfiguration.NoOfRepetitions;
			ShortLeftCount = mConfiguration.NoOfRepetitions;
			LongRightCount = mConfiguration.NoOfRepetitions;
			ShortRightCount = mConfiguration.NoOfRepetitions;
			RemainingPreparationTime = mConfiguration.PreparationDuration;
			RemainingSubExerciseTime = mConfiguration.DurationPerStance;
			CurrentSubExercise = SubExercise.Undefined;
		}

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

		#region Equals

		protected bool Equals(Exercise other)
		{
			return LongLeftCount == other.LongLeftCount
				   && ShortLeftCount == other.ShortLeftCount
				   && LongRightCount == other.LongRightCount
				   && ShortRightCount == other.ShortRightCount
				   && RemainingPreparationTime == other.RemainingPreparationTime
				   && RemainingSubExerciseTime == other.RemainingSubExerciseTime
				   && CurrentSubExercise == other.CurrentSubExercise;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((Exercise)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (int)LongLeftCount;
				hashCode = (hashCode * 397) ^ (int)ShortLeftCount;
				hashCode = (hashCode * 397) ^ (int)LongRightCount;
				hashCode = (hashCode * 397) ^ (int)ShortRightCount;
				hashCode = (hashCode * 397) ^ (int)RemainingPreparationTime;
				hashCode = (hashCode * 397) ^ (int)RemainingSubExerciseTime;
				hashCode = (hashCode * 397) ^ (int)CurrentSubExercise;
				return hashCode;
			}
		}

		#endregion Equals
	}
}