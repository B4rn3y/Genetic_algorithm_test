using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Connection
    {
        public double minimum;
        public double maximum;
        public double weight;
        public double value = 0;
        public bool disabled = false;
        public Node start_node;
        public Node end_node;
        public int innovation_number;
        public bool bias = false;

        public Connection(Node start_node, Node end_node, double minimum, double maximum, NEAT_management management)
        {
            this.start_node = start_node;
            this.end_node = end_node;
            this.minimum = minimum;
            this.maximum = maximum;
            this.weight = management.getRandomNumber_double(minimum,maximum);
        }

    }
}
