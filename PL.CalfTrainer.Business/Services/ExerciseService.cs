using System;
using PL.CalfTrainer.Business.Entities;
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

		public static ExerciseService ExerciseServiceFromJson(string asJson, ExerciseConfiguration configuration, ITimerService timerService)
		{
			try
			{
				//var exercise = string.IsNullOrEmpty(asJson)
				//	? new Exercise(configuration)
				//	: JsonConvert.DeserializeObject<Exercise>(asJson);

				//return new ExerciseService(exercise, configuration, timerService);

				return null;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw;
			}
		}

		public string ToJson()
		{
			return String.Empty; //JsonConvert.SerializeObject(mExercise);
		}

		public void PrepareForNewExercise()
		{
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
	}
}