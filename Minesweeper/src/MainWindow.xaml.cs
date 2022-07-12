using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private readonly LevelSettings.Difficulty difficulty;

        private readonly GameLogicController logicController;

        /// <summary>
        ///     Initializes component and fills grid with default number of rows and cols.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            const LevelSettings.Difficulty difficulty = LevelSettings.Difficulty.Beginner;
            logicController = new GameLogicController(difficulty);

            int rows = LevelSettings.GetLevelSettings(difficulty).Rows;
            int cols = LevelSettings.GetLevelSettings(difficulty).Cols;


            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                buttonsGrid.RowDefinitions.Add(row);
            }

            for (int i = 0; i < cols; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                buttonsGrid.ColumnDefinitions.Add(col);
            }

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
                buttonsGrid.Children.Add(control);

                logicController.AddNewButton(button);
            }
        }

        /// <summary>
        ///     Method called when left mouse button was clicked on a button from a grid.
        /// </summary>
        private void OnLeftMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (logicController.IsGameLost)
                return;
            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;

            if (logicController.IsFirstMove)
            {
                logicController.StartNewGame(LevelSettings.Difficulty.Beginner, button.Row, button.Col);
            }

            if (button.IsMarked)
                return;

            if (!button.IsEnabled)
                logicController.MakeAction(GameLogicController.Action.RevealNeighbors, button);
            else
                logicController.MakeAction(GameLogicController.Action.Reveal, button);
        }

        /// <summary>
        ///     Method called when right mouse button was clicked on a button from a grid.
        /// </summary>
        private void OnRightMouseButtonClicked(object sender, MouseButtonEventArgs e)
        {
            if (logicController.IsGameLost)
                return;
            if (!(sender is ContentControl cc) || !(cc.Content is GridButton button))
                return;
            if (!button.IsEnabled)
                return;
            if (!button.IsMarked) logicController.MakeAction(GameLogicController.Action.Mark, button);
            else logicController.MakeAction(GameLogicController.Action.Unmark, button);
        }

        private void OnNewGameButtonClicked(object sender, RoutedEventArgs e)
        {
            logicController.ResetSquares();
        }
    }
}