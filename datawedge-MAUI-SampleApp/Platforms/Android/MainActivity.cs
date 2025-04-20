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
using System.Security.Principal;
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
        WeakReferenceMessenger.Default.Send(DisplayDotNetVersion());
        WeakReferenceMessenger.Default.Send(DisplayTargetApiLevel());




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
                    else if (li == "22") {
                        Intent i = new Intent();
                        i.SetAction("com.symbol.datawedge.api.ACTION");
                        i.PutExtra("com.symbol.datawedge.api.SCANNER_INPUT_PLUGIN", "ENABLE_PLUGIN");
                        i.PutExtra("SEND_RESULT", "true");
                        i.PutExtra("COMMAND_IDENTIFIER", "MY_ENABLE_SCANNER");  //Unique identifier
                        this.SendBroadcast(i);
                    }
                    else if (li == "33") {

                        global::Android.Content.Intent i = new global::Android.Content.Intent();
                        i.SetAction("com.symbol.datawedge.api.ACTION");
                        i.PutExtra("com.symbol.datawedge.api.GET_ACTIVE_PROFILE", "" );
                        //i.PutExtra("com.symbol.datawedge.api.GET_PROFILES_LIST", "" );
                        SendBroadcast(i);
                    }
                    else if (li == "44") {
                        ImportProfile("dwprofile_com.ndzl.dwmaui");
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

    private string DisplayDotNetVersion()
    {
        return "Current .NET version:"+ System.Environment.Version;

    }

    private string DisplayTargetApiLevel()
    {
        var packageInfo = PackageManager.GetPackageInfo(PackageName, 0);
        var targetSdkVersion = packageInfo.ApplicationInfo.TargetSdkVersion;
        return "Current Target API Level: " + targetSdkVersion;
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
        filter.AddAction("com.symbol.datawedge.api.RESULT_ACTION");

        Intent regres = AndroidX.Core.Content.ContextCompat.RegisterReceiver(this, new DWIntentReceiver(), filter, AndroidX.Core.Content.ContextCompat.ReceiverExported);
    }

    private void ImportProfile(string profileFilenameWithoutDbExtension)
    {
        // Define directories and file names
        // /enterprise/device/settings/datawedge/autoimport
        string autoImportDir = "/enterprise/device/settings/datawedge/autoimport/";
        string temporaryFileName = profileFilenameWithoutDbExtension + ".tmp";
        string finalFileName = profileFilenameWithoutDbExtension + ".db";

        Stream inputStream = null;
        FileStream fileOutputStream = null;
        FileInfo outputFile = null;
        FileInfo finalFile = null;

        try
        {
            // Access the asset stream from the application assets
            inputStream = FileSystem.Current.OpenAppPackageFileAsync(finalFileName).Result;

            // Create a directory for the output if it doesn't exist
            DirectoryInfo outputDirectory = new DirectoryInfo(autoImportDir);
            if (!outputDirectory.Exists)
            {
                outputDirectory.Create();
            }

            // Create temporary and final file objects
            outputFile = new FileInfo(Path.Combine(outputDirectory.FullName, temporaryFileName));
            finalFile = new FileInfo(Path.Combine(outputDirectory.FullName, finalFileName));

            // Create a FileStream for the temporary output file
            fileOutputStream = new FileStream(outputFile.FullName, FileMode.Create, FileAccess.Write);

            // Transfer bytes from the input stream to the output stream
            byte[] buffer = new byte[1024];
            int length;
            int tot = 0;
            while ((length = inputStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileOutputStream.Write(buffer, 0, length);
                tot += length;
            }
            Console.WriteLine($"{tot} bytes copied");

            // Flush and close the output stream
            fileOutputStream.Flush();
        }
        catch (Java.Lang.Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            // Ensure the stream is properly closed
            fileOutputStream?.Close();

            // Set permissions and rename the file if applicable
            if (outputFile != null)
            {
                // Rename the temporary file to the final file
                if (finalFile != null)
                {
                    if (outputFile.Exists)
                    {
                        if(finalFile.Exists)
                        {
                            finalFile.Delete();
                        }

                        FilePermissionHelper.setFilePermissions(outputFile.FullName, true, true, true);
                        outputFile.MoveTo(finalFile.FullName);
                    }
                }
            }
        }
    }
}
