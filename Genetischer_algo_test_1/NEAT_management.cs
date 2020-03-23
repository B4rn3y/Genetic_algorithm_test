using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class NEAT_management
    {
        public int population_size;
        public int best_fittness = 0;
        public bool running = false;
        public int inputs = 4, outputs = 1;
        public Net_Drawer drawer;
        public Log_Updater updater;
        public Neural_Network best_net;
        public List<Neural_Network> neural_networks = new List<Neural_Network>();
        public static Random random = new Random();
        

        public NEAT_management(int pop_size)
        {
            this.population_size = pop_size;
            if(pop_size > 0)
            {
                start_nets(pop_size);
            }
        }

        public double getRandomNumber_double(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        public int getRandomNumber_int(int max)
        {
            return random.Next(max);
        }

        public void start_nets(int pop_size)
        {
            neural_networks = new List<Neural_Network>();
            this.population_size = pop_size;
            for (int i = 0; i < pop_size; i++)
            {
                Console.WriteLine(String.Format("Creating net: {0}", i));
                updater.update_log(String.Format("Creating net: {0}", i));
                neural_networks.Add(new Neural_Network(i, inputs, outputs, this));
            }

            if(pop_size == 1)
            {
                drawer.draw_net(neural_networks[0]);
                best_net = neural_networks[0];
            }
        }

        public void remove_display()
        {
            drawer.remove_all_net_elements();
        }

        public void redraw_net()
        {
            drawer.remove_all_net_elements();
            drawer.draw_net(best_net);
        }

        public void fittness_function()
        {
            for(int i = 0; i<neural_networks.Count; i++)
            {

            }
        }

    }
}
