using System;
using System.Security.Cryptography;

namespace Neat.NN
{
    public class CryptoRandom
    {
        private double _value;

        /// <summary>
        /// Random value
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
        public CryptoRandom()
        {
            using (RNGCryptoServiceProvider p = new RNGCryptoServiceProvider())
            {
                Random r = new Random(p.GetHashCode());
                this._value = r.NextDouble();
            }
        }

    }
}
