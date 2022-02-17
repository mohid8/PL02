using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class ImageObjectTransform
    {
        public static float[][] TransformImage2Object(float[][] imagePoints, float[] abcxyzf)
        {
            float a = abcxyzf[0];
            float b = abcxyzf[1];
            float c = abcxyzf[2];
            float x = abcxyzf[3];
            float y = abcxyzf[4];
            float z = abcxyzf[5];
            float f = abcxyzf[6];

            //var rotMi = TransformSetArgs.CreateRotationFullZYX(-a, -b, -c);
            var rotMi = TransformSetArgs.CreateRotationFullXYZ(a, b, c);

            var rot = new TransformSetArgs(rotMi);
            var cam = rot.ApplyTransform3D(new float[][] { new float[] { x, y, z } })[0];

            float[][] obj = new float[imagePoints.Length][];
            for (int i = 0; i < imagePoints.Length; i++)
            {
                var p = rot.ApplyTransform3D(new float[][] { new float[] { x + imagePoints[i][0], y + imagePoints[i][1], z + f } })[0];
                float t = -cam[2] / (p[2] - cam[2]);
                obj[i] = new float[] { (p[0] - cam[0]) * t + cam[0], (p[1] - cam[1]) * t + cam[1] };
            }
            return obj;
        }

        public static float[][] TransformObject2Image(float[][] objectPoints, float[] abcxyzf)
        {
            float a = abcxyzf[0];
            float b = abcxyzf[1];
            float c = abcxyzf[2];
            float x = abcxyzf[3];
            float y = abcxyzf[4];
            float z = abcxyzf[5];
            float f = abcxyzf[6];

            //var rotMi = TransformSetArgs.CreateRotationFullZYX(-a, -b, -c);
            var rotMi = TransformSetArgs.CreateRotationFullXYZ(a, b, c);

            var rot = new TransformSetArgs(rotMi);
            var cam = rot.ApplyTransform3D(new float[][] { new float[] { x, y, z } })[0];

            var m = new TransformSetArgs(TransformSetArgs.CreateRotationFullZYX(-a, -b, -c));

            float[][] im = new float[objectPoints.Length][];
            for (int i = 0; i < objectPoints.Length; i++)
            {
                var p = rot.ApplyTransform3D(new float[][] { new float[] { -objectPoints[i][0], -objectPoints[i][1], 0 } })[0];
                p[0] += x; p[1] += y; p[2] += z;

                im[i] = new float[] { p[0] / p[2] * f, p[1] / p[2] * f };
            }
            return im;
        }
    }
}
