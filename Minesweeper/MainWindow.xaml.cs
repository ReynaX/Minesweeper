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
            LevelSettings.Difficulty difficulty = LevelSettings.Difficulty.BEGINNER;
            GameLogicController logicController = new GameLogicController(difficulty);

            uint rows = LevelSettings.GetLevelSettings(difficulty).rows;
            uint cols = LevelSettings.GetLevelSettings(difficulty).cols;

            
            for(int i = 0; i < rows; ++i)
            {
                for(int j = 0; j < cols; ++j)
                {
                    Button button = new Button();
                    button.Width = 40;
                    button.Height = 40;
                    button.Content = logicController.BoardValues[i, j].ToString();
                    grid.Children.Add(button);
                }
            }
        }

        private void On_Board_Button_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            button.Content = button.Content + "D";
        }
    }
}
