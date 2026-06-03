/// By Zachary Davis
/// <summary>
/// Represents the trash page where deleted clothing items are displayed.
/// </summary>
/// 
using Seamless.Models;
using System.Collections.Generic;

namespace Seamless;

public partial class TrashPage : ContentPage
{
	public TrashPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadTesterData();
    }

    // This just creates testing data
    private void LoadTesterData()
    {
        var testData = new List<ClothingItem>
        {
            new ClothingItem { ItemId = "t1", ItemName = "Old Scarf", IsTrashed = true },
            new ClothingItem { ItemId = "t2", ItemName = "Stained Shirt", IsTrashed = true },
            new ClothingItem { ItemId = "t3", ItemName = "Single Sock", IsTrashed = true }
        };

        TrashCollectionView.ItemsSource = testData;
    }
}