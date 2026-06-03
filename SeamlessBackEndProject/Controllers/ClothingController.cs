using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace SeamlessBackEndProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClothingController : ControllerBase
    {
        private DbHelper _db = new DbHelper();

        public class ClothingItem
        {
            public string ImageData { get; set; } = "";
            public List<string> Tags { get; set; } = new List<string>();
        }

        [HttpPost]
        public IActionResult AddClothing([FromBody] ClothingItem item)
        {
            if (string.IsNullOrEmpty(item.ImageData)) return BadRequest("No image");

            //Generate Random IDs 
            Random rnd = new Random();
            int itemID = rnd.Next(1, 100000);

            // Loop through every tag and sort it into the correct column
            foreach (var t in item.Tags)
            {
                int tagID = rnd.Next(1, 100000);
                string sql = "";

                if (new[] { "Casual", "Formal", "Semi-Formal", "Athletic" }.Contains(t))
                {
                    sql = $"INSERT INTO Tag (tagID, itemID, occasion) VALUES ('{tagID}','{itemID}','{t}')";
                }
                else if (new[] { "Hot", "Warm", "Cool", "Freezing" }.Contains(t))
                {
                    sql = $"INSERT INTO Tag (tagID, itemID, temperature) VALUES ('{tagID}','{itemID}','{t}')";
                }
                else if (new[] { "Long Sleeve", "Short Sleeve", "Sleeveless" }.Contains(t))
                {
                    sql = $"INSERT INTO Tag (tagID, itemID, shirt) VALUES ('{tagID}','{itemID}','{t}')";
                }
                else if (new[] { "Vest", "Hoodie", "Jacket" }.Contains(t))
                {
                    sql = $"INSERT INTO Tag (tagID, itemID, jacket) VALUES ('{tagID}','{itemID}','{t}')";
                }
                else if (new[] { "Shorts", "Skirt", "Mid-Length Pants", "Long Pants", "Leggings" }.Contains(t))
                {
                    sql = $"INSERT INTO Tag (tagID, itemID, lowerBody) VALUES ('{tagID}','{itemID}','{t}')";
                }
                else if (new[] { "Shoes", "Socks", "Hats", "Jewelry" }.Contains(t))
                {
                    sql = $"INSERT INTO Tag (tagID, itemID, misc) VALUES ('{tagID}','{itemID}','{t}')";
                }
                else
                {
                    // Default fallback (Color Category)
                    sql = $"INSERT INTO Tag (tagID, itemID, colorCategory) VALUES ('{tagID}','{itemID}','{t}')";
                }

                // Execute the specific insert for this tag
                try { _db.ExecuteQuery(sql); }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Tag Error: " + ex.Message); }
            }

            return Ok(new { Message = "Saved to Database!" });
        }

        [HttpGet]
        public IActionResult GetCloset()
        {
            // Simple select to get everything back
            
            string sql = "SELECT * FROM Tag";
            DataTable table = _db.GetDataTable(sql);

            // For now, we return the raw list so you can see it works
        
            return Ok(new { Message = "Connected to DB", Count = table.Rows.Count });
        }
    }
}