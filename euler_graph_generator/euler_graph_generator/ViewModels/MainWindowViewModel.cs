using euler_graph_generator.GraphElements;
using GraphSharp.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace euler_graph_generator.ViewModels
{
    //DO ZROBIENIA: 1. przycisk reset 2. zapis do pliku 3. naprawa grafu 
    //              4. podzielenie głównej metody na mniejsze funkcje 5. walidacja danych 6. lista algorytmów generowania grafu po polsku
    //              7. grafy nieskierowane 8. nagłówki do wierszy w macierzy incydencji 9. optymalizacja? xd 10. sprawozdanie :)

    //klasa odpowiedzialna za rysowanie -> taki canvas dla grafów
    public class GraphLayout : GraphLayout<Vertex, Edge, Graph>
    {
        public GraphLayout()
        {
            base.AnimationLength = TimeSpan.FromMilliseconds(0);
        }
    }
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        #region Private Data

        private DataTable _dataTable;
        private Random _random;
        private double[][] _matrix;
        #endregion

        #region Properties
        public List<string> LayoutAlgorithmTypes { get; } = new List<string>();

        private double ArraySingleValue = 0;

        private DataView _dataView;
        public DataView DataView
        {
            get
            {
                return _dataView;
            }
        }
       
        private int _numberOfVertices;
        public int NumberOfVertices
        {
            get { return _numberOfVertices; }
            set
            {
                _numberOfVertices = value;
                NotifyPropertyChanged("NumberOfVertices");
            }

        }


        private double _probabilityValue;
        public double ProbabilityValue
        {
            get { return _probabilityValue; }
            set
            {
                _probabilityValue = Math.Round(value, 2);
                NotifyPropertyChanged("ProbabilityValue");
            }

        }

        private string _layoutAlgorithmType;
        public string LayoutAlgorithmType
        {
            get { return _layoutAlgorithmType; }
            set
            {
                _layoutAlgorithmType = value;
                NotifyPropertyChanged("LayoutAlgorithmType");
            }
        }

        private Graph _graph;
        public Graph Graph
        {
            get { return _graph; }
            set
            {
                _graph = value;
                NotifyPropertyChanged("Graph");
            }
        }
        #endregion

        #region Ctor
        public MainWindowViewModel()
        {
            _dataTable = new DataTable();
            Graph = new Graph(true);
            _random = new Random();

            //Algorytmy rysowania/generowania grafów
            LayoutAlgorithmTypes.Add("BoundedFR");
            LayoutAlgorithmTypes.Add("Circular");
            LayoutAlgorithmTypes.Add("CompoundFDP");
            LayoutAlgorithmTypes.Add("EfficientSugiyama");
            LayoutAlgorithmTypes.Add("FR");
            LayoutAlgorithmTypes.Add("ISOM");
            LayoutAlgorithmTypes.Add("KK");
            LayoutAlgorithmTypes.Add("LinLog");
            LayoutAlgorithmTypes.Add("Tree");

            //Domyślny algorytm
            LayoutAlgorithmType = "Circular";
        }
        #endregion

        public void HideEdge(string from, string to)
        {
            Graph.Edges.Where(e => e.Source.VertexValue == from && e.Target.VertexValue == to).FirstOrDefault().EdgeVisibility = Visibility.Hidden;
        }

        public void ReLayoutGraph()
        {
            Graph = new Graph(true);
            _dataTable = new DataTable();
            _matrix = new double[_numberOfVertices][];

            //tablica(pusta) krawędzi/połączeń pomiędzy wierzchołkami
            for (int i = 0; i < _numberOfVertices; i++)
            {
                _matrix[i] = new double[_numberOfVertices];
            }

            //lista wierzchołków(pusta)
            List<Vertex> existingVertices = new List<Vertex>();

            //wygenerowanie odpowiedniej ilości wierzchołków
            for (int i = 0; i < _numberOfVertices; i++)
            {
                existingVertices.Add(new Vertex((i+1).ToString()));
            }

            //dodanie wierzchołków do głównego grafu
            foreach (Vertex vertex in existingVertices)
                Graph.AddVertex(vertex);

            //utworzenie kolumn z nagłówkami do macierzy 
            for (int i = 0; i < _numberOfVertices; i++)
            {
                _dataTable.Columns.Add(new DataColumn((i+1).ToString()));
            }


            //losowanie krawędzi(połączeń) w grafie
            int j = 0;
            for (int i = 0; i < _numberOfVertices; i++)
            {
                j = i;
                while (j < _numberOfVertices)
                {
                    ArraySingleValue = _random.NextDouble();
                    if (i != j)
                    {
                        if (ArraySingleValue <= ProbabilityValue)
                        {
                            ArraySingleValue = 1;
                            
                        }
                        else
                        {
                            ArraySingleValue = 0;
                        }
                    }
                    else
                    {
                        ArraySingleValue = 0;
                    }
                    _matrix[i][j] = ArraySingleValue;
                    j++;
                }
                
            }
            
            //dodanie krawędzi między wierzchołkami
            for (int i = 0; i < _numberOfVertices; i++)
            {
                j = i;
                while (j < _numberOfVertices)
                {
                    if (_matrix[i][j] == 1)
                    {
                        AddNewGraphEdge(existingVertices[i], existingVertices[j]);
                    }
                    j++;
                }
            }

            //uzupełnienie drugiej strony tablicy
            for (int i = 0; i < _numberOfVertices; i++)
            {
                for (j = 0; j < i ; j++)
                {
                    _matrix[i][j] = _matrix[j][i];
                }
            }

            //wpisanie danych do macierzy
            for (int i = 0; i < _numberOfVertices; i++)
            {
                var newRow = _dataTable.NewRow();
                for (j = 0; j < _numberOfVertices; j++)
                {
                    newRow[j] = _matrix[j][i];
                }
                _dataTable.Rows.Add(newRow);
            }

            //dane do macierzy połączeń
            _dataView = _dataTable.DefaultView;

            //odświeżenie interfejsu
            NotifyPropertyChanged("Graph");
            NotifyPropertyChanged("DataView");

        }

        #region Private Methods
        

        private Edge AddNewGraphEdge(Vertex from, Vertex to)
        {
            string edgeString = string.Format("Połączone wierzchołki: {0}-{1}", from.VertexValue, to.VertexValue);
            Color edgeColor = (_random.Next() % 2 == 0) ? Colors.Black : Colors.Red;
            Edge newEdge = new Edge(Visibility.Visible, edgeString, from, to);
            Graph.AddEdge(newEdge);
            return newEdge;
        }
        #endregion


        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion
    }
    public class EdgeVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (Visibility)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
