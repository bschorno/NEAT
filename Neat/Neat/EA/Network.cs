using System;
using System.Collections.Generic;

namespace Neat.EA
{
    public class Network
    {
        private EvolutionaryAlogorithm _ea;
        private Neuron[] _neurons;

        /// <summary>
        /// Neurons
        /// </summary>
        public Neuron[] Neurons
        {
            get
            {
                return this._neurons;
            }
            set
            {
                this._neurons = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ea"></param>
        public Network(EvolutionaryAlogorithm ea, Genome genome)
        {
            this._ea = ea;

            this._neurons = new Neuron[this._ea.MaxNodes + this._ea.Outputs];

            for (int i = 0; i < this._ea.Inputs; i++)
                this._neurons[i] = new Neuron();
            for (int i = 0; i < this._ea.Outputs; i++)
                this._neurons[i + this._ea.MaxNodes] = new Neuron();

            genome.Genes.Sort(delegate (Gene g1, Gene g2)
            {
                if (g1.Out == g2.Out) return 0;
                else if (g1.Out > g2.Out) return 1;
                else if (g1.Out < g2.Out) return -1;
                else return 0;
            });

            foreach (Gene gene in genome.Genes)
            {
                if (!gene.Enable)
                    continue;
                if (this._neurons[gene.Out] == null)
                    this._neurons[gene.Out] = new Neuron();

                if (this._neurons[gene.Into] == null)
                    this._neurons[gene.Into] = new Neuron();

                this._neurons[gene.Out].Incomings.Add(gene);
            }
        }

        /// <summary>
        /// Run network
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Run(double[] input)
        {
            if (input.GetLength(0) != this._ea.Inputs - 1)
                throw new ArgumentException("The number of inputs must match the number of neurons in the input layer");
            
            double[] output = new double[this._ea.Outputs];

            for (int i = 0; i < input.GetLength(0); i++)
                this._neurons[i].Value = input[i];
            this._neurons[this._ea.Inputs - 1].Value = 1;

            foreach (Neuron neuron in this._neurons)
            {
                if (neuron == null)
                    continue;
                double value = 0d;
                foreach (Gene gene in neuron.Incomings)
                    value += gene.Weight * this._neurons[gene.Into].Value;

                if (neuron.Incomings.Count > 0)
                    neuron.Value = this.Sigmoid(value);
            }

            for (int i = 0; i < output.GetLength(0); i++)
                output[i] = this._neurons[i + this._ea.MaxNodes].Value;

            return output;
        }

        /// <summary>
        /// Sigmoid function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Sigmoid(double x)
        {
            return 2 / (1 + Math.Exp(-4.9d * x)) - 1;
        }
    }
}
