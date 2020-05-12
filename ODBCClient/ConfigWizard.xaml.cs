using Microsoft.Win32;
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
            cmbDSNList.ItemsSource = EnumDsn();
        }

       

        private void btnODBCDataSource_Click(object sender, RoutedEventArgs e)
        {
            var process = System.Diagnostics.Process.Start(@"C:\WINDOWS\SysWOW64\odbcad32.exe.");
            process.WaitForExit();
            cmbDSNList.ItemsSource = null;
            cmbDSNList.ItemsSource = EnumDsn();
        }

        private List<string> EnumDsn()
        {
            List<string> list = new List<string>();
            list.AddRange(EnumDsn(Registry.CurrentUser));
            list.AddRange(EnumDsn(Registry.LocalMachine));
            return list;
        }

        private IEnumerable<string> EnumDsn(RegistryKey rootKey)
        {
            RegistryKey regKey = rootKey.OpenSubKey(@"Software\ODBC\ODBC.INI\ODBC Data Sources");
            if (regKey != null)
            {
                foreach (string name in regKey.GetValueNames())
                {
                    string value = regKey.GetValue(name, "").ToString();
                    yield return name;
                }
            }
        }

        private void cmbDSNList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Page1NextButtonEnable();
        }

        private void txtUser_TextChanged(object sender, TextChangedEventArgs e)
        {
            Page1NextButtonEnable();
        }

        private void txtPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            Page1NextButtonEnable();
        }


        private void Page1NextButtonEnable()
        {
            if (!string.IsNullOrEmpty(txtUser.Text)&& !string.IsNullOrEmpty(txtPassword.Password)&&cmbDSNList.SelectedIndex>0)
            {
                Page1.CanSelectNextPage = true;
            }
            else
            {
                Page1.CanSelectNextPage = false;
            }
        }
    }
}
