using System;
using System.Collections.Generic;
using Android.Content;
using Newtonsoft.Json;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.Services;

namespace CalfTrainer.Android
{
	/// <inheritdoc />
	/// <summary>Stores and loads exercise information from the SharedPreverences storage.
	/// Only the exercises of today are stored.
	/// </summary>
	/// <seealso cref="T:PL.CalfTrainer.Infrastructure.Services.IExerciseTrackerDataService" />
	internal class SharedPreferencesExerciseTrackerDataService : IExerciseTrackerDataService
	{
		#region Definitions

		private const string DailyExerciseTrackerKey = "DailyExerciseTracker";
		private readonly ISharedPreferences mSharedPreferences;
		private DailyExerciseTracker mDailyExerciseTracker;

		#endregion Definitions

		#region Constructor(s)

		public SharedPreferencesExerciseTrackerDataService(ISharedPreferences sharedPreferences)
		{
			mSharedPreferences = sharedPreferences;
			LoadDailyExerciseTracker();
		}

		#endregion Constructor(s)

		#region IExerciseTrackerDataService

		/// <inheritdoc />
		public void Add(ExersiseExecution exerciseExecution)
		{
			mDailyExerciseTracker.ExersiseExecutions.Add(exerciseExecution);
			var test = JsonConvert.SerializeObject(mDailyExerciseTracker);

			var editor = mSharedPreferences.Edit();
			editor.PutString(DailyExerciseTrackerKey, test);
			editor.Apply();
		}

		/// <inheritdoc />
		public IList<ExersiseExecution> GetByPeriod(DateTime periodStart, DateTime periodEnd)
		{
			if (mDailyExerciseTracker.Day >= periodStart && mDailyExerciseTracker.Day <= periodEnd)
			{
				return mDailyExerciseTracker.ExersiseExecutions;
			}

			return new List<ExersiseExecution>();
		}

		#endregion IExerciseTrackerDataService

		#region Helper functions

		private void LoadDailyExerciseTracker()
		{
			if (mSharedPreferences.Contains(DailyExerciseTrackerKey))
			{
				var serializedObject = mSharedPreferences.GetString(DailyExerciseTrackerKey, string.Empty);
				mDailyExerciseTracker = JsonConvert.DeserializeObject<DailyExerciseTracker>(serializedObject);
				if (mDailyExerciseTracker.Day != DateTime.Today)
				{
					mDailyExerciseTracker = null;
				}
			}

			if (mDailyExerciseTracker == null)
			{
				mDailyExerciseTracker = new DailyExerciseTracker(DateTime.Today, new List<ExersiseExecution>());
			}
		}

		#endregion Helper functions
	}
}