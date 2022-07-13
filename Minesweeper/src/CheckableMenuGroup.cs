using System.Windows.Controls;

namespace Minesweeper
{
    public class CheckableMenuGroup : MenuItem
    {
        public string GroupName { get; set; }
        public LevelSettings.Difficulty Difficulty { get; set; }
    }
}