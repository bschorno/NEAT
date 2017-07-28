using System;
using System.Collections.Generic;

namespace Neat.EA
{
    public class Pool
    {
        private EvolutionaryAlogorithm _ea;
        private List<Species> _species = new List<EA.Species>();
        private int _generation;
        private int _innovation;
        private int _currentSpecies;
        private double _maxFitness;

        /// <summary>
        /// Species
        /// </summary>
        public List<Species> Species
        {
            get
            {
                return this._species;
            }
            set
            {
                this._species = value;
            }
        }

        /// <summary>
        /// Generation
        /// </summary>
        public int Generation
        {
            get
            {
                return this._generation;
            }
            set
            {
                this._generation = value;
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
        /// Get current species
        /// </summary>
        public int CurrentSpecies
        {
            get
            {
                return this._currentSpecies;
            }
            set
            {
                this._currentSpecies = value;
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
            set
            {
                this._maxFitness = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ea"></param>
        public Pool(EvolutionaryAlogorithm ea)
        {
            this._ea = ea;
            this._innovation = this._ea.Outputs;
        }

        /// <summary>
        /// Initialize pool
        /// </summary>
        public void Initalize()
        {
            for (int i = 0; i < this._ea.PopulationSize; i++)
            {
                this.AddToSpecies(new Genome(this._ea, this));
            }
        }

        /// <summary>
        /// Get next innovation
        /// </summary>
        /// <returns></returns>
        public int GetNextInnovation()
        {
            return this._innovation++;
        }

        /// <summary>
        /// Get current species
        /// </summary>
        /// <returns></returns>
        public Species GetCurrentSpecies()
        {
            if (this._currentSpecies >= this._species.Count)
                return null;
            return this._species[this._currentSpecies];
        }

        /// <summary>
        /// Get current genome
        /// </summary>
        /// <returns></returns>
        public Genome GetCurrentGenome()
        {
            Species species = this.GetCurrentSpecies();
            if (species == null)
                return null;
            return this.GetCurrentSpecies().GetCurrentGenome();
        }

        /// <summary>
        /// Next species
        /// </summary>
        public void NextGenome()
        {
            if (++this.GetCurrentSpecies().CurrentGenome >= this.GetCurrentSpecies().Genomes.Count)
            {
                this.GetCurrentSpecies().CurrentGenome = 0;
                if (++this._currentSpecies >= this._species.Count)
                {
                    this.NextGeneration();
                    this._currentSpecies = 0;
                }
            }

        }

        /// <summary>
        /// Next generation
        /// </summary>
        private void NextGeneration()
        {
            this.CullSpecies(false);
            this.RankGlobally();
            this.RemoveStaleSpecies();
            this.RankGlobally();
            foreach (Species species in this._species)
                species.CalculateAverageFitness();
            this.RemoveWeakSpecies();

            double sum = this.TotalAverageFitness();
            List<Genome> children = new List<Genome>();
            foreach (Species species in this._species)
            {
                decimal breed = Math.Floor((decimal)(species.AverageFitness / sum * this._ea.PopulationSize));
                for (int i = 0; i < breed; i++)
                    children.Add(species.BreedChild());
            }
            this.CullSpecies(true);
            while (children.Count + this._species.Count < this._ea.PopulationSize)
            {
                children.Add(this._species[this._ea.Random.Next(0, this._species.Count)].BreedChild());
            }
            foreach (Genome child in children)
            {
                this.AddToSpecies(child);
            }
            this._generation++;
        }

        /// <summary>
        /// Add genome to species
        /// </summary>
        /// <param name="genome"></param>
        private void AddToSpecies(Genome genome)
        {
            foreach (Species s in this._species)
            {
                if (this.SameSpecies(genome, s.Genomes[0]))
                {
                    s.Genomes.Add(genome);
                    return;
                }
            }

            Species species = new Species(this._ea);
            species.Genomes.Add(genome);
            this._species.Add(species);
        }

        /// <summary>
        /// Check if same species
        /// </summary>
        /// <param name="g1"></param>
        /// <param name="g2"></param>
        /// <returns></returns>
        private bool SameSpecies(Genome g1, Genome g2)
        {
            double dd = this._ea.DeltaDisjoint * this.GetDisjoint(g1, g2);
            double dw = this._ea.DeltaWeights * this.GetWeights(g1, g2);
            return dd + dw < this._ea.DeltaThreshold;
        }

        /// <summary>
        /// Get disjoint of two genome
        /// </summary>
        /// <param name="genome1"></param>
        /// <param name="genome2"></param>
        /// <returns></returns>
        private double GetDisjoint(Genome genome1, Genome genome2)
        {
            List<int> innovation1 = new List<int>();
            List<int> innovation2 = new List<int>();
            foreach (Gene gene in genome1.Genes)
                innovation1.Add(gene.Innovation);
            foreach (Gene gene in genome2.Genes)
                innovation2.Add(gene.Innovation);

            int disjoint = 0;

            foreach (Gene gene in genome1.Genes)
                if (!innovation2.Contains(gene.Innovation))
                    disjoint++;
            
            foreach (Gene gene in genome2.Genes)
                if (!innovation1.Contains(gene.Innovation))
                    disjoint++;

            return disjoint / Math.Max(genome1.Genes.Count, genome2.Genes.Count);
        }

        /// <summary>
        /// Get weights of two genome
        /// </summary>
        /// <param name="genome1"></param>
        /// <param name="genome2"></param>
        /// <returns></returns>
        private double GetWeights(Genome genome1, Genome genome2)
        {
            double sum = 0;
            int coincident = 0;

            Gene[] genes = new Gene[this._ea.Pool.Innovation];

            foreach (Gene gene in genome2.Genes)
                genes[gene.Innovation] = gene;

            foreach (Gene gene in genome1.Genes)
            {
                if (genes[gene.Innovation] != null)
                {
                    sum += Math.Abs(gene.Weight - genes[gene.Innovation].Weight);
                    coincident++;
                }
            }

            return coincident == 0 ? 0 : sum / coincident;
        }

        /// <summary>
        /// Cull species
        /// </summary>
        /// <param name="cutToOne"></param>
        private void CullSpecies(bool cutToOne)
        {
            foreach (Species species in this._species)
            {
                species.Genomes.Sort(delegate (Genome g1, Genome g2)
                {
                    if (g1.Fitness == g2.Fitness) return 0;
                    else if (g1.Fitness > g2.Fitness) return 1;
                    else if (g1.Fitness < g2.Fitness) return -1;
                    else return 0;
                });

                decimal remaining = Math.Ceiling((decimal)(species.Genomes.Count / 2d));
                if (cutToOne)
                    remaining = 1;
                while (species.Genomes.Count > remaining)
                {
                    species.Genomes.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Rank globally
        /// </summary>
        private void RankGlobally()
        {
            List<Genome> global = new List<Genome>();

            foreach (Species species in this._species)
            {
                foreach (Genome genome in species.Genomes)
                {
                    global.Add(genome);
                }
            }

            global.Sort(delegate (Genome g1, Genome g2)
            {
                if (g1.Fitness == g2.Fitness) return 0;
                else if (g1.Fitness < g2.Fitness) return 1;
                else if (g1.Fitness > g2.Fitness) return -1;
                else return 0;
            });

            for (int i = 0; i < global.Count; i++)
            {
                global[i].GlobalRank = i;
            }
        }

        /// <summary>
        /// Remove stale species
        /// </summary>
        private void RemoveStaleSpecies()
        {
            List<Species> survived = new List<Species>();

            foreach (Species species in this._species)
            {
                species.Genomes.Sort(delegate (Genome g1, Genome g2)
                {
                    if (g1.Fitness == g2.Fitness) return 0;
                    else if (g1.Fitness > g2.Fitness) return 1;
                    else if (g1.Fitness < g2.Fitness) return -1;
                    else return 0;
                });

                if (species.Genomes[0].Fitness > species.TopFitness)
                {
                    species.TopFitness = species.Genomes[0].Fitness;
                    species.Staleness = 0;
                }
                else
                {
                    species.Staleness++;
                }
                if (species.Staleness < this._ea.StaleSpecies || species.TopFitness >= this._maxFitness)
                    survived.Add(species);
            }

            this._species = survived;
        }

        /// <summary>
        /// Total average fitness
        /// </summary>
        /// <returns></returns>
        private double TotalAverageFitness()
        {
            double total = 0d;

            foreach (Species species in this._species)
                total += species.AverageFitness;

            return total;
        }

        /// <summary>
        /// Remove weak species
        /// </summary>
        private void RemoveWeakSpecies()
        {
            List<Species> survived = new List<Species>();

            double sum = this.TotalAverageFitness();

            foreach (Species species in this._species)
            {
                decimal breed = Math.Floor((decimal)(species.AverageFitness / sum * this._ea.PopulationSize));
                if (breed >= 1)
                    survived.Add(species);
            }

            this._species = survived;
        }
    }
}
