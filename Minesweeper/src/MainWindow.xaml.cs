using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            difficulty = LevelSettings.Difficulty.INTERMEDIATE;
            logicController = new GameLogicController(difficulty);

            uint rows = LevelSettings.GetLevelSettings(difficulty).rows;
            uint cols = LevelSettings.GetLevelSettings(difficulty).cols;


            for (int i = 0; i < rows; ++i)
            {
                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }
            for (int i = 0; i < cols; ++i)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            for (int i = 0; i < rows; ++i)
            {
                for(int j = 0; j < cols; ++j)
                {
                    Button button = new Button();
                    int value = logicController.BoardValues[i, j];



                    StackPanel panel = new StackPanel();
                    panel.Orientation = Orientation.Horizontal;
                    //if (value == -1)
                    //{
                    //    Image image = new Image();
                    //    image.Source =  new BitmapImage(new Uri("pack://application:,,,/Resources/mine_icon.png"));
                    //    panel.Children.Add(image);
                    //}
                    //else if (value != 0)
                    //{
                    //    TextBlock block = new TextBlock();
                    //    block.Text = value.ToString();
                    //    //block.FontSize = button.ActualWidth / 2;
                    //    block.Foreground = Brushes.Green;
                    //    block.FontWeight = FontWeights.UltraBold;
                    //    panel.Children.Add(block);
                    //}                        

                    button.Content = panel;
                    button.Tag = (int)((i * cols) + j);
                    button.Click += On_Board_Button_Clicked;
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, j);
                    grid.Children.Add(button);
                }
            }
        }

        private void On_Board_Button_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (sender as Button);
            if (button.IsEnabled == false)
            {
                // TODO: 
            }
            else
            {
                int buttonTag = (int)button.Tag;
                uint rowClicked = (uint)buttonTag / LevelSettings.GetLevelSettings(difficulty).rows;
                uint colClicked = (uint)buttonTag % LevelSettings.GetLevelSettings(difficulty).cols;
                button.Content = logicController.BoardValues[rowClicked, colClicked];
                button.IsEnabled = false;
            }
        }

        private GameLogicController logicController;
        private LevelSettings.Difficulty difficulty;
    }
}
