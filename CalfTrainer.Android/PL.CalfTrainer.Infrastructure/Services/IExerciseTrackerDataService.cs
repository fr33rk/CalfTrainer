using System;
using System.Collections.Generic;
using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Infrastructure.Services
{
	/// <summary>Stores and retrieves exercices tracker data. 
	/// </summary>
	public interface IExerciseTrackerDataService
    {
		/// <summary> Adds the specified exercise execution.
		/// </summary>
		/// <param name="exerciseExecution">The exercise execution.</param>
		void Add(ExersiseExecution exerciseExecution);

		/// <summary>Load all exercise executions in the given period.
		/// </summary>
		/// <param name="periodStart">The period start.</param>
		/// <param name="periodEnd">The period end.</param>
		/// <returns>The list of found entries. When nothing is found, an empty list is returned.</returns>
		IList<ExersiseExecution> GetByPeriod(DateTime periodStart, DateTime periodEnd);
    }
}
