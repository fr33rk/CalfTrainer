using System;
using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Widget;
using Android.OS;
using CalfTrainer.Android.BuisinessLogic;

namespace CalfTrainer.Android
{
	[Activity(Label = "CalfTrainer", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		private ExerciseService mExerciseService;

		private TextView mCounterLeftLongCalf;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);

			mExerciseService = new ExerciseService();
			mExerciseService.ExerciseChanged += ExerciseServiceOnExerciseChanged;
			mExerciseService.ActiveSubExerciseChanged += ExerciseServiceOnActiveSubExerciseChanged;

			mCounterLeftLongCalf = FindViewById<TextView>(Resource.Id.textViewCounterLeftLongCalf);

			mExerciseService.Prepare();
		}

		private void ExerciseServiceOnActiveSubExerciseChanged(object sender, ActiveSubExcersizeChangedEventArgs activeSubExcersizeChangedEventArgs)
		{
			mCounterLeftLongCalf.SetTextColor(Color.Green);
		}

		private void ExerciseServiceOnExerciseChanged(object sender, ExerciseChangedEventArgs exerciseChangedEventArgs)
		{
			mCounterLeftLongCalf.Text = exerciseChangedEventArgs.Exercise.LongLeftCount.ToString();
		}


	}
}

