using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.Core;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace datawedge_MAUI_SampleApp.Platforms.Android
{

[BroadcastReceiver(Enabled = true, Exported =true)]
    public class DWIntentReceiver : BroadcastReceiver
    {

        public void ListExtrasKeys(Intent intent)
        {
            Bundle extras = intent.Extras;
            if (extras != null)
            {
                foreach (var key in extras.KeySet())
                {
                    Log.Debug("", "Extra key: " + key);
                }
            }
            else
            {
                Log.Debug("", "No extras found");
            }
        }

        public void DWDecodeData(Intent _intent)
        {

            var jual = _intent.Extras.Get("com.symbol.datawedge.decode_data");

            var javaList = jual as JavaList;

            if (javaList != null)
            {
                for (int i = 0; i < javaList.Size(); i++)
                {
                    byte[] bytes = (byte[])javaList.Get(i);
                    foreach (var item in bytes)
                    {
                        Log.Info("decode_data", ""+item);
                    }


                }
            }

        }

        public override void OnReceive(Context context, Intent intent)
        {
            System.Console.WriteLine("Here is DW on MAUI");
            if (intent.Extras != null)
            {
                if (intent.HasExtra("com.symbol.datawedge.barcodes"))
                {
                    WeakReferenceMessenger.Default.Send("NG SIMULSCAN - MULTIBARCODE");
                    List<Bundle> palobs = intent.Extras.GetParcelableArrayList("com.symbol.datawedge.barcodes").Cast<Bundle>().ToList();
                    StringBuilder sb = new StringBuilder();
                    foreach (Bundle b in palobs)
                    {
                        String barcode = b.GetString("com.symbol.datawedge.data_string");
                        String timestamp = b.GetString("com.symbol.datawedge.timestamp");
                        String symbology = b.GetString("com.symbol.datawedge.label_type");
                        sb.AppendLine("-"+symbology + " " + barcode);
                    }
                    WeakReferenceMessenger.Default.Send(sb.ToString());
                }
                else if (intent.HasExtra("com.symbol.datawedge.label_type"))
                {
                    String bc_type = intent.Extras.GetString("com.symbol.datawedge.label_type");
                    String bc_data = intent.Extras.GetString("com.symbol.datawedge.data_string");

                    DWDecodeData(intent);



                    WeakReferenceMessenger.Default.Send(bc_type + " " + bc_data);
                }
                else if (intent.HasExtra("com.symbol.datawedge.api.RESULT_GET_ACTIVE_PROFILE"))
                {
                    String activeProfile = intent.Extras.GetString("com.symbol.datawedge.api.RESULT_GET_ACTIVE_PROFILE");

                    try
                    {
                        WeakReferenceMessenger.Default.Send("ACTIVE PROFILE=<" + activeProfile + ">");
                    }
                    catch (Exception e) { }
                }
                else if (intent.HasExtra("com.symbol.datawedge.api.RESULT_GET_PROFILES_LIST"))
                {
                    String[] profilesList = intent.Extras.GetStringArray("com.symbol.datawedge.api.RESULT_GET_PROFILES_LIST");

                    try
                    {
                        WeakReferenceMessenger.Default.Send("profilesList size =<" + profilesList.Length + ">");
                    }
                    catch (Exception e) { }
                }
            }
        }

    }
}
