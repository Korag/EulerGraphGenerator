using euler_graph_generator.GraphElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace euler_graph_generator.GraphMethods
{
    public static class EdgeMethod
    {
        //generowanie krawędzi na podstawie macierzy(matrix)
        public static void GenerateEdges(double[][] matrix, List<Vertex> existingVertices, Graph graph)
        {
            var numberOfVertices = matrix.Length;
            for (int i = 0; i < numberOfVertices; i++)
            {
                int j = i;
                while (j < numberOfVertices)
                {
                    if (matrix[i][j] == 1)
                    {
                        AddNewGraphEdge(existingVertices[i], existingVertices[j], graph);
                    }
                    j++;
                }
            }
        }
        //utworzenie obiektu krawędzi i dodanie go do grafu 
        private static void AddNewGraphEdge(Vertex from, Vertex to, Graph graph)
        {
            string edgeString = string.Format("Connected vertices: {0}-{1}", from.VertexValue, to.VertexValue);
            Edge newEdge = new Edge(edgeString, from, to);

            //sprawdzenie czy określona krawędź istnieje
            if (graph.Edges.Where(x=>x.Source == newEdge.Source && x.Target == newEdge.Target).FirstOrDefault() == null)
            {
                graph.AddEdge(newEdge);
            }
        }

    }
}
