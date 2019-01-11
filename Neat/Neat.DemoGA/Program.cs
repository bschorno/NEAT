using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Neat.NN;
using Neat.GA;

namespace Neat.DemoGA
{
    class Program
    {
        public static NeuralNetwork network;

        static void Main(string[] args)
        {
            network = new NeuralNetwork(new int[] { 2, 2, 1 });

            GeneticAlgorithm ga = new GeneticAlgorithm(0.50, 0.01, 100, 200, 12);
            ga.FitnessFunction = new GAFunction(FitnessFunction);
            ga.Elitism = true;
            ga.Start();

            double[] weights;
            double fitness;
            ga.GetBest(out weights, out fitness);
            Console.WriteLine("Best brain had a fitness of " + fitness);

            SetNetworkWeights(weights);

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

        public static void SetNetworkWeights(double[] weights)
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

        public static double FitnessFunction(double[] weights)
        {
            double fitness = 0;

            SetNetworkWeights(weights);

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
    }
}
