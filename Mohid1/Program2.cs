using Rqim2Lib;
using RqimOpenCV;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mohid1
{
    class Program2
    {
        static void Main(string[] args)
        {
            Mat src = new Mat("C:/Users/Mohid/source/repos/rqim-mohid/images/10_cam_11.png", ImreadModes.Color);
            Mat gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);

            Mat binary = gray.Threshold(0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);
            Mat binary2 = binary.EmptyClone();

            Size size;
            size.Width = 3; size.Height = 3;
            var element = Cv2.GetStructuringElement(MorphShapes.Rect, size);
            Cv2.MorphologyEx(binary, binary2, MorphTypes.Dilate, element, null, 1);

            Mat Corner = src.Clone();
            Mat Fill = binary2.CvtColor(ColorConversionCodes.GRAY2BGR);
            Mat QuadCorners = src.EmptyClone();
            
            var Rect = new Rect();

            

            Point2f[] corners;

             
            
            // maxCorners – The maximum number of corners to return. If there are more corners
            // than that will be found, the strongest of them will be returned
            int maxCorners = 50;

            // qualityLevel – Characterizes the minimal accepted quality of image corners;
            // the value of the parameter is multiplied by the by the best corner quality
            // measure (which is the min eigenvalue, see cornerMinEigenVal() ,
            // or the Harris function response, see cornerHarris() ).
            // The corners, which quality measure is less than the product, will be rejected.
            // For example, if the best corner has the quality measure = 1500,
            // and the qualityLevel=0.01 , then all the corners which quality measure is
            // less than 15 will be rejected.
            double qualityLevel = 0.01;

            // minDistance – The minimum possible Euclidean distance between the returned corners
            double minDistance = 15;

            // mask – The optional region of interest. If the image is not empty (then it
            // needs to have the type CV_8UC1 and the same size as image ), it will specify
            // the region in which the corners are detected
            Mat mask = binary;
            // blockSize – Size of the averaging block for computing derivative covariation
            // matrix over each pixel neighborhood, see cornerEigenValsAndVecs()
            int blockSize = 6;

            // useHarrisDetector – Indicates, whether to use operator or cornerMinEigenVal()
            bool useHarrisDetector = false;

            // k – Free parameter of Harris detector
            double k = 0.04;

            corners = Cv2.GoodFeaturesToTrack(gray, maxCorners, qualityLevel, minDistance, mask, blockSize, useHarrisDetector, k);
            var color = new Scalar(0,0,255);

            for (int i = 0; i < corners.Length; i++)
            {
                Cv2.Circle(Corner,(Point)corners[i], 5, color);
            }

            Random rnd = new Random();

            int B = rnd.Next(256);
            int G = rnd.Next(256);
            int R = rnd.Next(256);

            var fillColor = new Scalar(B, G, R);
            var loDif = new Scalar(0,0,0);
            var hiDif = new Scalar(0,0,0);

            for (int i = 0; i < corners.Length; i++)
            {
                //Cv2.FloodFill(Fill, (Point)corners[i], fillColor);
                Cv2.FloodFill(Fill, (Point)corners[i], fillColor, out Rect, loDif , hiDif, FloodFillFlags.Link8);

                B = rnd.Next(256);
                G = rnd.Next(256);
                R = rnd.Next(256);

                fillColor = new Scalar(B, G, R);

            }


            int[][] cornerColors = new int[corners.Length][];

            for (int i = 0; i < corners.Length; i++)
            {

                var y = (int)corners[i].Y;
                var x = (int)corners[i].X;

                var pixelColorB = Fill.At<Vec3b>(y, x)[0];
                var pixelColorG = Fill.At<Vec3b>(y, x)[1];
                var pixelColorR = Fill.At<Vec3b>(y, x)[2];

                cornerColors[i] = new int[] { pixelColorB, pixelColorG, pixelColorR };


            }

            var cornerColorsList = cornerColors.ToList();

            List<List<int>> connectedPoints = new List<List<int>>();
            for (int i = 0; i < maxCorners; i++)
            {
                connectedPoints.Add(new List<int>());
            }
            

            for (int i = 0; i < cornerColorsList.Count; i++)
            {
                if (cornerColorsList[i] != null)
                {
                    connectedPoints[i].Add(i);
                    for (int j = i + 1; j < cornerColorsList.Count; j++)
                    {
                        if (cornerColorsList[j] != null)
                        {
                            if (cornerColorsList[i].SequenceEqual(cornerColorsList[j]))
                            {
                                connectedPoints[i].Add(j);
                                cornerColorsList[j] = null;
                            }
                        }
                    }

                }
                else
                {
                    connectedPoints[i] = null;
                }
                
            }
                     

            connectedPoints.RemoveAll(p => p == null);
            for (int i = 0; i < connectedPoints.Count; i++)
            {
                if (connectedPoints[i].Count == 0)
                {
                    connectedPoints[i] = null;
                }
            }
            connectedPoints.RemoveAll(p => p == null);

            List<Quads> quadList = new List<Quads>();
            for(int i = 0; i < connectedPoints.Count; i++)
            {
                quadList.Add(new Quads());
            }

            Cv2.ImShow("Corners", Corner);
            Cv2.ImShow("Fill", Fill);
            Cv2.ImShow("CornerMask", binary);
            Cv2.ImShow("Morph", binary2);
            

            for (int i = 0; i < connectedPoints.Count; i++)
            {
                if(connectedPoints[i].Count == 4)
                {
                    quadList[i].StorePoints(connectedPoints[i], corners);
                    quadList[i].ShowQuad(QuadCorners);
                    quadList[i].DrawLines(QuadCorners,Fill);
                    Cv2.ImShow("Quad_Corners", QuadCorners);
                    Cv2.WaitKey();
                    QuadCorners = src.EmptyClone();
                }
                          
                                                               
            }

                               

            {
                Cv2.WaitKey();
            }
        }
    }
}
