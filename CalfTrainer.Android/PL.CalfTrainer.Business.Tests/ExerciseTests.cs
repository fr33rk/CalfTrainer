using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PL.CalfTrainer.Business.Entities;

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


    }
}
