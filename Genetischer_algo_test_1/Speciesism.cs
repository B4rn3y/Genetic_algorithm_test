using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Speciesism
    {
        public List<int[]> Connection_nodes = new List<int[]>(); // the id of the start and end node of the connections - the innovation id is the index of this List

        public List<int> Parent_connection_id_node = new List<int>(); // the connection id of the connection removed by the new created node - the innovation id is the index of this List

        public Speciesism(int node_amount)
        {
            for(int i = 0; i<node_amount; i++)
            {
                Parent_connection_id_node.Add(-1);
            }
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
