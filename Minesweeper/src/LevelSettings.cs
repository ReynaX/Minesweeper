using System.Collections.Generic;

namespace Minesweeper
{
    public static class LevelSettings
    {
        public enum Difficulty : uint
        {
            Beginner = 0,
            Intermediate,
            Expert
        }

        public static Difficulty DefaultDifficulty => Difficulty.Beginner;

        private static Dictionary<Difficulty, Settings> levelSettings;
        private static bool areLevelSettingsLoaded;

        public static Settings GetLevelSettings(Difficulty difficulty)
        {
            LoadLevelSettings();
            return levelSettings[difficulty];
        }

        private static void LoadLevelSettings()
        {
            if (areLevelSettingsLoaded)
                return;

            levelSettings = new Dictionary<Difficulty, Settings>
            {
                { Difficulty.Beginner, new Settings(8, 8, 10) },
                { Difficulty.Intermediate, new Settings(16, 16, 40) },
                { Difficulty.Expert, new Settings(30, 16, 99) }
            };
            areLevelSettingsLoaded = true;
        }

        public readonly struct Settings
        {
            public Settings(int rows, int cols, int mines)
            {
                Rows = rows;
                Cols = cols;
                Mines = mines;
                Tiles = rows * cols;
            }

            public readonly int Tiles;
            public readonly int Rows;
            public readonly int Cols;
            public readonly int Mines;
        }
    }
}