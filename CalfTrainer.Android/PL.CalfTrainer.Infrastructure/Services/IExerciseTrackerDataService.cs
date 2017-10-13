using System;
using System.Collections.Generic;
using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Infrastructure.Services
{
	public interface IExerciseTrackerDataService
    {
	    void Add(ExersiseExecution exerciseExecution);
	    IList<ExersiseExecution> GetByPeriod(DateTime periodStart, DateTime periodEnd);
    }
}
