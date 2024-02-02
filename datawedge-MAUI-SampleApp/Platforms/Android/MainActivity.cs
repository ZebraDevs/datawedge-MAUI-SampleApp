using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Util;
using CommunityToolkit.Mvvm.Messaging;
using datawedge_MAUI_SampleApp.Platforms.Android;
using Java.Lang;
using System.Diagnostics.Metrics;
using System.Security.AccessControl;
using System.Xml.Linq;

namespace datawedge_MAUI_SampleApp;

//WITH CONFIGCHANGES! //[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true)]
public class MainActivity : MauiAppCompatActivity
{
    //protected override void OnCreate(Bundle savedInstanceState) {
    protected override void OnPostCreate(Bundle savedInstanceState)
    { 
        base.OnPostCreate(savedInstanceState);
        RegisterReceivers();




            WeakReferenceMessenger.Default.Register<string>(this, (r, li) =>
            {
                MainThread.BeginInvokeOnMainThread(() => {
                    if (li == "11")
                    {
                        Intent i = new Intent();
                        i.SetAction("com.symbol.datawedge.api.ACTION");
                        i.PutExtra("com.symbol.datawedge.api.SCANNER_INPUT_PLUGIN", "DISABLE_PLUGIN");
                        i.PutExtra("SEND_RESULT", "true");
                        i.PutExtra("COMMAND_IDENTIFIER", "MY_DISABLE_SCANNER");  //Unique identifier
                        this.SendBroadcast(i);
                    }
                    else if (li=="22") {
                        Intent i = new Intent();
                        i.SetAction("com.symbol.datawedge.api.ACTION");
                        i.PutExtra("com.symbol.datawedge.api.SCANNER_INPUT_PLUGIN", "ENABLE_PLUGIN");
                        i.PutExtra("SEND_RESULT", "true");
                        i.PutExtra("COMMAND_IDENTIFIER", "MY_ENABLE_SCANNER");  //Unique identifier
                        this.SendBroadcast(i);
                    }

                });
            });
        //showing saved states 
        try
        {
            string savedDatetime = savedInstanceState.GetString("time");
            if (savedDatetime is not null)
                WeakReferenceMessenger.Default.Send("Saved DateTime=" + savedDatetime);

        }
        catch(System.Exception ex) {
            WeakReferenceMessenger.Default.Send("No previously saved instance available");
        }
    }


    protected override void OnSaveInstanceState(Bundle outState)
    {
        string currentDatetime = DateTime.Now.ToString();
        outState.PutString("time", currentDatetime);
        base.OnSaveInstanceState(outState);
    }

    void RegisterReceivers()
	{
		IntentFilter filter = new IntentFilter();
		filter.AddCategory("android.intent.category.DEFAULT");
		filter.AddAction("com.ndzl.DW");
		filter.AddAction("com.zebra.sensors");

		Intent regres = RegisterReceiver(new DWIntentReceiver(), filter);
	}
}
