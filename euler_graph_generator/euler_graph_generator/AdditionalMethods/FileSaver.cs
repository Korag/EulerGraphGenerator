using euler_graph_generator.GraphElements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace euler_graph_generator.AdditionalMethods
{
    public static class FileSaver
    {
        public static string[] tempData;
        public static bool firstTime;

        public static void SaveToFile(Graph graph, double probability, double[][] incidenceMatrix, bool isConsistent, string isEuler, string message, bool deleteFile, List<int> eulerPath)
        {
            string filePath = "output.txt";

            if (deleteFile)
            {
                File.WriteAllText(filePath, "");
            }
            else
            {
                File.AppendAllText(filePath, "");
            }


            if (firstTime == false)
            {
                for (int i = 0; i < tempData.Length; i++)
                {
                    File.AppendAllText(filePath, tempData[i] + "\r\n");
                }
                if (message == "Before repair")
                {
                    return;
                }
            }


            string CurrentDate = "Date of generation: " + DateTime.Now + "\r\n";
            string School = "University of Bielsko-Biala - 50 years of tradition \r\n";
            string WorkingGroup = "Łukasz Czepielik, Kamil Haręża, Konrad Korzonkiewicz, Bartosz Wróbel \r\n\r\n";

            File.AppendAllText(filePath, CurrentDate);
            if (new FileInfo(filePath).Length < 30)
            {
                File.AppendAllText(filePath, School);
                File.AppendAllText(filePath, WorkingGroup);
            }



            string IsConsistent = isConsistent ? "YES" : "NO";

            File.AppendAllText(filePath, "Graph state: " + message + "\r\n");
            File.AppendAllText(filePath, "Number of vertices in the graph: " + graph.Vertices.Count() + "\r\n");
            File.AppendAllText(filePath, "Number of edges in the graph: " + graph.Edges.Count() + "\r\n");
            File.AppendAllText(filePath, "The plausibility of creating an edge: " + probability + "\r\n");
            File.AppendAllText(filePath, "Graph Consistent: " + IsConsistent + "\r\n");
            File.AppendAllText(filePath, "Eulerian graph: " + isEuler + "\r\n\r\n");


            File.AppendAllText(filePath, "Incident matrix " + message + ": \r\n\r\n");

            string Matrix = "";
            if (incidenceMatrix != null)
            {
                for (int i = 0; i < incidenceMatrix.Length; i++)
                {
                    for (int j = 0; j <= incidenceMatrix.Length; j++)
                    {
                        Matrix += incidenceMatrix[i][j] + " ";
                    }
                    Matrix += "\r\n";
                }
            }


            File.AppendAllText(filePath, Matrix);
            File.AppendAllText(filePath, "\r\n\r\n");
            string eulerPathString = "";
            File.AppendAllText(filePath, "Euler Path/Cycle " + message + ": \r\n\r\n");
            if (eulerPath.Count >= 1)
            {
                for (int i = 0; i < eulerPath.Count; i++)
                {
                    if (i != eulerPath.Count - 1)
                    {
                        eulerPathString += eulerPath[i] + " => ";
                    }
                    else
                    {
                        eulerPathString += eulerPath[i];
                    }


                }

                File.AppendAllText(filePath, eulerPathString);
                File.AppendAllText(filePath, "\r\n\r\n");
            }
            else
            {
                File.AppendAllText(filePath, "Graph does not have an Euler Path/Cycle");
                File.AppendAllText(filePath, "\r\n\r\n");
            }

            if (firstTime == true)
            {
                tempData = File.ReadAllLines(filePath);
                File.WriteAllText(filePath, "");
            }

        }
    }
}
