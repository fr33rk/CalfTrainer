namespace PL.CalfTrainer.Entities
{
	public class ExerciseConfiguration
	{
		public ExerciseConfiguration()
		{
			NoOfRepetitions = 8;
			DurationPerStance = 8;
			PreparationDuration = 3;
		}

		public int DurationPerStance { get; set; }
		public int PreparationDuration { get; set; }
		public int NoOfRepetitions { get; set; }
	}
}