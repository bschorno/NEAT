using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Neat.NN;
using Neat.GA;

namespace Neat.DemoSnake2
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Point> _snake = new List<Point>();
        private Point _food;
        private Random _random;

        //private EvolutionaryAlogorithm _ea;
        //private Thread _eaThread;
        private GeneticAlgorithm _ga;
        private Thread _gaThread;

        private NeuralNetwork _network;

        private Direction _direction        = Direction.Down;
        private Direction _lastDirection    = Direction.Down;
        private bool _gameOver = true;
        private int _score = 0;
        private double _fitness = 0d;

        private int _width = 16;
        private int _height = 16;

        public MainWindow()
        {
            InitializeComponent();

            this._network = new NeuralNetwork(new int[] { 24, 18, 4 });
            this._ga = new GeneticAlgorithm(0.50, 0.01, 2000, 200, 1008);
            this._ga.FitnessFunction = new GAFunction(EvaluateNetwork);
            this._ga.Elitism = true;
            //this._ga.Start();

            this._gaThread = new Thread(this._ga.Start);
            this._gaThread.SetApartmentState(ApartmentState.STA);
            this._gaThread.Start();

            //this._ea = new EvolutionaryAlogorithm(24, 2);
            //this._ea.EvaluateNetwork = this.EvaluateNetwork;
            //this._ea.PopulationSize = 500;
            //this._eaThread = new Thread(this._ea.Run);
            //this._eaThread.SetApartmentState(ApartmentState.STA);
            //this._eaThread.Start();
        }

        /// <summary>
        /// Set network weights
        /// </summary>
        /// <param name="weights"></param>
        private void SetNetworkWeights(double[] weights)
        {
            int index = 0;

            foreach (Layer layer in this._network.Layers)
            {
                foreach (Neuron neuron in layer.Neurons)
                {
                    foreach (Dendrite dendrite in neuron.Dendrites)
                    {
                        dendrite.Weight = weights[index++];
                    }
                    neuron.Bias = weights[index++];
                }
            }
        }

        /// <summary>
        /// Get distance between two points
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        private double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        /// <summary>
        /// Check if point is on tail
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private bool IsTail(Point point)
        {
            for (int i = 1; i < this._snake.Count; i++)
            {
                if (this._snake[i].X == point.X && this._snake[i].Y == point.Y)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Look in direction
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private double[] LookInDirection(Point head, Point direction)
        {
            double[] ret = new double[3];

            Point pos = head;
            bool foodFound = false;
            bool tailFound = false;
            float distance = 0;

            pos.X += direction.X;
            pos.Y += direction.Y;
            distance++;

            while (!(pos.X < 0 || pos.Y < 0 || pos.X >= this._width || pos.Y >= this._height))
            {
                if (!foodFound && pos.X == this._food.X && pos.Y == this._food.Y)
                {
                    ret[0] = 1;
                    foodFound = true;
                }

                if (!tailFound && this.IsTail(pos))
                {
                    ret[1] = 1 / distance;
                    tailFound = true;
                }

                pos.X += direction.X;
                pos.Y += direction.Y;
                distance++;
            }

            ret[2] = 1 / distance;
            return ret;
        }

        /// <summary>
        /// Evaluate network
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        private double EvaluateNetwork(double[] weights)
        {
            this._fitness = 0;

            long ticks = 0;

            int speed = (int)_sldSpeed.Dispatcher.Invoke(new Func<double>(() => _sldSpeed.Value));

            this.SetNetworkWeights(weights);

            this.StartRun();

            while (!this._gameOver)
            {
                double[] input = new double[24];
                double[] output = new double[4];

                Point head = this._snake[0];

                //int j = 0;
                //for (int x = -2; x <= 2; x++)
                //    for (int y = -2; y <= 2; y++)
                //    {
                //        if (x == 0 && y == 0)
                //            continue;

                //        for (int i = 1; i < this._snake.Count; i++)
                //        {
                //            if (this._snake[i].X == head.X + x && this._snake[i].Y == head.Y + y)
                //                input[j] = -1;
                //        }

                //        if (head.X + x >= this._width)
                //            input[j] = -1;
                //        if (head.X + x < 0)
                //            input[j] = -1;
                //        if (head.Y + y >= this._height)
                //            input[j] = -1;
                //        if (head.Y + y < 0)
                //            input[j] = -1;

                //        if (head.X + x == this._food.X && head.Y + y == this._food.Y)
                //            input[j] = 1;

                //        j++;
                //    }

                double[] ret;

                ret = this.LookInDirection(head, new Point(-1, 0));
                input[0]  = ret[0];
                input[1]  = ret[1];
                input[2]  = ret[2];
                ret = this.LookInDirection(head, new Point(-1, -1));
                input[3]  = ret[0];
                input[4]  = ret[1];
                input[5]  = ret[2];
                ret = this.LookInDirection(head, new Point(0, -1));
                input[6]  = ret[0];
                input[7]  = ret[1];
                input[8]  = ret[2];
                ret = this.LookInDirection(head, new Point(1, -1));
                input[9]  = ret[0];
                input[10] = ret[1];
                input[11] = ret[2];
                ret = this.LookInDirection(head, new Point(1, 0));
                input[12] = ret[0];
                input[13] = ret[1];
                input[14] = ret[2];
                ret = this.LookInDirection(head, new Point(1, 1));
                input[15] = ret[0];
                input[16] = ret[1];
                input[17] = ret[2];
                ret = this.LookInDirection(head, new Point(0, 1));
                input[18] = ret[0];
                input[19] = ret[1];
                input[20] = ret[2];
                ret = this.LookInDirection(head, new Point(-1, 1));
                input[21] = ret[0];
                input[22] = ret[1];
                input[23] = ret[2];

                //input[0] = Math.Abs(Distance(head.X, head.Y, this._width, head.Y));
                //input[1] = Math.Abs(Distance(head.X, head.Y, 0, head.Y));
                //input[2] = Math.Abs(Distance(head.X, head.Y, head.X, this._height));
                //input[3] = Math.Abs(Distance(head.X, head.Y, head.X, 0));

                //if (head.X < this._food.X) input[4] = Distance(head.X, head.Y, this._food.X, head.Y);
                //if (head.Y < this._food.Y) input[5] = Distance(head.X, head.Y, head.X, this._food.Y);
                //if (head.X > this._food.X) input[6] = Distance(head.X, head.Y, this._food.X, head.Y);
                //if (head.Y > this._food.Y) input[7] = Distance(head.X, head.Y, head.X, this._food.Y);

                //for (int i = 1; i < this._snake.Count; i++)
                //{
                //    double dist = 0;

                //    dist = Distance(head.X, head.Y, this._snake[i].X, head.Y);
                //    if (head.X < this._snake[i].X && (dist < input[8] || input[8] == 0)) input[8] = dist;

                //    dist = Distance(head.X, head.Y, head.X, this._snake[i].Y);
                //    if (head.Y < this._snake[i].Y && (dist < input[9] || input[9] == 0)) input[9] = dist;

                //    dist = Distance(head.X, head.Y, this._snake[i].X, head.Y);
                //    if (head.X > this._snake[i].X && (dist < input[10] || input[10] == 0)) input[10] = dist;

                //    dist = Distance(head.X, head.Y, head.X, this._snake[i].Y);
                //    if (head.Y > this._snake[i].Y && (dist < input[11] || input[11] == 0)) input[11] = dist;
                //}

                //output = network.Run(input);
                output = this._network.Run(new List<double>(input));

                double max = Math.Max(output[0], Math.Max(output[1], Math.Max(output[2], output[3])));

                //if (Math.Abs(output[0]) > Math.Abs(output[1]))
                //    output[1] = 0;
                //else
                //    output[0] = 0;

                //if (output[1] > 0 && this._lastDirection != Direction.Left)
                //    this._direction = Direction.Right;
                //else if (output[1] < 0 && this._lastDirection != Direction.Right)
                //    this._direction = Direction.Left;
                //else if (output[0] > 0 && this._lastDirection != Direction.Down)
                //    this._direction = Direction.Up;
                //else if (output[0] < 0 && this._lastDirection != Direction.Up)
                //    this._direction = Direction.Down;
                if (output[0] == max)// && this._lastDirection != Direction.Left)
                    this._direction = Direction.Right;
                else if (output[1] == max)// && this._lastDirection != Direction.Right)
                    this._direction = Direction.Left;
                else if (output[2] == max)// && this._lastDirection != Direction.Down)
                    this._direction = Direction.Up;
                else if (output[3] == max)// && this._lastDirection != Direction.Up)
                    this._direction = Direction.Down;

                this._lastDirection = this._direction;

                Dispatcher.Invoke((Action)(() =>
                {
                    this.UpdateScreenNetwork();
                }));

                this.UpdateSnake();

                this._fitness += 1;

                if (++ticks / 200 > this._score)
                {
                    this.GameOver();
                    //this._fitness = 0;
                }

                if (!this._gameOver)
                    Dispatcher.Invoke((Action)(() =>
                    {
                        this.UpdateScreenSnake();
                    }));
                
            
                Thread.Sleep(speed);
            }

            //this._fitness += this._score * 1000;
            //this._fitness += this._score * 20;
            double fitness = 0;
            if (this._score < 10)
                fitness = this._fitness * this._fitness * Math.Pow(2, this._score);
            else
                fitness = this._fitness * this._fitness * Math.Pow(2, 10) * (this._score - 9);

            return fitness;
        }

        /// <summary>
        /// Start new run
        /// </summary>
        private void StartRun()
        {
            this._snake.Clear();
            this._snake.Add(new Point((int)(this._width / 2), (int)(this._height / 2)));
            this._snake.Add(new Point(this._snake[0].X, this._snake[0].Y - 1));
            this._snake.Add(new Point(this._snake[0].X, this._snake[0].Y - 2));
            this._snake.Add(new Point(this._snake[0].X, this._snake[0].Y - 3));
            this._snake.Add(new Point(this._snake[0].X, this._snake[0].Y - 4));

            this._random = new Random();
            this.PlaceFood();

            this._direction = Direction.Down;
            this._lastDirection = Direction.Down;

            Dispatcher.Invoke((Action)(() =>
            {
                this.UpdateScreenSnake();
            }));

            this._score = 0;
            this._gameOver = false;
        }

        /// <summary>
        /// Place new food
        /// </summary>
        private void PlaceFood()
        {
            this._food = new Point(this._random.Next(0, this._width), this._random.Next(0, this._height));
        }

        /// <summary>
        /// Game over
        /// </summary>
        private void GameOver()
        {
            this._gameOver = true;
        }

        /// <summary>
        /// Update snake
        /// </summary>
        private void UpdateSnake()
        {
            for (int i = this._snake.Count - 1; i > 0; i--)
            {
                Point tail = this._snake[i];
                tail.X = this._snake[i - 1].X;
                tail.Y = this._snake[i - 1].Y;
                this._snake[i] = tail;
            }
            Point head = this._snake[0];
            switch (this._direction)
            {
                case Direction.Right: head.X++; break;
                case Direction.Left:  head.X--; break;
                case Direction.Down:  head.Y++; break;
                case Direction.Up:    head.Y--; break;
            }

            if (head.X >= this._width || head.Y >= this._height ||
                head.X < 0 || head.Y < 0)
                this.GameOver();

            for (int i = 1; i < this._snake.Count; i++)
            {
                if (head.X == this._snake[i].X && head.Y == this._snake[i].Y)
                    this.GameOver();
            }

            if (head.X == this._food.X && head.Y == this._food.Y)
            {
                this._snake.Add(new Point(head.X, head.Y));
                this._score++;
                this.PlaceFood();
            }
            this._snake[0] = head;
        }

        /// <summary>
        /// Update screen snake
        /// </summary>
        private void UpdateScreenSnake()
        {
            this._cnvSnake.Children.Clear();

            for (int i = 0; i < this._snake.Count; i++)
            {
                Rectangle tail = new Rectangle();
                tail.Stroke = new SolidColorBrush(i == 0 ? Colors.Black : Colors.Green);
                tail.Fill = new SolidColorBrush(i == 0 ? Colors.Black : Colors.Green);
                tail.Width = 12;
                tail.Height = 12;
                Canvas.SetLeft(tail, this._snake[i].X * 12);
                Canvas.SetTop(tail, this._snake[i].Y * 12);
                this._cnvSnake.Children.Add(tail);
            }

            Rectangle food = new Rectangle();
            food.Stroke = new SolidColorBrush(Colors.Red);
            food.Fill = new SolidColorBrush(Colors.Red);
            food.Width = 12;
            food.Height = 12;
            Canvas.SetLeft(food, this._food.X * 12);
            Canvas.SetTop(food, this._food.Y * 12);
            this._cnvSnake.Children.Add(food);

            this._lblFitness.Content = this._fitness;
            this._lblMaxFitness.Content = this._ga.MaxFitness;
            this._lblGeneration.Content = this._ga.CurrentGeneration;
            this._lblPopulation.Content = this._ga.CurrentPopulation;
            //this._lblMaxFitness.Content = this._ea.MaxFitness;
            //this._lblGeneration.Content = this._ea.CurrentGeneration;
            //this._lblSpecies.Content = this._ea.CurrentSpecies;
            //this._lblGenome.Content = this._ea.CurrentGenome;
        }

        /// <summary>
        /// Update screen network
        /// </summary>
        private void UpdateScreenNetwork()
        {
            this._cnvNetwork.Children.Clear();

            //Network network = this._ea.Pool.GetCurrentGenome().Network;

            //if (network == null)
            //    return;

            ////List<GuiNeuron> neuron = new List<GuiNeuron>();
            //GuiNeuron[] neuron = new GuiNeuron[network.Neurons.GetLength(0)];
            //GuiGene[] gene = new GuiGene[network.Genome.Genes.Count];

            //int j = 0;
            //for (int x = 0; x < 5; x++)
            //    for (int y = 0; y < 5; y++)
            //    {
            //        if (x == 2 && y == 2)
            //            continue;
            //        neuron[j] = new GuiNeuron(x, y, network.Neurons[j++].Value);
            //    }
            //neuron[j] = new GuiNeuron(4, 6, network.Neurons[j++].Value);

            //for (int i = j; i < this._ea.MaxNodes; i++)
            //{
            //    if (network.Neurons[j] != null)
            //        neuron[j] = new GuiNeuron(10 + (j / 8), (j % 8), network.Neurons[j++].Value);
            //    else
            //        j++;
            //}

            //for (int i = 0; i < this._ea.Outputs; i++)
            //{
            //    neuron[j] = new GuiNeuron(40, i, network.Neurons[j++].Value);
            //}

            
            //for (int i = 0; i < gene.GetLength(0); i++)
            //{
            //    Gene g = network.Genome.Genes[i];
            //    if (g.Enable)
            //        gene[i] = new GuiGene(neuron[g.Out].X, neuron[g.Out].Y, neuron[g.Into].X, neuron[g.Into].Y, g.Weight);
            //}

            //for (int i = 0; i < neuron.GetLength(0); i++)
            //{
            //    if (neuron[i] == null)
            //        continue;
            //    //if (neuron[i].Value == 0)
            //    //    continue;
            //    Rectangle n = new Rectangle();
            //    n.Stroke = new SolidColorBrush(Colors.Black);
            //    if (neuron[i].Value != 0)
            //        n.Fill = new SolidColorBrush(neuron[i].Value > 0 ? Colors.White : Colors.Black);
            //    n.Width = 12;
            //    n.Height = 12;
            //    Canvas.SetLeft(n, neuron[i].X * 12);
            //    Canvas.SetTop(n, neuron[i].Y * 12);
            //    this._cnvNetwork.Children.Add(n);
            //}

            //for (int i = 0; i < gene.GetLength(0); i++)
            //{
            //    if (gene[i] == null)
            //        continue;
            //    if (gene[i].Weight == 0)
            //        continue;
            //    Line g = new Line();
            //    g.X1 = gene[i].X1 * 12 + 6;
            //    g.Y1 = gene[i].Y1 * 12 + 6;
            //    g.X2 = gene[i].X2 * 12 + 6;
            //    g.Y2 = gene[i].Y2 * 12 + 6;
            //    g.Stroke = new SolidColorBrush(gene[i].Weight > 0 ? Colors.Green : Colors.Red);
            //    g.StrokeThickness = 1;
            //    this._cnvNetwork.Children.Add(g);
            //}

            //Rectangle s = new Rectangle();
            //s.Stroke = new SolidColorBrush(Colors.Black);
            //s.Fill = new SolidColorBrush(Colors.Red);
            //s.Width = 12;
            //s.Height = 12;
            //Canvas.SetLeft(s, 2 * 12);
            //Canvas.SetTop(s, 2 * 12);
            //this._cnvNetwork.Children.Add(s);
        }

        /// <summary>
        /// Keydown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.GameOver();
        }
    }
}
