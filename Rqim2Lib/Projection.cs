using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class TransformArgs
    {
        public float a;
        public float b;
        public float c;
        //public float d;
        public float fx = 1;
        public float fy = 1;

        public float x;
        public float y;
        public float z;

        public float[] GetViewPosition()
        {
            float[] pos = new float[] { 0, 0, 1 };
            return TransformSetArgs.ApplyTransform3D(this, new[] { pos })[0];
        }
    }


    public class TransformSetArgs
    {
        public float[,] M;

        public float fx = 1;
        public float fy = 1;

        public TransformSetArgs()
        {
            M = new float[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, 1 } };
        }
        public TransformSetArgs(float[,] m)
        {
            M = new float[3, 3];
            int a = 0;
            int b = 0;
            for (int i = 0; i < 9; i++)
            {
                M[a, b] = m[a, b];
                b++;
                if (b == 3)
                {
                    a++; b = 0;
                }
            }
        }

        public TransformSetArgs(float[][] m)
        {
            M = new float[3, 3];
            int a = 0;
            int b = 0;
            for (int i = 0; i < 9; i++)
            {
                M[a, b] = m[a][b];
                b++;
                if (b == 3)
                {
                    a++; b = 0;
                }
            }
        }

        public void AddRotation2D(float a, float b, float c)
        {
            var rot = CreateRotation(a, b, c);
            var M2 = new float[3, 3];
            for (int r = 0; r < 3; r++)
            {
                for (int col = 0; col < 3; col++)
                {
                    M2[r, col] = rot[r][0] * M[0, col] + rot[r][1] * M[1, col];// +rot[r][2] * M[2, col];
                }
            }

            M = M2;
        }

        public void AddTranslation3D(float x, float y, float z)
        {
            M[0, 2] -= x;
            M[1, 2] -= y;
            M[2, 2] -= z;
        }

        public void AddTransform(TransformArgs t)
        {
            if (t.a != 0 || t.b != 0 || t.c != 0)
            {
                AddRotation2D(t.a, t.b, t.c);
            }
            AddTranslation3D(t.x, t.y, t.z);
        }

        public static float[][] ApplyTransformProject2D(TransformArgs t, float[][] p)
        {
            TransformSetArgs ts = new TransformSetArgs();
            ts.AddTransform(t);
            return ts.ApplyTransformProject2D(t.fx, t.fy, p);
        }

        public static float[][] ApplyTransformProject2D(TransformSetArgs ts, float[][] p)
        {
            return ts.ApplyTransformProject2D(ts.fx, ts.fy, p);
        }

        public static float[][] ApplyTransform3D(TransformArgs t, float[][] p)
        {
            TransformSetArgs ts = new TransformSetArgs();
            ts.AddTransform(t);
            return ts.ApplyTransform3D(p);
        }

        public float[][] ApplyTransformProject2D(float fx, float fy, float[][] p)
        {
            var ptt = new float[p.Length][];
            for (int i = 0; i < p.Length; i++)
            {
                ptt[i] = new float[2];
                float pz = p[i][0] * M[2, 0] + p[i][1] * M[2, 1] + M[2, 2];
                ptt[i][0] = (p[i][0] * M[0, 0] + p[i][1] * M[0, 1] + M[0, 2]) * fx / (pz);
                ptt[i][1] = (p[i][0] * M[1, 0] + p[i][1] * M[1, 1] + M[1, 2]) * fy / (pz);
            }
            return ptt;
        }

        public float[][] ApplyTransform3D(float[][] p)
        {
            var ptt = new float[p.Length][];
            for (int i = 0; i < p.Length; i++)
            {
                ptt[i] = new float[3];
                ptt[i][2] = p[i][0] * M[2, 0] + p[i][1] * M[2, 1] + p[i][2] * M[2, 2];
                ptt[i][0] = (p[i][0] * M[0, 0] + p[i][1] * M[0, 1] + p[i][2] * M[0, 2]);
                ptt[i][1] = (p[i][0] * M[1, 0] + p[i][1] * M[1, 1] + p[i][2] * M[1, 2]);
            }
            return ptt;
        }

        public static float[][] CreateRotation(float a, float b, float c)
        {
            a *= (float)Math.PI / 180;
            b *= (float)Math.PI / 180;
            c *= (float)Math.PI / 180;
            float cx = (float)Math.Cos(a), sx = (float)Math.Sin(a);
            float cy = (float)Math.Cos(b), sy = (float)Math.Sin(b);
            float cz = (float)Math.Cos(c), sz = (float)Math.Sin(c);

            var roto = new float[][] { // last column not needed, our vector has z=0
               new []{ cz * cy, cz * sy * sx - sz * cx },
               new []{ sz * cy, sz * sy * sx + cz * cx },
               new []{ -sy, cy * sx }
           };
            return roto;
        }

        //http://www.songho.ca/opengl/gl_anglestoaxes.html
        public static float[][] CreateRotationFullXYZ(float a, float b, float c)
        {
            a *= (float)Math.PI / 180;
            b *= (float)Math.PI / 180;
            c *= (float)Math.PI / 180;
            float ca = (float)Math.Cos(a), sa = (float)Math.Sin(a);
            float cb = (float)Math.Cos(b), sb = (float)Math.Sin(b);
            float cc = (float)Math.Cos(c), sc = (float)Math.Sin(c);

            var roto = new float[][] { 
               new []{ cc * cb, -cb*sc, sb},
               new []{ sa*sb*cc+ca*sc, -sa*sb*sc+ca*cc, -sa*cb},
               new []{ -ca*sb*cc+sa*sc, ca*sb*sc+sa*cc, ca*cb }
           };
            return roto;
        }

        public static float[][] CreateRotationFullZYX(float a, float b, float c)
        {
            a *= (float)Math.PI / 180;
            b *= (float)Math.PI / 180;
            c *= (float)Math.PI / 180;
            float ca = (float)Math.Cos(a), sa = (float)Math.Sin(a);
            float cb = (float)Math.Cos(b), sb = (float)Math.Sin(b);
            float cc = (float)Math.Cos(c), sc = (float)Math.Sin(c);

            var roto = new float[][] { 
               new []{ cc * cb, -sc*ca+cc*sb*sa, sc*sa+cc*sb*ca},
               new []{ sc*cb, cc*ca+sc*sb*sa, -cc*sa+sc*sb*ca},
               new []{ -sb,cb*sa,cb*ca }
           };
            return roto;
        }


        public void AddRotation3D(float a, float b, float c)
        {
            var rot = CreateRotationFullXYZ(a, b, c);
            var M2 = new float[3, 3];
            for (int r = 0; r < 3; r++)
            {
                for (int col = 0; col < 3; col++)
                {
                    M2[r, col] = rot[r][0] * M[0, col] + rot[r][1] * M[1, col] + rot[r][2] * M[2, col];
                }
            }

            M = M2;
        }


        public static float[][] RotatePoints3D(float a, float b, float c, float[][] p)
        {
            TransformSetArgs ts = new TransformSetArgs();
            ts.AddRotation3D(a, b, c);
            return ts.ApplyTransform3D(p);

        }

        static float Norm3(float[] f)
        {
            return (float)Math.Sqrt(f[0] * f[0] + f[1] * f[1] + f[2] * f[2]);
        }

        static void NormalizeArray3(float[] f)
        {
            float lenInv = 1.0f / Norm3(f);
            f[0] *= lenInv;
            f[1] *= lenInv;
            f[2] *= lenInv;
        }

        static float[] Cross3(float[] a, float[] b)
        {
            return new float[] {
            a[1]*b[2]-a[2]*b[1],
            -a[0]*b[2]+a[2]*b[0],
            a[0]*b[1]-a[1]*b[0]
           };
        }

        public static readonly float Pi_180 = (float)Math.PI / 180;
        public static readonly float _180_Pi = 180 / (float)Math.PI;

        /// <summary>
        /// http://nghiaho.com/?page_id=846
        /// </summary>
        /// <param name="M"></param>
        /// <returns></returns>
        public static float[] DecomposeToRotationTranslation(float[,] M)
        {
            //OpenCvSharp.Cv.Normalize(h1, r1);

            float[] r1 = new float[] { M[0, 0], M[1, 0], M[2, 0] };
            var n1 = Norm3(r1);
            NormalizeArray3(r1);

            float[] r2 = new float[] { M[0, 1], M[1, 1], M[2, 1] };
            var n2 = Norm3(r2);
            NormalizeArray3(r2);

            var r3 = Cross3(r1, r2);
            NormalizeArray3(r3);

            var tnormInv = 1.0f / ((n1 + n2) * 0.5f);

            var abcxyz = new float[6];

            abcxyz[3] = M[0, 2] * tnormInv;
            abcxyz[4] = M[1, 2] * tnormInv;
            abcxyz[5] = M[2, 2] * tnormInv;

            abcxyz[0] = (float)Math.Atan2(r2[2], r3[2]) * _180_Pi;
            //abcxyz[1] = (float)Math.Atan2(-r1[2], Math.Sqrt(r2[2] * r2[2] + r3[2] * r3[2])) * _180_Pi;
            abcxyz[1] = -(float)Math.Asin(r1[2]) * _180_Pi;
            abcxyz[2] = (float)Math.Atan2(r1[1], r1[0]) * _180_Pi;

            return abcxyz;
            //OpenCvSharp.Cv.Normalize(h2, r2);
            //r1.CrossProduct(r2, r3);
            //OpenCvSharp.Cv.Normalize(r3, r3);
            //var norm1 = (float)h1.Norm();
            //var norm2 = (float)h2.Norm();
            //var tnorm = (norm1 + norm2) / 2.0f;
            //var T = h3 / tnorm;

            //OpenCvSharp.CvMat rot = new OpenCvSharp.CvMat(1, 3, OpenCvSharp.MatrixType.F32C1);

            //OpenCvSharp.Cv.Rodrigues2(R, rot);

        }

        public static float[] DecomposeRotationToABC(float[,] r)
        {
            var abc = new float[3];

            abc[0] = (float)Math.Atan2(r[1, 2], r[2, 2]) * _180_Pi;
            //abcxyz[1] = (float)Math.Atan2(-r1[2], Math.Sqrt(r2[2] * r2[2] + r3[2] * r3[2])) * _180_Pi;
            abc[1] = -(float)Math.Asin(r[0, 2]) * _180_Pi;
            abc[2] = (float)Math.Atan2(r[0, 1], r[0, 0]) * _180_Pi;
            return abc;
        }

    }
}
