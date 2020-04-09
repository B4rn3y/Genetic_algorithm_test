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
        public double coeffecient_weights = 0.4;
        public int N_switch = 20; // the amount of genes needed in a net in order to take effect in the compatability distance function

        public double species_threshold = 1; // the allowed compatability distance of two nets to be determined as the same species

        public double average_species_fitness = 0;
        public double average_fitness_sum = 0;
        public int species_counter = 0;

        public NEAT_management management;

        public List<Species> species = new List<Species>();

        public List<int[]> Connection_nodes = new List<int[]>(); // the id of the start and end node of the connections - the innovation id is the index of this List

        public List<int> Parent_connection_id_node = new List<int>(); // the connection id of the connection removed by the new created node - the innovation id is the index of this List

        public Speciesism(int node_amount, NEAT_management management)
        {
            this.management = management;
            for(int i = 0; i<node_amount; i++)
            {
                Parent_connection_id_node.Add(-1);
            }
        }

        public void update_average_species_fitness()
        {
            double average_fitness = 0;
            // update the average fitness of every species
            Console.WriteLine("species count = {0}", species.Count);
            for(int i = 0; i < species.Count; i++)
            {
                species[i].update_average_fitness();
                average_fitness += species[i].average_fitness;
            }

            if(species.Count > 0 && average_fitness != 0)
            {
                average_species_fitness = average_fitness / species.Count;
            }
            else
            {
                average_species_fitness = 0;
            }
            average_fitness_sum = average_fitness;

            Console.WriteLine("average_species_fitness = {0} - average_fitness_sum = {1}", average_species_fitness, average_fitness_sum);
        }

        // calculates the allowed pop size of each species depending on their achieved fitness
        public void calculate_allowed_pop_size_of_species()
        {
            update_average_species_fitness(); // first update the fitness of all species
            // loop through every species
            for (int i= 0; i<species.Count; i++)
            {
                double pop_size = management.population_size * (species[i].average_fitness / average_fitness_sum);
                species[i].allowed_pop_size = Math.Round(pop_size);
                Console.WriteLine(String.Format("Species: {0} - allowed pop size: {1}", species[i].id, species[i].allowed_pop_size));
            }
        }


        // loops through every network to calculate which species it belongs to - if no matching species is found, it creates a new one
        public void group_networks_into_species()
        {
            // loop through every neural net
            for(int i = 0; i < management.neural_networks.Count; i++)
            {
                if(species.Count == 0)
                {
                    Species cur_spec = new Species(management.neural_networks[i], this, species_counter);
                    management.neural_networks[i].species = cur_spec;
                    species.Add(cur_spec);
                    species_counter++;
                }
                else
                {
                    bool found_match = false;
                    // loop through every existing species to find a matching one
                    for(int h = 0; h < species.Count; h++)
                    {
                        // this net is similar to this species, so add it to it
                        if(get_compatability_distance(management.neural_networks[i], species[h].representative) <= species_threshold)
                        {
                            species[h].population.Add(management.neural_networks[i]);
                            management.neural_networks[i].species = species[h];
                            found_match = true;
                            break;
                        }
                    }
                    // there was no species similar to this one => create a new one!
                    if(!(found_match))
                    {
                        Species cur_species = new Species(management.neural_networks[i], this, species_counter);
                        management.neural_networks[i].species = cur_species;
                        species.Add(cur_species);
                        species_counter++;
                    }
                }
            }
            Console.WriteLine("We have {0} species before deletion", species.Count);
            delete_empty_species();
            Console.WriteLine("We have {0} species after deletion", species.Count); 
        }

        public void delete_empty_species()
        {
            List<Species> species_to_remove = new List<Species>(); 

            for(int i= 0; i < species.Count; i++)
            {
                if (species[i].population.Count == 0)
                {
                    species_to_remove.Add(species[i]);
                }
                else
                {
                    Console.WriteLine("Species: {0} - Pop size: {1}",species[i].id, species[i].population.Count);
                }
            }

            for(int i = 0; i < species_to_remove.Count; i++)
            {
                species.Remove(species_to_remove[i]);
            }
        }

        public void clear_species() // this functions deletes every species besides the 2 best performing species
        {
            update_average_species_fitness();
            List<Species> species_copy = species;
            List<Species> best_species = new List<Species>();

            for(int i = 0; i < 2; i++)
            {
                double highest_fittness = double.MinValue;
                int best_species_index = 0;

                for(int h = 0; h < species_copy.Count; h++)
                {
                    if(highest_fittness < species_copy[h].average_fitness)
                    {
                        best_species_index = h;
                    }
                }

                best_species.Add(species_copy[best_species_index]);

                species_copy.RemoveAt(best_species_index);
            }

            species = best_species;
        }

        // this method kills the worst half of all species and replaces the entire population then with the offspring of the remaining best half of the species
        public void create_offspring()
        {
            calculate_allowed_pop_size_of_species();

            management.neural_networks = new List<Neural_Network>();

            for(int i = 0; i < species.Count; i++) // loop through every species
            {
                List<Neural_Network> species_pop_copy = species[i].population;
                int worst_net_index = 0;
                double lowest_fitness = double.MinValue;
                double pop_half = species[i].population.Count / 2;


                for (int p = 0; p < Math.Round(pop_half); p++) // delete the worst half of this species
                {
                    for(int h = 0; h < species_pop_copy.Count; h++) // loop through every remaining net in this list to get the worst every time
                    {
                        if(species_pop_copy[h].fitness <= lowest_fitness)
                        {
                            worst_net_index = h;
                            lowest_fitness = species_pop_copy[h].fitness;
                        }
                    }

                    species_pop_copy.RemoveAt(worst_net_index); // delete the worst net
                }

                int best_net_index = 0;
                double highest_fittnes = double.MinValue;

                for(int u = 0; u < species_pop_copy.Count; u++)
                {
                    if(species_pop_copy[u].fitness > highest_fittnes)
                    {
                        best_net_index = u;
                    }
                }

                Neural_Network best_net_species = species_pop_copy[best_net_index];


                species[i].population = new List<Neural_Network>(); // delete the population of the species

                species[i].population.Add(best_net_species);

                double amount_of_nets_to_mutate = Math.Round(species[i].allowed_pop_size / 4) - 1; // 25% of offsrping results from mutation without crossover

                for(int k = 0; k < amount_of_nets_to_mutate; k++)
                {
                    Neural_Network net_to_muatate = species_pop_copy[management.getRandomNumber_int(species_pop_copy.Count)];
                    net_to_muatate.check_mutations();
                    //species[i].population.Add(net_to_muatate);
                    management.neural_networks.Add(net_to_muatate);
                }

                for(int h = 0; h < (species[i].allowed_pop_size - amount_of_nets_to_mutate) - 1; h++) //create the offspring of the remaining not eleminated nets and add it to the population of the species
                {
                    Neural_Network offspring = management.crossover_nets.get_crossover(species_pop_copy[management.getRandomNumber_int(species_pop_copy.Count)], species_pop_copy[management.getRandomNumber_int(species_pop_copy.Count)]);

                    if(management.getRandomNumber_int(100) <= management.mutation_rate) // with a 5% chance(currently) the net could mutate
                    {
                        offspring.check_mutations();
                    }

                    //species[i].population.Add(offspring);
                    management.neural_networks.Add(offspring);
                }

                //species[i].representative = species_pop_copy[management.getRandomNumber_int(species_pop_copy.Count)]; // select a random representative for this species from the previous generation
                species[i].representative = best_net_species; // select the best net as representative
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
