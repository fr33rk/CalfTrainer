using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalfTrainer.Android
{
    public class ExerciseConfiguration
    {
		public ExerciseConfiguration()
		{
			NoOfRepetitions = 8;
			DurationPerStance = 8;
		}

		public uint DurationPerStance { get; set; }
		public uint NoOfRepetitions { get; set; }
    }
}
