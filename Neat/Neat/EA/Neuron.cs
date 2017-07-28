using System;
using System.Collections.Generic;

namespace Neat.EA
{
    public class Neuron
    {
        private List<Gene> _incomings = new List<Gene>();
        private double _value;

        /// <summary>
        /// Incomings genes
        /// </summary>
        public List<Gene> Incomings
        {
            get
            {
                return this._incomings;
            }
            set
            {
                this._incomings = value;
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
            
        }
    }
}
