
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Minesweeper
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //private readonly LevelSettings.Difficulty difficulty;

        /// <summary>
        ///     Initializes component and fills grid with default number of rows and cols.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            const LevelSettings.Difficulty difficulty = LevelSettings.Difficulty.Beginner;

            ChangeGridSize(difficulty);
        }

        /// <summary>
        ///     Method called when left mouse button was clicked on a button from a grid.
        /// </summary>
        private static void OnLeftMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (GameLogicController.Instance.IsGameLost)
                return;
            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;

            if (GameLogicController.Instance.IsFirstMove)
            {
                GameLogicController.Instance.StartNewGame(button.Row, button.Col);
            }

            if (button.IsMarked)
                return;

            if (!button.IsEnabled)
                GameLogicController.Instance.MakeAction(GameLogicController.Action.RevealNeighbors, button);
            else
                GameLogicController.Instance.MakeAction(GameLogicController.Action.Reveal, button);
            Trace.WriteLine("Tiles remaining: " + GameLogicController.Instance.TilesRemaining);
        }

        /// <summary>
        ///     Method called when right mouse button was clicked on a button from a grid.
        /// </summary>
        private static void OnRightMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (GameLogicController.Instance.IsGameLost)
                return;
            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;
            if (!button.IsEnabled)
                return;
            if (!button.IsMarked) GameLogicController.Instance.MakeAction(GameLogicController.Action.Mark, button);
            else GameLogicController.Instance.MakeAction(GameLogicController.Action.Unmark, button);
            Trace.WriteLine("Mines marked: " + GameLogicController.Instance.MinesMarked);
        }

        private void OnNewGameButtonClicked(object sender, RoutedEventArgs e)
        {
            ChangeGridSize(GameLogicController.Instance.Difficulty);
        }

        private void OnDifficultyChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is CheckableMenuGroup item))
                return;

            if (item.Parent is ItemsControl ic)
            {
                var rmi = ic.Items.OfType<CheckableMenuGroup>().First(i =>
                    i.GroupName == item.GroupName && i.IsChecked);
                rmi.IsChecked = false;

                item.IsChecked = true;
            }

            ChangeGridSize(item.Difficulty);
        }

        private void ChangeGridSize(LevelSettings.Difficulty difficulty)
        {
            ButtonsGrid.Children.Clear();
            ButtonsGrid.RowDefinitions.Clear();
            ButtonsGrid.ColumnDefinitions.Clear();

            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;
            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                ButtonsGrid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < cols; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                ButtonsGrid.ColumnDefinitions.Add(col);
            }

            GameLogicController.Instance.ResetGame(difficulty);
            for (int i = 0; i < rows; ++i)
            for (int j = 0; j < cols; ++j)
            {
                ContentControl control = new ContentControl();
                GridButton button = new GridButton(i, j);
                control.Content = button;
                control.PreviewMouseLeftButtonDown += OnLeftMouseButtonClicked;
                control.PreviewMouseRightButtonDown += OnRightMouseButtonClicked;

                Grid.SetRow(control, i);
                Grid.SetColumn(control, j);
                ButtonsGrid.Children.Add(control);

                GameLogicController.Instance.AddNewButton(button);
            }
        }
    }
}