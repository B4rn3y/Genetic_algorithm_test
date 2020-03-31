using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Genetischer_algo_test_1
{
    class Net_Drawer
    {
        public double stackpanel_width = 0;
        public double width = 0;
        public double height = 0;
        public Grid main_grid;
        public Canvas mycanvas;
        public Log_Updater updater;
        public List<Ellipse> ellipses = new List<Ellipse>();
        public List<int> ellipses_int_list = new List<int>();
        public List<Line> lines = new List<Line>();
        public List<TextBlock> node_id_elements = new List<TextBlock>();
        public Neural_Network cur_net;
        public List<TextBlock> connection_id_element = new List<TextBlock>();

        public void draw_net(Neural_Network net)
        {
            cur_net = net;
            /*
            // delete every ellipse drawn on the screen
            for(int k = 0; k<ellipses.Count; k++)
            {
                mycanvas.Children.Remove(ellipses[k]);
            }*/
            remove_all_net_elements();

            double layer_distance = mycanvas.ActualWidth / (net.layers + 1);

            for (int i = 0; i < net.layers; i++)
            {
                List<Node> nodes_to_draw = new List<Node>();
                for (int k = 0; k < net.nn_nodes.Count; k++)
                {
                    if (net.nn_nodes[k].layer == i)
                    {
                        nodes_to_draw.Add(net.nn_nodes[k]);
                    }
                }

                double node_distance_height = mycanvas.ActualHeight / (nodes_to_draw.Count + 1);
                double x = layer_distance * (i+1);
                // draw every node
                for (int r = 0; r < nodes_to_draw.Count; r++)
                {
                    double y = node_distance_height * (r+1);
                    draw_circle(x,y, nodes_to_draw[r]);
                }
            }

            draw_lines();
            draw_elements();
        }

        public void draw_elements()
        {
            for(int i = 0; i<lines.Count; i++)
            {
                mycanvas.Children.Add(lines[i]);
            }

            for (int i = 0; i < ellipses.Count; i++)
            {
                mycanvas.Children.Add(ellipses[i]);
            }

            for (int i = 0; i < node_id_elements.Count; i++)
            {
                mycanvas.Children.Add(node_id_elements[i]);
            }

            for (int i = 0; i < connection_id_element.Count; i++)
            {
                mycanvas.Children.Add(connection_id_element[i]);
            }
            
        }

        public void remove_all_net_elements()
        {
            // delete every ellipse drawn on the screen
            for (int k = 0; k < ellipses.Count; k++)
            {
                mycanvas.Children.Remove(ellipses[k]);
            }
            // delete existing lines
            for (int i = 0; i < lines.Count; i++)
            {
                mycanvas.Children.Remove(lines[i]);
            }
            // delete existing textbklocks
            for (int i = 0; i < node_id_elements.Count; i++)
            {
                mycanvas.Children.Remove(node_id_elements[i]);
            }
            // delete textblocks of connections
            for (int i = 0; i < connection_id_element.Count; i++)
            {
                mycanvas.Children.Remove(connection_id_element[i]);
            }
            // delete vars
            ellipses = new List<Ellipse>();
            lines = new List<Line>();
            ellipses_int_list = new List<int>();
            node_id_elements = new List<TextBlock>();
            connection_id_element = new List<TextBlock>();
        }




        // a funciton to return all nodes this node is leading to
        public List<Connection> find_node_connections_by_id(int node_id)
        {
            Node start_node = null;
            List<Connection> connections = new List<Connection>();
            // Loop through every node to find the node to this specific id
            for(int i = 0; i<cur_net.nn_nodes.Count; i++)
            {
                if(cur_net.nn_nodes[i].id == node_id)
                {
                    start_node = cur_net.nn_nodes[i];
                    break;
                }
            }

            if (start_node == null)
            {
                return connections;
            }
            else
            {
                // loop through all connections to find every node this node is leading to
                for (int i = 0; i < cur_net.nn_connections.Count; i++)
                {
                    if(cur_net.nn_connections[i].start_node == start_node)
                    {
                        connections.Add(cur_net.nn_connections[i]);
                    }
                }
                return connections;
            }
        }

        public void draw_lines()
        {
            // delete existing lines
            for(int i = 0; i<lines.Count; i++)
            {
                mycanvas.Children.Remove(lines[i]);
            }

            for(int n = 0; n<ellipses.Count; n++)
            {
                Ellipse cur_ellipse = ellipses[n];
                int node_id = ellipses_int_list[n];

                List<Connection> connections = find_node_connections_by_id(node_id);
                // there r nodes this node leads to
                if(connections.Count > 0)
                {
                    // loop through this nodes to find the ellipse representing it
                    for(int i = 0; i< connections.Count;i++)
                    {
                        // save the id we r currently looking for
                        Connection cur_connection = connections[i];
                        int node_id_to_look_for = cur_connection.end_node.id;
                        // loop through every drawn ellipse and compare it to our id
                        for(int a = 0; a<ellipses_int_list.Count; a++)
                        {
                            // if id matches draw the line
                            if(node_id_to_look_for == ellipses_int_list[a])
                            {
                                double start_ellipse_x = Canvas.GetLeft(cur_ellipse)+20;
                                double start_ellipse_y = Canvas.GetTop(cur_ellipse)+10;

                                double end_ellipse_x = Canvas.GetLeft(ellipses[a]);
                                double end_ellipse_y = Canvas.GetTop(ellipses[a])+10;

                                Line cur_line = new Line();
                                //mycanvas.Children.Add(cur_line);
                                lines.Add(cur_line);

                                cur_line.StrokeThickness = 2;

                                if(cur_connection.disabled)
                                {
                                    cur_line.Stroke = Brushes.Yellow;
                                } else if(cur_connection.weight > 0)
                                {
                                    cur_line.Stroke = Brushes.Red;
                                } else if(cur_connection.weight < 0)
                                {
                                    cur_line.Stroke = Brushes.Blue;
                                } else
                                {
                                    cur_line.Stroke = Brushes.Black;
                                }
                                cur_line.X1 = start_ellipse_x; 
                                cur_line.Y1 = start_ellipse_y; 

                                cur_line.X2 = end_ellipse_x; 
                                cur_line.Y2 = end_ellipse_y; 


                                //Console.WriteLine(String.Format("{0}", start_ellipse_x));
                                //Console.WriteLine(String.Format("{0}", end_ellipse_x));
                                //Console.WriteLine(String.Format("{0}", start_ellipse_y));
                                //Console.WriteLine(String.Format("{0}", end_ellipse_y));

                                start_ellipse_x -= 20;
                                start_ellipse_y -= 10;
                                end_ellipse_y -= 10;
                                TextBlock text = new TextBlock();
                                text.Text = String.Format("{0}", connections[i].innovation_number);
                                TextBlock.SetForeground(text, Brushes.Green);
                                Canvas.SetTop(text, start_ellipse_y + ((end_ellipse_y - start_ellipse_y) /2));
                                Canvas.SetLeft(text, start_ellipse_x + ((end_ellipse_x - start_ellipse_x) / 2));
                                connection_id_element.Add(text);
                            }
                        }
                    }
                }
            }
        }

        public void draw_circle(double x, double y, Node node_to_draw)
        {
            if(mycanvas == null)
            {
                Console.WriteLine("ERROR: myCanvas is null");
                updater.update_log("ERROR: myCanvas is null");
                return;
            }

            TextBlock text = new TextBlock();
            text.Text = String.Format("{0}",node_to_draw.innovation_number);
            TextBlock.SetForeground(text, Brushes.Orange);
            Canvas.SetTop(text,y+2);
            if(node_to_draw.id >= 10)
            {
                Canvas.SetLeft(text, x + 3);
            }
            else
            {
                Canvas.SetLeft(text, x + 7);
            }
            node_id_elements.Add(text);

            Ellipse cur_ellipse = new Ellipse();
            //mycanvas.Children.Add(cur_ellipse);
            if(node_to_draw.bias)
            {
                cur_ellipse.Fill = Brushes.Green;
            } else if(node_to_draw.output)
            {
                cur_ellipse.Fill = Brushes.Red;
            } else if(node_to_draw.input)
            {
                cur_ellipse.Fill = Brushes.Blue;
            } else
            {
                cur_ellipse.Fill = Brushes.Black;
            }
            cur_ellipse.StrokeThickness = 1;
            cur_ellipse.Stroke = Brushes.Black;
            cur_ellipse.Height = 20;
            cur_ellipse.Width = 20;
            Canvas.SetTop(cur_ellipse,y);
            Canvas.SetLeft(cur_ellipse, x);
            ellipses.Add(cur_ellipse);
            ellipses_int_list.Add(node_to_draw.id);

            //mycanvas.Children.Add(text);
        }
    }
}
