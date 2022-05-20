using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Mohid1
{
    public class Quads2
    {
        private static int quadCount = 0;
        private int quadId = 0;
        public List<Point2f> pointList;

        public Quads2()
        {
            quadId = quadCount;
            quadCount++;
            pointList = new List<Point2f>();
        }

        ~Quads2()
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

        public void StorePoint(Point2f point)
        {
            pointList.Add(point);            
        }

        public void ShowQuad(Mat img)
        {
            var color = new Scalar(255, 255, 255);
            for(int i = 0; i < pointList.Count; i++)
            {
                Cv2.Circle(img, (Point)pointList[i], 2, color, -1);
            }
            
        }

        public void DrawLines(Mat img, Mat fill)
        {
            var color = new Scalar(255, 255, 255);
            for(int i = 0; i < pointList.Count; i++)
            {
                for(int j = i+1; j< pointList.Count; j++)
                {
                    var mid = (pointList[i]+pointList[j])*0.5;
                    var qtri = (pointList[i] + mid)*0.5;
                    var qtrj = (pointList[j] + mid) * 0.5;
                    var ethi = (pointList[i]+qtri)*0.5;
                    var ethj = (pointList[j] + qtrj) * 0.5;

                    if ((fill.At<Vec3b>((int)(mid.Y),(int)(mid.X)).Equals(fill.At<Vec3b>((int)pointList[i].Y, (int)pointList[i].X)))&&(
                        fill.At<Vec3b>((int)(qtri.Y), (int)(qtri.X)).Equals(fill.At<Vec3b>((int)pointList[i].Y, (int)pointList[i].X))&&(
                        fill.At<Vec3b>((int)(qtrj.Y), (int)(qtrj.X)).Equals(fill.At<Vec3b>((int)pointList[i].Y, (int)pointList[i].X))&&(
                        fill.At<Vec3b>((int)(ethi.Y), (int)(ethi.X)).Equals(fill.At<Vec3b>((int)pointList[i].Y, (int)pointList[i].X))&&(
                        fill.At<Vec3b>((int)(ethj.Y), (int)(ethj.X)).Equals(fill.At<Vec3b>((int)pointList[i].Y, (int)pointList[i].X)))))))
                    {
                        Cv2.Line(img, (Point)pointList[i], (Point)pointList[j], color);
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
