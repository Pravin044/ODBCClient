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
        int ReocrdLimit;
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
                    odbcCon = new OdbcConnection("DSN="+ cmbDSNList.SelectedItem+";Uid="+txtUser.Text+";Pwd="+txtPassword.Password);
                    odbcCon.Open();
                    if (odbcCon.State.ToString() == "Open")
                    {
                        MessageBox.Show("connected");
                        btnConnect.Content = "Disconnect";
                        brAccess.Visibility = Visibility.Visible;
                        ResetAllFields();
                        VisibilityState(false);
                    }
                }
                else
                {
                    odbcCon.Close();
                    btnConnect.Content = "Connect";
                    brAccess.Visibility = Visibility.Collapsed;
                    ResetAllFields();
                    VisibilityState(true);

                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void VisibilityState(bool State)
        {
            cmbDSNList.IsEnabled = State;
            btnODBCDataSource.IsEnabled = State;
            txtUser.IsEnabled = State;
            txtPassword.IsEnabled = State;
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
            cmbDSNList.ItemsSource = null;
            cmbDSNList.ItemsSource = EnumDsn();
        }

        private void btnGetData_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                treeView.Items.Clear();
                if (cmbAccessMethod.SelectedIndex == 0)
                {
                    dataSet = new DataSet();
                    string qaury = getColumsNames();
                    OdbcData = new OdbcDataAdapter(qaury, odbcCon);
                    OdbcData.Fill(dataSet);

                   /// if (cmbEnableGrp.SelectedIndex == 0)
                        GetFixedTableGrupData(dataSet);
                    //else
                      //  getFixedTableUnGrpData(dataSet);
                }
                //dataSet = new DataSet();
                ////string qaury = getQuary();
                //string qaury = getColumsNames();
                //OdbcData = new OdbcDataAdapter(qaury, odbcCon);
                //OdbcData.Fill(dataSet);
                //for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                //{

                //    foreach (var childItem in dataSet.Tables[0].Rows[i].ItemArray)
                //    {
                //   TreeViewItem parentItem = new TreeViewItem();
                //    parentItem.Header = i;
                //    treeView.Items.Add(parentItem);


                //    }

                //}


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

        }

        //private void getFixedTableUnGrpData(DataSet dataSet)
        //{
        //    try
        //    {
        //        for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
        //        {
        //            foreach (var childItem in dataSet.Tables[0].Rows[i].ItemArray)
        //            {
        //                TreeViewItem parentItem = new TreeViewItem();
        //                parentItem.Header = childItem + "_" + i;
        //                treeView.Items.Add(parentItem);
        //                DataSet dataSet1 = new DataSet();
        //                string Subqaury = "select " + childItem + " from " + cmbTable.SelectedItem;
        //                OdbcData = new OdbcDataAdapter(Subqaury, odbcCon);
        //                OdbcData.Fill(dataSet1);
        //                for (int index = 0; index < dataSet1.Tables[0].Rows.Count; index++)
        //                {
        //                    foreach (var item in dataSet1.Tables[0].Rows[index].ItemArray)
        //                    {

        //                        TreeViewItem treeChildItem = new TreeViewItem();
        //                        treeChildItem.Header = item;
        //                        parentItem.Items.Add(item);


        //                    }
        //                }


        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        private void GetFixedTableGrupData(DataSet dataSet)
        {
            try
            {
                if (ReocrdLimit >= Convert.ToInt32(txtRecordLimit.Text))
                {
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        foreach (var childItem in dataSet.Tables[0].Rows[i].ItemArray)
                        {
                            TreeViewItem parentItem = new TreeViewItem();
                            parentItem.Header = childItem;
                            treeView.Items.Add(parentItem);
                            DataSet dataSet1 = new DataSet();
                            string Subqaury = "select TOP("+ Convert.ToInt16(txtRecordLimit.Text)+") " + childItem + " from " + cmbTable.SelectedItem;
                            OdbcData = new OdbcDataAdapter(Subqaury, odbcCon);
                            OdbcData.Fill(dataSet1);
                            for (int index = 0; index < dataSet1.Tables[0].Rows.Count; index++)
                            {
                                foreach (var item in dataSet1.Tables[0].Rows[index].ItemArray)
                                {

                                    TreeViewItem treeChildItem = new TreeViewItem();
                                    treeChildItem.Header = item;
                                    parentItem.Items.Add(item);


                                }
                            }


                        }

                    }
                }
                else
                {
                    MessageBox.Show("Please enter Record limit below" + ReocrdLimit);
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex);
            }
        }

        private string getColumsNames()
        {
            return "select COLUMN_NAME from " + odbcCon.Database + ".INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '" + cmbTable.SelectedItem + "'";
        }

        private string getQuary()
        {
            if (cmbAccessMethod.SelectedIndex == 0)
                return "select * from " + cmbTable.SelectedItem;
            if (cmbAccessMethod.SelectedIndex == 1)
                return "Select * from " + cmbTable.SelectedItem;
            if (cmbAccessMethod.SelectedIndex == 2)
                return txtQuary.Text;
            if (cmbAccessMethod.SelectedIndex == 3)
                return "select * from " + cmbTable.SelectedItem;
            return null;
        }

        private void ResetAllFields()
        {
            cmbAccessMethod.SelectedIndex = -1;
            cmbTable.SelectedIndex = -1;
            txtQuary.Text = null;
            treeView.Items.Clear();
        }




        private void cmbAccessMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbAccessMethod.SelectedIndex == 0)
            {
                lblQuary.Visibility = Visibility.Collapsed;
                lblStore.Visibility = Visibility.Collapsed;
                lblTable.Visibility = Visibility.Visible;
                cmbTable.Visibility = Visibility.Visible;
                txtQuary.Visibility = Visibility.Collapsed;
                cmbTable.Items.Clear();
                getTableInfo();

            }
            else if (cmbAccessMethod.SelectedIndex == 1)
            {
                lblQuary.Visibility = Visibility.Collapsed;
                lblStore.Visibility = Visibility.Collapsed;
                lblTable.Visibility = Visibility.Visible;
                cmbTable.Visibility = Visibility.Visible;
                txtQuary.Visibility = Visibility.Collapsed;
            }
            else if (cmbAccessMethod.SelectedIndex == 2)
            {
                lblQuary.Visibility = Visibility.Visible;
                lblStore.Visibility = Visibility.Collapsed;
                lblTable.Visibility = Visibility.Collapsed;
                cmbTable.Visibility = Visibility.Collapsed;
                txtQuary.Visibility = Visibility.Visible;
            }
            else if (cmbAccessMethod.SelectedIndex == 3)
            {
                lblQuary.Visibility = Visibility.Collapsed;
                lblStore.Visibility = Visibility.Visible;
                lblTable.Visibility = Visibility.Collapsed;
                cmbTable.Visibility = Visibility.Visible;
                txtQuary.Visibility = Visibility.Collapsed;
            }

        }

        private void getTableInfo()
        {
            try
            {
                if (odbcCon.State.ToString() == "Open")
                {
                    dataSet = new DataSet();
                    OdbcData = new OdbcDataAdapter("SELECT * FROM " + odbcCon.Database + ".INFORMATION_SCHEMA.TABLES", odbcCon);
                    OdbcData.Fill(dataSet);
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        cmbTable.Items.Add(dataSet.Tables[0].Rows[i].ItemArray[2]);
                    }
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

        private void cmbDSNList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txtUser.Text = null;
            txtPassword.Password = null;
        }

        private void cmbTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (odbcCon.State.ToString() == "Open")
                {
                    dataSet = new DataSet();
                    OdbcData = new OdbcDataAdapter("SELECT count(*) FROM " + cmbTable.SelectedItem, odbcCon);
                    OdbcData.Fill(dataSet);
                   ReocrdLimit=Convert.ToInt32(dataSet.Tables[0].Rows[0].ItemArray[0]);
                    txtRecordLimit.Text = ReocrdLimit.ToString();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
