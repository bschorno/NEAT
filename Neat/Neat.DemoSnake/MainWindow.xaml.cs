using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Threading;

using Neat.EA;

namespace Neat.DemoSnake
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Point> _snake = new List<Point>();
        private Point _food;
        private Random _random;

        private EvolutionaryAlogorithm _ea;
        private Thread _eaThread;

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

            this._ea = new EvolutionaryAlogorithm(24, 2);
            this._ea.EvaluateNetwork = this.EvaluateNetwork;
            this._eaThread = new Thread(this._ea.Run);
            this._eaThread.SetApartmentState(ApartmentState.STA);
            this._eaThread.Start();
        }

        /// <summary>
        /// Evaluate network
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        private double EvaluateNetwork(Network network)
        {
            this._fitness = 0;

            this.StartRun();

            while (!this._gameOver)
            {
                double[] input = new double[24];
                double[] output = new double[2];

                Point head = this._snake[0];

                int j = 0;
                for (int x = -2; x <= 2; x++)
                    for (int y = -2; y <= 2; y++)
                    {
                        if (x == 0 && y == 0)
                            continue;

                        for (int i = 1; i < this._snake.Count; i++)
                        {
                            if (this._snake[i].X == head.X + x && this._snake[i].Y == head.Y + y)
                                input[j] = -1;
                        }

                        if (head.X + x >= this._width)
                            input[j] = -1;
                        if (head.X + x < 0)
                            input[j] = -1;
                        if (head.Y + y >= this._height)
                            input[j] = -1;
                        if (head.Y + y < 0)
                            input[j] = -1;

                        if (head.X + x == this._food.X && head.Y + y == this._food.Y)
                            input[j] = 1;

                        j++;                        
                    }

                output = network.Run(input);

                if (Math.Abs(output[0]) > Math.Abs(output[1]))
                    output[1] = 0;
                else
                    output[0] = 0;

                if (output[1] > 0 && this._lastDirection != Direction.Left)
                    this._direction = Direction.Right;
                else if (output[1] < 0 && this._lastDirection != Direction.Right)
                    this._direction = Direction.Left;
                else if (output[0] > 0 && this._lastDirection != Direction.Down)
                    this._direction = Direction.Up;
                else if (output[0] < 0 && this._lastDirection != Direction.Up)
                    this._direction = Direction.Down;

                this._lastDirection = this._direction;

                Dispatcher.Invoke((Action)(() =>
                {
                    this.UpdateScreenNetwork();
                }));

                this.UpdateSnake();

                this._fitness += 1;

                if (!this._gameOver)
                    Dispatcher.Invoke((Action)(() =>
                    {
                        this.UpdateScreenSnake();
                    }));
                
            
                Thread.Sleep(1);
            }

            this._fitness += this._score * 1000;

            return this._fitness;
        }

        /// <summary>
        /// Start new run
        /// </summary>
        private void StartRun()
        {
            this._snake.Clear();
            this._snake.Add(new Point((int)(this._width / 2), (int)(this._height / 2)));

            this._random = new Random(1997);
            this.PlaceFood();

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
            this._lblMaxFitness.Content = this._ea.MaxFitness;
            this._lblGeneration.Content = this._ea.CurrentGeneration;
            this._lblSpecies.Content = this._ea.CurrentSpecies;
            this._lblGenome.Content = this._ea.CurrentGenome;
        }

        /// <summary>
        /// Update screen network
        /// </summary>
        private void UpdateScreenNetwork()
        {
            this._cnvNetwork.Children.Clear();

            Network network = this._ea.Pool.GetCurrentGenome().Network;
            if (network == null)
                return;

            List<GuiNeuron> neuron = new List<GuiNeuron>();

            int j = 0;
            for (int x = 0; x < 5; x++)
                for (int y = 0; y < 5; y++)
                {
                    if (x == 2 && y == 2)
                        continue;
                    neuron.Add(new GuiNeuron(x, y, network.Neurons[j++].Value));
                }
            neuron.Add(new GuiNeuron(4, 6, network.Neurons[j++].Value));

            for (int i = 0; i < neuron.Count; i++)
            {
                if (neuron[i].Value == 0)
                    continue;
                Rectangle n = new Rectangle();
                n.Stroke = new SolidColorBrush(Colors.Black);
                n.Fill = new SolidColorBrush(neuron[i].Value > 0 ? Colors.White : Colors.Black);
                n.Width = 8;
                n.Height = 8;
                Canvas.SetLeft(n, (neuron[i].X + 1) * 8);
                Canvas.SetTop(n, (neuron[i].Y + 1) * 8);
                this._cnvNetwork.Children.Add(n);
            }

            Rectangle s = new Rectangle();
            s.Stroke = new SolidColorBrush(Colors.Black);
            s.Fill = new SolidColorBrush(Colors.Red);
            s.Width = 8;
            s.Height = 8;
            Canvas.SetLeft(s, 3 * 8);
            Canvas.SetTop(s, 3 * 8);
            this._cnvNetwork.Children.Add(s);
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

    internal struct GuiNeuron
    {
        public int X;
        public int Y;
        public double Value;

        public GuiNeuron(int x, int y, double value)
        {
            X = x;
            Y = y;
            Value = value;
        }
    }

    internal struct GuiGene
    {
        public int X;
        public int Y;
        public double Weight;

        public GuiGene(int x, int y, double weight)
        {
            X = x;
            Y = y;
            Weight = weight;
        }
    }
}
