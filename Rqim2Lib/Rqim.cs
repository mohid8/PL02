using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    
        [DataContract]
        public class Rqim//:IEnumerable<Quad>,IEnumerator<Quad>
        {
            public void AddQuad(Quad q)
            {
                if (q.Id == -1)
                {
                    q.Id = Quads.Count;
                }
                Quads.Add(q);
            }

            public Quad this[int k]
            {
                get
                {
                    return Quads[k];
                }
            }

            public int QuadCount
            {
                get
                {
                    return Quads.Count;
                }
            }

          
            public List<Quad> GetQuadList()
            {
                return Quads;
            }
            [DataMember]
            protected List<Quad> Quads = new List<Quad>();
            [DataMember]
            public float Width = 1;
            [DataMember]
            public float Height = 1;

            public void Save(Stream s)
            {
                //XmlSerializer xml = new XmlSerializer(typeof(Rqim));
                //xml.Serialize(s, this);

                DataContractSerializer dcs = new DataContractSerializer(typeof(Rqim));
                dcs.WriteObject(s, this);

            }

            public void FixWinding()
            {
                foreach (var q in Quads)
                {
                    q.FixWinding();
                }
            }

            public static Rqim Load(Stream s)
            {
                //var xml = new XmlSerializer(typeof(Rqim));
                //return (Rqim)xml.Deserialize(s);
                DataContractSerializer dcs = new DataContractSerializer(typeof(Rqim));
                var rqim= (Rqim)dcs.ReadObject(s);
                rqim.FixWinding();
                return rqim;
            }

            public void Save(string filename)
            {
                using (var fs = File.Create(filename))
                {
                    Save(fs);
                }
            }

            public static Rqim Load(string filename)
            {
                using (var fs = File.OpenRead(filename))
                {
                    return Load(fs);
                }
            }


            //public Rqim WarpPerspective(TransformSetArgs t)
            //{
            //    Rqim rqim = new Rqim();
            //    foreach (var q in Quads)
            //    {
            //        var qt = q.PerspectiveTransform(t);
            //        rqim.AddQuad(qt);
            //    }
            //    return rqim;
            //}

            public Rqim Translate2D(float x, float y)
            {
                Rqim rqim = new Rqim();
                foreach (var q in Quads)
                {
                    var qt = q.Translate2D(x, y);
                    rqim.AddQuad(qt);
                }
                return rqim;
            }

            public Rqim Scale2D(float scale)
            {
                Rqim rqim = new Rqim();
                foreach (var q in Quads)
                {
                    var qt = q.Scale2D(scale);
                    rqim.AddQuad(qt);
                }
                return rqim;
            }

            public Rqim Transformabcxyzf(TransformArgs t)
            {
                Rqim rqim = new Rqim();
                foreach (var q in Quads)
                {
                    var qt = q.Transformabcxyzf(t);
                    rqim.AddQuad(qt);
                }
                return rqim;
            }

            public Rqim AddNoise(float stdDev)
            {
                Rqim rqim = new Rqim();
                foreach (var q in Quads)
                {
                    var qt = q.AddNoise(stdDev);
                    rqim.AddQuad(qt);
                }
                return rqim;
            }

            public Rqim AddQuantizer(float quant)
            {
                Rqim rqim = new Rqim();
                foreach (var q in Quads)
                {
                    var qt = q.AddQuantizer(quant);
                    rqim.AddQuad(qt);
                }
                return rqim;
            }

            //public void SaveimageRotated(string filename, TransformArgs t)
            //{
            //    var im = RqimRenderer.RenderWithWarp2(this, 640, 480, t);
            //    im.SaveImage(filename);

            //    // var im2 = RqimRenderer.RenderWithWarp(this, 640, 480, t);
            //    // im2.SaveImage(filename+"1.png");
            //}

            //public void SaveImage(string filename)
            //{
            //    var im = RqimRenderer.Render(this, 640, 480);
            //    im.SaveImage(filename);
            //}
            public void SaveImage(string filename, int size=640)
            {
                var lines = new List<Tuple<PointF, PointF, float>>();
                foreach (var q in Quads)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        lines.Add(new Tuple<PointF, PointF, float>(q.Corners[i], q.Corners[i + 1], q.InnerLength));
                    }
                }

                Drawing.DrawLinesToFile(filename, lines.ToArray(), size, size);

            }

            public void Clip(float minx, float maxx, float miny, float maxy)
            {
                Quads.RemoveAll(x => x.Corners.Any(y => y.X < minx || y.X > maxx || y.Y < miny || y.Y > maxy));
            }


            public void EstimatePose(Rqim reference)
            {
                //TODO
                //RobustPlanarPose.Pose.EstimatePose()
            }

        }
    

}
