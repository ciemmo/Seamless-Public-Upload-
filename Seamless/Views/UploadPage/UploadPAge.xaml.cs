namespace Seamless
{
    public partial class UploadPage : ContentPage
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private FileResult _selectedPhoto; // Variable to hold the picked photo

        public UploadPage()
        {
            InitializeComponent();
        }

        // 1. Code for the "Pick Photo" button
        private async void OnPickPhotoClicked(object sender, EventArgs e)
        {
            try
            {
                _selectedPhoto = await MediaPicker.PickPhotoAsync();

                if (_selectedPhoto == null)
                    return; // User cancelled

                // Show a preview
                var stream = await _selectedPhoto.OpenReadAsync();
                PreviewImage.Source = ImageSource.FromStream(() => stream);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to pick photo: {ex.Message}", "OK");
            }
        }

        // 2. Code for the "Upload Post" button
        private async void OnUploadClicked(object sender, EventArgs e)
        {
            if (_selectedPhoto == null)
            {
                await DisplayAlert("Wait", "Please pick a photo first.", "OK");
                return;
            }

            var apiUrl = "https://api.yourapp.com/upload";

            try
            {
                // We use "MultipartFormDataContent" to send a file AND text
                using var content = new MultipartFormDataContent();

                // Add the image file
                var fileStream = await _selectedPhoto.OpenReadAsync();
                content.Add(new StreamContent(fileStream), "file", _selectedPhoto.FileName);

                // Add the description text
                content.Add(new StringContent(DescriptionEditor.Text), "description");

            

                // Send it all to the API
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Your post was uploaded!", "OK");
                    // Go back to the feed page
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Upload failed. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }
    }
}