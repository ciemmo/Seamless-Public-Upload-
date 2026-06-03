using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace SeamlessBackEndProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private DbHelper _db = new DbHelper();

        public class AuthRequest
        {
            public string Username { get; set; } = "";
            public string Password { get; set; } = "";
            public string Email { get; set; } = "";
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] AuthRequest request)
        {
            // Check the 'Closet' table for the user 
            string sql = $"SELECT * FROM Closet WHERE (username = '{request.Username}' OR email = '{request.Username}') AND passwordHash = '{request.Password}'";

            DataTable table = _db.GetDataTable(sql);

            if (table.Rows.Count > 0)
            {
                return Ok(new { Message = "Login Successful" });
            }
            return Unauthorized(new { Message = "Wrong username or password" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthRequest request)
        {
            // Generate Random ID 
            Random rnd = new Random();
            int randomID = rnd.Next(1000, 10000);

            //Insert into 'Closet' table
            string sql = $"INSERT INTO Closet (userID, username, passwordHash, email) VALUES ('{randomID}', '{request.Username}', '{request.Password}', '{request.Email}')";

            try
            {
                _db.ExecuteQuery(sql);
                return Ok(new { Message = "User registered successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}