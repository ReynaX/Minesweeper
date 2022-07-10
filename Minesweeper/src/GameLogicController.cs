using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Documents;

namespace Minesweeper
{
    internal class GameLogicController
    {
        public GameLogicController(LevelSettings.Difficulty difficulty)
        {
            StartNewGame(difficulty);
        }

        public LevelSettings.Difficulty Difficulty { get; private set; }

        public int[,] BoardValues { get; private set; }
        /// <summary>
        /// Start a new game with given difficulty
        /// </summary>
        /// <param name="difficulty"> difficulty of a new game </param>
        public void StartNewGame(LevelSettings.Difficulty difficulty)
        {
            Difficulty = difficulty;

            LevelSettings.Settings settings = LevelSettings.GetLevelSettings(difficulty);

            BoardValues = new int[settings.Rows, settings.Cols];

            GenerateMines(settings.Mines, settings.Rows, settings.Cols);
            GenerateNonMinesSquares(settings.Rows, settings.Cols);
        }

        public static bool IsValid(int r, int c, int rows, int cols)
        {
            return r >= 0 && r < rows && c >= 0 && c < cols;
        }
        /// <summary>
        /// Fill random squares with mines
        /// </summary>
        /// <param name="minesToGenerate"> number of mines to generate</param>
        /// <param name="rows"> number of rows in current game difficulty </param>
        /// <param name="cols"> number of columns in current game difficulty </param>
        private void GenerateMines(int minesToGenerate, int rows, int cols)
        {
            Random r = new Random();
            int minesGenerated = 0;
            while (minesGenerated < minesToGenerate)
            {
                int row = r.Next(0, (int)rows), col = r.Next(0, (int)cols);

                if (BoardValues[row, col] != 0)
                    continue;

                BoardValues[row, col] = -1;
                ++minesGenerated;
            }
        }
        /// <summary>
        /// Fill non mines squares with values that represent number of neighboring mines
        /// </summary>
        /// <param name="rows"> number of rows in current game difficulty </param>
        /// <param name="cols"> number of columns in current game difficulty </param>
        private void GenerateNonMinesSquares(int rows, int cols)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (BoardValues[i, j] == -1)
                        continue;

                    int counter = 0;
                    // Check all neighbors of a square
                    for (int adjRow = -1; adjRow <= 1; ++adjRow)
                    {
                        for (int adjCol = -1; adjCol <= 1; ++adjCol)
                        {
                            if (IsValid(i + adjRow, j + adjCol, rows, cols) &&
                                BoardValues[i + adjRow, j + adjCol] == -1)
                                ++counter;
                        }
                    }
                    BoardValues[i, j] = counter;
                }
            }
        }

        /// <summary>
        ///  Method called when square with value of 0  was clicked on
        /// </summary>
        /// <param name="row"> row number of clicked square </param>
        /// <param name="col"> col number of clicked square </param>
        /// <param name="rows"> number of rows in current game difficulty </param>
        /// <param name="cols"> number of columns in current game difficulty </param>
        /// <returns> List of indices of squares to reveal given by equation (row * cols + col)  </returns>
        public List<int> GetSquaresToReveal(int row, int col, int rows, int cols, bool[,] visited)
        {
            List<int> squaresToReveal = new List<int>();
            visited[row, col] = true;
            for (int adjRow = -1; adjRow <= 1; ++adjRow)
            {
                for (int adjCol = -1; adjCol <= 1; ++adjCol)
                {
                    if (!IsValid(row + adjRow, col + adjCol, rows, cols))
                        continue;
                    if (BoardValues[row + adjRow, col + adjCol] == 0 && visited[row + adjRow, col + adjCol] == false)
                    {
                        int index = (row + adjRow) * (int)cols + (col + adjCol); // index of square in grid
                        squaresToReveal.Add(index);
                        squaresToReveal.AddRange(GetSquaresToReveal(row + adjRow, col + adjCol, rows, cols, visited));
                    }else if (BoardValues[row + adjRow, col + adjCol] > 0 && visited[row + adjRow, col + adjCol] == false)
                    {
                        int index = (row + adjRow) * (int)cols + (col + adjCol); // index of square in grid
                        squaresToReveal.Add(index);
                        visited[row + adjRow, col + adjCol] = true;
                    }
                }
            }
            return squaresToReveal;
        }


    }
}