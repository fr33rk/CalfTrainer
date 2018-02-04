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
		private static ExerciseConfiguration CreateTestExerciseConfiguration()
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
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();

			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubTimerService, stubExerciseTrackerService);
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
		public void ExerciseService_PrepareExerciseServiceFromInvalidString_CreatedDefault()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var stubExerciseTimer = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var invalidString = "Invalid string";
			var expectedResult = new Exercise(exerciseConfiguration).ToString();

			// Act
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(invalidString, exerciseConfiguration, stubExerciseTimer, stubExerciseTrackerService);
			var actualResult = unitUnderTest.StateToString();

			// Assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		public void ExerciseService_StateToString_ReturnsExerciseToString()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var stubExerciseTImer = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubExerciseTImer, stubExerciseTrackerService);
			var expectedResult = new Exercise(exerciseConfiguration).ToString();

			// Act
			var actualResult = unitUnderTest.StateToString();

			// Assert
			Assert.AreEqual(expectedResult, actualResult);
		}

		[Test]
		public void ExerciseService_Prepare_SignalsExerciseChanged()
		{
			// Arrange
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService, stubExerciseTrackerService);
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
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();

			// TODO: Replace string.Empty with saved state
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubTimerService, stubExerciseTrackerService);
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
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, mockTimerService, stubExerciseTrackerService);

			// Act
			unitUnderTest.Run();

			// Assert
			mockTimerService.Received(1).Start(Arg.Any<int>());
		}

		[Test]
		public void ExerciseService_Pause_PausesTimer()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var mockTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, mockTimerService, stubExerciseTrackerService);
			unitUnderTest.Run();

			// Act
			unitUnderTest.Pause();

			// Assert
			mockTimerService.Received(1).Pause();
		}

		[Test]
		public void ExerciseService_Resume_ResumesTimer()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var mockTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, mockTimerService, stubExerciseTrackerService);
			unitUnderTest.Run();
			unitUnderTest.Pause();

			// Act
			unitUnderTest.Run();

			// Assert
			mockTimerService.Received(1).Resume();
		}

		[Test]
		public void ExerciseService_TimerTick_SignalExerciseChanged()
		{
			// Arrange
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService, stubExerciseTrackerService);
			var receivedChangedEvent = new AutoResetEvent(false);

			unitUnderTest.ExerciseChanged += (sender, args) => receivedChangedEvent.Set();

			unitUnderTest.Run();

			// Act
			stubTimerService.Elapsed += Raise.Event();
			// Assert
			Assert.IsTrue(receivedChangedEvent.WaitOne(5000), "Expected to receive changed event");
		}

		[Test]
		[TestCase(1, "Start", "LongLeftCount=8;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=1")]
		[TestCase(2, "1st second", "LongLeftCount=8;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=2;RemainingSubExerciseTime=8;CurrentSubExercise=1")]
		[TestCase(5, "Preparation finished", "LongLeftCount=8;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=0;RemainingSubExerciseTime=7;CurrentSubExercise=1")]
		[TestCase(13, "Next sub exercise", "LongLeftCount=7;LongRightCount=8;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=3")]
		[TestCase(25, "3d sub exercise", "LongLeftCount=7;LongRightCount=7;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=1")]
		[TestCase(193, "1st short sub exercise", "LongLeftCount=0;LongRightCount=0;ShortLeftCount=8;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=2")]
		[TestCase(205, "2st short sub exercise", "LongLeftCount=0;LongRightCount=0;ShortLeftCount=7;ShortRightCount=8;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=4")]
		[TestCase(217, "2nd time 1st short sub exercise", "LongLeftCount=0;LongRightCount=0;ShortLeftCount=7;ShortRightCount=7;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=2")]
		[TestCase(385, "Done", "LongLeftCount=0;LongRightCount=0;ShortLeftCount=0;ShortRightCount=0;RemainingPreparationTime=3;RemainingSubExerciseTime=8;CurrentSubExercise=0")]
		public void ExerciseService_TimerTick_ExerciseChangedCorrect(int elapsedTicks, string testCaseDescription, string expectedExerciseState)
		{
			// Arrange
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService, stubExerciseTrackerService);
			Exercise actualExercise = null;
			bool handled = false;

			unitUnderTest.Run();

			for (var i = 0; i < elapsedTicks - 1; i++)
			{
				stubTimerService.Elapsed += Raise.Event();
			}

			unitUnderTest.ExerciseChanged += (sender, args) =>
			{
				if (!handled)
				{
					actualExercise = args.Exercise;
					handled = true;
				}
			};

			// Act
			stubTimerService.Elapsed += Raise.Event();

			// Assert
			Assert.AreEqual(expectedExerciseState, actualExercise.ToString(), $"TestCase {testCaseDescription} failed");
		}

		[Test]
		public void ExerciseService_Stop_SignalsActiveSubExerciseChnaged()
		{
			// Arrange
			var actualOldSubExercise = SubExercise.Undefined;
			var actualNewSubExercise = SubExercise.Undefined;
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService, stubExerciseTrackerService);

			unitUnderTest.Run();

			for (var i = 0; i < 193; i++)
			{
				stubTimerService.Elapsed += Raise.Event();
			}

			unitUnderTest.ActiveSubExerciseChanged += (sender, args) =>
			{
				actualOldSubExercise = args.OldSubExercise;
				actualNewSubExercise = args.NewSubExercise;
			};

			// Act
			unitUnderTest.Stop();

			// Assert
			Assert.AreEqual(SubExercise.ShortLeft, actualOldSubExercise);
			Assert.AreEqual(SubExercise.Undefined, actualNewSubExercise);
		}

		[Test]
		[TestCase(1, SubExercise.Undefined, SubExercise.LongLeft)]
		[TestCase(13, SubExercise.LongLeft, SubExercise.LongRight)]
		[TestCase(25, SubExercise.LongRight, SubExercise.LongLeft)]
		[TestCase(193, SubExercise.LongRight, SubExercise.ShortLeft)]
		[TestCase(385, SubExercise.ShortRight, SubExercise.Undefined)]
		public void ExerciseService_TimerTick_SignalsActiveSubExerciseChanged(int elapsedTicks, SubExercise expectedOldSubExercise, SubExercise expectedNewSubExercise)
		{
			// Arrange
			var actualOldSubExercise = SubExercise.Undefined;
			var actualNewSubExercise = SubExercise.Undefined;
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, CreateTestExerciseConfiguration(), stubTimerService, stubExerciseTrackerService);
			bool handled = false;

			unitUnderTest.Run();

			for (var i = 0; i < elapsedTicks - 1; i++)
			{
				stubTimerService.Elapsed += Raise.Event();
			}

			unitUnderTest.ActiveSubExerciseChanged += (sender, args) =>
			{
				if (!handled)
				{
					actualOldSubExercise = args.OldSubExercise;
					actualNewSubExercise = args.NewSubExercise;
					handled = true;
				}
			};

			// Act
			stubTimerService.Elapsed += Raise.Event();

			// Assert
			Assert.AreEqual(expectedOldSubExercise, actualOldSubExercise);
			Assert.AreEqual(expectedNewSubExercise, actualNewSubExercise);
		}

		[Test]
		public void ExerciseService_IsRunning_ReturnsTrueWhenRunning()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubTimerService, stubExerciseTrackerService);
			stubTimerService.IsRunning.Returns(true);

			// Act and assert
			Assert.IsTrue(unitUnderTest.IsRunning);
		}

		[Test]
		public void ExerciseService_IsRunning_ReturnsFalseWhenNotRunning()
		{
			// Arrange
			var exerciseConfiguration = CreateTestExerciseConfiguration();
			var stubTimerService = Substitute.For<ITimerService>();
			var stubExerciseTrackerService = Substitute.For<IExerciseTrackerService>();
			var unitUnderTest = ExerciseService.ExerciseServiceFromString(string.Empty, exerciseConfiguration, stubTimerService, stubExerciseTrackerService);
			stubTimerService.IsRunning.Returns(false);

			// Act and assert
			Assert.IsFalse(unitUnderTest.IsRunning);
		}
	}
}