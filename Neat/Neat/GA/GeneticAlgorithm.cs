using System;
using System.Collections.Generic;
using System.IO;

namespace Neat.GA
{
    public delegate double GAFunction(params double[] values);

    public class GeneticAlgorithm
    {
        private List<Genome> _thisGeneration;
        private List<Genome> _nextGeneration;
        private List<double> _fitnessTable;
        private Random _random = new Random();
        private double _mutationRate;
        private double _crossoverRate;
        private int _populationSize;
        private int _generationSize;
        private int _genomeSize;
        private double _totalFitness;
        private bool _elitism;
        private GAFunction _fitnessFunction;
        private int _currentGeneration;
        private int _currentPopulation;
        private double _maxFitness;

        /// <summary>
        /// Population size
        /// </summary>
        public int PopulationSize
        {
            get
            {
                return this._populationSize;
            }
            set
            {
                this._populationSize = value;
            }
        }

        /// <summary>
        /// Generation size
        /// </summary>
		public int GenerationSize
        {
            get
            {
                return this._generationSize;
            }
            set
            {
                this._generationSize = value;
            }
        }

        /// <summary>
        /// Genome size
        /// </summary>
		public int GenomeSize
        {
            get
            {
                return this._genomeSize;
            }
            set
            {
                this._genomeSize = value;
            }
        }

        /// <summary>
        /// Crossover rate
        /// </summary>
		public double CrossoverRate
        {
            get
            {
                return this._crossoverRate;
            }
            set
            {
                this._crossoverRate = value;
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
        /// Keep previous generation's fittest individual in place of worst in current
        /// </summary>
        public bool Elitism
        {
            get
            {
                return this._elitism;
            }
            set
            {
                this._elitism = value;
            }
        }

        /// <summary>
        /// Fitness function
        /// </summary>
        public GAFunction FitnessFunction
        {
            get
            {
                return this._fitnessFunction;
            }
            set
            {
                this._fitnessFunction = value;
            }
        }

        /// <summary>
        /// Current generation
        /// </summary>
        public int CurrentGeneration
        {
            get
            {
                return this._currentGeneration;
            }
        }

        /// <summary>
        /// Current population
        /// </summary>
        public int CurrentPopulation
        {
            get
            {
                return this._currentPopulation;
            }
        }

        /// <summary>
        /// Max fitness
        /// </summary>
        public double MaxFitness
        {
            get
            {
                return this._maxFitness;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public GeneticAlgorithm()
        {
            this._mutationRate = 0.05;
            this._crossoverRate = 0.80;
            this._populationSize = 100;
            this._generationSize = 2000;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="crossoverRate"></param>
        /// <param name="mutationRate"></param>
        /// <param name="populationSize"></param>
        /// <param name="generationSize"></param>
        /// <param name="genomeSize"></param>
		public GeneticAlgorithm(double crossoverRate, double mutationRate, int populationSize, int generationSize, int genomeSize)
        {
            this._mutationRate = mutationRate;
            this._crossoverRate = crossoverRate;
            this._populationSize = populationSize;
            this._generationSize = generationSize;
            this._genomeSize = genomeSize;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="genomeSize"></param>
		public GeneticAlgorithm(int genomeSize)
        {
            this._genomeSize = genomeSize;
        }

        /// <summary>
        /// Start GA
        /// </summary>
        public void Start()
        {
            if (this._fitnessFunction == null)
                throw new ArgumentNullException("Need to supply fitness function");
            if (this._genomeSize == 0)
                throw new IndexOutOfRangeException("Genome size not set");

            this._fitnessTable = new List<double>();
            this._maxFitness = 0;
            this._currentGeneration = 0;

            //this._thisGeneration = new List<Genome>(this._generationSize);
            //this._nextGeneration = new List<Genome>(this._generationSize);
            this._thisGeneration = new List<Genome>(this._populationSize);
            this._nextGeneration = new List<Genome>(this._populationSize);

            this.CreateGenomes();
            this.RankPopulation();

            for (int i = 0; i < this._generationSize; i++)
            {
                this._currentGeneration++;
                this.CreateNextGeneration();
                double fitness = this.RankPopulation();

                if (i % 100 == 0)
                {
                    Console.WriteLine("Generation " + i + ", Best Fitness: " + fitness);
                }
            }
        }

        /// <summary>
        /// After ranking all the genomes by fitness, use a 'roulette wheel' selection
        /// method.  This allocates a large probability of selection to those with the 
        /// highest fitness.
        /// </summary>
        /// <returns></returns>
        private int RouletteSelection()
        {
            double randomFitness = this._random.Next(0, (int)this._totalFitness);
            double runFitness = 0;

            for (int i = 0; i < this._fitnessTable.Count; i++)
            {
                runFitness += this._fitnessTable[i];
                if (runFitness > randomFitness)
                    return i;
            }
            return 0;
            //double randomFitness = this._random.NextDouble() * this._totalFitness;
            //int idx = -1;
            //int mid;
            //int first = 0;
            //int last = this._populationSize - 1;
            //mid = (last - first) / 2;

            //// Binary Searchs
            //while (idx == -1 && first <= last)
            //{
            //    if (randomFitness < this._fitnessTable[mid])
            //    {
            //        last = mid;
            //    }
            //    else if (randomFitness > this._fitnessTable[mid])
            //    {
            //        first = mid;
            //    }
            //    mid = (first + last) / 2;

            //    if ((last - first) == 1)
            //        idx = last;
            //}
            //return idx;
        }

        /// <summary>
		/// Rank population and sort in order of fitness.
		/// </summary>
		private double RankPopulation()
        {
            this._totalFitness = 0d;
            this._currentPopulation = 0;
            foreach (Genome g in this._thisGeneration)
            {
                g.Fitness = this._fitnessFunction(g.Genes);
                this._totalFitness += g.Fitness;
                
                if (g.Fitness > this._maxFitness)
                    this._maxFitness = g.Fitness;

                this._currentPopulation++;
            }
            this._thisGeneration.Sort(delegate (Genome x, Genome y)
            {
                return Comparer<double>.Default.Compare(x.Fitness, y.Fitness);
            });

            //  Now sorted in order of fitness
            double fitness = 0d;
            this._fitnessTable.Clear();
            foreach (Genome t in this._thisGeneration)
            {
                fitness += t.Fitness;
                this._fitnessTable.Add(t.Fitness);
            }

            return this._fitnessTable[this._fitnessTable.Count - 1];
        }

        /// <summary>
		/// Create the *initial* genomes by repeated calling the supplied fitness function
		/// </summary>
		private void CreateGenomes()
        {
            for (int i = 0; i < this._populationSize; i++)
            {
                Genome g = new Genome(this._genomeSize, this._mutationRate);
                this._thisGeneration.Add(g);
            }
        }

        /// <summary>
        /// Create next generation
        /// </summary>
		private void CreateNextGeneration()
        {
            this._nextGeneration.Clear();
            Genome g = null;
            if (this._elitism)
                g = this._thisGeneration[this._populationSize - 1].DeepCopy();

            for (int i = 0; i < this._populationSize; i += 2)
            {
                int pidx1 = this.RouletteSelection();
                int pidx2 = this.RouletteSelection();
                Genome child1 = null;
                Genome child2 = null;
                Genome parent1 = this._thisGeneration[pidx1];
                Genome parent2 = this._thisGeneration[pidx2];

                if (this._random.NextDouble() < this._crossoverRate)
                {
                    parent1.Crossover(ref parent2, out child1, out child2);
                }
                else
                {
                    child1 = parent1;
                    child2 = parent2;
                }
                child1.Mutate();
                child2.Mutate();

                this._nextGeneration.Add(child1);
                this._nextGeneration.Add(child2);
            }
            if (this._elitism && g != null)
                this._nextGeneration[0] = g;

            this._thisGeneration.Clear();
            foreach (Genome ge in this._nextGeneration)
                this._thisGeneration.Add(ge);
        }

        /// <summary>
        /// Get best genome
        /// </summary>
        /// <param name="values"></param>
        /// <param name="fitness"></param>
        public void GetBest(out double[] values, out double fitness)
        {
            if (this._thisGeneration == null || this._thisGeneration.Count == 0)
            {
                values = new double[0];
                fitness = 0d;
                return;
            }
            this.GetGenome(this._thisGeneration.Count - 1, out values, out fitness);
        }

        /// <summary>
        /// Get worst genome
        /// </summary>
        /// <param name="values"></param>
        /// <param name="fitness"></param>
        public void GetWorst(out double[] values, out double fitness)
        {
            if (this._thisGeneration == null || this._thisGeneration.Count == 0)
            {
                values = new double[0];
                fitness = 0d;
                return;
            }
            this.GetGenome(0, out values, out fitness);
        }

        /// <summary>
        /// Get specific genome
        /// </summary>
        /// <param name="n"></param>
        /// <param name="values"></param>
        /// <param name="fitness"></param>
        public void GetGenome(int n, out double[] values, out double fitness)
        {
            if (n < 0 || n >= this._thisGeneration.Count)
                throw new ArgumentOutOfRangeException("n too large, or too small");
            Genome g = this._thisGeneration[n];
            values = new double[g.Length];
            g.GetValues(ref values);
            fitness = g.Fitness;
        }
    }
}
