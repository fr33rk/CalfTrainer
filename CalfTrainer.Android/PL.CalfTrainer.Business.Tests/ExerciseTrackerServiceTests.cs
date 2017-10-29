using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

	    [Test]
	    public void ExerciseTrackerService_ExerciseFinished_RaisesDailyExerciseTrackerChangedEvent()
	    {
		    // Arrange
		    var stubDataService = Substitute.For<IExerciseTrackerDataService>();
		    var stubTimeProvider = Substitute.For<ITimeProvider>();
		    var stubExercise = new TestableExercise(new ExerciseConfiguration());
			var unitUnderTest = new ExerciseTrackerService(stubDataService, stubTimeProvider);
		    var eventRaised = new AutoResetEvent(false);

		    unitUnderTest.DailyExerciseTrackerChanged += (sender, args) => eventRaised.Set();

		    // Act
		    unitUnderTest.ExerciseFinished(stubExercise);

		    // Assert
		    Assert.IsTrue(eventRaised.WaitOne(5000), "Expected event to be raised");
		}

	    [Test]
	    public void ExerciseTrackerService_GetExecutionsOfDay_ReturnsAllOfDay()
	    {
			// Arrange
		    var testDataSet = CreateTestDataSet();
		    var stubDataService = Substitute.For<IExerciseTrackerDataService>();
		    var stubTimeProvider = Substitute.For<ITimeProvider>();
			var unitUnderTest = new ExerciseTrackerService(stubDataService, stubTimeProvider);
		    var expectedDay = new DateTime(2017, 10, 19);

			stubDataService.GetByPeriod(Arg.Any<DateTime>(), Arg.Any<DateTime>())
			    .Returns(x => testDataSet
				    .Where(ee => ee.ExecutionTime >= (DateTime)x[0] && ee.ExecutionTime <= (DateTime)x[1])
					.ToList());

			// Using the percentages here instead of the date to make sure that the
			//  test won't use the same mistake in date selection.
		    var expectedExerciseList = testDataSet.Where(ee => ee.PercentageCompleted >= 30 && ee.PercentageCompleted <= 50).ToList();

			// Act
		    var actualResult = unitUnderTest.GetExecutionsOfDay(expectedDay);

			// Assert
			Assert.AreEqual(expectedDay, actualResult.Day);
			Assert.AreEqual(expectedExerciseList.Count, actualResult.ExersiseExecutions.Count);
		    for (var i = 0; i < expectedExerciseList.Count; i++)
		    {
			    Assert.AreEqual(expectedExerciseList[i], actualResult.ExersiseExecutions[i]);
		    }
	    }

	    [Test]
	    public void ExerciseTrackerService_GetExecutionsOfPeriod_ReturnsAllOfPeriod()
	    {
		    // Arrange
		    var testDataSet = CreateTestDataSet();
		    var stubDataService = Substitute.For<IExerciseTrackerDataService>();
		    var stubTimeProvider = Substitute.For<ITimeProvider>();
		    var unitUnderTest = new ExerciseTrackerService(stubDataService, stubTimeProvider);
			var expectedPeriodStart = new DateTime(2017, 10, 19);
		    var expectedPeriodEnd = new DateTime(2017, 10, 20);

			stubDataService.GetByPeriod(Arg.Any<DateTime>(), Arg.Any<DateTime>())
			    .Returns(x => testDataSet
				    .Where(ee => ee.ExecutionTime >= (DateTime)x[0] && ee.ExecutionTime <= (DateTime)x[1])
				    .ToList());


		    // Act
		    var actualResult = unitUnderTest.GetExecutionsOfPeriod(expectedPeriodStart, expectedPeriodEnd);

		    // Assert
		    //Assert.AreEqual(2, actualResult.Count, "Expected to get the data of 2 days");

	    }

		private static IList<ExersiseExecution> CreateTestDataSet()
	    {
		    return new List<ExersiseExecution>
		    {
			    new ExersiseExecution(new DateTime(2017, 10, 18, 12, 00, 00), 10),
			    new ExersiseExecution(new DateTime(2017, 10, 18, 23, 59, 59), 20),
			    new ExersiseExecution(new DateTime(2017, 10, 19, 00, 00, 00), 30),
			    new ExersiseExecution(new DateTime(2017, 10, 19, 12, 00, 00), 40),
			    new ExersiseExecution(new DateTime(2017, 10, 19, 23, 59, 59), 50),
			    new ExersiseExecution(new DateTime(2017, 10, 20, 00, 00, 00), 60),
			    new ExersiseExecution(new DateTime(2017, 10, 20, 12, 00, 00), 70),
			    new ExersiseExecution(new DateTime(2017, 10, 20, 23, 59, 59), 80),
			    new ExersiseExecution(new DateTime(2017, 10, 21, 00, 00, 00), 90)
		    };		    
	    }

	}
}
