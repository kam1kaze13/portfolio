using System.Diagnostics;
using Marble.Core;

Stopwatch stopwatch = new Stopwatch();

stopwatch.Start();
GameSolver.SolveGames("AdvancedExample.txt");
stopwatch.Stop();

Console.WriteLine($"All time: {stopwatch.ElapsedMilliseconds} ms\n");
