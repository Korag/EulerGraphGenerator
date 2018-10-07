using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace euler_graph_generator.GraphElements
{
    public class Vertex
    {
        public string VertexValue { get; set; }

        public Vertex(string value)
        {
            VertexValue = value;
        }
    }
}
