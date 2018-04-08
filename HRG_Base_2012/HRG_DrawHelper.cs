using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HRG_BaseLibrary_2012
{
    #region 绘图程序
    public class HRG_DrawHelper
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
            int x = 100;
            int y = 100;
            for (int i = 0; i <60; i++)
            {
                Clear();

                _DrawRegion.FillEllipse(new SolidBrush( Color.Green), x, y, 50, 50);
                System.Threading.Thread.Sleep(40);
                x = x + 2;
                y = y + 2;
            }
        }

        //width 宽度  ， height 高度, time时间（秒）
        public void EllipseMove(Point start, Point end, int width, int height, int time, SolidBrush brush)
        {
            int FlagNum = time * GlobalVariables.INT_HRG_FLAGNUM_PERSECOND;
            double X_DIST = Convert.ToDouble(end.X - start.X) / Convert.ToDouble(FlagNum);
            double Y_DIST = Convert.ToDouble(end.Y - start.Y) / Convert.ToDouble(FlagNum);
            for(int i = 0; i < FlagNum; i++)
            {
                Clear();

                _DrawRegion.FillEllipse(new SolidBrush(Color.Green), start.X + Convert.ToInt32(i * X_DIST), start.Y + Convert.ToInt32(i * Y_DIST) , 50, 50);
                System.Threading.Thread.Sleep(GlobalVariables.INT_HRG_FLAGINTERVAL);

            }
        }
    }
    #endregion
}
