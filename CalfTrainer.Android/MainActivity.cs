using Android.App;
using Android.Content.PM;
using Android.Widget;
using Android.OS;

namespace CalfTrainer.Android
{
	[Activity(Label = "CalfTrainer", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : Activity
	{
		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.Main);
		}
	}
}

