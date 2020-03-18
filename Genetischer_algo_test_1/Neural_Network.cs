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
        int layers = 2;
        int nodes_counter = 0;
        static bool bias_enabled = true; // whether the bias node is enabeld or not
        static double minimum = -2;
        static double maximum = 2;
        public double mutate_weight_random_prob = 0.8;
        public double mutate_weight_shift_prob = 0.3;
        public double mutate_connection_prob = 0.05;
        public double mutate_node_prob = 0.01;
        public double mutate_enable_disable_connection = 0.025;
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
        // calculate the output for the net depending on the input - WIP
        public List<double> get_output(List<double> the_inputs)
        {
            List<double> result = new List<double>();
            // loop through every layer and calculate the values of the connections and the status of the nodes
            for (int i = 0; i < layers; i++)
            {
                if (i == 0) // first layer
                {
                    int counter = 0;
                    List<Node> input_nodes = new List<Node>();
                    for (int k = 0; k < nn_nodes.Count; k++)
                    {
                        if (nn_nodes[k].input)
                        {
                            input_nodes.Add(nn_nodes[k]);
                            nn_nodes[k].value = the_inputs[counter];
                            counter++;
                        }
                        else if (nn_nodes[k].bias)
                        {
                            input_nodes.Add(nn_nodes[k]);
                        }
                    }
                    for (int h = 0; h < nn_connections.Count; h++)
                    {
                        if (input_nodes.Contains(nn_connections[h].start_node))
                        {
                            // do not multiply with the bias node! output = sum(weights*inputs)+bias // bias is always 1
                            if(nn_connections[h].start_node.bias)
                            {
                                nn_connections[h].bias = true;
                                nn_connections[h].value = nn_connections[h].start_node.value;
                            } else
                            {
                                nn_connections[h].value = nn_connections[h].start_node.value * nn_connections[h].weight;
                            }
                        }
                    }
                } else if (i+1 == layers) // last layer
                {
                    
                }
                else
                {

                }
            }

            return result;
        }

        public static double Sigmoid(double value)
        {
            return 1.0f / (1.0f + Math.Exp(-value));
        }

        // create the nework starting with the minimun Nodes needed for it (Input and Output Nodes + bias) + the Connections
        void create_network(int inputs, int outputs, int id)
        {
            List<Node> input_nodes = new List<Node>();
            List<Node> output_nodes = new List<Node>();

            for (int i = 0; i < inputs; i++)
            {
                Node input_node = new Node(id, nodes_counter, true, false, false, false, 0);
                this.nn_nodes.Add(input_node);
                input_nodes.Add(input_node);
                nodes_counter++;
            }

            for (int i = 0; i < outputs; i++)
            {
                Node output_node = new Node(id, nodes_counter, false, true, false, false, 1);
                this.nn_nodes.Add(output_node);
                output_nodes.Add(output_node);
                nodes_counter++;
            }

            if (bias_enabled)
            {
                Node bias_node = new Node(id, nodes_counter, false, false, true, false, 0);
                bias_node.value = 1;
                this.nn_nodes.Add(bias_node);
                input_nodes.Add(bias_node);
            }
            else
            {
                nodes_counter--;
            }
            // create connections from every input node to every output node
            for (int i = 0; i < input_nodes.Count; i++)
            {
                for(int k = 0; k < output_nodes.Count; k++)
                {
                    nn_connections.Add(new Connection(input_nodes[i],output_nodes[k],minimum,maximum));
                }
            }
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
            if (GetRandomNumber(0, 1) <= this.mutate_weight_random_prob && nn_connections.Count > 0)
            {
                mutate_connection_weight_random();
            }
            // add new node - cur. 1%
            if (GetRandomNumber(0, 1) <= this.mutate_node_prob)
            {
                add_node();
            }
            // enable_disable_connection - cur. 2.5%
            if (GetRandomNumber(0, 1) <= this.mutate_enable_disable_connection && nn_connections.Count > 0)
            {
                enable_disable_connection();
            }
            // mutate the weight of a connection by dividing it - cur. 30%
            if (GetRandomNumber(0, 1) <= this.mutate_weight_shift_prob)
            {
                mutate_weight_shift();
            }
        }
        // checks whether some nodes that are connected sit on the same layer(what they shouldnt); if it finds something like this it takes action by incrementing the layer of the end node. This does it as long as its needed for the net to be in order again
        void check_net_nature()
        {
            while (true)
            {
                bool fixed_a_node = false;
                for (int c = 0; c < nn_connections.Count; c++)
                {
                    Connection cur_connection = nn_connections[c];
                    if (cur_connection.start_node.layer == cur_connection.end_node.layer)
                    {
                        fixed_a_node = true;
                        Node node_to_increment_layer_of = cur_connection.end_node;
                        int cur_layer = node_to_increment_layer_of.layer;
                        if (cur_layer + 1 >= layers - 1) // theres no other layer we could increment to => create a new layer
                        {
                            layers++;
                            for (int i = 0; i < nn_nodes.Count; i++)
                            {
                                if (nn_nodes[i].layer > cur_layer || nn_nodes[i] == node_to_increment_layer_of)
                                {
                                    nn_nodes[i].layer += 1;
                                }
                            }
                        }
                        else // theres a layer we can simply add the node to :p
                        {
                            node_to_increment_layer_of.layer += 1;
                        }

                    }
                }
                if(!(fixed_a_node))
                {
                    break;
                }
            }
        }

        void add_node()
        {
            Random random = new Random();
            Connection connection_node_add = nn_connections[random.Next(nn_connections.Count)];
            Node connection_start_node = connection_node_add.start_node;
            Node connection_end_node = connection_node_add.end_node;
            double connection_weight = connection_node_add.weight;
            bool connection_disabled = connection_node_add.disabled;
            nn_connections.Remove(connection_node_add);
            /*
            int start_layer = connection_start_node.layer;
            int end_layer = connection_end_node.layer;
            int new_node_layer;
            // The nodes are on the same layer
            if ((end_layer - start_layer) == 0)
            {
                // 2 layers for the nodes, +1 for the output layer and +1 because the nodes start counting with 0 and the var layers doesnt :X
                if (start_layer + 4 > layers)
                {
                    layers += (start_layer + 4 - layers);
                    for (int n = 0; n < nn_nodes.Count; n++)
                    {

                        Node cur_node = nn_nodes[n];
                        if (cur_node.layer > start_layer)
                        {
                            cur_node.layer++;
                        }
                        else if (cur_node == connection_end_node)
                        {
                            cur_node.layer += 2;
                        }
                    }
                }
                new_node_layer = start_layer + 1;



            }
            else if ((end_layer - start_layer) == 1) // the nodes are one layer apart
            {
                // increment the layer var of the network
                layers++;
                // increment the layer var of all Nodes sitting in an equal or higher layer than the end Node of the deleted Connection
                for (int n = 0; n < nn_nodes.Count; n++)
                {
                    Node cur_node = nn_nodes[n];
                    if (cur_node.layer >= end_layer)
                    {
                        cur_node.layer++;
                    }
                }
                new_node_layer = end_layer;
            }
            else // The nodes are more than 1 layer apart
            {
                new_node_layer = start_layer + 1;
            }
            */
            // create the new Node and the 2 new connections of it
            Node new_node = new Node(id, nn_nodes.Count - 1, false, false, false, true, connection_start_node.layer);
            nn_nodes.Add(new_node);
            Connection to_connection = new Connection(connection_start_node, new_node, minimum, maximum);
            to_connection.weight = 1;
            nn_connections.Add(to_connection);
            Connection from_connection = new Connection(new_node, connection_end_node, minimum, maximum);
            from_connection.disabled = connection_disabled;
            from_connection.weight = connection_weight;
            nn_connections.Add(from_connection);
            check_net_nature();

        }

        // adds a new connection to the neural network when mutated especially
        void add_connection()
        {
            // Make a list to store all possible Connections
            List<Connection> possible_connections = new List<Connection>();
            // Loop through every Node
            for (int i = 0; i < nn_nodes.Count; i++)
            {
                Node cur_start_node = nn_nodes[i];
                // Only consider it a start node when its a mutated/input/bias node
                if(!(cur_start_node.output))
                {
                    // Loop through every Node with every Node to find every possible Connection
                    for (int k = 0; k < nn_nodes.Count; k++)
                    {
                        Node cur_end_node = nn_nodes[k];
                        // Connection only possible when: The Nodes arent the same node; the layer of the end Node is higher or equal to the layer of the start node; the end node cant be a input or bias node
                        if (!(cur_start_node == cur_end_node) && cur_end_node.layer >= cur_start_node.layer && !(cur_end_node.input || cur_end_node.bias))
                        {
                            // check every established connection if this connection already exists
                            bool new_connection = true;
                            for (int h = 0; h < nn_nodes.Count; h++)
                            {
                                Connection connection_to_check = nn_connections[h];
                                if (connection_to_check.start_node == cur_end_node && connection_to_check.end_node == cur_end_node)
                                {
                                    new_connection = false;
                                    break;
                                }
                            }
                            // if it doesnt exist add the Connection to the possible connections list
                            if (new_connection)
                            {
                                possible_connections.Add(new Connection(cur_start_node, cur_end_node, minimum, maximum));
                            }
                        }
                    }
                }
            }

            // select a random Connection from the possible Connection list - only when list is full of course XD
            if (possible_connections.Count > 0)
            {
                Random random = new Random();
                nn_connections.Add(possible_connections[random.Next(possible_connections.Count)]);
                check_net_nature();
            }


            /*
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
            */
            

        }
        // enables a disabled connection or disables a enabled connection if random chosen
        void enable_disable_connection()
        {
            if (nn_connections.Count > 0)
            {
                Random random = new Random();
                Connection con_to_mutate = nn_connections[random.Next(nn_connections.Count)];
                con_to_mutate.disabled = !(con_to_mutate.disabled);
            }

        }
        // mutates the weight of a connection in a given range
        void mutate_connection_weight_random()
        {
            if(nn_connections.Count > 0)
            { 
                Random random = new Random();
                Connection connection_to_mutate = nn_connections[random.Next(nn_connections.Count)];
                connection_to_mutate.weight = GetRandomNumber(minimum, maximum);
            }
        }

        // shift the weight of a connection by dividing it by 2
        void mutate_weight_shift()
        {
            if (nn_connections.Count > 0)
            {
                Random random = new Random();
                Connection connection_to_mutate = nn_connections[random.Next(nn_connections.Count)];
                if (connection_to_mutate.weight != 0)
                {
                    connection_to_mutate.weight = connection_to_mutate.weight / 2;
                }
            }
        }

        static double GetRandomNumber(double minimum, double maximum)
        {
            Random random = new Random();
            return random.NextDouble() * (maximum - minimum) + minimum;
        }


    }
}
