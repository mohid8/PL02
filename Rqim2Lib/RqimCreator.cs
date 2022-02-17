using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class RqimCreator
    {

        //public static Rqim CreateTestRqim()
        //{




        //    Rqim rqim = new Rqim();

        //    Quad q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.3f, 0.2f);
        //    q.Inner1 = new System.Drawing.PointF(0.38f, 0.11f);
        //    q.Inner2 = new System.Drawing.PointF(0.52f, 0.13f);
        //    q.End2 = new System.Drawing.PointF(0.6f, 0.3f);
        //    rqim.AddQuad(q);

        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.7f, 0.67f);
        //    q.Inner1 = new System.Drawing.PointF(0.83f, 0.85f);
        //    q.Inner2 = new System.Drawing.PointF(0.65f, 0.93f);
        //    q.End2 = new System.Drawing.PointF(0.56f, 0.71f);
        //    rqim.AddQuad(q);

        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.35f, 0.54f);
        //    q.Inner1 = new System.Drawing.PointF(0.13f, 0.75f);
        //    q.Inner2 = new System.Drawing.PointF(0.42f, 0.83f);
        //    q.End2 = new System.Drawing.PointF(0.47f, 0.67f);
        //    rqim.AddQuad(q);

        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.26f, 0.22f);
        //    q.Inner1 = new System.Drawing.PointF(0.09f, 0.1f);
        //    q.Inner2 = new System.Drawing.PointF(0.13f, 0.42f);
        //    q.End2 = new System.Drawing.PointF(0.31f, 0.37f);
        //    rqim.AddQuad(q);

        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.39f, 0.41f);
        //    q.Inner1 = new System.Drawing.PointF(0.51f, 0.33f);
        //    q.Inner2 = new System.Drawing.PointF(0.61f, 0.49f);
        //    q.End2 = new System.Drawing.PointF(0.46f, 0.55f);
        //    rqim.AddQuad(q);

        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.69f, 0.33f);
        //    q.Inner1 = new System.Drawing.PointF(0.65f, 0.08f);
        //    q.Inner2 = new System.Drawing.PointF(0.93f, 0.15f);
        //    q.End2 = new System.Drawing.PointF(0.87f, 0.39f);
        //    rqim.AddQuad(q);


        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.83f, 0.21f);
        //    q.Inner1 = new System.Drawing.PointF(0.72f, 0.22f);
        //    q.Inner2 = new System.Drawing.PointF(0.73f, 0.34f);
        //    q.End2 = new System.Drawing.PointF(0.81f, 0.46f);
        //    rqim.AddQuad(q);


        //    q = new Quad();
        //    q.End1 = new System.Drawing.PointF(0.10f, 0.96f);
        //    q.Inner1 = new System.Drawing.PointF(0.08f, 0.83f);
        //    q.Inner2 = new System.Drawing.PointF(0.24f, 0.85f);
        //    q.End2 = new System.Drawing.PointF(0.20f, 0.95f);
        //    rqim.AddQuad(q);


        //    var im = RqimRenderer.Render(rqim, 640, 480);

        //    im.SaveImage("test1.png");

        //    return rqim;

        //}

        static Random random = new Random(0xaa50);
        public static Rqim CreateDiscreteRandomRqim(int num, float sizeDistribution = 6)
        {
            var sizes = new List<float>(num);
            int n = 0;
            while (n < num)
            {
                float s = (float)Math.Exp(-random.NextDouble() * sizeDistribution);
                if (s <= 1.3f && s >= 0.01)
                {
                    sizes.Add(s);
                    n++;
                }
            }

            sizes.Sort((a, b) => { if (a < b) return 1; if (a == b) return 0; return -1; });

            Rqim rqim = new Rqim();

            n = 0;
            int tries = 0;
            int actualtries = 0;
            while (n < num)
            {
                float mins; 
                var size=Quad.DiscreteSizes.MinWhere(x=>Math.Abs(x-sizes[n]), out mins);
                var q = Quad.CreateDiscreteRandom(float.NaN, float.NaN, size, float.NaN);
                //var q = Quad.CreateRandom(sizes[n], 0.4f, 1.6f, 40.0f / 180.0f * (float)Math.PI, 140.0f / 180.0f * (float)Math.PI, r);
                if (q.IsValid())
                {
                    bool ok = true;
                    for (int i = 0; i < rqim.QuadCount; i++)
                    {
                        if (q.HasIntersectionWith(rqim[i], 0.03f))
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {

                        //if (n == 30)
                        //{

                        //}
                        n++;
                        rqim.AddQuad(q);
                        actualtries = 0;
                        Console.WriteLine(string.Format("{0}/{1}", n, num));
                        //RqimRenderer.SaveAsImage("random" + n.ToString() + ".png", rqim);

                    }
                }
                tries++;
                actualtries++;
                if (actualtries > 1000000)
                {
                    n++;
                    actualtries = 0;
                }
                if (tries > 100000000)
                {
                    break;
                }
            }

            return rqim;

        }



        public static Rqim CreateRandomRqim(int num, float sizeDistribution = 6)
        {

            Random r = new Random(100);

            var sizes = new List<float>(num);
            int n = 0;
            while (n < num)
            {
                float s = (float)Math.Exp(-r.NextDouble() * sizeDistribution);
                if (s < 0.7f && s > 0.05)
                {
                    sizes.Add(s);
                    n++;
                }
            }

            sizes.Sort((a, b) => { if (a < b) return 1; if (a == b) return 0; return -1; });

            Rqim rqim = new Rqim();

            n = 0;
            int tries = 0;
            int actualtries = 0;
            while (n < num)
            {

                var q = Quad.CreateRandom(sizes[n], 0.4f, 1.6f, 40.0f / 180.0f * (float)Math.PI, 140.0f / 180.0f * (float)Math.PI, r);
                if (q.IsValid())
                {
                    bool ok = true;
                    for (int i = 0; i < rqim.QuadCount; i++)
                    {
                        if (q.HasIntersectionWith(rqim[i], 0.05f))
                        {
                            ok = false;
                            break;
                        }
                    }
                    if (ok)
                    {

                        //if (n == 30)
                        //{

                        //}
                        n++;
                        rqim.AddQuad(q);
                        actualtries = 0;

                        //RqimRenderer.SaveAsImage("random" + n.ToString() + ".png", rqim);

                    }
                }
                tries++;
                actualtries++;
                if (actualtries > 1000000)
                {
                    n++;
                    actualtries = 0;
                }
                if (tries > 100000000)
                {
                    break;
                }
            }

            return rqim;

        }






    }
}
