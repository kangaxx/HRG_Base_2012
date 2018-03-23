using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HRG_Base_2012
{
    #region 绘图程序
    class HRG_DrawHelper
    {
        private Graphics _DrawRegion;
        private Color _color;

        public HRG_DrawHelper(Graphics g, Color c)
        {
            _DrawRegion = g;
            _color = c;
        }


        public void DrawLineDemo()
        {
            _DrawRegion.DrawLine(new Pen(Color.Black, 1.0f), new Point(10, 10), new Point(20, 20));
        }



        public void Clear() //清理画布，也可用于绘制动画帧
        {
            _DrawRegion.Clear(_color);
        }

        public void AnimateDemo()
        {


        }
    }
    #endregion
}
