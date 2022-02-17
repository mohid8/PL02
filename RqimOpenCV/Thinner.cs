using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rqim2Lib
{
    //public class Pixel : PixelBase
    //{
       
    //    public float Distance2(Pixel p2)
    //    {
    //        return (p2.u - u) * (p2.u - u) + (p2.v - v) * (p2.v - v);
    //    }
    //    public Pixel[] Neighbors = new Pixel[8];
    //    public int NeighborsNum = 0;
    //}
    public class Pixel
    {
        public bool IsSkeletonPixel;
        public double Area3D;
        public int SkeletonPixelsRemovedNum;
        public int u;
        public int v;
        public int id;
        public float Distance2(Pixel p2)
        {
            return (p2.u - u) * (p2.u - u) + (p2.v - v) * (p2.v - v);
        }
        public Pixel[] Neighbors = new Pixel[8];
        public int NeighborsNum = 0;
        public int GroupId;
    }

    public class Thinner
    {

        public struct P
        {
            public P(int X, int Y)
            {
                x = X; y = Y;
                value = true;
                pMap = null;
                processed = false;

            }
            public P(int X, int Y, bool v)
            {
                x = X; y = Y; value = v;
                pMap = null;
                processed = false;
            }
            public P(int X, int Y, bool v, P[,] pMap)
            {
                x = X; y = Y; value = v;
                this.pMap = pMap;
                processed = false;
            }
            public P[,] pMap;
            public int x;
            public int y;
            public bool value;
            public bool processed;
            public bool EqualsPos(P p)
            {
                return x == p.x && y == p.y && value == p.value;
            }
            public bool EqualsAll(P p)
            {
                //var p=(P)obj;
                return x == p.x && y == p.y && value == p.value;
            }

            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                if (pMap != null)
                {

                    int w = pMap.GetLength(0);
                    int h = pMap.GetLength(1);
                    //
                    for (int j = -1; j <= 1; j++)
                    {
                        for (int i = -1; i <= 1; i++)
                        {
                            if (i == 0 && j == 0)
                                sb.Append("!");
                            if (y + j >= 0 && x + i >= 0 && x + i < w && y + j < h)
                            {
                                sb.AppendFormat("{0}", pMap[x + i, y + j].value ? 1 : 0);
                            }
                            else
                            {
                                sb.AppendFormat("-");
                            }
                            if (i == 0 && j == 0)
                                sb.Append("!");
                            sb.Append(" ");
                        }
                        sb.AppendLine(";");
                    }


                }
                return sb.ToString();
            }
        }

        int W;
        int H;
        int mapOrigX, mapOrigY;
        public Thinner(Pixel[,] map, int mapOrigX, int mapOrigY)
        {
            PixelsNum = 0;
            pixelMap = map;
            W = map.GetLength(0);
            H = map.GetLength(1);
            this.mapOrigX = mapOrigX;
            this.mapOrigY = mapOrigY;
            boolMap = new bool[W, H];
            for (int i = 0; i < W; i++)
            {
                for (int j = 0; j < H; j++)
                {
                    boolMap[i, j] = false;
                    if (map[i, j] != null)
                    {
                        boolMap[i, j] = true;
                        PixelsNum++;
                    }
                }
            }

        }
        Pixel[,] pixelMap;

        bool[,] boolMap;


        bool[,] CopyMap(bool[,] map)
        {
            int w = map.GetLength(0); int h = map.GetLength(1);
            var map2 = new bool[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    map2[i, j] = map[i, j];
                }
            }
            return map2;
        }
        public static P Q_N = new P(0, -1);
        public static P Q_NE = new P(1, -1);
        public static P Q_E = new P(1, 0);
        public static P Q_SE = new P(1, 1);
        public static P Q_S = new P(0, 1);
        public static P Q_SW = new P(-1, 1);
        public static P Q_W = new P(-1, 0);
        public static P Q_NW = new P(-1, -1);

        public static P[] Q_NE_SW = new P[] { Q_NE, Q_SW };
        public static P[] Q_N_E_S_W = new P[] { Q_N, Q_E, Q_S, Q_W };
        public static P[] Q_NE_SW_NW_SE = new P[] { Q_NE, Q_SW, Q_NW, Q_SE };

        P[] N8Star = new P[] { Q_NW, Q_N, Q_NE, Q_E, Q_SE, Q_S, Q_SW, Q_W };
        P[] N4Star = new P[] { Q_N, Q_E, Q_S, Q_W };


        P[] Opposites = new P[]{
           
            Q_NE, Q_N, Q_E, 
            Q_N, Q_NW, Q_NE,
            Q_NW, Q_W, Q_N,
            Q_E, Q_NE, Q_SE,
            new P(0,0), new P(0,0), new P(0,0),
            Q_W, Q_NW, Q_SW,
            Q_SE, Q_E, Q_S,
            Q_S, Q_SE, Q_SW,
            Q_SW, Q_S, Q_E,
        };

        static Random random = new Random(1);
        void AddToOppositeFrom(P p, P d, int[,] ii)
        {

            var d2 = new P(-d.x, -d.y);
            {
                if (Ofset(d2.x, d2.y, p))
                {
                    ii[p.x + d2.x, p.y + d2.y] += ii[p.x, p.y] + 1;
                    return;
                }
            }

        }
        int PixelsNum;

        Pixel[,] CreatePixelsMap(bool[,] bMap, Pixel[,] pMap)
        {

            int w = bMap.GetLength(0); int h = bMap.GetLength(1);
            Pixel[,] map = new Pixel[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (bMap[j, j])
                    {
                        map[i, j] = pMap[i, j];
                    }
                    else
                    {
                        map[i, j] = null;
                    }
                }
            }
            return map;
        }

        P[,] CreatePMap(bool[,] map)
        {
            int w = map.GetLength(0); int h = map.GetLength(1);
            P[,] Pmap = new P[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Pmap[i, j] = new P(i, j, map[i, j], Pmap);
                }
            }
            return Pmap;
        }

        void CreatePProcessedMap(P[,] pmap, out P[] AllPixels, out bool[] ProcessedBool)
        {
            int w = pmap.GetLength(0);
            int h = pmap.GetLength(1);
            ProcessedBool = new bool[PixelsNum];
            AllPixels = new P[PixelsNum];
            int n = 0;
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (pmap[i, j].value)
                    {
                        AllPixels[n] = (pmap[i, j]);
                        ProcessedBool[n] = false;
                        n++;
                    }
                }
            }

        }
        P[,] CreatePMap(bool[,] map, out P[] AllPixels, out bool[] ProcessedBool, out int[,] Indices)
        {
            int w = map.GetLength(0); int h = map.GetLength(1);
            P[,] Pmap = new P[w, h];
            ProcessedBool = new bool[PixelsNum];
            AllPixels = new P[PixelsNum];
            int n = 0;
            Indices = new int[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    Pmap[i, j] = new P(i, j, map[i, j], Pmap);
                    if (map[i, j])
                    {
                        AllPixels[n] = (Pmap[i, j]);
                        ProcessedBool[n] = false;
                        Indices[i, j] = n;
                        n++;
                    }
                }
            }
            return Pmap;
        }
        HashSet<P> CreatePHashSet(P[,] pmap)
        {
            int w = pmap.GetLength(0);
            int h = pmap.GetLength(1);
            var P = new HashSet<P>();//w*h

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (pmap[i, j].value)
                    {
                        P.Add(pmap[i, j]);
                    }
                }
            }
            return P;
        }
        bool isValidCoordinate(int x, int y)
        {
            return (x >= 0 && y >= 0 && x < W && y < H);
        }

        bool Ofset(int x1, int y1, int x2, int y2)
        {
            int x = x1 + x2;
            int y = y1 + y2;
            if (isValidCoordinate(x, y))
            {
                return boolMap[x, y];
            }
            return false;
        }

        bool Ofset(int x1, int y1, P p)
        {
            int x = x1 + p.x;
            int y = y1 + p.y;
            if (isValidCoordinate(x, y))
            {
                return boolMap[x, y];
            }
            return false;
        }

        bool Ofset(P p0, P p, P[,] pmap)
        {
            int x = p0.x + p.x;
            int y = p0.y + p.y;
            if (isValidCoordinate(x, y))
            {
                return pmap[x, y].value;
            }
            return false;
        }
        P? OfsetPoint(P p0, P p, P[,] pmap)
        {
            int x = p0.x + p.x;
            int y = p0.y + p.y;
            if (isValidCoordinate(x, y))
            {
                return pmap[x, y];
            }
            return null;
        }
        bool IsBorder(P p, P[,] pmap)
        {
            if (!p.value) return false;
            return N4Star.Any(x => !Ofset(p, x, pmap));
        }

        bool IsBorder4(int x, int y)
        {
            return N4Star.Any((p) => !Ofset(x, y, p));
        }
        bool IsBorder8(int x, int y)
        {
            return N8Star.Any((p) => !Ofset(x, y, p));
            //if (Ofset(x, y, 0, -1) ||
            //    Ofset(x, y, 1, 0) ||
            //    Ofset(x, y, 0, 1) ||
            //    Ofset(x, y, -1, 0) ||
            //    Ofset(x,y,-1,1) ||
            //    Ofset(x,y,1,1) ||
            //    Ofset(x,y,1,-1) ||
            //    Ofset(x,y,-1,-1)
            //    )
            //    return true;
            //return false;
        }
        bool IsBorderD(P p, P d, P[,] pmap)
        {
            if (!p.value)
                return false;
            if (N4Star.Count((x) => Ofset(p, x, pmap)) == 4)
                return false;
            if (d.x == 0 || d.y == 0)
            {
                return !Ofset(p, d, pmap);
            }
            return !Ofset(p, new P(d.x, 0), pmap) || !Ofset(p, new P(0, d.y), pmap);

        }

        int N8pStar(int x, int y)
        {
            return N8Star.Count((p) => Ofset(x, y, p));
            //int N = 0;
            //for (int i = -1; i <= 1; i++)
            //{
            //    for (int j = -1; j <= 1; j++)
            //    {
            //        if (i != 0 && j != 0)
            //        {
            //            if (Ofset(x, y, i, j))
            //                N++;
            //        }
            //    }
            //}
            //return N;
        }

        int N4pStar(int x, int y)
        {
            return N4Star.Count((p) => Ofset(x, y, p));
            //int N=0;
            //if (Ofset(x, y, 0, 1)) N++;
            //if (Ofset(x, y, 0, -1)) N++;
            //if (Ofset(x, y, 1, 0)) N++;
            //if (Ofset(x, y, -1, 0)) N++;
            //return N;
        }


        bool[] ArticulatedTable ={
           false, false, false, false, false, true, false, false, false, true, false, false, false, true, false, false,
false, true, true, true, true, true, true, true, false, true, false, false, false, true, false, false,
false, true, true, true, true, true, true, true, false, true, false, false, false, true, false, false,
false, true, true, true, true, true, true, true, false, true, false, false, false, true, false, false,
false, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
true, true, true, true, true, true, true, true, true, true, true, true, true, true, true, true,
false, true, true, true, true, true, true, true, false, true, false, false, false, true, false, false,
false, true, true, true, true, true, true, true, false, true, false, false, false, true, false, false,
false, false, false, false, true, true, false, false, true, true, false, false, true, true, false, false,
true, true, true, true, true, true, true, true, true, true, false, false, true, true, false, false,
false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false,
false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false,
false, false, false, false, true, true, false, false, true, true, false, false, true, true, false, false,
true, true, true, true, true, true, true, true, true, true, false, false, true, true, false, false,
false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false,
false, false, false, false, true, true, false, false, false, false, false, false, false, false, false, false,
                                     };
        /*0 0 0 0 0 1 0 0 0 1 0 0 0 1 0 0
        0 1 1 1 1 1 1 1 0 1 0 0 0 1 0 0
        0 1 1 1 1 1 1 1 0 1 0 0 0 1 0 0
        0 1 1 1 1 1 1 1 0 1 0 0 0 1 0 0
        0 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
        1 1 1 1 1 1 1 1 1 1 1 1 1 1 1 1
        0 1 1 1 1 1 1 1 0 1 0 0 0 1 0 0
        0 1 1 1 1 1 1 1 0 1 0 0 0 1 0 0
        0 0 0 0 1 1 0 0 1 1 0 0 1 1 0 0
        1 1 1 1 1 1 1 1 1 1 0 0 1 1 0 0
        0 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0
        0 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0
        0 0 0 0 1 1 0 0 1 1 0 0 1 1 0 0
        1 1 1 1 1 1 1 1 1 1 0 0 1 1 0 0
        0 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0
        0 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0*/
        static List<P> Q_W_SW_S_SE_E_NE_N_NW = new List<P>() { Q_W, Q_SW, Q_S, Q_SE, Q_E, Q_NE, Q_N, Q_NW };
        bool IsArticulationPoint(P p, P[,] pmap, P? exception = null)
        {
            //int changes = 0;
            //bool actual=false;
            //bool first=true;
            //for (int j = 0; j < 9; j++)
            //{
            //    var pp = OfsetPoint(p, Q_W_SW_S_E_SE_E_NE_N_NW[j%8], pmap);
            //    if (pp.HasValue)
            //    {

            //        if (first)
            //        {
            //            first = false;
            //        }
            //        else
            //        {
            //            if (pp.Value.value)
            //            {
            //                if (!actual)
            //                {
            //                    changes++;
            //                }
            //            }
            //            else
            //            {
            //                if (actual)
            //                {
            //                    changes++;
            //                }
            //            }

            //        } 
            //        actual = pp.Value.value;
            //    }
            //}
            //return changes<=2;


            int i = 0;
            for (int j = 0; j < 8; j++)
            {
                var pp = OfsetPoint(p, Q_W_SW_S_SE_E_NE_N_NW[j], pmap);
                if (pp.HasValue && pp.Value.value)
                {
                    if (!exception.HasValue)
                    {
                        i |= 1;
                    }
                    else
                    {
                        if (!exception.Value.EqualsPos(pp.Value))
                        {
                            i |= 1;
                        }
                    }
                    //if (!(exception.HasValue && pp.Value.EqualsPos(p)))
                    //{
                    //    i |= 1;

                    //}
                }
                i = i << 1;

            }
            i = i >> 1;
            return !ArticulatedTable[i];
            /*
                if (Ofset(p,Q_W,pmap)) i |= 1;
                var p2=OfsetPoint(p,
                i = i << 1;
                if (Ofset(p,Q_SW,pmap)) i |= 1;
                i = i << 1;
                if (Ofset(p,Q_S,pmap)) i |= 1;
                i = i << 1;
                if (Ofset(p,Q_SE,pmap)) i |= 1;
                i = i << 1;
                if (Ofset(p, Q_E, pmap)) i |= 1;
                i = i << 1;
                if (Ofset(p, Q_NE, pmap)) i |= 1;
                i = i << 1;
                if (Ofset(p, Q_N, pmap)) i |= 1;
                i = i << 1;
                if (Ofset(p, Q_NW, pmap)) i |= 1;*/

            //return !ArticulatedTable[i];
        }

        //bool IsBlackComponent(int x, int y)
        //{
        //    return N8pStar(x, y) == 8;
        //}

        //bool IsSimple(int x, int y)
        //{
        //    //return IsBorder4(x, y) && N8pStar(x, y) == 1;
        //    return IsBorder4(x, y) && IsArticulationPoint(;
        //    //return IsBorder4(x, y) && N8Star.Count((p) => IsBlackComponent(x + p.x, y + p.y))==1;
        //}
        int N8pStar(P p, P[,] pmap, P? exclude = null)
        {
            return N8Star.Count((x) => (Ofset(p, x, pmap) && (exclude.HasValue ? !(x.x + p.x == exclude.Value.x && x.y + p.y == exclude.Value.y) : true)));
        }
        //bool IsBlackComponent(P p, P[,] pmap, P? exlcude)
        //{
        //    return N8pStar(p,pmap,exlcude) == (exlcude.HasValue?8:7);
        //}
        bool IsSimple(P p, P[,] pmap, P? exclude = null)
        {
            return IsArticulationPoint(p, pmap, exclude);

            //var border=IsBorder(p,pmap);
            ////return border && !IsArticulationPoint(p,pmap);
            //if (!border)
            //    return false;

            //int changes = 0;

            //bool actual = Ofset(p, Q_W_SW_S_SE_E_NE_N_NW[0], pmap);
            //var startP=OfsetPoint(p, Q_W_SW_S_SE_E_NE_N_NW[0], pmap);
            //if (exclude.HasValue && startP.HasValue)
            //{
            //    if (exclude.Value.EqualsPos(startP.Value))
            //    {
            //        actual = false;
            //    }
            //}

            //bool val=actual;

            //for (int i = 1; i < 9; i++)
            //{
            //    var pp = OfsetPoint(p, Q_W_SW_S_SE_E_NE_N_NW[i % 8], pmap);
            //    if (pp.HasValue)
            //    {
            //        val = pp.Value.value;
            //        if (exclude.HasValue)
            //        {
            //            if (exclude.Value.EqualsPos(pp.Value))
            //            {
            //                val = false;
            //            }
            //        }
            //        if (val)
            //        {
            //            if (!actual)
            //            {
            //                changes++;
            //            }
            //        }
            //        else
            //        {
            //            if (actual)
            //            {
            //                changes++;
            //            }
            //        }
            //    }
            //    actual = val;

            //}

            //if (changes <= 2)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}

        }

        static P[] Exception1 = new P[] { new P(0, 0, true), new P(1, 1, true) };
        static P[] Exception2 = new P[] { new P(0, 0, true), new P(-1, 1, true) };

        static P[] ExceptionA = new P[] { new P(0, 0, true), new P(1, 0, false), new P(0, 1, false), new P(1, 1, true) };
        static P[] ExceptionB = new P[] { new P(0, 0, true), new P(-1, 0, false), new P(0, 1, false), new P(-1, 1, true) };
        static P[] ExceptionC = new P[] { new P(0, 0, true), new P(-1, 0, false), new P(0, 1, true), new P(-1, 1, true) };
        static P[] ExceptionD = new P[] { new P(0, 0, true), new P(1, 0, false), new P(0, 1, true), new P(1, 1, true) };
        static P[] ExceptionE = new P[] { new P(0, 0, true), new P(1, 0, true), new P(0, 1, false), new P(1, 1, true) };
        static P[] ExceptionF = new P[] { new P(0, 0, true), new P(1, 0, true), new P(0, 1, false), new P(1, 1, true) };
        static P[] ExceptionG = new P[] { new P(0, 0, true), new P(1, 0, true), new P(0, 1, true), new P(1, 1, true) };

        /// <summary>
        /// lexicographical order relation
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        bool LOR(int x1, int y1, int x2, int y2)
        {
            if (y1 < y2)
                return true;
            if (y2 > y1)
                return false;
            return x1 < x2;
        }
        bool LOR(P q, P p)
        {
            if (q.y < p.y)
                return true;
            if (p.y < q.y)
                return false;
            return q.x < p.x;
        }
        bool IsPCoinciding(P p, P[] Exception, P[,] pmap)
        {
            //return (Exception1.All(p=>(Ofset(x,y,p)==p.value)) &&
            //    Exception2.All(p=>(Ofset(x,y,p)==p.value))
            //    );
            return (Exception.All(x => (Ofset(p, x, pmap) == x.value)));
        }

        bool IsSI_d_deletable(P p, P d, P[,] pmap)
        {
            //p is already simple
            //if (IsSimple(P)...
            if (!IsBorderD(p, d, pmap))
                return false;
            // if (!IsSimple(p, pmap))
            //{
            //    return false;
            //}
            //bool ok = false;
            foreach (var q_ in N4Star)
            {
                var q = OfsetPoint(p, q_, pmap);
                if (q.HasValue && q.Value.value)
                {
                    //if (!q.Value.value)
                    //{
                    //    return false;
                    //}
                    if (IsBorderD(q.Value, d, pmap) && IsSimple(q.Value, pmap))
                    {
                        var b3 = LOR(q.Value, p);
                        //return b3;

                        bool b1 = false;
                        bool b2 = false;
                        if (!b3)
                        {
                            b1 = IsSimple(p, pmap, q.Value);
                            b2 = IsSimple(q.Value, pmap, p);
                        }
                        if (!(b1 || b2 || b3))
                        {
                            return false;
                        }
                        //else
                        //{
                        //    ok = true;
                        //}
                    }

                }
            }
            //if (!ok)
            //{
            //    //return false;
            //}
            //if (d.EqualsPos(Q_N) || d.EqualsPos(Q_E) || d.EqualsPos(Q_S) || d.EqualsPos(Q_W))
            //{
            //    if (IsPCoinciding(p, ExceptionA, pmap) || IsPCoinciding(p, ExceptionB, pmap))
            //        return false;
            //}
            //if (d.EqualsPos(Q_NE)){
            //    if (IsPCoinciding(p,ExceptionA,pmap) || IsPCoinciding(p,ExceptionB,pmap) || IsPCoinciding(p,ExceptionC,pmap) || IsPCoinciding(p,ExceptionE,pmap) || IsPCoinciding(p,ExceptionF,pmap)){
            //        return false;
            //    }
            //}
            //if (d.EqualsPos(Q_SW)){
            //    if (IsPCoinciding(p,ExceptionA,pmap) || IsPCoinciding(p,ExceptionB,pmap) || IsPCoinciding(p,ExceptionC,pmap) || IsPCoinciding(p,ExceptionD,pmap) || IsPCoinciding(p,ExceptionF,pmap)){
            //        return false;
            //    }
            //}
            //if (d.EqualsPos(Q_NW)){
            //    if (IsPCoinciding(p,ExceptionA,pmap) || IsPCoinciding(p,ExceptionB,pmap) || IsPCoinciding(p,ExceptionD,pmap) || IsPCoinciding(p,ExceptionE,pmap) || IsPCoinciding(p,ExceptionF,pmap)){
            //        return false;
            //    }
            //}
            //if (d.EqualsPos(Q_SE)){
            //    if (IsPCoinciding(p,ExceptionA,pmap) || IsPCoinciding(p,ExceptionB,pmap) || IsPCoinciding(p,ExceptionC,pmap) || IsPCoinciding(p,ExceptionD,pmap) || IsPCoinciding(p,ExceptionE,pmap)){
            //        return false;
            //    }
            //}            

            return true;
        }



        public Pixel[,] Thin_SI_Q_Isthmus(P[] Q, List<Pixel> pixelsList)
        {
            //var Y = CopyMap(boolMap);

            // var Yset = CreatePHashSet(Ymap);
            bool[] ProcessedBool;
            P[] AllPixels;
            //CreatePProcessedMap(Ymap, out AllPixels, out ProcessedBool);
            int[,] Indices;
            var Ymap = CreatePMap(boolMap, out AllPixels, out ProcessedBool, out Indices);
            int n = ProcessedBool.Length;
            int nD;
            //HashSet<P> I = new HashSet<P>();
            bool[] I = new bool[n];

            int[,] removedNum = new int[boolMap.GetLength(0), boolMap.GetLength(1)];
            //P[,] removedDir = new P[boolMap.GetLength(0), boolMap.GetLength(1)];
            int counter = 0;

            do
            {
                //LinkedList<P> D=new LinkedList<P>();
                nD = 0;
                foreach (var d in Q)
                {
                    var Dd = new List<P>();
                    //var B = new List<P>();//B = {p | p 6∈ I and p is a border point in Y }
                    for (int i = 0; i < n; i++)
                    {
                        if (!ProcessedBool[i])
                        {
                            var p = AllPixels[i];
                            if (IsBorder(p, Ymap) && !I[Indices[p.x, p.y]])//!I.Contains(p))
                            {
                                //B.Add(p);

                                if (IsSimple(p, Ymap))
                                {
                                    if (IsSI_d_deletable(p, d, Ymap))
                                    {
                                        Dd.Add(p);
                                        //AddToOppositeFrom(p,d,removedNum);
                                        //removedDir[p.x, p.y] = d;
                                    }
                                }
                                else
                                {
                                    I[Indices[p.x, p.y]] = true;
                                    //I.Add(p);
                                }

                            }
                        }
                    }
                    //foreach (var p in Yset)
                    //{

                    //    if (IsBorder(p, Ymap) && !I.Contains(p))
                    //    {
                    //        //B
                    //        B.Add(p);
                    //    }
                    //}
                    //var S = new List<P>();//S = {p | p ∈ B and p is a simple point in Y }
                    //var BnotS = new List<P>();
                    //foreach (var p in B)
                    //{
                    //    if (p.x + mapOrigX == 157 && p.y + mapOrigY == 309)
                    //    {
                    //    }
                    //    if (IsSimple(p, Ymap))
                    //    {
                    //        S.Add(p);
                    //    }
                    //    else
                    //    {
                    //        BnotS.Add(p);
                    //    }
                    //}
                    //foreach (var p in BnotS)//I = I ∪ (B \ S)
                    //{
                    //    I.Add(p);
                    //}
                    //var Dd = new List<P>();//Dd = {p | p ∈ S and p is SI-d-deletable in Y }
                    //foreach (var p in S)
                    //{
                    //    //if (p.x + mapOrigX == 126 && p.y + mapOrigY == 297)
                    //    //{
                    //    //}

                    //    if (IsSI_d_deletable(p, d, Ymap))
                    //    {
                    //        Dd.Add(p);
                    //    }
                    //}
                    foreach (var p in Dd)//Y = Y \ Dd
                    {
                        //Yset.Remove(p);
                        ProcessedBool[Indices[p.x, p.y]] = true;
                        Ymap[p.x, p.y].value = false;


                        //AddToOppositeFrom(p, removedDir[p.x, p.y], removedNum);
                        pixelMap[p.x, p.y].SkeletonPixelsRemovedNum = counter;
                        removedNum[p.x, p.y] = counter;
                        //pixelMap[p.x, p.y].SkeletonPixelsRemovedNum = counter;
                        //int max = 0;
                        //foreach (var dd in Q_W_SW_S_SE_E_NE_N_NW)
                        //{
                        //    if (Ofset(dd.x, dd.y, p))
                        //    {
                        //        //pixelMap[p.x+dd.x, p.y+dd.y].SkeletonPixelsRemovedNum ++;
                        //        int rn=pixelMap[p.x+dd.x, p.y+dd.y].SkeletonPixelsRemovedNum ;
                        //        if (rn> max)
                        //        {
                        //            max = rn;
                        //        }
                        //    }
                        //}
                        //pixelMap[p.x, p.y].SkeletonPixelsRemovedNum = max+1;

                        //removedNum[p.x, p.y] = counter;

                        //D.AddLast(
                        nD++;
                    }

                    //D = D ∪ Dd
                }
                counter++;
            } while (nD > 0);
            int w = Ymap.GetLength(0);
            int h = Ymap.GetLength(1);
            //var Y = new bool[w,h];
            if (pixelsList != null)
            {
                pixelsList.Clear();
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    //Y[i, j] = Ymap[i, j].value;
                    if (!Ymap[i, j].value)
                    {
                        if (pixelMap[i, j] != null) { }
                        pixelMap[i, j] = null;
                    }
                    if (pixelMap[i, j] != null)
                    {
                        if (pixelsList != null)
                        {
                            pixelsList.Add(pixelMap[i, j]);
                            pixelMap[i, j].IsSkeletonPixel = true;
                            //pixelMap[i, j].SkeletonPixelsRemovedNum = counter;

                            int max = 0;
                            foreach (var dd in Q_W_SW_S_SE_E_NE_N_NW)
                            {
                                if (Ofset(dd.x, dd.y, i, j))
                                {
                                    //pixelMap[p.x+dd.x, p.y+dd.y].SkeletonPixelsRemovedNum ++;
                                    int rn = removedNum[i + dd.x, j + dd.y];
                                    if (rn > max)
                                    {
                                        max = rn;
                                    }
                                }
                            }
                            pixelMap[i, j].SkeletonPixelsRemovedNum = max + 1;

                        }
                    }
                }
            }
            return pixelMap;
            //return CreatePixelsMap(Y, pixelMap); ;
        }

       // static Random random = new Random();

        bool SkipQ(double x)
        {
            // double th = -0.8 + 0.976666 * w - 0.175 * w * w + 0.008333333 * w * w * w;
            double th;
            {
                //double a = -0.05508;
                //double b = 4.6934;
                //double c = 1.7773;
                //double d = 0.8190;
                //th = d + (a - d) / (1 + Math.Pow(x / c, b));
                th = -0.8 + 0.976666 * x - 0.175 * x * x + 0.008333333 * x * x * x;
            }

            if (random.NextDouble() <= th)
            {
                return true;
            }
            return false;

        }



        public Pixel[,] Thin_SI_Q_IsthmusCorrected(P[] Q, List<Pixel> pixelsList)
        {

            bool[] ProcessedBool;
            P[] AllPixels;
            int[,] Indices;
            var Ymap = CreatePMap(boolMap, out AllPixels, out ProcessedBool, out Indices);
            int n = ProcessedBool.Length;
            int nD;
            bool[] I = new bool[n];

            int[,] removedNum = new int[boolMap.GetLength(0), boolMap.GetLength(1)];
            int counter = 0;

            Random r = new Random(0xaa55);
            //var toDelete = new Dictionary<P, List<P>>();
            //foreach (var d in Q)
            //{
            //    toDelete.Add(d, new List<P>());
            //}
            do
            {
                //LinkedList<P> D=new LinkedList<P>();
                nD = 0;

                //Create weight distribution
                //var edgePoints = new List<P>();
                var edgeDict = new Dictionary<P, List<P>>();
                foreach (var d in Q)
                {
                    edgeDict.Add(d, new List<P>());
                }
                foreach (var d in Q)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (!ProcessedBool[i])
                        {
                            var p = AllPixels[i];
                            if (IsBorder(p, Ymap) && !I[Indices[p.x, p.y]])//!I.Contains(p))
                            {
                                //B.Add(p);

                                if (IsSimple(p, Ymap))
                                {
                                    if (IsSI_d_deletable(p, d, Ymap))
                                    {
                                        //edgePoints.Add(p);
                                        edgeDict[d].Add(p);
                                    }
                                }
                            }
                        }
                    }
                }
                //var weightDistribution = edgePoints.Select(x => pixelMap[x.x, x.y].Area3D).OrderBy(y=>y).ToArray();

                var dirWeights = Q.Select(d => edgeDict[d].Select(x => pixelMap[x.x, x.y].Area3D).Mean(y => y)).Select(z => Math.Round(z, 4)).ToArray();
                //var dirWeightsOrdered=dirWeights.OrderBy(x=>x).ToArray();
                //var dirindex = r.Next(dirWeights.Count());
                //var minw = dirWeights.Min();

                var opposites = Q.Select(d => Q.WhereIndexFirst(od => od.x == -d.x && od.y == -d.y)).ToArray();

                //foreach (var d in Q)
                for (int di = 0; di < Q.Length; di++)
                {
                    var d = Q[di];
                    var opposited = dirWeights[opposites[di]];
                    // if (dirWeights[di] > dirWeightsOrdered[dirindex])

                    //don't skip if weights are all the same
                    //if (!dirWeights.All(x => x == dirWeights[0]))
                    {

                        //don't skip if the actual has the lowest weight
                        if (dirWeights[di] != opposited)
                        {
                            double x = dirWeights[di] / opposited;

                            //if (r.NextDouble()<=(x-1)/x)
                            if (r.NextDouble() <= (x) / (x + 1))
                            //if (di == Utilities.RandomUsingWeights(new double[]{minw,dirWeights[di]}))
                            //if (r.NextDouble()<=dirWeights[di]/(dirWeights[di]+minw))
                            //if (SkipQ(x))
                            {
                                nD++;//do not end thinning 
                                continue;
                            }
                        }
                    }



                    var Dd = new List<P>();
                    //var B = new List<P>();//B = {p | p 6∈ I and p is a border point in Y }
                    for (int i = 0; i < n; i++)
                    {
                        if (!ProcessedBool[i])
                        {
                            var p = AllPixels[i];
                            if (IsBorder(p, Ymap) && !I[Indices[p.x, p.y]])//!I.Contains(p))
                            {
                                //B.Add(p);

                                if (IsSimple(p, Ymap))
                                {
                                    if (IsSI_d_deletable(p, d, Ymap))
                                    {
                                        Dd.Add(p);
                                        //AddToOppositeFrom(p,d,removedNum);
                                        //removedDir[p.x, p.y] = d;
                                    }
                                }
                                else
                                {
                                    I[Indices[p.x, p.y]] = true;
                                    //I.Add(p);
                                }

                            }
                        }
                    }
                    //toDelete[d].AddRange(Dd);
                    //var deleted = new List<P>(); 
                    //var rindex = r.Next(edgePoints.Count);
                    //foreach (var p in toDelete[d])//Y = Y \ Dd
                    foreach (var p in Dd)//Y = Y \ Dd
                    {


                        //if (edgePoints.Count==0 || pixelMap[p.x, p.y].Area3D <= pixelMap[edgePoints[rindex].x, edgePoints[rindex].y].Area3D)
                        {
                            ProcessedBool[Indices[p.x, p.y]] = true;
                            Ymap[p.x, p.y].value = false;


                            pixelMap[p.x, p.y].SkeletonPixelsRemovedNum = counter;
                            removedNum[p.x, p.y] = counter;
                            nD++;
                            // deleted.Add(p);
                        }
                    }

                    //foreach (var pp in deleted)
                    //{
                    //    toDelete[d].Remove(pp);
                    //}

                    //D = D ∪ Dd
                }
                counter++;
            } while (nD > 0);
            int w = Ymap.GetLength(0);
            int h = Ymap.GetLength(1);
            if (pixelsList != null)
            {
                pixelsList.Clear();
            }
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    if (!Ymap[i, j].value)
                    {
                        if (pixelMap[i, j] != null) { }
                        pixelMap[i, j] = null;
                    }
                    if (pixelMap[i, j] != null)
                    {
                        if (pixelsList != null)
                        {
                            pixelsList.Add(pixelMap[i, j]);
                            pixelMap[i, j].IsSkeletonPixel = true;
                            int max = 0;
                            foreach (var dd in Q_W_SW_S_SE_E_NE_N_NW)
                            {
                                if (Ofset(dd.x, dd.y, i, j))
                                {
                                    int rn = removedNum[i + dd.x, j + dd.y];
                                    if (rn > max)
                                    {
                                        max = rn;
                                    }
                                }
                            }
                            pixelMap[i, j].SkeletonPixelsRemovedNum = max + 1;

                        }
                    }
                }
            }
            return pixelMap;
        }


        bool IsSimple_old(P p, P[,] pmap, P? exclude = null)
        {
            var border = IsBorder(p, pmap);
            //return border && !IsArticulationPoint(p,pmap);
            if (!border)
                return false;

            int changes = 0;
            bool val;
            bool actual = Ofset(p, Q_W_SW_S_SE_E_NE_N_NW[0], pmap);
            bool first = true;
            //List<P?> ppp = new List<P?>(9);
            int n = exclude.HasValue ? 8 : 9;
            int j = 0;
            int i = 0;
            //for (int j = 0; j < 9; j++)
            while (j < n)
            {
                var pp = OfsetPoint(p, Q_W_SW_S_SE_E_NE_N_NW[i % 8], pmap);

                //j++;
                //ppp.Add(pp);
                if (pp.HasValue)
                {
                    val = pp.Value.value;
                    if (exclude.HasValue)
                    {
                        //if (exclude.Value.EqualsP(pp.Value))
                        if (exclude.Value.EqualsPos(pp.Value))
                        {
                            //if (j % 8 == 0)
                            //    actual = Q_W_SW_S_E_SE_E_NE_N_NW[(j + 1) % 8].value;

                            //i++;
                            //continue;
                            val = false;
                            //pp = new P(pp.Value.x, pp.Value.y, false);
                            //pp.Value.value = false;
                        }
                    }
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        if (val)
                        {
                            if (!actual)
                            {
                                changes++;
                            }
                        }
                        else
                        {
                            if (actual)
                            {
                                changes++;
                            }
                        }
                        //n++;
                    }
                    actual = val;// pp.Value.value;
                    j++;
                    i++;
                }
                else
                {
                }
            }
            if (changes <= 2)
            {
                return true;
            }
            else
            {
                return false;
            }
            //return changes <= 2;

            //var n8ps=N8pStar(p,pmap,exclude);
            //return border && (n8ps == 1);
            //return border && IsArticulationPoint(p,pmap);
            //return IsBorder(p, pmap) && (N8Star.Count((p2) => IsBlackComponent(OfsetPoint(p2, p, pmap).Value, pmap, exclude))==1);
        }
    }

    public static class ThinnerUtils
    {
        public static int WhereIndexFirst<TSource>(this IEnumerable<TSource> list, Func<TSource, bool> selector)
        {
            int i = 0;
            foreach (var l in list)
            {
                if (selector(l))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        public static double Mean<TSource>(this IEnumerable<TSource> list, Func<TSource, double> selector)
        {
            double sum = 0;
            int n = 0;
            foreach (var l in list)
            {
                var v = selector(l);
                if (double.IsNaN(v)) { continue; }
                sum += v;
                n++;
            }
            if (n == 0) return double.NaN;
            return sum / n;
        }


    }
}
