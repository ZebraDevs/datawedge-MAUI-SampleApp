using Android.App;
using Android.Content;
using Android.Nfc;
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


using Newtonsoft.Json.Linq;
using Android.Graphics;
using System.Text.RegularExpressions;





namespace datawedge_MAUI_SampleApp
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

            var jual = _intent.Extras.Get ("com.symbol.datawedge.decode_data");

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

                Bundle extras = intent.Extras;
                foreach (var key in extras.KeySet())
                {
                    var value = extras.Get(key);
                    Log.Debug("IntentExtras", $"Key: {key}, Value: {value}");
                }
                

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
                        sb.AppendLine("-" + symbology + " " + barcode);
                    }
                    WeakReferenceMessenger.Default.Send(sb.ToString());
                }
                else if (intent.HasExtra("com.symbol.datawedge.label_type"))
                {
                    String bc_type = intent.Extras.GetString("com.symbol.datawedge.label_type");
                    String bc_data = intent.Extras.GetString("com.symbol.datawedge.data_string");

                    if(System.Environment.Version.Major<10)
                        DWDecodeData(intent); //shows in logcat the raw decode_data (so capturing all non-printable chars)
                    //else if(System.Environment.Version.Major==10)
                       



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
                else if (intent.HasExtra("workflow_name") && intent.GetStringExtra("workflow_name").Equals("free_form_capture")) {
                    var bundle = intent.Extras;
                    string jsonData = bundle?.GetString("com.symbol.datawedge.data");

                    try
                    {
                        var jsonArray = JArray.Parse(jsonData);
                        foreach (var item in jsonArray)
                        {
                            var jsonObject = (JObject)item;
                            if (jsonObject.ContainsKey("string_data"))
                            {
                                String bcRes = OutputDecodeData(jsonArray);
                                try
                                {
                                    WeakReferenceMessenger.Default.Send("FREEFORM IMAGE CAPTURE\n" + bcRes);
                                }
                                catch (Exception e) { }
                            }
                            else if (jsonObject.ContainsKey("uri"))
                            {
                                string uri = jsonObject["uri"]?.ToString();
                                OutputImageData(context, uri, jsonObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("DW FREEFORM CAPTURE", $"Error receiving data: {ex.Message}");
                    }

                }
                else if (intent.HasExtra("workflow_name") && intent.GetStringExtra("workflow_name").Equals("document_capture"))
                {
                    var bundle = intent.Extras;
                    string jsonData = bundle?.GetString("com.symbol.datawedge.data");

                    try
                    {
                        var jsonArray = JArray.Parse(jsonData);
                        foreach (var item in jsonArray)
                        {
                            var jsonObject = (JObject)item;
                            if (jsonObject.ContainsKey("string_data"))
                            {

                                String bcRes = OutputDecodeData(jsonArray);
                                try
                                {
                                    WeakReferenceMessenger.Default.Send("DOCUMENT CAPTURE\n" + bcRes);
                                }
                                catch (Exception e) { }
                            }
                            else 
                            if (jsonObject.ContainsKey("uri"))
                            {
                                string uri = jsonObject["uri"]?.ToString();
                                OutputImageData(context, uri, jsonObject);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("DW DOCUMENT CAPTURE", $"Error receiving data: {ex.Message}");
                    }

                }

            }
        }



        private String OutputDecodeData(JArray jsonArray)
        {
            var sboutput = new StringBuilder();

            StringBuilder sb = new StringBuilder();
            foreach (var item in jsonArray)
                {
                    var jsonObject = (JObject)item;
                    string stringData = jsonObject["string_data"]?.ToString();
                    if (!string.IsNullOrEmpty(stringData))
                    {
                        string groupID = jsonObject["group_id"]?.ToString() ?? "N/A";
                        string barcodeType = jsonObject["barcodetype"]?.ToString() ?? "N/A";
                        int dataSize = jsonObject["data_size"]?.ToObject<int>() ?? 0;

                        sboutput.AppendLine($" {stringData}-{barcodeType}");
                }
            }
            return sboutput.ToString();

        }

        private Bitmap ConvertYUVToBitmap(byte[] yuvData, int width, int height, string yuvFormat)
        {
            // Determine the YUV format (e.g., NV21)
            ImageFormatType imageFormat = yuvFormat.Equals("NV21", StringComparison.OrdinalIgnoreCase)
                ? ImageFormatType.Nv21
                : ImageFormatType.Yuv420888; // Default to YUV420

            // Create a YuvImage instance
            YuvImage yuvImage = new YuvImage(yuvData, imageFormat, width, height, null);

            // Convert YUV to JPEG
            using (var outputStream = new MemoryStream())
            {
                yuvImage.CompressToJpeg(new Android.Graphics.Rect(0, 0, width, height), 100, outputStream);

                // Decode the JPEG data into a Bitmap
                byte[] jpegData = outputStream.ToArray();
                return BitmapFactory.DecodeByteArray(jpegData, 0, jpegData.Length);
            }
        }

        private string OutputDocumentStringData(Context context, string uri, JObject jsonObject)
        {
            try
            {
                var cursor = context.ContentResolver.Query(Android.Net.Uri.Parse(uri), null, null, null, null);
                using var baos = new MemoryStream();

                if (cursor != null && cursor.MoveToFirst())
                {
                    //for (int i = 0; i < cursor.ColumnCount; i++)
                    //{
                    //    string col = cursor.GetColumnName(i);
                    //}

                    int rawDataIndex = cursor.GetColumnIndex("raw_data");
                    int nextUriIndex = cursor.GetColumnIndex("next_data_uri");

                    baos.Write(cursor.GetBlob(rawDataIndex));
                    if (nextUriIndex > 0)
                    {
                        string nextUri = cursor.GetString(nextUriIndex);

                        while (!string.IsNullOrEmpty(nextUri))
                        {
                            var nextCursor = context.ContentResolver.Query(Android.Net.Uri.Parse(nextUri), null, null, null, null);
                            if (nextCursor != null && nextCursor.MoveToFirst())
                            {
                                baos.Write(nextCursor.GetBlob(rawDataIndex));
                                nextUri = nextCursor.GetString(nextUriIndex);
                                nextCursor.Close();
                            }
                        }
                    }

                    cursor.Close();
                    baos.Position = 0; // Reset to the beginning
                    using var reader = new StreamReader(baos, Encoding.UTF8);
                    string result = reader.ReadToEnd();
                    return result.ToString();
                }
                else
                    return "No data found";

            }
            catch (Exception ex)
            {
                Log.Error("DOCUMENT CAPTURE", $"Error processing image data: {ex.Message}");
                return "ERROR";
            }
        }

        private void OutputImageData(Context context, string uri, JObject jsonObject)
        {
            try
            {

                var cursor = context.ContentResolver.Query( Android.Net.Uri.Parse(uri), null, null, null, null);
                using var baos = new MemoryStream();

                if (cursor != null && cursor.MoveToFirst())
                {
                    //for (int i = 0; i < cursor.ColumnCount; i++)
                    //{
                    //    string col = cursor.GetColumnName(i);
                    //}

                    int rawDataIndex = cursor.GetColumnIndex("raw_data");
                    int nextUriIndex = cursor.GetColumnIndex("next_data_uri");

                    baos.Write(cursor.GetBlob(rawDataIndex));
                    if (nextUriIndex > 0) { 
                        string nextUri = cursor.GetString(nextUriIndex);

                        while (!string.IsNullOrEmpty(nextUri))
                        {
                            var nextCursor = context.ContentResolver.Query(Android.Net.Uri.Parse(nextUri), null, null, null, null);
                            if (nextCursor != null && nextCursor.MoveToFirst())
                            {
                                baos.Write(nextCursor.GetBlob(rawDataIndex));
                                nextUri = nextCursor.GetString(nextUriIndex);
                                nextCursor.Close();
                            }
                        }
                    }

                    cursor.Close();
                }

                int size = jsonObject["size"]?.ToObject<int>() ?? 0;
                int width = jsonObject["width"]?.ToObject<int>() ?? 0;
                int height = jsonObject["height"]?.ToObject<int>() ?? 0;
                int stride = jsonObject["stride"]?.ToObject<int>() ?? 0;
                int orientation = jsonObject["orientation"]?.ToObject<int>() ?? 0;
                string imageFormat = jsonObject["imageformat"]?.ToString();

                baos.Position = 0; // Reset the stream position to the beginning
                //var bitmap = BitmapFactory.DecodeStream(baos);
                var bitmap = ConvertYUVToBitmap(baos.ToArray(), width, height, "NV21");

                //save bitmap to picture folder
                string path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures).AbsolutePath;
                string fileName = $"Image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                string filePath = System.IO.Path.Combine(path, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    bitmap.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
                }
                bitmap.Dispose();
                baos.Dispose();
                WeakReferenceMessenger.Default.Send($"Image saved to {filePath}");

            }
            catch (Exception ex)
            {
                Log.Error("DW FREEFORM CAPTURE", $"Error processing image data: {ex.Message}");
            }
        }
    }



}

    

