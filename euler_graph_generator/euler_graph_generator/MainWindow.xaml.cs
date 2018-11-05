using euler_graph_generator.ViewModels;
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

namespace euler_graph_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        public MainWindow()
        {
            vm = new MainWindowViewModel();
            this.DataContext = vm;
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
        }


        private void Generuj_Click(object sender, RoutedEventArgs e)
        {
            vm.ReLayoutGraph();
        }

        private void Napraw_Click(object sender, RoutedEventArgs e)
        {
            //vm.HideEdge("1","2");
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {

            //vm.RepairGraph();
        }
    }
}
