using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using WpfApp6.Models;
using WpfApp6.CustomClasses;

namespace WpfApp6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window  // Define a partial class for the MainWindow, which inherits from Window
    {

        public MainWindow()
        {
            InitializeComponent();
            GlobalMethods.main = this; // Assign the current instance of MainWindow to the static 'main' variable
            //initComboBox();
            AppInt.initAApp(this);
            GlobalMethods.startInfoOperations();



        }
        private void initComboBox()
        {
            // Create a collection to hold user types
            ObservableCollection<TblUserType> userRanks = new ObservableCollection<TblUserType>();
            userRanks.Add(new TblUserType() { UserTypeId = 0, UserTypeName = "Please Pick User Type" });


            // Create DbContextOptions
            var optionsBuilder = new DbContextOptionsBuilder<PMSContext>();


            // Fetch user types from the database
            using (PMSContext context = new PMSContext(optionsBuilder.Options))
            {
                var vrUserType = context.TblUserType; // Add a default item prompting user selection
                foreach (var item in vrUserType)
                {
                    userRanks.Add(item);
                }

            }
            // Set the ComboBox's item source to the userRanks collection
            cmbUserRank.ItemsSource = userRanks;
            cmbUserRank.DisplayMemberPath = "UserTypeName";
            cmbUserRank.SelectedIndex = 0;


        }

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (cmbUserRank.SelectedIndex < 1) // Check if a user rank has been selected
            {
                MessageBox.Show("Error! Please enter your user rank/role first");
                return;
            }
            // Proceed with the registration process
            RegisterOperations.CompleteRegister(this);

        }

        private void btnLogin_Click(object sender, RoutedEventArgs e) // Event handler for the login button click event
        {
            // Call the LoginTry method from the LoginOperations class,
            // passing the current instance of the window (this) as an argument.
            // This method will handle the login process, such as validating user credentials.
            LoginOperations.LoginTry(this);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            LoginOperations.tryLogout();
        }

        private void btnRefreshDrugs_Click(object sender, RoutedEventArgs e)
        {
            LoggedInOperators.refreshDrugsDtaGrid();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            LoggedInOperators.saveDrugChange();

        }





        private void btnDeleteSelectedDrug_Click(object sender, RoutedEventArgs e)
        {
            LoggedInOperators.DeleteSelectedDrug();

        }

        private void btnLoadDrugData_Click(object sender, RoutedEventArgs e)
        {
            LoggedInOperators.loadReadDrugData();

        }
        private void cmbSortingDrug_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoggedInOperators.refreshDrugsDtaGrid();

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            LoggedInOperators.refreshDrugsDtaGrid();

        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tabDrugs.IsSelected)
                {
                    
                    LoggedInOperators.refreshDrugsDtaGrid();
                }
                if (tabLogin.IsSelected)
                {
                    LoginOperations.LoginTry(this);
                }
                if (tabRegister.IsSelected)
                {
                    RegisterOperations.CompleteRegister(this);
                }

            }
            
        }
    }
}
