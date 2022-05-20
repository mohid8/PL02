using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Mohid1
{
    public class Quads
    {
        private static int quadCount = 0;
        private int quadId = 0;
        private Point2f[] points;

        public Quads()
        {
            quadId = quadCount;
            quadCount++;
        }

        ~Quads()
        {
            quadCount--;
        }


        public static int GetQuads()
        {
            return quadCount;
        }

        public int GetQuadId()
        {
            return quadId;
        }

        public void StorePoints(List<int> pointlist, Point2f[] corners)
        {
            points = new Point2f[pointlist.Count];

            for(int i = 0; i < pointlist.Count; i++)
            {
                points[i] = corners[pointlist[i]];
            }
        }

        public void ShowQuad(Mat img)
        {
            var color = new Scalar(255, 255, 255);
            for(int i = 0; i < points.Length; i++)
            {
                Cv2.Circle(img, (Point)points[i], 2, color, -1);
            }
            
        }

        public void DrawLines(Mat img, Mat fill)
        {
            var color = new Scalar(255, 255, 255);
            for(int i = 0; i < points.Length; i++)
            {
                for(int j = i+1; j< points.Length; j++)
                {
                    var mid = (points[i]+points[j])*0.5;
                    var qtri = (points[i] + mid)*0.5;
                    var qtrj = (points[j] + mid) * 0.5;
                    var ethi = (points[i]+qtri)*0.5;
                    var ethj = (points[j] + qtrj) * 0.5;

                    if ((fill.At<Vec3b>((int)(mid.Y),(int)(mid.X)).Equals(fill.At<Vec3b>((int)points[i].Y, (int)points[i].X)))&&(
                        fill.At<Vec3b>((int)(qtri.Y), (int)(qtri.X)).Equals(fill.At<Vec3b>((int)points[i].Y, (int)points[i].X))&&(
                        fill.At<Vec3b>((int)(qtrj.Y), (int)(qtrj.X)).Equals(fill.At<Vec3b>((int)points[i].Y, (int)points[i].X))&&(
                        fill.At<Vec3b>((int)(ethi.Y), (int)(ethi.X)).Equals(fill.At<Vec3b>((int)points[i].Y, (int)points[i].X))&&(
                        fill.At<Vec3b>((int)(ethj.Y), (int)(ethj.X)).Equals(fill.At<Vec3b>((int)points[i].Y, (int)points[i].X)))))))
                    {
                        Cv2.Line(img, (Point)points[i], (Point)points[j], color);
                    }

                    
                    
                   /* else if (fill.At<Vec3b>((int)qtr.Y, (int)qtr.X).Equals(fill.At<Vec3b>((int)points[j].Y, (int)points[j].X)))
                    {
                        Cv2.Line(img, (Point)points[i], (Point)points[j], color);
                    }

                    else if (fill.At<Vec3b>((int)eth.Y, (int)eth.X).Equals(fill.At<Vec3b>((int)points[j].Y, (int)points[j].X)))
                    {
                        Cv2.Line(img, (Point)points[i], (Point)points[j], color);
                    }
                   */

                }
                
                
            }
            
        }



    }
}
