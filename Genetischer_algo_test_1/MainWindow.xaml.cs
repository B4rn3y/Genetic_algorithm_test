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
        private NEAT_management managment = new NEAT_management();
        private Net_Drawer drawer = new Net_Drawer();
        private Log_Updater updater_log = new Log_Updater();
        private double window_height;
        private double window_width;
        private double stackpanel_width;

        public MainWindow()
        {
            InitializeComponent();
            
            updater_log.log_box = Textbox_log_display;
            managment.updater = updater_log;
            drawer.updater = updater_log;
            managment.drawer = drawer;
            drawer.main_grid = net_grid;
            drawer.textblock_species_id_display = textblock_species_id_display;
            drawer.textblock_generation = textblock_generation;
            drawer.textblock_fittness = textblock_fittness;
            drawer.textblock_species_pop_size_display = textblock_species_pop_size_display;
            BTN_Next_Generation.IsEnabled = false;
        }

        internal NEAT_management Managment { get => managment; set => managment = value; }
        internal Net_Drawer Drawer { get => drawer; set => drawer = value; }
        internal Log_Updater Updater_log { get => updater_log; set => updater_log = value; }
        internal double Window_height { get => window_height; set => window_height = value; }
        internal double Window_width { get => window_width; set => window_width = value; }
        internal double Stackpanel_width { get => stackpanel_width; set => stackpanel_width = value; }

        private void start_stop_neat(object sender, RoutedEventArgs e)
        {
            
            if (!(TextBox_set_pop_size.Text == String.Empty))
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
                    updater_log.update_log("ERROR: Please enter a valid integer!");
                    return;
                }
                int pop_size = int.Parse(TextBox_set_pop_size.Text);
                Console.WriteLine(String.Format("NEAT Started: Population Size: {0}", pop_size));
                updater_log.update_log(String.Format("NEAT Started: Population Size: {0}", pop_size));

                drawer.stackpanel_width = Stackpanel_w_controls.ActualWidth;
                drawer.width = window_width;
                drawer.height = window_height;

                Managment.start_nets(pop_size);

                BTN_Next_Generation.IsEnabled = true;
            }
            else
            {
                updater_log.update_log("ERROR: No Population Size has been entered!");
            }
        }

        private void BTN_Next_Generation_Click(object sender, RoutedEventArgs e)
        {
            managment.next_gen();
            updater_log.update_log(String.Format("Generation {0} was generated.",managment.generation_counter));
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

        private int _numValue = 0;

        public int NumValue
        {
            get { return _numValue; }
            set
            {
                _numValue = value;
                txtNum.Text = value.ToString();
            }
        }

        public void NumberUpDown()
        {
            InitializeComponent();
            txtNum.Text = _numValue.ToString();
        }

        private void cmdUp_Click(object sender, RoutedEventArgs e)
        {
            NumValue++;
        }

        private void cmdDown_Click(object sender, RoutedEventArgs e)
        {
            NumValue--;
        }

        private void txtNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtNum == null)
            {
                return;
            }

            if (!int.TryParse(txtNum.Text, out _numValue))
                txtNum.Text = _numValue.ToString();
        }

        private void BTN_calculate_generations_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < _numValue; i++)
            {
                managment.next_gen();
                updater_log.update_log(String.Format("Generation {0} was generated.", managment.generation_counter));
                if(managment.best_fittness >= managment.max_fittness)
                {
                    updater_log.update_log("Max Fittness was reached.");
                    break;
                }
            }
        }
    }
}
