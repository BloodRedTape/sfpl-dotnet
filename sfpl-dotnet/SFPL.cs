using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SFPL
{
    public struct OutputParameters
    {
        public int ImageWidth;
        public int ImageHeight;
        public string PlotTitle;
        public string XAxisName;
        public string YAxisName;

        public OutputParameters(int imageWidth = 1280, int imageHeight = 720, string plotTitle = "", string xAxisName = "", string yAxisName = "")
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            PlotTitle = plotTitle;
            XAxisName = xAxisName;
            YAxisName = yAxisName;
        }
    }
    public struct DataSource
    {
        public double[] X;
        public double[] Y;
        public string Name;
    }

    public class PlotBuilder
    {
        public static unsafe bool Build(DataSource[] sources, string outFilePath, OutputParameters parameters)
        {
            var xs = new double *[sources.Length];
            var ys = new double *[sources.Length];
            var sizes = new int[sources.Length];
            var names = new string[sources.Length];

            var dataHandles = new List<GCHandle>();

            for(var i = 0; i<sources.Length; i++)
            {
                var xHandle = GCHandle.Alloc(sources[i].X, GCHandleType.Pinned);
                var yHandle = GCHandle.Alloc(sources[i].Y, GCHandleType.Pinned);
                dataHandles.Add(xHandle);
                dataHandles.Add(yHandle);
                
                xs[i] = (double*)xHandle.AddrOfPinnedObject();
                ys[i] = (double*)yHandle.AddrOfPinnedObject();
                sizes[i] = Math.Min(sources[i].X.Length, sources[i].Y.Length);
                names[i] = sources[i].Name;
            }

            bool result = PlotBuilder_BuildImplNative(xs, ys, sizes, names, sources.Length, outFilePath, 
                parameters.ImageWidth, parameters.ImageHeight, parameters.PlotTitle, parameters.XAxisName, parameters.YAxisName);

            foreach (var handle in dataHandles)
                handle.Free();
            
            return result;
        }

        public static bool Build(DataSource source, string outFilePath, OutputParameters parameters)
        {
            return Build(new []{source}, outFilePath, parameters);
        }

        [DllImport("./sfpl", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe bool PlotBuilder_BuildImplNative(
            double*[] xs,
            double*[] ys, 
            int[] sizes,
            string[] names, 
            int count, 
            string outFilePath, 
            int imageWidth, int imageHeight, 
            string plotTitle, 
            string xAxisName, 
            string yAxisName);
    }
}
