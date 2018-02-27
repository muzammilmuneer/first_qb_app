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
using System.Data;
using System.IO;
using QBFC13Lib;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;

            try
            {
                //Create the session Manager object
                sessionManager = new QBSessionManager();

                //Create the message set request object to hold our request
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 8, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                //Connect to QuickBooks and begin a session
                sessionManager.OpenConnection("", "WpfApplication1");
                connectionOpen = true;
                sessionManager.BeginSession("", ENOpenMode.omDontCare);
                sessionBegun = true;

                ICustomerAdd customerAddRq = requestMsgSet.AppendCustomerAddRq();
                customerAddRq.Name.SetValue(Customer.Text);

                //Send the request and get the response from QuickBooks
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ICustomerRet customerRet = (ICustomerRet)response.Detail;

                QuickBooksID.Text = customerRet.ListID.GetValue();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                //End the session and close the connection to QuickBooks
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
            }
        }


        private void view_customers_Click(object sender, RoutedEventArgs e)
        {
            bool sessionBegun = false;
            bool connectionOpen = false;
            QBSessionManager sessionManager = null;

            try
            {
                //Create the session Manager object
                sessionManager = new QBSessionManager();

                //Create the message set request object to hold our request
                IMsgSetRequest requestMsgSet = sessionManager.CreateMsgSetRequest("US", 8, 0);
                requestMsgSet.Attributes.OnError = ENRqOnError.roeContinue;

                //Connect to QuickBooks and begin a session
                sessionManager.OpenConnection("", "WpfApplication1");
                connectionOpen = true;
                sessionManager.BeginSession(@"C:\Users\Public\Documents\Intuit\QuickBooks\Company Files\Imperial2.qbw", ENOpenMode.omDontCare);
                sessionBegun = true;

                ICustomerQuery customerQueryRq = requestMsgSet.AppendCustomerQueryRq();

                //Send the request and get the response from QuickBooks
                IMsgSetResponse responseMsgSet = sessionManager.DoRequests(requestMsgSet);
                IResponse response = responseMsgSet.ResponseList.GetAt(0);
                ICustomerRetList customerRetList = (ICustomerRetList)response.Detail;

                var customers = new List<Customer>();

                if (customerRetList != null)
                {
                    for (int i = 0; i < customerRetList.Count; i++)
                    {
                        ICustomerRet customerRet = customerRetList.GetAt(i);

                        var customer = new Customer
                        {
                            Name = customerRet.Name.GetValue(),
                            Id = customerRet.ListID.GetValue()
                            //EditSequence = customerRet.EditSequence.GetValue()
                        };

                        customers.Add(customer);
                    }
                }
                dataGrid.ItemsSource = customers;
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                //End the session and close the connection to QuickBooks
                if (sessionBegun)
                {
                    sessionManager.EndSession();
                }
                if (connectionOpen)
                {
                    sessionManager.CloseConnection();
                }
            }
        }

    }

    public class Customer
    {
        public string Id { get; set; }

        public string Name { get; set; }

     
    }
}
