using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class Species
    {
        public int id;
        public Neural_Network representative;
        public Neural_Network best_net;
        public double best_fitness = 0;
        public double average_fitness = 0;
        public Speciesism speciesism;
        public double allowed_pop_size = 1;
        public List<Neural_Network> population = new List<Neural_Network>();

        public Species(Neural_Network representative, Speciesism speciesism, int id)
        {
            this.id = id;
            this.representative = representative;
            this.speciesism = speciesism;
            this.population.Add(representative);
        }

        public void update_average_fitness()
        {
            double average_fitness = 0;

            if(population.Count > 0)
            {
                for(int i = 0; i<population.Count; i++)
                {
                    if (population[i].fitness != 0)
                    {
                        average_fitness += population[i].fitness;
                    }
                }
            }

            if(average_fitness != 0)
            {
                average_fitness = average_fitness / population.Count;
            }

            this.average_fitness = average_fitness;
            
        }
    }
}
