using System;
using System.Collections.Generic;

namespace Neat.EA
{
    public class Genome
    {
        private EvolutionaryAlogorithm _ea;
        private Pool _pool;
        private List<Gene> _genes = new List<Gene>();
        private Network _network;
        private double _fitness;
        private double _adjustedFitness;
        private int _globalRank;
        private int _maxNeuron;
        private float _connectionsMutationChance;
        private float _connectionsMutationStep;
        private float _linkMutationChance;
        private float _nodeMutationChance;
        private float _biasMutationChance;
        private float _disableMutationChance;
        private float _enableMutationChance;

        /// <summary>
        /// Genes
        /// </summary>
        public List<Gene> Genes
        {
            get
            {
                return this._genes;
            }
            set
            {
                this._genes = value;
            }
        }

        /// <summary>
        /// Network
        /// </summary>
        public Network Network
        {
            get
            {
                return this._network;
            }
            set
            {
                this._network = value;
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
        /// Adjusted fitness
        /// </summary>
        public double AdjustedFitness
        {
            get
            {
                return this._adjustedFitness;
            }
            set
            {
                this._adjustedFitness = value;
            }
        }

        /// <summary>
        /// Global rank
        /// </summary>
        public int GlobalRank
        {
            get
            {
                return this._globalRank;
            }
            set
            {
                this._globalRank = value;
            }
        }

        /// <summary>
        /// Max neuron
        /// </summary>
        public int MaxNeuron
        {
            get
            {
                return this._maxNeuron;
            }
            set
            {
                this._maxNeuron = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ea"></param>
        public Genome(EvolutionaryAlogorithm ea, Pool pool)
        {
            this._ea = ea;
            this._pool = pool;
            this._maxNeuron = this._ea.Inputs;
            this._connectionsMutationChance = this._ea.ConnectionsMutationChance;
            this._connectionsMutationStep   = this._ea.ConnectionsMutationStep;
            this._linkMutationChance        = this._ea.LinkMutationChance;
            this._biasMutationChance        = this._ea.BiasMutationChance;
            this._nodeMutationChance        = this._ea.NodeMutationChance;
            this._enableMutationChance      = this._ea.EnableMutationChance;
            this._disableMutationChance     = this._ea.DisableMutationChance;
            this.Mutate();
        }

        /// <summary>
        /// Constructor (Copy genome)
        /// </summary>
        /// <param name="genome"></param>
        public Genome(Genome genome)
            : this(genome._ea, genome._pool)
        {
            for (int i = 0; i < genome._genes.Count; i++)
                this._genes.Add(new Gene(genome._genes[i]));
            this._maxNeuron                 = genome._maxNeuron;
            this._connectionsMutationChance = genome._connectionsMutationChance;
            this._linkMutationChance        = genome._linkMutationChance;
            this._biasMutationChance        = genome._biasMutationChance;
            this._nodeMutationChance        = genome._nodeMutationChance;
            this._enableMutationChance      = genome._enableMutationChance;
            this._disableMutationChance     = genome._disableMutationChance;
        }

        /// <summary>
        /// Constructor (Crossover two genome)
        /// </summary>
        /// <param name="genome1"></param>
        /// <param name="genome2"></param>
        public Genome(Genome genome1, Genome genome2)
        {
            this._ea = genome1._ea;
            this._pool = genome1._pool;

            if (genome2._fitness > genome1._fitness)
            {
                Genome temp = genome1;
                genome1 = genome2;
                genome2 = temp;
            }

            Gene[] innovations = new Gene[this._ea.Pool.Innovation];
            foreach (Gene g2 in genome2._genes)
            {
                innovations[g2.Innovation] = g2;
            }

            foreach (Gene g1 in genome1._genes)
            {
                Gene g2 = innovations[g1.Innovation];
                if (g2 != null && g2.Enable && this._ea.Random.Next(0, 2) == 1)
                    this._genes.Add(new Gene(g2));
                else
                    this._genes.Add(new Gene(g1));
            }

            this._maxNeuron = Math.Max(genome1._maxNeuron, genome2._maxNeuron);
            this._connectionsMutationChance = genome1._connectionsMutationChance;
            this._linkMutationChance        = genome1._linkMutationChance;
            this._biasMutationChance        = genome1._biasMutationChance;
            this._nodeMutationChance        = genome1._nodeMutationChance;
            this._enableMutationChance      = genome1._enableMutationChance;
            this._disableMutationChance     = genome1._disableMutationChance;
        }

        /// <summary>
        /// Mutate genom
        /// </summary>
        public void Mutate()
        {
            Random random = this._ea.Random;
            
            if (random.Next(1, 2) == 1)
            {
                this._connectionsMutationChance *= 0.95f;
                this._connectionsMutationStep   *= 0.95f;
                this._linkMutationChance        *= 0.95f;
                this._biasMutationChance        *= 0.95f;
                this._nodeMutationChance        *= 0.95f;
                this._enableMutationChance      *= 0.95f;
                this._disableMutationChance     *= 0.95f;
            }
            else
            {
                this._connectionsMutationChance *= 1.05263f;
                this._connectionsMutationStep   *= 1.05263f;
                this._linkMutationChance        *= 1.05263f;
                this._biasMutationChance        *= 1.05263f;
                this._nodeMutationChance        *= 1.05263f;
                this._enableMutationChance      *= 1.05263f;
                this._disableMutationChance     *= 1.05263f;
            }

            if (random.NextDouble() < this._connectionsMutationChance)
                this.ConnectionMutate();

            float var1 = this._linkMutationChance;
            while (var1 > 0)
            {
                if (random.NextDouble() < var1)
                    this.LinkMutate();
                var1--;
            }

            var1 = this._biasMutationChance;
            while (var1 > 0)
            {
                if (random.NextDouble() < var1)
                    this.BiasMutate();
                var1--;
            }

            var1 = this._nodeMutationChance;
            while (var1 > 0)
            {
                if (random.NextDouble() < var1)
                    this.NodeMutate();
                var1--;
            }

            var1 = this._enableMutationChance;
            while (var1 > 0)
            {
                if (random.NextDouble() < var1)
                    this.EnableMutate();
                var1--;
            }

            var1 = this._disableMutationChance;
            while (var1 > 0)
            {
                if (random.NextDouble() < var1)
                    this.DisableMutate();
                var1--;
            }
        }

        /// <summary>
        /// Connection mutate
        /// </summary>
        private void ConnectionMutate()
        {
            foreach (Gene gene in this._genes)
            {
                if (this._ea.Random.NextDouble() < this._ea.PerturbChance)
                    gene.Weight += this._ea.Random.NextDouble() * this._connectionsMutationStep * 2 - this._connectionsMutationStep;
                else
                    gene.Weight = this._ea.Random.NextDouble() * 4 - 2;
            }
        }

        /// <summary>
        /// Link mutate
        /// </summary>
        private void LinkMutate()
        {
            int var1 = this.RandomNeuron(false);
            int var2 = this.RandomNeuron(true);

            if (var1 < this._ea.Inputs && var2 < this._ea.Inputs)
                return;

            if (var2 < this._ea.Inputs)
            {
                int tmp = var1;
                var1 = var2;
                var2 = tmp;
            }

            Gene var3 = new Gene(this._ea)
            {
                Into = var1,
                Out = var2
            };

            if (this._genes.Contains(var3))
                return;

            var3.Innovation = this._pool.GetNextInnovation();
            var3.Weight = this._ea.Random.NextDouble() * 4 - 2;

            this._genes.Add(var3);
        }

        /// <summary>
        /// Bias mutate
        /// </summary>
        private void BiasMutate()
        {
            int neuron = this.RandomNeuron(false);

            Gene var3 = new Gene(this._ea)
            {
                Into = this._ea.Inputs - 1,
                Out = neuron
            };

            if (this._genes.Contains(var3))
                return;

            var3.Innovation = this._ea.Pool.GetNextInnovation();
            var3.Weight = this._ea.Random.NextDouble() * 4 - 2;

            this._genes.Add(var3);
        }

        /// <summary>
        /// Node mutate
        /// </summary>
        private void NodeMutate()
        {
            if (this._genes.Count == 0)
                return;

            //int maxNode = this._network.Neurons.Count - this._ea.Outputs;
            this._maxNeuron++;
            //Neuron neuron = new Neuron(this._ea, ++maxNode);
            //this._network.Neurons.Add(maxNode, neuron);

            Gene cGene = null;
            Gene nGene = null;

            cGene = this._genes[this._ea.Random.Next(0, this._genes.Count - 1)];
            if (!cGene.Enable)
                return;
            cGene.Enable = false;

            nGene = new Gene(cGene);
            nGene.Out        = this._maxNeuron;
            nGene.Weight     = 1.0d;
            nGene.Innovation = this._pool.GetNextInnovation();
            nGene.Enable     = true;
            this._genes.Add(nGene);

            nGene = new Gene(cGene);
            nGene.Into       = this._maxNeuron;
            nGene.Weight     = 1.0d;
            nGene.Innovation = this._pool.GetNextInnovation();
            nGene.Enable     = true;
            this._genes.Add(nGene);
        }

        /// <summary>
        /// Enable mutate
        /// </summary>
        private void EnableMutate()
        {
            List<Gene> genes = new List<Gene>();

            foreach (Gene g in this._genes)
            {
                if (!g.Enable)
                    genes.Add(g);
            }

            if (genes.Count == 0)
                return;

            genes[this._ea.Random.Next(0, genes.Count - 1)].Enable = true;
        }

        /// <summary>
        /// Disable mutate
        /// </summary>
        private void DisableMutate()
        {
            List<Gene> genes = new List<Gene>();

            foreach (Gene g in this._genes)
            {
                if (g.Enable)
                    genes.Add(g);
            }

            if (genes.Count == 0)
                return;

            genes[this._ea.Random.Next(0, genes.Count - 1)].Enable = false;
        }

        /// <summary>
        /// Get random neuron
        /// </summary>
        /// <param name="noInput">No input neuron</param>
        /// <returns></returns>
        private int RandomNeuron(bool noInput)
        {
            List<int> neurons = new List<int>();

            if (!noInput)
            {
                for (int i = 0; i < this._ea.Inputs; i++)
                    neurons.Add(i);
            }

            for (int i = 0; i < this._ea.Outputs; i++)
                    neurons.Add(this._ea.MaxNodes + i);

            foreach (Gene gene in this._genes)
            {
                if (!noInput || gene.Into > this._ea.Inputs)
                    if (!neurons.Contains(gene.Into))
                        neurons.Add(gene.Into);
                if (!noInput || gene.Out > this._ea.Inputs)
                    if (!neurons.Contains(gene.Out))
                        neurons.Add(gene.Out);
            }

            int var1 = this._ea.Random.Next(0, neurons.Count);

            return neurons[var1];
        }
    }
}
