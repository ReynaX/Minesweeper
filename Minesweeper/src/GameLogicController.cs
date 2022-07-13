using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    public sealed class GameLogicController
    {
        public enum Action
        {
            Mark,
            Unmark,
            Reveal,
            RevealNeighbors
        }

        private static GameLogicController instance;

        // Stores buttons from the grid
        private readonly List<GridButton> buttons;

        private GameLogicController(LevelSettings.Difficulty difficulty)
        {
            Difficulty = difficulty;
            buttons = new List<GridButton>(TilesRemaining);
            IsFirstMove = true;
        }

        public static GameLogicController Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameLogicController(LevelSettings.Difficulty.Beginner);
                return instance;
            }
        }

        public LevelSettings.Difficulty Difficulty { get; private set; }

        public bool IsGameLost { get; private set; }
        public bool IsGameWon { get; private set; }
        public bool IsFirstMove { get; private set; }
        public int TilesRemaining { get; private set; }
        public int MinesMarked { get; private set; }

        private int[,] BoardValues { get; set; }

        /// <summary>
        ///     Start a new game with given difficulty. Button in given row and column will have a value 0.
        /// </summary>
        /// <param name="rowClicked"> row of a button that has been clicked by a user </param>
        /// <param name="colClicked"> col of a button that has been clicked by a user </param>
        public void StartNewGame(int rowClicked, int colClicked)
        {
            IsGameLost = false;
            IsGameWon = false;
            IsFirstMove = false;
            LevelSettings.Settings settings = LevelSettings.GetLevelSettings(Difficulty);

            BoardValues = new int[settings.Rows, settings.Cols];
            TilesRemaining = settings.Rows * settings.Cols;
            MinesMarked = 0;

            GenerateMines(settings.Mines, rowClicked, colClicked);
            GenerateNonMinesSquares();
        }

        public void AddNewButton(GridButton newButton)
        {
            buttons.Add(newButton);
        }

        private static bool IsValid(int r, int c, int rows, int cols)
        {
            return r >= 0 && r < rows && c >= 0 && c < cols;
        }

        public void MakeAction(Action action, GridButton buttonClicked)
        {
            switch (action)
            {
                case Action.Mark:
                    MarkSquare(buttonClicked);
                    break;
                case Action.Unmark:
                    UnmarkSquare(buttonClicked);
                    break;
                case Action.Reveal:
                    AssignContentOfButton(buttonClicked);
                    break;
                case Action.RevealNeighbors:
                    RevealNeighbors(buttonClicked);
                    break;
            }

            if (TilesRemaining - MinesMarked == 0 &&
                MinesMarked == LevelSettings.GetLevelSettings(Difficulty).Mines)
                IsGameWon = true;
        }

        /// <summary>
        ///     Mark given button, change its content and update number of mines marked
        /// </summary>
        private void MarkSquare(GridButton button)
        {
            button.IsMarked = true;
            ++MinesMarked;
            // Load bitmap image for a marked button
            Image image = new Image
            {
                Source = new BitmapImage(new Uri("pack://application:,,,/Resources/flag_icon.png"))
            };
            button.SetContent(image);
        }

        /// <summary>
        ///     Unmark given button, change its content and update number of mines marked
        /// </summary>
        private void UnmarkSquare(GridButton button)
        {
            button.IsMarked = false;
            --MinesMarked;
            button.SetContent(new UIElement());
        }

        /// <summary>
        ///     Called whenever already revealed button was clicked.
        /// </summary>
        /// <param name="buttonClicked"></param>
        private void RevealNeighbors(GridButton buttonClicked)
        {
            int rowClicked = buttonClicked.Row, colClicked = buttonClicked.Col;
            bool numberOfMarksCorrect = CountMarkedSquares(rowClicked, colClicked);
            if (numberOfMarksCorrect)
            {
                bool areMarksCorrect = CheckForMarkedSquares(rowClicked, colClicked);
                if (!areMarksCorrect)
                {
                    IsGameLost = true;
                    MineClicked();
                    return;
                }

                int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
                int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
                List<int> squaresToReveal =
                    GetSquaresToReveal(rowClicked, colClicked, rows, cols, new bool[rows, cols]);
                RevealSquares(squaresToReveal);
            }
        }

        /// <summary>
        ///     Fill random squares with mines. Squares adjacent to square given by parameters <code>rowClicked, colClicked</code>
        ///     can't have a mine.
        /// </summary>
        /// <param name="minesToGenerate"> number of mines to generate</param>
        /// <param name="rowClicked"> row of a button that has been clicked by a user </param>
        /// <param name="colClicked"> col of a button that has been clicked by a user </param>
        private void GenerateMines(int minesToGenerate, int rowClicked, int colClicked)
        {
            int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            Random r = new Random();
            int minesGenerated = 0;
            while (minesGenerated < minesToGenerate)
            {
                int row = r.Next(0, rows), col = r.Next(0, cols);

                if (BoardValues[row, col] != 0 || (Math.Abs(row - rowClicked) <= 1 && Math.Abs(col - colClicked) <= 1))
                    continue;

                BoardValues[row, col] = -1;
                ++minesGenerated;
            }
        }

        /// <summary>
        ///     Reveals buttons on positions in a grid given as a list of indices.
        ///     Disables the button and fills button with a number if that values is different than 0.
        /// </summary>
        /// <param name="squaresToReveal"></param>
        private void RevealSquares(List<int> squaresToReveal)
        {
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            foreach (int square in squaresToReveal)
            {
                int r = square / cols, c = square % cols;
                GridButton button = buttons[r * cols + c];
                if (!button.IsEnabled)
                    continue;
                int value = BoardValues[r, c];
                if (value != 0)
                {
                    TextBlock block = new TextBlock
                    {
                        Text = value.ToString(),
                        Foreground = Brushes.Green,
                        FontWeight = FontWeights.UltraBold
                    };
                    button.SetContent(block);
                }

                --TilesRemaining;
                button.IsEnabled = false;
            }
        }

        /// <summary>
        ///     Counts number of marked squares around a given square.
        /// </summary>
        /// <param name="rowClicked"> row number of clicked square </param>
        /// <param name="colClicked"> col number of clicked square </param>
        /// <returns>True if number of marked squares around a given squares is the same as value of that square, false otherwise</returns>
        private bool CountMarkedSquares(int rowClicked, int colClicked)
        {
            int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            int minesCount = BoardValues[rowClicked, colClicked];
            int counter = 0;
            for (int i = -1; i <= 1; ++i)
            for (int j = -1; j <= 1; ++j)
            {
                int r = rowClicked + i, c = colClicked + j;
                if (!IsValid(r, c, rows, cols))
                    continue;
                GridButton button = buttons[r * cols + c];
                if (button.IsMarked)
                    ++counter;
            }

            return counter == minesCount;
        }

        /// <summary>
        ///     Checks if marked squares have value equal to -1. Method should be only if <code>CountMakredSquares</code>
        ///     returns true.
        /// </summary>
        /// <param name="rowClicked"> row number of clicked square </param>
        /// <param name="colClicked"> col number of clicked square </param>
        /// <returns>True if all marked squares around a given squares have value equal to -1, false otherwise </returns>
        private bool CheckForMarkedSquares(int rowClicked, int colClicked)
        {
            int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            bool areMarkedCorrect = true;

            for (int i = -1; i <= 1; ++i)
            for (int j = -1; j <= 1; ++j)
            {
                int r = rowClicked + i, c = colClicked + j;
                if (!IsValid(r, c, rows, cols))
                    continue;
                GridButton button = buttons[r * cols + c];

                if (!button.IsMarked)
                    continue;
                // Check if square is marked rightfully
                if (BoardValues[rowClicked + i, colClicked + j] != -1)
                {
                    areMarkedCorrect = false;
                    break;
                }
            }

            return areMarkedCorrect;
        }

        /// <summary>
        ///     Fill non mines squares with values that represent number of neighboring mines.
        /// </summary>
        private void GenerateNonMinesSquares()
        {
            int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
            {
                if (BoardValues[i, j] == -1)
                    continue;

                int counter = 0;
                // Check all neighbors of a square
                for (int adjRow = -1; adjRow <= 1; ++adjRow)
                for (int adjCol = -1; adjCol <= 1; ++adjCol)
                    if (IsValid(i + adjRow, j + adjCol, rows, cols) &&
                        BoardValues[i + adjRow, j + adjCol] == -1)
                        ++counter;

                BoardValues[i, j] = counter;
            }
        }

        /// <summary>
        ///     Method called when square with value of 0  was clicked on
        /// </summary>
        /// <param name="row"> row number of clicked square </param>
        /// <param name="col"> col number of clicked square </param>
        /// <param name="rows"> number of rows in current game difficulty </param>
        /// <param name="cols"> number of columns in current game difficulty </param>
        /// <param name="visited"> saves squares that have been visited already to avoid infinite recursion</param>
        /// <returns> List of indices of squares to reveal given by equation (row * cols + col)  </returns>
        private List<int> GetSquaresToReveal(int row, int col, int rows, int cols, bool[,] visited)
        {
            List<int> squaresToReveal = new List<int> { row * cols + col };
            visited[row, col] = true;
            for (int adjRow = -1; adjRow <= 1; ++adjRow)
            for (int adjCol = -1; adjCol <= 1; ++adjCol)
            {
                if (!IsValid(row + adjRow, col + adjCol, rows, cols))
                    continue;
                if (BoardValues[row + adjRow, col + adjCol] == 0 && visited[row + adjRow, col + adjCol] == false)
                {
                    int index = (row + adjRow) * cols + col + adjCol; // index of square in grid
                    squaresToReveal.Add(index);
                    squaresToReveal.AddRange(GetSquaresToReveal(row + adjRow, col + adjCol, rows, cols, visited));
                }
                else if (BoardValues[row + adjRow, col + adjCol] > 0 &&
                         visited[row + adjRow, col + adjCol] == false)
                {
                    int index = (row + adjRow) * cols + col + adjCol; // index of square in grid
                    squaresToReveal.Add(index);
                    visited[row + adjRow, col + adjCol] = true;
                }
            }

            return squaresToReveal;
        }

        /// <summary>
        ///     Assign correct UIElement to buttons
        /// </summary>
        /// <param name="buttonClicked"> clicked button </param>
        private void AssignContentOfButton(GridButton buttonClicked)
        {
            int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            int valueClicked = BoardValues[buttonClicked.Row, buttonClicked.Col];
            switch (valueClicked)
            {
                case -1:
                {
                    MineClicked();
                    break;
                }
                case 0:
                {
                    int rowClicked = buttonClicked.Row, colClicked = buttonClicked.Col;
                    List<int> squaresToReveal =
                        GetSquaresToReveal(rowClicked, colClicked, rows, cols, new bool[rows, cols]);
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
                    --TilesRemaining;
                    buttonClicked.SetContent(block);
                    buttonClicked.IsEnabled = false;
                    break;
                }
            }
        }

        /// <summary>
        ///     Executed when mine was clicked
        /// </summary>
        private void MineClicked()
        {
            int rows = LevelSettings.GetLevelSettings(Difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(Difficulty).Cols;
            // Reveal all mines
            for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
                if (BoardValues[i, j] == -1)
                {
                    Image image = new Image
                    {
                        Source = new BitmapImage(new Uri("pack://application:,,,/Resources/mine_icon.png"))
                    };
                    int index = i * cols + j;
                    buttons[index].SetContent(image);
                }

            IsGameLost = true;
        }

        /// <summary>
        ///     Resets board
        /// </summary>
        /// <param name="difficulty"> difficulty of the game to start </param>
        public void ResetGame(LevelSettings.Difficulty difficulty)
        {
            buttons.Clear();

            Difficulty = difficulty;
            IsGameLost = false;
            IsGameWon = false;
            IsFirstMove = true;
            LevelSettings.Settings settings = LevelSettings.GetLevelSettings(Difficulty);
            BoardValues = new int[settings.Rows, settings.Cols];
            TilesRemaining = settings.Rows * settings.Cols;
            MinesMarked = 0;
        }
    }
}