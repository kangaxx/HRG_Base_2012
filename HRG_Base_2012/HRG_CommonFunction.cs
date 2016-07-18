using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Reflection;
using System.Dynamic;


namespace HRG_BaseLibrary_2012
{
    #region 全局通用变量类
    public sealed class GlobalVariables
    {
        public static string STRING_SQLTYPE_FLAG = "sqltype"; //数据库类型标签名称
        public static string STRING_SQLTYPE_NAME_MYSQL = "mysql"; //数据库类型标签值， mysql
        public static string STRING_SQLTYPE_NAME_OLEDB = "oledb"; //数据库类型标签值， OLEDB
        public static string STRING_SQL_CONNECTION_TAG_USER = "user id"; //数据库链接信息标签， 用户名
        public static string STRING_SQL_CONNECTION_TAG_SERVER = "server"; //数据库链接信息标签，服务器
        public static string STRING_SQL_CONNECTION_TAG_PASSWORD = "password"; //数据库链接信息标签，密码
        public static string STRING_SQL_CONNECTION_TAG_DATABASE = "database"; //数据库链接信息标签, 数据库
        public static string STRING_SQL_CONNECTION_TAG_DATASOURCE = "Data Source"; //数据库链接信息标签, oledb数据源
        public static string STRING_SQL_CONNECTION_TAG_PROVIDER = "Provider"; //数据库链接信息标签, oledb类型
        public static int INT_SQL_PARAM_DIRECTION_INPUT = 1;  //输入参数类型
        public static int INT_SQL_PARAM_DIRECTION_OUTPUT = 2;  //输出参数类型
        public static int INT_SQL_PARAM_DIRECTION_RETURN = 3;  //返回值参数类型
    }
    #endregion

    #region 基本功能类，包括一些文件，字符串等等的c#基本元素处理功能，原则上一律提供静态函数，复杂功能不要添加在本类
    public static class CommonFunction
    {
        #region Json转换
        //对象转json
        public static string ToJson(object obj)
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            setting.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented, setting);
            return json;
        }

        //json转对象
        public static T FromJson<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();

            StringReader sr = new StringReader(json);
            object obj = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
            T t = obj as T;
            return t;
        }

        //json转对象集合
        public static List<T> ListFromJson<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }


        /// 反序列化JSON转匿名对象.
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        {
            T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
            return t;
        }


        /// 反序列化JSON转list对象序列
        public static List<T> DeserializeJsonToList<T>(string json) where T : class
        {
            JsonSerializer serializer = new JsonSerializer();
            StringReader sr = new StringReader(json);
            object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
            List<T> list = o as List<T>;
            return list;
        }

        #endregion

        #region 字符串解析

        //从一串配置字符串中获取指定的配置值 ,tag是希望获取的配置的名称， splitChar各个配置项之间的分割符号，默认的配置名称与配置值之间的分割是等号 =
        public static string GetSettingValueByName(string source, string tag, char[] splitChar, char flag = '=')
        {
            string[] configs = source.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
            foreach (string str in configs)
            {
                string[] aConfigValue = str.Split(new char[]{flag}, StringSplitOptions.RemoveEmptyEntries);
                if (aConfigValue.Length == 2) //这里的长度恒为2， 没有特殊理由不会变。
                {
                    if (aConfigValue[0].Trim() == tag.Trim())
                        return aConfigValue[1].Trim();
                }

            }
            return "";
        }

        #endregion
    }
    #endregion

    #region 转换数据表到动态类的功能
    //数据集类型
    public class FieldUnit
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public class DynDBHelper
    {
        private int columnNum;
        private List<List<FieldUnit>> rows; //数据集记录


        public DynDBHelper()
        {
            columnNum = 0; //初始化时列数初始值
            rows = new List<List<FieldUnit>>();
        }

        //添加数据
        public void addRow(List<FieldUnit> row)
        {
            if (row.Count() > 0)
            {
                //如果已经添加过有效的条目row，则判断新的条目列数是否一致，不一致就要报错了
                if (columnNum > 0 && columnNum != row.Count())
                {
                    throw new Exception(String.Format("ERROR , in DynDBHelper.addRow, columnNum not equal old {0} , new {1}", columnNum, row.Count()));
                }
                rows.Add(row);
                if (columnNum <= 0)
                    columnNum = row.Count(); //设置正确列数
            }
        }

        //设置表结构 gxx, 稍后完成 ， 20160523
        //public void setTableStruct

        //将数据转换为动态类列表
        public List<object> getDataList()
        {
            if (columnNum <= 0 || rows.Count() <= 0)
                throw new Exception("ERROR, datatable struct is NULL!");

            List<object> result = new List<object>();
            foreach (List<FieldUnit> row in rows)
            {
                dynamic temp = new ExpandoObject();
                foreach (FieldUnit field in row)
                {
                    (temp as ICollection<KeyValuePair<string, object>>).Add(new KeyValuePair<string, object>(field.FieldName,field.FieldValue)); //动态类的动态属性，性能不是最优哦，但是这里也没办法了。
                }
                result.Add((object)temp);
            }
            return result;
        }

    }
    #endregion

    #region Log日志类

    #endregion



}
