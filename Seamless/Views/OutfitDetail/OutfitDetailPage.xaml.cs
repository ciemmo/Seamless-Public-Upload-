using Seamless.Models;
using System.Data;
using System.Collections.ObjectModel;
using Connection;

namespace Seamless;

public partial class OutfitDetailPage : ContentPage
{
    private DisplayItem _outfit;
    public ObservableCollection<DisplayItem> OutfitItems { get; set; }

    public OutfitDetailPage(DisplayItem outfit)
    {
        InitializeComponent();
        _outfit = outfit;
        OutfitNameLabel.Text = _outfit.ItemName;

        OutfitItems = new ObservableCollection<DisplayItem>();
        ItemsCollectionView.ItemsSource = OutfitItems;

        LoadItemsForOutfit();
    }

    private void LoadItemsForOutfit()
    {
        try
        {
            Connection.Connection c = new Connection.Connection();

            // Join the tables to find items linked to this specific OutfitID
            string sql = $"SELECT i.* FROM ClothingItem i JOIN OutfitItems oi ON i.itemID = oi.itemID WHERE oi.outfitID = '{_outfit.ItemID}'";
            DataTable dt = c.GetData(sql);

            OutfitItems.Clear();
            foreach (DataRow row in dt.Rows)
            {
                OutfitItems.Add(new DisplayItem
                {
                    ItemID = row["itemID"].ToString(),
                    ItemName = row["itemName"].ToString(),
                    PhotoUrl = ImageSource.FromFile(row["PhotoUrl"].ToString())
                });
            }
            c.closeConnection();
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", "Could not load outfit items: " + ex.Message, "OK");
        }
    }

    private async void OnDeleteOutfitClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete?", "Delete this outfit? The items will stay in your closet.", "Yes", "No");
        if (!confirm) return;

        try
        {
            Connection.Connection c = new Connection.Connection();
            // Delete links
            c.query($"DELETE FROM OutfitItems WHERE outfitID = '{_outfit.ItemID}'");
            // Delete outfit header
            c.query($"DELETE FROM Outfits WHERE outfitID = '{_outfit.ItemID}'");
            c.closeConnection();

            await DisplayAlert("Deleted", "Outfit removed.", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
}