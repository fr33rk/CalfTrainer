using System.Threading;
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
		private ExerciseConfiguration CreateTestExerciseConfiguration()
		{
			return new ExerciseConfiguration
			{
				NoOfRepetitions = 8,
				DurationPerStance = 8,
				PreparationDuration = 3
			};
		}

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

			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubTimerService);
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
			Assert.AreEqual(expectedExercise.ToString(), actualExercise.ToString());
		}

		[Test]
		public void ExerciseService_Prepare_SignalsExerciseChanged()
		{
			// Arrange
			var stubTimerService = Substitute.For<ITimerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService);
			var receivedChangedEvent = new AutoResetEvent(false);

			unitUnderTest.ExerciseChanged += (sender, args) => receivedChangedEvent.Set();

			// Act
			unitUnderTest.PrepareForNewExercise();

			// Assert
			Assert.IsTrue(receivedChangedEvent.WaitOne(5000));
		}

		[Test]
		public void ExerciseService_Prepare_ExerciseReset()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var stubTimerService = Substitute.For<ITimerService>();

			// TODO: Replace string.Empty with saved state
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubTimerService);
			var expectedExercise = new Exercise(exerciseConfiguration);
			Exercise actualExercise = null;

			unitUnderTest.ExerciseChanged += (sender, args) => actualExercise = args.Exercise;

			// Act
			unitUnderTest.PrepareForNewExercise();

			// Assert
			Assert.AreEqual(expectedExercise.ToString(), actualExercise?.ToString());
		}

		[Test]
		public void ExerciseService_Start_StartsTimer()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var mockTimerService = Substitute.For<ITimerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, mockTimerService);

			// Act
			unitUnderTest.Start();

			// Assert
			mockTimerService.Received(1).Start(Arg.Any<int>());
		}

		[Test]
		public void ExerciseService_TimerTick_SignalExerciseChanged()
		{
			// Arrange
			var stubTimerService = Substitute.For<ITimerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService);
			var receivedChangedEvent = new AutoResetEvent(false);

			unitUnderTest.ExerciseChanged += (sender, args) => receivedChangedEvent.Set();

			unitUnderTest.Start();

			// Act
			stubTimerService.Elapsed += Raise.Event();
			// Assert
			Assert.IsTrue(receivedChangedEvent.WaitOne(5000), "Expected to receive changed event");
		}

		[Test]
		[TestCase(1, "LongLeftCount=8;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=0")]
		[TestCase(2, "LongLeftCount=8;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=2;RemainingSubExerciseTime=8;CurrentSubExercise=0")]
		[TestCase(5, "LongLeftCount=8;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=0;RemainingSubExerciseTime=7;CurrentSubExercise=0")]
		[TestCase(13, "LongLeftCount=7;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=1")]
		public void ExerciseService_TimerTick_ExerciseChangedCorrect(int elapsedTicks, string expectedExerciseState)
		{
			// Arrange
			var stubTimerService = Substitute.For<ITimerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService);
			Exercise actualExercise = null;

			unitUnderTest.ExerciseChanged += (sender, args) => actualExercise = args.Exercise;
			unitUnderTest.Start();

			// Act
			for (int i = 0; i < elapsedTicks; i++)
			{
				stubTimerService.Elapsed += Raise.Event();
			}

			// Assert
			Assert.AreEqual(expectedExerciseState, actualExercise.ToString());
		}
	}
}