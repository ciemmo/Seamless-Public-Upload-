using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Seamless; 

namespace ClothesTagger.ViewModels 
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _selectedImagePath;
        private string _base64Image;
        private string _pendingTagsDisplay = "None";

        public Command PickImageCommand { get; }
        public Command SaveItemCommand { get; }

        //Data for the UI
        public ObservableCollection<SelectableTag> AvailableTags { get; set; }
        public ObservableCollection<DisplayItem> Clothes { get; set; } 

        public MainViewModel()
        {
            Clothes = new ObservableCollection<DisplayItem>();

            // Setup the checkboxes
            AvailableTags = new ObservableCollection<SelectableTag>
            {
                new SelectableTag("Casual", OnTagChanged),
                new SelectableTag("Formal", OnTagChanged),
                new SelectableTag("Summer", OnTagChanged),
                new SelectableTag("Winter", OnTagChanged),
                new SelectableTag("Work", OnTagChanged),
                new SelectableTag("Party", OnTagChanged)
            };

            PickImageCommand = new Command(OnPickImage);
            SaveItemCommand = new Command(OnSaveItem);

            // Load data from Backend immediately
            LoadCloset();
        }

        
        // PROPERTIES
       
        public string SelectedImagePath
        {
            get => _selectedImagePath;
            set { _selectedImagePath = value; OnPropertyChanged(); }
        }

        public string PendingTagsDisplay
        {
            get => _pendingTagsDisplay;
            set { _pendingTagsDisplay = value; OnPropertyChanged(); }
        }

       
        // LOGIC: Pick Image
       
        private async void OnPickImage()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync();
                if (result != null)
                {
                    SelectedImagePath = result.FullPath;

                    using var stream = await result.OpenReadAsync();
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    _base64Image = Convert.ToBase64String(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pick Error: {ex.Message}");
            }
        }

       
        // LOGIC: Update "Selected Tags" Label
        
        private void OnTagChanged()
        {
            var selected = AvailableTags.Where(t => t.IsSelected).Select(t => t.Name);
            PendingTagsDisplay = selected.Any() ? string.Join(", ", selected) : "None";
        }

        
        // LOGIC: Save to Backend
      
        private async void OnSaveItem()
        {
            if (string.IsNullOrEmpty(_base64Image))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Pick an image first", "OK");
                return;
            }

            var tags = AvailableTags.Where(t => t.IsSelected).Select(t => t.Name).ToList();

            // Call the Kitchen
            var service = new ApiService();
            bool success = await service.SaveClothingItem(_base64Image, tags);

            if (success)
            {
                await Application.Current.MainPage.DisplayAlert("Success", "Saved to Closet!", "OK");
                LoadCloset(); // Refresh list

                // Reset UI
                SelectedImagePath = null;
                _base64Image = null;
                foreach (var t in AvailableTags) t.IsSelected = false;
                PendingTagsDisplay = "None";
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Backend connection failed", "OK");
            }
        }

        
        // LOGIC: Load from Backend
        
        private async void LoadCloset()
        {
            var service = new ApiService();
            var items = await service.GetCloset();

            Clothes.Clear();
            foreach (var item in items)
            {
                var bytes = Convert.FromBase64String(item.ImageData);
                Clothes.Add(new DisplayItem
                {
                    ImagePath = ImageSource.FromStream(() => new MemoryStream(bytes)),
                    Tags = item.Tags
                });
            }
        }

        
        // HELPERS 
        
        public class SelectableTag : INotifyPropertyChanged
        {
            private bool _isSelected;
            private Action _onChanged;

            public string Name { get; set; }
            public bool IsSelected
            {
                get => _isSelected;
                set
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                    _onChanged?.Invoke(); // Trigger the label update
                }
            }

            public SelectableTag(string name, Action onChanged)
            {
                Name = name;
                _onChanged = onChanged;
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public class DisplayItem
        {
            public ImageSource ImagePath { get; set; }
            public List<string> Tags { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}