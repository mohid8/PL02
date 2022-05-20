using Rqim2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create discrete random
            //var rrqim = RqimCreator.CreateDiscreteRandomRqim(100, 10);//60 7
            //rrqim.Save("test.rqim");
            //rrqim=rrqim.Translate2D(-0.5f, -0.5f);
            //rrqim.SaveImage("test.png",2560);

            //Create random
            //  var rrqim =RqimCreator.CreateRandomRqim(100,80);
            //  rrqim.Save("test.rqim");

            //load file
            var rqim_reference = Rqim.Load("..\\data\\rqim10.rqim");

            //save image
            //rqim.SaveImage("..\\images\\teszt35.png");

            //translate
            //var rqim2 = rqim.Translate2D(-0.5f, -0.5f);

            //Scale
            rqim_reference = rqim_reference.Scale2D(640);

            //Save file 
            //rqim.Save("..\\data\\teszt35.rqim");

            //Detect
            var irqm = RqimDetector.FromImage(@"..\images\10_cam_11.png");











            //--------------------------


            //var alap00 = new Rqim(); alap00.AddQuad(new Quad() { Corners = new System.Drawing.PointF[] { new System.Drawing.PointF(0, 0), new System.Drawing.PointF(1, 0), new System.Drawing.PointF(1, 1), new System.Drawing.PointF(0, 1), } });
            //alap00.FixWinding();
            //alap00.Save("p.rqim");
            //var alap0 = Rqim.Load("p.rqim");
            ////rqim3_01.SaveImage("t01.png");
            //var alap = alap0.Translate2D(-0.5f, -0.5f);
            //alap = alap.Scale2D(0.5f);
            //alap = alap.Scale2D(200);
            //alap.Save("05as.rqim");
           // alap.SaveImage("alap05.png");
            //var tr = new TransformArgs() { a = 20f, b = 0, c = 0, fx = 1, fy = 1, x = 0, y = 0, z = -2 };
            //var alapTr = alap.Transformabcxyzf(tr);
            //alapTr.SaveImage("alapTr.png");
             
            //TransformSetArgs ts2;
            //var tr2 = RqimProcessor.EstimatePosition3DRQIMMatch(alapTr, alap, 1, out ts2, 500, 1);

            //var alapTr2 = alapTr.Transformabcxyzf(new TransformArgs() {a=-tr2.a, b=-tr2.b, c=-tr2.c, fx=1, fy=1, x=-tr2.x, y=-tr2.y, z=-1 });
            //alapTr2.SaveImage("alapTr2.png");

           // var irqm = RqimDetector.FromImage(@"..\images\10_cam_14.png");
 //           var irqm = RqimDetector.FromImage(@"..\images\10_cam_11.png"); 
            //var irqm = RqimDetector.FromImage(@"..\images\10_cam_22.png");
 ///           var irqm = RqimDetector.FromImage(@"..\images\05a_xpt_03.png");

            //XperiaT f=660, fov=51.73deg
            //var irqm = RqimDetector.FromImage(@"..\images\T35_3ds05_75deg_30d20_100_50_170.png");
    //        var irqm = RqimDetector.FromImage(@"..\images\t10_20_40_-1.5.png");
            //var irqm = RqimDetector.FromImage(@"..\images\10_cam_04.png");
            //var irqm = RqimDetector.FromImage(@"p.png");
            //irqm = irqm.Scale2D(1.0f / 640).Translate2D(-0.5f, -0.5f)/*Scale2D(1.0f / 640).*/; 
   //         irqm = irqm.Translate2D(-320,-240)/*Scale2D(1.0f / 640).*/;
           // irqm=irqm.Scale2D(1.0f / 615);
   //         irqm = irqm.Scale2D(1.0f / 417);
  ///          irqm.Save("0a_xpt_03.rqim");
          //  var irqm = Rqim.Load("Teszt35.png.rqim");
            //irqm=irqm.Scale2D(1.0f / 660);
          //  irqm=irqm.Scale2D(1.0f / 615);
            //irqm.Save("Test10d_04.rqim");
            //irqm.SaveImage("Test10d_04.rqim.png");

            //var irqm=Rqim.Load("Test10d_04.rqim");

            //var irqm = new Rqim(); irqm.AddQuad(new Quad() { Corners = new System.Drawing.PointF[] { new System.Drawing.PointF(0, 0), new System.Drawing.PointF(1, 0), new System.Drawing.PointF(1, 1), new System.Drawing.PointF(0, 1), } });
           // irqm = irqm.Translate2D(-0.5f, -0.5f);
           // irqm.Save("05a_xd_01.rqim");

           // var irqm=Rqim.Load("Teszt10.rqim");
           // irqm = irqm.Translate2D(-0.5f, -0.5f)./*Scale2D(1.0f / 640).*/Scale2D(1.0f / 2); 
           

            //var rqim3_01 = Rqim.Load("Teszt10.rqim");
   //         var rqim3 = Rqim.Load("Teszt35.rqim");
            //rqim3_01.SaveImage("t01.png");
   //         rqim3 = rqim3.Translate2D(-0.5f, -0.5f);
   //         rqim3 = rqim3.Scale2D(280);
            //irqm = irqm.Scale2D(1.0f / 310);

   //         TransformSetArgs ts;
            //var match=RqimProcessor.EstimatePosition3DRQIMMatch(irqm, rqim3, 1, out ts, 0.1f ,3);
            //var match = RqimProcessor.EstimatePosition3DRQIMMatch(irqm, rqim3, 1, out ts, 0.03f, 5);

            //var retrans = rqim3.Transformabcxyzf(match);
            //retrans = retrans.Translate2D(0.5f, 0.5f);
            //retrans = retrans.Scale2D(1.0f/280/2);
            //retrans = retrans.Translate2D(0.5f, 0.5f);
            //retrans.SaveImage("retrans_3ds_05.png");
            //retrans.SaveImage("retrans_3ds_05.png");

            //rqim3.SaveImage("t10.png");
            //var rqim3r0 = rqim3.Transformabcxyzf(new TransformArgs() { a = 60, b = 40, c = 0, x = 0, y = 0, z = -2, fx = 1, fy = 1 });
            //var rqim3r = rqim3r0.AddNoise(0.0000f);

            //rqim3r.SaveImage("t10_60_40_-2.png");

            //rqim3r = rqim3.Transformabcxyzf(new TransformArgs() { a = 20, b = 40, c = 0, x = 0, y = 0, z = -1.5f, fx = 1, fy = 1 });
            //rqim3r.SaveImage("t10_20_40_-1.5.png");

            //rqim3r = rqim3.Transformabcxyzf(new TransformArgs() { a = -40, b = 40, c = 0, x = 0, y = 0, z = -2, fx = 1, fy = 1 });
            //rqim3r.SaveImage("t10_-40_40_-2.png");

            //rqim3r = rqim3.Transformabcxyzf(new TransformArgs() { a = -40, b = 40, c = 0, x = 0, y = 0, z = -3, fx = 1, fy = 1 });
            //rqim3r.SaveImage("t10_-40_40_-3.png");

            //rqim3r = rqim3.Transformabcxyzf(new TransformArgs() { a = -20, b = 60, c = 0, x = 0, y = 0, z = -2, fx = 1, fy = 1 });
            //rqim3r.SaveImage("t10_-20_60_-2.png");



        }
    }
}
