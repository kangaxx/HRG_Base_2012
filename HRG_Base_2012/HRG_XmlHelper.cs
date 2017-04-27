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

        public HRG_XmlHelper(byte[] xml)
        {
            _fileName = "xml_from_stream.log";
            doc.LoadXml(System.Text.Encoding.Default.GetString(xml));
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
        public void SetFileName(string fileName)
        {
            _fileName = fileName;
            doc.Load(_fileName);
            GlobalVariables.LOG_MANAGER.Info(String.Format("设置xml文件名[{0}]", _fileName));
        }

        //查找根节点
        private XmlElement Root()
        {
            return doc.DocumentElement;
        }

        //遍历一级节点
        private List<XmlElement> GetElementsLevelOne()
        {
            List<XmlElement> result = new List<XmlElement>();
            foreach (XmlNode elem in Root().ChildNodes)
            {
                if (elem is XmlElement)
                    result.Add((XmlElement)elem);
            }
            return result;
        }

        //按照nodename,在源节点树中查找节点
        public XmlElement GetElementByName(string nodeName, XmlElement element = null)
        {
            //如果没有设定节点，则从根节点开始搜索
            if (element == null)
            {
                element = Root();
            }

            if (element.Name == nodeName)
                return element;

            foreach (XmlNode node in element.ChildNodes)
            {
                if (node is XmlElement)
                {
                    XmlElement temp = GetElementByName(nodeName, (XmlElement)node);
                    if (temp != null)
                        return temp;
                }
            }
            return null;
        }

        //按照nodename,在源节点树中查找节点
        public List<XmlElement> GetElementsByName(string nodeName, XmlElement parent = null)
        {
            List<XmlElement> result = new List<XmlElement>();
            //如果没有设定节点，则从根节点开始搜索
            if (parent == null)
            {
                parent = Root();
            }

            if (parent.Name == nodeName)
                result.Add(parent);

            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node is XmlElement)
                {
                    List<XmlElement> tmp = GetElementsByName(nodeName, (XmlElement)node);
                    if (tmp != null)
                       result.AddRange(GetElementsByName(nodeName, (XmlElement)node));
                }
            }
            return result;
        }


        //读取node节点innerText值
        public string GetElementValueByName(string nodeName, XmlElement element = null)
        {
            XmlElement temp = GetElementByName(nodeName, element);
            if (temp == null)
                return "";
            else
                return temp.InnerText;
        }

        //读取node节点attribute的值
        public string GetAttributeByName(string nodeName, string attributeName, XmlElement element = null)
        {
            XmlElement temp = GetElementByName(nodeName, element);
            if (temp == null)
                return "";
            else
                return temp.GetAttribute(attributeName);
        }

        //更新node节点信息
        public void UpdateElementByName(string nodeName, string newValue, XmlElement element)
        {
            XmlElement temp = GetElementByName(nodeName, element);
            temp.Value = newValue;
            doc.Save(_fileName);
        }

        //更新节点属性值
        public void UpdateElementAttribute(string name, string newValue, XmlElement element)
        {
            element.SetAttribute(name, newValue);
            doc.Save(_fileName);
        }

        //更新所有同名节点的同一个属性
        public void UpdateElementsAttribute(string elemName, string attrName, string newValue, XmlElement parent = null)
        {
            if (parent == null)
                parent = Root();
            foreach (XmlElement elem in GetElementsByName(elemName, parent))
            {
                elem.SetAttribute(attrName, newValue);
            }
            doc.Save(_fileName);

        }

        //添加子节点
        public void AppendElement(string name, string innerText, XmlElement parent = null)
        {
            if (parent == null)
                parent = Root();
            XmlNode node = doc.CreateNode(XmlNodeType.Element, name, innerText);
            parent.AppendChild(node);
            doc.Save(_fileName);
        }

        //创建子元素,注意此元素未被保存于文件中
        public XmlElement CreateNewElementNoSave(string name, XmlElement parent = null)
        {
            if (parent == null)
                parent = Root();
            XmlElement elem = doc.CreateElement(name);
            parent.AppendChild(elem);
            return elem;
        }

        //配合 CreateNewElement函数使用，添加属性到element
        public void AddAttribute(string name, string value, XmlElement elem)
        {
            elem.SetAttribute(name, value);
            doc.Save(_fileName);
        }



    }


    #endregion
}
