using MySql.Data.MySqlClient;
using System.Data; 

namespace Connection
{
    public class Connection
    {
        string connectionString = "server=seamless-swe-db-do-user-29190676-0.g.db.ondigitalocean.com;user=doadmin;database=SeamlessDB;port=25060;password=AVNS_7-sfilglpNgGHIHoq5y";
        MySqlConnection connection;

        public Connection()
        {
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Connection Error: " + ex.Message);
            }
        }

        // Used for INSERT, UPDATE, DELETE, CREATE TABLE
        public void query(string s)
        {
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    MySqlCommand command = new MySqlCommand(s, connection);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Query Error: " + ex.Message);
            }
        }

        // --- NEW METHOD: Used to READ data (SELECT) ---
        public DataTable GetData(string sql)
        {
            DataTable table = new DataTable();
            try
            {
                if (connection.State == ConnectionState.Open)
                {
                    MySqlDataAdapter adapter = new MySqlDataAdapter(sql, connection);
                    adapter.Fill(table);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Read Error: " + ex.Message);
            }
            return table;
        }

        public void closeConnection()
        {
            try
            {
                if (connection != null && connection.State == ConnectionState.Open)
                    connection.Close();
            }
            catch { }
        }
    }
}