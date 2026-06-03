using Seamless; 

namespace Seamless
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Get values from the named boxes
            string first = FirstNameEntry.Text;
            string last = LastNameEntry.Text;
            string email = EmailEntry.Text;
            string pass = PasswordEntry.Text;
            string confirm = ConfirmPasswordEntry.Text;

            // Validation
            if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(pass))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (pass != confirm)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            RegisterButton.IsEnabled = false;
            RegisterButton.Text = "Sending...";

            try
            {
                // Send to Kitchen (Backend)
                ApiService service = new ApiService();

                // Combine names for the username field
                string username = $"{first} {last}";

                // NOTE: We use placeholders for phone/dob since the UI doesn't have them yet
                bool success = await service.Register(username, pass, email, "555-0000", "01/01/2000");

                if (success)
                {
                    await DisplayAlert("Success", "Account created!", "OK");
                    // Go back to Login Page
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Registration failed. Server error.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Connection failed: {ex.Message}", "OK");
            }
            finally
            {
                RegisterButton.IsEnabled = true;
                RegisterButton.Text = "Register";
            }
        }

        private async void OnBackToLoginClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}