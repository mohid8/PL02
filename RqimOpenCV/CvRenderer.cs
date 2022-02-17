using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RqimOpenCV
{
        public class CvRenderer
        {
               
            public static void DrawLinesToFile(string filename, Tuple<PointF,PointF,float>[] lines, int width, int height, float thicknessMultiplier = 0.010f){
                 var image=new OpenCvSharp.Mat(new OpenCvSharp.Size(width,height),OpenCvSharp.MatType.CV_8UC1,OpenCvSharp.Scalar.White);
                 //StringBuilder sb = new StringBuilder();
                 foreach (var l in lines)
	            {
                     var x1=(int)Math.Round((l.Item1.X + 0.5f) * width); 
                     var y1=(int)Math.Round((l.Item1.Y + 0.5f) * height);
                     var x2=(int)Math.Round((l.Item2.X + 0.5f) * width);
                     var y2 = (int)Math.Round((l.Item2.Y + 0.5f) * height);
                    // sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", x1, y1, x2, y2));
		             image.Line( 
                            new OpenCvSharp.Point(x1, y1),
                            new OpenCvSharp.Point(x2, y2),
                            OpenCvSharp.Scalar.Black, 
                            Math.Min(255, 1 + (int)Math.Round(thicknessMultiplier * width * l.Item3)),
                            OpenCvSharp.LineTypes.AntiAlias   
                            );    
	             }
                image.SaveImage(filename);
                //image.Release();
                //image.Dispose();
            }


            //public static void RenderToFile(string filename, PointF[][] rqim, int width, int height, float thicknessMultiplier = 0.010f)
            //{
            //    var image=new OpenCvSharp.Mat(new OpenCvSharp.Size(width,height),OpenCvSharp.MatType.CV_8UC1,OpenCvSharp.Scalar.White);
            //    foreach (var q in rqim)
            //    {
            //         for (int i=0;i<q.Length-1;i++)
            //         {
            //            image.Line( 
            //                new OpenCvSharp.Point((int)Math.Round((q[i].X + 0.5f) * width), (int)Math.Round((q[i].Y + 0.5f) * height)),
            //                new OpenCvSharp.Point((int)Math.Round((q[i+1].X + 0.5f) * width), (int)Math.Round((q[i+1].Y + 0.5f) * height)),
            //                OpenCvSharp.Scalar.Black, 
            //                Math.Min(255, 1 + (int)Math.Round(thicknessMultiplier * width * quad.InnerLength)),
            //                OpenCvSharp.LineTypes.AntiAlias
            //                );
            //         }
            //    }


                
            //    OpenCvSharp.IplImage image = new OpenCvSharp.IplImage(width, height, OpenCvSharp.BitDepth.U8, 3);
            //    image.Set(new OpenCvSharp.CvScalar(255, 255, 255));
            //    //foreach (var quad in rqim)
            //    for (int qi = 0; qi < rqim.QuadCount; qi++)
            //    {
            //        var quad = rqim[qi];
            //        for (int i = 0; i < 3; i++)
            //        {
            //            image.DrawLine(
            //                new OpenCvSharp.CvPoint((int)Math.Round((quad.Corners[i].X + 0.5f) * width), (int)Math.Round((quad.Corners[i].Y + 0.5f) * height)),
            //                new OpenCvSharp.CvPoint((int)Math.Round((quad.Corners[i + 1].X + 0.5f) * width), (int)Math.Round((quad.Corners[i + 1].Y + 0.5f) * height)),
            //                OpenCvSharp.CvColor.Black, Math.Min(255, 1 + (int)Math.Round(thicknessMultiplier * width * quad.InnerLength)),
            //                OpenCvSharp.LineType.AntiAlias
            //                );
            //        }
            //    }


            //    return image;
            //}



            //public static OpenCvSharp.IplImage RenderWithWarp(Rqim rqim, int width, int height, TransformArgs t, float thicknessMultiplier = 0.010f)
            //{
            //    var f = t.z;
            //    var src = Render(rqim, width, height, thicknessMultiplier);
            //    OpenCvSharp.IplImage dst = new OpenCvSharp.IplImage(width, height, OpenCvSharp.BitDepth.U8, 3);
            //    dst.Set(new OpenCvSharp.CvScalar(255, 255, 255));

            //    //a = a - (float)(Math.PI / 2);
            //    //b = b - (float)(Math.PI / 2);
            //    //c = c - (float)(Math.PI / 2);
            //    var a = Geometry.ToRad(t.a);
            //    var b = Geometry.ToRad(t.b);
            //    var c = Geometry.ToRad(t.c);
            //    var d = t.z;
            //    OpenCvSharp.CvMat Projection = new OpenCvSharp.CvMat(4, 3, OpenCvSharp.MatrixType.F32C1, new float[]{
            //        1,0,-width/2, 
            //        0, 1, -height/2, 
            //        0,0,0, 
            //        0,0,1});

            //    OpenCvSharp.CvMat abc = new OpenCvSharp.CvMat(3, 1, OpenCvSharp.MatrixType.F32C1);
            //    abc[0] = a; abc[1] = b; abc[2] = c;
            //    //OpenCvSharp.CvMat m = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1);
            //    OpenCvSharp.CvMat m = new OpenCvSharp.CvMat(4, 4, OpenCvSharp.MatrixType.F32C1);

            //    OpenCvSharp.CvMat Rx = new OpenCvSharp.CvMat(4, 4, OpenCvSharp.MatrixType.F32C1, new float[]{
            //    1,0,0,0,
            //    0,(float)Math.Cos(a), -(float)Math.Sin(a), 0,
            //    0, (float)Math.Sin(a), (float)Math.Cos(a), 0,
            //    0,0,0,1
            //    });
            //    OpenCvSharp.CvMat Ry = new OpenCvSharp.CvMat(4, 4, OpenCvSharp.MatrixType.F32C1, new float[]{
            //    (float)Math.Cos(b), 0, -(float)Math.Sin(b), 0,
            //    0, 1, 0,0,
            //    (float)Math.Sin(b), 0, (float)Math.Cos(b), 0,
            //    0,0,0,1
            //    });

            //    OpenCvSharp.CvMat Rz = new OpenCvSharp.CvMat(4, 4, OpenCvSharp.MatrixType.F32C1, new float[]{
            //    (float)Math.Cos(c), -(float)Math.Sin(c), 0,0,
            //    (float)Math.Sin(c), (float)Math.Cos(c), 0,0,
            //    0,0,1,0,
            //    0,0,0,1
            //    });
            //    //var r = OpenCvSharp.Cv.Rodrigues2(abc, m);

            //    m = Rx * Ry * Rz;

            //    // Composed rotation matrix with (RX, RY, RZ)


            //    OpenCvSharp.CvMat Rotation = new OpenCvSharp.CvMat(4, 4, OpenCvSharp.MatrixType.F32C1, (new float[] { 
            //        (float)m[0,0], (float)m[0,1], (float)m[0,2], 0, 
            //        (float)m[1,0], (float)m[1,1], (float)m[1,2], 0,  
            //        (float)m[2,0], (float)m[2,1], (float)m[2,2], 0,
            //        0,0,0,1
            //        }));


            //    OpenCvSharp.CvMat Translation = new OpenCvSharp.CvMat(4, 4, OpenCvSharp.MatrixType.F32C1, new float[]{
            //        1,0,0,0,
            //        0,1,0,0,
            //        0,0,1,d,
            //        0,0,0,1 });

            //    OpenCvSharp.CvMat Intrinsic = new OpenCvSharp.CvMat(3, 4, OpenCvSharp.MatrixType.F32C1, new float[]{
            //        f,0,width/2,0,
            //        0,f,height/2,0,
            //        0,0,0,1 });

            //    var T = Intrinsic * (Translation * (Rotation * Projection));

            //    src.WarpPerspective(dst, T, OpenCvSharp.Interpolation.Lanczos4, new OpenCvSharp.CvScalar(255, 255, 255));//|OpenCvSharp.Interpolation.InverseMap
            //    //OpenCvSharp.Cv.ShowImage("Dst",dst);
            //    return dst;


            //}


            //public static OpenCvSharp.IplImage RenderWithWarp2(Rqim rqim, int width, int height, TransformArgs t, float thicknessMultiplier = 0.010f)
            //{
            //    int supersample = 3;

            //    var src = Render(rqim, width * supersample, height * supersample, thicknessMultiplier);
            //    float w = width * supersample;
            //    float h = height * supersample;
            //    //f = d;


            //    //float cx = (float)Math.Cos(a), sx = (float)Math.Sin(a);
            //    //float cy = (float)Math.Cos(b), sy = (float)Math.Sin(b);
            //    //float cz = (float)Math.Cos(c), sz = (float)Math.Sin(c);



            //    //var roto = new float[][] { // last column not needed, our vector has z=0
            //    //    new []{ cz * cy, cz * sy * sx - sz * cx },
            //    //    new []{ sz * cy, sz * sy * sx + cz * cx },
            //    //    new []{ -sy, cy * sx }
            //    //};
            //    //var pt = new float[][] { new float[] { -w / 2, -h / 2}, new float[] { w / 2, -h / 2 }, new float[] { w / 2, h / 2 }, new float[] { -w / 2, h / 2 } };//, new float[]{0,0} 
            //    //var ptt = new float[pt.Length][];
            //    //for (int i = 0; i < pt.Length; i++)
            //    //{
            //    //    ptt[i] = new float[2];
            //    //    float pz = pt[i][0] * roto[2][0] + pt[i][1] * roto[2][1];
            //    //    ptt[i][0] = w / 2 + (pt[i][0] * roto[0][0] + pt[i][1] * roto[0][1]) * f * h / (f * h + pz);
            //    //    ptt[i][1] = h / 2 + (pt[i][0] * roto[1][0] + pt[i][1] * roto[1][1]) * f * h / (f * h + pz);
            //    //}

            //    var pt = new float[][] { new float[] { 0, 0 }, new float[] { 1, 0 }, new float[] { 1, 1 }, new float[] { 0, 1 } };
            //    var ptt = TransformSetArgs.ApplyTransformProject2D(t, pt);
            //    //Perspective.PerspectiveTransformPoint2D(width * supersample, height * supersample, t, pt);
            //    //var ptt = new float[4][];
            //    //for (int i = 0; i < 4; i++)
            //    //{
            //    //    ptt[i] =Perspective.PerspectiveTransformPoint(width*supersample, height*supersample, a, b, c, d, pt[i][0], pt[i][1]);
            //    //}

            //    float w2 = supersample * width / t.z;
            //    float h2 = supersample * height / t.z;

            //    //var in_pt = new OpenCvSharp.CvPoint2D32f[]{ 
            //    //    new OpenCvSharp.CvPoint2D32f(w/2-w2/2,h/2-h2/2),
            //    //    new OpenCvSharp.CvPoint2D32f(w/2+w2/2, h/2-h2/2),
            //    //    new OpenCvSharp.CvPoint2D32f(w/2+w2/2, h/2+h2/2),
            //    //    new OpenCvSharp.CvPoint2D32f(w/2-w2/2, h/2+h2/2)
            //    //};
            //    var in_pt = new OpenCvSharp.CvPoint2D32f[]{ 
            //        new OpenCvSharp.CvPoint2D32f(0,0),
            //        new OpenCvSharp.CvPoint2D32f(w, 0),
            //        new OpenCvSharp.CvPoint2D32f(w, h),
            //        new OpenCvSharp.CvPoint2D32f(0, h)
            //    };

            //    //OpenCvSharp.CvMat in_pt = new OpenCvSharp.CvMat(4, 2, OpenCvSharp.MatrixType.F32C1, new float[]{
            //    //    0, 0, 
            //    //    width, 0, 
            //    //    width, height, 
            //    //    0, height});

            //    var out_pt = new OpenCvSharp.CvPoint2D32f[]{ 
            //        new OpenCvSharp.CvPoint2D32f(ptt[0][0], ptt[0][1]),
            //        new OpenCvSharp.CvPoint2D32f(ptt[1][0], ptt[1][1]),
            //        new OpenCvSharp.CvPoint2D32f(ptt[2][0], ptt[2][1]),
            //        new OpenCvSharp.CvPoint2D32f(ptt[3][0], ptt[3][1]),
            //    };

            //    //OpenCvSharp.CvMat out_pt = new OpenCvSharp.CvMat(4, 2, OpenCvSharp.MatrixType.F32C1, new float[]{
            //    //     ptt[0][0], ptt[0][1],
            //    //    ptt[1][0], ptt[1][1], 
            //    //    ptt[2][0], ptt[2][1], 
            //    //    ptt[3][0], ptt[3][1]);


            //    var T = OpenCvSharp.Cv.GetPerspectiveTransform(in_pt, out_pt);


            //    //  var rot=Perspective.CreateRotation(t.a, t.b, t.c);
            //    // var T = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1,new float[]{0,0,0, 0,0,0, 0,0,0});
            //    // var Rot = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1, new float[]{rot[0][0], rot[0][1], 0, rot[1][0], rot[1][1], 0, rot[2][0], rot[2][1], 0 });
            //    //// var translate = new OpenCvSharp.CvMat(3, 1, OpenCvSharp.MatrixType.F32C1, new float[] { -width*supersample/2, -height*supersample/2, 1 });
            //    // var tt = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1, new float[] { 0,0, width * supersample / 2/t.d,  0,0, height * supersample / 2/t.d, 0, 0, 0 });
            //    // // var tt= I * translate;
            //    // //var T = Rot;// +tt;
            //    // //(Rot+tt).Inv(T);
            //    // T = Rot + tt;
            //    // //for (int i = 0; i < 3; i++)
            //    // //{
            //    // //    for (int j = 0; j < 3; j++)
            //    // //    {

            //    // //    }
            //    // //}
            //    // T = T0;


            //    //var srcup=OpenCvSharp.Cv.CreateImage(new OpenCvSharp.CvSize(width*supersample,height*supersample), OpenCvSharp.BitDepth.U8,3);

            //    OpenCvSharp.IplImage dstup = new OpenCvSharp.IplImage(width * supersample, height * supersample, OpenCvSharp.BitDepth.U8, 3);
            //    dstup.Set(new OpenCvSharp.CvScalar(255, 255, 255));

            //    //src.Resize(srcup, OpenCvSharp.Interpolation.Lanczos4);

            //    //src.WarpPerspective(dstup, T, OpenCvSharp.Interpolation.Lanczos4 | OpenCvSharp.Interpolation.InverseMap, new OpenCvSharp.CvScalar(255, 255, 255));//|OpenCvSharp.Interpolation.InverseMap
            //    src.WarpPerspective(dstup, T, OpenCvSharp.Interpolation.Lanczos4, new OpenCvSharp.CvScalar(255, 255, 255));//|OpenCvSharp.Interpolation.InverseMap

            //    OpenCvSharp.IplImage dst = new OpenCvSharp.IplImage(width, height, OpenCvSharp.BitDepth.U8, 3);

            //    OpenCvSharp.IplImage dstupsmooth = new OpenCvSharp.IplImage(width * supersample, height * supersample, OpenCvSharp.BitDepth.U8, 3);

            //    var kernel = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1);
            //    for (int i = 0; i < 3; i++)
            //    {
            //        for (int j = 0; j < 3; j++)
            //        {
            //            kernel[i, j] = 1.0f / 10;
            //            if (i == 1 && j == 1)
            //            {
            //                kernel[i, j] = 2.0f / 10;
            //            }
            //        }
            //    }

            //    dstup.Filter2D(dstupsmooth, kernel);

            //    dstupsmooth.Resize(dst, OpenCvSharp.Interpolation.NearestNeighbor);

            //    //OpenCvSharp.Cv.ShowImage("Dst",dst);
            //    return dst;


            //}

            //public static OpenCvSharp.IplImage RenderWithWarp3(Rqim rqim, int width, int height, TransformArgs t, float thicknessMultiplier = 0.010f)
            //{
            //    int supersample = 3;

            //    var src = Render(rqim, width * supersample, height * supersample, thicknessMultiplier);
            //    float w = width * supersample;
            //    float h = height * supersample;
            //    //f = d;



            //    var r=Perspective.CreateRotationFull(t.a, t.b, t.c);
            //      var R=new OpenCvSharp.CvMat(3, 3,OpenCvSharp.MatrixType.F32C1, new float[]{r[0][0], r[0][1], r[0][2], r[1][0], r[1][1], r[1][2], r[2][0], r[2][1], r[2][2]} );
            //      var tr = new OpenCvSharp.CvMat(3, 1, OpenCvSharp.MatrixType.F32C1, new float[] { -width * supersample / 2 / t.z, -height * supersample / 2 / t.z, 1 });
            //      var n = R * new OpenCvSharp.CvMat(3, 1, OpenCvSharp.MatrixType.F32C1, new float[] { 0,0, 1 });
            //      var nt = n.Transpose();
            //      var T = R - tr * nt / t.z;

            //     //var T = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1,new float[]{0,0,0, 0,0,0, 0,0,0});
            //     //var translate = new OpenCvSharp.CvMat(3, 1, OpenCvSharp.MatrixType.F32C1, new float[] { -width * supersample / 2/t.d, -height * supersample / 2/t.d, 1 });
            //     //var rt = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1, new float[]{rot[0][0], rot[0][1], (float)translate[0][0], rot[1][0], rot[1][1], (float)translate[1][0], rot[2][0], rot[2][1], (float)translate[2][0] });
            //    //// 
            //    // var tt = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1, new float[] { 0,0, width * supersample / 2/t.d,  0,0, height * supersample / 2/t.d, 0, 0, 0 });
            //    // // var tt= I * translate;
            //    // //var T = Rot;// +tt;
            //    // //(Rot+tt).Inv(T);
            //    // T = Rot + tt;
            //    // //for (int i = 0; i < 3; i++)
            //    // //{
            //    // //    for (int j = 0; j < 3; j++)
            //    // //    {

            //    // //    }
            //    // //}
            //     //rt.Inv(T);
            //    // T = rt;


            //    //var srcup=OpenCvSharp.Cv.CreateImage(new OpenCvSharp.CvSize(width*supersample,height*supersample), OpenCvSharp.BitDepth.U8,3);

            //    OpenCvSharp.IplImage dstup = new OpenCvSharp.IplImage(width * supersample, height * supersample, OpenCvSharp.BitDepth.U8, 3);
            //    dstup.Set(new OpenCvSharp.CvScalar(255, 255, 255));

            //    //src.Resize(srcup, OpenCvSharp.Interpolation.Lanczos4);

            //    //src.WarpPerspective(dstup, T, OpenCvSharp.Interpolation.Lanczos4 | OpenCvSharp.Interpolation.InverseMap, new OpenCvSharp.CvScalar(255, 255, 255));//|OpenCvSharp.Interpolation.InverseMap
            //    src.WarpPerspective(dstup, T, OpenCvSharp.Interpolation.Lanczos4, new OpenCvSharp.CvScalar(255, 255, 255));//|OpenCvSharp.Interpolation.InverseMap

            //    OpenCvSharp.IplImage dst = new OpenCvSharp.IplImage(width, height, OpenCvSharp.BitDepth.U8, 3);

            //    OpenCvSharp.IplImage dstupsmooth = new OpenCvSharp.IplImage(width * supersample, height * supersample, OpenCvSharp.BitDepth.U8, 3);

            //    var kernel = new OpenCvSharp.CvMat(3, 3, OpenCvSharp.MatrixType.F32C1);
            //    for (int i = 0; i < 3; i++)
            //    {
            //        for (int j = 0; j < 3; j++)
            //        {
            //            kernel[i, j] = 1.0f / 10;
            //            if (i == 1 && j == 1)
            //            {
            //                kernel[i, j] = 2.0f / 10;
            //            }
            //        }
            //    }

            //    dstup.Filter2D(dstupsmooth, kernel);

            //    dstupsmooth.Resize(dst, OpenCvSharp.Interpolation.NearestNeighbor);

            //    //OpenCvSharp.Cv.ShowImage("Dst",dst);
            //    return dst;


            //}

            //public static void SaveAsImage(string filename, Rqim rqim)
            //{
            //    var im = RqimRenderer.Render(rqim, 640, 480);

            //    im.SaveImage(filename);
            //    im.ReleaseData();
            //    im.Dispose();
            //}

       // }
    }

}
