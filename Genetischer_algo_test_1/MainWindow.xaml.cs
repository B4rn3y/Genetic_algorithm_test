using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Genetischer_algo_test_1
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NEAT_management managment = new NEAT_management(0);
        private Net_Drawer drawer = new Net_Drawer();
        private Log_Updater updater_log = new Log_Updater();
        private double window_height;
        private double window_width;
        private double stackpanel_width;

        public MainWindow()
        {
            InitializeComponent();

            managment.net_listbox = listbox_nets;
            updater_log.log_box = Textbox_log_display;
            managment.updater = updater_log;
            drawer.updater = updater_log;
            managment.drawer = drawer;
            drawer.main_grid = net_grid;
        }

        internal NEAT_management Managment { get => managment; set => managment = value; }
        internal Net_Drawer Drawer { get => drawer; set => drawer = value; }
        internal Log_Updater Updater_log { get => updater_log; set => updater_log = value; }
        internal double Window_height { get => window_height; set => window_height = value; }
        internal double Window_width { get => window_width; set => window_width = value; }
        internal double Stackpanel_width { get => stackpanel_width; set => stackpanel_width = value; }

        private async void start_stop_neat(object sender, RoutedEventArgs e)
        {
            // reset any error msg
            textblock_show_error_messages.Text = "";
            // check if the management object already runs, if it does stop it
            if (Managment.running)
            {
                Managment.running = false;
                BTN_start_stop.Content = "Start";
                Console.WriteLine("NEAT Stopped");
                updater_log.update_log("NEAT Stopped");
                Managment.remove_display();

            } else if (!(TextBox_set_pop_size.Text == String.Empty))
            {
                bool exit = false;
                char[] int_chars = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                for(int i = 0; i<TextBox_set_pop_size.Text.Length; i++)
                {
                    char cur_char = TextBox_set_pop_size.Text[i];
                    if(!(int_chars.Contains(cur_char)))
                    {
                        exit = true;
                        break;
                    }
                }

                if (exit)
                {
                    textblock_show_error_messages.Text = "ERROR: Please enter a " + Environment.NewLine + "valid integer!";
                    updater_log.update_log("ERROR: Please enter a valid integer!");
                    await Task.Delay(30000);
                    textblock_show_error_messages.Text = "";
                }
                int pop_size = int.Parse(TextBox_set_pop_size.Text);
                Console.WriteLine(String.Format("NEAT Started: Population Size: {0}", pop_size));
                updater_log.update_log(String.Format("NEAT Started: Population Size: {0}", pop_size));

                drawer.stackpanel_width = Stackpanel_w_controls.ActualWidth;
                drawer.width = window_width;
                drawer.height = window_height;

                Managment.running = true;
                Managment.start_nets(pop_size);
                BTN_start_stop.Content = "Stop";
            }
            else
            {
                Managment.running = false;
                textblock_show_error_messages.Text = "ERROR: No Population Size has" + Environment.NewLine + "been entered!";
                updater_log.update_log("ERROR: No Population Size has been entered!");
                await Task.Delay(30000);
                textblock_show_error_messages.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            FrameworkElement pnlClient = this.Content as FrameworkElement;
            if (pnlClient != null)
            {
                Window_width = pnlClient.ActualWidth;
                Window_height = pnlClient.ActualHeight;
                Console.WriteLine(String.Format("Window:{0}:{1}", pnlClient.ActualWidth, pnlClient.ActualHeight));
            }
        }

        private void Canvas_draw_Loaded(object sender, RoutedEventArgs e)
        {
            drawer.mycanvas = canvas_draw;
            /*
            FrameworkElement pnlClient = this.Content as FrameworkElement;
            if (pnlClient != null)
            {
                Window_width = pnlClient.ActualWidth;
                Window_height = pnlClient.ActualHeight;

                Console.WriteLine(String.Format("Canvas:{0}:{1} - {2}:{3}", Window_width, Window_height, canvas_draw.ActualWidth, canvas_draw.ActualHeight));
            }
            */
        }

        private void BTN_start_Node_mutation_Click(object sender, RoutedEventArgs e)
        {
            if(!(managment.running))
            {
                return;
            }
            managment.cur_net.add_node();
            managment.redraw_net();
        }

        private void BTN_add_connection_mutation_Click(object sender, RoutedEventArgs e)
        {
            if (!(managment.running))
            {
                return;
            }
            managment.cur_net.add_connection();
            managment.redraw_net();
        }

        private void BTN_enable_disable_connection_mutation_Click(object sender, RoutedEventArgs e)
        {
            if (!(managment.running))
            {
                return;
            }
            managment.cur_net.enable_disable_connection();
            managment.redraw_net();
        }

        private void BTN_connection_weight_mutation_Click(object sender, RoutedEventArgs e)
        {
            if (!(managment.running))
            {
                return;
            }
            managment.cur_net.mutate_connection_weight_random();
            managment.redraw_net();
        }

        private void BTN_shift_connection_weight_Click(object sender, RoutedEventArgs e)
        {
            if (!(managment.running))
            {
                return;
            }
            managment.cur_net.mutate_weight_shift();
            managment.redraw_net();
        }

        private void Listbox_nets_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Listbox_nets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox listbox_w_nets = sender as ListBox;
            
            if (listbox_w_nets.HasItems)
            {
                int net_id = listbox_w_nets.SelectedIndex;
                updater_log.update_log(String.Format("Net {0} selected", net_id));
                managment.show_net(net_id);
            }
        }
    }
}
