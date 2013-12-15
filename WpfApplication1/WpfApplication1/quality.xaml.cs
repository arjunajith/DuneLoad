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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace Duneload
{
    /// <summary>
    /// Interaction logic for quality.xaml
    /// </summary>
    public partial class quality : MetroWindow
    {
        public int quali;

        public quality()
        {
            InitializeComponent();
            quali = 360;
        }

        private void q_button_Click(object sender, RoutedEventArgs e)
        {
            if ((q360.IsChecked == false) && (q480.IsChecked == false) && (q720.IsChecked == false) && (q1080.IsChecked == false))
            {
                quali = 360;
                q360.IsChecked = true;
                this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void q360_Checked(object sender, RoutedEventArgs e)
        {
            quali = 360;
            q480.IsChecked = false;
            q720.IsChecked = false;
            q1080.IsChecked = false;
        }

        private void q480_Checked(object sender, RoutedEventArgs e)
        {
            quali = 480;
            q720.IsChecked = false;
            q1080.IsChecked = false;
            q360.IsChecked = false;
        }

        private void q720_Checked(object sender, RoutedEventArgs e)
        {
            quali = 720;
            q360.IsChecked = false;
            q480.IsChecked = false;
            q1080.IsChecked = false;
        }

        private void q1080_Checked(object sender, RoutedEventArgs e)
        {
            quali = 1080;
            q360.IsChecked = false;
            q480.IsChecked = false;
            q720.IsChecked = false;
        }


    }
}
