using Seamless;
using Connection; 

namespace Seamless
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //BUILD THE TABLE 
            try
            {
                // Open Connection
                Connection.Connection c = new Connection.Connection();

                // The SQL command to build the table
                string createTableSql = @"
                    CREATE TABLE IF NOT EXISTS Clothing (
                        itemID INT,
                        imagePath TEXT,
                        itemName TEXT
                    );";

                //Run the command
                c.query(createTableSql);

                // Close
                c.closeConnection();

                System.Diagnostics.Debug.WriteLine("SUCCESS: Clothing table created!");
            }
            catch (Exception ex)
            {
                //   a connection problem
                System.Diagnostics.Debug.WriteLine("ERROR creating table: " + ex.Message);
            }
            
        }

        // Clicked "Join Now" -> Go to Registration
        private async void OnJoinNowClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }

        // Tapped "Login" -> Go to Login
        private async void OnLoginTapped(object sender, TappedEventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}