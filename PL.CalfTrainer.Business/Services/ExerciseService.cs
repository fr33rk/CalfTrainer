using System;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.EventArgs;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Services
{
	public class ExerciseService
	{
		private Exercise mExercise;
		private ExerciseConfiguration mExerciseConfiguration;
		private ITimerService mTimerService;
		private const int TIMER_INTERVAL_MS = 1000;

		protected ExerciseService(Exercise exercise, ExerciseConfiguration configuration, ITimerService timerService)
		{
			mExercise = exercise;
			mExerciseConfiguration = configuration;
			mTimerService = timerService;
			mTimerService.Elapsed += TimerServiceElapsed;
		}

		public static ExerciseService ExerciseServiceFromString(string stateAsString, ExerciseConfiguration configuration, ITimerService timerService)
		{
			try
			{
				return new ExerciseService(new Exercise(configuration), configuration, timerService);
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public string StateToString()
		{
			return mExercise.ToString();
		}

		public void PrepareForNewExercise()
		{
			mExercise = new Exercise(mExerciseConfiguration);
			SignalExerciseChanged();
		}

		public void Start()
		{
			mTimerService.Start(TIMER_INTERVAL_MS);
		}

		public void Pause()
		{
			mTimerService.Pause();
		}

		public void Resume()
		{
			mTimerService.Resume();
		}

		public void Stop()
		{
			mTimerService.Stop();
			mExercise.Reset();
			// Inform
		}

		public bool IsRunning => mTimerService.IsRunning;

		private void TimerServiceElapsed(object sender, EventArgs args)
		{
			// Handle next tick
			if (!mExercise.IsDone)
			{
				if (mExercise.CurrentSubExercise == SubExercise.Undefined)
				{
					TryStartNextSubExercise();
					return;
				}

				if (mExercise.RemainingPreparationTime > 0)
				{
					mExercise.RemainingPreparationTime--;
				}
				else if (mExercise.RemainingSubExerciseTime > 0)
				{
					mExercise.RemainingSubExerciseTime--;
				}
				else
				{
					if (!TryStartNextSubExercise())
					{
						//SignalIsDone();
					}
				}

				// Inform whoever is interested.
				SignalExerciseChanged();
			}
		}

		#region Event ExersizeChanged

		private void SignalExerciseChanged()
		{
			ExerciseChanged?.Invoke(this, new ExerciseChangedEventArgs(mExercise));
		}

		public event EventHandler<ExerciseChangedEventArgs> ExerciseChanged;

		#endregion Event ExersizeChanged

		#region event ActiveSubExerciseChanged

		private void SignalActiveSubExerciseChanged(SubExercise fromSubExercise, SubExercise toSubExercise)
		{
			ActiveSubExerciseChanged?.Invoke(this, new ActiveSubExerciseChangedEventArgs(fromSubExercise, toSubExercise));
		}

		public event EventHandler<ActiveSubExerciseChangedEventArgs> ActiveSubExerciseChanged;

		#endregion event ActiveSubExerciseChanged

		private bool TryStartNextSubExercise()
		{
			switch (mExercise.CurrentSubExercise)
			{
				case SubExercise.Undefined:
					mExercise.CurrentSubExercise = SubExercise.LongLeft;
					break;
				case SubExercise.LongLeft:
					mExercise.LongLeftCount--;
					mExercise.CurrentSubExercise = SubExercise.LongRight;
					break;
				case SubExercise.LongRight:
					mExercise.LongRightCount--;
					mExercise.CurrentSubExercise = mExercise.LongLeftCount > 0
						? SubExercise.LongLeft
						: SubExercise.ShortLeft;
					break;
				case SubExercise.ShortLeft:
					mExercise.ShortLeftCount--;
					mExercise.CurrentSubExercise = SubExercise.ShortRight;
					break;
				case SubExercise.ShortRight:
					mExercise.ShortRightCount--;
					mExercise.CurrentSubExercise = mExercise.ShortLeftCount > 0
						? SubExercise.ShortLeft
						: SubExercise.Undefined;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			mExercise.RemainingPreparationTime = mExerciseConfiguration.PreparationDuration;
			mExercise.RemainingSubExerciseTime = mExerciseConfiguration.DurationPerStance;
			SignalExerciseChanged();

			return mExercise.CurrentSubExercise != SubExercise.Undefined;
		}
	}
}