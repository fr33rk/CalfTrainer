using NSubstitute;
using NUnit.Framework;
using PL.CalfTrainer.Business.Services;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Tests
{
	[TestFixture]
	public class ExerciseServiceTests
	{
		[Test]
		public void ExerciseService_PrepareExerciseServiceFromJsonWithEmptyString_CreatesDefault()
		{
			// Arrange
			var eventThrown = false;
			var exerciseConfiguration = new ExerciseConfiguration
			{
				NoOfRepetitions = 8,
				DurationPerStance = 10,
				PreparationDuration = 3
			};

			Exercise actualExercise = null;
			var expectedExercise = new Exercise(exerciseConfiguration);

			var stubTimerService = Substitute.For<ITimerService>();

			var unitUnderTest = ExerciseService.ExerciseServiceFromJson(string.Empty, exerciseConfiguration, stubTimerService);
			unitUnderTest.ExerciseChanged += (sender, args) =>
			{
				eventThrown = true;
				actualExercise = args.Exercise;
			};

			// Act
			unitUnderTest.PrepareForNewExercise();

			// Assert
			// TODO: Move this to a sepparate test
			Assert.IsTrue(eventThrown, "Expected exercise changed event thrown");
			Assert.AreEqual(expectedExercise, actualExercise);
		}

		[Test]
		public void ExerciseService_Prepare_SignalsExerciseChanged()
		{
			
		}
	}
}