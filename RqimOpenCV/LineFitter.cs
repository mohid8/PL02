using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RqimOpenCV
{

    public class Line
    {
        public float vx;
        public float vy;
        public float px;
        public float py;

        public List<Rqim2Lib.Pixel> Pixels;

        public void NormalizeV()
        {
            var len2 = (vx * vx + vy * vy);
            var len_ = 1/(float)Math.Sqrt(len2);
            vx *= len_;
            vy *= len_;
        }
        public Line()
        {

        }
        public Line(float ax, float ay, float bx, float by)
        {
            px = ax;
            py = ay;
            vx = (ax - bx);
            vy = (ay - by);
            NormalizeV();
        }

        public float PointDistance(float x, float y)
        {
            var d= vx * (py - y) - (px - x) * vy;
            return Math.Abs(d);
        }

        public void Get2Points(float d, out PointF a, out PointF b)
        {
            a = new PointF(px - vx * d, py - vy * d);
            b = new PointF(px + vx * d, py + vy * d);
        }

        //void SetupPixelNeighbors()
        //{
        //    foreach (var p1 in Pixels)
        //    {
        //        p1.NeighborsNum = 0;
        //        p1.GroupId = -1;
        //        foreach (var p2 in Pixels)
        //        {
        //            if (p1.Distance2(p2) < 3 && p1 != p2)
        //            {
        //                p1.Neighbors[p1.NeighborsNum] = p2;
        //                p1.NeighborsNum++;
        //            }
        //        }
        //    }
        //}
         
        //List<HashSet<Rqim2Lib.Pixel>> GroupPixelsByConnectivity()
        //{
        //    List<HashSet<Rqim2Lib.Pixel>> groups = new List<HashSet<Rqim2Lib.Pixel>>();
        //    SetupPixelNeighbors();
        //    int groupCounter = 0;
        //    foreach (var p in Pixels)
        //    {
        //        if (p.GroupId == -1)
        //        {
        //            GroupPixelNeighborsRec(p, groupCounter);
        //            groupCounter++;
        //            groups.Add(new HashSet<Rqim2Lib.Pixel>());
        //        }
        //    }
        //    foreach (var p in Pixels)
        //    {
        //        groups[p.GroupId].Add(p);
        //    }
        //    return groups;
        //}

        //public HashSet<Rqim2Lib.Pixel> GetLargestPixelGroup()
        //{
        //    var groups = GroupPixelsByConnectivity();
        //    float f;
        //    return groups.MaxWhere(x => x.Count, out f);
        //}

        //void GroupPixelNeighborsRec(Rqim2Lib.Pixel p, int groupId)
        //{
        //    p.GroupId = groupId;
        //    for (int i = 0; i < p.NeighborsNum; i++)
        //    {
        //        if (p.Neighbors[i].GroupId != groupId)
        //        {
        //            GroupPixelNeighborsRec(p.Neighbors[i], groupId);
        //        }
        //    }
        //}

        public Rqim2Lib.Pixel IsNeighborWith(Line line2) {
            foreach (var p1 in Pixels)
            {
                foreach (var p2 in line2.Pixels)
                {
                    if (p1.Distance2(p2) < 3) {
                        return p1;
                    }
                }
            }
            return null;
        }

        public PointF InterSection(Line l)
        {
            var x1 = px;
            var x2 = px + vx;
            var y1 = py;
            var y2 = py + vy;
            var x3 = l.px;
            var y3 = l.py;
            var x4 = l.px + l.vx;
            var y4 = l.py + l.vy;

            var denum = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);
            var x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / denum;
            var y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / denum;

            return new PointF(x, y);
        }

        public Rqim2Lib.Pixel[] GetEndPoints()
        {
            List<Rqim2Lib.Pixel> endpoints = new List<Rqim2Lib.Pixel>();

            foreach (var p0 in Pixels)
            {
                int n = 0;
                foreach (var p1 in Pixels)
                {
                    if (p0.Distance2(p1) < 3 && p0 != p1)
                    {
                        n++;
                    }
                }
                if (n == 1)
                {
                    endpoints.Add(p0);
                }
            }

            return endpoints.ToArray();
        }

    }



    public class LineFitter
    {
        //public static Line Fit(PointF a, PointF b)
        //{
        //    return new Line(a, b);
        //}
        public static Line Fit(float  ax, float ay, float bx, float by)
        {
            return new Line(ax,ay,bx,by);
        }

        public LineFitter(int numPoints)
	    {
            X = new List<double>(numPoints);
            Y = new List<double>(numPoints);
	    }
        protected List<double> X;
        protected List<double> Y;
        protected double sumx = 0;
        protected double sumy = 0;

        protected int n = 0;

        public int PointsNum { get { return n; } }

        public void AddPoint(double x, double y)
        {
            X.Add(x);
            Y.Add(y);
            sumx += x;
            sumy += y;
            n++;
        }


        public Line GetFittedXY()
        {
            double x, y;
            if (n < 2)
            {
                x = double.NaN;
                y = double.NaN;
                return null;
            }
            double meanx = sumx / n;
            double meany = sumy / n;

            double varx2 = 0;
            double vary2 = 0;
            double varxy = 0;
            for (int i = 0; i < n; i++)
            {
                varx2 += (X[i] - meanx) * (X[i] - meanx);
                vary2 += (Y[i] - meany) * (Y[i] - meany);
                varxy += (X[i] - meanx) * (Y[i] - meany);
            }
            varx2 /= n - 1;
            vary2 /= n - 1;
            varxy /= n - 1;

            if (varxy == 0)
            {
                if (varx2 > vary2)
                {
                    x = 1; y = 0;
                    return new Line() { px = (float)meanx, py = (float)meany, vx = (float)x, vy = (float)y };
                }
                if (vary2 > varx2)
                {
                    x = 0; y = 1;
                    return new Line() { px = (float)meanx, py = (float)meany, vx = (float)x, vy = (float)y };
                }
                x = double.NaN; y = double.NaN;
                return null;
            }

            var l1 = (varx2 + vary2 + Math.Sqrt(Math.Pow(varx2 + vary2, 2) - 4 * (varx2 * vary2 - varxy * varxy)))*0.5;
            var l2 =( varx2 + vary2 - Math.Sqrt(Math.Pow(varx2 + vary2, 2) - 4 * (varx2 * vary2 - varxy * varxy)))*0.5;

            var l = l1;
            if (Math.Abs(l2) > Math.Abs(l1)) l = l2;

            double v2=-(varx2-l)/varxy;
            double len = Math.Sqrt( 1 + v2 * v2);
            v2 /= len;
            double v1 = 1 / len;

            x = v1;
            y = v2;
            if (y < 0)
            {
                x = -v1;
                y = -v2;
            }
            return new Line() { px = (float)meanx, py = (float)meany, vx = (float)x, vy = (float)y };
        }

    }
}
