using Rqim2Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RqimOpenCV
{
    class QuadFitter
    {
        static Random r = new Random(0x55aa);
        public QuadFitter (List<Rqim2Lib.Pixel> pixels)
	    {
            Pixels=pixels;
        }
        List<Rqim2Lib.Pixel> Pixels;
        Tuple<Rqim2Lib.Pixel, Rqim2Lib.Pixel> Select2Random(float minDist2, float maxDist2=float.MaxValue, HashSet<Pixel> exclude=null)
        {
            int timeOut = 500;
            while (timeOut>0)
            {
                timeOut--;
                var i1 = r.Next(Pixels.Count);
                var i2 = r.Next(Pixels.Count);
                   
                if (exclude != null && (exclude.Contains(Pixels[i1]) || exclude.Contains(Pixels[i2]))) { continue; }
                var d = Pixels[i1].Distance2(Pixels[i2]);
                if ((d > minDist2 && d<maxDist2) || timeOut==0)
                {
                    return new Tuple<Rqim2Lib.Pixel, Rqim2Lib.Pixel>(Pixels[i1], Pixels[i2]);
                }
                
            }
            return null;
        }

        public float  Fit3Lines(out Line[] lines)
        {
            float minDist2 = 5 * 5*2;
            float maxDist2=20*20;
            //fit

            //Line lll = new Line(0, 0, 10, 0);
            //var ddd=lll.PointDistance(5, 5);

            //var p2 = Select2Random(minDist2);
            //var l2 = LineFitter.Fit(p2.Item1.u, p2.Item1.v, p2.Item2.u, p2.Item2.v);

            //var p3 = Select2Random(minDist2);
            //var l3 = LineFitter.Fit(p3.Item1.u, p3.Item1.v, p3.Item2.u, p3.Item2.v);

            //lines = new Line[3] {l1,l2,l3 };
            lines = new Line[3];

            //estimate error
            var hashSets=new HashSet<Pixel>[]{new HashSet<Pixel>(),new HashSet<Pixel>(),new HashSet<Pixel>()};
            var errors = new float[] {0,0,0 };
            //var indices=new int[]{0,1,2};
            var distances=new float[3];
            //var pointsNum=new int[]{0,0,0};
            HashSet<Pixel> taken = new HashSet<Pixel>();
            HashSet<Pixel> nottaken = new HashSet<Pixel>(Pixels);

            float minPointsFirstDistTh = 4;
            int minPointsNumTh = 20;
            float minPointsDistTh = 4;

            for (int li = 0; li < 3; li++)
            {
                var pp = Select2Random(minDist2, maxDist2, taken);
                if (pp == null)
                {
                    return float.MaxValue;
                }
                lines[li] = LineFitter.Fit(pp.Item1.u, pp.Item1.v, pp.Item2.u, pp.Item2.v);

                HashSet<Pixel> toremove=new HashSet<Pixel>();

                foreach (var p in nottaken)
                {
                    var dist = lines[li].PointDistance(p.u,p.v);
                    if (dist < minPointsFirstDistTh) { toremove.Add(p); }
                }

                toremove = Utils.GetLargestPixelGroup(toremove);
                
                foreach (var p in toremove)
                {
                    taken.Add(p);
                    nottaken.Remove(p);
                    hashSets[li].Add(p);
                }
                if (hashSets[li].Count < minPointsNumTh)
                {
                    return float.MaxValue;
                }

                //foreach (var p in Pixels)
                //{
                //    for (int i = 0; i < 3; i++) { distances[i] = lines[i].PointDistance(p.u, p.v); }
                //    var minDist = Math.Min(Math.Min(distances[0], distances[1]), distances[2]);
                //    var minDistIndex = 0;
                //    for (int i = 1; i < 3; i++) { if (minDist == distances[i]) minDistIndex = i; }
                //    int[] otherIndices = new int[] { ((minDistIndex + 1) % 3), ((minDistIndex + 2) % 3) };

                //    if (minDist < 10 && distances[otherIndices[0]] > 10 && distances[otherIndices[1]] > 10)
                //    {
                //        errors[minDistIndex] += distances[minDistIndex];
                //        hashSets[minDistIndex].Add(p);
                //    }
                //    //pointsNum[minDistIndex]++;

                //    //var distances=indices.Select((x) => lines[x].PointDistance(p.u, p.v));
                //    //var closestIndex = indices.MinWhere(x=>distances[x]);//indices.MinWhere((x) => lines[x].PointDistance(p.u, p.v));
                //    //hashSets[closestIndex].Add(p);
                //    //errors[closestIndex]+=
                //}

                
                

            }
var pn1 = hashSets[0].Count;
                var pn2 = hashSets[1].Count;
                var pn3 = hashSets[2].Count;


            if (pn1 < minPointsNumTh || pn2 < minPointsNumTh || pn3 < minPointsNumTh)
            {
                return float.MaxValue;
            }
            if (errors[0] / pn1> minPointsDistTh || errors[1] / pn2 > minPointsDistTh || errors[2] / pn3 > minPointsDistTh)
            {
                return float.MaxValue;
            }

            var lf1 = new LineFitter(pn1);
            foreach (var p in hashSets[0])
            {
                lf1.AddPoint(p.u, p.v);
            }
            var lf2 = new LineFitter(pn2);
            foreach (var p in hashSets[1])
            {
                lf2.AddPoint(p.u, p.v);
            }
            var lf3 = new LineFitter(pn3);
            foreach (var p in hashSets[2])
            {
                lf3.AddPoint(p.u, p.v);
            }

            var linesRefitted = new Line[] { lf1.GetFittedXY(), lf2.GetFittedXY(), lf3.GetFittedXY() };
            var errorsRefitted = new float[] { 0, 0, 0 };
            var numRefitted=new int[3];
            linesRefitted[0].Pixels = new List<Pixel>(lf1.PointsNum);
            linesRefitted[1].Pixels = new List<Pixel>(lf2.PointsNum);
            linesRefitted[2].Pixels = new List<Pixel>(lf3.PointsNum);
            foreach (var p in hashSets[0])
            {
                var e = linesRefitted[0].PointDistance(p.u,p.v);
                if (e < minPointsDistTh) { errorsRefitted[0] += e; numRefitted[0]++; linesRefitted[0].Pixels.Add(p); }
            }
            foreach (var p in hashSets[1])
            {
                var e = linesRefitted[1].PointDistance(p.u, p.v);
                if (e < minPointsDistTh) { errorsRefitted[1] += e; numRefitted[1]++; linesRefitted[1].Pixels.Add(p); }
            }
            foreach (var p in hashSets[2])
            {
                var e = linesRefitted[2].PointDistance(p.u, p.v);
                if (e < minPointsDistTh) { errorsRefitted[2] += e; numRefitted[2]++; linesRefitted[2].Pixels.Add(p); }
            }


            lines = linesRefitted;

            //lines[0].Pixels = lines[0].GetLargestPixelGroup();
            //lines[1].Pixels = lines[1].GetLargestPixelGroup();
            //lines[2].Pixels = lines[2].GetLargestPixelGroup();

            //return errorsRefitted[0]/numRefitted[0]+errorsRefitted[1]/numRefitted[1]+errorsRefitted[2]/numRefitted[2];
            return nottaken.Count;

            //float sumError = 0;

            //foreach (var p in Pixels)
            //{
            //    for (int i = 0; i < 3; i++) { distances[i] = lines[i].PointDistance(p.u, p.v); }
            //    var minDist = Math.Min(Math.Min(distances[0], distances[1]), distances[2]);
            //    sumError += minDist;
            //}
            //return sumError;
        }


        public float Fit3Lines(out Line[] bestLines, int trials)
        {
            Line[] lines;          
            bestLines = null;
            float bestE = float.MaxValue;
            while (trials > 0)
            {
                var e = Fit3Lines(out lines);
                if (e < bestE)
                {
                    bestE = e;
                    bestLines = lines;
                }
                trials--;
                if (e < Pixels.Count * 0.05f) { return bestE; }
            }

            return bestE;

        }

    }
}
