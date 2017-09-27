namespace PL.CalfTrainer.Business.Entities
{
	public class ExerciseConfiguration
	{
		public ExerciseConfiguration()
		{
			NoOfRepetitions = 8;
			DurationPerStance = 8;
			PreparationDuration = 3;
		}

		public uint DurationPerStance { get; set; }
		public uint PreparationDuration { get; set; }
		public uint NoOfRepetitions { get; set; }
	}
}