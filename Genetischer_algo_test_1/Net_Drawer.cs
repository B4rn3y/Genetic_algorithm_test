using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Net_Drawer
    {
        public double stackpanel_width = 0;
        public double width = 0;
        public double height = 0;
        public int border_margin = 20;

        public void draw_net(Neural_Network net)
        {
            double max_x = width - border_margin;
            double min_x = stackpanel_width + border_margin;
            double max_y = height - border_margin;
            double min_y = 0 + border_margin;

            for(int i = 0; i<net.layers; i++)
            {
                List<Node> nodes_to_draw = new List<Node>();
                for(int k = 0; k<net.nn_nodes.Count; k++)
                {
                    if(net.nn_nodes[k].layer == i)
                    {
                        nodes_to_draw.Add(net.nn_nodes[k]);
                    }
                }
            }
        }
    }
}
