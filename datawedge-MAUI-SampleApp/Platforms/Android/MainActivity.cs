using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using CommunityToolkit.Mvvm.Messaging;
using datawedge_MAUI_SampleApp.Platforms.Android;
using System.Diagnostics.Metrics;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace datawedge_MAUI_SampleApp;

//WITH CONFIGCHANGES! //[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : MauiAppCompatActivity
{
	protected override void OnCreate(Bundle savedInstanceState) {
        base.OnCreate(savedInstanceState);
        RegisterReceivers();

        //showing saved states 
        try
        {
            String savedDatetime = savedInstanceState.GetString("time");
            if (savedDatetime is not null)
                WeakReferenceMessenger.Default.Send("Saved DateTime=" + savedDatetime);
        }catch(Exception ex) {
            WeakReferenceMessenger.Default.Send("No previously saved instance available");
        }
    }


    protected override void OnSaveInstanceState(Bundle outState)
    {
        String currentDatetime = DateTime.Now.ToString();
        outState.PutString("time", currentDatetime);
        base.OnSaveInstanceState(outState);
    }

    void RegisterReceivers()
	{
		IntentFilter filter = new IntentFilter();
		filter.AddCategory("android.intent.category.DEFAULT");
		filter.AddAction("com.ndzl.DW");

		Intent regres = RegisterReceiver(new DWIntentReceiver(), filter);
	}
}
