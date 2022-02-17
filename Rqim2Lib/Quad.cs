using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Rqim2Lib
{
     [DataContract]
    public class Quad
    {
        [DataMember]
        public int Id = -1;
        [DataMember]
        public PointF[] Corners = new PointF[4];
        [XmlIgnore]
        public PointF End1 { get { return Corners[0]; } set { Corners[0] = value; } }
        [XmlIgnore]
        public PointF End2 { get { return Corners[3]; } set { Corners[3] = value; } }
        [XmlIgnore]
        public PointF Inner1 { get { return Corners[1]; } set { Corners[1] = value; } }
        [XmlIgnore]
        public PointF Inner2 { get { return Corners[2]; } set { Corners[2] = value; } }
        [XmlIgnore]
        public float InnerLength
        {
            get
            {
                return (float)Math.Sqrt(Geometry.DistanceSq(Corners[1], Corners[2]));
            }
        }
        [XmlIgnore]
        public float Area
        {
            get
            {
                var a = new PointF(Corners[0].X - Corners[2].X, Corners[0].Y - Corners[2].Y);
                var b = new PointF(Corners[1].X - Corners[3].X, Corners[1].Y - Corners[3].Y);
                return (float)Math.Sqrt(0.5f * Math.Abs((a.X - b.Y) * (a.Y - b.X)));
            }
        }


        public static float[] DiscreteAngles = new float[] {30, 60, 90, 135 };
        public static float[] DiscreteSideMultipliers = new float[] { 0.40f, 0.60f, 0.80f, 1.0f, 1.25f, 1.50f, 1.75f, 2.0f, };
        public static float[] DiscreteOrientations = new float[] { 0, 22.5f, 45f, 67.5f, 90f, 112.5f, 135f, 157.5f, 180f, 202.5f, 225f, 247.5f, 270f, 292.5f, 315f, 337.5f };
        public static float[] DiscreteSizes = new float[] { 1, 0.8f, 0.6f, 0.5f, 0.4f, 0.3f, 0.25f, 0.2f, 0.1f, 0.08f, 0.06f, 0.05f, 0.04f, 0.03f, 0.025f, 0.02f };
        public static Quad CreateDiscrete(int qID, float x=0, float y=0)
        {
            //size (4)  orientation (4)  multiplier b (3)  multiplier c (3)  angle b (3)  angle c (3) -> 4+4+12=20bit

            int angleBIndex=(qID & (7<<0))>>0;
            int angleCIndex=(qID & (7<<3))>>3;
            int multiplierBIndex=(qID & (7<<6))>>6;
            int multiplierCIndex=(qID & (7<<9))>>9;
            int orientationIndex=(qID & (7<<12))>>12;
            int sizeIndex = (qID & (7 << 16)) >> 16;

            var angleB = DiscreteAngles[angleBIndex];
            var angleC = DiscreteAngles[angleCIndex];
            var multiplierB = DiscreteSideMultipliers[multiplierBIndex];
            var multiplierC = DiscreteSideMultipliers[multiplierCIndex];
            var orientation = DiscreteOrientations[orientationIndex];
            var size = DiscreteSizes[sizeIndex];

            var q = CreateQuad(size, angleB, angleC, multiplierB, multiplierC,orientation);

            q.FixWinding();
            q.Id = qID;

            return q.Translate2D(x+0.5f, y+0.5f);

        }

        static Random random = new Random(0xaa62);

        public static Quad CreateDiscreteRandom(float x=float.NaN, float y=float.NaN, float Size=float.NaN, float Orientation=float.NaN)
        {
            //size (4)  orientation (4)  multiplier b (3)  multiplier c (3)  angle b (3)  angle c (3) -> 4+4+12=20bit

            int angleBIndex = random.Next(4);
            int angleCIndex = random.Next(4 );
            int multiplierBIndex = random.Next(8);
            int multiplierCIndex = random.Next(8);
            int orientationIndex = random.Next(16);
            int sizeIndex = random.Next(16);

            var angleB = DiscreteAngles[angleBIndex]* (float)Math.PI / 180;
            var angleC = DiscreteAngles[angleCIndex] * (float)Math.PI / 180;
            var multiplierB = DiscreteSideMultipliers[multiplierBIndex];
            var multiplierC = DiscreteSideMultipliers[multiplierCIndex];
            var orientation = DiscreteOrientations[orientationIndex];
            if (!float.IsNaN(Orientation)) { orientation = Orientation; }
            var size = DiscreteSizes[sizeIndex];
            if (!float.IsNaN(Size)) { size = Size; }
            orientation  *=(float)Math.PI / 180;

            var q = CreateQuad(size, angleB , angleC, multiplierB, multiplierC, orientation );

            q.FixWinding();
            if (float.IsNaN(x))
            {
                x = (float)random.Next(1024) / 1024.0f - 0.5f;
            }
            if (float.IsNaN(y))
            {
                y = (float)random.Next(1024) / 1024.0f - 0.5f;
            }

            return q.Translate2D(x + 0.5f, y + 0.5f);

        }

      

        public static Quad Rotate(Quad q, float orient)
        {
            Quad q2 = new Quad();
            for (int i = 0; i < 4; i++)
            {
                q2.Corners[i]=Rotate(q.Corners[i],orient);
            }
            return q2;
        }

        public static Quad CreateQuad(float a, float bangle, float cangle, float bm, float cm, float orient)
        {
            Quad q = new Quad();
            
            q.Corners[0] = new PointF((float)Math.Cos(bangle) * a * bm-a/2, (float)Math.Sin(bangle) * a * bm);
            q.Corners[1] = new PointF(-a / 2, 0);
            q.Corners[2] = new PointF(a / 2, 0);
            q.Corners[3] = new PointF((float)Math.Cos(Math.PI-cangle) * a * cm + a / 2, (float)Math.Sin(Math.PI-cangle) * a * cm);
            if (orient != 0)
            {
                return Rotate(q, orient);
            }
            return q;
        }


        public static Quad CreateRandom(float a, float caMin, float caMax, float alphaMin, float alphaMax, Random r)
        {
            Quad q = new Quad();

            q.Inner1 = new PointF((float)r.Next(1000) / 1000.0f, (float)r.Next(1000) / 1000.0f);
            float rotate = (float)(r.NextDouble() * 2 * Math.PI);

            q.Inner2 = new PointF(a, 0).Rotate(rotate).Translate(q.Inner1);

            q.End1 = new PointF((float)(a * (caMin + r.NextDouble() * (caMax - caMin))), 0).Rotate(rotate + alphaMin + r.NextDouble() * (alphaMax - alphaMin)).Translate(q.Inner1);

            q.End2 = new PointF((float)(-a * (caMin + r.NextDouble() * (caMax - caMin))), 0).Rotate(rotate - alphaMin - r.NextDouble() * (alphaMax - alphaMin)).Translate(q.Inner2);

            q.FixWinding();
            return q;

        }

        //Tuple<PointF, PointF>[] GetLines()
        //{
        //    return new []{
        //        new Tuple<PointF,PointF>(Corners[0],Corners}
        //}
        [XmlIgnore]
        static readonly float Margin = 0.03f;
        [XmlIgnore]
        public PointF Center
        {
            get
            {
                return new PointF((Corners[0].X + Corners[1].X + Corners[2].X + Corners[3].X) * 0.25f, (Corners[0].Y + Corners[1].Y + Corners[2].Y + Corners[3].Y) * 0.25f);
            }
        }

        public bool IsValid()
        {
            for (int i = 0; i < 4; i++)
            {
                if (Corners[i].X < Margin || Corners[i].X > 1 - Margin || Corners[i].Y < Margin || Corners[i].Y > 1 - Margin)
                {
                    return false;
                }
            }
            float innerLengthSq = Geometry.DistanceSq(Corners[1], Corners[2]);
            float endSegmentDist = Geometry.LineSegmentsIntersect(Corners[0], Corners[1], Corners[2], Corners[3]);
            if (endSegmentDist * endSegmentDist * 36 < innerLengthSq)
            {
                return false;
            }
            float endDistSq = Geometry.DistanceSq(Corners[0], Corners[3]);
            if (endDistSq * 36 < innerLengthSq)
            {
                return false;
            }
            //float innerLength=(float)Math.Sqrt(innerLengthSq);
            //if (Geometry.DistanceLinePoint(Corners[1], Corners[2], Corners[0]) < innerLength/100)//Margin/4)
            //{
            //    return false;
            //}
            //if (Geometry.DistanceLinePoint(Corners[1], Corners[2], Corners[3]) < innerLength/100)//Margin/4)
            //{
            //    return false;
            //}


            return true;
        }

        public bool HasIntersectionWith(Quad q, float minDist)
        {

            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 3; j++)
                {
                    if (i < 3 && j < 3 && Geometry.LineSegmentsIntersect(Corners[i], Corners[i + 1], q.Corners[j], q.Corners[j + 1]) < minDist)
                    {
                        return true;
                    }
                    if (Geometry.DistanceSq(Corners[i], q.Corners[j]) < minDist * minDist)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

       
        public float Distance(Quad q)
        {
            return (Corners[0].Distance(q.Corners[0]) + Corners[1].Distance(q.Corners[1]) + Corners[2].Distance(q.Corners[2]) + Corners[3].Distance(q.Corners[3])) / 4;
        }

        public override string ToString()
        {
            return string.Format("\r\n{0} \t{1} \r\n{2} \t{3} \r\n{4} \t{5} \r\n{6} \t{7} \r\n", Corners[0].X, Corners[0].Y, Corners[1].X, Corners[1].Y, Corners[2].X, Corners[2].Y, Corners[3].X, Corners[3].Y);
        }
        public Quad Translate2D(float x, float y)
        {
            Quad q = new Quad();
           // var ps = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                q.Corners[i] = new PointF(Corners[i].X+x, Corners[i].Y+y);
            }
            return q;
        }

        static PointF Rotate(PointF p, float orient)
        {
            var c = (float)Math.Cos(orient);
            var s = (float)Math.Sin(orient);
            return new PointF(c * p.X - s * p.Y, s * p.X + c * p.Y);
        }

        public Quad Scale2D(float scale)
        {
            Quad q = new Quad();
            var ps = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                q.Corners[i] = new PointF(Corners[i].X*scale, Corners[i].Y*scale);
            }
            return q;
        }


        public Quad Transformabcxyzf(TransformArgs t)
        {
            Quad q = new Quad();
            var ps = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                //float den = M[2, 0] * Corners[i].X + M[2, 1] * Corners[i].Y + M[2, 2];
                //q.Corners[i] = new PointF(
                //    (M[0, 0] * Corners[i].X + M[0, 1] * Corners[i].Y + M[0, 2]) / den,
                //    (M[1, 0] * Corners[i].X + M[1, 1] * Corners[i].Y + M[1, 2]) / den);
                ps[i] = new float[] { Corners[i].X, Corners[i].Y };

            }
            //var pt=TransformSetArgs.ApplyTransformProject2D(t,ps);
            var pt = ImageObjectTransform.TransformObject2Image(ps, new float[] { t.a, t.b, t.c, t.x, t.y, t.z, t.fx });
            for (int i = 0; i < 4; i++)
            {
                q.Corners[i] = new PointF(pt[i][0], pt[i][1]);
            }
            return q;
        }

        public Quad AddNoise(float stdDev)
        {
            Quad q = new Quad();
            for (int i = 0; i < 4; i++)
            {
                q.Corners[i] = new PointF(Corners[i].X + MyRandom.GetStdDevRandom(stdDev), Corners[i].Y + MyRandom.GetStdDevRandom(stdDev));
            }
            return q;
        }
        public Quad AddQuantizer(float quant)
        {
            Quad q = new Quad();
            for (int i = 0; i < 4; i++)
            {
                q.Corners[i] = new PointF((float)Math.Round(Corners[i].X * quant / 2) / quant * 2, (float)Math.Round(Corners[i].Y * quant / 2) / quant * 2);
            }
            return q;
        }


        public void FixWinding()
        {
            var area2 = (Corners[1].X - Corners[0].X) * (Corners[1].Y + Corners[0].Y) +
                (Corners[2].X - Corners[1].X) * (Corners[2].Y + Corners[1].Y) +
                (Corners[3].X - Corners[2].X) * (Corners[3].Y + Corners[2].Y) +
                (Corners[0].X - Corners[3].X) * (Corners[0].Y + Corners[3].Y);
            if (area2 > 0)
            {
                var tmp = Corners[0];
                var tmp2 = Corners[1];
                Corners[0] = Corners[3];
                Corners[1] = Corners[2];
                Corners[3] = tmp;
                Corners[2] = tmp2;
            }
        }




    }
}
