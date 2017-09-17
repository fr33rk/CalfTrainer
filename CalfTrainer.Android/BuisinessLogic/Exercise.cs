namespace CalfTrainer.Android.BuisinessLogic
{
    public class Exercise
    {
	    public Exercise(ExerciseConfiguration configuration)
	    {
		    LongLeftCount = configuration.NoOfRepetitions;
		    ShortLeftCount = configuration.NoOfRepetitions;
		    LongRightCount = configuration.NoOfRepetitions;
		    ShortRightCount = configuration.NoOfRepetitions;
	    }

        public uint LongLeftCount { get; set; }
		public uint ShortLeftCount { get; set; }
		public uint LongRightCount { get; set; }
		public uint ShortRightCount { get; set; }
    }
}
