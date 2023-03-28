using BenchmarkDotNet.Running;
using EfCoreVsDapper;

Console.WriteLine("Starting benchmarks...");

BenchmarkRunner.Run<Benchmarks>();

Console.WriteLine("Finished benchmarks...");