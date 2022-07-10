using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly LevelSettings.Difficulty difficulty;

        private readonly GameLogicController logicController;

        public MainWindow()
        {
            InitializeComponent();
            difficulty = LevelSettings.Difficulty.Beginner;
            logicController = new GameLogicController(difficulty);

            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;


            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < cols; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    GridButton button = new GridButton(i,j);

                    button.PreviewMouseLeftButtonDown += OnLeftMouseButtonClicked;
                    button.PreviewMouseRightButtonDown += OnRightMouseButtonClicked;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    grid.Children.Add(button);
                }
            }
        }

        private void OnLeftMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is GridButton button))
                return;

            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            int rowClicked = button.Row;
            int colClicked = button.Col;
            int valueClicked = logicController.BoardValues[rowClicked, colClicked];

            if (button.IsEnabled == false)
            {
                // TODO: 
                bool isCorrect = CheckForMarkedSquares(rowClicked, colClicked);
                if(isCorrect)
                    Trace.WriteLine("Correct");
                else
                    Trace.WriteLine("Incorrect");
            }
            else
            {
                Trace.WriteLine(valueClicked);
                switch (valueClicked)
                {
                    case -1:
                    {
                        // Load bitmap image for mine
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mine_icon.png"))
                        };
                        button.SetContent(image);
                        break;
                    }
                    case 0:
                    {
                        var squaresToReveal = logicController.GetSquaresToReveal(rowClicked, colClicked, rows, cols, new bool[rows, cols]);
                        RevealSquares(squaresToReveal);
                        break;
                    }
                    default:
                    {
                        // Create a text block and add it to clicked button's stack panel
                        TextBlock block = new TextBlock
                        {
                            Text = valueClicked.ToString(),
                            Foreground = Brushes.Green,
                            FontWeight = FontWeights.UltraBold
                        };
                        button.SetContent(block);
                        break;
                    }
                }

                button.IsEnabled = false;
            }
        }

        private void OnRightMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is GridButton button))
                return;

            if (!button.IsMarked)
            {
                // Load bitmap image for a marked button
                Image image = new Image
                {
                    Source = new BitmapImage(new Uri("pack://application:,,,/Resources/flag_icon.png"))
                };
                button.SetContent(image);
                button.IsMarked = true;
            }
            else
            {
                button.SetContent(new UIElement());
                button.IsMarked = false;
            }
        }


        private void RevealSquares(List<int> squaresToReveal)
        {
            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            foreach (var square in squaresToReveal)
            {
                int r = square / rows, c = square % cols;
                // Check if given square is an enabled button
                if (!(grid.Children[square] is GridButton button) || !button.IsEnabled)
                    continue;
                int v = logicController.BoardValues[r, c];
                if (v != 0)
                {
                    TextBlock block = new TextBlock
                    {
                        Text = v.ToString(),
                        Foreground = Brushes.Green,
                        FontWeight = FontWeights.UltraBold
                    };
                    button.SetContent(block);
                }

                button.IsEnabled = false;
            }
        }


        public bool CheckForMarkedSquares(int rowClicked, int colClicked)
        {
            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            int minesCount = logicController.BoardValues[rowClicked, colClicked];
            int counter = 0;
            bool areMarkedCorrect = true;

            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (!GameLogicController.IsValid(rowClicked + i, colClicked + j, rows, cols))
                        continue;
                    int index = (rowClicked + i) * cols + (colClicked + j);
                    // Check if square is marked
                    if (!(grid.Children[index] is GridButton button) || !button.IsMarked)
                        continue;
                    ++counter;
                    // Check if square is marked rightfully
                    if (logicController.BoardValues[rowClicked + i, colClicked + j] != -1)
                    {
                        areMarkedCorrect = false;
                    }
                }
            }

            return areMarkedCorrect && counter == minesCount;
        }
    }
}