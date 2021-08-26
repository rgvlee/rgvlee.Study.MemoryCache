using BenchmarkDotNet.Running;

namespace ConsoleApp1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<SingleThreadBenchmarks>();
            BenchmarkRunner.Run<MultipleThreadBenchmarks>();
        }
    }
}