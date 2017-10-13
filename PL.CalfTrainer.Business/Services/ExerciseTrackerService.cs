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
			// Test how far the test is finished
			// Create a ExerciseResult object and store it.
			throw new NotImplementedException();
		}

		public DailyExerciseTracker GetExecutionsOfDay(DateTime day)
		{
			throw new NotImplementedException();
		}

		public IList<DailyExerciseTracker> GetExecutionsOfPeriod(DateTime startOfPeriod, DateTime endOfPeriod)
		{
			throw new NotImplementedException();
		}

		public event EventHandler<DailyExerciseTrackerChangedArgs> DailyExerciseTrackerChanged;
	}
}