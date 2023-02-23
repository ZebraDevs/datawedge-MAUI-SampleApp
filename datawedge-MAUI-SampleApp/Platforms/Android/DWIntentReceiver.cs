using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace datawedge_MAUI_SampleApp.Platforms.Android
{

	[BroadcastReceiver(Enabled = true, Exported =true)]
	[IntentFilter(new[] { "com.ndzl.DW" })]
	public class DWIntentReceiver : BroadcastReceiver
	{
		public override void OnReceive(Context context, Intent intent)
		{
			//System.Console.WriteLine("Here is DW on MAUI");
			if (intent.Extras != null)
			{
				String bc_type = intent.Extras.GetString("com.symbol.datawedge.label_type");
				String bc_data = intent.Extras.GetString("com.symbol.datawedge.data_string");

				WeakReferenceMessenger.Default.Send(bc_type + " "+ bc_data);
			}


		}
	}
}
