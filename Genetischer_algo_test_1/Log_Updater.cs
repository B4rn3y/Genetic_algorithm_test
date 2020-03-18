using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Genetischer_algo_test_1
{
    class Log_Updater
    {
        public TextBox log_box;

        public void update_log(string text)
        {
            if (text != string.Empty)
            {

                string new_Message = "[" + DateTime.Now.ToString("HH:mm") + "] " + text + "\n";
                log_box.Text = log_box.Text + new_Message;
                log_box.ScrollToEnd();

            };
        }
    }
}
