using System;
using System.Collections.Generic;

namespace PL.CalfTrainer.Entities
{
	public class DailyExerciseTracker
	{
		public DateTime Day;
		public IList<ExersiseExecution> ExersiseExecutions;
	}
}
