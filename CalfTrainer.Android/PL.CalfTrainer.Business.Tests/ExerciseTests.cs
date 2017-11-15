using System;
using NUnit.Framework;
using PL.CalfTrainer.Entities;

namespace PL.CalfTrainer.Business.Tests
{
	[TestFixture]
	public class ExerciseTests
	{
		private static ExerciseConfiguration CreateTestExerciseConfiguration()
		{
			return new ExerciseConfiguration
			{
				DurationPerStance = 8,
				NoOfRepetitions = 8,
				PreparationDuration = 3
			};
		}

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
			const string expectedResult = "LongLeftCount=2109;LongRightCount=2109;ShortLeftCount=2109;ShortRightCount=2109;RemainingPreparationTime=78;RemainingSubExerciseTime=19;CurrentSubExercise=0";

			// Act
			var actualResult = unitUnderTest.ToString();

			// Assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		public void Exercise_ExerciseFromString_ReturnsCorrectObject()
		{
			// Arrange
			const string exerciseAsString = "LongLeftCount=1;LongRightCount=2;ShortLeftCount=3;ShortRightCount=4;RemainingPreparationTime=5;RemainingSubExerciseTime=6;CurrentSubExercise=2";
			var stubConfiguration = new ExerciseConfiguration();

			// Act
			var unitUnderTest = Exercise.ExerciseFromString(exerciseAsString, stubConfiguration);

			// Assert
			Assert.AreEqual(1, unitUnderTest.LongLeftCount);
			Assert.AreEqual(2, unitUnderTest.LongRightCount);
			Assert.AreEqual(3, unitUnderTest.ShortLeftCount);
			Assert.AreEqual(4, unitUnderTest.ShortRightCount);
			Assert.AreEqual(5, unitUnderTest.RemainingPreparationTime);
			Assert.AreEqual(6, unitUnderTest.RemainingSubExerciseTime);
			Assert.AreEqual((SubExercise)2, unitUnderTest.CurrentSubExercise);
		}

		[Test]
		public void Exercise_ExerciseFromInvalidString_ReturnsResetObject()
		{
			// Arrange
			const string exerciseAsString = "invalid sting";
			var stubConfiguration = CreateTestExerciseConfiguration();

			// Act
			var unitUnderTest = Exercise.ExerciseFromString(exerciseAsString, stubConfiguration);

			// Assert
			Assert.AreEqual(8, unitUnderTest.LongLeftCount);
			Assert.AreEqual(8, unitUnderTest.LongRightCount);
			Assert.AreEqual(8, unitUnderTest.ShortLeftCount);
			Assert.AreEqual(8, unitUnderTest.ShortRightCount);
			Assert.AreEqual(3, unitUnderTest.RemainingPreparationTime);
			Assert.AreEqual(8, unitUnderTest.RemainingSubExerciseTime);
		}

		[Test]
		public void Exercise_RemainingTotalTimeWhenStarted_ReturnsCorrectValue()
		{
			// Arrange
			var stubConfiguration = CreateTestExerciseConfiguration();
			var expectedRemainingTime = TimeSpan.FromSeconds((8 + 3 + 1) * 8 * 4); // ((TimePerSubExercise + PrepTime + 1s for switching) * Repetitions * NoOfSubExercises)

			// Act
			var unitUnderTest = Exercise.ExerciseFromConfiguration(stubConfiguration);

			// Assert
			Assert.AreEqual(expectedRemainingTime, unitUnderTest.RemainingTotalTime);
		}

		[Test]
		public void Exercise_Started_PercentageCompletedIsZero()
		{
			// Arrange
			var stubConfiguration = new ExerciseConfiguration()
			{
				NoOfRepetitions = 8,
				DurationPerStance = 8,
				PreparationDuration = 3
			};
			var expectedPercentage = 0;

			var unitUnderTest = new Exercise(stubConfiguration);

			// Assert
			Assert.AreEqual(expectedPercentage, unitUnderTest.PercentageCompleted);
		}

		[Test]
		public void Exercise_HalfWay_PercentageCompletedIsFifty()
		{
			// Arrange
			var stubConfiguration = new ExerciseConfiguration()
			{
				NoOfRepetitions = 8,
				DurationPerStance = 8,
				PreparationDuration = 3
			};
			var expectedPercentage = 50;

			var unitUnderTest = new Exercise(stubConfiguration)
			{
				LongLeftCount = 0,
				LongRightCount = 0
			};

			// Assert
			Assert.AreEqual(expectedPercentage, unitUnderTest.PercentageCompleted);
		}

		[Test]
		public void Exercise_Done_PercentageCompletedIsHundred()
		{
			// Arrange
			var stubConfiguration = new ExerciseConfiguration()
			{
				NoOfRepetitions = 8,
				DurationPerStance = 8,
				PreparationDuration = 3
			};

			const int expectedPercentage = 100;

			var unitUnderTest = new Exercise(stubConfiguration)
			{
				LongLeftCount = 0,
				LongRightCount = 0,
				ShortLeftCount = 0,
				ShortRightCount = 0,
				RemainingPreparationTime = 0,
				RemainingSubExerciseTime = 0
			};

			// Assert
			Assert.AreEqual(expectedPercentage, unitUnderTest.PercentageCompleted);
		}
	}
}