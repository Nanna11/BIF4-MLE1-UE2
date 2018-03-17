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
            //Console.WriteLine("**********************");
            //Console.WriteLine("       Iris Data      ");
            //Console.WriteLine("**********************");
            Stopwatch s = new Stopwatch();
            //s.Start();
            //knn k = new knn("bezdekIris.data", " ", 4, false, 2.5, ",");
            //Console.WriteLine("\nTime for initialization: {0}", s.Elapsed);
            //s.Restart();
            //k.Test(10, 10);
            //Console.WriteLine("\nTime for Testpackages: {0}", s.Elapsed);
            //k.DisplayConfusionMatrix();
            //s.Restart();
            //for (int i = 0; i < 100; i++)
            //{
            //    k.Classify("5,1 3,5 1,4 0,2", 10);
            //}
            //Console.WriteLine("\nTime for 100 classifications: {0}", s.Elapsed);
            //s.Restart();
            //for (int i = 0; i < 1000; i++)
            //{
            //    k.Classify("5,1 3,5 1,4 0,2", 10);
            //}
            //Console.WriteLine("\nTime for 1000 classifications: {0}", s.Elapsed);
            //s.Restart();
            //for (int i = 0; i < 10000; i++)
            //{
            //    k.Classify("5,1 3,5 1,4 0,2", 10);
            //}
            //Console.WriteLine("\nTime for 10000 classifications: {0}", s.Elapsed);


            Console.WriteLine("\n\n\n**********************");
            Console.WriteLine("       Wine Data      ");
            Console.WriteLine("**********************");
            s = new Stopwatch();
            s.Start();
            knn k2 = new knn("winequality-white.csv", ";", 11, true, 2.5, ".");
            Console.WriteLine("\nTime for initialization: {0}", s.Elapsed);
            s.Restart();
            k2.Test(10,10);
            Console.WriteLine("\nTime for Testpackages: {0}", s.Elapsed);
            k2.DisplayConfusionMatrix();
            s.Restart();
            for(int i = 0; i < 100; i++)
            {
                k2.Classify("7;0,27;0,36;20,7;0,045;45;170;1,001;3;0,45;8,8", 10);
            }
            Console.WriteLine("\nTime for 100 classifications: {0}", s.Elapsed);
            s.Restart();
            for (int i = 0; i < 1000; i++)
            {
                k2.Classify("7;0,27;0,36;20,7;0,045;45;170;1,001;3;0,45;8,8", 10);
            }
            Console.WriteLine("\nTime for 1000 classifications: {0}", s.Elapsed);
            s.Restart();
            for (int i = 0; i < 10000; i++)
            {
                k2.Classify("7;0,27;0,36;20,7;0,045;45;170;1,001;3;0,45;8,8", 10);
            }
            Console.WriteLine("\nTime for 10000 classifications: {0}", s.Elapsed);
            Console.ReadKey();
        }
    }
}
