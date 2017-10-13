using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PL.CalfTrainer.Entities
{
	public class Exercise
	{
		#region Definitions

		private readonly ExerciseConfiguration mConfiguration;
		private bool mIsDone;
		private const char KEY_VALUE_SEPARATOR = '=';
		private const string KEY_VALUE_PAIR_SEPARATOR = ";";

		#endregion Definitions

		#region Constructor(s)

		public Exercise(ExerciseConfiguration configuration)
		{
			mConfiguration = configuration;

			Reset();
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
			catch (Exception e)
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

		public int LongLeftCount { get; internal set; }
		public int ShortLeftCount { get; internal set; }
		public int LongRightCount { get; internal set; }
		public int ShortRightCount { get; internal set; }

		public int RemainingPreparationTime { get; internal set; }
		public int RemainingSubExerciseTime { get; internal set; }

		#endregion Current counters

		public SubExercise CurrentSubExercise { get; internal set; }

		#region Remaining total time

		public TimeSpan RemainingTotalTime => TimeSpan.FromSeconds(
			(mConfiguration.DurationPerStance + mConfiguration.PreparationDuration + 1) // + 1 for switching
			* (LongLeftCount + LongRightCount + ShortLeftCount + ShortRightCount - 1) // -1 because the time of the last one depends on the remaining time.
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

			return string.Join(KEY_VALUE_PAIR_SEPARATOR, keyValuePairStrings);
		}

		private static string CreateKeyValuePairString(string key, int value)
		{
			return $"{key}{KEY_VALUE_SEPARATOR}{value}";
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
			var keyValuePair = keyValuePairString.Split(KEY_VALUE_SEPARATOR);
			dictionary[keyValuePair[0]] = Convert.ToInt32(keyValuePair[1]);
		}

		#endregion To and from String
	}
}