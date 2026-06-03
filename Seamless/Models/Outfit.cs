/// By Zachary Davis
/// <summary>
/// Model representing an outfit, which is a collection of clothing items.
/// </summary>


namespace Seamless.Models
{
    public class Outfit
    {
public string OutfitId { get; set; }
        
        public string ItemName { get; set; } // This will store the Outfit Name
        public string PhotoUrl { get; set; } // This will be a representative photo
        
        public List<string> Tags { get; set; } = new List<string>();
    
        // Store the IDs of ClothingItems that are part of this Outfitw
        public List<string> ClothingItemIds { get; set; } = new List<string>();
    }
};