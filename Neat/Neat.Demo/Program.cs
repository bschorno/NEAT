using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Neat.NN;
using Neat.GA;
using Neat.EA;

namespace Neat.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            int x1 = 6;
            int y1 = 0;
            int x2 = 5;
            int y2 = 1;

            double var = Distance(x1, y1, x2, y1);
            Console.WriteLine(var);
            Console.ReadLine();
        }

        private static double Angle(int x1, int y1, int x2, int y2)
        {
            return Math.Atan2(y2 - y1, x2 - x1);
        }

        private static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }
    }
}
