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
        public int innovation_number;

        public Node(int nn_id, int id, bool input, bool output, bool bias, bool mutated)
        {
            this.nn_id = nn_id;
            this.id = id;
            this.input = input;
            this.output = output;
            this.bias = bias;
            this.mutated = mutated;
        }
    }
}
