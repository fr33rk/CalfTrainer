using System;

namespace CalfTrainer.Android.BusinessLogic
{
	public class Exercise
	{
		#region Definitions

		private readonly ExerciseConfiguration mConfiguration;
		private SubExercise mCurrentSubExercise;
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
			mCurrentSubExercise = SubExercise.Undefined;
		}

		#endregion Constructor(s)

		#region Current counters

		public uint LongLeftCount { get; private set; }
		public uint ShortLeftCount { get; private set; }
		public uint LongRightCount { get; private set; }
		public uint ShortRightCount { get; private set; }

		public uint RemainingPreparationTime { get; private set; }
		public uint RemainingSubExerciseTime { get; private set; }

		#endregion Current counters

		#region Remaining total time

		public TimeSpan RemainingTotalTime => TimeSpan.FromSeconds(
			(mConfiguration.DurationPerStance + mConfiguration.PreparationDuration + 1) // + 1 for switching
			* (LongLeftCount + LongRightCount + ShortLeftCount + ShortRightCount - 1) // -1 because the time of the last one depends on the remaining time.
			+ RemainingPreparationTime + RemainingSubExerciseTime
			+ 1); // First tick is used to start.

		#endregion Remaining total time

		#region Event ActiveSubExerciseChanged

		private void SignalActiveSubExerciseChanged(SubExercise newSubExercise)
		{
			ActiveSubExerciseChanged?.Invoke(this, new ActiveSubExerciseChangedEventArgs(mCurrentSubExercise, newSubExercise));
		}

		public event EventHandler<ActiveSubExerciseChangedEventArgs> ActiveSubExerciseChanged;

		#endregion Event ActiveSubExerciseChanged

		#region Event IsDone

		private void SignalIsDone()
		{
			IsDone?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler IsDone;

		#endregion Event IsDone

		public void HandleNextClockTick()
		{
			if (!mIsDone)
			{
				if (mCurrentSubExercise == SubExercise.Undefined)
				{
					NextSubExercise();
					return;
				}

				if (RemainingPreparationTime > 0)
				{
					RemainingPreparationTime--;
					return;
				}

				if (RemainingSubExerciseTime > 0)
				{
					RemainingSubExerciseTime--;
				}
				else
				{
					NextSubExercise();
					if (mCurrentSubExercise == SubExercise.Undefined)
					{
						mIsDone = true;
						SignalIsDone();
					}
				}
			}
		}

		#region Helper methods

		private void NextSubExercise()
		{
			SubExercise newExercise;

			switch (mCurrentSubExercise)
			{
				case SubExercise.Undefined:
					newExercise = SubExercise.LongLeft;
					break;

				case SubExercise.LongLeft:
					newExercise = SubExercise.LongRight;
					break;

				case SubExercise.LongRight:
					newExercise = GetCountForSubExercise(SubExercise.LongLeft) > 0 ? SubExercise.LongLeft : SubExercise.ShortLeft;
					break;

				case SubExercise.ShortLeft:
					newExercise = SubExercise.ShortRight;
					break;

				case SubExercise.ShortRight:
					newExercise = GetCountForSubExercise(SubExercise.ShortLeft) > 0 ? SubExercise.ShortLeft : SubExercise.Undefined;
					break;

				default:
					throw new ArgumentOutOfRangeException();
			}

			DecreaseSubExerciseCounter(mCurrentSubExercise);
			SignalActiveSubExerciseChanged(newExercise);
			mCurrentSubExercise = newExercise;
			RemainingPreparationTime = mConfiguration.PreparationDuration;
			RemainingSubExerciseTime = mConfiguration.DurationPerStance;
		}

		private uint GetCountForSubExercise(SubExercise subExercise)
		{
			switch (subExercise)
			{
				case SubExercise.Undefined:
					return 0;

				case SubExercise.LongLeft:
					return LongLeftCount;

				case SubExercise.ShortLeft:
					return ShortLeftCount;

				case SubExercise.LongRight:
					return LongRightCount;

				case SubExercise.ShortRight:
					return ShortRightCount;

				default:
					throw new ArgumentOutOfRangeException(nameof(subExercise), subExercise, null);
			}
		}

		private void DecreaseSubExerciseCounter(SubExercise subExercise)
		{
			switch (subExercise)
			{
				case SubExercise.Undefined:
					break;

				case SubExercise.LongLeft:
					LongLeftCount--;
					break;

				case SubExercise.ShortLeft:
					ShortLeftCount--;
					break;

				case SubExercise.LongRight:
					LongRightCount--;
					break;

				case SubExercise.ShortRight:
					ShortRightCount--;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(subExercise), subExercise, null);
			}
		}

		#endregion Helper methods
	}
}