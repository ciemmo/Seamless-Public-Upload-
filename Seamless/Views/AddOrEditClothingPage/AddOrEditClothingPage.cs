/// By Zachary Davis
/// <summary>
/// Page for adding or editing a clothing item.
/// </summary>
/// 
using Seamless.Models;
using Seamless.Models.Enums;
using Microsoft.Maui.Media;

namespace Seamless;

public partial class AddOrEditClothingPage : ContentPage
{
    private ClothingItem _currentItem;
    private bool _isEditMode;
    private FileResult _selectedPhoto;

    // Constructor for ADDING a new item
    public AddOrEditClothingPage()
    {
        InitializeComponent();
        _isEditMode = false;
        _currentItem = new ClothingItem();
        Title = "Add New Item";
    }

    // Constructor for EDITING an existing item
    public AddOrEditClothingPage(ClothingItem item)
    {
        InitializeComponent();
        _isEditMode = true;
        _currentItem = item;
        Title = "Edit Item";
        
        PopulateForm();
    }


    // Method to pre-fill the form
    private void PopulateForm()
    {
        // Pre-fill fields from the existing item
        ItemNameEntry.Text = _currentItem.ItemName;
        NotesEditor.Text = _currentItem.Notes;
        
        if (!string.IsNullOrEmpty(_currentItem.PhotoUrl))
        {
            ItemImage.Source = ImageSource.FromUri(new Uri(_currentItem.PhotoUrl));
        }

        // TODO: Pre-select the RadioButton for ClothingType

        // TODO: Pre-select Weather and Occasion RadioButtons

    }


    private async void OnUploadPhotoClicked(object sender, EventArgs e)
    {
        // Logic copied from UploadPage to pick a photo
        try
        {
            _selectedPhoto = await MediaPicker.PickPhotoAsync();

            if (_selectedPhoto == null)
                return; // User cancelled

            // Show a preview
            var stream = await _selectedPhoto.OpenReadAsync();
            ItemImage.Source = ImageSource.FromStream(() => stream);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to pick photo: {ex.Message}", "OK");
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // TODO: Read all values from the form
        _currentItem.ItemName = ItemNameEntry.Text;
        _currentItem.Notes = NotesEditor.Text;

        // TODO: Get selected RadioButton values

        // TODO: Save the _currentItem to the database/API

        // TODO: Add Weather/Occasion to the model or save them as Tags
        
        // Backend Integration Point

        // recieve currentItem data as json object
        // recieve photoStream
        // Upload to database


        Stream photoStream = null;
        string photFileName = null;

        if (_selectedPhoto != null)
        {
            photoStream = await _selectedPhoto.OpenReadAsync();
            photFileName = _selectedPhoto.FileName;
        }

        if (_isEditMode)
        {
            await DisplayAlert("Save", "Item updated! (Logic not implemented)", "OK");
        }
        else
        {
            await DisplayAlert("Save", "New item saved! (Logic not implemented)", "OK");
        }

        // Go back to the previous page (Closet)
        await Navigation.PopAsync();
    }
}