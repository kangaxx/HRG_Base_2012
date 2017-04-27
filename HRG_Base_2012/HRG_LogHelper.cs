using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;


namespace HRG_BaseLibrary_2012
{
    public enum LogBackupType
    {
        LBT_EVERY_DAY = 1,
        LBT_EVERY_MONTH = 2,
        LBT_NONE = 99
    }

    public class HRG_LogHelper
    {
        private string _file;
        private LogBackupType _backup{set; get;} //备份策略， 每天 ， 每个月， 或者从不备份
        public HRG_LogHelper()
        {
            _file = new StringBuilder("C://default.log").ToString();
        }

        public HRG_LogHelper(string file, LogBackupType type = LogBackupType.LBT_EVERY_DAY)
        {
            _file = file;
            _backup = type;
        }

        private void CheckFile()
        {
            FileInfo fi = new FileInfo(_file);
            if (File.Exists(_file) && NeedBackupFile(_file))
            {
                string filename = fi.Name.Replace(fi.Extension, "");
                string backup = fi.FullName.Replace(filename, new StringBuilder(filename + "_" + fi.LastWriteTime.ToShortDateString()).ToString()).Replace("/", "_");
                File.Move(_file, backup);
            }
            if (!Directory.Exists(fi.FullName.Replace(fi.Name, "")))
                Directory.CreateDirectory(fi.FullName.Replace(fi.Name, ""));
        }

        //根据设定的备份参数判断是否需要备份文件，重建新文件
        private bool NeedBackupFile(string _file)
        {
            FileInfo fi = new FileInfo(_file);
            switch(_backup)
            {
                case LogBackupType.LBT_EVERY_DAY:
                    if (fi.LastWriteTime.Date != DateTime.Today)
                        return true;  //按天备份的不是今天的就需要备份
                    break;

                case LogBackupType.LBT_EVERY_MONTH:
                    if (fi.LastWriteTime.Month != DateTime.Today.Month)
                        return true;
                    break;
            }
            return false;
        }

        public void Info(string info)
        {
            try
            {
                CheckFile();

                using (FileStream fs = new FileStream(_file, FileMode.OpenOrCreate))
                {
                    fs.Seek(0, SeekOrigin.End);
                    byte[] infos = System.Text.Encoding.Default.GetBytes(String.Format("[{0}] INFO : {1} \r\n", System.DateTime.Now.ToLocalTime(), info));
                    fs.Write(infos, 0, infos.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            {

            }
        }

        public void Error(string err, string className)
        {
            try
            {
                CheckFile();
                using (FileStream fs = new FileStream(_file, FileMode.OpenOrCreate))
                {
                    fs.Seek(0, SeekOrigin.End);
                    string debugStr;
                    if (String.IsNullOrEmpty(className))
                        debugStr = "！";
                    else
                        debugStr = new StringBuilder("，in class" + className + "！").ToString();
                    byte[] infos = System.Text.Encoding.Default.GetBytes(String.Format("[{0}] ERROR : {1}{2} \r\n", System.DateTime.Now.ToLocalTime(), err, debugStr));
                    fs.Write(infos, 0, infos.Length);
                    fs.Flush();
                    fs.Close();
                }
            }
            catch
            {
                //can do nothing
            }
        }

        public void Debug(string message, string className)
        {
            try
            {
                LogInDebugMode(message, className);
            }
            catch
            {

            }
        }

        [Conditional("DEBUG")]
        private void LogInDebugMode(string message, string className)
        {
            CheckFile();
            using (FileStream fs = new FileStream(_file, FileMode.OpenOrCreate))
            {
                fs.Seek(0, SeekOrigin.End);
                byte[] infos = System.Text.Encoding.Default.GetBytes(String.Format("[{0}] DEBUG : {1},in class {2} \r\n", System.DateTime.Now.ToLocalTime(), message, className));
                fs.Write(infos, 0, infos.Length);
                fs.Flush();
                fs.Close();
            }

        }

    }
}
