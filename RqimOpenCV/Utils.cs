using Rqim2Lib;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RqimOpenCV
{
    public static class Utils
    {
        public static float Distance(this PointF p1, PointF p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static float Distance2(this PointF p1, PointF p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        public static TSource MinWhere<TSource>(this IEnumerable<TSource> list, Func<TSource, float> selector, out float min)
        {
            min = float.MaxValue;
            TSource minVal = default(TSource);
            foreach (var l in list)
            {
                var v = selector(l);
                if (v <= min)
                {
                    min = v;
                    minVal = l;
                }
            }
            return minVal;
        }


        public static TSource MaxWhere<TSource>(this IEnumerable<TSource> list, Func<TSource, float> selector, out float max)
        {
            max = float.MinValue;
            TSource maxVal = default(TSource);
            foreach (var l in list)
            {
                var v = selector(l);
                if (v > max)
                {
                    max = v;
                    maxVal = l;
                }
            }
            return maxVal;
        }

        static void SetupPixelNeighbors(HashSet<Pixel> Pixels)
        {
            foreach (var p1 in Pixels)
            {
                p1.NeighborsNum = 0;
                p1.GroupId = -1;
                foreach (var p2 in Pixels)
                {
                    if (p1.Distance2(p2) < 3 && p1 != p2)
                    {
                        p1.Neighbors[p1.NeighborsNum] = p2;
                        p1.NeighborsNum++;
                    }
                }
            }
        }

        static List<HashSet<Rqim2Lib.Pixel>> GroupPixelsByConnectivity(HashSet<Pixel> Pixels)
        {
            List<HashSet<Rqim2Lib.Pixel>> groups = new List<HashSet<Rqim2Lib.Pixel>>();
            SetupPixelNeighbors(Pixels);
            int groupCounter = 0;
            foreach (var p in Pixels)
            {
                if (p.GroupId == -1)
                {
                    GroupPixelNeighborsRec(p, groupCounter);
                    groupCounter++;
                    groups.Add(new HashSet<Rqim2Lib.Pixel>());
                }
            }
            foreach (var p in Pixels)
            {
                groups[p.GroupId].Add(p);
            }
            return groups;
        }

        public static HashSet<Rqim2Lib.Pixel> GetLargestPixelGroup(HashSet<Pixel> Pixels)
        {
            var groups = GroupPixelsByConnectivity(Pixels);
            float f;
            return groups.MaxWhere(x => x.Count, out f);
        }

        static void GroupPixelNeighborsRec(Rqim2Lib.Pixel p, int groupId)
        {
            p.GroupId = groupId;
            for (int i = 0; i < p.NeighborsNum; i++)
            {
                if (p.Neighbors[i].GroupId != groupId)
                {
                    GroupPixelNeighborsRec(p.Neighbors[i], groupId);
                }
            }
        }

        public static float GetAngle(PointF p1, PointF p2, PointF p3)
        {
            float A2 = p1.Distance2(p2);
            float B2 = p2.Distance2(p3);
            float C2 = p3.Distance2(p1);

            float cc=(A2+B2-C2)/(2*(float)Math.Sqrt(A2)*(float)Math.Sqrt(B2));
            var angle=(float)Math.Acos(cc)*180/(float)Math.PI;
            return angle;
        }

    }
}
