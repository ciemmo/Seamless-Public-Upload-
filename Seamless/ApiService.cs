using System.Net.Http.Json;
using Microsoft.Maui.Devices;

namespace Seamless
{
    public class ApiService
    {
        HttpClient _client;
      
     
        const string Port = "7220";

        public ApiService()
        {
            // Setup the Connection
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;

            _client = new HttpClient(handler);

            string baseUrl = (DeviceInfo.Platform == DevicePlatform.Android)
                             ? $"https://10.0.2.2:{Port}"
                             : $"https://localhost:{Port}";

            _client.BaseAddress = new Uri(baseUrl);
            _client.Timeout = TimeSpan.FromSeconds(10);
        }

        // TOOL 1: Login 
        public async Task<bool> Login(string username, string password)
        {
            try
            {
                var loginData = new { Username = username, Password = password };
                var response = await _client.PostAsJsonAsync("/Auth/login", loginData);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // TOOL 2: Register 
        public async Task<bool> Register(string user, string pass, string email, string phone, string dob)
        {
            try
            {
                var data = new { Username = user, Password = pass, Email = email, Phone = phone, DOB = dob };
                var response = await _client.PostAsJsonAsync("/Auth/register", data);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // TOOL 3: Weather 
        public async Task<string> GetWeatherForLocation(string city, string state)
        {
            try
            {
                string url = $"/WeatherForecast/{city}/{state}";
                return await _client.GetStringAsync(url);
            }
            catch (Exception ex)
            {
                return $"ERROR: {ex.Message}";
            }
        }
      

public async Task<bool> SaveClothingItem(string base64Image, List<string> tags)
        {
            try
            {
                var item = new
                {
                    ImageData = base64Image,
                    Tags = tags
                };

                // Send to the new ClothingController
                var response = await _client.PostAsJsonAsync("/Clothing", item);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Upload Error: {ex.Message}");
                return false;
            }
        }

        public async Task<List<ClothingResponse>> GetCloset()
        {
            try
            {
                return await _client.GetFromJsonAsync<List<ClothingResponse>>("/Clothing");
            }
            catch
            {
                return new List<ClothingResponse>();
            }
        }

        // Helper class for reading the response
        public class ClothingResponse
        {
            public string ImageData { get; set; }
            public List<string> Tags { get; set; }
        }
    }
    }
