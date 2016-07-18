/***************************************************************************************************************
 * 哈工大机器人集团 基本函数底层库
 * 2016. 5. 20
 * 顾欣欣
 * 说明：
 *          HRG内部vs2012开发公共类库，将一些基本功能及引用编译到一起，方便软件编写程序安装,
 *          减少重复性工作，提升整体效率。
 * 
 * 类列表 : HRG_SecurityHelper 字符串加解密功能
 *          
 * 
 * 
 * ************************************************************************************************************/
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace HRG_BaseLibrary_2012
{
    #region 字符串加解密功能类
    public static class SecurityHelper
    {
        #region Des 加/解密

        private static string IV = "455GXX6B-1719-45dc-971B-801027FC04DD";
        private static string KEY = "0E8B1C9A-B42C-42a7-A96B-6E67AF70B02F";
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="encryptedValue">要解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptDES(string encryptedValue)
        {
            string s = KEY.Substring(0, 8);
            string str2 = IV.Substring(0, 8);
            SymmetricAlgorithm algorithm = new DESCryptoServiceProvider();
            algorithm.Key = Encoding.ASCII.GetBytes(s);
            algorithm.IV = Encoding.ASCII.GetBytes(str2);
            ICryptoTransform transform = algorithm.CreateDecryptor();
            byte[] buffer = Convert.FromBase64String(encryptedValue);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            stream2.Close();
            return Encoding.UTF8.GetString(stream.ToArray());
        }
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="originalValue">需要加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptDES(string originalValue)
        {
            string s = KEY.Substring(0, 8);
            string str2 = IV.Substring(0, 8);
            SymmetricAlgorithm algorithm = new DESCryptoServiceProvider();
            algorithm.Key = Encoding.ASCII.GetBytes(s);
            algorithm.IV = Encoding.ASCII.GetBytes(str2);
            ICryptoTransform transform = algorithm.CreateEncryptor();
            byte[] bytes = Encoding.UTF8.GetBytes(originalValue);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            stream2.Close();
            return Convert.ToBase64String(stream.ToArray());
        }
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <returns></returns>
        public static string DecodeDes(string str)
        {
            return DecodeDes(str, "wwwl.80.hk");
        }
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string EncodeDes(string str)
        {
            return EncodeDes(str, "wwwl.80.hk");
        }

        /// <summary>
        /// 加密 GB2312 
        /// </summary>
        /// <param name="str">加密串</param>
        /// <param name="key">Key</param>
        /// <returns></returns>
        public static string EncodeDes(string str, string key)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, 8));
            byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(str);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            stream.Close();
            return builder.ToString();
        }

        /// <summary>
        /// 解密 GB2312 
        /// </summary>
        /// <param name="str">Desc string</param>
        /// <param name="key">Key ,必须为8位 </param>
        /// <returns></returns>
        public static string DecodeDes(string str, string key)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            provider.Key = Encoding.ASCII.GetBytes(key.Substring(0, 8));
            provider.IV = Encoding.ASCII.GetBytes(key.Substring(0, 8));
            byte[] buffer = new byte[str.Length / 2];
            for (int i = 0; i < (str.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(str.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            stream.Close();
            return Encoding.GetEncoding("GB2312").GetString(stream.ToArray());
        }


        #endregion

        #region 移位加/解密
        /// <summary>
        /// 字符串加密  进行位移操作
        /// </summary>
        /// <param name="Input">待加密数据</param>
        /// <returns>加密后的数据</returns>
        public static string EncryptShift(string Input)
        {
            string _temp = "";
            int _inttemp;
            char[] _chartemp = Input.ToCharArray();
            for (int i = 0; i < _chartemp.Length; i++)
            {
                _inttemp = _chartemp[i] + 1;
                _chartemp[i] = (char)_inttemp;
                _temp += _chartemp[i];
            }
            return _temp;
        }
        /// <summary>
        /// 字符串解密 位移操作后的
        /// </summary>
        /// <param name="Input">待解密数据</param>
        /// <returns>解密成功后的数据</returns>
        public static string DecryptShift(string Input)
        {
            string _temp = "";
            int _inttemp;
            char[] _chartemp = Input.ToCharArray();
            for (int i = 0; i < _chartemp.Length; i++)
            {
                _inttemp = _chartemp[i] - 1;
                _chartemp[i] = (char)_inttemp;
                _temp += _chartemp[i];
            }
            return _temp;
        }
        #endregion

        #region AES加密/解密

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptAES(string encryptString)
        {
            return EncryptAES(encryptString, "wwwl.80.hk");
        }
        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptAES(string decryptString)
        {
            return DecryptAES(decryptString, "wwwl.80.hk");
        }
        //默认密钥向量
        private static byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <returns>加密后的字符串</returns>
        public static string EncryptAES(string encryptString, string encryptKey)
        {
            encryptKey = GetSubString(encryptKey, 0, 32, "");
            encryptKey = encryptKey.PadRight(32, ' ');

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

            byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
            byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Convert.ToBase64String(encryptedData);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <returns>解密后的字符串</returns>
        public static string DecryptAES(string decryptString, string decryptKey)
        {
            decryptKey = GetSubString(decryptKey, 0, 32, "");
            decryptKey = decryptKey.PadRight(32, ' ');

            RijndaelManaged rijndaelProvider = new RijndaelManaged();
            rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
            rijndaelProvider.IV = Keys;
            ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

            byte[] inputData = Convert.FromBase64String(decryptString);
            byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

            return Encoding.UTF8.GetString(decryptedData);
        }
        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        private static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;

            Byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
            foreach (char c in Encoding.UTF8.GetChars(bComments))
            {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                {
                    //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                    //当截取的起始位置超出字段串长度时
                    if (p_StartIndex >= p_SrcString.Length)
                        return "";
                    else
                        return p_SrcString.Substring(p_StartIndex,
                                                       ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }

            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }

                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {
                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                                nFlag = 1;
                        }
                        else
                            nFlag = 0;

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        nRealLength = p_Length + 1;

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);
                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }
        #endregion

        #region 数据库链接统一功能
        public static string EncryptDBConn(string encryptString)
        {
            return EncryptAES(encryptString);
        }


        public static string DecryptDBConn(string decryptString)
        {
            return DecryptAES(decryptString);
        }
        #endregion
    }
    #endregion
}
