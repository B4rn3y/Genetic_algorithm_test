using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Neural_Network
    {
        int id, species_id, inputs, outputs;
        int fitness = 0;
        static double minimum = -1;
        static double maximum = 1;
        public double mutate_weight_prob = 0.8;
        public double mutate_connection_prob = 0.05;
        public double mutate_node_prob = 0.01;
        public double mutate_remove_connection_prob = 0.025;
        public double mutate_remove_node_prob = 0.005;
        public List<Node> nn_nodes = new List<Node>();
        public List<Connection> nn_connections = new List<Connection>();
       

        // initialize the NN with the right amount of inputs and outputs
        public Neural_Network(int id, int inputs, int outputs)
        {
            this.id = id;
            this.inputs = inputs;
            this.outputs = outputs;
        }
            

        // create the nework starting with the minimun Nodes needed for it (Input and Output Nodes + bias)
        void create_network(int inputs, int outputs, int id)
        {
            int counter = 0;
            for (int i = 0; i < inputs; i++)
            {
                Node input_node = new Node(id, counter, true, false, false);
                this.nn_nodes.Add(input_node);
                counter++;
            }

            for (int i = 0; i < outputs; i++)
            {
                Node output_node = new Node(id, counter, false, true, false);
                this.nn_nodes.Add(output_node);
                counter++;
            }

            Node bias_node = new Node(id, counter, false, false, true);
            this.nn_nodes.Add(bias_node);
        }

        // checks if to mutate the neural network
        void check_mutations()
        {
            // add a new connection to the net - cur. 5%
            if (GetRandomNumber(0, 1) <= this.mutate_connection_prob)
            {
                add_connection();
            }
            // mutate weight - cur. 80%
            if (GetRandomNumber(0, 1) <= this.mutate_weight_prob)
            {
                mutate_connection_weight();
            }
            // add new node - cur. 1%
            if (GetRandomNumber(0, 1) <= this.mutate_node_prob)
            {
            }
            // remove a connection - cur. 2.5%
            if (GetRandomNumber(0, 1) <= this.mutate_remove_connection_prob)
            {
            }
            // remove a node - cur. 0.5%
            if (GetRandomNumber(0, 1) <= this.mutate_remove_node_prob)
            {
            }
        }
        // adds a new connection to the neural network when mutated especially
        void add_connection()
        {
            List<Node> start_nodes = new List<Node>();
            List<Node> end_nodes = new List<Node>();
            // find out which Nodes are start notes and which arent
            for (int i = 0; i < nn_nodes.Count; i++)
            {
                if (nn_nodes[i].input || nn_nodes[i].bias)
                {
                    start_nodes.Add(nn_nodes[i]);
                }
                else
                {
                    end_nodes.Add(nn_nodes[i]);
                }
            }
            // choose a random start and end node
            Random random = new Random();
            Node start_node = start_nodes[random.Next(start_nodes.Count)];
            Node end_node = end_nodes[random.Next(end_nodes.Count)];
            // establish new Connection between these Nodes
            Connection new_Con = new Connection(start_node, end_node, minimum, maximum);
            // Add the Connection to the List Var of this Neural Network
            nn_connections.Add(new_Con);
            
        }
        // mutates the weight of a connection in a given range
        void mutate_connection_weight()
        {
            Random random = new Random();
            Connection connection_to_mutate = nn_connections[random.Next(nn_connections.Count)];
            connection_to_mutate.weight = GetRandomNumber(minimum, maximum);
        }

        static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }


    }
}
