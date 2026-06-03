using System.Data;
using MySql.Data.MySqlClient;

namespace SeamlessBackEndProject
{
    public class DbHelper
    {
        // CONNECTION STRING
        private string connectionString = "server=seamless-swe-db-do-user-29190676-0.g.db.ondigitalocean.com;user=doadmin;database=SeamlessDB;port=25060;password=AVNS_7-sfilglpNgGHIHoq5y";

        // EXECUTE: For INSERT, UPDATE, DELETE
        public void ExecuteQuery(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DB Error: " + ex.Message);
                    throw; // Send error up to the Controller
                }
            }
        }

        //GET DATA: For SELECT (Login / Loading Closet)
        public DataTable GetDataTable(string sql)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        DataTable result = new DataTable();
                        adapter.Fill(result);
                        return result;
                    }
                }
            }
        }
    }
}