
using Seamless.Models;

namespace Seamless;

// This attribute tells the page to expect a "Post" object
[QueryProperty("Post","Post")]
public partial class ImageDetailPage : ContentPage
{
    private ImagePost _post;
    public ImagePost Post
    {
        get => _post;
        set
        {
            _post = value;
            OnPostSet(); // Call this method when the data arrives
        }
    }

    public ImageDetailPage()
    {
        InitializeComponent();
    }

    // This runs after the "Post" data has been received
    void OnPostSet()
    {
        if (Post != null)
        {
            // Set the UI elements with the post's data
            MainImage.Source = Post.ImageUrl;
            DescriptionLabel.Text = Post.Description;
            // You can also set the user info here if you want
        }
    }

    // A simple placeholder for your "Like" button
    private void OnLikeClicked(object sender, EventArgs e)
    {
        LikeButton.Text = "❤️ Liked!";
        // In a real app, you would send this "like" to your API
    }
}