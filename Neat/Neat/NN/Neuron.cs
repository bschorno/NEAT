using System;
using System.Collections.Generic;

namespace Neat.NN
{
    public class Neuron
    {
        private readonly List<Dendrite> _dendrites;
        private double _bias;
        private double _delta;
        private double _value;

        /// <summary>
        /// List of dendrites
        /// </summary>
        public List<Dendrite> Dendrites
        {
            get
            {
                return this._dendrites;
            }
        }

        /// <summary>
        /// Bias
        /// </summary>
        public double Bias
        {
            get
            {
                return this._bias;
            }
            set
            {
                this._bias = value;
            }
        }

        /// <summary>
        /// Delta
        /// </summary>
        public double Delta
        {
            get
            {
                return this._delta;
            }
            set
            {
                this._delta = value;
            }
        }

        /// <summary>
        /// Neuron value
        /// </summary>
        public double Value
        {
            get
            {
                return this._value;
            }
            set
            {
                this._value = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Neuron()
        {
            Random n = new Random(Environment.TickCount);
            this._bias = n.NextDouble();
            this._dendrites = new List<Dendrite>();
        }
    }
}
