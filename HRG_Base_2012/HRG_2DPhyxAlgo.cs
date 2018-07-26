using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace HRG_BaseLibrary_2012
{
    public class RobotSpeed
    {
        public RobotSpeed()
        {

        }

        public float GetSpeed()
        {
            return 1.0f;
        }

    }

    //移动路线
    public class Route
    {
        private PointF _start, _end;
        public Route(PointF s, PointF e)
        {
            _start = s;
            _end = e;
        }
    }
    //虚拟移动设备
    public class Robot
    {
        private PointF _pos; //当前位置
        private Route _route;
        private int _id;
        private int _status; //机器人当前状态。
        private float _SignalFreq; //信号频率
        public Robot(Route r, PointF p, int id)
        {
            _route = r;
            _pos = p;
            _id = id;
        }

        //设置一个行进目的地
        public int SetTarget(PointF target)
        {
            return -1;

        }

        //获取机器人下一步移动的位置坐标
        public PointF GetNextPos()
        {
            PointF result = new PointF(0f, 0f);
            return result;
        }


        //获取机器人id
        public int GetRobotId()
        {
            return _id;
        }

        //移动一步 ，由于坐标系统中只有x，y轴是整数，如果是斜向移动，则需要合理分配横纵向上
        private void MoveOneStep()
        {

        }

    }

    //平面物理计算
    public class HRG_2DPhyxAlgo
    {

        private int _width, _height;
        public HRG_2DPhyxAlgo(int Width, int Height)
        {
            _width = Width;
            _height = Height;
        }

        



    }

    //路径规划智能算法，改进迪杰斯特拉算法。
    class HRG_PathCreator
    {
        private int _width, _height;
        public HRG_PathCreator(int width, int height)
        {
            _width = width;
            _height = height;
        }



        //判断当前机器人膨胀半径内是否有障碍物 g 地图 p 当前机器人位置 TerranPoint 地形障碍物坐标列表，TempPoint 临时障碍物坐标列表， RobotPoint 其他机器人坐标列表， Radius 碰撞半径
        public int IsBlocked(Graphics g, List<PointF> TerranPoint, List<PointF> TempPoint, List<Robot> RobotList, int Radius)
        {
            if (Radius <= 1) throw new Exception("Input arg radius is error!");
            int result = 0;
            //地形及临时障碍物碰撞算法暂时未加入，需要添加地图分区索引，否则计算将会非常耗时

            //判断机器人下一步移动的位置是否会和其他机器人冲突
            foreach(Robot temp in RobotList)
            {

            }


            return GlobalVariables.INT_HRG_PHYX_BLOCK_TYPE_NONE;
        }

        //判断机器人移动是否有阻碍 返回1 不能移动 返回0 可以移动 返回-1 发生故障， 路径堵死 
        public int RobotGoStatus(List<Robot> RobotList)
        {

            return -1;
        }

        private double GetDistance(Point startPoint, Point endPoint)
        {
            int x = System.Math.Abs(endPoint.X - startPoint.X);
            int y = System.Math.Abs(endPoint.Y - startPoint.Y);
            return Math.Sqrt(x * x + y * y);
        }
    }
}

