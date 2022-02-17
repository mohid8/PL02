using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class MyRandom
    {
        static Random rand = new Random(0xaa55);
        //public static double GetNormalRandom(double mean, double stdDev)
        //{
        //    double u1 = rand.NextDouble();
        //    double u2 = rand.NextDouble();
        //    double stdnorm = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2);
        //    var r = mean + stdDev * stdnorm;
        //    if (r < 0) { r = 0; }
        //    return r;
        //}

        readonly static object lockO = new object();

        public static float GetStdDevRandom(float stdDev)
        {
            double u1, u2;
            lock (lockO)
            {
                u1 = rand.NextDouble();
                u2 = rand.NextDouble();
            }
            double stdnorm = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2);
            return (float)(stdDev * stdnorm);
        }

    }
}
