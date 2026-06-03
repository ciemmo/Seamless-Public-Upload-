using Seamless;

namespace Seamless
{
    
    public partial class Login : ContentPage
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void OnSignInClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            // Simple Validation
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both email and password", "OK");
                return;
            }

            SignInButton.IsEnabled = false;
            SignInButton.Text = "Checking...";

            try
            {
                ApiService service = new ApiService();
                bool isSuccess = await service.Login(email, password);

                if (isSuccess)
                {
                   
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    await DisplayAlert("Login Failed", "Incorrect email or password.\nTry 'admin' / 'password'", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Connection failed: {ex.Message}", "OK");
            }
            finally
            {
                SignInButton.IsEnabled = true;
                SignInButton.Text = "Sign In";
            }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            // Safe navigation back
            if (Navigation.NavigationStack.Count > 1)
            {
                await Navigation.PopAsync();
            }
        }

        private async void OnJoinNowClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegistrationPage());
        }
    }
}