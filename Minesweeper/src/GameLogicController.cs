using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    class GameLogicController
    {
        public GameLogicController(LevelSettings.Difficulty difficulty)
        {
            StartNewGame(difficulty);
        }

        public LevelSettings.Difficulty Difficulty { get; private set; }

        public int[,] BoardValues { get; private set; }

        public void StartNewGame(LevelSettings.Difficulty difficulty)
        {
            Difficulty = difficulty;

            LevelSettings.Settings settings = LevelSettings.GetLevelSettings(difficulty);

            BoardValues = new int[settings.rows, settings.cols];

            GenerateMines(settings.mines, settings.rows, settings.cols);
            GenerateNonMinesSquares(settings.rows, settings.cols);
        }

        private bool IsValid(int r, int c, uint rows, uint cols)
        {
            if (r >= 0 && r < rows && c >= 0 && c < cols)
                return true;
            return false;
        }
        private void GenerateMines(uint minesToGenerate, uint rows, uint cols)
        {
            Random r = new Random();
            int minesGenerated = 0;
            while (minesGenerated <= minesToGenerate)
            {
                int row = r.Next(0, (int)rows), col = r.Next(0, (int)cols);

                if (BoardValues[row, col] != 0)
                    continue;

                BoardValues[row, col] = -1;
                ++minesGenerated;
            }
        }
        private void GenerateNonMinesSquares(uint rows, uint cols)
        {
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                {
                    if (BoardValues[i, j] == -1)
                        continue;

                    int counter = 0;
                    // Check all neighbors of a square
                    if (IsValid(i - 1, j - 1, rows, cols) && BoardValues[i - 1, j - 1] == -1)
                        ++counter;
                    if (IsValid(i - 1, j, rows, cols) && BoardValues[i - 1, j] == -1)
                        ++counter;
                    if (IsValid(i - 1, j + 1, rows, cols) && BoardValues[i - 1, j + 1] == -1)
                        ++counter;
                    if (IsValid(i, j + 1, rows, cols) && BoardValues[i, j + 1] == -1)
                        ++counter;
                    if (IsValid(i + 1, j + 1, rows, cols) && BoardValues[i + 1, j + 1] == -1)
                        ++counter;
                    if (IsValid(i + 1, j, rows, cols) && BoardValues[i + 1, j] == -1)
                        ++counter;
                    if (IsValid(i + 1, j - 1, rows, cols) && BoardValues[i + 1, j - 1] == -1)
                        ++counter;
                    if (IsValid(i, j - 1, rows, cols) && BoardValues[i, j - 1] == -1)
                        ++counter;
                    BoardValues[i, j] = counter;
                }
            }
        }
    }
}
