using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Data;
using Seamless.Models;
using Connection;
using System.Linq; 

namespace Seamless
{
    public class ClosetViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // --- PROPERTIES ---
        private string _selectedImagePath;
        private string _pendingTagsDisplay = "None";
        private bool _isBusy;
        private List<DisplayItem> _allItemsBackup = new List<DisplayItem>();

        public Command PickImageCommand { get; }
        public Command SaveItemCommand { get; }
        public Command RefreshClosetCommand { get; }
        public Command LoadOutfitsCommand { get; }

        public ObservableCollection<SelectableTag> TypeTags { get; set; }
        public ObservableCollection<SelectableTag> ColorTags { get; set; }
        public ObservableCollection<SelectableTag> OccasionTags { get; set; }
        public ObservableCollection<SelectableTag> SeasonTags { get; set; }
        public ObservableCollection<SelectableTag> MaterialTags { get; set; }

        public ObservableCollection<DisplayItem> MyCloset { get; set; }

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

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ClosetViewModel()
        {
            MyCloset = new ObservableCollection<DisplayItem>();

            TypeTags = CreateTags("T-Shirt", "Shirt", "Blouse", "Sweater", "Hoodie",
                "Jacket", "Coat", "Blazer",
                "Pants", "Jeans", "Shorts", "Skirt", "Leggings",
                "Dress", "Jumpsuit", "Activewear", "Swimwear",
                "Shoes", "Sneakers", "Boots", "Heels", "Sandals",
                "Bag", "Jewelry", "Hat", "Accessories");


            ColorTags = CreateTags("Black", "White", "Grey", "Beige", "Brown", "Cream",
                "Blue", "Navy", "Red", "Maroon",
                "Green", "Olive", "Yellow", "Orange",
                "Pink", "Purple", "Gold", "Silver", "Multi-Color");


            OccasionTags = CreateTags("Casual", "Everyday", "Work", "Business Casual",
                "Formal", "Date Night", "Party", "Wedding",
                "Athletic", "Gym", "Loungewear", "Comfort",
                "Vacation", "Beach", "Festival");


            SeasonTags = CreateTags("Summer", "Winter", "Spring", "Fall", "All-Season");


            MaterialTags = CreateTags("Cotton", "Denim", "Wool", "Leather", "Faux Leather",
                "Polyester", "Spandex", "Silk", "Satin",
                "Linen", "Velvet", "Suede", "Knitted", "Fleece");

            PickImageCommand = new Command(async () => await OnPickImage());
            SaveItemCommand = new Command(async () => await OnSaveItem());
            RefreshClosetCommand = new Command(async () => await LoadCloset());
            LoadOutfitsCommand = new Command(async () => await LoadOutfits());
        }

        // --- LOAD ITEMS ---
        public async Task LoadCloset()
        {
            if (IsBusy) return; IsBusy = true;
            try
            {
                var items = await Task.Run(() =>
                {
                    var list = new List<DisplayItem>();
                    Connection.Connection c = new Connection.Connection();
                    DataTable dt = c.GetData("SELECT * FROM ClothingItem");

                    foreach (DataRow row in dt.Rows)
                    {
                        string id = row["itemID"].ToString();
                        string path = row["PhotoUrl"].ToString();
                        string name = row["itemName"].ToString();

                        // IsOutfit = false because these are single items
                        list.Add(new DisplayItem { ItemID = id, PhotoUrl = ImageSource.FromFile(path), ItemName = name, IsOutfit = false });
                    }
                    c.closeConnection();
                    return list;
                });

                MyCloset.Clear();
                _allItemsBackup.Clear();
                foreach (var item in items) { MyCloset.Add(item); _allItemsBackup.Add(item); }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Load Error: " + ex.Message); }
            finally { IsBusy = false; }
        }

        // --- LOAD OUTFITS ---
        public async Task LoadOutfits()
        {
            if (IsBusy) return; IsBusy = true;
            try
            {
                var outfits = await Task.Run(() =>
                {
                    var list = new List<DisplayItem>();
                    Connection.Connection c = new Connection.Connection();

                    DataTable dt = c.GetData("SELECT * FROM Outfits");

                    foreach (DataRow row in dt.Rows)
                    {
                        string id = row["outfitID"].ToString();
                        string name = row["outfitName"].ToString();

                        // Find a 'Cover Image'
                        DataTable imgTable = c.GetData($"SELECT i.PhotoUrl FROM ClothingItem i JOIN OutfitItems oi ON i.itemID = oi.itemID WHERE oi.outfitID = '{id}' LIMIT 1");

                        ImageSource img = "dotnet_bot.png";
                        if (imgTable.Rows.Count > 0)
                        {
                            string path = imgTable.Rows[0]["PhotoUrl"].ToString();
                            img = ImageSource.FromFile(path);
                        }

                        // IsOutfit = true because these are collections
                        list.Add(new DisplayItem { ItemID = id, PhotoUrl = img, ItemName = name, IsOutfit = true });
                    }
                    c.closeConnection();
                    return list;
                });

                MyCloset.Clear();
                foreach (var o in outfits) MyCloset.Add(o);
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Load Outfit Error: " + ex.Message); }
            finally { IsBusy = false; }
        }

        // --- SORT & FILTER ---
        public void SortItems(string sortType)
        {
            if (MyCloset.Count == 0) return;
            var sortedList = sortType == "Alphabetical"
                ? MyCloset.OrderBy(x => x.ItemName).ToList()
                : MyCloset.OrderByDescending(x => x.ItemID).ToList();
            MyCloset.Clear();
            foreach (var item in sortedList) MyCloset.Add(item);
        }

        public void FilterItems(string tag)
        {
            MyCloset.Clear();
            if (tag == "Show All") { foreach (var item in _allItemsBackup) MyCloset.Add(item); return; }
            foreach (var item in _allItemsBackup) { if (item.ItemName.Contains(tag, StringComparison.OrdinalIgnoreCase)) MyCloset.Add(item); }
        }

        // --- IMAGE & SAVE ---
        private async Task OnPickImage() { try { var r = await MediaPicker.Default.PickPhotoAsync(); if (r != null) SelectedImagePath = r.FullPath; } catch { } }

        private async Task OnSaveItem()
        {
            if (string.IsNullOrEmpty(SelectedImagePath)) { await Application.Current.MainPage.DisplayAlert("Error", "Pick Image", "OK"); return; }
            if (IsBusy) return; IsBusy = true;
            try
            {
                await Task.Run(() => {
                    Connection.Connection c = new Connection.Connection();
                    Random rnd = new Random();
                    int itemID = rnd.Next(1, 100000);
                    c.query("CREATE TABLE IF NOT EXISTS ClothingItem (itemID INT PRIMARY KEY, PhotoUrl TEXT, itemName TEXT);");
                    c.query("CREATE TABLE IF NOT EXISTS ItemTags (tagID INT PRIMARY KEY, itemID INT, tagValue TEXT);");
                    string cleanPath = SelectedImagePath.Replace("\\", "\\\\");
                    string defaultName = TypeTags.Concat(ColorTags).Concat(OccasionTags).FirstOrDefault(t => t.IsSelected)?.Name ?? "Item";
                    c.query($"INSERT INTO ClothingItem (itemID, PhotoUrl, itemName) VALUES ('{itemID}', '{cleanPath}', '{defaultName}')");

                    var allTags = TypeTags.Concat(ColorTags).Concat(OccasionTags).Concat(SeasonTags).Concat(MaterialTags);
                    var selectedTags = allTags.Where(t => t.IsSelected).Select(t => t.Name).ToList();
                    foreach (var t in selectedTags)
                    {
                        int tagID = rnd.Next(1, 100000);
                        c.query($"INSERT INTO ItemTags (tagID, itemID, tagValue) VALUES ('{tagID}', '{itemID}', '{t}')");
                    }
                    c.closeConnection();
                });
                await Application.Current.MainPage.DisplayAlert("Success", "Saved!", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            catch (Exception ex) { await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"); }
            finally { IsBusy = false; }
        }

        private ObservableCollection<SelectableTag> CreateTags(params string[] names)
        {
            var list = new ObservableCollection<SelectableTag>();
            foreach (var name in names) list.Add(new SelectableTag(name, UpdatePendingTags));
            return list;
        }
        private void UpdatePendingTags()
        {
            var allTags = TypeTags.Concat(ColorTags).Concat(OccasionTags).Concat(SeasonTags).Concat(MaterialTags);
            var selected = allTags.Where(t => t.IsSelected).Select(t => t.Name);
            PendingTagsDisplay = selected.Any() ? string.Join(", ", selected) : "None";
        }
    }

    public class SelectableTag : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private bool _isSelected;
        private Action _onChanged;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); _onChanged?.Invoke(); } }
        public SelectableTag(string name, Action onChanged) { Name = name; _onChanged = onChanged; }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string n = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    // --- UPDATED CLASS ---
    public class DisplayItem
    {
        public string ItemID { get; set; }
        public ImageSource PhotoUrl { get; set; }
        public string ItemName { get; set; }
        public bool IsOutfit { get; set; } // New Flag
    }
}