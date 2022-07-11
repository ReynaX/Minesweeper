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
        /// <summary>
        /// Initializes component and fills grid with default number of rows and cols.
        /// </summary>
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
                    ContentControl control = new ContentControl();
                    GridButton button = new GridButton(i,j);
                    control.Content = button;

                    control.PreviewMouseLeftButtonDown += OnLeftMouseButtonClicked;
                    control.PreviewMouseRightButtonDown += OnRightMouseButtonClicked;
                    Grid.SetRow(control, i);
                    Grid.SetColumn(control, j);
                    grid.Children.Add(control);
                }
            }
        }
        /// <summary>
        /// Method called when left mouse button was clicked on a button from a grid.
        /// </summary>
        private void OnLeftMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;

            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            int rowClicked = button.Row;
            int colClicked = button.Col;
            int valueClicked = logicController.BoardValues[rowClicked, colClicked];

            if (button.IsEnabled == false)
                FindNeigboringSquaresToReveal(rowClicked, colClicked);
            else
            {
                AssignContnetOfButton(button, valueClicked);
            }
        }
        /// <summary>
        /// Method called when right mouse button was clicked on a button from a grid.
        /// </summary>
        private void OnRightMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;
            if (!button.IsEnabled)
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

        /// <summary>
        /// Reveals buttons on positions in a grid given as a list of indices.
        /// Disables the button and fills button with a number if that values is different than 0.
        /// </summary>
        /// <param name="squaresToReveal"></param>
        private void RevealSquares(List<int> squaresToReveal)
        {
            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            foreach (var square in squaresToReveal)
            {
                int r = square / rows, c = square % cols;
                // Check if given square is an enabled button
                if (!(grid.Children[square] is ContentControl cc) || !(cc.Content is GridButton button))
                    return;
                if (!button.IsEnabled)
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


        private bool CheckForMarkedSquares(int rowClicked, int colClicked)
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
                    if (!(grid.Children[index] is ContentControl cc) || !(cc.Content is GridButton button))
                        continue;

                    if (!button.IsMarked)
                        continue;
                    ++counter;
                    // Check if square is marked rightfully
                    if (logicController.BoardValues[rowClicked + i, colClicked + j] != -1)
                    {
                        areMarkedCorrect = false;
                        break;
                    }
                }
            }

            return areMarkedCorrect && counter == minesCount;
        }

        private void FindNeigboringSquaresToReveal(int rowClicked, int colClicked)
        {
            bool isCorrect = CheckForMarkedSquares(rowClicked, colClicked);
            if (isCorrect){
                int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
                int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
                List<int> squaresToReveal = new List<int>();
                for (int i = -1; i <= 1; ++i)
                {
                    for (int j = -1; j <= 1; ++j)
                    {
                        if (!GameLogicController.IsValid(rowClicked + i, colClicked + j, rows, cols))
                            continue;
                        int index = (rowClicked + i) * cols + (colClicked + j);
                        if (!(grid.Children[index] is ContentControl cc1) || !(cc1.Content is GridButton b))
                            continue;
                        if (b.IsMarked || !b.IsEnabled)
                            continue;
                        squaresToReveal.Add(index);
                    }
                }
                RevealSquares(squaresToReveal);
            }
        }

        private void AssignContnetOfButton(GridButton buttonClicked, int valueClicked)
        {
            switch (valueClicked)
            {
                case -1:
                {
                    // Load bitmap image for mine
                    Image image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mine_icon.png"))
                    };
                    buttonClicked.SetContent(image);
                    break;
                }
                case 0:
                {
                    int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
                    int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
                    int rowClicked = buttonClicked.Row, colClicked = buttonClicked.Col;
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
                    buttonClicked.SetContent(block);
                    break;
                }
            }

            buttonClicked.IsEnabled = false;
        }
    }
}