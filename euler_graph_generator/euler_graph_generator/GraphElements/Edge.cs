using QuickGraph;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace euler_graph_generator.GraphElements
{
    public class Edge : Edge<Vertex>, INotifyPropertyChanged
    {
        private string id;

        public string ID
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged("ID");
            }
        }

        public Color EdgeColor { get; set; }
        public Visibility EdgeVisibility { get; set; }

        public Edge(Visibility edgeVisibility, string id, Vertex source, Vertex target)
            : base(source, target)
        {
            EdgeVisibility = edgeVisibility;
            ID = id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }


    }
}
