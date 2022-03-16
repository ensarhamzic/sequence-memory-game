using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Timers;
using System.Media;

namespace sequence_memory_game
{

    public partial class MainWindow : Window
    {
        private int score;
        private int bestScore;
        private bool game; // used to restrict user clicking
        private Random rand;
        private List<int> fields; // sequence of tiles
        private Timer showTimer; // timer to show tiles one after another
        int counter;
        SoundPlayer sound;
        public MainWindow()
        {
            InitializeComponent();
            bestScore = -1;
            game = false;
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            // Resetting stuff
            score = 0;
            counter = 0;
            StartGame.IsEnabled = false;
            fields = new List<int>();
            rand = new Random();
            AddNewField();
        }

        private async void GameGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!game) return;
            
            bool hit = false; // used for implementing visual feedback on clicked tile
            Border clickedBorder = e.OriginalSource as Border;
            string clickedName = clickedBorder.Name;
            int clickedNum = int.Parse(clickedName.Substring(1)); // Extract number from tile name
            if(counter == fields.Count-1 && fields[counter] == clickedNum) // if last tile and correct
            {
                hit = true;
                game = false;
                AddNewField();
                score++;
                Score.Text = $"Score: {score}";
            } else if(fields[counter++] != clickedNum) // Game over
            {
                playIncorrectSound();
                game = false;
                if(score > bestScore)
                {
                    bestScore = score;
                    BestScore.Text = $"Best Score: {bestScore}";
                }
                StartGame.IsEnabled = true;
                MessageBox.Show($"You Lost. Your score is {score}", "You clicked on the wrong tile");
            } else // correct tile but not last one
            {
                hit = true;
            }

            if(hit)
            {
                playCorrectSound();
                this.Dispatcher.Invoke(() =>
                {
                    clickedBorder.Background = Brushes.Green;
                });

                await Task.Delay(150);

                this.Dispatcher.Invoke(() =>
                {
                    clickedBorder.Background = Brushes.LightGray;
                });
            }
        }

        private void AddNewField()
        {
            counter = 0;
            int randomNum = rand.Next(1, 10);
            if(fields.Count > 0)
            {
                while (randomNum == fields[fields.Count - 1]) // two same numbers one after another not allowed
                {
                    randomNum = rand.Next(1, 10);
                }
            }
            fields.Add(randomNum);
            showTimer = new Timer(650); // Timer to show tiles one after another
            showTimer.AutoReset = true;
            showTimer.Elapsed += ShowTimer_Elapsed;
            showTimer.Enabled = true;
        }

        // Removes blue color from previous tile, adds blue color to current tile
        private void ShowTimer_Elapsed(object sender, ElapsedEventArgs e) 
        {
            if(counter == fields.Count) // Last one to show
            {
                this.Dispatcher.Invoke(() =>
                {
                    Border prevB = GameGrid.FindName($"r{fields[counter-1]}") as Border;
                    prevB.Background = Brushes.LightGray; // Remove color of previous
                });
                game = true;
                showTimer.Enabled = false;
                counter = 0;
            } else
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (counter != 0) // Not first one
                    {
                        Border prevB = GameGrid.FindName($"r{fields[counter - 1]}") as Border;
                        prevB.Background = Brushes.LightGray;
                    }
                    Border b = GameGrid.FindName($"r{fields[counter++]}") as Border;
                    b.Background = Brushes.Blue;
                });
            }
            
        }

        private void playCorrectSound()
        {
            sound = new SoundPlayer(Properties.Resources.correct);
            sound.Play();
        }

        private void playIncorrectSound()
        {
            sound = new SoundPlayer(Properties.Resources.incorrect);
            sound.Play();
        }
    }
}
