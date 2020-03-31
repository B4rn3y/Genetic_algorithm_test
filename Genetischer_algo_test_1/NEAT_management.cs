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
        public int best_fittness = 0;
        public bool running = false;
        public int inputs = 2, outputs = 1;
        public Net_Drawer drawer;
        public Log_Updater updater;
        public Neural_Network best_net;
        public Neural_Network cur_net;
        public ListBox net_listbox;
        public bool bias_enabled = true;
        public List<Neural_Network> neural_networks = new List<Neural_Network>();
        public static Random random = new Random();
        public Speciesism species_manager;
        public Crossover crossover_nets;


        public NEAT_management(int pop_size)
        {
            int node_counter = inputs + outputs;
            if(bias_enabled)
            {
                node_counter += 1;
            }

            species_manager = new Speciesism(node_counter);
            crossover_nets = new Crossover(this);

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
            net_listbox.Items.Clear();
            for (int i = 0; i < pop_size; i++)
            {
                Console.WriteLine(String.Format("Creating net: {0}", i));
                updater.update_log(String.Format("Creating net: {0}", i));
                Neural_Network cur_net = new Neural_Network(i, inputs, outputs, this, bias_enabled);
                neural_networks.Add(cur_net);
                ListBoxItem litem = new ListBoxItem();
                litem.Content = String.Format("Net {0} - Fitness {1}", i, 0);
                
                litem.Tag = i;
                net_listbox.Items.Add(litem);
                if (i == 0)
                {
                    litem.IsSelected = true;
                }
            }
        }

        public void refresh_listbox()
        {
            drawer.remove_all_net_elements();
            net_listbox.Items.Clear();

            for (int i = 0; i < neural_networks.Count; i++)
            { 
                ListBoxItem litem = new ListBoxItem();
                litem.Content = String.Format("Net {0} - Fitness {1}", i, neural_networks[i].fitness);

                litem.Tag = i;
                net_listbox.Items.Add(litem);
                if (i == 0)
                {
                    litem.IsSelected = true;
                }
            }
        }
        
        public void remove_display()
        {
            drawer.remove_all_net_elements();
            net_listbox.Items.Clear();
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

        public void fittness_function()
        {
            for(int i = 0; i<neural_networks.Count; i++)
            {

            }
        }

    }
}
