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
        public int layer;

        public Node(int nn_id, int id, bool input, bool output)
        {
            this.nn_id = nn_id;
            this.id = id;
            if (input)
            {
                this.layer = 0;
            }
            else if (output) {
                this.layer = 1;
            }
            
        }
    }
}
