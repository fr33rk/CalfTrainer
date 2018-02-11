using System;
using System.Collections.Generic;

namespace PL.CalfTrainer.Entities
{
	public class Exercise
	{
		#region Definitions

		private readonly ExerciseConfiguration mConfiguration;
		private const char KeyValueSeparator = '=';
		private const string KeyValuePairSeparator = ";";
		private TimeSpan mTotalTime;

		#endregion Definitions

		#region Constructor(s)

		public Exercise(ExerciseConfiguration configuration)
		{
			mConfiguration = configuration;
			CalculateTotalTime();
			Reset();
		}

		private void CalculateTotalTime()
		{
			mTotalTime = TimeSpan.FromSeconds(
				(mConfiguration.DurationPerStance + mConfiguration.PreparationDuration + 1) // + 1 for switching
				* (mConfiguration.NoOfRepetitions * 4) // -1 because the time of the last one depends on the remaining time.
				+ 1); // First tick is used to start.
		}

		public static Exercise ExerciseFromConfiguration(ExerciseConfiguration configuration)
		{
			return new Exercise(configuration);
		}

		public static Exercise ExerciseFromString(string exerciseAsString, ExerciseConfiguration configurationUsedForReset)
		{
			try
			{
				var keyValuePairFromString = GetNameValuePairsFromString(exerciseAsString);

				return new Exercise(configurationUsedForReset)
				{
					LongLeftCount = keyValuePairFromString[nameof(LongLeftCount)],
					LongRightCount = keyValuePairFromString[nameof(LongRightCount)],
					ShortLeftCount = keyValuePairFromString[nameof(ShortLeftCount)],
					ShortRightCount = keyValuePairFromString[nameof(ShortRightCount)],
					RemainingPreparationTime = keyValuePairFromString[nameof(RemainingPreparationTime)],
					RemainingSubExerciseTime = keyValuePairFromString[nameof(RemainingSubExerciseTime)],
					CurrentSubExercise = (SubExercise)keyValuePairFromString[nameof(CurrentSubExercise)]
				};
			}
			catch (Exception)
			{
				return ExerciseFromConfiguration(configurationUsedForReset);
			}
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

		public int LongLeftCount { get; set; }
		public int ShortLeftCount { get; set; }
		public int LongRightCount { get; set; }
		public int ShortRightCount { get; set; }

		public int RemainingPreparationTime { get; set; }
		public int RemainingSubExerciseTime { get; set; }

		#endregion Current counters

		public SubExercise CurrentSubExercise { get; set; }

		#region Remaining total time

		public TimeSpan RemainingTotalTime => TimeSpan.FromSeconds(
			(mConfiguration.DurationPerStance + mConfiguration.PreparationDuration + 1) // + 1 for switching
			* Math.Max(0, (LongLeftCount + LongRightCount + ShortLeftCount + ShortRightCount - 1)) // -1 because the time of the last one depends on the remaining time.
			+ RemainingPreparationTime + RemainingSubExerciseTime
			+ 1); // First tick is used to start.

		#endregion Remaining total time

		#region To and from String

		public override string ToString()
		{
			var keyValuePairStrings = new List<string>
			{
				CreateKeyValuePairString(nameof(LongLeftCount), LongLeftCount),
				CreateKeyValuePairString(nameof(LongRightCount), LongRightCount),
				CreateKeyValuePairString(nameof(ShortLeftCount), ShortLeftCount),
				CreateKeyValuePairString(nameof(ShortRightCount), ShortRightCount),
				CreateKeyValuePairString(nameof(RemainingPreparationTime), RemainingPreparationTime),
				CreateKeyValuePairString(nameof(RemainingSubExerciseTime), RemainingSubExerciseTime),
				CreateKeyValuePairString(nameof(CurrentSubExercise), (int)CurrentSubExercise)
			};

			return string.Join(KeyValuePairSeparator, keyValuePairStrings);
		}

		private static string CreateKeyValuePairString(string key, int value)
		{
			return $"{key}{KeyValueSeparator}{value}";
		}

		private static Dictionary<string, int> GetNameValuePairsFromString(string exerciseAsString)
		{
			var retValue = new Dictionary<string, int>();

			foreach (var s in exerciseAsString.Split(';'))
			{
				AddNameValueToDictionary(retValue, s);
			}

			return retValue;
		}

		private static void AddNameValueToDictionary(IDictionary<string, int> dictionary, string keyValuePairString)
		{
			var keyValuePair = keyValuePairString.Split(KeyValueSeparator);
			dictionary[keyValuePair[0]] = Convert.ToInt32(keyValuePair[1]);
		}

		#endregion To and from String

		public virtual int PercentageCompleted => 100 - Convert.ToInt32(RemainingTotalTime.TotalSeconds / mTotalTime.TotalSeconds * 100);

		public bool IsDone => PercentageCompleted == 100;
	}
}