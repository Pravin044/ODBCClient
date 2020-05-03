using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
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

namespace ODBCClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OdbcConnection odbcCon;
        OdbcDataAdapter OdbcData;
        DataSet dataSet;
        public MainWindow()
        {
            InitializeComponent();
            var ODBCdrivers = GetOdbcDriverNames();
            cmbDSNList.ItemsSource = EnumDsn(); ;
            //cbmDriverList.ItemsSource = ODBCdrivers;
        }

        private void connectToDB()
        {
            try
            {
                if (btnConnect.Content.ToString().ToLower() == "connect")
                {
                    odbcCon = new OdbcConnection("DSN=" + cmbDSNList.SelectedItem + ";Trusted_Connection=True;USER=" + txtUser.Text + ";PASSWORD=" + txtPassword.Password);
                    odbcCon.Open();
                    if (odbcCon.State.ToString() == "Open")
                    {
                        MessageBox.Show("connected");
                        btnConnect.Content = "Disconnect";

                    }
                }
                else
                {
                    odbcCon.Close();
                    btnConnect.Content = "Connect";
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            connectToDB();

        }


        /// <summary>
        /// Gets the ODBC driver names from the registry.
        /// </summary>
        /// <returns>a string array containing the ODBC driver names, if the registry key is present; null, otherwise.</returns>
        public static string[] GetOdbcDriverNames()
        {
            string[] odbcDriverNames = null;
            using (RegistryKey localMachineHive = Registry.LocalMachine)
            using (RegistryKey odbcDriversKey = localMachineHive.OpenSubKey(@"SOFTWARE\ODBC\ODBCINST.INI\ODBC Drivers"))
            {
                if (odbcDriversKey != null)
                {
                    odbcDriverNames = odbcDriversKey.GetValueNames();
                }
            }

            return odbcDriverNames;
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

        private void btnODBCDataSource_Click(object sender, RoutedEventArgs e)
        {
            var process = System.Diagnostics.Process.Start(@"C:\WINDOWS\SysWOW64\odbcad32.exe.");
            process.WaitForExit();
            cmbDSNList.ItemsSource=null;
            cmbDSNList.ItemsSource = EnumDsn();
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cmbAccessMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbAccessMethod.SelectedIndex==0)
            {
                lblQuary.Visibility = Visibility.Collapsed;
                lblStore.Visibility = Visibility.Collapsed;
                lblTable.Visibility = Visibility.Visible;
                cmbTable.Visibility = Visibility.Visible;
                txtQuary.Visibility = Visibility.Collapsed;
            }
            else if (cmbAccessMethod.SelectedIndex==1)
            {
                lblQuary.Visibility = Visibility.Collapsed;
                lblStore.Visibility = Visibility.Collapsed;
                lblTable.Visibility = Visibility.Visible;
                cmbTable.Visibility = Visibility.Visible;
                txtQuary.Visibility = Visibility.Collapsed;
            }
            else if(cmbAccessMethod.SelectedIndex==2)
            {
                lblQuary.Visibility = Visibility.Visible;
                lblStore.Visibility = Visibility.Collapsed;
                lblTable.Visibility = Visibility.Collapsed;
                cmbTable.Visibility = Visibility.Collapsed;
                txtQuary.Visibility = Visibility.Visible;
            }
            else if(cmbAccessMethod.SelectedIndex==3)
            {
                lblQuary.Visibility = Visibility.Collapsed;
                lblStore.Visibility = Visibility.Visible;
                lblTable.Visibility = Visibility.Collapsed;
                cmbTable.Visibility = Visibility.Visible;
                txtQuary.Visibility = Visibility.Collapsed;
            }

        }
    }
}
