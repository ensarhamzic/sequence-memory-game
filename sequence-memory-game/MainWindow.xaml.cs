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
using System.Timers;

namespace sequence_memory_game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int score;
        private int bestScore;
        private bool game;
        private Random rand;
        private List<int> fields;
        private Timer showTimer;
        int counter;
        public MainWindow()
        {
            InitializeComponent();
            bestScore = -1;
            game = false;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            score = 0;
            counter = 0;
            StartGame.IsEnabled = false;
            fields = new List<int>();
            rand = new Random();
            AddNewField();

        }

        private void GameGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!game) return;
            string clickedName = (e.OriginalSource as Border).Name;
            int clickedNum = int.Parse(clickedName.Substring(1));
            if(counter == fields.Count-1 && fields[counter] == clickedNum)
            {
                game = false;
                AddNewField();
                score++;
                Score.Text = $"Score: {score}";
            } else if(fields[counter++] != clickedNum)
            {
                game = false;
                if(score > bestScore)
                {
                    bestScore = score;
                    BestScore.Text = $"Best Score: {bestScore}";
                    StartGame.IsEnabled = true;
                    MessageBox.Show($"You Lost. Your score is {score}", "You clicked on the wrong tile");
                }
            }
        }

        private void AddNewField()
        {
            counter = 0;
            int randomNum = rand.Next(1, 10);
            if(fields.Count > 0)
            {
                while (randomNum == fields[fields.Count - 1])
                {
                    randomNum = rand.Next(1, 10);
                }
            }
            fields.Add(randomNum);
            showTimer = new Timer(600);
            showTimer.AutoReset = true;
            showTimer.Elapsed += ShowTimer_Elapsed;
            showTimer.Enabled = true;

        }

        private void ShowTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(counter == fields.Count)
            {
                this.Dispatcher.Invoke(() =>
                {
                    Border prevB = GameGrid.FindName($"r{fields[counter-1]}") as Border;
                    prevB.Background = Brushes.LightGray;
                });
                game = true;
                showTimer.Enabled = false;
                counter = 0;
            } else
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (counter != 0)
                    {
                        Border prevB = GameGrid.FindName($"r{fields[counter - 1]}") as Border;
                        prevB.Background = Brushes.LightGray;
                    }
                    Border b = GameGrid.FindName($"r{fields[counter++]}") as Border;
                    b.Background = Brushes.Blue;
                });
            }
            
        }
    }
}
