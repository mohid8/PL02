using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RqimOpenCV
{
    public class CvDetector
    {

        public static List<List<System.Drawing.PointF>> LoadCorners(string filename)
        {          
                   int Margin=5;
            var gray=OpenCvSharp.Cv2.ImRead(filename, OpenCvSharp.ImreadModes.GrayScale);  
            int Width=gray.Width;
            int Height=gray.Height;
           // var blur = gray.GaussianBlur(new OpenCvSharp.Size(5,5), 1.5, 1.5, OpenCvSharp.BorderTypes.Constant);

            var binary=gray.Threshold(0, 255, OpenCvSharp.ThresholdTypes.BinaryInv | OpenCvSharp.ThresholdTypes.Otsu);

            var element = OpenCvSharp.Cv2.GetStructuringElement(OpenCvSharp.MorphShapes.Rect, new OpenCvSharp.Size(3, 3)); 
            var morphed= binary.MorphologyEx(OpenCvSharp.MorphTypes.Close, element);

            OpenCvSharp.Window.ShowImages(new[] { morphed });
            //var moments=OpenCvSharp.Cv2.Moments(binary, true);
            //var blobdetector=OpenCvSharp.SimpleBlobDetector.Create(new OpenCvSharp.SimpleBlobDetector.Params(){  )
            //f.c
            //OpenCvSharp.Cv2.conn

          
             


            OpenCvSharp.Point[][] contours;
            OpenCvSharp.HierarchyIndex[] hierarchy;

            OpenCvSharp.Cv2.FindContours(binary, out contours, out hierarchy, OpenCvSharp.RetrievalModes.External, OpenCvSharp.ContourApproximationModes.ApproxNone);

            var selectedBlobsImage = new OpenCvSharp.Mat(new OpenCvSharp.Size(gray.Width, gray.Height), OpenCvSharp.MatType.CV_8U, OpenCvSharp.Scalar.Black);


            //Dictionary<uint, int> ColorIndexDict = new Dictionary<uint, int>();
            //Dictionary<byte, byte> ColorIndexDict = new Dictionary<byte, byte>();
            byte blobIndex=1;

            for (int i=0;i<contours.Length;i++){
                var contour=contours[i];
                var area = OpenCvSharp.Cv2.ContourArea(contour);
                var length = contour.Length;

                var ratio = area/length;
                if (area > 50 && ratio < 10 && blobIndex<255)
                {
                    //selectedBlobsImage.DrawContours(contours, i, OpenCvSharp.Scalar.White, -1);
                    //var color = ImageTools.GetRandomColor("Blobs", i,255,1);
                    blobIndex++;
                    byte color=(byte)(254-blobIndex);
                    //ColorIndexDict.Add(color, i);
                    //selectedBlobsImage.DrawContours(contours, i, (OpenCvSharp.Scalar)color, -1);
                    selectedBlobsImage.DrawContours(contours, i, OpenCvSharp.Scalar.FromRgb(color,color,color), -1);
                }
            }
            OpenCvSharp.Window.ShowImages(new[] { selectedBlobsImage });

            Rqim2Lib.Pixel[,] pixels = new Rqim2Lib.Pixel[morphed.Width, morphed.Height];

            for (int i = 0; i < morphed.Width; i++)
            {
                for (int j = 0; j < morphed.Height; j++)
                {
                    var color=selectedBlobsImage.At<byte>(j, i);
                    if (color > 0)
                    {
                        
                        var p = new Rqim2Lib.Pixel() { u = i, v = j, id=color };
                        pixels[i, j] = p;
                    }
                }
            }

            Rqim2Lib.Thinner thinner = new Rqim2Lib.Thinner(pixels, 0, 0);
            List<Rqim2Lib.Pixel> thinPixels = new List<Rqim2Lib.Pixel>();
            thinner.Thin_SI_Q_Isthmus(Rqim2Lib.Thinner.Q_NE_SW_NW_SE, thinPixels);
            var skeletonImage = new OpenCvSharp.Mat(new OpenCvSharp.Size(morphed.Width, morphed.Height), OpenCvSharp.MatType.CV_8U, OpenCvSharp.Scalar.Black);

            Dictionary<byte, List<Rqim2Lib.Pixel>> ThinQuads = new Dictionary<byte, List<Rqim2Lib.Pixel>>();
            for (int i = 0; i < thinPixels.Count; i++)
            {
                var key=(byte)thinPixels[i].id;
                if (!ThinQuads.ContainsKey(key)) { ThinQuads.Add(key, new List<Rqim2Lib.Pixel>()); }
                ThinQuads[key].Add(thinPixels[i]);
                skeletonImage.Set<byte>(thinPixels[i].v, thinPixels[i].u, key);
            }
            OpenCvSharp.Window.ShowImages(new[] { skeletonImage });
             
           // OpenCvSharp.Window.ShowImages(new[] { binary });

            List<List<System.Drawing.PointF>> cornerlist = new List<List<System.Drawing.PointF>>();


            foreach (var key in ThinQuads.Keys)
            {
                var quadImage = new OpenCvSharp.Mat(new OpenCvSharp.Size(morphed.Width, morphed.Height), OpenCvSharp.MatType.CV_8U, OpenCvSharp.Scalar.Black);
                foreach (var p in ThinQuads[key])
                {
                    quadImage.Set<byte>(p.v, p.u, 127);
                }



                QuadFitter qf = new QuadFitter(ThinQuads[key]);
                Line[] bestLines;
                qf.Fit3Lines(out bestLines, 500);
                System.Drawing.PointF a,b;
                if (bestLines!=null){
                    Line center, side1, side2;
                    var bestlinesOrderedByLen = bestLines.OrderBy(x => x.Pixels.Count).ToArray();
                    if (bestlinesOrderedByLen[0].IsNeighborWith(bestlinesOrderedByLen[1])==null){
                        center=bestlinesOrderedByLen[2];
                        side1=bestlinesOrderedByLen[0];
                        side2=bestlinesOrderedByLen[1];
                    }else if (bestlinesOrderedByLen[0].IsNeighborWith(bestlinesOrderedByLen[2])==null){
                        center=bestlinesOrderedByLen[1];
                        side1=bestlinesOrderedByLen[0];
                        side2=bestlinesOrderedByLen[2];
                    }else{
                        center=bestlinesOrderedByLen[0];
                        side1=bestlinesOrderedByLen[1];
                        side2=bestlinesOrderedByLen[2];
                    }

                    var c2 = center.InterSection(side1);
                    var c3 = center.InterSection(side2);

                    var e1s = side1.GetEndPoints();
                    var e2s = side2.GetEndPoints();
                    float tmp;
                    var c1p = e1s.MaxWhere(x => x.Distance2(new Rqim2Lib.Pixel() { u = (int)Math.Round(c2.X), v = (int)Math.Round(c2.Y) }), out tmp);
                    var c4p = e2s.MaxWhere(x => x.Distance2(new Rqim2Lib.Pixel() { u = (int)Math.Round(c3.X), v = (int)Math.Round(c3.Y) }), out tmp);

                    var c1=new System.Drawing.PointF(c1p.u, c1p.v);
                    var c4=new System.Drawing.PointF(c4p.u, c4p.v);

                    for (int i=0;i<3;i++){
                        //bestLines[i].Get2Points(100,out a, out b);
                        //quadImage.Line((int)a.X, (int)a.Y, (int)b.X, (int)b.Y, new OpenCvSharp.Scalar(i*50+50));
                        foreach (var p in bestlinesOrderedByLen[i].Pixels)
                        {
                            quadImage.Set<byte>(p.v, p.u, (byte)(i*50+100));
                        }
                    }

                    if (Utils.GetAngle(c1, c2, c3) <170 && Utils.GetAngle(c4, c3, c2) <170)
                    {
                        //RotateLeft90(ref c1, gray.Width, gray.Height);
                        //RotateLeft90(ref c2, gray.Width, gray.Height);
                        //RotateLeft90(ref c3, gray.Width, gray.Height);
                        //RotateLeft90(ref c4, gray.Width, gray.Height);
                        List<System.Drawing.PointF> corners = new List<System.Drawing.PointF>() { c1, c2, c3, c4 };

                        //var c0 = new System.Drawing.PointF(0.25f * (c1.X + c2.X + c3.X + c4.X), 0.25f * (c1.Y + c2.Y + c3.Y + c4.Y));
                        //var orientations = corners.Select(x => (float)Math.Atan2(x.Y - c0.Y, x.X - c0.X) + Math.PI * 2).ToArray();

                        bool exit = false;

                        foreach (var c in corners)
                        {
                            if (c.X < Margin || c.X > Width - Margin || c.Y < Margin || c.Y > Height - Margin)
                            {
                                exit = true;
                            }
                        }
                        if (exit)
                        {
                            continue;
                        }
                        cornerlist.Add(corners);

                        quadImage.DrawMarker((int)c1.X, (int)c1.Y, OpenCvSharp.Scalar.White);
                        quadImage.DrawMarker((int)c2.X, (int)c2.Y, OpenCvSharp.Scalar.White);
                        quadImage.DrawMarker((int)c3.X, (int)c3.Y, OpenCvSharp.Scalar.White);
                        quadImage.DrawMarker((int)c4.X, (int)c4.Y, OpenCvSharp.Scalar.White);

                    }
                    OpenCvSharp.Window.ShowImages(new[] { quadImage });
                 }    

               
                
            }

            foreach (var cs in cornerlist)
            {
                for (int ci = 0; ci < 4; ci++)
                {
                     skeletonImage.DrawMarker((int)cs[ci].X, (int)cs[ci].Y, OpenCvSharp.Scalar.White);
                }
            }


            OpenCvSharp.Window.ShowImages(new[] { skeletonImage });
             
            
            return cornerlist; 
             }

        //static void RotateLeft90(ref System.Drawing.PointF p, int width, int height)
        //{
        //    var tmp = p.X;
        //    p.X = p.Y;
        //    p.Y = height - tmp;
        //}
    }
}
