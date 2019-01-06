using euler_graph_generator.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace euler_graph_generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    /* 
     W sumie to tak:
     1. naprawa czasem nie działa(graf po naprawie jest albo niespójny(to zdarza się najczęściej) albo jest spójny ale nie eulerowski)
     2. Przycisk do powtórzenia kolorowania nie blokuje się, więc można parę razy kliknąć
         */
    public partial class MainWindow : Window
    {
        private bool isConnected = false;
        private bool isEuler = false;
        private string message = "przed naprawą";
        private MainWindowViewModel vm;

        public MainWindow()
        {
            vm = new MainWindowViewModel();
            this.DataContext = vm;
            this.WindowState = WindowState.Maximized;
            vm.worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            InitializeComponent();
        }
        //to się dzieje po skończeniu naprawy
        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!vm.worker.IsBusy)
            {
                isConnected = vm.DepthFirstSearch();
                if (isConnected)
                {
                    isEuler = vm.CheckIfEuler();
                    if (isEuler)
                    {
                        IsEuler.Content = "TAK";
                        IsEuler.Foreground = Brushes.Green;
                    }
                    else
                    {
                        IsEuler.Content = "NIE";
                        IsEuler.Foreground = Brushes.Red;
                        isEuler = vm.CheckIfEuler();
                        Generuj_Click(this,null);
                        vm.worker.RunWorkerAsync();
                    }
                    IsConnected.Content = "TAK";
                    IsConnected.Foreground = Brushes.Green;
                }
                else
                {
                    IsEuler.Content = "NIE";
                    IsEuler.Foreground = Brushes.Red;
                    IsConnected.Content = "NIE";
                    IsConnected.Foreground = Brushes.Red;
                    Generuj_Click(this, null);
                    vm.worker.RunWorkerAsync();
                    //isConnected = vm.DepthFirstSearch();
                }
                Napraw_graf.IsEnabled = true;
                Generuj.IsEnabled = true;
                message = "po naprawie";
                vm.SaveToFile(isConnected,isEuler,message, false);
                Zapisz.IsEnabled = true;
            }
        }



        private void Generuj_Click(object sender, RoutedEventArgs e)
        {
            IsEuler.Visibility = Visibility.Hidden;
            IsConnected.Visibility = Visibility.Hidden;
            vm.ResetData();
            vm.ReLayoutGraph();
            Reset.IsEnabled = true;
            if (vm.Graph.Vertices.Count()>1)
            {
                Napraw_graf.IsEnabled = true;
                Euler.IsEnabled = true;
            }
            
            if(vm.Graph.Vertices.Count()>1)
            {
                IsEuler.Visibility = Visibility.Visible;
                IsConnected.Visibility = Visibility.Visible;
                isConnected = vm.DepthFirstSearch();
                if (isConnected)
                {
                    IsConnected.Content = "TAK";
                    IsConnected.Foreground = Brushes.Green;
                    isEuler = vm.CheckIfEuler();
                    if (isEuler)
                    {
                        IsEuler.Content = "TAK";
                        IsEuler.Foreground = Brushes.Green;
                        Napraw_graf.IsEnabled = false;
                    }
                    else
                    {
                        IsEuler.Content = "NIE";
                        IsEuler.Foreground = Brushes.Red;
                        
                    }
                }
                else
                {
                    IsConnected.Content = "NIE";
                    IsConnected.Foreground = Brushes.Red;
                    IsEuler.Content = "NIE";
                    IsEuler.Foreground = Brushes.Red;
                }
            }
            message = "przed naprawą";
            vm.SaveToFile(isConnected, isEuler, message,true);
            Zapisz.IsEnabled = true;
        }

        private void Napraw_Click(object sender, RoutedEventArgs e)
        {
            Zapisz.IsEnabled = false;
            Generuj.IsEnabled = false;
            Napraw_graf.IsEnabled = false;
            vm.worker.RunWorkerAsync();
            
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            Generuj.IsEnabled = true;
            Reset.IsEnabled = false;
            Euler.IsEnabled = false;
            Zapisz.IsEnabled = false;
            Napraw_graf.IsEnabled = false;
            liczbaWierzcholkow.Value = 0;
            prawdopodobienstwo.Value = 0;

            IsEuler.Visibility = Visibility.Hidden;
            IsConnected.Visibility = Visibility.Hidden;
            vm.ResetData();
        }

        private void Zapisz_Click(object sender, RoutedEventArgs e)
        {
            Zapisz.IsEnabled = false;
            vm.SaveToFile(isConnected,isEuler,message,true);
        }

        private void Euler_Click(object sender, RoutedEventArgs e)
        {
            if (vm.Graph.Vertices.Count() > 1)
            {
                IsEuler.Visibility = Visibility.Visible;
                IsConnected.Visibility = Visibility.Visible;
                if (vm.DepthFirstSearch())
                {
                    IsConnected.Content = "TAK";
                    IsConnected.Foreground = Brushes.Green;
                    if (vm.CheckIfEuler())
                    {
                        IsEuler.Content = "TAK";
                        IsEuler.Foreground = Brushes.Green;
                       
                    }
                    else
                    {
                        IsEuler.Content = "NIE";
                        IsEuler.Foreground = Brushes.Red;

                    }
                }
                else
                {
                    IsConnected.Content = "NIE";
                    IsConnected.Foreground = Brushes.Red;
                    IsEuler.Content = "NIE";
                    IsEuler.Foreground = Brushes.Red;
                }
            }
        }


    }
}
