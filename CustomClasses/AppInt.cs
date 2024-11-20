using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfApp6.Models;
using static WpfApp6.CustomClasses.AppInt;

namespace WpfApp6.CustomClasses
{
    public static class AppInt
    {

        public enum enwhichSorting  // Enum representing different sorting options for drugs
        {
            [Description("Sort By Drug Name Ascending")]
            SortByDrugNameAsc,
            [Description( "Sort By Drug Name Descending")]
            SortByDrugNameDesc,
            [Description("Sort By Drug Dose Ascending")]
            SortByDoseAsc,
            [Description("Sort By Drug Dose Descending")]
            SortByDoseDesc,
            [Description("Sort By Drug Id Ascending")]
            SortByDrugIdAsc,
            [Description("Sort By Drug Id Descending")]
            SortByDrugIdDesc,
        }

        public class sortingOption // Class to hold sorting option information
        {
            public enwhichSorting whichsort {  get; set; } // The sorting method

            public string srDesription {  get; set; }
        }
        private static List<sortingOption> ISortingOptions;         // List to store sorting options

        private static void initSortingOption() // Method to initialize sorting options
        {
            ISortingOptions = new List<sortingOption>(); // Instantiate the list

            foreach (enwhichSorting sort in Enum.GetValues(typeof(enwhichSorting)))       // Iterate through all enum values
            {
                ISortingOptions.Add(new sortingOption() { srDesription = StringValueOfEnum(sort), whichsort = sort });
                // Add a new sorting option with description and enum value



            }

        }
        static string StringValueOfEnum(Enum value) // Helper method to get the description of the enum value
        {
            FieldInfo fi = value.GetType().GetField(value.ToString()); // Get custom attributes of type DescriptionAttribute


            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)  // If a description attribute exists, return its description; otherwise, return the enum name
            {
                return attributes[0].Description; 
            }
            else
            {
                return value .ToString();   
            }

        }

        public static void initAApp(MainWindow main)
        {
            // Create a collection to hold user types
            ObservableCollection<TblUserType> userRanks = new ObservableCollection<TblUserType>();
            userRanks.Add(new TblUserType() { UserTypeId = 0, UserTypeName = "Please Pick User Type" });

            var optionsBuilder = new DbContextOptionsBuilder<PMSContext>();
            // Add a default item prompting user selection
            using (PMSContext context = new PMSContext(optionsBuilder.Options))
            {
                var vrUserType = context.TblUserType; // Fetch user types from the database
                foreach (var item in vrUserType)
                {
                    userRanks.Add(item);
                }

            }
            // Set the ComboBox's item source to the userRanks collection
            main.cmbUserRank.ItemsSource = userRanks;
            main.cmbUserRank.DisplayMemberPath = "UserTypeName";
            main.cmbUserRank.SelectedIndex = 0;

            GlobalMethods.setDrugsPanelVisibility();

            //initialize sorting options
            initSortingOption();

            main.cmbSortingDrugs.ItemsSource = ISortingOptions;
            main.cmbSortingDrugs.DisplayMemberPath = "srDesription";
            main.cmbSortingDrugs.SelectedIndex = 4;
      


        }
        static IEnumerable<object> GetEnum<T>()         // Method to retrieve enum values and names as a collection
        {
            var type = typeof(T);
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var pairs =
                Enumerable.Range(0, names.Length)
                .Select(i => new
                {
                    Name = names.GetValue(i)
                    ,
                    Value = values.GetValue(i)

                })
                .OrderBy(pair => pair.Name);
            return pairs;
        } //method
    }
}
