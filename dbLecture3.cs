using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;






public static class dbLecture3 // Static class to handle database operations
    {
        // Consider moving this to a configuration file or environment variable
        public static readonly string ConnectionString = "server=DESKTOP-NCFE678; database=PMS; integrated security=SSPI; persist security info=FALSE; TrustServerCertificate=True;";
        

        public static DataSet selectDataset(string query) // Method to execute a SELECT query and return the results as a DataSet
        {
            DataSet dataSet = new DataSet(); // Create a new DataSet to hold the results

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        dataAdapter.Fill(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"An error occurred: {ex.Message}");

                // Return an empty DataSet if there was an error
                dataSet = new DataSet();
            }

            return dataSet;
        }


        
        public static int updateInsert(string srCommand,  params SqlParameter[] parameters) //Method to execute Insert,Delete , Update
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(srCommand, connection))
                    {
                        // Add parameters to the command if provided
                        if (parameters != null && parameters.Length > 0)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        return command.ExecuteNonQuery(); // ExecuteNonQuery returns the number of rows affected
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                MessageBox.Show("An error occurred: " + ex.Message);
                return -1;
            }



        }

        public class csDbConnection
        {
            private static string srConnectionString = "server=DESKTOP-NCFE678; database=school; integrated security = SSPI; persist security info= FALSE; Trusted_Connection = Yes;";

            public static DataTable db_Select_DataSet(string query)
            {
                DataTable dataSet = new DataTable();
                using (SqlConnection connection = new SqlConnection(srConnectionString))
                {
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    dataAdapter.Fill(dataSet);
                }
                return dataSet;
            }
        }
        public class paramaterizedQueryObj
        {
            public string srParameterName { get; set; }

            public SqlDbType typeofParameter { get; set; }

            public object valueofParameter { get; set; }

        }
        public class parameterizedQuery
        {
            public List<paramaterizedQueryObj> listofParameters = new List<paramaterizedQueryObj>();

            public string srQuery { get; set; }
        }

        public static DataTable db_Parameterized_Select_DataTable(parameterizedQuery queryObject, bool b1setCommit = false)
        {
            DataTable dtResult = new DataTable(); // Use DataTable for the result
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        // Set the command text from the queryObject
                        cmd.CommandText = queryObject.srQuery; // Updated from CommandText to srQuery
                        cmd.Connection = connection;

                        // Add parameters to the command
                        foreach (var param in queryObject.listofParameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.srParameterName, param.typeofParameter) // Updated property names
                            {
                                Value = param.valueofParameter // Updated property name
                            });
                        }

                        using (SqlDataAdapter sqlAdapt = new SqlDataAdapter(cmd))
                        {
                            sqlAdapt.Fill(dtResult); // Fill DataTable directly
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return dtResult;
        }

        public static int db_Parameterized_Update_Delete_insert(parameterizedQuery queryObject, bool b1setCommit = false)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                        cmd.CommandText = queryObject.srQuery;
                        cmd.Connection = connection;

                        foreach (var param in queryObject.listofParameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.srParameterName, param.valueofParameter));
                        }

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
                return -1;
            }

        }
    }





