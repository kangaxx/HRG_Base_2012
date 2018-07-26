using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;



namespace HRG_BaseLibrary_2012
{
    //串口功能类
    public class HRG_SerialPortHelper
    {
        private SerialPort _port;

        /// <summary>  
        /// 端口名称数组  
        /// </summary>  
        public string[] PortNameArr { get; set; }

        /// <summary>  
        /// 串口通信开启状态  
        /// </summary>  
        public bool PortState { get; set; } = false;

        /// <summary>  
        /// 编码类型  
        /// </summary>  
        public Encoding EncodingType { get; set; } = Encoding.Unicode;

        /// <summary>  
        /// 串口接收数据委托  
        /// </summary>  
        public delegate void ComReceiveDataHandler(byte[] data);

        public ComReceiveDataHandler OnComReceiveDataHandler = null;

        public HRG_SerialPortHelper()
        {
            _port = new SerialPort();
            _port.DataReceived += new SerialDataReceivedEventHandler(_port_DataReceived);
        }

        ~HRG_SerialPortHelper()
        {
            _port.Close();
        }

        private void _port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] buffer = new byte[_port.BytesToRead];
            _port.Read(buffer, 0, buffer.Length);
            //string str = EncodingType.GetString(buffer);
            OnComReceiveDataHandler?.Invoke(buffer);
        }

        /// <summary>  
        /// 发送数据  
        /// </summary>  
        /// <param name="sendData"></param>  
        public void SendData(string sendData)
        {
            try
            {
                _port.Encoding = EncodingType;
                _port.Write(sendData);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        /// <summary>  
        /// 发送数据  
        /// </summary>  
        /// <param name="sendData"></param>  
        public void SendData(byte[] sendData)
        {
            try
            {
                _port.Encoding = EncodingType;
                _port.Write(sendData, 0, sendData.Length);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //获取接口列表
        public string[] GetPortList()
        {
            return SerialPort.GetPortNames();
        }


        /// <summary>  
        /// 打开端口  
        /// </summary>  
        /// <param name="portName">端口名称</param>  
        /// <param name="boudRate">波特率</param>  
        /// <param name="dataBit">数据位</param>  
        /// <param name="stopBit">停止位</param>  
        /// <param name="timeout">超时时间</param>  
        public void OpenPort(string portName, int boudRate = 115200, int dataBit = 8, int stopBit = 1, int timeout = 5000)
        {
            try
            {
                _port.PortName = portName;
                _port.BaudRate = boudRate;
                _port.DataBits = dataBit;
                _port.StopBits = (StopBits)stopBit;
                _port.ReadTimeout = timeout;
                _port.Open();
                PortState = true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>  
        /// 关闭端口  
        /// </summary>  
        public void ClosePort()
        {
            try
            {
                _port.Close();
                PortState = false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

    }
}
