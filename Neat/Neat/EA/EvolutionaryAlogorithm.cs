using System;
using System.Collections.Generic;

namespace Neat.EA
{
    public delegate double EAFunction(Network network);

    public class EvolutionaryAlogorithm
    {
        private Pool _pool;
        private Random _random;

        private int _inputs;
        private int _outputs;

        private int _populationSize   = 300;
        private int _staleSpecies     = 15;
        private int _maxNodes         = 10000;

        private float _deltaDisjoint  = 2.0f;
        private float _deltaWeights   = 0.4f;
        private float _deltaThreshold = 1.0f;
        
        private float _perturbChance             = 0.90f;
        private float _crossoverChance           = 0.75f;
        private float _connectionsMutationChance = 0.25f;
        private float _connectionsMutationStep   = 0.10f;
        private float _linkMutationChance        = 2.00f;
        private float _nodeMutationChance        = 0.50f;
        private float _biasMutationChance        = 0.40f;
        private float _disableMutationChance     = 0.40f;
        private float _enableMutationChance      = 0.20f;

        private EAFunction _evaluateNetwork;

        /// <summary>
        /// Random
        /// </summary>
        public Random Random
        {
            get
            {
                return this._random;
            }
            set
            {
                this._random = value;
            }
        }

        /// <summary>
        /// Number of input neurons
        /// </summary>
        public int Inputs
        {
            get
            {
                return this._inputs;
            }
            set
            {
                this._inputs = value;
            }
        }

        /// <summary>
        /// Number of output neurons
        /// </summary>
        public int Outputs
        {
            get
            {
                return this._outputs;
            }
            set
            {
                this._outputs = value;
            }
        }

        /// <summary>
        /// Pool
        /// </summary>
        public Pool Pool
        {
            get
            {
                return this._pool;
            }
            set
            {
                this._pool = value;
            }
        }

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
        /// Stale species
        /// </summary>
        public int StaleSpecies
        {
            get
            {
                return this._staleSpecies;
            }
            set
            {
                this._staleSpecies = value;
            }
        }

        /// <summary>
        /// Max numbers of nodes
        /// </summary>
        public int MaxNodes
        {
            get
            {
                return this._maxNodes;
            }
            set
            {
                this._maxNodes = value;
            }
        }

        /// <summary>
        /// Delta disjoint
        /// </summary>
        public float DeltaDisjoint
        {
            get
            {
                return this._deltaDisjoint;
            }
            set
            {
                this._deltaDisjoint = value;
            }
        }

        /// <summary>
        /// Delta weights
        /// </summary>
        public float DeltaWeights
        {
            get
            {
                return this._deltaWeights;
            }
            set
            {
                this._deltaWeights = value;
            }
        }

        /// <summary>
        /// Delta threshold
        /// </summary>
        public float DeltaThreshold
        {
            get
            {
                return this._deltaThreshold;
            }
            set
            {
                this._deltaThreshold = value;
            }
        }

        /// <summary>
        /// Connections mutation chance
        /// </summary>
        public float ConnectionsMutationChance
        {
            get
            {
                return this._connectionsMutationChance;
            }
            set
            {
                this._connectionsMutationChance = value;
            }
        }

        /// <summary>
        /// Perturb chance
        /// </summary>
        public float PerturbChance
        {
            get
            {
                return this._perturbChance;
            }
            set
            {
                this._perturbChance = value;
            }
        }

        /// <summary>
        /// Crossover chance
        /// </summary>
        public float CrossoverChance
        {
            get
            {
                return this._crossoverChance;
            }
            set
            {
                this._crossoverChance = value;
            }
        }

        /// <summary>
        /// Link mutation chance
        /// </summary>
        public float LinkMutationChance
        {
            get
            {
                return this._linkMutationChance;
            }
            set
            {
                this._linkMutationChance = value;
            }
        }

        /// <summary>
        /// Node mutation chance
        /// </summary>
        public float NodeMutationChance
        {
            get
            {
                return this._nodeMutationChance;
            }
            set
            {
                this._nodeMutationChance = value;
            }
        }

        /// <summary>
        /// Bias mutation chance
        /// </summary>
        public float BiasMutationChance
        {
            get
            {
                return this._biasMutationChance;
            }
            set
            {
                this._biasMutationChance = value;
            }
        }

        /// <summary>
        /// Disable mutation chance
        /// </summary>
        public float DisableMutationChance
        {
            get
            {
                return this._disableMutationChance;
            }
            set
            {
                this._disableMutationChance = value;
            }
        }

        /// <summary>
        /// Enable mutation chance
        /// </summary>
        public float EnableMutationChance
        {
            get
            {
                return this._enableMutationChance;
            }
            set
            {
                this._enableMutationChance = value;
            }
        }

        /// <summary>
        /// Step size
        /// </summary>
        public float ConnectionsMutationStep
        {
            get
            {
                return this._connectionsMutationStep;
            }
            set
            {
                this._connectionsMutationStep = value;
            }
        }

        /// <summary>
        /// Evaluate network
        /// </summary>
        public EAFunction EvaluateNetwork
        {
            get
            {
                return this._evaluateNetwork;
            }
            set
            {
                this._evaluateNetwork = value;
            }
        }

        /// <summary>
        /// Max fitness
        /// </summary>
        public double MaxFitness
        {
            get
            {
                return this._pool.MaxFitness;
            }
        }

        /// <summary>
        /// Current generation
        /// </summary>
        public int CurrentGeneration
        {
            get
            {
                return this._pool.Generation;
            }
        }

        /// <summary>
        /// Current species
        /// </summary>
        public int CurrentSpecies
        {
            get
            {
                return this._pool.CurrentSpecies;
            }
        }

        /// <summary>
        /// Current genome
        /// </summary>
        public int CurrentGenome
        {
            get
            {
                Species species = this._pool.GetCurrentSpecies();
                if (species == null)
                    return 0;
                return this._pool.GetCurrentSpecies().CurrentGenome;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public EvolutionaryAlogorithm(int inputs, int outputs)
        {
            this._inputs = inputs + 1;
            this._outputs = outputs;
            this._random = new Random();
        }

        /// <summary>
        /// Run
        /// </summary>
        public void Run()
        {
            if (this._evaluateNetwork == null)
                throw new ArgumentNullException("Need to supply fitness function");

            this._pool = new Pool(this);
            this._pool.Initalize();

            while (true)
            {
                this._pool.GetCurrentGenome().Network = new Network(this, this._pool.GetCurrentGenome());

                double fitness = this._evaluateNetwork(this._pool.GetCurrentGenome().Network);

                this._pool.GetCurrentGenome().Fitness = fitness;

                if (fitness > this._pool.MaxFitness)
                    this._pool.MaxFitness = fitness;

                this.Next();

            }
        }

        /// <summary>
        /// Next genome
        /// </summary>
        private void Next()
        {
            this._pool.NextGenome();
        }
    }
}
