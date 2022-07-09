using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minesweeper
{
    public static class LevelSettings
    {
        public enum Difficulty : uint { BEGINNER = 0, INTERMEDIATE, EXPERT };

        private static Dictionary<Difficulty, Settings> levelSettings;
        private static bool areLevelSettingsLoaded = false;

        public static Settings GetLevelSettings(Difficulty difficulty)
        {
            LoadLevelSettings();
            return levelSettings[difficulty];
        }

        public static void LoadLevelSettings()
        {
            if (areLevelSettingsLoaded)
                return;

            levelSettings = new Dictionary<Difficulty, Settings>
            {
                { Difficulty.BEGINNER, new Settings(8, 8, 10)},
                { Difficulty.INTERMEDIATE, new Settings(16, 16, 40)},
                { Difficulty.EXPERT, new Settings(30, 16, 99)},
            };
            areLevelSettingsLoaded = true;
        }

        public readonly struct Settings
        {
            public Settings(uint rows, uint cols, uint mines)
            {
                this.rows = rows;
                this.cols = cols;
                this.mines = mines;
                tiles = rows * cols;
            }

            public readonly uint tiles;
            public readonly uint rows;
            public readonly uint cols;
            public readonly uint mines;
        }
    }
}
