using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Neural_Network
    {
        public int id, species_id, inputs, outputs;
        public int fitness = 0;
        public int layers = 2;
        public int nodes_counter = 0;
        public bool bias_enabled; // whether the bias node is enabeld or not
        public double minimum = -2;
        public double maximum = 2;
        public int mutate_weight_random_prob = 80;
        public int mutate_weight_shift_prob = 30;
        public int mutate_connection_prob = 5;
        public int mutate_node_prob = 1;
        public int mutate_enable_disable_connection = 3;
        public List<Node> nn_nodes = new List<Node>();
        public List<Connection> nn_connections = new List<Connection>();
        public NEAT_management management;
       

        // initialize the NN with the right amount of inputs and outputs
        public Neural_Network(int id, int inputs, int outputs, NEAT_management management, bool bias_enabled, bool crossover = false)
        {
            this.id = id;
            this.inputs = inputs;
            this.outputs = outputs;
            this.management = management;
            this.bias_enabled = bias_enabled;
            create_network(inputs, outputs, id, crossover);
        }
        // calculate the output for the net depending on the input - WIP
        public List<double> get_output(List<double> the_inputs)
        {
            List<double> result = new List<double>();
            List<Node> output_nodes = new List<Node>();
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
                    for(int j = 0;j<nn_nodes.Count;j++)
                    {
                        if(nn_nodes[j].output)
                        {
                            output_nodes.Add(nn_nodes[j]);
                        }
                    }

                    for(int u = 0; u<output_nodes.Count;u++)
                    {
                        Node cur_node = output_nodes[u];
                        for(int o = 0; o<nn_connections.Count; o++)
                        {
                            Connection cur_connection = nn_connections[o];
                            if(cur_connection.end_node == cur_node)
                            {
                                cur_node.value += cur_connection.value;
                            }
                        }
                    }
                }
                else // hidden layer/s
                {
                    List<Node> nodes_current_layer = new List<Node>();
                    List<Connection> leading_connection = new List<Connection>();
                    // find nodes of the current layer
                    for (int k = 0; k < nn_nodes.Count; k++)
                    {
                        if (nn_nodes[k].layer == i)
                        {
                            nodes_current_layer.Add(nn_nodes[k]);
                        }
                    }
                    for (int a = 0; a < nodes_current_layer.Count; a++)
                    {
                        Node cur_node = nodes_current_layer[a];
                        // find connections of this Node
                        for (int h = 0; h < nn_connections.Count; h++)
                        {
                            Connection cur_connection = nn_connections[h];
                            if(cur_connection.start_node == cur_node)
                            {
                                leading_connection.Add(cur_connection);
                            }
                            else if(cur_connection.end_node == cur_node)
                            {
                                cur_node.value += cur_connection.value;
                            }
                        }
                    }
                    // Calculate the values of all Connections going to upper layers from this layer out
                    for(int g = 0; g<leading_connection.Count; g++)
                    {
                        leading_connection[g].value = leading_connection[g].start_node.value * leading_connection[g].weight;
                    }
                }
            }
            // calculate the output by sumbitting every value of every output node to the sigmoid delivery function and return it as a list of doubles to be used in the fittness function
            for(int i = 0; i<output_nodes.Count; i++)
            {
                result.Add(Sigmoid(output_nodes[i].value));
            }
            reset_net_values();
            return result;
        }
        // reset the values of all nodes and connections
        public void reset_net_values()
        {
            for(int i = 0; i<nn_connections.Count;i++)
            {
                nn_connections[i].value = 0;
            }
            for (int i = 0; i < nn_nodes.Count; i++)
            {
                nn_nodes[i].value = 0;
            }
        }
        // sigmoid function - used as a delivery function for the outputs of the neural network
        public static double Sigmoid(double value)
        {
            return 1.0f / (1.0f + Math.Exp(-value));
        }

        // create the nework starting with the minimun Nodes needed for it (Input and Output Nodes + bias) + the Connections
        void create_network(int inputs, int outputs, int id, bool crossover)
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
                if(crossover)
                {
                    Node output_node = new Node(id, nodes_counter, false, true, false, false, 2);
                    this.nn_nodes.Add(output_node);
                    output_nodes.Add(output_node);
                    nodes_counter++;
                }
                else
                {
                    Node output_node = new Node(id, nodes_counter, false, true, false, false, 1);
                    this.nn_nodes.Add(output_node);
                    output_nodes.Add(output_node);
                    nodes_counter++;
                }
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
            if(!(crossover))
            {
                for (int i = 0; i < input_nodes.Count; i++)
                {
                    for(int k = 0; k < output_nodes.Count; k++)
                    {
                        nn_connections.Add(new Connection(input_nodes[i],output_nodes[k],minimum,maximum, management));
                    }
                }
            }
        }

        // checks if to mutate the neural network
        void check_mutations()
        {
            // add a new connection to the net - cur. 5%
            if (management.getRandomNumber_int(100) <= this.mutate_connection_prob)
            {
                add_connection();
            }
            // mutate weight - cur. 80%
            if (management.getRandomNumber_int(100) <= this.mutate_weight_random_prob)
            {
                mutate_connection_weight_random();
            }
            // add new node - cur. 1%
            if (management.getRandomNumber_int(100) <= this.mutate_node_prob)
            {
                add_node();
            }
            // enable_disable_connection - cur. 2.5%
            if (management.getRandomNumber_int(100) <= this.mutate_enable_disable_connection)
            {
                enable_disable_connection();
            }
            // mutate the weight of a connection by dividing it - cur. 30%
            if (management.getRandomNumber_int(100) <= this.mutate_weight_shift_prob)
            {
                mutate_weight_shift();
            }
        }
        // checks whether some nodes that are connected sit on the same layer(what they shouldnt); if it finds something like this it takes action by incrementing the layer of the end node. This does it as long as its needed for the net to be in order again
        public void check_net_nature()
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

        public void add_node()
        {
            Connection connection_node_add = nn_connections[management.getRandomNumber_int(nn_connections.Count)];
            Node connection_start_node = connection_node_add.start_node;
            Node connection_end_node = connection_node_add.end_node;
            double connection_weight = connection_node_add.weight;
            bool connection_disabled = connection_node_add.disabled;
            int connection_inno_id = connection_node_add.innovation_number;
            nn_connections.Remove(connection_node_add);

            int Node_inno_id = management.species_manager.get_node_innovation_id(connection_inno_id);

            // create the new Node and the 2 new connections of it
            Node new_node = new Node(id, nn_nodes.Count, false, false, false, true, connection_start_node.layer, Node_inno_id);
            nn_nodes.Add(new_node);
            Connection to_connection = new Connection(connection_start_node, new_node, minimum, maximum, management);
            to_connection.weight = 1;
            nn_connections.Add(to_connection);
            Connection from_connection = new Connection(new_node, connection_end_node, minimum, maximum, management);
            from_connection.disabled = connection_disabled;
            from_connection.weight = connection_weight;
            nn_connections.Add(from_connection);
            check_net_nature();

        }

        // adds a new connection to the neural network when mutated especially
        public void add_connection()
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
                            for (int h = 0; h < nn_connections.Count; h++)
                            {
                                Connection connection_to_check = nn_connections[h];
                                if (connection_to_check.start_node == cur_start_node && connection_to_check.end_node == cur_end_node)
                                {
                                    new_connection = false;
                                    break;
                                }
                            }
                            // if it doesnt exist add the Connection to the possible connections list
                            if (new_connection)
                            {
                                possible_connections.Add(new Connection(cur_start_node, cur_end_node, minimum, maximum, management));
                            }
                        }
                    }
                }
            }

            // select a random Connection from the possible Connection list - only when list is full of course XD
            Console.WriteLine(String.Format("{0}",possible_connections.Count));
            if (possible_connections.Count > 0)
            {
                nn_connections.Add(possible_connections[management.getRandomNumber_int(possible_connections.Count)]);
                check_net_nature();
            }


            
            

        }
        // enables a disabled connection or disables a enabled connection if random chosen
        public void enable_disable_connection()
        {
            if (nn_connections.Count > 0)
            {
                Connection con_to_mutate = nn_connections[management.getRandomNumber_int(nn_connections.Count)];
                con_to_mutate.disabled = !(con_to_mutate.disabled);
            }

        }
        // mutates the weight of a connection in a given range
        public void mutate_connection_weight_random()
        {
            if(nn_connections.Count > 0)
            { 
                Connection connection_to_mutate = nn_connections[management.getRandomNumber_int(nn_connections.Count)];
                connection_to_mutate.weight = GetRandomNumber(minimum, maximum);
            }
        }

        // shift the weight of a connection by dividing it by 2
        public void mutate_weight_shift()
        {
            if (nn_connections.Count > 0)
            {
                Connection connection_to_mutate = nn_connections[management.getRandomNumber_int(nn_connections.Count)];
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
