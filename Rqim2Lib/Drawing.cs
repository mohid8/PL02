using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rqim2Lib
{
    class Drawing
    {
        public static void DrawLinesToFile(string filename, Tuple<PointF, PointF, float>[] lines, int width, int height, float thicknessMultiplier = 0.010f)
        {
            using (var image = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppRgb))
            {
                using (var g = Graphics.FromImage(image))
                {
                    g.FillRectangle(Brushes.White, 0, 0, width, height);
                    g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    foreach (var l in lines)
                    {
                        var x1 = (int)Math.Round((l.Item1.X + 0.5f) * width);
                        var y1 = (int)Math.Round((l.Item1.Y + 0.5f) * height);
                        var x2 = (int)Math.Round((l.Item2.X + 0.5f) * width);
                        var y2 = (int)Math.Round((l.Item2.Y + 0.5f) * height);
                        // sb.AppendLine(string.Format("{0}\t{1}\t{2}\t{3}", x1, y1, x2, y2));
                        using (var pen = new Pen(Color.Black, Math.Min(255, 1 + (int)Math.Round(thicknessMultiplier * width * l.Item3))))
                        {
                            g.DrawLine(pen, x1, y1, x2, y2);
                        }
                       
                    }
                }
                image.Save(filename);
            }


        }
    }
}
