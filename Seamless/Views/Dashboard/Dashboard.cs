using System.Text.Json;
using Seamless; 

namespace Seamless
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private async void OnGetWeatherClicked(object sender, EventArgs e)
        {
            //Validate Input
            if (string.IsNullOrWhiteSpace(CityEntry.Text) || string.IsNullOrWhiteSpace(StateEntry.Text))
            {
                await DisplayAlert("Error", "Please enter both city and state.", "OK");
                return;
            }

            string city = CityEntry.Text.Trim();
            string state = StateEntry.Text.Trim();

            // Set Loading State
            WeatherLocationLabel.Text = "Searching...";
            WeatherTempLabel.Text = "--";
            WeatherConditionLabel.Text = "Connecting...";

            try
            {
                //Call the Service
                ApiService service = new ApiService();

              
                string jsonResult = await service.GetWeatherForLocation(city, state);

                if (jsonResult.StartsWith("ERROR"))
                {
                    WeatherLocationLabel.Text = "Error";
                    WeatherConditionLabel.Text = jsonResult;
                }
                else
                {
                    using JsonDocument doc = JsonDocument.Parse(jsonResult);
                    var root = doc.RootElement;

                    string location = root.GetProperty("location").GetString();
                    string temp = root.GetProperty("temperature").GetString();
                    string condition = root.GetProperty("condition").GetString();

                    WeatherLocationLabel.Text = location;
                    WeatherTempLabel.Text = temp;
                    WeatherConditionLabel.Text = condition;
                }
            }
            catch (Exception ex)
            {
                WeatherLocationLabel.Text = "Failed";
                WeatherConditionLabel.Text = ex.Message;
            }
        }

        private async void OnViewClosetClicked(object sender, EventArgs e) => await Navigation.PushAsync(new ClosetPage());
        
    }
}