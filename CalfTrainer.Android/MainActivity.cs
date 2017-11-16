using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using PL.CalfTrainer.Business.Services;
using PL.CalfTrainer.Entities;
using PL.CalfTrainer.Infrastructure.EventArgs;
using PL.CalfTrainer.Infrastructure.Services;

namespace CalfTrainer.Android
{
	[Activity(Label = "CalfTrainer", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private IExerciseService mExerciseService;

		private TextView mMainCounter;
		private TextView mCounterLeftLongCalf;
		private TextView mCounterRightLongCalf;
		private TextView mCounterLeftShortCalf;
		private TextView mCounterRightShortCalf;
		private TextView mTotalTimeRemaining;
		private Button mStartPauseButton;
		private Button mStopButton;
		private ProgressBar mProgressBar;

		private const string SavedExerciseStateKey = "SavedExerciseStateKey";
		private const string SavedExerciseConfigurationKey = "SavedExerciseConfigurationKey";

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
			CreateExerciseService(savedInstanceState);
			AttachControlsToMemberVariables();
			AttachControlEventHandler();

			mExerciseService.SendExerciseState();
		}

		public override void OnSaveInstanceState(Bundle outState, PersistableBundle outPersistentState)
		{
			outState.PutString(SavedExerciseStateKey, mExerciseService.StateToString());
			base.OnSaveInstanceState(outState, outPersistentState);
		}

		private void CreateExerciseService(BaseBundle savedInstance)
		{
			var exerciseState = savedInstance?.GetString(SavedExerciseStateKey, string.Empty);

			// Create the exercise service
			mExerciseService = ExerciseService.ExerciseServiceFromString(exerciseState, new ExerciseConfiguration(), new TimerService());

			mExerciseService.ExerciseChanged += ExerciseServiceOnExerciseChanged;
			mExerciseService.ActiveSubExerciseChanged += ExerciseServiceOnActiveSubExerciseChanged;
		}

		private void AttachControlsToMemberVariables()
		{
			mMainCounter = FindViewById<TextView>(Resource.Id.textViewMainCounter);
			mCounterLeftLongCalf = FindViewById<TextView>(Resource.Id.textViewCounterLeftLongCalf);
			mCounterRightLongCalf = FindViewById<TextView>(Resource.Id.textViewCounterRightLongCalf);
			mCounterLeftShortCalf = FindViewById<TextView>(Resource.Id.textViewCounterLeftShortCalf);
			mCounterRightShortCalf = FindViewById<TextView>(Resource.Id.textViewCounterRightShortCalf);
			mTotalTimeRemaining = FindViewById<TextView>(Resource.Id.textViewTotalTimeRemaining);
			mStartPauseButton = FindViewById<Button>(Resource.Id.buttonStart);
			mStopButton = FindViewById<Button>(Resource.Id.buttonStop);
			mProgressBar = FindViewById<ProgressBar>(Resource.Id.totalProgressBar);
		}

		private void AttachControlEventHandler()
		{
			mStartPauseButton.Click += StartPauseButtonOnClick;
			mStopButton.Click += StopButtonOnClick;
		}

		private void StartPauseButtonOnClick(object sender, EventArgs eventArgs)
		{
			if (mExerciseService.IsRunning)
			{
				mExerciseService.Pause();
				mStartPauseButton.Text = Resources.GetString(Resource.String.resume);
			}
			else
			{
				mExerciseService.Start();
				mStartPauseButton.Text = Resources.GetString(Resource.String.pause);
			}

			mStopButton.Enabled = true;
		}

		private void StopButtonOnClick(object sender, EventArgs eventArgs)
		{
			mExerciseService.Stop();
		}

		private void ExerciseServiceOnActiveSubExerciseChanged(object sender, ActiveSubExerciseChangedEventArgs activeSubExerciseChangedEventArgs)
		{
			RunOnUiThread(() =>
			{
				var textViewOldSubExercise = GetTextViewForSubExercise(activeSubExerciseChangedEventArgs.OldSubExercise);
				var textViewNewSubExercise = GetTextViewForSubExercise(activeSubExerciseChangedEventArgs.NewSubExercise);

				textViewOldSubExercise?.SetTextColor(Color.WhiteSmoke);
				textViewNewSubExercise?.SetTextColor(Color.Green);
			});
		}

		private TextView GetTextViewForSubExercise(SubExercise subExercise)
		{
			switch (subExercise)
			{
				case SubExercise.Undefined:
					return null;

				case SubExercise.LongLeft:
					return mCounterLeftLongCalf;

				case SubExercise.ShortLeft:
					return mCounterLeftShortCalf;

				case SubExercise.LongRight:
					return mCounterRightLongCalf;

				case SubExercise.ShortRight:
					return mCounterRightShortCalf;

				default:
					throw new ArgumentOutOfRangeException(nameof(subExercise), subExercise, null);
			}
		}

		private void ExerciseServiceOnExerciseChanged(object sender, ExerciseChangedEventArgs exerciseChangedEventArgs)
		{
			RunOnUiThread(() =>
			{
				var exercise = exerciseChangedEventArgs.Exercise;

				if (exercise.IsDone)
				{
					mStartPauseButton.Text = Resources.GetString(Resource.String.start);
					mStopButton.Enabled = false;
				}

				if (exercise.RemainingPreparationTime > 0)
				{
					mMainCounter.Text = exercise.RemainingPreparationTime.ToString();
					mMainCounter.SetTextColor(Color.OrangeRed);
				}
				else
				{
					mMainCounter.Text = exercise.RemainingSubExerciseTime.ToString();
					mMainCounter.SetTextColor(Color.WhiteSmoke);
				}

				mCounterLeftLongCalf.Text = exercise.LongLeftCount.ToString();
				mCounterRightLongCalf.Text = exercise.LongRightCount.ToString();
				mCounterLeftShortCalf.Text = exercise.ShortLeftCount.ToString();
				mCounterRightShortCalf.Text = exercise.ShortRightCount.ToString();
				mTotalTimeRemaining.Text = string.Format(Resources.GetString(Resource.String.totalTimeRemaining),
					exercise.RemainingTotalTime.ToString(@"mm\:ss"));
				mProgressBar.Progress = exercise.PercentageCompleted;
			});
		}
	}
}