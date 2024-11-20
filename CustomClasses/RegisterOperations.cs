using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfApp6.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using WpfApp6.CustomClasses;


namespace WpfApp6.CustomClasses
{
    public static class RegisterOperations
    {

        public static void CompleteRegister(MainWindow main)
        {
            var optionsBuilder = new DbContextOptionsBuilder<PMSContext>();

            using (PMSContext context = new PMSContext(optionsBuilder.Options))
            {
                // Create a new TblUser object to hold the user data
                TblUser myUser = new TblUser();
                myUser.FirstName = main.txtFirstName1.Text;
                myUser.LastName = main.txtLastName1.Text;

                if (main.txtPassword.Password != main.txtPw2.Password)
                {
                    MessageBox.Show("Error1 Entered password are not matching,Please re-type your password");
                    return;
                }
                Guid obj = Guid.NewGuid();
                myUser.SaltOfPw = obj.ToString();




                // Provide both parameters to returnUserPw

                myUser.UserPw = GlobalMethods.returnUserPw(main.txtPassword.Password, myUser.SaltOfPw);

                myUser.SaltOfPw = obj.ToString(); 
                // Ensure that the user rank/role has been selected
                if (main.cmbUserRank.SelectedIndex < 1)
                {
                    MessageBox.Show("Error! Please enter your user rank/role first");
                    return;
                }
                myUser.UserType = (main.cmbUserRank.SelectedItem as TblUserType).UserTypeId;

                // Validate the email format using Regex
                myUser.UserEmail = main.txtEmail.Text;
                if (!IsValidEmail(myUser.UserEmail))
                {
                    MessageBox.Show("Error! Please enter a valid email address.");
                    return;
                }
           
                myUser.RegisterIp = GlobalMethods.returnUserIp();
                // Try to add user to the database
                try
                {
                    context.TblUser.Add(myUser);
                    context.SaveChanges();
                    MessageBox.Show("User has been successfully registered.");

                  ;

                    //to do after registered 
                    main.txtLoginEmail.Text = main.txtEmail.Text;
                    main.txtLoginPassword.Password = main.txtPassword.Password;
                    main.txtEmail.Text = "";
                    main.txtFirstName1.Text = "";
                    main.txtLastName1.Text = "";
                    main.txtPassword.Password = "";
                    main.txtPw2.Password = "";

                    

                    // Proceed to login
                    LoginOperations.LoginTry(main);

                   

                    

                }
                catch (Exception e)
                {
                    MessageBox.Show("An error has occurred while registering. Error: \n" + e.Message + "\n\n" + e?.InnerException?.Message);
                    return;
                }


            }

        }
        // Method to validate email format
        private static bool IsValidEmail(string Email)
        {
            // Define the regex pattern for a valid email address
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(Email, pattern);
        }

    }
}
