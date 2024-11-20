using Eco.UmlRt.Impl;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Forms.VisualStyles;
using System.Windows.Threading;
using WpfApp6.CustomClasses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WpfApp6
{
    public static class GlobalMethods
    {
        public static MainWindow main;  // Static reference to the MainWindow instance, allows access from other parts of the application
        private static DateTime _appRunDate = DateTimeHelper.ServerTime;


        // Method to start operations related to displaying information
        public static void startInfoOperations()
        {
            main.btnLogout.Visibility = System.Windows.Visibility.Hidden; //Hide the logout button initially when starting the operations

            //  DispatcherTimer setup
            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(updateInfoLabel);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

        }
        //Method to update the information displayed on the UI
        private static void updateInfoLabel(object sender, EventArgs e) 
        {
            if(LoginOperations.loggedUser == null) // Check if no user is logged in
            {
                main.lblLoginStatus.Dispatcher.BeginInvoke(() => // Display a message indicating the user is not logged in and show the application run time
                {
                    main.lblLoginStatus.Content = "Not logged In. Application Run Time: " + (DateTimeHelper.ServerTime - _appRunDate).TotalSeconds.ToString("F2")+ "seconds" ;
                });
                return;

            }
            main.lblLoginStatus.Dispatcher.BeginInvoke(() => // If a user is logged in, update the label with user details and session run time
            {
                var sessionDuration = DateTimeHelper.ServerTime - LoginOperations.dtLoggedTime;
                string userName = $"{LoginOperations.loggedUser.FirstName.Trim()} {LoginOperations.loggedUser.LastName.Trim()}";
                main.lblLoginStatus.Content = $" Logged User: {LoginOperations.loggedUser.FirstName}{LoginOperations.loggedUser.LastName},Session Run Time: " + (DateTimeHelper.ServerTime - LoginOperations.dtLoggedTime).TotalSeconds.ToString("F2") + "seconds";


            });
            
        }
        private static string ComputeSha256Hash(this string rawData)
        {
            //Create a SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                //CONVERT byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }

        }
        // Method to return a hashed password using a random string for salting
        public static string returnUserPw(string srUserPw, string srRandomString)
        {
            //no rainbow table will be useful with salted encrypted password
            return ComputeSha256Hash(srRandomString + srUserPw);
        }

        // Method to return a static IP address as a string
        public static string returnUserIp() 
        {
            return "12.34.64.123";
        }

        public static void setDrugsPanelVisibility()
        {
            // Check if the user is logged in and their user type
            if (isA_DoctorLoggedIn() == false)
            {
                GlobalMethods.main.tabDrugs.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                GlobalMethods.main.tabDrugs.Visibility = System.Windows.Visibility.Visible;
            }

        }

        public static bool isA_DoctorLoggedIn()
        {
            if (LoginOperations.loggedUser?.UserType == 1)
                return true;
            return false;

        }
        public static void ChangeLoginStatus()
        {
            // Toggle the enabled state of the login and register tabs
            main.tabLogin.IsEnabled = !main.tabLogin.IsEnabled;
            main.tabRegister.IsEnabled = !main.tabRegister.IsEnabled;
            main.tabLogin.IsEnabled = !main.tabLogin.IsEnabled;


            // Toggle the visibility of the logout button
            switch (main.btnLogout.Visibility)
            {
                case System.Windows.Visibility.Visible:
                    main.btnLogout.Visibility = System.Windows.Visibility.Hidden;
                    break;
                case System.Windows.Visibility.Hidden:
                    main.btnLogout.Visibility = System.Windows.Visibility.Visible;
                    break;
                case System.Windows.Visibility.Collapsed: 
                    break; // Do nothing if collapsed
                default:
                    break; // Do nothing for unexpected cases

            }

            GlobalMethods.setDrugsPanelVisibility();  // Update the drugs panel visibility

        }
        public static BindingList<T> ToBindingList<T>(this IList<T> source)  // Extension method to convert IList to BindingList
        {
            return new BindingList<T>(source); // Return a new BindingList from the source
        }
    }
        

    static public class DateTimeHelper // Helper class for date and time operations
    {
        public static DateTime ServerTime // Property to get the current server time as UTC
        {
            get { return DateTime.UtcNow; }
        }
    }
    
}
