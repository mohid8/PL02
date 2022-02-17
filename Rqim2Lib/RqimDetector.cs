using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    public class RqimDetector
    {

        public static Rqim FromImage(string filename)
        {
            var quads = RqimOpenCV.CvDetector.LoadCorners(filename);
            if (quads != null)
            {
                Rqim rqim = new Rqim();
                foreach (var q in quads)
                {
                    rqim.AddQuad(new Quad() { Corners = q.ToArray() });
                }
                rqim.FixWinding();
                return rqim;
            }
            return null;
        }


    }
}
