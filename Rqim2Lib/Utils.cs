using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public static class Utils
    {
        public static float Distance(this PointF p1, PointF p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
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


    






    }
}
