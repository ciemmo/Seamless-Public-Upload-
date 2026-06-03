using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SeamlessBackEndProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        //  API Key safe on the server
        private const string ApiKey = "96e868353549c8ccc62eee780c732449";
        private readonly HttpClient _httpClient;

        public WeatherForecastController()
        {
            _httpClient = new HttpClient();
        }

        // creates an endpoint 
        [HttpGet("{city}/{state}")]
        public async Task<IActionResult> Get(string city, string state)
        {
            //Build the OpenWeatherMap URL 
            string location = $"{city},{state},US";
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={ApiKey}&units=imperial";

            try
            {
                // Call the external API
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest($"Could not find weather for {city}. Check spelling.");
                }

                string json = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(json);

                // Extract the specific data your app needs
                
                double tempF = doc.RootElement.GetProperty("main").GetProperty("temp").GetDouble();
                string condition = doc.RootElement.GetProperty("weather")[0].GetProperty("description").GetString();
                string cityName = doc.RootElement.GetProperty("name").GetString();

                //Send a clean object back to the App
                var result = new
                {
                    Location = cityName,
                    Temperature = $"{tempF:F1}°F",
                    Condition = condition,
                    Message = $"It is {condition} and {tempF:F0}°F in {cityName}."
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }
    }
}