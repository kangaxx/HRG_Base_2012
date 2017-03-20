using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace HRG_BaseLibrary_2012
{
    public class HRG_LogHelper
    {
        private string _file;
        public HRG_LogHelper()
        {
            _file = new StringBuilder("C://default.log").ToString();
        }

        public HRG_LogHelper(string file)
        {
            _file = file;
        }

        public void Info(string info)
        {
            FileStream fs = new FileStream(_file, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.End);
            byte [] infos = System.Text.Encoding.Default.GetBytes(String.Format("[{0}] : {1} \r\n", System.DateTime.Now.ToLocalTime(), info));
            fs.Write(infos, 0, infos.Length);
            fs.Flush();
            fs.Close();
        }
    }
}
