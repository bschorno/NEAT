using System;

namespace Neat.EA
{
    public class Gene : IEquatable<Gene>
    {
        private EvolutionaryAlogorithm _ea;
        private int _into;
        private int _out;
        private double _weight;
        private bool _enable;
        private int _innovation;

        /// <summary>
        /// Into
        /// </summary>
        public int Into
        {
            get
            {
                return this._into;
            }
            set
            {
                this._into = value;
            }
        }

        /// <summary>
        /// Out
        /// </summary>
        public int Out
        {
            get
            {
                return this._out;
            }
            set
            {
                this._out = value;
            }
        }

        /// <summary>
        /// Weight
        /// </summary>
        public double Weight
        {
            get
            {
                return this._weight;
            }
            set
            {
                this._weight = value;
            }
        }

        /// <summary>
        /// Is enable
        /// </summary>
        public bool Enable
        {
            get
            {
                return this._enable;
            }
            set
            {
                this._enable = value;
            }
        }

        /// <summary>
        /// Innovation
        /// </summary>
        public int Innovation
        {
            get
            {
                return this._innovation;
            }
            set
            {
                this._innovation = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ea"></param>
        public Gene(EvolutionaryAlogorithm ea)
        {
            this._ea = ea;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gene"></param>
        public Gene(Gene gene)
            : this(gene._ea)
        {
            this._into = gene._into;
            this._out = gene._out;
            this._weight = gene._weight;
            this._enable = gene._enable;
            this._innovation = gene._innovation;
        }

        /// <summary>
        /// Check equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(Gene other)
        {
            return this._into == other._into && this._out == other._out;
        }
    }
}
