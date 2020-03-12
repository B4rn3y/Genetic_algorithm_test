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
        static int max_nodes_connection_add_switch = 50; // This is the amount of inputs needed for the add_connection method to switch used methods
        static double minimum = -2;
        static double maximum = 2;
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
                Node input_node = new Node(id, counter, true, false, false, false);
                this.nn_nodes.Add(input_node);
                counter++;
            }

            for (int i = 0; i < outputs; i++)
            {
                Node output_node = new Node(id, counter, false, true, false, false);
                this.nn_nodes.Add(output_node);
                counter++;
            }

            Node bias_node = new Node(id, counter, false, false, true, false);
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
            if (GetRandomNumber(0, 1) <= this.mutate_weight_prob && nn_connections.Count > 0)
            {
                mutate_connection_weight();
            }
            // add new node - cur. 1%
            if (GetRandomNumber(0, 1) <= this.mutate_node_prob)
            {
            }
            // remove a connection - cur. 2.5%
            if (GetRandomNumber(0, 1) <= this.mutate_remove_connection_prob && nn_connections.Count > 0)
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
            
            List<Node> input_nodes = new List<Node>();
            List<Node> output_nodes = new List<Node>();
            List<Node> start_nodes = new List<Node>();
            List<Node> mutated_nodes = new List<Node>();
            Node start_node, end_node;

            // find out which Nodes are start notes and which arent
            for (int i = 0; i < nn_nodes.Count; i++)
            {
                if (nn_nodes[i].input || nn_nodes[i].bias)
                {
                    input_nodes.Add(nn_nodes[i]);
                }
                else if (nn_nodes[i].mutated)
                {
                    mutated_nodes.Add(nn_nodes[i]);
                }
                else 
                {
                    output_nodes.Add(nn_nodes[i]);
                }
            }
            // Join the input and mutated Notes list into the start nodes list
            start_nodes.AddRange(input_nodes);
            start_nodes.AddRange(mutated_nodes);
            // -----------------------------------------NEW SHIT BEGINS











            //// --------------------------------------NEW SHIT END
            bool already_connected = false;
            int counter = 0;
            while (true)
            {
                counter++;
                // choose a random start node
                Random random = new Random();
                start_node = start_nodes[random.Next(start_nodes.Count)];

                // Join the mutated and output Nodes list together to find out to which Node the connection should go to
                List<Node> end_nodes = new List<Node>();
                end_nodes.AddRange(output_nodes);
                // Check if which mutated Nodes can be added, one could have been selected as the start node so we have to check
                for (int i = 0; i < mutated_nodes.Count; i++)
                {
                    if (!(mutated_nodes[i] == start_node))
                    {
                        end_nodes.Add(mutated_nodes[i]);
                    }
                }
                // Select end node
                end_node = end_nodes[random.Next(end_nodes.Count)];

                // Check if the Node already has a connection to this Node
                already_connected = false;
                for (int i = 0; i < nn_connections.Count; i++)
                {
                    // if the Nodes are connected quit the loop, no need to further check other Connection sources and targets
                    if (nn_connections[i].start_node == start_node && nn_connections[i].end_node == end_node)
                    {
                        already_connected = true;
                        break;
                    }
                }

                if (already_connected)
                {
                    if (counter >= 10000)
                    {
                        break;
                    } else
                    {
                        continue;
                    }
                   
                }
                else
                {
                    break;
                }
            }

            // Check if the Connection should be added or no suitable Connection could be found
            if (!(already_connected))
            {
                // create the new Connection
                Connection new_Con = new Connection(start_node, end_node, minimum, maximum);
                // Add the Connection to the List Var of this Neural Network
                nn_connections.Add(new_Con);
            }
            

        }
        // removes a connection random from the net !!!Watch Out!!! Connections to mutated Nodes should not be deleted!! ---- HIER MUSS ICH NOCHMAL RAN
        void remove_connection()
        {
            List<Node> mutated_nodes = new List<Node>();
            List<Connection> removable_cons = new List<Connection>();

            for (int i = 0; i < nn_nodes.Count; i++)
            {
                if (!(nn_nodes[i].bias || nn_nodes[i].input || nn_nodes[i].output))
                {
                    mutated_nodes.Add(nn_nodes[i]);
                }
            }

            if (mutated_nodes.Count > 0)
            {
                for (int i = 0; i < nn_connections.Count; i++)
                {

                }
            }

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
