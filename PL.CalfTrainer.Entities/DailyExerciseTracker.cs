using System;
using System.Collections.Generic;

namespace PL.CalfTrainer.Entities
{
	public class DailyExerciseTracker
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
		public DailyExerciseTracker(DateTime day, IList<ExersiseExecution> exersiseExecutions)
		{
			Day = day;
			ExersiseExecutions = exersiseExecutions;
		}

		public DateTime Day { get; }
		public IList<ExersiseExecution> ExersiseExecutions { get; }
	}
}
