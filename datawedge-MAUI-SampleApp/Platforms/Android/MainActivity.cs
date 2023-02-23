using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using datawedge_MAUI_SampleApp.Platforms.Android;

namespace datawedge_MAUI_SampleApp;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState) {
		base.OnCreate(savedInstanceState);
		RegisterReceivers();
	}


	void RegisterReceivers()
	{
		IntentFilter filter = new IntentFilter();
		filter.AddCategory("android.intent.category.DEFAULT");
		filter.AddAction("com.ndzl.DW");

		Intent regres = RegisterReceiver(new DWIntentReceiver(), filter);
	}
}
