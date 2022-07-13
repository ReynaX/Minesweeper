using System.Windows;
using System.Windows.Controls;

namespace Minesweeper
{
    public class GridButton : Button
    {
        private readonly StackPanel panel;

        public GridButton(int row, int col)
        {
            panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            Row = row;
            Col = col;
            Content = panel;
        }

        public int Row { get; }
        public int Col { get; }
        public bool IsMarked { get; set; }

        /// <summary>
        ///     Clear current content of a button and stores a new given one.
        /// </summary>
        /// <typeparam name="T">Type of UIElement that can be stored inside a button(Textblock or image).</typeparam>
        /// <param name="content">Element to be stored inside a button.</param>
        public void SetContent<T>(T content) where T : UIElement
        {
            panel.Children.Clear();
            panel.Children.Add(content);
        }
    }
}