using System;
using System.Collections.Generic;

namespace Neat.NN
{
    public class Layer
    {
        private readonly List<Neuron> _neurons;

        /// <summary>
        /// List of neurons in this layer
        /// </summary>
        public List<Neuron> Neurons
        {
            get
            {
                return this._neurons;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numNeurons"></param>
        public Layer(int numNeurons)
        {
            this._neurons = new List<Neuron>(numNeurons);
        }
    }
}
