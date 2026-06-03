using System.Collections.ObjectModel;
using System.ComponentModel; 

namespace Seamless
{
    // A simple class to hold post data
    public class UserPost
    {
        public string PostImageUrl { get; set; }
    }


    //  UI update automatically when data changes.
    public partial class UserProfilePage : ContentPage, INotifyPropertyChanged
    {
        // --- Properties for Data Binding ---
        private string _displayName;
        public string DisplayName
        {
            get => _displayName;
            set
            {
                _displayName = value;
                OnPropertyChanged(nameof(DisplayName)); // Notify UI of change
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username)); // Notify UI of change
            }
        }

        public ObservableCollection<UserPost> UserPosts { get; set; }

        // --- Page Constructor ---
        public UserProfilePage()
        {
            InitializeComponent();

            // Load user data (replace with real data)
            DisplayName = "[UserName]";
            Username = "your_username";

            // Load sample data for the post grid
            UserPosts = new ObservableCollection<UserPost>
            {
                new UserPost { PostImageUrl = "https://placehold.co/300x300/F7971E/white?text=Post+1" },
                new UserPost { PostImageUrl = "https://placehold.co/300x300/333/white?text=Post+2" },
                new UserPost { PostImageUrl = "https://placehold.co/300x300/F7971E/white?text=Post+3" },
                new UserPost { PostImageUrl = "https://placehold.co/300x300/333/white?text=Post+4" },
                new UserPost { PostImageUrl = "https://placehold.co/300x300/F7971E/white?text=Post+5" },
                new UserPost { PostImageUrl = "https://placehold.co/300x300/333/white?text=Post+6" },
            };

            // Set the BindingContext so the XAML can find properties
            this.BindingContext = this;
        }

        // --- Button Click Handlers ---

        private void OnEditProfileClicked(object sender, EventArgs e)
        {
            // Show the "Edit" layout and hide the "View" layout
            ViewProfileLayout.IsVisible = false;
            EditProfileLayout.IsVisible = true;
        }

        private async void OnSaveProfileClicked(object sender, EventArgs e)
        {
         
            await DisplayAlert("Saved", "Your profile has been updated.", "OK");

            // Show the "View" layout and hide the "Edit" layout
            EditProfileLayout.IsVisible = false;
            ViewProfileLayout.IsVisible = true;
        }

        private void OnCancelEditClicked(object sender, EventArgs e)

        {


            // Show the "View" layout and hide the "Edit" layout
            EditProfileLayout.IsVisible = false;
            ViewProfileLayout.IsVisible = true;
        }

        private async void onChangeProfilePictureClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Not Implemented", "Change picture logic goes here.", "OK");
        }


        // --- Property Changed Event Handler ---
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}