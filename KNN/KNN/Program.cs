using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace KNN
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            knn k = new knn("winequality-white.csv", ";", 11, true, 2, ".");
            Console.WriteLine("\nTime for initialization: {0}", s.Elapsed);
            s.Restart();
            k.Test(12,10);
            Console.WriteLine("\nTime for Testpackages: {0}", s.Elapsed);
            k.DisplayConfusionMatrix();
            s.Restart();
            for(int i = 0; i < 100; i++)
            {
                k.Classify("7;0,27;0,36;20,7;0,045;45;170;1,001;3;0,45;8,8", 10);
            }
            Console.WriteLine("\nTime for 100 classifications: {0}", s.Elapsed);
            s.Restart();
            for (int i = 0; i < 1000; i++)
            {
                k.Classify("7;0,27;0,36;20,7;0,045;45;170;1,001;3;0,45;8,8", 10);
            }
            Console.WriteLine("\nTime for 1000 classifications: {0}", s.Elapsed);
            s.Restart();
            for (int i = 0; i < 10000; i++)
            {
                k.Classify("7;0,27;0,36;20,7;0,045;45;170;1,001;3;0,45;8,8", 10);
            }
            Console.WriteLine("\nTime for 10000 classifications: {0}", s.Elapsed);
            Console.ReadKey();
        }
    }
}
