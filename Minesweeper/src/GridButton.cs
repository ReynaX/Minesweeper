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
        /// <summary>
        /// Clear current content of a button and stores a new given one.
        /// </summary>
        /// <typeparam name="T">Type of UIElement that can be stored inside a button(Textblock or image).</typeparam>
        /// <param name="content">Element to be stored inside a button.</param>
        public void SetContent<T>(T content) where T:UIElement
        {
            panel.Children.Clear();
            panel.Children.Add(content);
        }

    }
}
