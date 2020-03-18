using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Node
    {
        int nn_id, id;
        public bool input;
        public bool output;
        public bool bias;
        public bool mutated;
        public int bias_weight = 0;
        public double value = 0;
        public int layer;
        public int innovation_number;
        public double calculated_output;

        public Node(int nn_id, int id, bool input, bool output, bool bias, bool mutated, int layer)
        {
            this.nn_id = nn_id;
            this.id = id;
            this.input = input;
            this.output = output;
            this.bias = bias;
            this.mutated = mutated;
            this.layer = layer;
        }
    }
}
