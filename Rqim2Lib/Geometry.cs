using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class Geometry
    {
        public static readonly float PIf = (float)Math.PI;

        public static float ToRad(float deg)
        {
            return PIf / 180.0f * deg;
        }

        public static float ToDeg(float rad)
        {
            return 180.0f / PIf * rad;
        }


        public static float LineSegmentsIntersect(PointF P1, PointF P2, PointF Q1, PointF Q2)
        {

            var dx = P2.X - P1.X;
            var dy = P2.Y - P1.Y;
            double alpha = Math.Atan2(dy, dx);

            var p2 = new PointF(dx, dy).Rotate(-alpha);
            var q1 = new PointF(Q1.X - P1.X, Q1.Y - P1.Y).Rotate(-alpha);
            var q2 = new PointF(Q2.X - P1.X, Q2.Y - P1.Y).Rotate(-alpha);

            if (Math.Abs(q1.X - q2.X) < 0.0001f)
            {
                //fuggoleges
                if (q1.X >= 0 && q1.X <= p2.X)
                {
                    return 0;
                }
                return Math.Min(Math.Abs(q1.X), Math.Abs(q1.X - p2.X));
            }

            if (q2.X < q1.X)
            {
                var tmp = q2;
                q2 = q1;
                q1 = tmp;
            }

            var qdx = q2.X - q1.X;
            var qdy = q2.Y - q1.Y;

            float intersectX = -q1.Y * qdx / qdy + q1.X;
            if ((intersectX >= 0) && (intersectX <= p2.X) && (intersectX >= q1.X) && (intersectX <= q2.X))
            {
                return 0;
            }

            float m = qdy / qdx;

            float[] distSqs = new float[8];
            for (int i = 0; i < 8; i++) { distSqs[i] = float.MaxValue; }

            if (q1.X >= 0 && q1.X <= p2.X)
            {
                distSqs[0] = q1.Y * q1.Y;
            }
            if (q2.X >= 0 && q2.X <= p2.X)
            {
                distSqs[1] = q2.Y * q2.Y;
            }



          

            //TODO: mindket iranyba! mindket szakaszra vizsgalni!
            float u;
            {
                u = ((0 - q1.X) * (q2.X - q1.X) + (0 - q1.Y) * (q2.Y - q1.Y)) / (Geometry.DistanceSq(q1, q2));
                if (u >= 0 && u <= 1)
                {
                    var w = new PointF(q1.X + u * (q2.X - q1.X), q1.Y + u * (q2.Y - q1.Y));
                    distSqs[6] = w.X * w.X + w.Y * w.Y;
                }
            }

            {
                u = ((p2.X - q1.X) * (q2.X - q1.X) + (0 - q1.Y) * (q2.Y - q1.Y)) / (Geometry.DistanceSq(q1, q2));
                if (u >= 0 && u <= 1)
                {
                    var w = new PointF(q1.X + u * (q2.X - q1.X), q1.Y + u * (q2.Y - q1.Y));
                    distSqs[7] = Geometry.DistanceSq(p2, w);
                }
            }

            float min = distSqs[0];
            for (int i = 1; i < 8; i++)
            {
                if (min > distSqs[i])
                {
                    min = distSqs[i];
                }
            }
            //valami nem ok
            return (float)Math.Sqrt(min);

           
        }

        public static float DistanceSq(PointF a, PointF b)
        {
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }

        public static float DistanceLinePoint(PointF A, PointF B, PointF x)
        {
            float nom = (B.Y - A.Y) * x.X - (B.X - A.X) * x.Y + B.X * A.Y - B.Y * A.X;
            return nom*nom/DistanceSq(A,B);
        }
       

    }

    public static class GeometryHelper
    {
        public static PointF Rotate(this PointF a, double theta)
        {
            var c = (float)Math.Cos(theta);
            var s = (float)Math.Sin(theta);
            return new PointF(a.X * c - a.Y * s, a.X * s + a.Y * c);
        }

        public static PointF Translate(this PointF a, PointF b)
        {
            return new PointF(a.X + b.X, a.Y + b.Y);
        }
    }
}
