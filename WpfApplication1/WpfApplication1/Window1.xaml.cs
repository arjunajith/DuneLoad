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

namespace Duneload
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public int check;
        public bool closecheck;

        public Window1()
        {
            InitializeComponent();
            check = -1;
            closecheck = false;
        }

        private void yesb_Click(object sender, RoutedEventArgs e)
        {
            check = 1;
            closecheck = true;
            this.Close();
        }

        private void nob_Click(object sender, RoutedEventArgs e)
        {
            check = 0;
            closecheck = true;
            this.Close();
        }

    }
}
