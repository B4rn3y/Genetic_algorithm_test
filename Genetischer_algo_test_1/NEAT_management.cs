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
        public List<Neural_Network> neural_networks = new List<Neural_Network>();

        public NEAT_management(int pop_size)
        {
            this.population_size = pop_size;
        }
    }
}
