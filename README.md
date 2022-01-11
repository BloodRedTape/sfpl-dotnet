# SFPL.Net - Simple and Fast Plotting Library port for dotnet
### Png, jpg and tga output image formats are supported
***
## Usage
## Examples

### Minimum amount of code to create a simple plot:
```c#
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

        PlotBuilder.Build(source, "parabola.jpg", new OutputParameters(1280, 720));
    }
}

```
Expected result:
![](https://github.com/BloodRedTape/sfpl/blob/master/examples/parabola.jpg?raw=true)

### Array of DataSource can be passed in order to plot multiple data sets
```c#

    DataSource[] sources = new DataSource[size];
    //... code to fill sources array
    PlotBuilder.Build(sources, "test.jpg", new OutputParameters(1280, 720));

```
### Each trace can be provided with a name

```c#
    DataSource source = new DataSource(); 
    //... code to fill source structure
    source.Name = "Source Name";

```

### Axises names and plot title can be passed as OutputParameters

```c#

    PlotBuilder.Build(sources, "test.jpg", new OutputParameters(1280, 720, "PlotTitle", "XAxisName", "YAxisName"));

```

### Combining this techniques can lead to something like this
***

![](https://github.com/BloodRedTape/sfpl/blob/master/examples/sort.jpg?raw=true)

## Original library intention was to plot operation benchmarks

It can be done somehow like this

```c#
using System.Diagnostics;
using SFPL;

class Program
{
    static void HeavyOperation(int n)
    {
        ++n;
        for (int i = 0; i < n; i++)
        {
            char[] array = new char[n * 20];
            for (int j = 0; j < n * 20; j++)
                array[j] = 'a';
        }
    }

    static void Main(string[] args)
    {
        const int BenchmarkSize = 1000;

        double[] x = new double[BenchmarkSize];
        double[] y = new double[BenchmarkSize];


        Stopwatch sw = new Stopwatch();
        for (int j = 0; j < BenchmarkSize; ++j)
        {

            sw.Start();
            HeavyOperation(j);
            sw.Stop();

            x[j] = j;
            y[j] = sw.ElapsedTicks;
            sw.Reset();
        }

        OutputParameters parameters = new OutputParameters();
        parameters.ImageWidth = 1280;
        parameters.ImageHeight = 720;
        parameters.PlotTitle = "Heavy Operation Test";
        parameters.XAxisName = "Heavy operation size";
        parameters.YAxisName = "Time [ns]";

        DataSource source = new DataSource();
        source.X = x;
        source.Y = y;

        PlotBuilder.Build(source, "heavy_test.jpg",  parameters);
    }
}

```

Possible Result:
![](https://github.com/BloodRedTape/sfpl/blob/master/examples/heavy_test.jpg?raw=true)
