using Seamless.Models;
using Seamless.Models.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using Connection;

namespace Seamless;

// Wrapper Class to handle the checkbox selection
public class SelectableClothingItem : INotifyPropertyChanged
{
    public ClothingItem Item { get; set; }

    private bool _isSelected;
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}

public partial class AddOrEditOutfitPage : ContentPage
{
    private ObservableCollection<SelectableClothingItem> _selectedItems;

    public AddOrEditOutfitPage()
    {
        InitializeComponent();
        _selectedItems = new ObservableCollection<SelectableClothingItem>();
        ItemsCollectionView.ItemsSource = _selectedItems;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadClosetItemsAsync();
    }

    // Load items so we can select them
    private async Task LoadClosetItemsAsync()
    {
        _selectedItems.Clear();

        await Task.Run(() =>
        {
            try
            {
                Connection.Connection c = new Connection.Connection();
                DataTable dt = c.GetData("SELECT * FROM ClothingItem");
                var realItems = new List<SelectableClothingItem>();

                foreach (DataRow row in dt.Rows)
                {
                    string id = row["itemID"].ToString();
                    string path = row["PhotoUrl"].ToString();
                    string name = row["itemName"].ToString();

                    var item = new ClothingItem
                    {
                        ItemId = id,
                        ItemName = name,
                        PhotoUrl = path,
                        ItemType = ClothingType.Undefined
                    };
                    realItems.Add(new SelectableClothingItem { Item = item, IsSelected = false });
                }
                c.closeConnection();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    foreach (var i in realItems) _selectedItems.Add(i);
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading items: " + ex.Message);
            }
        });
    }

    private void OnItemTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject bindable && bindable.BindingContext is SelectableClothingItem tappedItem)
        {
            tappedItem.IsSelected = !tappedItem.IsSelected;
        }
    }

    // --- UPDATED: SAVE TO DATABASE ---
    private async void OnSaveClicked(object sender, EventArgs e)
    {
        string outfitName = OutfitNameEntry.Text;

        var selectedItems = _selectedItems.Where(item => item.IsSelected).ToList();

        if (selectedItems.Count == 0)
        {
            await DisplayAlert("Error", "Please select at least one item.", "OK");
            return;
        }
        if (string.IsNullOrEmpty(outfitName))
        {
            await DisplayAlert("Error", "Please name your outfit.", "OK");
            return;
        }

        await Task.Run(() =>
        {
            try
            {
                Connection.Connection c = new Connection.Connection();
                Random rnd = new Random();
                int outfitID = rnd.Next(1, 100000);

                // 1. Create Tables if they don't exist
                c.query("CREATE TABLE IF NOT EXISTS Outfits (outfitID INT PRIMARY KEY, outfitName TEXT);");
                c.query("CREATE TABLE IF NOT EXISTS OutfitItems (id INT PRIMARY KEY, outfitID INT, itemID INT);");

                // 2. Insert the Outfit
                c.query($"INSERT INTO Outfits (outfitID, outfitName) VALUES ('{outfitID}', '{outfitName}')");

                // 3. Insert the Links (Items in this outfit)
                foreach (var sItem in selectedItems)
                {
                    int linkID = rnd.Next(1, 1000000);
                    c.query($"INSERT INTO OutfitItems (id, outfitID, itemID) VALUES ('{linkID}', '{outfitID}', '{sItem.Item.ItemId}')");
                }
                c.closeConnection();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Save Outfit Error: " + ex.Message);
            }
        });

        await DisplayAlert("Success", $"Outfit '{outfitName}' Saved!", "OK");
        await Navigation.PopAsync();
    }
}