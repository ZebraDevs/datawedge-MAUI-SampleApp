using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace datawedge_MAUI_SampleApp.Platforms.Android
{
    public class FilePermissionHelper
    {
        public static Boolean setFilePermissions(String filePath, Boolean readable, Boolean writable, Boolean executable)
        {
            Java.IO.File file = new Java.IO.File(filePath);
            Boolean success = true;
            if (file.Exists())
            {
                if (!file.SetReadable(readable, false)) success = false;
                if (!file.SetWritable(writable, false)) success = false;
                if (!file.SetExecutable(executable, false)) success = false;
            }
            else success = false;
            return success;
        }
    }
}
