using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Genetic_algorithm
    {
        int inputs, outputs, population_size;
        double mutation_rate;

        public Genetic_algorithm (int inputs, int outputs, double mutation_rate, int pop_size)
        {
            this.inputs = inputs;
            this.outputs = outputs;
            this.population_size = pop_size;
            this.mutation_rate = mutation_rate;
        }
    }


    class Node
    {
        int node_id, weight = 0;
        bool input, output;

        public Node(int node_id, bool input, bool output)
        {
            this.node_id = node_id;
            this.input = input;
            this.output = output;
        }
    }


    class Node_Connection
    {
        int start_node, end_node;
        bool disabled;

        public Node_Connection(int start_node, int end_node, bool disabled)
        {
            this.start_node = start_node;
            this.end_node = end_node;
            this.disabled = disabled;
        }
    }
}
