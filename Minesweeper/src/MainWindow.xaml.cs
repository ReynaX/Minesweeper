using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Minesweeper
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly DispatcherTimer timer;
        private long time;
        /// <summary>
        ///     Initializes component and fills grid with default number of rows and cols.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += UpdateClock;
            timer.Interval = new TimeSpan(0, 0, 1);
            LevelSettings.Difficulty difficulty = LevelSettings.DefaultDifficulty;

            ChangeGridSize(difficulty);
        }

        private void UpdateMinesCounter()
        {
            int minesLeft = LevelSettings.GetLevelSettings(GameLogicController.Instance.Difficulty).Mines -
                            GameLogicController.Instance.MinesMarked;
            MinesLeftLabel.Text = (minesLeft).ToString("D3");
            if (minesLeft == 0)
                MinesLeftLabel.Foreground = Brushes.Aquamarine;
            else MinesLeftLabel.Foreground = Brushes.Red;
        }

        private void UpdateClock(object sender, EventArgs e)
        {
            if (timer.IsEnabled)
            {
                time += 1;
                TimerLabel.Text = time.ToString("D3");
            }
        }

        private void RestartClock()
        {
            timer.Stop();
            time = 0;
            TimerLabel.Text = time.ToString("D3");
        }
        /// <summary>
        ///     Method called when left mouse button was clicked on a button from a grid.
        /// </summary>
        private void OnLeftMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (GameLogicController.Instance.IsGameLost || GameLogicController.Instance.IsGameWon)
                return;

            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;

            if (GameLogicController.Instance.IsFirstMove)
            {
                GameLogicController.Instance.StartNewGame(button.Row, button.Col);
                timer.Start();
            }

            if (button.IsMarked)
                return;

            if (!button.IsEnabled)
                GameLogicController.Instance.MakeAction(GameLogicController.Action.RevealNeighbors, button);
            else
                GameLogicController.Instance.MakeAction(GameLogicController.Action.Reveal, button);
            CheckForEmojiChange();
        }

        /// <summary>
        ///     Method called when right mouse button was clicked on a button from a grid.
        /// </summary>
        private void OnRightMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (GameLogicController.Instance.IsGameLost || GameLogicController.Instance.IsGameWon)
                return;

            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;
            if (!button.IsEnabled)
                return;
            if (!button.IsMarked) GameLogicController.Instance.MakeAction(GameLogicController.Action.Mark, button);
            else GameLogicController.Instance.MakeAction(GameLogicController.Action.Unmark, button);
            UpdateMinesCounter();
            CheckForEmojiChange();
        }

        /// <summary>
        ///     Called when button with an emoji was clicked
        /// </summary>
        private void OnNewGameButtonClicked(object sender, RoutedEventArgs e)
        {
            ChangeGridSize(GameLogicController.Instance.Difficulty);
            RestartClock();
            UpdateMinesCounter();
        }

        /// <summary>
        ///     Called when any difficulty menu item was clicekd
        /// </summary>
        private void OnDifficultyChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckableMenuGroup item))
                return;

            if (item.Parent is ItemsControl ic)
            {
                CheckableMenuGroup rmi = ic.Items.OfType<CheckableMenuGroup>().First(i =>
                    i.GroupName == item.GroupName && i.IsChecked);
                rmi.IsChecked = false;

                item.IsChecked = true;
                GameLogicController.Instance.Difficulty = item.Difficulty;
            }
            OnNewGameButtonClicked(null, new RoutedEventArgs());
        }

        /// <summary>
        ///     Called whenever the game starts from the beginning.
        ///     Clear content of a grid and reinitializes rows and columns
        /// </summary>
        /// <param name="difficulty"> difficulty of the game to start </param>
        private void ChangeGridSize(LevelSettings.Difficulty difficulty)
        {
            ButtonsGrid.Children.Clear();
            ButtonsGrid.RowDefinitions.Clear();
            ButtonsGrid.ColumnDefinitions.Clear();

            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                ButtonsGrid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < cols; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                ButtonsGrid.ColumnDefinitions.Add(col);
            }

            GameLogicController.Instance.ResetGame(difficulty);
            for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
            {
                ContentControl control = new ContentControl();
                GridButton button = new GridButton(i, j);
                control.Content = button;
                control.PreviewMouseLeftButtonDown += OnLeftMouseButtonClicked;
                control.PreviewMouseRightButtonDown += OnRightMouseButtonClicked;

                Grid.SetRow(control, i);
                Grid.SetColumn(control, j);
                ButtonsGrid.Children.Add(control);

                GameLogicController.Instance.AddNewButton(button);
            }

            Image image = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/happy_emoji_icon.png"))
            };
            NewGameButton.Content = image;
        }

        /// <summary>
        ///     Checks if the game is whether won or lost and changes the emoji on new game button.
        /// </summary>
        private void CheckForEmojiChange()
        {
            if (GameLogicController.Instance.IsGameLost)
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/sad_emoji_icon.png"))
                };
                NewGameButton.Content = image;
                timer.Stop();
            }

            if (GameLogicController.Instance.IsGameWon)
            {
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/chad_emoji_icon.png"))
                };
                NewGameButton.Content = image;
                timer.Stop();
            }
        }

        private void OnQuitMenuItemClicked(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
