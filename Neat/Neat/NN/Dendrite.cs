using System;

namespace Neat.NN
{
    public class Dendrite
    {
        private double _weight;

        /// <summary>
        /// Weight of dendrite
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
        /// Constructor
        /// </summary>
        public Dendrite()
        {
            this._weight = new CryptoRandom().Value;
        }
    }

}
