using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using CalfTrainer.Android.BuisinessLogic;

namespace CalfTrainer.Android
{
	[Activity(Label = "CalfTrainer", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private ExerciseService mExerciseService;

		private TextView mMainCounter;
		private TextView mCounterLeftLongCalf;
		private TextView mCounterRightLongCalf;
		private TextView mCounterLeftShortCalf;
		private TextView mCounterRightShortCalf;
		private TextView mTotalTimeRemaining;
		private Button mStartStopButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			// Create the exercise service
			mExerciseService = new ExerciseService();
			mExerciseService.ExerciseChanged += ExerciseServiceOnExerciseChanged;
			mExerciseService.ActiveSubExerciseChanged += ExerciseServiceOnActiveSubExerciseChanged;

			// Get the controls
			mMainCounter = FindViewById<TextView>(Resource.Id.textViewMainCounter);
			mCounterLeftLongCalf = FindViewById<TextView>(Resource.Id.textViewCounterLeftLongCalf);
			mCounterRightLongCalf = FindViewById<TextView>(Resource.Id.textViewCounterRightLongCalf);
			mCounterLeftShortCalf = FindViewById<TextView>(Resource.Id.textViewCounterLeftShortCalf);
			mCounterRightShortCalf = FindViewById<TextView>(Resource.Id.textViewCounterRightShortCalf);
			mTotalTimeRemaining = FindViewById<TextView>(Resource.Id.textViewTotalTimeRemaining);
			mStartStopButton = FindViewById<Button>(Resource.Id.buttonStart);

			mStartStopButton.Click += StartStopButtonOnClick;

			// Prepare for a new exercise.
			mExerciseService.Prepare();
		}

		private void StartStopButtonOnClick(object sender, EventArgs eventArgs)
		{
			mStartStopButton.Enabled = false;
			mExerciseService.Start();
		}

		private void ExerciseServiceOnActiveSubExerciseChanged(object sender, ActiveSubExcersizeChangedEventArgs activeSubExcersizeChangedEventArgs)
		{
			RunOnUiThread(() =>
			{
				var textViewOldSubExercise = GetTextViewForSubExercise(activeSubExcersizeChangedEventArgs.OldSubExercise);
				var textViewNewSubExercise = GetTextViewForSubExercise(activeSubExcersizeChangedEventArgs.NewSubExercise);

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
				mTotalTimeRemaining.Text = $@"Total time remaining: {exercise.RemainingTotalTime:mm\:ss}";
			});
		}
	}
}