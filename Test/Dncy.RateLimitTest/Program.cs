// See https://aka.ms/new-console-template for more information

using Dncy.RateLimit.Core;
using Dncy.RateLimit.Core.Rules;
using Dncy.RateLimit.MemoryCache.Algorithms;


//var al = new FixedWindowAlgorithm(new FixedWindowRule
//    {
        
//    }
//);
////Task.Run(() =>
////{
////    for (int i = 0; i < 100; i++)
////    {
////        Thread.Sleep(TimeSpan.FromSeconds(10));
////        var res = al.Check(new LimitCheckContext {Target = "hello"});
////        Console.WriteLine($"t1:{res.Passed}");
////    }
////});
//Task.Run(() =>
//{
//    for (int i = 0; i < 100; i++)
//    {
//        var res = al.Check(new LimitCheckContext("123"));
//        Thread.Sleep(TimeSpan.FromSeconds(1));
//        Console.WriteLine($"t2:{res.Passed}");
//    }
//});

Console.WriteLine("Hello, World!");

Console.Read();
