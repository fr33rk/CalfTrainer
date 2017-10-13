using System;

namespace PL.CalfTrainer.Entities
{
	public class ExersiseExecution
    {
	    /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
	    public ExersiseExecution(DateTime executionTime, int percentageCompleted)
	    {
		    ExecutionTime = executionTime;
		    PercentageCompleted = percentageCompleted;
	    }

	    public DateTime ExecutionTime { get; set; }

	    public int PercentageCompleted { get; set; }
    }
}
