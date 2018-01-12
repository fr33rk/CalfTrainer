using System;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.Enums;
using PL.CalfTrainer.Infrastructure.EventArgs;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Services
{
	public class ExerciseService : IExerciseService
	{
		private Exercise mExercise;
		private ExerciseConfiguration mExerciseConfiguration;
		private ITimerService mTimerService;
		private const int TIMER_INTERVAL_MS = 1000;
		private ExerciseServiceState mCurrentState = ExerciseServiceState.Stopped;
		private IExerciseTrackerService mExerciseTrackerService;

		protected ExerciseService(Exercise exercise, ExerciseConfiguration configuration, ITimerService timerService, IExerciseTrackerService exerciseTrackerService)
		{
			mExercise = exercise;
			mExerciseConfiguration = configuration;
			mTimerService = timerService;
			mExerciseTrackerService = exerciseTrackerService;

			mTimerService.Elapsed += TimerServiceElapsed;
		}

		public static ExerciseService ExerciseServiceFromString(string stateAsString, ExerciseConfiguration configuration, ITimerService timerService, IExerciseTrackerService exerciseTrackerService)
		{
			return string.IsNullOrEmpty(stateAsString)
				? new ExerciseService(new Exercise(configuration), configuration, timerService, exerciseTrackerService)
				: new ExerciseService(Exercise.ExerciseFromString(stateAsString, configuration), configuration, timerService, exerciseTrackerService);
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

		public void Run()
		{
			if (mCurrentState == ExerciseServiceState.Stopped)
			{
				mTimerService.Start(TIMER_INTERVAL_MS);
			}
			else if (mCurrentState == ExerciseServiceState.Paused)
			{
				mTimerService.Resume();
			}

			ChangeState(ExerciseServiceState.Started);
		}

		public void Pause()
		{
			mTimerService.Pause();
			ChangeState(ExerciseServiceState.Paused);
		}

		public void Stop()
		{
			mExerciseTrackerService.ExerciseFinished(mExercise);
			mTimerService.Stop();
			SignalActiveSubExerciseChanged(mExercise.CurrentSubExercise, SubExercise.Undefined);
			mExercise.Reset();
			SignalExerciseChanged();
			ChangeState(ExerciseServiceState.Stopped);
		}

		public void SendExerciseState()
		{
			SignalExerciseChanged();
		}

		public bool IsRunning => mTimerService.IsRunning;

		private void TimerServiceElapsed(object sender, EventArgs args)
		{
			// Handle next tick
			if(mCurrentState == ExerciseServiceState.Started)
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
						Stop();
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
			var newSubExercise = SubExercise.Undefined;

			switch (mExercise.CurrentSubExercise)
			{
				case SubExercise.Undefined:
					newSubExercise = SubExercise.LongLeft;
					break;

				case SubExercise.LongLeft:
					mExercise.LongLeftCount--;
					newSubExercise = SubExercise.LongRight;
					break;

				case SubExercise.LongRight:
					mExercise.LongRightCount--;
					newSubExercise = mExercise.LongLeftCount > 0
						? SubExercise.LongLeft
						: SubExercise.ShortLeft;
					break;

				case SubExercise.ShortLeft:
					mExercise.ShortLeftCount--;
					newSubExercise = SubExercise.ShortRight;
					break;

				case SubExercise.ShortRight:
					mExercise.ShortRightCount--;
					newSubExercise = mExercise.ShortLeftCount > 0
						? SubExercise.ShortLeft
						: SubExercise.Undefined;
					break;
			}

			SignalActiveSubExerciseChanged(mExercise.CurrentSubExercise, newSubExercise);
			mExercise.CurrentSubExercise = newSubExercise;
			mExercise.RemainingPreparationTime = mExerciseConfiguration.PreparationDuration;
			mExercise.RemainingSubExerciseTime = mExerciseConfiguration.DurationPerStance;
			SignalExerciseChanged();

			return mExercise.CurrentSubExercise != SubExercise.Undefined;
		}

		private void ChangeState(ExerciseServiceState newState)
		{
			mCurrentState = newState;
			SignalStateChanged(newState);
		}

		public event EventHandler<ExerciseServiceStateChangedArgs> StateChanged;

		private void SignalStateChanged(ExerciseServiceState newState)
		{
			StateChanged?.Invoke(this, new ExerciseServiceStateChangedArgs(newState));
		}
	}
}