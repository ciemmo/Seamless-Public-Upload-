namespace Seamless
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        // "Logout" button.
        private async void LogoutBtn_Clicked(object sender, EventArgs e)
        {
            // This will send the user back to the LoginPage
            // and clear the main app from memory.
            Application.Current.MainPage = new Page();
        }
    }
}