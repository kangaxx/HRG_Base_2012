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
        private Color _back, _ellipse;

        private List<List<object>> DrawedRecords; //记录当前绘制过的对象
        public HRG_DrawHelper(Graphics g, Color back)
        {
            _DrawRegion = g;
            _back = back;
            _ellipse = Color.Green;
        }

        public HRG_DrawHelper(Graphics g, Color back, Color ellipse)
        {
            _DrawRegion = g;
            _back = back;
            _ellipse = ellipse;
        }

        public void DrawLineDemo()
        {
            _DrawRegion.DrawLine(new Pen(Color.Black, 1.0f), new Point(10, 10), new Point(20, 20));
        }



        public void Clear() //清理画布，也可用于绘制动画帧
        {
            _DrawRegion.Clear(_back);
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

                _DrawRegion.FillEllipse(new SolidBrush(_ellipse), start.X + Convert.ToInt32(i * X_DIST), start.Y + Convert.ToInt32(i * Y_DIST) , 50, 50);
                System.Threading.Thread.Sleep(GlobalVariables.INT_HRG_FLAGINTERVAL);

            }
        }

        //width 宽度  ， height 高度, time时间（秒）
        public void EllipseMove(PointF start, PointF end, int width, int height, int time, SolidBrush brush)
        {
            int FlagNum = time * GlobalVariables.INT_HRG_FLAGNUM_PERSECOND;
            double X_DIST = Convert.ToDouble(end.X - start.X) / Convert.ToDouble(FlagNum);
            double Y_DIST = Convert.ToDouble(end.Y - start.Y) / Convert.ToDouble(FlagNum);
            for (int i = 0; i < FlagNum; i++)
            {
                Clear();

                _DrawRegion.FillEllipse(new SolidBrush(_ellipse), start.X + Convert.ToInt32(i * X_DIST), start.Y + Convert.ToInt32(i * Y_DIST), 50, 50);
                System.Threading.Thread.Sleep(GlobalVariables.INT_HRG_FLAGINTERVAL);

            }
        }

        public void EllipseLaneChangeDemo(PointF CarS, PointF CarE, PointF RobotS, PointF RobotE, int width, int height, int time , SolidBrush CarBrush, SolidBrush RobotBrush)
        {
            int FlagNum = time * GlobalVariables.INT_HRG_FLAGNUM_PERSECOND;
            double X_DIST_Car = Convert.ToDouble(CarE.X - CarS.X) / Convert.ToDouble(FlagNum);
            double Y_DIST_Car = Convert.ToDouble(CarE.Y - CarS.Y) / Convert.ToDouble(FlagNum);

            double X_DIST_Robot = Convert.ToDouble(RobotE.X - RobotS.X) / Convert.ToDouble(FlagNum);
            double Y_DIST_Robot = Convert.ToDouble(RobotE.Y - RobotS.Y) / Convert.ToDouble(FlagNum);
            for (int i = 0; i < FlagNum; i++)
            {
                Clear();

                _DrawRegion.FillEllipse(new SolidBrush(Color.Blue), CarS.X + Convert.ToInt32(i * X_DIST_Car), CarS.Y + Convert.ToInt32(i * Y_DIST_Car), width, height);
                _DrawRegion.FillEllipse(new SolidBrush(Color.Red), RobotS.X + Convert.ToInt32(i * X_DIST_Robot), RobotS.Y + Convert.ToInt32(i * Y_DIST_Robot), width, height);

                System.Threading.Thread.Sleep(GlobalVariables.INT_HRG_FLAGINTERVAL);

            }
        }

        public void DrawEllipse(PointF p, int w, int h)
        {
            _DrawRegion.FillEllipse(new SolidBrush(_ellipse), p.X, p.Y, w, h);
        }

        public void DrawPositionText(PointF p, float x, float y)
        {
            _DrawRegion.DrawString(String.Format("mouse position X : {0} Y : {1}", x, y), new Font("宋体", 20, FontStyle.Bold), new SolidBrush(Color.Black), p);
        }

    }
    #endregion


}

