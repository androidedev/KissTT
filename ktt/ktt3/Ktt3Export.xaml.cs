using ktt3.Model;
using ktt3.Util;
using ktt3.ViewModel;
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

namespace ktt3
{

    public partial class Ktt3Export : Window
    {
        MainWindowViewModel dataContext;
       

        public Ktt3Export(MainWindowViewModel _dataContext)
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            dataContext = _dataContext;
            InitializeComponent();
            DataContext = dataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
