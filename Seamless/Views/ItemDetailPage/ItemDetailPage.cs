using Seamless.Models;
using Connection;

namespace Seamless;

public partial class ItemDetailPage : ContentPage
{
    private DisplayItem _item;

    // Constructor accepts the DisplayItem passed from ClosetPage
    public ItemDetailPage(DisplayItem item)
    {
        InitializeComponent();
        _item = item;
        BindingContext = _item; //  fills in the Image and Text
    }

    private async void OnEditItemClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Coming Soon", "Editing coming soon!", "OK");
    }

    // --- DELETE LOGIC ---
    private async void OnDeleteItemClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Delete Item", "Are you sure you want to delete this?", "Yes", "No");
        if (!confirm) return;

        try
        {
            Connection.Connection c = new Connection.Connection();

            // 1. Delete the Tags for this item
            c.query($"DELETE FROM ItemTags WHERE itemID = '{_item.ItemID}'");

            // 2. Delete the Item itself
            c.query($"DELETE FROM ClothingItem WHERE itemID = '{_item.ItemID}'");

            c.closeConnection();

            await DisplayAlert("Deleted", "Item removed from closet.", "OK");

            // Go back to closet
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Could not delete: " + ex.Message, "OK");
        }
    }
}