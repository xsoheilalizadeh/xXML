using System;

namespace xXML.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkDotNet.Running.BenchmarkRunner.Run<Benchmarks>();
        }
    }
}
