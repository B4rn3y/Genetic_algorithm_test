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
        public MainWindow()
        {
            InitializeComponent();
            


        }

        private async void start_stop_neat(object sender, RoutedEventArgs e)
        {
            if (!(TextBox_set_pop_size.Text == String.Empty))
            {
                int pop_size = int.Parse(TextBox_set_pop_size.Text);
                Console.WriteLine(String.Format("{0}", pop_size));
            }
            else
            {
                textblock_show_error_messages.Text = "No Population Size has" + Environment.NewLine + "been entered!";
                await Task.Delay(30000);
                textblock_show_error_messages.Text = "";
            }
        }
    }
}
