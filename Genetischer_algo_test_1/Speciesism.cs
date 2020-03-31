using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Speciesism
    {
        public double coeffecient_disjoints = 1;
        public double coeffecient_excesses = 1;
        public double coeffecient_weights = 1;
        public int N_switch = 20;

        public List<int[]> Connection_nodes = new List<int[]>(); // the id of the start and end node of the connections - the innovation id is the index of this List

        public List<int> Parent_connection_id_node = new List<int>(); // the connection id of the connection removed by the new created node - the innovation id is the index of this List

        public Speciesism(int node_amount)
        {
            for(int i = 0; i<node_amount; i++)
            {
                Parent_connection_id_node.Add(-1);
            }
        }
        
        public double get_compatability_distance(Neural_Network main_network, Neural_Network network_to_measure_distsance_to)
        {
            int main_network_highest_inno_id = 0;
            int network_to_measure_distsance_to_highest_inno_id = 0;

            List<Connection> connections_resulting = new List<Connection>();
            HashSet<Connection> main_network_common_connections = new HashSet<Connection>();
            HashSet<Connection> network_to_measure_distsance_to_common_connections = new HashSet<Connection>();
            List<Connection> common_connections_final = new List<Connection>();

            int disjoints = 0;
            int excesses = 0;
            double average_weight_difference_common_connections = 0;
            double weight_difference_common_connections = 0;

            // find highest inno id of the nets to define what connections r disjoints and which are excess
            for (int i = 0; i < main_network.nn_connections.Count; i++)
            {
                if (main_network.nn_connections[i].innovation_number > main_network_highest_inno_id)
                {
                    main_network_highest_inno_id = main_network.nn_connections[i].innovation_number;
                }
            }

            for (int i = 0; i < network_to_measure_distsance_to.nn_connections.Count; i++)
            {
                if (network_to_measure_distsance_to.nn_connections[i].innovation_number > network_to_measure_distsance_to_highest_inno_id)
                {
                    network_to_measure_distsance_to_highest_inno_id = network_to_measure_distsance_to.nn_connections[i].innovation_number;
                }
            }
            // check the connections to define the disjoint and excess connections of parent 1
            for (int i = 0; i < main_network.nn_connections.Count; i++)
            {
                int cur_inno_id = main_network.nn_connections[i].innovation_number;

                if (cur_inno_id > network_to_measure_distsance_to_highest_inno_id)
                {
                    excesses += 1;
                }
                else
                {
                    bool found_match = false;
                    for (int k = 0; k < network_to_measure_distsance_to.nn_connections.Count; k++)
                    {
                        if (cur_inno_id == network_to_measure_distsance_to.nn_connections[k].innovation_number)
                        {
                            main_network_common_connections.Add(main_network.nn_connections[i]);
                            found_match = true;
                            break;
                        }
                    }

                    if (!(found_match))
                    {
                        disjoints += 1;
                    }
                }
            }
            // check the connections to define the disjoint and excess connections of parent 2
            for (int i = 0; i < network_to_measure_distsance_to.nn_connections.Count; i++)
            {
                int cur_inno_id = network_to_measure_distsance_to.nn_connections[i].innovation_number;

                if (cur_inno_id > main_network_highest_inno_id)
                {
                    excesses += 1;
                }
                else
                {
                    bool found_match = false;
                    for (int k = 0; k < main_network.nn_connections.Count; k++)
                    {
                        if (cur_inno_id == main_network.nn_connections[k].innovation_number)
                        {
                            network_to_measure_distsance_to_common_connections.Add(network_to_measure_distsance_to.nn_connections[i]);
                            found_match = true;
                            break;
                        }
                    }

                    if (!(found_match))
                    {
                        disjoints += 1;
                    }
                }
            }
            // add the commom connections
            List<Connection> main_network_common_connections_list = main_network_common_connections.ToList();
            List<Connection> network_to_measure_distsance_to_common_connections_list = network_to_measure_distsance_to_common_connections.ToList();

            for(int i= 0; i< network_to_measure_distsance_to_common_connections_list.Count; i++)
            {
                for(int k = 0; k< main_network_common_connections_list.Count; k++)
                {
                    if(network_to_measure_distsance_to_common_connections_list[i].innovation_number == main_network_common_connections_list[k].innovation_number)
                    {
                        if(network_to_measure_distsance_to_common_connections_list[i].weight > main_network_common_connections_list[k].weight)
                        {
                            weight_difference_common_connections += network_to_measure_distsance_to_common_connections_list[i].weight - main_network_common_connections_list[k].weight;
                        }
                        else
                        {
                            weight_difference_common_connections += main_network_common_connections_list[k].weight - network_to_measure_distsance_to_common_connections_list[i].weight;
                        }
                        break;
                    }
                }

            }

            average_weight_difference_common_connections = weight_difference_common_connections / network_to_measure_distsance_to_common_connections_list.Count;

            int highest_genes_count = 0;
            if (main_network.nn_connections.Count > network_to_measure_distsance_to.nn_connections.Count)
            {
                highest_genes_count = main_network.nn_connections.Count;
            }
            else
            {
                highest_genes_count = network_to_measure_distsance_to.nn_connections.Count;
            }

            if(highest_genes_count < N_switch)
            {
                highest_genes_count = 1;
            }

            return (((coeffecient_disjoints * disjoints)/highest_genes_count) + ((coeffecient_excesses * excesses)/highest_genes_count) + coeffecient_weights * average_weight_difference_common_connections);
        }





        // return innovation id of any connection give, if this kind of connection is not present yet it gets inserted and a new innovation id will be added and returned
        public int get_connection_innovation_id(Connection cur_con)
        {
            int innovation_id = -1;

            int start_node_id = cur_con.start_node.innovation_number;
            int end_node_id = cur_con.end_node.innovation_number;
            bool found_connection = false;

            // loop through all existing Connections
            for (int i = 0; i< Connection_nodes.Count; i++)
            {
                // we found a connection that fits the bill
                if (Connection_nodes[i][0] == start_node_id && Connection_nodes[i][1] == end_node_id)
                {
                    innovation_id = i;
                    found_connection = true;
                    break;
                }
            }

            if(!(found_connection))
            {
                innovation_id = Connection_nodes.Count;
                int[] arr = { start_node_id, end_node_id};
                Connection_nodes.Add(arr);
            }
            
            return innovation_id;
        }

        public int get_node_innovation_id(int innovation_id_connection_being_replaced)
        {
            int innovation_id = -1;
            bool node_exists = false;

            for(int i = 0; i < Parent_connection_id_node.Count; i++)
            {
                if(Parent_connection_id_node[i] == innovation_id_connection_being_replaced)
                {
                    innovation_id = i;
                    node_exists = true;
                    break;
                }
            }

            if(!(node_exists))
            {
                innovation_id = Parent_connection_id_node.Count;
                Parent_connection_id_node.Add(innovation_id_connection_being_replaced);
            }

            
            return innovation_id;
        }
    }
}
