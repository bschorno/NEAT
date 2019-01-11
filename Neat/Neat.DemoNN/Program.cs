using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Neat.NN;

namespace Neat.DemoNN
{
    class Program
    {
        static void Main(string[] args)
        {
            NeuralNetwork network = new NeuralNetwork(new int[] { 2, 2, 1 });

            for (int i = 0; i < 100000; i++)
            {
                network.Train(new List<double>() { 0, 0 }, new List<double>() { 0 }, 1);
                network.Train(new List<double>() { 1, 0 }, new List<double>() { 0 }, 1);
                network.Train(new List<double>() { 0, 1 }, new List<double>() { 0 }, 1);
                network.Train(new List<double>() { 1, 1 }, new List<double>() { 1 }, 1);
            }

            double[] result;
            result = network.Run(new List<double>() { 0, 0 });
            Console.WriteLine(result[0]);

            result = network.Run(new List<double>() { 1, 0 });
            Console.WriteLine(result[0]);

            result = network.Run(new List<double>() { 0, 1 });
            Console.WriteLine(result[0]);

            result = network.Run(new List<double>() { 1, 1 });
            Console.WriteLine(result[0]);

            Console.ReadLine();
        }
    }
}
