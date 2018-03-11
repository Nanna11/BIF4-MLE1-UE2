using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNN
{
    class Program
    {
        static void Main(string[] args)
        {
            knn k = new knn("winequality-white.csv", ";", 11, 3, true, 2, ".");
            k.Test();
            k.DisplayConfusionMatrix();
            Console.ReadKey();
        }
    }
}
