using System;
using System.Threading;
using NUnit.Framework;
using PL.CalfTrainer.Business.Services;

namespace PL.CalfTrainer.Business.Tests
{
	[TestFixture]
	public class TimerServiceTests
	{
		[Test]
		public void TimerService_TimerStarted_EventsThrown()
		{
			// Arrange
			var timerElapsedEvent = new AutoResetEvent(false);
			var unitUnderTest = new TimerService();
			unitUnderTest.Elapsed += (sender, args) => timerElapsedEvent.Set();

			// Act
			unitUnderTest.Start(100);

			// Assert
			Assert.IsTrue(timerElapsedEvent.WaitOne(200), "Expected Elapsed event");
			Assert.IsTrue(timerElapsedEvent.WaitOne(200), "Expected 2nd Elapsed event");
		}

		[Test]
		public void TimerService_TimerPaused_NoEventsThrown()
		{
			// Arrange
			var timerElapsedEvent = new AutoResetEvent(false);
			var unitUnderTest = new TimerService();
			unitUnderTest.Elapsed += (sender, args) => timerElapsedEvent.Set();
			unitUnderTest.Start(100);

			// Act
			unitUnderTest.Pause();
			timerElapsedEvent.Reset();

			// Assert
			Assert.IsFalse(timerElapsedEvent.WaitOne(200), "Did not expected Elapsed event");
		}

		[Test]
		public void TimerService_TimerResumed_EventsThrown()
		{
			// Arrange
			var timerElapsedEvent = new AutoResetEvent(false);
			var unitUnderTest = new TimerService();
			unitUnderTest.Elapsed += (sender, args) => timerElapsedEvent.Set();
			unitUnderTest.Start(100);

			// Act
			unitUnderTest.Pause();
			timerElapsedEvent.Reset();
			unitUnderTest.Resume();

			// Assert
			Assert.IsTrue(timerElapsedEvent.WaitOne(200), "Expected Elapsed event");
			Assert.IsTrue(timerElapsedEvent.WaitOne(200), "Expected 2nd Elapsed event");
		}

		[Test]
		public void TimerService_TimerStopped_NoEventThrown()
		{
			// Arrange
			var timerElapsedEvent = new AutoResetEvent(false);
			var unitUnderTest = new TimerService();
			unitUnderTest.Elapsed += (sender, args) => timerElapsedEvent.Set();
			unitUnderTest.Start(100);

			// Act
			unitUnderTest.Stop();
			timerElapsedEvent.Reset();

			// Assert
			Assert.IsFalse(timerElapsedEvent.WaitOne(200), "Did not expected Elapsed event");
		}

		[Test]
		public void TimerService_TimerStarted_IntervalCorrect()
		{
			// Arrange
			const int eventsToCapture = 10;
			const int eventInterval = 300;
			var timerElapsedEvent = new AutoResetEvent(false);
			var unitUnderTest = new TimerService();
			var eventList = new long[eventsToCapture];
			var eventCount = 0;
			unitUnderTest.Elapsed += (sender, args) =>
			{
				if (eventCount < eventsToCapture)
				{
					eventList[eventCount] = DateTime.Now.Ticks;
				}
				eventCount++;
				if (eventCount == eventsToCapture)
				{
					timerElapsedEvent.Set();
				}
			};

			// Act
			unitUnderTest.Start(eventInterval);

			timerElapsedEvent.WaitOne(5000);

			// Assert
			for (var i = 0; i < eventsToCapture - 1; i++)
			{
				var timeSpan = new TimeSpan(eventList[i + 1] - eventList[i]);
				Console.WriteLine(timeSpan.TotalMilliseconds);

				Assert.IsTrue(Math.Abs(eventInterval - timeSpan.TotalMilliseconds) < 20,
					$"Interval not correct between {i} and {i + 1}, difference = {timeSpan.TotalMilliseconds}");
			}

			//Assert.AreEqual();
		}

		[Test]
		public void TimerService_TimerCreated_IsNotRunning()
		{
			// Arrange
			var unitUnderTest = new TimerService();

			// Assert
			Assert.IsFalse(unitUnderTest.IsRunning);
		}

		[Test]
		public void TimerService_TimerStarted_IsRunning()
		{
			// Arrange
			var unitUnderTest = new TimerService();

			// Act
			unitUnderTest.Start(100);

			// Arrange
			Assert.IsTrue(unitUnderTest.IsRunning);
		}

		[Test]
		public void TimerService_TimerPaused_IsNotRunning()
		{
			// Arrange
			var unitUnderTest = new TimerService();
			unitUnderTest.Start(100);

			// Act
			unitUnderTest.Pause();

			// Arrange
			Assert.IsFalse(unitUnderTest.IsRunning);
		}

		[Test]
		public void TimerService_TimerResumed_IsRunning()
		{
			// Arrange
			var unitUnderTest = new TimerService();
			unitUnderTest.Start(100);
			unitUnderTest.Pause();

			// Act
			unitUnderTest.Resume();

			// Arrange
			Assert.IsTrue(unitUnderTest.IsRunning);
		}

		[Test]
		public void TimerService_TimerStopped_IsRunning()
		{
			// Arrange
			var unitUnderTest = new TimerService();
			unitUnderTest.Start(100);
			unitUnderTest.Pause();
			unitUnderTest.Resume();

			// Act
			unitUnderTest.Stop();

			// Arrange
			Assert.IsFalse(unitUnderTest.IsRunning);
		}

		[Test]
		public void TimerService_TimerStoppedBeforeStarted_ThrowsException()
		{
			// Arrange
			var unitUnderTest = new TimerService();

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => unitUnderTest.Stop());
		}

		[Test]
		public void TimerService_TimerStartedBeforeStopped_ThrowsException()
		{
			// Arrange
			var unitUnderTest = new TimerService();
			unitUnderTest.Start(100);

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => unitUnderTest.Start(100));
		}
	}
}