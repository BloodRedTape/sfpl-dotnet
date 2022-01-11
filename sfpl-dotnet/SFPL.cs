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
        private const string NativeDllPath = "./sfpl";
        public static unsafe void Build(DataSource[] sources, string outFilePath, OutputParameters parameters)
        {
            if (sources == null)
                throw new NullReferenceException("sources argument can't be null");
            
            if(sources.Length == 0)
                throw new Exception("Can't build a plot from zero sources");

            if (!IsValidFilePath(outFilePath))
                throw new Exception("Unsupported output image format, use .png .jpg or .tga");

            parameters = ValidateParameters(parameters);

            var xs = new double *[sources.Length];
            var ys = new double *[sources.Length];
            var sizes = new int[sources.Length];
            var names = new string[sources.Length];

            var dataHandles = new List<GCHandle>();

            for(var i = 0; i<sources.Length; i++)
            {
                var exception = IsSourceValid(sources[i]);

                if (exception != null)
                {
                    Release(dataHandles);
                    throw exception;
                }

                var xHandle = GCHandle.Alloc(sources[i].X, GCHandleType.Pinned);
                var yHandle = GCHandle.Alloc(sources[i].Y, GCHandleType.Pinned);
                dataHandles.Add(xHandle);
                dataHandles.Add(yHandle);
                
                xs[i] = (double*)xHandle.AddrOfPinnedObject();
                ys[i] = (double*)yHandle.AddrOfPinnedObject();
                sizes[i] = Math.Min(sources[i].X.Length, sources[i].Y.Length);
                
                names[i] = sources[i].Name ?? string.Empty;
            }

            var result = PlotBuilder_BuildImplNative(xs, ys, sizes, names, sources.Length, outFilePath, 
                parameters.ImageWidth, parameters.ImageHeight, 
                parameters.PlotTitle, 
                parameters.XAxisName, 
                parameters.YAxisName);

            Release(dataHandles);

            if (!result)
            {
                throw new Exception($"Can't save image at {outFilePath}");
            }
        }

        public static void Build(DataSource source, string outFilePath, OutputParameters parameters)
        {
            Build(new[] {source}, outFilePath, parameters);
        }

        [DllImport(NativeDllPath, CallingConvention = CallingConvention.StdCall)]
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

        private static bool IsValidFilePath(string path)
        {
            return !string.IsNullOrEmpty(path) && (path.EndsWith(".jpg") || path.EndsWith(".png") || path.EndsWith(".tga"));
        }

        private static OutputParameters ValidateParameters(OutputParameters parameters)
        {
            const int minImageSize = 50;
            
            return new OutputParameters(
                Math.Max(minImageSize, parameters.ImageWidth),
                Math.Max(minImageSize, parameters.ImageHeight),
                parameters.PlotTitle ?? string.Empty,
                parameters.XAxisName ?? string.Empty,
                parameters.YAxisName ?? string.Empty
            );
        }

        private static void Release(List<GCHandle> handles)
        {
            if (handles == null) return;
            
            foreach(var handle in handles)
                handle.Free();
        }

        private static Exception IsSourceValid(DataSource source)
        {
            if (source.X == null)
                return new Exception("source.X should not be null");

            if (source.X.Length < 2)
                return new Exception("source.X should have at least two elements");
                
            if (source.Y == null)
                return new Exception("source.Y should not be null");
            
            if (source.Y.Length < 2)
                return new Exception("source.Y should have at least two elements");

            return null;
        }
    }
}
