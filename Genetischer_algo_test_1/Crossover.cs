using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Crossover
    {
        public NEAT_management management;

        public Crossover(NEAT_management management)
        {
            this.management = management;
        }

        public Neural_Network get_crossover(Neural_Network parent_1, Neural_Network parent_2)
        {
            

            int parent_1_highest_inno_id = 0;
            int parent_2_highest_inno_id = 0;

            List<Connection> connections_resulting = new List<Connection>();
            HashSet<int> common_connections = new HashSet<int>();
            List<int> disjoints_parent_1 = new List<int>();
            List<int> excess_parent_1 = new List<int>();
            List<int> disjoints_parent_2 = new List<int>();
            List<int> excess_parent_2 = new List<int>();
            
            // find highest inno id of the nets to define what connections r disjoints and which are excess
            for (int i = 0; i < parent_1.nn_connections.Count; i++)
            {
                if(parent_1.nn_connections[i].innovation_number > parent_1_highest_inno_id)
                {
                    parent_1_highest_inno_id = parent_1.nn_connections[i].innovation_number;
                }
            }

            for (int i = 0; i < parent_2.nn_connections.Count; i++)
            {
                if (parent_2.nn_connections[i].innovation_number > parent_2_highest_inno_id)
                {
                    parent_2_highest_inno_id = parent_2.nn_connections[i].innovation_number;
                }
            }
            // check the connections to define the disjoint and excess connections of parent 1
            for (int i = 0; i < parent_1.nn_connections.Count; i++)
            {
                int cur_inno_id = parent_1.nn_connections[i].innovation_number;

                if (cur_inno_id > parent_2_highest_inno_id)
                {
                    excess_parent_1.Add(parent_1.nn_connections[i].innovation_number);
                }
                else
                {
                    bool found_match = false;
                    for (int k = 0; k < parent_2.nn_connections.Count; k++)
                    {
                        if (cur_inno_id == parent_2.nn_connections[k].innovation_number)
                        {
                            common_connections.Add(cur_inno_id);
                            found_match = true;
                            break;
                        }
                    }

                    if(!(found_match))
                    {
                        disjoints_parent_1.Add(parent_1.nn_connections[i].innovation_number);
                    }
                }
            }
            // check the connections to define the disjoint and excess connections of parent 2
            for (int i = 0; i < parent_2.nn_connections.Count; i++)
            {
                int cur_inno_id = parent_2.nn_connections[i].innovation_number;

                if (cur_inno_id > parent_1_highest_inno_id)
                {
                    excess_parent_2.Add(parent_2.nn_connections[i].innovation_number);
                }
                else
                {
                    bool found_match = false;
                    for (int k = 0; k < parent_1.nn_connections.Count; k++)
                    {
                        if (cur_inno_id == parent_1.nn_connections[k].innovation_number)
                        {
                            common_connections.Add(cur_inno_id);
                            found_match = true;
                            break;
                        }
                    }

                    if (!(found_match))
                    {
                        disjoints_parent_2.Add(parent_2.nn_connections[i].innovation_number);
                    }
                }
            }
            // add the commom connections
            List<int> common_connections_list = common_connections.ToList();

            for (int i = 0; i < common_connections_list.Count; i++)
            {
                if (management.getRandomNumber_int(100) > 50)
                {
                    for (int k = 0; k < parent_1.nn_connections.Count; k++)
                    {
                        if (parent_1.nn_connections[k].innovation_number == common_connections_list[i])
                        {
                            connections_resulting.Add(parent_1.nn_connections[k]);
                            break;
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < parent_2.nn_connections.Count; k++)
                    {
                        if (parent_2.nn_connections[k].innovation_number == common_connections_list[i])
                        {
                            connections_resulting.Add(parent_2.nn_connections[k]);
                            break;
                        }
                    }
                }
            }

            // add the disjoints to the net
            for(int i = 0; i < disjoints_parent_1.Count; i++)
            {
                for(int k = 0; k< parent_1.nn_connections.Count; k++)
                {
                    if(parent_1.nn_connections[k].innovation_number == disjoints_parent_1[i])
                    {
                        connections_resulting.Add(parent_1.nn_connections[k]);
                        break;
                    }
                }
            }
            for (int i = 0; i < disjoints_parent_2.Count; i++)
            {
                for (int k = 0; k < parent_2.nn_connections.Count; k++)
                {
                    if (parent_2.nn_connections[k].innovation_number == disjoints_parent_2[i])
                    {
                        connections_resulting.Add(parent_2.nn_connections[k]);
                        break;
                    }
                }
            }

            // now handle the excess connections - only keep the excess connections of the NN with more fitness
            if (parent_1.fitness > parent_2.fitness)
            {
                for (int i = 0; i < excess_parent_1.Count; i++)
                {
                    for (int k = 0; k < parent_1.nn_connections.Count; k++)
                    {
                        if (parent_1.nn_connections[k].innovation_number == excess_parent_1[i])
                        {
                            connections_resulting.Add(parent_1.nn_connections[k]);
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < excess_parent_2.Count; i++)
                {
                    for (int k = 0; k < parent_2.nn_connections.Count; k++)
                    {
                        if (parent_2.nn_connections[k].innovation_number == excess_parent_2[i])
                        {
                            connections_resulting.Add(parent_2.nn_connections[k]);
                            break;
                        }
                    }
                }
            }
            

            Neural_Network offspring = new Neural_Network(management.neural_networks.Count, management.inputs, management.outputs, management, management.bias_enabled, true)
            {
                layers = 3
            };

            // create the nodes in the net
            List<int> created_nodes_inno_ids = new List<int>();

            for(int i = 0; i< offspring.nn_nodes.Count; i++)
            {
                created_nodes_inno_ids.Add(offspring.nn_nodes[i].innovation_number);
            }
            // create all mutated nodes, the input and output nodes got created when the net was initialised
            for(int i = 0; i < connections_resulting.Count; i++)
            {
                if(!(created_nodes_inno_ids.Contains(connections_resulting[i].start_node.innovation_number)))
                {
                    Node cur_node = new Node(offspring.id, offspring.nn_nodes.Count, false, false, false, true, 1, connections_resulting[i].start_node.innovation_number);
                    created_nodes_inno_ids.Add(connections_resulting[i].start_node.innovation_number);
                    offspring.nn_nodes.Add(cur_node);

                } else if (!(created_nodes_inno_ids.Contains(connections_resulting[i].end_node.innovation_number)))
                {
                    Node cur_node = new Node(offspring.id, offspring.nn_nodes.Count, false, false, false, true, 1, connections_resulting[i].end_node.innovation_number);
                    created_nodes_inno_ids.Add(connections_resulting[i].end_node.innovation_number);
                    offspring.nn_nodes.Add(cur_node);
                }
            } // all nodes of the offspring net should be created now

            // create connections
            for (int i = 0; i < connections_resulting.Count; i++)
            {
                int start_node_inno_id = connections_resulting[i].start_node.innovation_number;
                int end_node_inno_id = connections_resulting[i].end_node.innovation_number;
                bool connection_exists = false;

                for(int k = 0; k < offspring.nn_connections.Count; k++)
                {
                    if(offspring.nn_connections[k].start_node.innovation_number == start_node_inno_id && offspring.nn_connections[k].end_node.innovation_number == end_node_inno_id)
                    {
                        connection_exists = true;
                        break;
                    }
                }

                if(!(connection_exists))
                {
                    int start_node_index = -1;
                    int end_node_index = -1;

                    for (int h = 0; h < offspring.nn_nodes.Count; h++)
                    {
                        if(offspring.nn_nodes[h].innovation_number == start_node_inno_id)
                        {
                            start_node_index = h;
                        }
                        else if(offspring.nn_nodes[h].innovation_number == end_node_inno_id)
                        {
                            end_node_index = h;
                        }
                    }
                    if (start_node_index >= 0 && end_node_index >= 0)
                    {
                        Connection new_connection = new Connection(offspring.nn_nodes[start_node_index], offspring.nn_nodes[end_node_index], offspring.minimum, offspring.maximum, management);
                        new_connection.weight = connections_resulting[i].weight;
                        new_connection.disabled = connections_resulting[i].disabled;
                        new_connection.innovation_number = connections_resulting[i].innovation_number;

                        offspring.nn_connections.Add(new_connection);
                    }

                }
            }
            offspring.check_net_nature();
            return offspring;
        }
    }
}
