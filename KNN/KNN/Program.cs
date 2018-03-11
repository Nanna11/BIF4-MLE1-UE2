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
            knn k = new knn("bezdekIris.data", ",", 4, 10, false, ".");
            Console.ReadKey();
        }
    }
}
