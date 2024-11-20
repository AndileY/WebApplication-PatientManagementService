using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WpfApp6.Models;

namespace WpfApp6.CustomClasses
{
    public static class LoginOperations
    {

       
        public static TblUser? loggedUser = null; // Static field to hold the currently logged-in user
        public static DateTime dtLoggedTime; // Static field to track the date and time when the user logged in
        public static void LoginTry(MainWindow main)
        {
 


            // Fetch user types from the database
            using (PMSContext context = new PMSContext())
            {
                // Check the number of login attempts from the current IP address within the last 15 minutes
                var vrIpTestResult = context.TblLoginTry.Where(pr => pr.TryIp == GlobalMethods.returnUserIp() & pr.DateofTry.AddMinutes(15) > DateTimeHelper.ServerTime).Count();
                if (vrIpTestResult >= 5) // If there have been 5 or more attempts, block further attempts
                {
                    increaseLoginTry(); // Log the failed attempt

                    MessageBox.Show("Error! You have so many times tried to login within 15 minutes.Try again later");
                    return;


                }
                var vrUserEmail = context.TblUser.FirstOrDefault(pr => pr.UserEmail == main.txtLoginEmail.Text); // Retrieve the user by email from the database
                if (vrUserEmail == null) // If no user is found, notify and exit
                {
                    increaseLoginTry();


                    MessageBox.Show("Error! no such user is found!");
                    return;
                }
                // Check if the provided password matches the stored hash
                if (vrUserEmail.UserPw != GlobalMethods.returnUserPw(main.txtLoginPassword.Password.ToString(), vrUserEmail.SaltOfPw))
                {
                    increaseLoginTry();

                    MessageBox.Show("Error! The password is incorrect");
                    return;

                }

                // If login is successful, store the user and the login time
                loggedUser = vrUserEmail;
                loggedUser.UserPw = null; // you can modify the sensitive information so they won't remain in ram memory
                dtLoggedTime = DateTimeHelper.ServerTime;

                GlobalMethods.ChangeLoginStatus(); // Update the UI to reflect the login status
                if(loggedUser.UserType == 1)
                    GlobalMethods.main.tabDrugs.IsSelected = true;
                MessageBox.Show("You have successful logged in");

            
            }

        }
        // Method to validate email format
        private static bool IsValidEmail(string email)
        {
            // Define the regex pattern for a valid email address
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern);
        }
        private static void increaseLoginTry()
        {
            var optionsBuilder = new DbContextOptionsBuilder<PMSContext>();

            using (PMSContext context = new PMSContext(optionsBuilder.Options))
            {
                TblLoginTry tempTry = new TblLoginTry();
                tempTry.TryIp = GlobalMethods.returnUserIp();
                tempTry.DateofTry = DateTimeHelper.ServerTime;
                context.TblLoginTry.Add(tempTry);
                context.SaveChanges();
                MessageBox.Show("Error! The password is incorrect");
                return;


            }
        }
        public static void tryLogout()
        {

            loggedUser = null;
            GlobalMethods.ChangeLoginStatus();
            GlobalMethods.main.tabLogin.IsSelected = true;
            MessageBox.Show("You have successful logged out");


        }
    }
}
