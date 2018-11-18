using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace euler_graph_generator.GraphElements
{
    public class Vertex
    {
        public List<Vertex> Neighbors;

        public string VertexValue { get; set; }
        public int VertexDegree { get; set; }
        public int Index { get; set; }
        public Vertex(string value, int index)
        {
            Neighbors = new List<Vertex>();
            VertexValue = value;
            Index = index;
        }
       
    }
}
