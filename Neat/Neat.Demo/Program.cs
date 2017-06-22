using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Neat.NN;
using Neat.GA;

namespace Neat.Demo
{
    class Program
    {
        public static NeuralNetwork network;

        public static void setNetworkWeights(NeuralNetwork aNetwork, double[] weights)
        {
            int index = 0;

            foreach (Layer layer in network.Layers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    foreach (Dendrite dendrite in neuron.Dendrites)
                    {
                        dendrite.Weight = weights[index++];
                    }
                    neuron.Bias = weights[index++];
                }
            }
        }

        public static double fitnessFunction(double[] weights)
        {
            double fitness = 0;

            setNetworkWeights(network, weights);

            // AND
            double output = network.Run(new List<double>(new double[2] { 0, 0 }))[0];
            // The closest the output is to zero, the more fit it is.
            fitness += 1 - output;

            output = network.Run(new List<double>(new double[2] { 0, 1 }))[0];
            // The closest the output is to zero, the more fit it is.
            fitness += 1 - output;

            output = network.Run(new List<double>(new double[2] { 1, 0 }))[0];
            // The closest the output is to zero, the more fit it is.
            fitness += 1 - output;

            output = network.Run(new List<double>(new double[2] { 1, 1 }))[0];
            // The closest the output is to one, the more fit it is.
            fitness += output;

            return fitness;
        }

        static void Main(string[] args)
        {
            network = new NeuralNetwork(new int[] { 2, 2, 1 });

            GeneticAlgorithm ga = new GeneticAlgorithm(0.50, 0.01, 100, 2000, 12);
            ga.FitnessFunction = new GAFunction(fitnessFunction);
            ga.Elitism = true;
            ga.Start();

            double[] weights;
            double fitness;
            ga.GetBest(out weights, out fitness);
            Console.WriteLine("Best brain had a fitness of " + fitness);

            setNetworkWeights(network, weights);

            double input1;
            string strInput1 = "";
            while (strInput1 != "q")
            {
                Console.Write("Input 1: ");
                strInput1 = Console.ReadLine().ToString();

                if (strInput1 != "q")
                {
                    input1 = Convert.ToDouble(strInput1);
                    if (input1 != 'q')
                    {
                        Console.Write("Input 2: ");
                        double input2 = Convert.ToDouble(Console.ReadLine().ToString());
                        double[] output = network.Run(new List<double>(new double[2] { input1, input2 }));
                        Console.WriteLine("Output: " + output[0].ToString("0.##########"));
                    }
                }
            }
        }
    }
}
