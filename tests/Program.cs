using System;
using SFPL;

class Program
{
    static void Main(string[] args)
    {
        const int size = 10;
        
        DataSource source = new DataSource();
        source.X = new double[size];
        source.Y = new double[size];

        for (int i = 0; i < size; i++)
        {
            source.X[i] = i;
            source.Y[i] = i * i;
        }

        var res = PlotBuilder.Build(source, "test.jpg", new OutputParameters(1280, 720, "Test"));

        Console.WriteLine(res ? "Done" : "Error");
    }
}