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
        public int inputs = 2, outputs = 1;
        public Log_Updater updater;
        public List<Neural_Network> neural_networks = new List<Neural_Network>();
        

        public NEAT_management(int pop_size)
        {
            this.population_size = pop_size;
            if(pop_size > 0)
            {
                start_nets(pop_size);
            }
        }

        public void start_nets(int pop_size)
        {
            neural_networks = new List<Neural_Network>();
            for (int i = 0; i < pop_size; i++)
            {
                Console.WriteLine(String.Format("Creating net: {0}", i));
                updater.update_log(String.Format("Creating net: {0}", i));
                neural_networks.Add(new Neural_Network(i, inputs, outputs));
            }
        }

    }
}
