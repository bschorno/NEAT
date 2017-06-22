using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neat.NN
{
    public class NeuralNetwork
    {
        private readonly List<Layer> _layers;

        /// <summary>
        /// Layers
        /// </summary>
        public List<Layer> Layers
        {
            get
            {
                return this._layers;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="learningRate"></param>
        /// <param name="layers"></param>
        public NeuralNetwork(int[] layers)
        {
            if (layers.Length < 2)
                return;

            this._layers = new List<Layer>();

            for (int i = 0; i < layers.Length; i++)
            {
                Layer layer = new Layer(layers[i]);
                this.Layers.Add(layer);

                for (int j = 0; j < layers[i]; j++)
                    layer.Neurons.Add(new Neuron());

                layer.Neurons.ForEach((neuron) =>
                {
                    if (i == 0)
                        neuron.Bias = 0;
                    else
                        for (int k = 0; k < layers[i - 1]; k++)
                            neuron.Dendrites.Add(new Dendrite());
                });
            }
        }

        /// <summary>
        /// Sigmoid function
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private double Sigmoid(double x)
        {
            return 1 / (1 + Math.Exp(-x));
        }

        /// <summary>
        /// Run network
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public double[] Run(List<double> input)
        {
            if (input.Count != this._layers[0].Neurons.Count)
                throw new ArgumentException("The number of inputs must match the number of neurons in the first layer");

            for (int i = 0; i < this._layers.Count; i++)
            {
                Layer layer = this._layers[i];

                for (int j = 0; j < layer.Neurons.Count; j++)
                {
                    Neuron neuron = layer.Neurons[j];

                    if (i == 0)
                        neuron.Value = input[j];
                    else
                    {
                        neuron.Value = 0;
                        for (int k = 0; k < this._layers[i - 1].Neurons.Count; k++)
                            neuron.Value = neuron.Value + this._layers[i - 1].Neurons[k].Value * neuron.Dendrites[k].Weight;

                        neuron.Value = this.Sigmoid(neuron.Value + neuron.Bias);
                    }
                }
            }

            Layer last = this.Layers[this._layers.Count - 1];
            double[] output = new double[last.Neurons.Count];
            for (int i = 0; i < last.Neurons.Count; i++)
                output[i] = last.Neurons[i].Value;

            return output;
        }

        /// <summary>
        /// Train network
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        public bool Train(List<double> input, List<double> output, double learningRate)
        {
            if (input.Count != this._layers[0].Neurons.Count)
                throw new ArgumentException("The number of inputs must match the number of neurons in the first layer");
            if (output.Count != this._layers[this._layers.Count - 1].Neurons.Count)
                throw new ArgumentException("The number of output must match the number of neurons in the last layer");

            this.Run(input);

            for (int i = 0; i < this._layers[this._layers.Count - 1].Neurons.Count; i++)
            {
                Neuron neuron = this._layers[this._layers.Count - 1].Neurons[i];

                neuron.Delta = neuron.Value * (1 - neuron.Value) * (output[i] - neuron.Value);

                for (int j = this._layers.Count - 2; j > 2; j--)
                {
                    for (int k = 0; k < this._layers[j].Neurons.Count; k++)
                    {
                        Neuron n = this._layers[j].Neurons[k];

                        n.Delta = n.Value *
                                  (1 - n.Value) *
                                  this._layers[j + 1].Neurons[i].Dendrites[k].Weight *
                                  this._layers[j + 1].Neurons[i].Delta;
                    }
                }
            }

            for (int i = this._layers.Count - 1; i > 1; i--)
            {
                for (int j = 0; j < this._layers[i].Neurons.Count; j++)
                {
                    Neuron n = this._layers[i].Neurons[j];
                    n.Bias += learningRate * n.Delta;

                    for (int k = 0; k < n.Dendrites.Count; k++)
                        n.Dendrites[k].Weight += learningRate * this._layers[i - 1].Neurons[k].Value * n.Delta;
                }
            }

            return true;
        }
    }
}
