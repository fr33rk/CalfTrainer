using NUnit.Framework;
using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Business.Tests
{
	[TestFixture]
	public class ExerciseTests
	{
		[Test]
		public void Exercise_Constructed_CountersSet()
		{
			// Arrange
			var stubConfiguration = new ExerciseConfiguration()
			{
				NoOfRepetitions = 2109,
				DurationPerStance = 19,
				PreparationDuration = 78
			};

			// Act
			var unitUnderTest = new Exercise(stubConfiguration);

			// Assert
			Assert.AreEqual(SubExercise.Undefined, unitUnderTest.CurrentSubExercise);
			Assert.AreEqual(stubConfiguration.NoOfRepetitions, unitUnderTest.LongLeftCount);
			Assert.AreEqual(stubConfiguration.NoOfRepetitions, unitUnderTest.LongRightCount);
			Assert.AreEqual(stubConfiguration.NoOfRepetitions, unitUnderTest.ShortLeftCount);
			Assert.AreEqual(stubConfiguration.NoOfRepetitions, unitUnderTest.ShortRightCount);
			Assert.AreEqual(stubConfiguration.DurationPerStance, unitUnderTest.RemainingSubExerciseTime);
			Assert.AreEqual(stubConfiguration.PreparationDuration, unitUnderTest.RemainingPreparationTime);
		}

		[Test]
		public void Exercise_ToString_ReturnsCorrectString()
		{
			// Arrange
			var stubConfiguration = new ExerciseConfiguration()
			{
				NoOfRepetitions = 2109,
				DurationPerStance = 19,
				PreparationDuration = 78
			};
			var unitUnderTest = new Exercise(stubConfiguration);
			const string expectedResult = "LongLeftCount=2109;LongRightCount=2109;ShortLeftCount=2109;ShortRightCount=2109;RemainingPreparationTime=78;RemainingSubExerciseTime=19";

			// Act
			var actualResult = unitUnderTest.ToString();

			// Assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		public void Exercise_ExerciseFromString_ReturnsCorrectObject()
		{
			
		}
	}
}