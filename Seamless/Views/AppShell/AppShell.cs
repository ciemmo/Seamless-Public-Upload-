namespace Seamless
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            // Throw away the Shell/Flyout and go back to the Landing Page
            Application.Current.MainPage = new NavigationPage(new MainPage());
        }

        // Handle the Settings click if you have it
        private async void OnSettingsMenuItemClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Settings", "Coming soon!", "OK");
        }
    }
}