using Eco.Persistence.Connection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using WpfApp6.CustomClasses;
using WpfApp6.Models;
using System.IO;
using System.Data.Entity;
using EntityState = Microsoft.EntityFrameworkCore.EntityState;
using static WpfApp6.CustomClasses.AppInt;
using Eco.Framework.Impl.Frontside;

namespace WpfApp6
{
    public static class LoggedInOperators // A static class to manage logged-in operators
    {
        private static PMSContext _drugData = new PMSContext(); 
        // Static context to access the drug data from the database
        public static void refreshDrugsDtaGrid()
        {
            if (GlobalMethods.isA_DoctorLoggedIn() == false) // Check if a doctor is logged in; if not, exit the method
                return;

            _drugData = new PMSContext();//this will refresh the local data 

            string srSearchTerm = "";

            if (GlobalMethods.main.txtSearchByDrugName.Text.Length > 0)
                srSearchTerm = GlobalMethods.main.txtSearchByDrugName.Text;

            IQueryable<TblDrugs> query = _drugData.TblDrugs.AsQueryable();

            // Only filter by drug name if a search term is provided
            if (!string.IsNullOrEmpty(srSearchTerm))
            {
                query = query.Where(pr => pr.DrugName.Contains(srSearchTerm)); // Filter by drug name
            }



            switch (((sortingOption)GlobalMethods.main.cmbSortingDrugs.SelectedItem).whichsort)// Determine sorting option selected in the UI and load data accordingly
            {
                case enwhichSorting.SortByDrugNameAsc:
                    _drugData.TblDrugs.Where(pr => pr.DrugName.Contains(srSearchTerm)).OrderBy(pr => pr.DrugName).Take(100).Load();
                    break;
                case enwhichSorting.SortByDrugNameDesc:
                    _drugData.TblDrugs.Where(pr => pr.DrugName.Contains(srSearchTerm)).OrderByDescending(pr => pr.DrugName).Take(100).Load();
                    break;
                case enwhichSorting.SortByDoseAsc:
                    _drugData.TblDrugs.Where(pr => pr.DrugName.Contains(srSearchTerm)).OrderBy(pr => pr.DoseMg).Take(100).Load();
                    break;
                case enwhichSorting.SortByDoseDesc:
                    _drugData.TblDrugs.Where(pr => pr.DrugName.Contains(srSearchTerm)).OrderByDescending(pr => pr.DoseMg).Take(100).Load();
                    break;
                case enwhichSorting.SortByDrugIdAsc:
                    _drugData.TblDrugs.Where(pr => pr.DrugName.Contains(srSearchTerm)).OrderBy(pr => pr.DrugsId).Take(100).Load();//Lod requires using Microsoft.EntityFrameworkCore;reference
                    break;
                case enwhichSorting.SortByDrugIdDesc:
                    _drugData.TblDrugs.Where(pr => pr.DrugName.Contains(srSearchTerm)).OrderByDescending(pr => pr.DrugsId).Take(100).Load();
                    break;
                default:
                    break;

            
            }
            // Execute the query and take the top 100 results
            query = query.Take(100);

            // Load the data into the local collection
            query.Load();



            //_drugData.TblDrugs.OrderBy(pr => pr.DrugsId).Take(100).Load();//Lod requires using Microsoft.EntityFrameworkCore;reference

            GlobalMethods.main.dataGridDrugs.ItemsSource = _drugData.TblDrugs.Local.ToBindingList(); // Bind the local drug data to the data grid's item source


            GlobalMethods.main.dataGridDrugs.CurrentCellChanged += DataGridDrugs_CurrentCellChanged;  // Subscribe to the event for when the current cell changes

            // Hide the "Treatments" column if it exists
            var treatmentColumn = GlobalMethods.main.dataGridDrugs.Columns
                .FirstOrDefault(c => c.Header.ToString().Equals("Treatments", StringComparison.InvariantCultureIgnoreCase));


            if (treatmentColumn != null)
            {
                treatmentColumn.Visibility = System.Windows.Visibility.Collapsed; // Use Collapsed
            }

            // Make "DrugsId" read-only
            var drugsIdColumn = GlobalMethods.main.dataGridDrugs.Columns
                .FirstOrDefault(c => c.Header.ToString().Equals("DrugsId", StringComparison.InvariantCultureIgnoreCase));

            if (drugsIdColumn != null)
            {
                drugsIdColumn.IsReadOnly = true;
            }

            // Set column widths
            foreach (var column in GlobalMethods.main.dataGridDrugs.Columns)
            {
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); // Set each column equally
            }

            GlobalMethods.main.dataGridDrugs.Items.Refresh(); // Refresh the data grid to reflect changes
        }





        internal static void loadReadDrugData() // Method to load drug data from a CSV file
        {
            Task.Factory.StartNew(() => // Start a new task to handle file reading
            {


                using (PMSContext context = new PMSContext()) // Using statement to ensure the context is disposed properly
                {
                    int irCounter = 1; // Counter for processed lines
                    foreach (var vrLine in File.ReadLines("staticdata\\pddf_2024_10_08.csv"))
                    {
                        try
                        {
                            // Split by comma and trim any whitespace
                            var fields = vrLine.Split(",").Select(f => f.Trim()).ToArray();

                            // Ensure there are enough fields
                            if (fields.Length < 10) continue; // Adjust as necessary

                            TblDrugs myDrug = new TblDrugs();

                            // Assuming the drug name is at index 8 (0-based)
                            string srDrugName = fields[9];
                            myDrug.DrugName = srDrugName;

                            // Look for "MG" in the drug name
                            if (srDrugName.Contains("MG", StringComparison.InvariantCultureIgnoreCase))
                            {
                                // Extract the dosage assuming it is before "MG"
                                var dosagePart = srDrugName.Split(new[] { "MG", "mg" }, StringSplitOptions.None)[0].Trim();

                                if (decimal.TryParse(dosagePart.Split(' ').Last(), out decimal doseMg))
                                {
                                    myDrug.DoseMg = doseMg;
                                }
                            }
                            if (myDrug.DoseMg != 0)
                            {
                                var vrSelect = context.TblDrugs.Where(pr => pr.DrugName == myDrug.DrugName && pr.DoseMg == myDrug.DoseMg).FirstOrDefault();
                                if (vrSelect == null) // Check if it doesn't exist
                                {
                                    context.TblDrugs.Add(myDrug);
                                }
                                
                            }
                            irCounter++;
                            // Save changes in batches 
                            if (irCounter % 100 == 0)
                            {
                                context.SaveChanges();
                                setDrugScreenMsg("So far inserted drugs count" + irCounter.ToString("D8"));
                            }
                            irCounter++;
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine($"Checking if already exists: {myDrug.DrugName} with Dose: {myDrug.DoseMg}");

                            Console.WriteLine($"Error processing line {irCounter}: {ex.Message}");
                        }
                    }
                    // Final save for any remaining records
                    //context.SaveChanges();





                }
            });    
        }    
        private static void DataGridDrugs_CurrentCellChanged(object? sender, EventArgs? e)         // Event handler for when the current cell in the data grid changes
        {
            // Create a message showing the number of added, modified, and deleted drug entries
            string srMsg = $"New: {_drugData.ChangeTracker.Entries().Count(e => e.State == EntityState.Added)}, " +
                  $"Modified: {_drugData.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified)}, " +
                  $"Deleted: {_drugData.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted)} Drugs to be saved";
            setDrugScreenMsg(srMsg); // Set the message on the drug screen

        }

       
        public static void saveDrugChange()        // Method to save changes to the database
        {
            // Save changes to the database
            if (GlobalMethods.isA_DoctorLoggedIn() == false)
                return;
            _drugData.SaveChanges();
            
            setDrugScreenMsg("Changes(update/delete/add) are saved at the database.");

        }

        public static void setDrugScreenMsg(string srMsg) // Method to set a message on the drug screen
        {
            GlobalMethods.main.lblDrugScreen.Dispatcher.BeginInvoke(new Action(() =>
            {
                GlobalMethods.main.lblDrugScreen.Content = srMsg; // Update the label content


            }));
           
        }
        public static BindingList<T> ToBindingList<T>(this IList<T> source)   // Extension method to convert an IList to a BindingList
        {
            return new BindingList<T>(source); // Wrap the IList in a BindingList
        }
        public static void DeleteSelectedDrug()       // Method to delete the selected drug from the data grid
        {

            if (GlobalMethods.isA_DoctorLoggedIn() == false)
                return;
            var selectedItem = (TblDrugs)GlobalMethods.main.dataGridDrugs.SelectedItem; // Get the selected drug item
            if (selectedItem != null)
            {
                var drugToRemove = _drugData.TblDrugs.Local.FirstOrDefault(pr => pr.DrugsId == selectedItem.DrugsId);
                if (drugToRemove != null)
                {
                    _drugData.TblDrugs.Local.Remove(drugToRemove);
                    DataGridDrugs_CurrentCellChanged(null, null); // Update the current cell changed event to refresh the message
                }
            }
        }


    }

    

}
