using System;
using System.Collections.Generic;
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

        public MainWindow()
        {
            InitializeComponent();

            updater_log.log_box = Textbox_log_display;
            managment.updater = updater_log;
            //Console.WriteLine(String.Format("{0}:{1}", ((Panel)Application.Current.MainWindow.Content).ActualWidth, ((Panel)Application.Current.MainWindow.Content).ActualHeight));

        }

        internal NEAT_management Managment { get => managment; set => managment = value; }
        internal Net_Drawer Drawer { get => drawer; set => drawer = value; }
        internal Log_Updater Updater_log { get => updater_log; set => updater_log = value; }
        internal double Window_height { get => window_height; set => window_height = value; }
        internal double Window_width { get => window_width; set => window_width = value; }

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
                    updater_log.update_log("ERROR: Please enter a " + Environment.NewLine + "valid integer!");
                    await Task.Delay(30000);
                    textblock_show_error_messages.Text = "";
                }
                int pop_size = int.Parse(TextBox_set_pop_size.Text);
                Console.WriteLine(String.Format("NEAT Started: Population Size: {0}", pop_size));
                updater_log.update_log(String.Format("NEAT Started: Population Size: {0}", pop_size));
                Managment.population_size = pop_size;
                Managment.running = true;
                Managment.start_nets(pop_size);
                BTN_start_stop.Content = "Stop";
            }
            else
            {
                textblock_show_error_messages.Text = "ERROR: No Population Size has" + Environment.NewLine + "been entered!";
                updater_log.update_log("ERROR: No Population Size has" + Environment.NewLine + "been entered!");
                await Task.Delay(30000);
                textblock_show_error_messages.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double dWidth = -1;
            double dHeight = -1;
            FrameworkElement pnlClient = this.Content as FrameworkElement;
            if (pnlClient != null)
            {
                Window_width = pnlClient.ActualWidth;
                Window_height = pnlClient.ActualHeight;

                Console.WriteLine(String.Format("{0}:{1}",dWidth, dHeight));
            }
        }
    }
}
