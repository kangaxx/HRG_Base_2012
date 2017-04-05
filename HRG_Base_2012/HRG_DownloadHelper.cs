using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HRG_BaseLibrary_2012
{
    public enum DOWNLOADER_TYPE
    {
        NET_FTP = 0,
        HTTP = 1,
        OTHER = 8
    }

    public interface IDownloader
    {

        void CreateDirectory(string remoteDirectoryName);

        /// <summary>  
        /// 更改目录或文件名  
        /// </summary>  
        /// <param name="currentName">当前名称</param>  
        /// <param name="newName">修改后新名称</param>  
        void Rename(string currentName, string newName);

        /// <summary>  
        /// 删除目录(包括下面所有子目录和子文件)  
        /// </summary>  
        /// <param name="remoteDirectoryName">要删除的带路径目录名：如web/test</param>  
        /* 
         * 例：删除test目录 
         FTPHelper helper = new FTPHelper("x.x.x.x", "web", "user", "password");                   
         helper.RemoveDirectory("web/test"); 
         */
        void RemoveDirectory(string remoteDirectoryName);

        /// <summary>    
        /// 切换当前目录    
        /// </summary>    
        /// <param name="IsRoot">true:绝对路径 false:相对路径</param>     
        void GotoDirectory(string DirectoryName, bool IsRoot);

        /// <summary>
        /// 如果调用过gotodirectory，则goback会返回goto之前所在的路径
        /// </summary>
        void GoBack();


        /// <summary>  
        /// 文件上传  
        /// </summary>  
        /// <param name="localFilePath">本地文件路径</param>  
        void Upload(string localFilePath);

        /// <summary>  
        /// 获取当前目录的文件和一级子目录信息  
        /// </summary>  
        /// <returns></returns>  
        List<FileStruct> ListFilesAndDirectories();

        /// <summary>    
        /// 删除文件    
        /// </summary>    
        /// <param name="remoteFileName">要删除的文件名</param>  
        void DeleteFile(string remoteFileName);

        /// <summary>         
        /// 列出当前目录的所有文件         
        /// </summary>         
        List<FileStruct> ListFiles();

        /// <summary>         
        /// 列出当前目录的所有一级子目录         
        /// </summary>         
        List<FileStruct> ListDirectories();

        /// <summary>         
        /// 判断当前目录下指定的子目录或文件是否存在         
        /// </summary>         
        /// <param name="remoteName">指定的目录或文件名</param>        
        bool IsExist(string remoteName);

        /// <summary>         
        /// 判断当前目录下指定的一级子目录是否存在         
        /// </summary>         
        /// <param name="RemoteDirectoryName">指定的目录名</param>        
        bool IsDirectoryExist(string remoteDirectoryName);

        /// <summary>         
        /// 判断当前目录下指定的子文件是否存在        
        /// </summary>         
        /// <param name="RemoteFileName">远程文件名</param>         
        bool IsFileExist(string remoteFileName);

        /// <summary>  
        /// 下载  
        /// </summary>  
        /// <param name="saveFilePath">下载后的保存路径</param>  
        /// <param name="downloadFileName">要下载的文件名</param>  
        void Download(string saveFilePath, string downloadFileName);

        /// <summary>
        /// 读取指定文件的文件内容
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string ReadFile(string fileName);
    }

    public class FtpDownloader : IDownloader, IDisposable
    {
        /// <summary>  
        /// FTP请求对象  
        /// </summary>  
        FtpWebRequest request = null;

        NetworkCredential _credential = null;
        /// <summary>  
        /// FTP响应对象  
        /// </summary>  
        FtpWebResponse response = null;
        /// <summary>  
        /// FTP服务器地址  
        /// </summary>  
        public string ftpURI { get; private set; }

        /// <summary>  
        /// FTP服务器IP  
        /// </summary>  
        public string ftpServerIP { get; private set; }

        /// <summary>  
        /// FTP服务器默认目录  
        /// </summary>  
        public string ftpRemotePath { get; private set; }


        /// <summary>  
        /// FTP服务器登录用户名  
        /// </summary>  
        public string ftpUserID { get; private set; }
        /// <summary>  
        /// FTP服务器登录密码  
        /// </summary>  
        public string ftpPassword { get; private set; }

        public FtpDownloader(string serverIP, string remotePath, string userID, string password)
        {
            ftpServerIP = serverIP;
            ftpRemotePath = remotePath;
            ftpUserID = userID;
            ftpPassword = password;
            _credential = new NetworkCredential(this.ftpUserID, this.ftpPassword);
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }
                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
            }
        }

        ~FtpDownloader()
        {

        }

        /// <summary>  
        /// 建立FTP链接,返回响应对象  
        /// </summary>  
        /// <param name="uri">FTP地址</param>  
        /// <param name="ftpMethod">操作命令</param>  
        /// <returns></returns>  
        private FtpWebResponse Open(Uri uri, string ftpMethod)
        {

            request = (FtpWebRequest)FtpWebRequest.Create(uri);
            request.Method = ftpMethod;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Credentials = _credential;
            try
            {
                FtpWebResponse result = null;
                while (result == null)
                    result = (FtpWebResponse)request.GetResponse();
                return result;
            }
            catch (System.Net.WebException exp)
            {
                Console.WriteLine(exp.Status.ToString());
                return null;
            }
        }


        /// <summary>         
        /// 建立FTP链接,返回请求对象         
        /// </summary>        
        /// <param name="uri">FTP地址</param>         
        /// <param name="ftpMethod">操作命令</param>         
        private FtpWebRequest OpenRequest(Uri uri, string ftpMethod)
        {
            request = (FtpWebRequest)WebRequest.Create(uri);
            request.Method = ftpMethod;
            request.UseBinary = true;
            request.KeepAlive = false;
            request.Credentials = _credential;
            return request;
        }


        /// <summary>  
        /// 创建目录  
        /// </summary>  
        /// <param name="remoteDirectoryName">目录名</param>  
        public void CreateDirectory(string remoteDirectoryName)
        {
            Open(new Uri(ftpURI + remoteDirectoryName), WebRequestMethods.Ftp.MakeDirectory);
        }
        /// <summary>  
        /// 更改目录或文件名  
        /// </summary>  
        /// <param name="currentName">当前名称</param>  
        /// <param name="newName">修改后新名称</param>  
        public void Rename(string currentName, string newName)
        {
            request = OpenRequest(new Uri(ftpURI + currentName), WebRequestMethods.Ftp.Rename);
            request.RenameTo = newName;
        }
        /// <summary>    
        /// 切换当前目录    
        /// </summary>    
        /// <param name="IsRoot">true:绝对路径 false:相对路径</param>     
        public void GotoDirectory(string DirectoryName, bool IsRoot)
        {
            if (IsRoot)
                ftpRemotePath = DirectoryName;
            else
                ftpRemotePath += "/" + DirectoryName;

            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";
        }

        public void GoBack()
        {
            ftpRemotePath = ftpRemotePath.Substring(0, ftpRemotePath.LastIndexOf('/'));
            ftpURI = "ftp://" + ftpServerIP + "/" + ftpRemotePath + "/";

        }
        /// <summary>  
        /// 删除目录(包括下面所有子目录和子文件)  
        /// </summary>  
        /// <param name="remoteDirectoryName">要删除的带路径目录名：如web/test</param>  
        /* 
         * 例：删除test目录 
         FTPHelper helper = new FTPHelper("x.x.x.x", "web", "user", "password");                   
         helper.RemoveDirectory("web/test"); 
         */
        public void RemoveDirectory(string remoteDirectoryName)
        {
            GotoDirectory(remoteDirectoryName, true);
            var listAll = ListFilesAndDirectories();
            foreach (var m in listAll)
            {
                if (m.IsDirectory)
                    RemoveDirectory(m.Path);
                else
                    DeleteFile(m.Name);
            }
            GotoDirectory(remoteDirectoryName, true);
            Open(new Uri(ftpURI), WebRequestMethods.Ftp.RemoveDirectory);
        }
        /// <summary>  
        /// 文件上传  
        /// </summary>  
        /// <param name="localFilePath">本地文件路径</param>  
        public void Upload(string localFilePath)
        {
            FileInfo fileInf = new FileInfo(localFilePath);
            request = OpenRequest(new Uri(ftpURI + fileInf.Name), WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            using (var fs = fileInf.OpenRead())
            {
                using (var strm = request.GetRequestStream())
                {
                    contentLen = fs.Read(buff, 0, buffLength);
                    while (contentLen != 0)
                    {
                        strm.Write(buff, 0, contentLen);
                        contentLen = fs.Read(buff, 0, buffLength);
                    }
                }
            }
        }
        /// <summary>    
        /// 删除文件    
        /// </summary>    
        /// <param name="remoteFileName">要删除的文件名</param>  
        public void DeleteFile(string remoteFileName)
        {
            Open(new Uri(ftpURI + remoteFileName), WebRequestMethods.Ftp.DeleteFile);
        }

        /// <summary>  
        /// 获取当前目录的文件和一级子目录信息  
        /// </summary>  
        /// <returns></returns>  
        public List<FileStruct> ListFilesAndDirectories()
        {
            var fileList = new List<FileStruct>();
            FtpWebResponse resp = null;
            while (resp == null)
                resp = Open(new Uri(ftpURI), WebRequestMethods.Ftp.ListDirectoryDetails);
            
            using (var stream = resp.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    string line = null;
                    try
                    {
                        while (sr != null && (line = sr.ReadLine()) != null)
                        {
                            //line的格式如下：  
                            //08-18-13  11:05PM       <DIR>          aspnet_client  
                            //09-22-13  11:39PM                 2946 Default.aspx  
                            //DateTime dtDate = DateTime.ParseExact(line.Substring(0, 8), "MM-dd-yy", null);
                            //DateTime dtDateTime = DateTime.Parse(dtDate.ToString("yyyy-MM-dd") + line.Substring(8, 9));
                            string[] arrs = line.Split(' ');
                            bool isDir;
                            if (arrs.Length > 0)
                                isDir = arrs[0].IndexOf("d") >= 0 ? true : false;
                            else
                                isDir = false;
                            var model = new FileStruct()
                            {
                                IsDirectory = isDir,
                                //CreateTime = dtDateTime,
                                Name = arrs[arrs.Length - 1],
                                Path = ftpRemotePath + "/" + arrs[arrs.Length - 1]
                            };
                            fileList.Add(model);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return fileList;
        }
        /// <summary>         
        /// 列出当前目录的所有文件         
        /// </summary>         
        public List<FileStruct> ListFiles()
        {
            return ListFiles_FileZilla();
        }

        /// <summary>
        /// 列出所有文件，因为filezilla情况特殊需要用复杂的方法来获取文件名
        /// </summary>
        /// <returns></returns>
        private List<FileStruct> ListFiles_FileZilla()
        {
            var fileList = new List<string>();
            FtpWebResponse resp = null;
            while (resp == null)
                resp = Open(new Uri(ftpURI), WebRequestMethods.Ftp.ListDirectory);
            using (var stream = resp.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    string line = null;
                    try
                    {
                        while (sr != null && (line = sr.ReadLine()) != null)
                        {
                            fileList.Add(line);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            var dirList = ListDirectoryNames();
            foreach (string dirName in dirList)
            {
                fileList.Remove(dirName);
            }

            var result = new List<FileStruct>();

            foreach (string fileName in fileList)
            {

                var model = new FileStruct()
                {
                    IsDirectory = false,
                    //CreateTime = dtDateTime,
                    Name = fileName,
                    Path = ftpRemotePath + "/" + fileName
                };
                result.Add(model);
            }
            return result;
        }

        /// <summary>         
        /// 列出当前目录的所有一级子目录         
        /// </summary>         
        public List<FileStruct> ListDirectories()
        {
            var listAll = ListFilesAndDirectories();
            var listFile = listAll.Where(m => m.IsDirectory == true).ToList();
            return listFile;
        }

        /// <summary>
        /// 直接列出文件夹的名字
        /// </summary>
        /// <returns></returns>
        private List<string> ListDirectoryNames()
        {
            var listDirs = new List<string>();
            FtpWebResponse resp = null;
            while (resp == null)
                resp = Open(new Uri(ftpURI), WebRequestMethods.Ftp.ListDirectoryDetails);
            using (var stream = resp.GetResponseStream())
            {
                using (var sr = new StreamReader(stream))
                {
                    string line = null;
                    try
                    {
                        while (sr != null && (line = sr.ReadLine()) != null)
                        {
                            //line的格式如下：  
                            //08-18-13  11:05PM       <DIR>          aspnet_client  
                            //09-22-13  11:39PM                 2946 Default.aspx  
                            //DateTime dtDate = DateTime.ParseExact(line.Substring(0, 8), "MM-dd-yy", null);
                            //DateTime dtDateTime = DateTime.Parse(dtDate.ToString("yyyy-MM-dd") + line.Substring(8, 9));
                            string[] arrs = line.Split(' ');
                            if (arrs[0].IndexOf("d") >= 0)
                                listDirs.Add(arrs[arrs.Length - 1]);
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return listDirs;
        }

        /// <summary>         
        /// 判断当前目录下指定的子目录或文件是否存在         
        /// </summary>         
        /// <param name="remoteName">指定的目录或文件名</param>        
        public bool IsExist(string remoteName)
        {
            var list = ListFilesAndDirectories();
            if (list.Count(m => m.Name == remoteName) > 0)
                return true;
            return false;
        }
        /// <summary>         
        /// 判断当前目录下指定的一级子目录是否存在         
        /// </summary>         
        /// <param name="RemoteDirectoryName">指定的目录名</param>        
        public bool IsDirectoryExist(string remoteDirectoryName)
        {
            var listDir = ListDirectories();
            if (listDir.Count(m => m.Name == remoteDirectoryName) > 0)
                return true;
            return false;
        }
        /// <summary>         
        /// 判断当前目录下指定的子文件是否存在        
        /// </summary>         
        /// <param name="RemoteFileName">远程文件名</param>         
        public bool IsFileExist(string remoteFileName)
        {
            var listFile = ListFiles();
            if (listFile.Count(m => m.Name == remoteFileName) > 0)
                return true;
            return false;
        }

        /// <summary>  
        /// 下载  
        /// </summary>  
        /// <param name="saveFilePath">下载后的保存路径</param>  
        /// <param name="downloadFileName">要下载的文件名</param>  
        public void Download(string saveFilePath, string downloadFileName)
        {
            using (FileStream outputStream = new FileStream(saveFilePath + "\\" + downloadFileName, FileMode.Create))
            {
                FtpWebResponse resp = null;
                while (resp == null)
                    resp = Open(new Uri(ftpURI + downloadFileName), WebRequestMethods.Ftp.DownloadFile);
                using (Stream ftpStream = resp.GetResponseStream())
                {
                    long cl = resp.ContentLength;
                    int bufferSize = 2048;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }
                }
            }
        }


        public string ReadFile(string fileName)
        {
            FtpWebResponse resp = null;
            while (resp == null)
                resp = Open(new Uri(ftpURI + fileName), WebRequestMethods.Ftp.DownloadFile);
            using (Stream ftpStream = resp.GetResponseStream())
            {
                long cl = resp.ContentLength;
                int bufferSize = 32;
                int readCount;
                byte[] buffer = new byte[bufferSize];
                readCount = ftpStream.Read(buffer, 0, bufferSize);
                while (readCount > 0)
                {
                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                }
                string result = System.Text.Encoding.Default.GetString(buffer).Trim();
                return Regex.Replace(result, @"[^\d\.]*", "");
            }

        }


    }

    public class FileStruct
    {
        /// <summary>  
        /// 是否为目录  
        /// </summary>  
        public bool IsDirectory { get; set; }
        /// <summary>  
        /// 创建时间  
        /// </summary>  
        public DateTime CreateTime { get; set; }
        /// <summary>  
        /// 文件或目录名称  
        /// </summary>  
        public string Name { get; set; }
        /// <summary>  
        /// 路径  
        /// </summary>  
        public string Path { get; set; }
    }

    public class HRG_DownloadHelper : IDisposable
    {
        IDownloader _downloader;
        public HRG_DownloadHelper(string ftpServerIP, string ftpRemotePath, string ftpUserID, string ftpPassword, DOWNLOADER_TYPE type = DOWNLOADER_TYPE.NET_FTP)
        {
            switch (type)
            {
                case DOWNLOADER_TYPE.NET_FTP: _downloader = new FtpDownloader(ftpServerIP, ftpRemotePath, ftpUserID, ftpPassword);
                    break;

            }
        }

        /// <summary>
        /// 获取目录列表
        /// </summary>
        /// <returns></returns>
        public List<string> GetDirList()
        {
            List<string> result = new List<string>();
            foreach (FileStruct fs in _downloader.ListDirectories())
            {
                result.Add(fs.Name);
            }
            return result;
        }

        /// <summary>
        /// 获取最新版本的版本编号（供判断是否需要升级）
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string GetLastestVersion()
        {
            foreach (FileStruct fs in _downloader.ListFiles())
            {
                return _downloader.ReadFile("updateinfo.txt");
            }
            return string.Empty;
        }


        //下载功能
        public void DownloadFile(string saveFilePath)
        {

            //下载根目录及其文件夹
            RescureFolder(saveFilePath);
            //从根目录往下逐个遍历文件夹列表下载文件

        }

        private void RescureFolder(string saveFolderName)
        {
            //1. 如果saveFolder不存在则创建savefolder
            if (!System.IO.Directory.Exists(saveFolderName))
            {
                System.IO.Directory.CreateDirectory(saveFolderName);
            }
            //2.将downFolder文件夹内的文件逐个下载到当前路径
            foreach (FileStruct fileName in _downloader.ListFiles())
            {
                _downloader.Download(saveFolderName, fileName.Name);

            }

            //3. 逐个遍历当前文件夹下的文件夹
            foreach (FileStruct dirName in _downloader.ListDirectories())
            {
                _downloader.GotoDirectory(dirName.Name, false);

                RescureFolder(new StringBuilder(saveFolderName + "//" + dirName.Name).ToString());

                _downloader.GoBack();

            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            ((IDisposable)_downloader).Dispose();
        }

        ~HRG_DownloadHelper() { this.Dispose(false); }
    }
}
