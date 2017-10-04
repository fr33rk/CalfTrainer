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

		public static ExerciseService ExerciseServiceFromJson(string stateAsString, ExerciseConfiguration configuration, ITimerService timerService)
		{
			try
			{
				return new ExerciseService(new Exercise(configuration), configuration, timerService );
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
			// Inform whoever is interested.
		}


		#region Event ExersizeChanged

		private void SignalExerciseChanged()
		{
			ExerciseChanged?.Invoke(this, new ExerciseChangedEventArgs(mExercise));
		}

		public event EventHandler<ExerciseChangedEventArgs> ExerciseChanged;

		#endregion Event ExersizeChanged

	}
}