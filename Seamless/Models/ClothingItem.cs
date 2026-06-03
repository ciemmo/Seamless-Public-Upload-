/// By Zachary Davis
/// <summary>
/// Represents a clothing item with various attributes.
/// </summary>

using Seamless.Models.Enums;

namespace Seamless.Models;

public enum WeatherType
{
    Any,
    Warm,
    Cold,
    Rainy,
    Snowy,
    Undefined
}

public enum OccasionType
{
    Casual,
    Formal,
    Sport,
    Party,
    Work,
    Evening,
    Undefined
}

public class ClothingItem
{
    public string ItemId { get; set; }
    public string ItemName { get; set; }
    public string PhotoUrl { get; set; }
    public ClothingType ItemType { get; set; } 
    public ItemCategory ItemTypeCategory { get; set; }
    public string Notes { get; set; }
    public List<string> Tags { get; set; } = new List<string>();
    public bool IsTrashed { get; set; } = false;
}