using System;
using System.Collections.Generic;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.EventArgs;

namespace PL.CalfTrainer.Infrastructure.Services
{
	public interface IExerciseTrackerService
	{
		void ExerciseFinished(Exercise exercise);

		DailyExerciseTracker GetExecutionsOfDay(DateTime day);

		IList<DailyExerciseTracker> GetExecutionsOfPeriod(DateTime startOfPeriod, DateTime endOfPeriod);

		event EventHandler<DailyExerciseTrackerChangedArgs> DailyExerciseTrackerChanged;
	}
}