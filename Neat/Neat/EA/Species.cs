using System;
using System.Collections.Generic;

namespace Neat.EA
{
    public class Species
    {
        private EvolutionaryAlogorithm _ea;
        private List<Genome> _genomes = new List<Genome>();
        private double _topFitness;
        private double _averageFitness;
        private int _staleness;
        private int _currentGenome;

        /// <summary>
        /// Genomes
        /// </summary>
        public List<Genome> Genomes
        {
            get
            {
                return this._genomes;
            }
            set
            {
                this._genomes = value;
            }
        }

        /// <summary>
        /// Top fitness
        /// </summary>
        public double TopFitness
        {
            get
            {
                return this._topFitness;
            }
            set
            {
                this._topFitness = value;
            }
        }

        /// <summary>
        /// Top fitness
        /// </summary>
        public double AverageFitness
        {
            get
            {
                return this._averageFitness;
            }
            set
            {
                this._averageFitness = value;
            }
        }

        /// <summary>
        /// Top fitness
        /// </summary>
        public int Staleness
        {
            get
            {
                return this._staleness;
            }
            set
            {
                this._staleness = value;
            }
        }

        /// <summary>
        /// Get current genome
        /// </summary>
        public int CurrentGenome
        {
            get
            {
                return this._currentGenome;
            }
            set
            {
                this._currentGenome = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public Species(EvolutionaryAlogorithm ea)
        {
            this._ea = ea;
        }

        /// <summary>
        /// Get current genome
        /// </summary>
        /// <returns></returns>
        public Genome GetCurrentGenome()
        {
            if (this._currentGenome >= this._genomes.Count)
                return null;
            return this._genomes[this._currentGenome];
        }

        /// <summary>
        /// Calculate average fitness
        /// </summary>
        public void CalculateAverageFitness()
        {
            int total = 0;

            foreach (Genome genome in this._genomes)
            {
                total += genome.GlobalRank;
            }

            this._averageFitness = total / this._genomes.Count;
        }

        /// <summary>
        /// Breed child
        /// </summary>
        /// <returns></returns>
        public Genome BreedChild()
        {
            Genome child = null;

            if (this._ea.Random.NextDouble() < this._ea.CrossoverChance)
            {
                Genome g1 = this._genomes[this._ea.Random.Next(0, this._genomes.Count)];
                Genome g2 = this._genomes[this._ea.Random.Next(0, this._genomes.Count)];
                child = new Genome(g1, g2);
            }
            else
            {
                Genome g1 = this._genomes[this._ea.Random.Next(0, this._genomes.Count)];
                child = new Genome(g1);
            }

            child.Mutate();

            return child;
        }
    }
}
