using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Minesweeper
{
    class GridButton : Button
    {
        public GridButton(int row, int col)
        {
            panel = new StackPanel()
            {
                Orientation = Orientation.Horizontal
            };
            Row = row;
            Col = col;
            Content = panel;
        }

        public int Row { get; private set; }
        public int Col { get; private set; }
        public bool IsMarked { get; set; }

        private readonly StackPanel panel;

        public void SetContent<T>(T content) where T:UIElement
        {
            panel.Children.Clear();
            panel.Children.Add(content);
        }
    }
}
