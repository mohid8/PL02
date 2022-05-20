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
    class Program
    {
        static void Main(string[] args)
        {
            Mat src = new Mat("C:/Users/Mohid/source/repos/rqim-mohid/images/rqim10.png", ImreadModes.Color);
            Mat gray = src.CvtColor(ColorConversionCodes.BGR2GRAY);
            Mat binary = gray.Threshold(0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);                                
            Mat labelView = src.EmptyClone();
            Mat rectView = binary.CvtColor(ColorConversionCodes.GRAY2BGR);

            ConnectedComponents cc = Cv2.ConnectedComponentsEx(binary);
            
            if (cc.LabelCount <= 1)
                return;

            // draw labels
            cc.RenderBlobs(labelView);

            // draw buonding boxes except background
            foreach (var blob in cc.Blobs.Skip(1))
            {
                rectView.Rectangle(blob.Rect, Scalar.Red);
            }

            // filter maximum blob
            var maxBlob = cc.GetLargestBlob();
            var filtered = new Mat();
            cc.FilterByBlob(binary, filtered, maxBlob);

            using (new Window("src", src))
            using (new Window("gray", gray))
            using (new Window("binary", binary))
            using (new Window("labels", labelView))
            using (new Window("bonding boxes", rectView))
            using (new Window("maximum blob", filtered))

            

            {
                Cv2.WaitKey();
            }
        }
    }
}
