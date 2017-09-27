using System;

namespace PL.CalfTrainer.Business.Entities
{
    public class ExersiseExecution
    {
	    public enum ExersizeResult
	    {
		    Started,
			PartiallyFinished,
			Finished
	    }

	    public DateTime ExecutionTime { get; set; }
	    public ExersizeResult Result { get; set; }
    }
}
