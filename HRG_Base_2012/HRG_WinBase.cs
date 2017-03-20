using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HRG_BaseLibrary_2012
{
    //windows 相关操作
    public class HRG_WinBase
    {
        #region 以windows管理员权限运行程序
        public static void RunAsWindowsAdmin(Form frmMain, string args)
        {
            //获得当前登录的Windows用户标示
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            //创建Windows用户主题
            Application.EnableVisualStyles();

            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            //判断当前登录用户是否为管理员
            if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
            {
                //如果是管理员，则直接运行
                Application.Run(frmMain);
            }
            else
            {
                //创建启动对象
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                //设置运行文件
                startInfo.FileName = System.Windows.Forms.Application.ExecutablePath;
                //设置启动参数
                startInfo.Arguments = String.Join(" ", args);
                //设置启动动作,确保以管理员身份运行
                startInfo.Verb = "runas";
                //如果不是管理员，则启动UAC
                System.Diagnostics.Process.Start(startInfo);
                //退出
                System.Windows.Forms.Application.Exit();
            }

        }
        #endregion
    }

    #region win鼠标钩子
    public class MouseHook
    {
        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_RBUTTONDOWN = 0x204;
        private const int WM_MBUTTONDOWN = 0x207;
        private const int WM_LBUTTONUP = 0x202;
        private const int WM_RBUTTONUP = 0x205;
        private const int WM_MBUTTONUP = 0x208;
        private const int WM_LBUTTONDBLCLK = 0x203;
        private const int WM_RBUTTONDBLCLK = 0x206;
        private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>  
        /// 点  
        /// </summary>  
        [StructLayout(LayoutKind.Sequential)]
        public class POINT
        {
            public int x;
            public int y;
        }

        /// <summary>  
        /// 钩子结构体  
        /// </summary>  
        [StructLayout(LayoutKind.Sequential)]
        public class MouseHookStruct
        {
            public POINT pt;
            public int hWnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        public const int WH_MOUSE_LL = 14; // mouse hook constant  

        // 装置钩子的函数  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        // 卸下钩子的函数  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);

        // 下一个钩挂的函数  
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        // 全局的鼠标事件  
        public event MouseEventHandler OnMouseActivity;
        public event MouseEventHandler OnMouseUp;
        public event MouseEventHandler OnMouseDown;
        public event MouseEventHandler OnMouseDBClick;
        // 钩子回调函数  
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

        // 声明鼠标钩子事件类型  
        private HookProc _mouseHookProcedure;
        private static int _hMouseHook = 0; // 鼠标钩子句柄  

        /// <summary>  
        /// 构造函数  
        /// </summary>  
        public MouseHook()
        {

        }

        /// <summary>  
        /// 析构函数  
        /// </summary>  
        ~MouseHook()
        {
            Stop();
        }

        /// <summary>  
        /// 启动全局钩子  
        /// </summary>  
        public void Start()
        {
            // 安装鼠标钩子  
            if (_hMouseHook == 0)
            {
                // 生成一个HookProc的实例.  
                _mouseHookProcedure = new HookProc(MouseHookProc);

                _hMouseHook = SetWindowsHookEx(WH_MOUSE_LL, _mouseHookProcedure, IntPtr.Zero, 0);

                //如果装置失败停止钩子  
                if (_hMouseHook == 0)
                {
                    Stop();
                    throw new Exception("SetWindowsHookEx failed.");
                }
            }
        }

        /// <summary>  
        /// 停止全局钩子  
        /// </summary>  
        public void Stop()
        {
            bool retMouse = true;

            if (_hMouseHook != 0)
            {
                retMouse = UnhookWindowsHookEx(_hMouseHook);
                _hMouseHook = 0;
            }

            // 如果卸下钩子失败  
            if (!(retMouse))
                throw new Exception("UnhookWindowsHookEx failed.");
        }

        /// <summary>  
        /// 鼠标钩子回调函数  
        /// </summary>  
        private int MouseHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            // 如果正常运行并且用户要监听鼠标的消息  
            if (nCode >= 0)
            {
                MouseButtons button = MouseButtons.None;
                int clickCount = 0;
                bool isMouseDown = false;
                bool isMouseUp = false;
                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        button = MouseButtons.Left;
                        isMouseDown = true;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        button = MouseButtons.Left;
                        isMouseUp = true;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        button = MouseButtons.Right;
                        isMouseDown = true;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        button = MouseButtons.Right;
                        isMouseUp = true;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                }

                // 从回调函数中得到鼠标的信息  
                MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseHookStruct));
                MouseEventArgs e = new MouseEventArgs(button, clickCount, MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y, 0);

                // 如果想要限制鼠标在屏幕中的移动区域可以在此处设置  
                // 后期需要考虑实际的x、y的容差  
                if (!Screen.PrimaryScreen.Bounds.Contains(e.X, e.Y))
                {
                    //return 1;  
                }

                if (OnMouseUp != null && isMouseUp)
                    OnMouseUp(this, e);
                if (OnMouseDown != null && isMouseDown)
                    OnMouseDown(this, e);
                if (OnMouseDBClick != null && clickCount == 2)
                    OnMouseDBClick(this, e);
                if (OnMouseDBClick != null && !isMouseDown && !isMouseUp && clickCount == 1) //似乎不会有啊
                    OnMouseActivity(this, e);

            }

            // 启动下一次钩子  
            return CallNextHookEx(_hMouseHook, nCode, wParam, lParam);
        }
    }
    #endregion

    #region win键盘钩子
    public class KeyboardHook
    {
        #region 常数和结构
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;
        private const int WH_KEYBOARD_LL = 13;

        [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型 
        private class KeyboardHookStruct
        {
            public int vkCode; //表示一个在1到254间的虚似键盘码 
            public int scanCode; //表示硬件扫描码 
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        #endregion
        #region Api
        public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        //安装钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);
        //卸下钩子的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern bool UnhookWindowsHookEx(int idHook);
        //下一个钩挂的函数 
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);
        [DllImport("user32")]
        public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);
        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion
        int hHook;
        private bool _DoForbidden = true; //是否屏蔽特定按键
        HookProc KeyboardHookDelegate;
        public event KeyEventHandler OnKeyDownEvent;
        public event KeyEventHandler OnKeyUpEvent;
        public event KeyPressEventHandler OnKeyPressEvent;
        public KeyboardHook() { }
        public void SetHook()
        {
            KeyboardHookDelegate = new HookProc(KeyboardHookProc);
            Process cProcess = Process.GetCurrentProcess();
            ProcessModule cModule = cProcess.MainModule;
            var mh = GetModuleHandle(cModule.ModuleName);
            hHook = SetWindowsHookEx(WH_KEYBOARD_LL, KeyboardHookDelegate, mh, 0);
        }
        public void UnHook()
        {
            UnhookWindowsHookEx(hHook);
        }

        private List<Keys> forbiddenKeysList = new List<Keys>(); //存放禁止响应的按键
        private List<Keys> preKeysList = new List<Keys>();//存放被按下的控制键，用来生成具体的键
        public void AddForbidKey(Keys key)
        {
            forbiddenKeysList.Add(key);
        }

        public void TurnForbiddenSwitch()
        {
            _DoForbidden = !_DoForbidden;
        }

        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            //forbiddenKeysList.Add(Keys.LWin);
            //forbiddenKeysList.Add(Keys.RWin);
            //forbiddenKeysList.Add(Keys.Tab); 
            //forbiddenKeysList.Add((Keys)162); //左侧ctrl
            //forbiddenKeysList.Add((Keys)163); //右侧ctrl
            //forbiddenKeysList.Add((Keys)164); //左侧alt
            //forbiddenKeysList.Add((Keys)165); //右侧alt
            //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
            if ((nCode >= 0) && (OnKeyDownEvent != null || OnKeyUpEvent != null || OnKeyPressEvent != null))
            {
                KeyboardHookStruct KeyDataFromHook = (KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(KeyboardHookStruct));
                Keys keyData = (Keys)KeyDataFromHook.vkCode;
                foreach (Keys key in forbiddenKeysList)
                {
                    if (KeyDataFromHook.vkCode == Convert.ToInt32(key) && _DoForbidden)
                        return -1;//当出现禁止响应的按键时，停止键盘操作
                }
                //按下控制键
                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    if (IsCtrlAltShiftKeys(keyData) && preKeysList.IndexOf(keyData) == -1)
                    {
                        preKeysList.Add(keyData);
                    }
                }
                //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件
                if (OnKeyDownEvent != null && (wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN))
                {
                    KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));

                    OnKeyDownEvent(this, e);
                }
                //WM_KEYDOWN消息将引发OnKeyPressEvent 
                if (OnKeyPressEvent != null && wParam == WM_KEYDOWN)
                {
                    byte[] keyState = new byte[256];
                    GetKeyboardState(keyState);
                    byte[] inBuffer = new byte[2];
                    if (ToAscii(KeyDataFromHook.vkCode, KeyDataFromHook.scanCode, keyState, inBuffer, KeyDataFromHook.flags) == 1)
                    {
                        KeyPressEventArgs e = new KeyPressEventArgs((char)inBuffer[0]);
                        OnKeyPressEvent(this, e);
                    }
                }
                //松开控制键
                if ((OnKeyDownEvent != null || OnKeyPressEvent != null) && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    if (IsCtrlAltShiftKeys(keyData))
                    {
                        for (int i = preKeysList.Count - 1; i >= 0; i--)
                        {
                            if (preKeysList[i] == keyData) { preKeysList.RemoveAt(i); }
                        }
                    }
                }
                //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 
                if (OnKeyUpEvent != null && (wParam == WM_KEYUP || wParam == WM_SYSKEYUP))
                {
                    KeyEventArgs e = new KeyEventArgs(GetDownKeys(keyData));
                    OnKeyUpEvent(this, e);
                }
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }
        //根据已经按下的控制键生成key
        private Keys GetDownKeys(Keys key)
        {
            Keys rtnKey = Keys.None;
            foreach (Keys i in preKeysList)
            {
                if (i == Keys.LControlKey || i == Keys.RControlKey) { rtnKey = rtnKey | Keys.Control; }
                if (i == Keys.LMenu || i == Keys.RMenu) { rtnKey = rtnKey | Keys.Alt; }
                if (i == Keys.LShiftKey || i == Keys.RShiftKey) { rtnKey = rtnKey | Keys.Shift; }
            }
            return rtnKey | key;
        }

        private Boolean IsCtrlAltShiftKeys(Keys key)
        {
            if (key == Keys.LControlKey || key == Keys.RControlKey || key == Keys.LMenu || key == Keys.RMenu || key == Keys.LShiftKey || key == Keys.RShiftKey) { return true; }
            return false;
        }

    }

    #endregion

    #region 用图片做的多边形按钮及窗体

    ///

    /// Summary description for BitmapRegion.
    ///
    public class BitmapRegion
    {
        public BitmapRegion()
        { }

        ///
        /// Create and apply the region on the supplied control
        /// 创建支持位图区域的控件（目前有button和form）
        ///
        /// The Control object to apply the region to控件
        /// The Bitmap object to create the region from位图
        public static void AdjustControlRegion(Control control, Bitmap bitmap,Point location, int width, int height)
        {
            // Return if control and bitmap are null
            //判断是否存在控件和位图
            if (control == null || bitmap == null)
                return;
            // Set our control''s size to be the same as the bitmap
            //设置控件大小为位图大小
            control.Width = bitmap.Width;
            control.Height = bitmap.Height;
            // Check if we are dealing with Form here
            //当控件是form时
            if (control is System.Windows.Forms.Form)
            {
                // Cast to a Form object
                //强制转换为FORM
                Form form = (Form)control;
                // Set our form''s size to be a little larger that the bitmap just
                // in case the form''s border style is not set to none in the first place
                //当FORM的边界FormBorderStyle不为NONE时，应将FORM的大小设置成比位图大小稍大一点
                form.Height = height;
                form.Width = width;
                form.Location = location;
                form.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
                // No border
                //没有边界
                form.FormBorderStyle = FormBorderStyle.None;
                // Set bitmap as the background image
                //将位图设置成窗体背景图片
                form.BackgroundImage = bitmap;
                // Calculate the graphics path based on the bitmap supplied
                //计算位图中不透明部分的边界
                GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
                // Apply new region
                //应用新的区域
                form.Region = new Region(graphicsPath);

            }
            // Check if we are dealing with Button here
            //当控件是button时
            else if (control is System.Windows.Forms.Button)
            {
                // Cast to a button object
                //强制转换为 button
                Button button = (Button)control;
                // Do not show button text
                //不显示button text
                button.Text = "";
                // Change cursor to hand when over button
                //改变 cursor的style
                button.Cursor = Cursors.Hand;
                // Set background image of button
                //设置button的背景图片
                button.BackgroundImage = bitmap;
                // Calculate the graphics path based on the bitmap supplied
                //计算位图中不透明部分的边界
                GraphicsPath graphicsPath = CalculateControlGraphicsPath(bitmap);
                // Apply new region
                //应用新的区域
                button.Region = new Region(graphicsPath);
            }
        }
        ///
        /// Calculate the graphics path that representing the figure in the bitmap
        /// excluding the transparent color which is the top left pixel.
        /// //计算位图中不透明部分的边界
        ///
        /// The Bitmap object to calculate our graphics path from
        /// Calculated graphics path
        private static GraphicsPath CalculateControlGraphicsPath(Bitmap bitmap)
        {
            // Create GraphicsPath for our bitmap calculation
            //创建 GraphicsPath
            GraphicsPath graphicsPath = new GraphicsPath();
            // Use the top left pixel as our transparent color
            //使用左上角的一点的颜色作为我们透明色
            Color colorTransparent = bitmap.GetPixel(0, 0);
            // This is to store the column value where an opaque pixel is first found.
            // This value will determine where we start scanning for trailing opaque pixels.
            //第一个找到点的X
            int colOpaquePixel = 0;
            // Go through all rows (Y axis)
            // 偏历所有行（Y方向）
            for (int row = 0; row < bitmap.Height; row++)
            {
                // Reset value
                //重设
                colOpaquePixel = 0;
                // Go through all columns (X axis)
                //偏历所有列（X方向）
                for (int col = 0; col < bitmap.Width; col++)
                {
                    // If this is an opaque pixel, mark it and search for anymore trailing behind
                    //如果是不需要透明处理的点则标记，然后继续偏历
                    if (bitmap.GetPixel(col, row) != colorTransparent)
                    {
                        // Opaque pixel found, mark current position
                        //记录当前
                        colOpaquePixel = col;
                        // Create another variable to set the current pixel position
                        //建立新变量来记录当前点
                        int colNext = col;
                        // Starting from current found opaque pixel, search for anymore opaque pixels
                        // trailing behind, until a transparent   pixel is found or minimum width is reached
                        ///从找到的不透明点开始，继续寻找不透明点,一直到找到或则达到图片宽度
                        for (colNext = colOpaquePixel; colNext < bitmap.Width; colNext++)
                            if (bitmap.GetPixel(colNext, row) == colorTransparent)
                                break;
                        // Form a rectangle for line of opaque   pixels found and add it to our graphics path
                        //将不透明点加到graphics path
                        graphicsPath.AddRectangle(new Rectangle(colOpaquePixel, row, colNext - colOpaquePixel, 1));
                        // No need to scan the line of opaque pixels just found
                        col = colNext;
                    }
                }
            }
            // Return calculated graphics path
            return graphicsPath;
        }
    }
#endregion
}
