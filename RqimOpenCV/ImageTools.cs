using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RqimOpenCV
{
    public static class ImageTools
    {

       
        static Dictionary<string, IDictionary<object, UInt32>> colorDict = new Dictionary<string, IDictionary<object, uint>>();
        static List<UInt32> colors = new List<uint>() {
                0xff000000U,
                0xffff0000U,
                0xff00ff00U,
                0xff0000ffU,
                0xff00ffffU,
                0xffff00ffU,
                0xff800000U,
                0xff008000U,
                0xff000080U,
                0xff008080U,
                0xff808000U,
                0xff808080U,
                0xffff8000U,
                0xffff0080U,
                0xff80ff00U,
                0xff8000ffU,
                0xff80ff80U,
                0xff40b000U,
                0xff40b040U,
                0xffb0b000U,
                0xffb04000U,
                0xffb0b0b0U,
                0xff40b0b0U,
                0xffb040b0U,
                
             };
        static Random r = new Random();
        public static UInt32 GetColor<T>(string ID, T k)
        {
            IDictionary<object, UInt32> d;
            if (colorDict.ContainsKey(ID))
            {
                d = colorDict[ID];
            }
            else
            {
                d = new Dictionary<object, uint>();
                colorDict.Add(ID, d);
            }
            uint val;
            if (d.ContainsKey(k))
            {
                val = d[k];
            }
            else
            {

                val = colors.Where(x => !d.Values.Contains(x)).FirstOrDefault();
                if (val == 0)
                {
                    val = 0xff000000U | (uint)r.Next(256) << 16 | (uint)r.Next(256) << 8 | (uint)r.Next(256);
                }
                d[k] = val;
            }
            return val;

        }

        public static UInt32 GetRandomColor<T>(string ID, T k, byte max = 255, byte min = 0)
        {
            IDictionary<object, UInt32> d;
            if (colorDict.ContainsKey(ID))
            {
                d = colorDict[ID];
            }
            else
            {
                d = new Dictionary<object, uint>();
                colorDict.Add(ID, d);
            }
            uint val;
            if (d.ContainsKey(k))
            {
                val = d[k];
            }
            else
            {
                val = RandomRGB(max, min);
                /*val = colors.Where(x => !d.Values.Contains(x)).FirstOrDefault();
                if (val == 0)
                {
                    val = 0xff000000U | (uint)r.Next(256) << 16 | (uint)r.Next(256) << 8 | (uint)r.Next(256);
                }*/
                d[k] = val;
            }
            return val;

        }

        public static UInt32 RandomRGB(byte max = 255, byte min = 0)
        {
            return (UInt32)0xff000000U | (UInt32)(r.Next(min, max) << 16) | (UInt32)(r.Next(min, max) << 8) | (UInt32)(r.Next(min, max));
        }

        public static TSource MinWhere<TSource>(this IEnumerable<TSource> list, Func<TSource, double> selector)
        {
            double min = double.MaxValue;
            TSource minVal = default(TSource);
            foreach (var l in list)
            {
                var v = selector(l);
                if (v < min)
                {
                    min = v;
                    minVal = l;
                }
            }
            return minVal;
        }


    }
}
