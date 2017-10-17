using System;
using NSubstitute;
using NUnit.Framework;
using PL.CalfTrainer.Business.Services;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.Services;

namespace PL.CalfTrainer.Business.Tests
{
	[TestFixture]
    public class ExerciseTrackerServiceTests
    {
	    private class TestableExercise : Exercise
	    {
		    public TestableExercise(ExerciseConfiguration configuration) : base(configuration)
		    {
		    }

		    public override int PercentageCompleted => 21;
	    }

	    [Test]
	    public void ExerciseTrackerService_ExerciseFinished_AddsExerciseToRepository()
	    {
		    // Arrange
		    var mockDataService = Substitute.For<IExerciseTrackerDataService>();
		    var stubTimeProvider = Substitute.For<ITimeProvider>();
		    var unitUnderTest = new ExerciseTrackerService(mockDataService, stubTimeProvider);
		    var stubExercise = new TestableExercise(new ExerciseConfiguration());

		    stubTimeProvider.Today.Returns(DateTime.Parse("2017/10/17 12:15:30"));

			// Act
			unitUnderTest.ExerciseFinished(stubExercise);

			// Assert
			mockDataService.Received(1)
				.Add(Arg.Is<ExersiseExecution>(ee => 
					ee.ExecutionTime.Equals(DateTime.Parse("2017/10/17 12:15:30"))
					&& ee.PercentageCompleted==21));
	    }

	}
}
