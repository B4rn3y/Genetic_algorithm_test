using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetischer_algo_test_1
{
    class NEAT_management
    {
        public int population_size;
        public double node_activation_threshold = 0.5;
        public double best_fittness = double.MinValue;
        public int last_fittness_increase_generation_id = 0;
        public double max_fittness = 2000;
        public bool running = false;
        public bool paused = false;
        public bool stopped = false;
        public int inputs = 2, outputs = 1;
        public int generation_counter = 1;
        public int mutation_rate = 5;
        public int no_progress_deletion_threshold = 20;
        public Net_Drawer drawer;
        public Log_Updater updater;
        public Neural_Network best_net;
        public Neural_Network cur_net;
        public bool bias_enabled = false;
        public List<Neural_Network> neural_networks = new List<Neural_Network>();
        public static Random random = new Random(8);
        public Speciesism species_manager;
        public Crossover crossover_nets;


        public NEAT_management()
        {
            int node_counter = inputs + outputs;
            if(bias_enabled)
            {
                node_counter += 1;
            }

            species_manager = new Speciesism(node_counter, this);
            crossover_nets = new Crossover(this);
        }

        public void get_best_net()
        {
            for(int i = 0; i < neural_networks.Count; i++)
            {
                if(neural_networks[i].fitness > best_net.fitness)
                {
                    best_net = neural_networks[i];

                    if(best_net.fitness > best_fittness)
                    {
                        best_fittness = best_net.fitness;
                        last_fittness_increase_generation_id = generation_counter;
                    }
                }
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
                updater.update_log(String.Format("Creating net: {0}", i));
                Neural_Network cur_net = new Neural_Network(i, inputs, outputs, this, bias_enabled);
                
                neural_networks.Add(cur_net);   
            }
            best_net = neural_networks[0];
            drawer.update_generation_id(generation_counter);

        }

        public void next_gen()
        {
            // 1: fitness function
            // 2: specification
            // 3: create offspring
            // repeat

            // fittnes function call
            stopped = false;
            Console.WriteLine("Start fittness functions for {0} nets", neural_networks.Count);
            for (int i = 0; i < neural_networks.Count; i++)
            {
                neural_networks[i].fitness = 0;
                fittness_function(neural_networks[i]);

            }
            get_best_net(); // get best net
            species_manager.group_networks_into_species(); // group nets into species
            check_progress();
            species_manager.create_offspring(); // create the offspring of the nets. 25% of the offspring gets mutated, the rest gets created through crossover. Here the adjustet fittness functions calculate the amount of offspring each species is allowed to have

            generation_counter += 1;
            drawer.update_generation_id(generation_counter);


            drawer.draw_net(best_net);
        }

        public void check_progress()
        {
            if((generation_counter - last_fittness_increase_generation_id) > no_progress_deletion_threshold)
            {
                species_manager.clear_species();
                Console.WriteLine("Deleting all species besides the best 2");
            }
        }
        
        public void remove_display()
        {
            drawer.remove_all_net_elements();
        }

        public void show_net(int net_id)
        {
            for(int i = 0; i<neural_networks.Count; i++)
            {
                if(neural_networks[i].id == net_id)
                {
                    drawer.draw_net(neural_networks[i]);
                    cur_net = neural_networks[i];
                    break;
                }
            }
        }

        public void redraw_net()
        {
            drawer.remove_all_net_elements();
            drawer.draw_net(cur_net);
        }

        public void fittness_function(Neural_Network network_tested)
        {

            // here we test the net if it can create a xor gate. we test 4 different instances of this gate

            // 1: both gates r activated, output should be deactivated
            List<double> input = new List<double> { 1, 1 };
            List<double> output = network_tested.get_output(input);

            if (output[0] > node_activation_threshold)
            {
                network_tested.fitness -= 10;
            }
            else
            {
                network_tested.fitness += 500;
            }
            


            // 1: only only one gate ist activated, output should be activated
            input = new List<double> { -1, 1 };
            output = network_tested.get_output(input);

            if (output[0] > node_activation_threshold)
            {
                network_tested.fitness += 500;
            }
            else
            {
                network_tested.fitness -= 10;
            }
            // 2: only only one gate ist activated, output should be activated - but switched
            input = new List<double> { 1, -1 };
            output = network_tested.get_output(input);

            if (output[0] > node_activation_threshold)
            {
                network_tested.fitness += 500;
            }
            else
            {
                network_tested.fitness -= 10;
            }
            // 2: both inputs r deactivated, output should be deactivated
            input = new List<double> { -1, -1 };
            output = network_tested.get_output(input);

            if (output[0] > node_activation_threshold)
            {
                network_tested.fitness -= 10;
            }
            else
            {
                network_tested.fitness += 500;
            }

            //network_tested.fitness = network_tested.fitness - (network_tested.nn_connections.Count * 5);
        }

    }
}
