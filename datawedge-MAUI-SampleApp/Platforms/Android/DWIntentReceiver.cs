using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
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
		public override void OnReceive(Context context, Intent intent)
		{
            System.Console.WriteLine("Here is DW on MAUI");
            if (intent.Extras != null)
            {
                if (intent.HasExtra("com.symbol.datawedge.label_type")) {
                    String bc_type = intent.Extras.GetString("com.symbol.datawedge.label_type");
                    String bc_data = intent.Extras.GetString("com.symbol.datawedge.data_string");

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
