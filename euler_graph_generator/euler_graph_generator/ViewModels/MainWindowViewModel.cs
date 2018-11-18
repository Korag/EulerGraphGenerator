using euler_graph_generator.GraphElements;
using euler_graph_generator.GraphMethods;
using GraphSharp.Controls;
using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
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
        private Stack<Vertex>[] selector = new Stack<Vertex>[3];//tablica listy dla wierzchołków o różnych stopniach(0 stopni, parzyste oraz nieprzyste)
        private DataTable _dataTable;
        private Random _random = new Random();
        private double[][] _matrix;
        private List<Vertex> _existingVertices;
        #endregion

        #region Properties
        public List<string> LayoutAlgorithmTypes { get; } = new List<string>();

        public DataView DataView { get; private set; }

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
        private UndirectedBidirectionalGraph<Vertex, Edge> _undirectedGraph;
        public UndirectedBidirectionalGraph<Vertex, Edge> UndirectedGraph
        {
            get { return _undirectedGraph; }
            set
            {
                _undirectedGraph = value;
                NotifyPropertyChanged("UndirectedGraph");
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

        public void RepairGraph()
        {
            var selector = GetVertexDegreeInfo(_matrix, _existingVertices);

            while (selector[1].Count>0)
            {
                while (selector[0].Count>0)
                {

                    if (selector[1].Count>=2)
                    {
                        var start = selector[0].Pop();
                        var end1 = selector[1].Pop();
                        var end2 = selector[1].Pop();

                        _matrix[start.Index][end1.Index] = 1;
                        _matrix[end1.Index][start.Index] = 1;
                        _matrix[start.Index][end2.Index] = 1;
                        _matrix[end2.Index][start.Index] = 1;
                    }
                    
                    selector = GetVertexDegreeInfo(_matrix, _existingVertices);

                    if (selector[1].Count == 0 && selector[0].Count>0)
                    {
                            var start = selector[0].Pop();
                            var end = selector[2].Pop();
                            _matrix[start.Index][end.Index] = 1;
                            _matrix[end.Index][start.Index] = 1;
                        if (end.Neighbors.Count>0)
                        {
                            _matrix[start.Index][end.Neighbors[0].Index] = 1;
                            _matrix[end.Neighbors[0].Index][start.Index] = 1;
                            _matrix[end.Index][end.Neighbors[0].Index] = 0;
                            _matrix[end.Neighbors[0].Index][end.Index] = 0;
                        }
                    }
                }

                while (selector[1].Count > 0)
                {
                    List<Vertex> connection = new List<Vertex>();
                    connection.Add(selector[1].Pop());
                    connection.Add(selector[1].Pop());

                    if (_matrix[connection[0].Index][connection[1].Index]==1)
                    {
                        if (connection[0].Neighbors.Count == 1 && connection[1].Neighbors.Count == 1)
                        {
                            _matrix[connection[0].Index][connection[1].Index] = 0;
                            _matrix[connection[1].Index][connection[0].Index] = 0;
                        }
                        else
                        {
                            if (connection[0].Neighbors.Count > 1)
                            {
                                var helpList = connection[0].Neighbors.Where(v => v.Index != connection[1].Index).ToList();

                                _matrix[connection[0].Index][helpList[0].Index] = 0;
                                _matrix[helpList[0].Index][connection[0].Index] = 0;
                            }
                            else
                            {
                                var helpList = connection[1].Neighbors.Where(v => v.Index != connection[0].Index).ToList();

                                if (helpList.Count>0 && connection.Count>0)
                                {
                                    _matrix[connection[1].Index][helpList[0].Index] = 0;
                                    _matrix[helpList[0].Index][connection[1].Index] = 0;
                                }

                            }
                        }
                    }
                    else
                    {
                        _matrix[connection[0].Index][connection[1].Index] = 1;
                        _matrix[connection[1].Index][connection[0].Index] = 1;
                    }
                    
                }

                selector = GetVertexDegreeInfo(_matrix, _existingVertices);
                if (selector[1].Count == 2)
                {
                    break;
                }
            }
            SetVertexNeighbors();
            GenerateEdges();
            NotifyPropertyChanged("Graph");
        }
            

        public void ReLayoutGraph()
        {
            MatrixMethod matrixM = new MatrixMethod(_numberOfVertices, ProbabilityValue);
            Graph = new Graph(true);
            _dataTable = new DataTable();
            _matrix = matrixM.Matrix;
            //lista wierzchołków(pusta)
            _existingVertices = new List<Vertex>();

            

            //wygenerowanie odpowiedniej ilości wierzchołków
            for (int i = 0; i < _numberOfVertices; i++)
            {
                _existingVertices.Add(new Vertex((i+1).ToString(),i));
                Graph.AddVertex(_existingVertices[i]);
            }

            int j = 0;
            //dodanie krawędzi między wierzchołkami
            GenerateEdges();
            //suma
            CalculateTheSum();

            SetVertexNeighbors();


            //dane do macierzy połączeń
            DataView = matrixM.DataTable.DefaultView;
            UndirectedGraph = new UndirectedBidirectionalGraph<Vertex, Edge>(Graph);
            //odświeżenie interfejsu
            NotifyPropertyChanged("Graph");
            NotifyPropertyChanged("UndirectedGraph");
            NotifyPropertyChanged("DataView");
            
        }



        #region Private Methods
        private void GenerateEdges()
        {
            for (int i = 0; i < _numberOfVertices; i++)
            {

                int j = i;
                while (j < _numberOfVertices)
                {
                    if (_matrix[i][j] == 1)
                    {
                        AddNewGraphEdge(_existingVertices[i], _existingVertices[j]);
                    }
                    j++;
                }
            }
        }

        private void SetVertexNeighbors()
        {
            for (int i = 0; i < _matrix.Length; i++)
            {
                _existingVertices[i].Neighbors = new List<Vertex>();
                for (int k = 0; k < _matrix[i].Length - 1; k++)
                {
                    if (_matrix[i][k] == 1)
                    {
                        _existingVertices[i].Neighbors.Add(_existingVertices[k]);
                    }
                }
            }
        }
        private void CalculateTheSum()
        {
            for (int i = 0; i < _numberOfVertices; i++)
            {
                double sum = 0;
                for (int j = 0; j < _numberOfVertices; j++)
                {
                    sum += _matrix[i][j];
                }
                _matrix[i][_numberOfVertices] = sum;
                _existingVertices[i].VertexDegree = (int)sum;
            }
        }
        private Stack<Vertex>[] GetVertexDegreeInfo(double[][] matrix, List<Vertex> verticesList)
        {

            Stack<Vertex>[] tempArray = new Stack<Vertex>[3];
            tempArray[0] = new Stack<Vertex>();
            tempArray[1] = new Stack<Vertex>();
            tempArray[2] = new Stack<Vertex>();
            CalculateTheSum();
            //BŁĄD: suma nie jest zmieniana przez co wierzchołki są niepotzebnie dodawane
            //rozwiązanie zrobić z obliczania sumy funkcję a jej wywoanie dodać na początku tej funkcji
            for (int i = 0; i < matrix.Length; i++)
            {
                var lastColumn = matrix[i].Length-1;
                //0 stopien
                if (_matrix[i][lastColumn] == 0)
                {
                    tempArray[0].Push(verticesList[i]);
                }
                //nieparzyste
                if (_matrix[i][lastColumn] % 2 == 1)
                {
                    tempArray[1].Push(verticesList[i]);
                }
                //parzyste
                if (_matrix[i][lastColumn] % 2 == 0)
                {
                    tempArray[2].Push(verticesList[i]);
                }
            }

            return tempArray;
        }

        private Edge AddNewGraphEdge(Vertex from, Vertex to)
        {
            string edgeString = string.Format("Connected vertices: {0}-{1}", from.VertexValue, to.VertexValue);
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

    #region Converters
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
    public class ConvertPath : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Debug.Assert(values != null && values.Length == 9, "EdgeRouteToPathConverter should have 9 parameters: pos (1,2), size (3,4) of source; pos (5,6), size (7,8) of target; routeInformation (9).");


            //get the position of the source
            Point sourcePos = new Point()
            {
                X = (values[0] != DependencyProperty.UnsetValue ? (double)values[0] : 0.0),
                Y = (values[1] != DependencyProperty.UnsetValue ? (double)values[1] : 0.0)
            };
            //get the size of the source
            Size sourceSize = new Size()
            {
                Width = (values[2] != DependencyProperty.UnsetValue ? (double)values[2] : 0.0),
                Height = (values[3] != DependencyProperty.UnsetValue ? (double)values[3] : 0.0)
            };
            //get the position of the target
            Point targetPos = new Point()
            {
                X = (values[4] != DependencyProperty.UnsetValue ? (double)values[4] : 0.0),
                Y = (values[5] != DependencyProperty.UnsetValue ? (double)values[5] : 0.0)
            };
            //get the size of the target
            Size targetSize = new Size()
            {
                Width = (values[6] != DependencyProperty.UnsetValue ? (double)values[6] : 0.0),
                Height = (values[7] != DependencyProperty.UnsetValue ? (double)values[7] : 0.0)
            };



            //get the position of the source

            Point[] routeInformation = (values[8] != DependencyProperty.UnsetValue ? (Point[])values[8] : null);

            bool hasRouteInfo = routeInformation != null && routeInformation.Length > 0;

            //
            // Create the path
            //
            Point p1 = GraphConverterHelper.CalculateAttachPoint(sourcePos, sourceSize, (hasRouteInfo ? routeInformation[0] : targetPos));
            Point p2 = GraphConverterHelper.CalculateAttachPoint(targetPos, targetSize, (hasRouteInfo ? routeInformation[routeInformation.Length - 1] : sourcePos));


            PathSegment[] segments = new PathSegment[1 + (hasRouteInfo ? routeInformation.Length : 0)];
            if (hasRouteInfo)
                //append route points
                for (int i = 0; i < routeInformation.Length; i++)
                    segments[i] = new LineSegment(routeInformation[i], true);

            Point pLast = (hasRouteInfo ? routeInformation[routeInformation.Length - 1] : p1);
            Vector v = pLast - p2;
            v = v / v.Length * 0.01;
            Vector n = new Vector(-v.Y, v.X) * 0.4;

            segments[segments.Length - 1] = new LineSegment(p2 + v, true);

            PathFigureCollection pfc = new PathFigureCollection(2);
            pfc.Add(new PathFigure(p1, segments, false));
            //pfc.Add(new PathFigure(p2,
            //                         new PathSegment[] {
            //                                            new LineSegment(p2 + v - n, true),
            //                                            new LineSegment(p2 + v + n, true)}, true));

            return pfc;
        }
        private class GraphConverterHelper
        {
            public static Point CalculateAttachPoint(Point s, Size sourceSize, Point t)
            {
                double[] sides = new double[4];
                sides[0] = (s.X - sourceSize.Width / 2.0 - t.X) / (s.X - t.X);
                sides[1] = (s.Y - sourceSize.Height / 2.0 - t.Y) / (s.Y - t.Y);
                sides[2] = (s.X + sourceSize.Width / 2.0 - t.X) / (s.X - t.X);
                sides[3] = (s.Y + sourceSize.Height / 2.0 - t.Y) / (s.Y - t.Y);

                double fi = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (sides[i] <= 1)
                        fi = Math.Max(fi, sides[i]);
                }

                return t + fi * (s - t);
            }
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

}
