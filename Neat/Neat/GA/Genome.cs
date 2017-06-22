using System;
using System.Collections;
using System.Collections.Generic;

namespace Neat.GA
{
    public class Genome
    {
        private static Random random = new Random();

        public double[] _genes;
        private int _length;
        private double _fitness;
        private double _mutationRate;

        /// <summary>
        /// Genes
        /// </summary>
        public double[] Genes
        {
            get
            {
                return this._genes;
            }
        }

        /// <summary>
        /// Fitness
        /// </summary>
		public double Fitness
        {
            get
            {
                return this._fitness;
            }
            set
            {
                this._fitness = value;
            }
        }

        /// <summary>
        /// Mutation rate
        /// </summary>
		public double MutationRate
        {
            get
            {
                return this._mutationRate;
            }
            set
            {
                this._mutationRate = value;
            }
        }

        /// <summary>
        /// Length
        /// </summary>
        public int Length
        {
            get
            {
                return this._length;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
		public Genome(double mutationRate)
        {
            this._mutationRate = mutationRate;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length"></param>
		public Genome(int length, double mutationRate)
        {
            this._length = length;
            this._genes = new double[length];
            this._mutationRate = mutationRate;
            this.CreateGenes();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="length"></param>
        /// <param name="createGenes"></param>
		public Genome(int length, bool createGenes, double mutationRate)
        {
            this._length = length;
            this._genes = new double[length];
            this._mutationRate = mutationRate;
            if (createGenes)
                this.CreateGenes();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="genes"></param>
        public Genome(ref double[] genes, double mutationRate)
        {
            this._length = genes.Length;
            this._genes = new double[this._length];
            this._mutationRate = mutationRate;
            Array.Copy(genes, this._genes, this._length);
        }

        /// <summary>
        /// Depp copy gemone
        /// </summary>
        /// <returns></returns>
        public Genome DeepCopy()
        {
            Genome g = new Genome(this._length, false, this._mutationRate);
            Array.Copy(this._genes, g.Genes, this._length);
            return g;
        }

        /// <summary>
        /// Create new genes
        /// </summary>
        private void CreateGenes()
        {
            for (int i = 0; i < this._genes.Length; i++)
                this._genes[i] = (random.NextDouble() + random.Next(-20, 20));
        }

        /// <summary>
        /// Crossover
        /// </summary>
        /// <param name="genome"></param>
        /// <param name="child1"></param>
        /// <param name="child2"></param>
        public void Crossover(ref Genome genome, out Genome child1, out Genome child2)
        {
            int pos = (int)(random.NextDouble() * (double)this._length);
            child1 = new Genome(this._length, false, this._mutationRate);
            child2 = new Genome(this._length, false, this._mutationRate);
            for (int i = 0; i < this._length; i++)
            {
                if (i < pos)
                {
                    child1.Genes[i] = this._genes[i];
                    child2.Genes[i] = genome.Genes[i];
                }
                else
                {
                    child1.Genes[i] = genome.Genes[i];
                    child2.Genes[i] = this._genes[i];
                }
            }
        }

        /// <summary>
        /// Mutate genome
        /// </summary>
		public void Mutate()
        {
            for (int pos = 0; pos < this._length; pos++)
            {
                if (random.NextDouble() < this._mutationRate)
                    this._genes[pos] = (this._genes[pos] + (random.NextDouble() + random.Next(-20, 20))) / 2.0;
            }
        }

        /// <summary>
        /// Get values
        /// </summary>
        /// <param name="values"></param>
        public void GetValues(ref double[] values)
        {
            for (int i = 0; i < this._length; i++)
                values[i] = this._genes[i];
        }
    }
}
