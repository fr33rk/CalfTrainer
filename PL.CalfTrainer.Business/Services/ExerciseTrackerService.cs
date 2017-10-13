// <copyright file="ExerciseTrackerService.cs" company="Delta Instruments">Copyright © Delta Instruments B.V.</copyright>
using System;
using System.Collections.Generic;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.EventArgs;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Services
{


	public class ExerciseTrackerService : IExerciseTrackerService
	{
		private IExerciseTrackerDataService mDataService;

		public ExerciseTrackerService(IExerciseTrackerDataService dataService)
		{
			mDataService = dataService;
		}

		public void ExerciseFinished(Exercise exercise)
		{
			var exerciseExecution = new ExersiseExecution(DateTime.Now, exercise.PercentageCompleted);
			mDataService.Add(exerciseExecution);
			SignalDailyExerciseTrackerChanged(GetExecutionsOfDay(DateTime.Today));
		}

		public DailyExerciseTracker GetExecutionsOfDay(DateTime day)
		{
			var executions = mDataService.GetByPeriod(DateTime.Today, DateTime.Today.AddDays(1).AddTicks(-1));

			return new DailyExerciseTracker(DateTime.Today, executions);
		}

		public IList<DailyExerciseTracker> GetExecutionsOfPeriod(DateTime startOfPeriod, DateTime endOfPeriod)
		{
			throw new NotImplementedException();
		}

		private void SignalDailyExerciseTrackerChanged(DailyExerciseTracker dailyExerciseTracker)
		{
			DailyExerciseTrackerChanged?.Invoke(this, new DailyExerciseTrackerChangedArgs(dailyExerciseTracker));
		}

		public event EventHandler<DailyExerciseTrackerChangedArgs> DailyExerciseTrackerChanged;
	}
}