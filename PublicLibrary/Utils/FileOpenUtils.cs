using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PublicLibrary.Utils
{
   public class FileOpenUtils
    {
       
      
        public static void  OpentFileByFullPath(string fileName)
        {
            Process MyProcess = new Process();
            MyProcess.StartInfo.FileName = @fileName;
            MyProcess.StartInfo.Verb = "Open";
            MyProcess.StartInfo.CreateNoWindow = true;
            MyProcess.Start();
        }


    }
}
