using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace HRG_BaseLibrary_2012
{
/// <summary>
/// XML文件处理类
    /// </summary>

    #region xml文件基本类
    public class HRG_XmlHelper
    {
        private string _fileName = null;
        private XmlDocument doc = new XmlDocument();
        public HRG_XmlHelper(string fileName)
        {
            _fileName = fileName;
            doc.Load(_fileName);
        }

        public HRG_XmlHelper()
        {
            //没有输入文件则直接在运行目录内寻找.xml后缀名的文件，找到的第一个xml会被作为当前信息集合
            DirectoryInfo direct = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);
            foreach (FileInfo file in direct.GetFiles())
            {
                if (file.Extension == ".xml")
                    _fileName = file.Name;
                doc.Load(_fileName);
                GlobalVariables.LOG_MANAGER.Info(String.Format("初始化xml文件处理功能，配置xml文件名为{0}",_fileName));
                return;
            }
            //如果没有找到任何xml文件则等待后续设置，不设置filename是不能执行基本读写功能的
            //相关信息会写入日志文件
            GlobalVariables.LOG_MANAGER.Info("初始化xml文件处理，未能正确设置xml文件的文件名");
        }

        //设置文件路径
        public void setFileName(string fileName)
        {
            _fileName = fileName;
            doc.Load(_fileName);
            GlobalVariables.LOG_MANAGER.Info(String.Format("设置xml文件名[{0}]", _fileName));
        }

        //查找根节点
        private XmlElement root()
        {
            return doc.DocumentElement;
        }

        //遍历一级节点
        private List<XmlElement> getElementsLevelOne()
        {
            List<XmlElement> result = new List<XmlElement>();
            foreach (XmlNode elem in root().ChildNodes)
            {
                if (elem is XmlElement)
                    result.Add((XmlElement)elem);
            }
            return result;
        }

        //按照nodename,在源节点树中查找节点
        public XmlElement getElementByName(string nodeName, XmlElement element = null)
        {
            //如果没有设定节点，则从根节点开始搜索
            if (element == null)
            {
                element = root();
            }

            if (element.Name == nodeName)
                return element;

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement temp = getElementByName(nodeName, (XmlElement)node);
                    if (temp != null)
                        return temp;
                }
            }
            return null;
        }

        //读取node节点innerText值
        public string getElementValueByName(string nodeName, XmlElement element = null)
        {
            XmlElement temp = getElementByName(nodeName, element);
            if (temp == null)
                return "";
            else
                return temp.InnerText;
        }

        //读取node节点attribute的值
        public string getAttributeByName(string nodeName, string attributeName, XmlElement element = null)
        {
            XmlElement temp = getElementByName(nodeName, element);
            if (temp == null)
                return "";
            else
                return temp.GetAttribute(attributeName);
        }

        //更新node节点信息
        public void updateElementByName(string nodeName, string newValue, XmlElement element = null)
        {
            XmlElement temp = getElementByName(nodeName, element);
            temp.Value = newValue;
            doc.Save(_fileName);
        }

    }
    #endregion
}
