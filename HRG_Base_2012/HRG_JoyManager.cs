using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;

namespace HRG_BaseLibrary_2012
{
    public class API
    {
        #region 消息定义
        public const int MM_JOY1MOVE = 0x3A0;
        public const int MM_JOY2MOVE = 0x3A1;
        public const int MM_JOY1BUTTONDOWN = 0x3B5;
        public const int MM_JOY2BUTTONDOWN = 0x3B6;
        public const int MM_JOY1BUTTONUP = 0x3B7;
        public const int MM_JOY2BUTTONUP = 0x3B8;
        #endregion

        #region 按钮定义
        public const int JOY_BUTTON1 = 0x0001;

        public const int JOY_BUTTON2 = 0x0002;

        public const int JOY_BUTTON3 = 0x0004;

        public const int JOY_BUTTON4 = 0x0008;

        public const int JOY_BUTTON5 = 0x0010;

        public const int JOY_BUTTON6 = 0x0020;

        public const int JOY_BUTTON7 = 0x0040;

        public const int JOY_BUTTON8 = 0x0080;

        public const int JOY_BUTTON9 = 0x0100;

        public const int JOY_BUTTON10 = 0x0200;

        public const int JOY_BUTTON11 = 0x0400;

        public const int JOY_BUTTON12 = 0x0800;

        //Button up/down
        public const int JOY_BUTTON1CHG = 0x0100;

        public const int JOY_BUTTON2CHG = 0x0200;

        public const int JOY_BUTTON3CHG = 0x0400;

        public const int JOY_BUTTON4CHG = 0x0800;
        #endregion

        #region 手柄Id定义
        /// <summary>
        /// 主游戏手柄Id
        /// </summary>
        public const int JOYSTICKID1 = 0;
        /// <summary>
        /// 副游戏手柄Id
        /// </summary>
        public const int JOYSTICKID2 = 1;
        #endregion

        #region 错误号定义
        /// <summary>
        /// 没有错误
        /// </summary>
        public const int JOYERR_NOERROR = 0;
        /// <summary>
        /// 参数错误
        /// </summary>
        public const int JOYERR_PARMS = 165;
        /// <summary>
        /// 无法正常工作
        /// </summary>
        public const int JOYERR_NOCANDO = 166;
        /// <summary>
        /// 操纵杆未连接 
        /// </summary>
        public const int JOYERR_UNPLUGGED = 167;
        #endregion

        /// <summary>
        /// 游戏手柄的参数信息JoystickEventArgs
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYCAPS
        {
            public ushort wMid;
            public ushort wPid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szPname;
            public int wXmin;
            public int wXmax;
            public int wYmin;
            public int wYmax;
            public int wZmin;
            public int wZmax;
            public int wNumButtons;
            public int wPeriodMin;
            public int wPeriodMax;
            public int wRmin;
            public int wRmax;
            public int wUmin;
            public int wUmax;
            public int wVmin;
            public int wVmax;
            public int wCaps;
            public int wMaxAxes;
            public int wNumAxes;
            public int wMaxButtons;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string szRegKey;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szOEMVxD;
        }

        /// <summary>
        /// 游戏手柄的位置与按钮状态
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct JOYINFO
        {
            public int wXpos;
            public int wYpos;
            public int wZpos;
            public int wButtons;
        }

        /// <summary>
        /// 检查系统是否配置了游戏端口和驱动程序。如果返回值为零，表明不支持操纵杆功能。如果返回值不为零，则说明系统支持游戏操纵杆功能。
        /// </summary>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetNumDevs();

        /// <summary>
        /// 获取某个游戏手柄的参数信息
        /// </summary>
        /// <param name="uJoyID">指定游戏杆(0-15)，它可以是JOYSTICKID1或JOYSTICKID2</param>
        /// <param name="pjc"></param>
        /// <param name="cbjc">JOYCAPS结构的大小</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetDevCaps(int uJoyID, ref JOYCAPS pjc, int cbjc);

        /// <summary>
        /// 向系统申请捕获某个游戏杆并定时将该设备的状态值通过消息发送到某个窗口 
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="uJoyID">指定游戏杆(0-15)，它可以是JOYSTICKID1或JOYSTICKID2</param>
        /// <param name="uPeriod">每隔给定的轮询间隔就给应用程序发送有关游戏杆的信息。这个参数是以毫妙为单位的轮询频率。</param>
        /// <param name="fChanged">是否允许程序当操纵杆移动一定的距离后才接受消息</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joySetCapture(IntPtr hWnd, int uJoyID, int uPeriod, bool fChanged);

        /// <summary>
        /// 释放操纵杆的捕获
        /// </summary>
        /// <param name="uJoyID">指定游戏杆(0-15)，它可以是JOYSTICKID1或JOYSTICKID2</param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyReleaseCapture(int uJoyID);

        /// <summary>
        /// 获取操纵杆位置和按钮状态
        /// </summary>
        /// <param name="uJoyID"></param>
        /// <param name="pji"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        public static extern int joyGetPos(int uJoyID, ref JOYINFO pji);

        /// <summary>
        /// 获取操纵杆位置和按钮状态
        /// </summary>
        /// <param name="uJoyID"></param>
        /// <param name="pji"></param>
        /// <returns></returns>
        //[DllImport("winmm.dll")]
        //public static extern int joyGetPosEx(int uJoyID, ref JOYINFOEX pji);
    }

    /// <summary>
    /// 游戏手柄的事件参数
    /// </summary>
    public class JoystickEventArgs : EventArgs
    {
        public JoystickEventArgs()
        {

        }
        /// <summary>
        /// 游戏手柄的事件参数
        /// </summary>
        /// <param name="joystickId">手柄Id</param>
        /// <param name="buttons">按钮</param>
        public JoystickEventArgs(int joystickId, JoystickButtons buttons)
        {
            this.JoystickId = joystickId;
            this.Buttons = buttons;
        }

        public JoystickEventArgs(int joystickId, JoystickButtons buttons, Point move)
        {
            this.JoystickId = joystickId;
            this.Buttons = buttons;
            this.Move = move;
        }
        /// <summary>
        /// 手柄Id
        /// </summary>
        public int JoystickId { get; set; }
        /// <summary>
        /// 按钮
        /// </summary>
        public JoystickButtons Buttons { get; set; }

        public Point Move;// { get; set; }
    }

    /// <summary>
    /// 游戏手柄的按钮定义
    /// </summary>
    [Flags]
    public enum JoystickButtons
    {
        //没有任何按钮
        None = 0x0,
        UP = 0x01,
        Down = 0x02,
        Left = 0x04,
        Right = 0x08,
        B1 = 0x10,
        B2 = 0x20,
        B3 = 0x40,
        B4 = 0x80,
        B5 = 0x100,
        B6 = 0x200,
        B7 = 0x400,
        B8 = 0x800,
        B9 = 0x1000,
        B10 = 0x2000,
        B11 = 0x4000,
        B12 = 0x8000,
        POV_UP = 0x10000,
        POV_Down = 0x20000,
        POV_Left = 0x40000,
        POV_Right = 0x80000,
        LeftUp = 0x100000,
        RightUp = 0x200000,
        LeftDown = 0x400000,
        RightDown = 0x800000
    }

    public class Joystick_P : IMessageFilter, IDisposable
    {
        #region 事件定义
        /// <summary>
        /// 按钮被单击
        /// </summary>
        public event EventHandler<JoystickEventArgs> Click;
        /// <summary>
        /// 按钮被弹起
        /// </summary>
        public event EventHandler<JoystickEventArgs> ButtonUp;
        /// <summary>
        /// 按钮已被按下
        /// </summary>
        public event EventHandler<JoystickEventArgs> ButtonDown;
        /// <summary>
        /// 触发单击事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnClick(JoystickEventArgs e)
        {
            EventHandler<JoystickEventArgs> h = this.Click;
            if (h != null) h(this, e);
        }
        /// <summary>
        /// 触发按钮弹起事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnButtonUp(JoystickEventArgs e)
        {
            EventHandler<JoystickEventArgs> h = this.ButtonUp;
            if (h != null) h(this, e);
        }
        /// <summary>
        /// 触发按钮按下事件
        /// </summary>
        /// <param name="e"></param>
        protected void OnButtonDown(JoystickEventArgs e)
        {
            EventHandler<JoystickEventArgs> h = this.ButtonDown;
            if (h != null) h(this, e);
        }

        /// <summary>
        /// 是否已注册消息
        /// </summary>
        private bool IsRegister = false;
        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="hWnd">需要捕获手柄消息的窗口</param>
        /// <param name="joystickId">要捕获的手柄Id</param>
        public bool Register(IntPtr hWnd, int joystickId)
        {
            bool flag = false;
            int result = 0;
            API.JOYCAPS caps = new API.JOYCAPS();
            if (API.joyGetNumDevs() != 0)
            {
                //拥有手柄.则判断手柄状态
                result = API.joyGetDevCaps(joystickId, ref caps, Marshal.SizeOf(typeof(API.JOYCAPS)));
                if (result == API.JOYERR_NOERROR)
                {
                    //手柄处于正常状态
                    flag = true;
                }
            }

            if (flag)
            {
                //注册消息
                if (!this.IsRegister)
                {
                    Application.AddMessageFilter(this);
                }
                this.IsRegister = true;

                result = API.joySetCapture(hWnd, joystickId, caps.wPeriodMin * 2, false);
                if (result != API.JOYERR_NOERROR)
                {
                    flag = false;
                }
            }
            return flag;
        }

        /// <summary>
        /// 取消注册
        /// </summary>
        /// <param name="joystickId"></param>
        public void UnRegister(int joystickId)
        {
            if (this.IsRegister)
            {
                API.joyReleaseCapture(joystickId);
            }
        }
        #endregion

        #region 消息处理
        #region IMessageFilter 成员
        /// <summary>
        /// 处理系统消息.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            bool flag = false;
            if (m.HWnd != IntPtr.Zero && (m.WParam != IntPtr.Zero || m.LParam != IntPtr.Zero))
            {
                Action<JoystickEventArgs> action = null;
                JoystickEventArgs e = new JoystickEventArgs();
                e.Buttons = JoystickButtons.None;
                //JoystickButtons buttons = JoystickButtons.None;
                int joystickId = -1;
                switch (m.Msg)
                {
                    case API.MM_JOY1MOVE:
                    case API.MM_JOY2MOVE:
                        //单击事件
                        e = GetButtonsStateFromMessageParam(m.WParam.ToInt64(), m.LParam.ToInt64());
                        action = this.OnClick;
                        joystickId = m.Msg == API.MM_JOY1MOVE ? API.JOYSTICKID1 : API.JOYSTICKID2;
                        break;
                    case API.MM_JOY1BUTTONDOWN:
                    case API.MM_JOY2BUTTONDOWN:
                        //按钮被按下
                        e = GetButtonsPressedStateFromMessageParam(m.WParam.ToInt32(), m.LParam.ToInt32());
                        action = this.OnButtonDown;
                        joystickId = m.Msg == API.MM_JOY1BUTTONDOWN ? API.JOYSTICKID1 : API.JOYSTICKID2;
                        break;
                    case API.MM_JOY1BUTTONUP:
                    case API.MM_JOY2BUTTONUP:
                        //按钮被弹起
                        e = GetButtonsPressedStateFromMessageParam(m.WParam.ToInt32(), m.LParam.ToInt32());
                        action = this.OnButtonUp;
                        joystickId = m.Msg == API.MM_JOY1BUTTONUP ? API.JOYSTICKID1 : API.JOYSTICKID2;
                        break;
                }
                //if (action != null && joystickId != -1 && e.Buttons != JoystickButtons.None)
                if (action != null && joystickId != -1)
                {
                    //阻止消息继续传递
                    flag = true;

                    e.JoystickId = joystickId;
                    //触发事件
                    action(e);//new JoystickEventArgs(joystickId, buttons));
                }
            }
            return flag;
        }
        #endregion

        Point GetXYMoveFromMessageParam(int wParam, int lParam)
        {
            Point pt = new Point();
            GetXYMoveStateFromLParam(lParam, ref pt);
            return pt;
        }
        /// <summary>
        /// 根据消息的参数获取按钮的状态值
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private JoystickEventArgs GetButtonsStateFromMessageParam(Int64 wParam, Int64 lParam)
        {
            JoystickEventArgs e = new JoystickEventArgs();

            e.Buttons = JoystickButtons.None;
            //JoystickButtons buttons = JoystickButtons.None;
            if ((wParam & API.JOY_BUTTON1) == API.JOY_BUTTON1)
            {
                e.Buttons |= JoystickButtons.B1;
            }
            if ((wParam & API.JOY_BUTTON2) == API.JOY_BUTTON2)
            {
                e.Buttons |= JoystickButtons.B2;
            }
            if ((wParam & API.JOY_BUTTON3) == API.JOY_BUTTON3)
            {
                e.Buttons |= JoystickButtons.B3;
            }
            if ((wParam & API.JOY_BUTTON4) == API.JOY_BUTTON4)
            {
                e.Buttons |= JoystickButtons.B4;
            }
            if ((wParam & API.JOY_BUTTON5) == API.JOY_BUTTON5)
            {
                e.Buttons |= JoystickButtons.B5;
            }
            if ((wParam & API.JOY_BUTTON6) == API.JOY_BUTTON6)
            {
                e.Buttons |= JoystickButtons.B6;
            }
            if ((wParam & API.JOY_BUTTON7) == API.JOY_BUTTON7)
            {
                e.Buttons |= JoystickButtons.B7;
            }
            if ((wParam & API.JOY_BUTTON8) == API.JOY_BUTTON8)
            {
                e.Buttons |= JoystickButtons.B8;
            }
            if ((wParam & API.JOY_BUTTON9) == API.JOY_BUTTON9)
            {
                e.Buttons |= JoystickButtons.B9;
            }
            if ((wParam & API.JOY_BUTTON10) == API.JOY_BUTTON10)
            {
                e.Buttons |= JoystickButtons.B10;
            }
            if ((wParam & API.JOY_BUTTON11) == API.JOY_BUTTON11)
            {
                e.Buttons |= JoystickButtons.B11;
            }
            if ((wParam & API.JOY_BUTTON12) == API.JOY_BUTTON12)
            {
                e.Buttons |= JoystickButtons.B12;
            }

            GetXYButtonsStateFromLParam(lParam, ref e);

            return e;
        }
        /// <summary>
        /// 根据消息的参数获取按钮的按压状态值
        /// </summary>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private JoystickEventArgs GetButtonsPressedStateFromMessageParam(int wParam, int lParam)
        {
            JoystickEventArgs e = new JoystickEventArgs();

            e.Buttons = JoystickButtons.None;
            //JoystickButtons buttons = JoystickButtons.None;
            if ((wParam & API.JOY_BUTTON1CHG) == API.JOY_BUTTON1CHG)
            {
                e.Buttons |= JoystickButtons.B1;
            }
            if ((wParam & API.JOY_BUTTON2CHG) == API.JOY_BUTTON2CHG)
            {
                e.Buttons |= JoystickButtons.B2;
            }
            if ((wParam & API.JOY_BUTTON3CHG) == API.JOY_BUTTON3CHG)
            {
                e.Buttons |= JoystickButtons.B3;
            }
            if ((wParam & API.JOY_BUTTON4CHG) == API.JOY_BUTTON4CHG)
            {
                e.Buttons |= JoystickButtons.B4;
            }

            GetXYButtonsStateFromLParam(lParam, ref e);

            return e;
        }
        /// <summary>
        /// 获取X,Y轴的状态
        /// </summary>
        /// <param name="lParam"></param>
        /// <param name="buttons"></param>
        private void GetXYButtonsStateFromLParam(Int64 lParam, ref JoystickEventArgs e)
        {
            //处理X,Y轴
            int x = ((int)(lParam & 0x0000FFFF)) / 2000;                //低16位存储X轴坐标
            int y = ((int)((lParam & 0xFFFF0000) >> 16)) / 2000; //高16位存储Y轴坐标(不直接移位是为避免0xFFFFFF)
            int m = 16;                             //中心点的值,

            e.Move.X = x - m;
            e.Move.Y = y - m;
            if (x > m)
            {
                e.Buttons |= JoystickButtons.Right;
            }
            else if (x < m)
            {
                e.Buttons |= JoystickButtons.Left;
            }
            if (y > m)
            {
                e.Buttons |= JoystickButtons.Down;
            }
            else if (y < m)
            {
                e.Buttons |= JoystickButtons.UP;
            }
        }
        private void GetXYMoveStateFromLParam(int lParam, ref Point move)
        {
            //处理X,Y轴
            move.X = (lParam & 0x0000FFFF) - 0x7EFF;                //低16位存储X轴坐标
            move.Y = (int)((lParam & 0xFFFF0000) >> 16) - 0x7EFF; //高16位存储Y轴坐标(不直接移位是为避免0xFFFFFF)
            //             int x = lParam & 0x0000FFFF;                //低16位存储X轴坐标
            //             int y = (int)((lParam & 0xFFFF0000) >> 16); //高16位存储Y轴坐标(不直接移位是为避免0xFFFFFF)
            //             int m = 0x7EFF;                             //中心点的值,
            //             if (x > m)
            //             {
            //                 buttons |= JoystickButtons.Right;
            //             }
            //             else if (x < m)
            //             {
            //                 buttons |= JoystickButtons.Left;
            //             }
            //             if (y > m)
            //             {
            //                 buttons |= JoystickButtons.Down;
            //             }
            //             else if (y < m)
            //             {
            //                 buttons |= JoystickButtons.UP;
            //             }
        }
        #endregion

        #region IDisposable 成员

        public void Dispose()
        {
            Application.RemoveMessageFilter(this);
        }

        #endregion
    }
}
