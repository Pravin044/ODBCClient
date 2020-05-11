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

namespace ODBCClient
{
    /// <summary>
    /// Interaction logic for ConfigWizard.xaml
    /// </summary>
    public partial class ConfigWizard : Window
    {
        public ConfigWizard()
        {
            InitializeComponent();
        }

        private void FirstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(FirstNameTextBox.Text))
            {
                Page1.CanSelectNextPage = false;
            }
            else
            {
                Page1.CanSelectNextPage = true;
            }
        }
    }
}
